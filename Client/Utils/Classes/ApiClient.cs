using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Interfaces;
using Microsoft.Extensions.Logging;

namespace Client.Utils.Classes;

public class ApiClient(HttpClient httpClient, ILogger<ApiClient> logger) : IApiClient
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<ApiResponse<T>> GetAllAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        if (AppConfig.IsDemoMode) return HandleDemoMode<T>(endpoint);

        try
        {
            logger.LogDebug("GET request to {Endpoint}", httpClient.BaseAddress + endpoint);
            
            var response = await httpClient.GetAsync(endpoint, cancellationToken);
            return await ProcessResponse<T>(response, cancellationToken);
        }
        catch (Exception e)
        {
            return HandleException<T>(e);
        }
    }

    public async Task<ApiResponse<T>> GetByIdAsync<T>(string endpoint, int id, CancellationToken cancellationToken = default)
    {
        if (AppConfig.IsDemoMode) return HandleDemoMode<T>(endpoint);

        var fullEndpoint = $"{endpoint}/{id}";
        try
        {
            logger.LogDebug("GET request to {Endpoint}", httpClient.BaseAddress + fullEndpoint);

            var response = await httpClient.GetAsync(fullEndpoint, cancellationToken);
            return await ProcessResponse<T>(response, cancellationToken);
        }
        catch (Exception e)
        {
            return HandleException<T>(e);
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        if (AppConfig.IsDemoMode) return HandleDemoMode<T>(endpoint);

        try
        {
            logger.LogDebug("POST request to {Endpoint}", endpoint);

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(endpoint, content, cancellationToken);
            return await ProcessResponse<T>(response, cancellationToken);
        }
        catch (Exception e)
        {
            return HandleException<T>(e);
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, int id, object data, CancellationToken cancellationToken = default)
    {
        if (AppConfig.IsDemoMode) return HandleDemoMode<T>(endpoint);

        try
        {
            logger.LogDebug("PUT request to {Endpoint}/{Id}", endpoint, id);

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{endpoint}/{id}", content, cancellationToken);
            return await ProcessResponse<T>(response, cancellationToken);
        }
        catch (Exception e)
        {
            return HandleException<T>(e);
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        if (AppConfig.IsDemoMode) return HandleDemoMode<T>(endpoint);

        try
        {
            logger.LogDebug("PUT request to {Endpoint}", endpoint);

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(endpoint, content, cancellationToken);
            return await ProcessResponse<T>(response, cancellationToken);
        }
        catch (Exception e)
        {
            return HandleException<T>(e);
        }
    }

    public async Task<ApiResponse<object?>> DeleteAsync<T>(string endpoint, int id, CancellationToken cancellationToken = default)
    {
        if (AppConfig.IsDemoMode) return new ApiResponse<object?> { IsSuccess = true, StatusCode = 200 };

        try
        {
            logger.LogDebug("DELETE request to {Endpoint}/{Id}", endpoint, id);

            var response = await httpClient.DeleteAsync($"{endpoint}/{id}", cancellationToken);

            return new ApiResponse<object?>
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = null,
                ErrorMessage = response.IsSuccessStatusCode ? string.Empty : $"HTTP {response.StatusCode}"
            };
        }
        catch (Exception e)
        {
            return new ApiResponse<object?>
            {
                IsSuccess = false,
                ErrorMessage = e.Message,
                StatusCode = 0
            };
        }
    }
    
    public async Task<ApiResponse<object?>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        if (AppConfig.IsDemoMode) return new ApiResponse<object?> { IsSuccess = true, StatusCode = 200 };

        try
        {
            logger.LogDebug("DELETE request to {Endpoint}", endpoint);

            var response = await httpClient.DeleteAsync(endpoint, cancellationToken);

            return new ApiResponse<object?>
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = null,
                ErrorMessage = response.IsSuccessStatusCode ? string.Empty : $"HTTP {response.StatusCode}"
            };
        }
        catch (Exception e)
        {
            return new ApiResponse<object?>
            {
                IsSuccess = false,
                ErrorMessage = e.Message,
                StatusCode = 0
            };
        }
    }
    
    private ApiResponse<T> HandleException<T>(Exception e)
    {
        logger.LogError(e, "API request failed with exception.");

        var errorMessage = e switch
        {
            HttpRequestException => "Network connection failed",
            TaskCanceledException when e.InnerException is TimeoutException => "Request timed out.",
            TaskCanceledException => "Request was cancelled.",
            _ => $"Unexpected error: {e.Message}"
        };

        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = e is TaskCanceledException ? 408 : 0
        };
    }

    private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var apiResponse = new ApiResponse<T>
        {
            StatusCode = (int)response.StatusCode,
            IsSuccess = response.IsSuccessStatusCode,
        };

        foreach (var header in response.Headers)
        {
            apiResponse.Headers[header.Key] = string.Join(", ", header.Value);
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    apiResponse.Data = (T)(object)content;
                }
                else if (!string.IsNullOrWhiteSpace(content))
                {
                    apiResponse.Data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                }
            }
            catch (JsonException e)
            {
                logger.LogError(e, "Failed to deserialize response from {StatusCode}.", response.StatusCode);
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessage = "Invalid response format.";
            }
        }
        else
        {
            apiResponse.ErrorMessage = $"HTTP {response.StatusCode}: {content}";
            logger.LogWarning("API request failed with status {StatusCode}: {Content}", response.StatusCode, content);
        }

        return apiResponse;
    }

    private ApiResponse<T> HandleDemoMode<T>(string endpoint)
    {
        try
        {
            // For fetching lists of employees
            if (typeof(T) == typeof(System.Collections.Generic.IEnumerable<Shared.EmployeeManagement.Responses.EmployeeResponse>))
            {
                var demoEmployees = new System.Collections.Generic.List<Shared.EmployeeManagement.Responses.EmployeeResponse>
                {
                    new() { Id = 1, BasicInfo = new() { FirstName = "John", LastName = "Doe", PreferredName = "Johnny", DateOfBirth = default }, ContactInfo = new() { PersonalEmail = "john.doe@demo.com" }, PositionDetails = new() { PositionName = "Software Engineer", Department = "Engineering" } },
                    new() { Id = 2, BasicInfo = new() { FirstName = "Jane", LastName = "Smith", DateOfBirth = default }, ContactInfo = new() { PersonalEmail = "jane.smith@demo.com" }, PositionDetails = new() { PositionName = "HR Manager", Department = "Human Resources" } },
                    new() { Id = 3, BasicInfo = new() { FirstName = "Alice", LastName = "Johnson", DateOfBirth = default }, ContactInfo = new() { PersonalEmail = "alice.j@demo.com" }, PositionDetails = new() { PositionName = "Sales Rep", Department = "Sales" } }
                };
                return new ApiResponse<T> { IsSuccess = true, StatusCode = 200, Data = (T)(object)demoEmployees };
            }
            
            // For fetching a single employee
            if (typeof(T) == typeof(Shared.EmployeeManagement.Responses.EmployeeResponse))
            {
                var employee = new Shared.EmployeeManagement.Responses.EmployeeResponse
                {
                    Id = 1, 
                    BasicInfo = new() { FirstName = "John", LastName = "Doe", DateOfBirth = default }, 
                    ContactInfo = new() { PersonalEmail = "john.doe@demo.com" }, 
                    PositionDetails = new() { PositionName = "Demo Employee", Department = "Demo" }
                };
                return new ApiResponse<T> { IsSuccess = true, StatusCode = 200, Data = (T)(object)employee };
            }

            // Fallback for returning empty lists for other endpoints
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
            {
                if (typeof(T).IsGenericType)
                {
                    var argType = typeof(T).GetGenericArguments()[0];
                    var emptyListType = typeof(System.Collections.Generic.List<>).MakeGenericType(argType);
                    var emptyList = Activator.CreateInstance(emptyListType);
                    return new ApiResponse<T> { IsSuccess = true, StatusCode = 200, Data = (T)emptyList! };
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating demo data");
        }

        return new ApiResponse<T> { IsSuccess = true, StatusCode = 200, Data = default };
    }
}
