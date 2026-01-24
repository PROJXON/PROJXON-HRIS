using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.TimeOffManagement.Requests;
using Shared.TimeOffManagement.Responses;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Intern Portal Time Off view
/// Allows interns to submit and track their time off requests
/// </summary>
public partial class TimeOffViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IApiClient _apiClient;

    public SidebarViewModel Sidebar { get; }

    #region My Time Off Requests

    [ObservableProperty]
    private ObservableCollection<TimeOffRequestViewModel> _myRequests = new();

    [ObservableProperty]
    private bool _hasNoRequests;

    [ObservableProperty]
    private string _requestCountText = "0 requests";

    #endregion

    #region Time Off Balance

    [ObservableProperty]
    private int _totalDaysAvailable = 10;

    [ObservableProperty]
    private int _daysUsed = 0;

    [ObservableProperty]
    private int _daysPending = 0;

    [ObservableProperty]
    private int _daysRemaining = 10;

    #endregion

    #region Submit Request Dialog

    [ObservableProperty]
    private bool _isSubmitDialogOpen;

    [ObservableProperty]
    private DateTimeOffset _requestStartDate = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset _requestEndDate = DateTimeOffset.Now;

    [ObservableProperty]
    private string _requestType = "Vacation";

    [ObservableProperty]
    private string _requestReason = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _calculatedDays = 1;

    [ObservableProperty]
    private ObservableCollection<string> _timeOffTypes = new()
    {
        "Vacation",
        "Sick Leave",
        "Personal Day",
        "Bereavement",
        "Other"
    };

    #endregion

    #region Cancel Request Dialog

    [ObservableProperty]
    private bool _isCancelDialogOpen;

    [ObservableProperty]
    private TimeOffRequestViewModel? _requestToCancel;

    #endregion

    public TimeOffViewModel(
        INavigationService navigationService,
        IApiClient apiClient,
        SidebarViewModel sidebarViewModel)
    {
        _navigationService = navigationService;
        _apiClient = apiClient;
        Sidebar = sidebarViewModel;
    }

    // Parameterless constructor for design-time support
    public TimeOffViewModel() : this(null!, null!, new SidebarViewModel())
    {
    }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Time Off";
        Sidebar.SetPortalMode(true); // Intern portal mode
        await LoadTimeOffDataAsync();
        await base.OnNavigatedToAsync();
    }

    public override async Task OnNavigatedFromAsync()
    {
        await Sidebar.OnNavigatedFromAsync();
        await base.OnNavigatedFromAsync();
    }

    private async Task LoadTimeOffDataAsync()
    {
        try
        {
            // Load intern's time off requests
            var requestsResult = await _apiClient.GetAllAsync<IEnumerable<TimeOffRequestResponse>>("api/timeoff/my-requests");
            if (requestsResult.IsSuccess && requestsResult.Data != null)
            {
                MyRequests.Clear();
                foreach (var request in requestsResult.Data.OrderByDescending(r => r.SubmittedDate))
                {
                    MyRequests.Add(new TimeOffRequestViewModel
                    {
                        Id = request.Id,
                        InternName = request.InternName,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        RequestType = request.RequestType,
                        Reason = request.Reason,
                        Status = request.Status,
                        SubmittedDate = request.SubmittedDate,
                        ReviewedDate = request.ReviewedDate,
                        ReviewedBy = request.ReviewedBy,
                        TotalDays = request.TotalDays
                    });
                }

                UpdateRequestCount();
            }

            // Load time off balance
            var balanceResult = await _apiClient.GetAllAsync<TimeOffBalanceResponse>("api/timeoff/balance");
            if (balanceResult.IsSuccess && balanceResult.Data != null)
            {
                TotalDaysAvailable = balanceResult.Data.TotalDaysAvailable;
                DaysUsed = balanceResult.Data.DaysUsed;
                DaysPending = balanceResult.Data.DaysPending;
                DaysRemaining = balanceResult.Data.DaysRemaining;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading time off data: {ex.Message}");
        }
    }

    private void UpdateRequestCount()
    {
        var count = MyRequests.Count;
        RequestCountText = count == 1 ? "1 request" : $"{count} requests";
        HasNoRequests = count == 0;
    }

    #region Submit Request Commands

    [RelayCommand]
    private void OpenSubmitDialog()
    {
        RequestStartDate = DateTimeOffset.Now;
        RequestEndDate = DateTimeOffset.Now;
        RequestType = "Vacation";
        RequestReason = string.Empty;
        CalculatedDays = 1;
        IsSubmitDialogOpen = true;
    }

    [RelayCommand]
    private void CloseSubmitDialog()
    {
        IsSubmitDialogOpen = false;
    }

    partial void OnRequestStartDateChanged(DateTimeOffset value)
    {
        // Ensure end date is not before start date
        if (RequestEndDate < value)
        {
            RequestEndDate = value;
        }
        CalculateDays();
    }

    partial void OnRequestEndDateChanged(DateTimeOffset value)
    {
        // Ensure end date is not before start date
        if (value < RequestStartDate)
        {
            RequestEndDate = RequestStartDate;
        }
        CalculateDays();
    }

    private void CalculateDays()
    {
        // Calculate business days between start and end date (inclusive)
        int days = 0;
        var current = RequestStartDate.DateTime;
        var end = RequestEndDate.DateTime;
        while (current <= end)
        {
            // Skip weekends
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                days++;
            }
            current = current.AddDays(1);
        }
        CalculatedDays = Math.Max(1, days);
    }

    [RelayCommand]
    private async Task SubmitRequest()
    {
        if (string.IsNullOrWhiteSpace(RequestReason))
            return;

        // Validate sufficient balance
        if (CalculatedDays > DaysRemaining)
        {
            Console.WriteLine("Insufficient time off balance");
            return;
        }

        var request = new CreateTimeOffRequest
        {
            StartDate = RequestStartDate.DateTime,
            EndDate = RequestEndDate.DateTime,
            RequestType = RequestType,
            Reason = RequestReason,
            TotalDays = CalculatedDays
        };

        IsLoading = true;
        try
        {
            var response = await _apiClient.PostAsync<TimeOffRequestResponse>("api/timeoff", request);

            if (response.IsSuccess && response.Data != null)
            {
                // Add to UI
                var newRequest = new TimeOffRequestViewModel
                {
                    Id = response.Data.Id,
                    InternName = response.Data.InternName,
                    StartDate = response.Data.StartDate,
                    EndDate = response.Data.EndDate,
                    RequestType = response.Data.RequestType,
                    Reason = response.Data.Reason,
                    Status = response.Data.Status,
                    SubmittedDate = response.Data.SubmittedDate,
                    TotalDays = response.Data.TotalDays
                };

                MyRequests.Insert(0, newRequest); // Add to top
                UpdateRequestCount();

                // Update balance
                DaysPending += CalculatedDays;
                DaysRemaining -= CalculatedDays;

                IsSubmitDialogOpen = false;

                // Clear fields
                RequestReason = string.Empty;
            }
            else
            {
                Console.WriteLine($"Failed to submit request: {response.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Cancel Request Commands

    [RelayCommand]
    private void OpenCancelDialog(TimeOffRequestViewModel? request)
    {
        if (request == null || request.Status != "Pending") return;

        RequestToCancel = request;
        IsCancelDialogOpen = true;
    }

    [RelayCommand]
    private void CloseCancelDialog()
    {
        IsCancelDialogOpen = false;
        RequestToCancel = null;
    }

    [RelayCommand]
    private async Task ConfirmCancel()
    {
        if (RequestToCancel == null) return;

        IsLoading = true;
        try
        {
            var result = await _apiClient.DeleteAsync<object>($"api/timeoff/{RequestToCancel.Id}");

            if (result.IsSuccess)
            {
                // Update balance
                DaysPending -= RequestToCancel.TotalDays;
                DaysRemaining += RequestToCancel.TotalDays;

                // Remove from UI
                MyRequests.Remove(RequestToCancel);
                UpdateRequestCount();

                IsCancelDialogOpen = false;
                RequestToCancel = null;
            }
            else
            {
                Console.WriteLine($"Failed to cancel request: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
}

/// <summary>
/// Represents a time off request
/// </summary>
public partial class TimeOffRequestViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _internName = string.Empty;

    [ObservableProperty]
    private DateTime _startDate;

    [ObservableProperty]
    private DateTime _endDate;

    [ObservableProperty]
    private string _requestType = string.Empty;

    [ObservableProperty]
    private string _reason = string.Empty;

    [ObservableProperty]
    private string _status = "Pending"; // Pending, Approved, Denied

    [ObservableProperty]
    private DateTime _submittedDate;

    [ObservableProperty]
    private DateTime? _reviewedDate;

    [ObservableProperty]
    private string _reviewedBy = string.Empty;

    [ObservableProperty]
    private int _totalDays;

    public string DateRange => $"{StartDate:MMM d} - {EndDate:MMM d, yyyy}";

    public string DaysText => TotalDays == 1 ? "1 day" : $"{TotalDays} days";

    public string SubmittedDateDisplay => $"Submitted {SubmittedDate:M/d/yyyy}";

    public string StatusColor => Status switch
    {
        "Approved" => "#22C55E", // Green
        "Denied" => "#EF4444",   // Red
        _ => "#F59E0B"           // Amber for Pending
    };

    public bool IsPending => Status == "Pending";

    public bool IsReviewed => Status == "Approved" || Status == "Denied";

    public string ReviewedInfo => IsReviewed && ReviewedDate.HasValue
        ? $"{Status} by {ReviewedBy} on {ReviewedDate.Value:M/d/yyyy}"
        : string.Empty;
}