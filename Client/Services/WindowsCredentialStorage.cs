using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Client.Utils.Exceptions.Auth;
using Microsoft.Extensions.Logging;

namespace Client.Services;

public class WindowsCredentialStorage(ILogger<WindowsCredentialStorage> logger, string applicationName)
    : ISecureTokenStorage
{
    private readonly ILogger<WindowsCredentialStorage> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly string _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));

    public async Task StoreTokenAsync(string key, string token)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        await Task.Run(() => StoreTokenSync(key, token));
    }

    public async Task<string?> RetrieveTokenAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        return await Task.Run(() => RetrieveTokenSync(key));
    }

    public async Task DeleteTokenAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        await Task.Run(() => DeleteTokenSync(key));
    }

    private void StoreTokenSync(string key, string token)
    {
        var targetName = GetTargetName(key);
        var tokenBytes = Encoding.UTF8.GetBytes(token);

        var credential = new CREDENTIAL
        {
            Type = CredType.GENERIC,
            TargetName = targetName,
            CredentialBlob = Marshal.AllocHGlobal(tokenBytes.Length),
            CredentialBlobSize = (uint)tokenBytes.Length,
            Persist = CredPersist.LOCAL_MACHINE,
            UserName = Environment.UserName,
            Comment = $"Token stored by {_applicationName}"
        };

        try
        {
            Marshal.Copy(tokenBytes, 0, credential.CredentialBlob, tokenBytes.Length);

            if (!CredWrite(ref credential, 0))
            {
                var error = Marshal.GetLastWin32Error();
                _logger.LogError("Failed store credential for key {Key}. Win32 Error: {Error}", key, error);

                throw new TokenStorageException(
                    $"Failed to store credential: {GetErrorMessage(error)}",
                    operation: "Store",
                    keyName: key,
                    innerException: new Win32Exception(error));
            }

            _logger.LogDebug("Successfully stored credential for key {Key}", key);
        }
        catch (Exception e) when (e is not TokenStorageException)
        {
            _logger.LogError(e, "Unexpected error storing credential for key {Key}", key);
            throw new TokenStorageException(
                "Unexpected error occurred while storing credential",
                operation: "Store",
                keyName: key,
                innerException: e);
        }
        finally
        {
            if (credential.CredentialBlob != IntPtr.Zero)
            {
                Marshal.Copy(new byte[tokenBytes.Length], 0, credential.CredentialBlob, tokenBytes.Length);
                Marshal.FreeHGlobal(credential.CredentialBlob);
            }
        }
    }

    private string? RetrieveTokenSync(string key)
    {
        var targetName = GetTargetName(key);
        IntPtr credentialPtr = IntPtr.Zero;

        try
        {
            if (!CredRead(targetName, CredType.GENERIC, 0, out credentialPtr))
            {
                var error = Marshal.GetLastWin32Error();

                if (error == ERROR_NOT_FOUND)
                {
                    _logger.LogDebug("Credential not found for key {Key}", key);
                    return null;
                }

                _logger.LogError("Failed to retrieve credential for key {Key}. Win32 Error: {Error}", key, error);

                throw new TokenStorageException(
                    $"Failed to retrieve credential: {GetErrorMessage(error)}",
                    operation: "Retrieve",
                    keyName: key,
                    innerException: new Win32Exception(error));
            }

            var credential = Marshal.PtrToStructure<CREDENTIAL>(credentialPtr);
            if (credential.CredentialBlob == IntPtr.Zero || credential.CredentialBlobSize == 0)
            {
                _logger.LogWarning("Retrieved credential for key {Key} has no data.", key);
                return null;
            }

            var tokenBytes = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, tokenBytes, 0, (int)credential.CredentialBlobSize);

            var token = Encoding.UTF8.GetString(tokenBytes);
            _logger.LogDebug("Successfully retrieved credential for key {Key}", key);

            return token;
        }
        catch (Exception e) when (e is not TokenStorageException)
        {
            _logger.LogError(e, "Unexpected error retrieving credential for {Key}", key);
            throw new TokenStorageException(
                "Unexpected error occurred while retrieving credential.",
                operation: "Retrieve",
                keyName: key,
                innerException: e);
        }
        finally
        {
            if (credentialPtr != IntPtr.Zero)
            {
                CredFree(credentialPtr);
            }
        }
    }

    private void DeleteTokenSync(string key)
    {
        var targetName = GetTargetName(key);

        try
        {
            if (!CredDelete(targetName, CredType.GENERIC, 0))
            {
                var error = Marshal.GetLastWin32Error();

                if (error == ERROR_NOT_FOUND)
                {
                    _logger.LogDebug("Credential not found for deletion, key {Key}.", key);
                    return;
                }
                
                _logger.LogError("Failed to delete credential for key {Key}. Win32 Error: {Error}", key, error);

                throw new TokenStorageException(
                    $"Failed to delete credential: {GetErrorMessage(error)}",
                    operation: "Delete",
                    keyName: key,
                    innerException: new Win32Exception(error));
            }
            
            _logger.LogDebug("Successfully deleted credential for key {Key}", key);
        }
        catch (Exception e) when (e is not TokenStorageException)
        {
            _logger.LogError(e, "Unexpected error deleting credential for key {Key}", key);
            throw new TokenStorageException(
                "Unexpected error occurred while deleting credential.",
                operation: "Delete",
                keyName: key,
                innerException: e);
        }
    }

    private string GetTargetName(string key) => $"{_applicationName}_{key}";

    private static string GetErrorMessage(int errorCode) => errorCode switch
    {
        ERROR_NOT_FOUND => "Credential not found.",
        ERROR_ACCESS_DENIED => "Access denied to credential store.",
        ERROR_INVALID_PARAMETER => "Invalid parameter provided.",
        ERROR_BAD_USERNAME => "Invalid username format.",
        _ => $"Win32 error: {errorCode}"
    };

    #region Win32 API Declarations

    private const int ERROR_NOT_FOUND = 1168;
    private const int ERROR_ACCESS_DENIED = 5;
    private const int ERROR_INVALID_PARAMETER = 87;
    private const int ERROR_BAD_USERNAME = 2202;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public uint Flags;
        public CredType Type;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;

        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public CredPersist Persist;
        public uint AttributeCount;
        public IntPtr Attributes;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetAlias;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string UserName;
    }

    private enum CredType : uint
    {
        GENERIC = 1,
        DOMAIN_PASSWORD = 2,
        DOMAIN_CERTIFICATE = 3,
        DOMAIN_VISIBLE_PASSWORD = 4,
        GENERIC_CERTIFICATE = 5,
        DOMAIN_EXTENDED = 6,
        MAXIMUM = 7,
        MAXIMUM_EX = (MAXIMUM + 1000)
    }
    
    private enum CredPersist : uint
    {
        SESSION = 1,
        LOCAL_MACHINE = 2,
        ENTERPRISE = 3
    }
    
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private extern static bool CredWrite([In] ref CREDENTIAL userCredential, [In] uint flags);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private extern static bool CredRead(string target, CredType type, int reservedFlag, out IntPtr credentialPtr);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private extern static bool CredDelete(string target, CredType type, int flags);

    [DllImport("advapi32.dll")]
    private extern static void CredFree([In] IntPtr cred);

    #endregion
}