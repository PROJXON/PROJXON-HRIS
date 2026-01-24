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
/// ViewModel for the HR Time Off Requests view
/// Allows HR to review, approve, and deny time off requests from interns
/// </summary>
public partial class TimeOffRequestsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IApiClient _apiClient;
    //private readonly IEmailService _emailService; -> for email feat, not ready yet

    public SidebarViewModel Sidebar { get; }

    #region Request Categories

    [ObservableProperty]
    private ObservableCollection<RequestCategoryViewModel> _categories = new();

    [ObservableProperty]
    private string _selectedFilter = "all"; // all, pending, approved, denied

    #endregion

    #region All Requests View

    [ObservableProperty]
    private ObservableCollection<TimeOffRequestItemViewModel> _allRequests = new();

    [ObservableProperty]
    private ObservableCollection<TimeOffRequestItemViewModel> _filteredRequests = new();

    [ObservableProperty]
    private bool _hasNoRequests;

    [ObservableProperty]
    private string _requestCountText = "0 requests";

    #endregion

    #region Review Dialog

    [ObservableProperty]
    private bool _isReviewDialogOpen;

    [ObservableProperty]
    private TimeOffRequestItemViewModel? _requestToReview;

    [ObservableProperty]
    private string _reviewNotes = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    #endregion

    #region Statistics

    [ObservableProperty]
    private int _totalPendingRequests = 0;

    [ObservableProperty]
    private int _totalApprovedThisMonth = 0;

    [ObservableProperty]
    private int _totalDeniedThisMonth = 0;

    #endregion

    public TimeOffRequestsViewModel(
        INavigationService navigationService,
        IApiClient apiClient,
        //IEmailService emailService, -> for email feat, not ready yet
        SidebarViewModel sidebarViewModel)
    {
        _navigationService = navigationService;
        _apiClient = apiClient;
        //_emailService = emailService; -> for email feat, not ready yet
        Sidebar = sidebarViewModel;
        InitializeCategories();
    }

    // Parameterless constructor for design-time support
    public TimeOffRequestsViewModel() : this(null!, null!, new SidebarViewModel())
    {
    }

    private void InitializeCategories()
    {
        Categories = new ObservableCollection<RequestCategoryViewModel>
        {
            new() { CategoryId = "all", CategoryName = "All Requests", FilterValue = "all" },
            new() { CategoryId = "pending", CategoryName = "Pending Review", FilterValue = "pending" },
            new() { CategoryId = "approved", CategoryName = "Approved", FilterValue = "approved" },
            new() { CategoryId = "denied", CategoryName = "Denied", FilterValue = "denied" }
        };
    }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Time Off Requests";
        Sidebar.SetPortalMode(false); // HR mode
        await LoadRequestsAsync();
        await base.OnNavigatedToAsync();
    }

    public override async Task OnNavigatedFromAsync()
    {
        await Sidebar.OnNavigatedFromAsync();
        await base.OnNavigatedFromAsync();
    }

    private async Task LoadRequestsAsync()
    {
        try
        {
            var result = await _apiClient.GetAllAsync<IEnumerable<TimeOffRequestResponse>>("api/timeoff/all");
            if (result.IsSuccess && result.Data != null)
            {
                AllRequests.Clear();

                foreach (var request in result.Data.OrderByDescending(r => r.SubmittedDate))
                {
                    AllRequests.Add(new TimeOffRequestItemViewModel
                    {
                        Id = request.Id,
                        InternId = request.InternId,
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

                // Update statistics
                CalculateStatistics();

                // Update category counts
                UpdateCategoryCounts();

                // Apply filter
                ApplyFilter(SelectedFilter);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading requests: {ex.Message}");
        }
    }

    private void CalculateStatistics()
    {
        TotalPendingRequests = AllRequests.Count(r => r.Status == "Pending");

        var thisMonth = DateTime.Now.Month;
        var thisYear = DateTime.Now.Year;

        TotalApprovedThisMonth = AllRequests.Count(r =>
            r.Status == "Approved" &&
            r.ReviewedDate.HasValue &&
            r.ReviewedDate.Value.Month == thisMonth &&
            r.ReviewedDate.Value.Year == thisYear);

        TotalDeniedThisMonth = AllRequests.Count(r =>
            r.Status == "Denied" &&
            r.ReviewedDate.HasValue &&
            r.ReviewedDate.Value.Month == thisMonth &&
            r.ReviewedDate.Value.Year == thisYear);
    }

    private void UpdateCategoryCounts()
    {
        foreach (var category in Categories)
        {
            int count = category.FilterValue switch
            {
                "all" => AllRequests.Count,
                "pending" => AllRequests.Count(r => r.Status == "Pending"),
                "approved" => AllRequests.Count(r => r.Status == "Approved"),
                "denied" => AllRequests.Count(r => r.Status == "Denied"),
                _ => 0
            };

            category.RequestCount = count;
            category.CountText = count == 1 ? "1 request" : $"{count} requests";
        }
    }

    #region Filter Commands

    [RelayCommand]
    private void SelectFilter(RequestCategoryViewModel? category)
    {
        if (category == null) return;

        // Update selection state
        foreach (var cat in Categories)
        {
            cat.IsSelected = cat.CategoryId == category.CategoryId;
        }

        SelectedFilter = category.FilterValue;
        ApplyFilter(SelectedFilter);
    }

    private void ApplyFilter(string filter)
    {
        FilteredRequests.Clear();

        var filtered = filter switch
        {
            "pending" => AllRequests.Where(r => r.Status == "Pending"),
            "approved" => AllRequests.Where(r => r.Status == "Approved"),
            "denied" => AllRequests.Where(r => r.Status == "Denied"),
            _ => AllRequests
        };

        foreach (var request in filtered)
        {
            FilteredRequests.Add(request);
        }

        var count = FilteredRequests.Count;
        RequestCountText = count == 1 ? "1 request" : $"{count} requests";
        HasNoRequests = count == 0;
    }

    #endregion

    #region Review Commands

    [RelayCommand]
    private void OpenReviewDialog(TimeOffRequestItemViewModel? request)
    {
        if (request == null || request.Status != "Pending") return;

        RequestToReview = request;
        ReviewNotes = string.Empty;
        IsReviewDialogOpen = true;
    }

    [RelayCommand]
    private void CloseReviewDialog()
    {
        IsReviewDialogOpen = false;
        RequestToReview = null;
        ReviewNotes = string.Empty;
    }

    [RelayCommand]
    private async Task ApproveRequest()
    {
        if (RequestToReview == null) return;

        await ReviewRequestAsync(RequestToReview, "Approved");
    }

    [RelayCommand]
    private async Task DenyRequest()
    {
        if (RequestToReview == null) return;

        await ReviewRequestAsync(RequestToReview, "Denied");
    }

    private async Task ReviewRequestAsync(TimeOffRequestItemViewModel request, string decision)
    {
        IsLoading = true;
        try
        {
            var reviewRequest = new ReviewTimeOffRequest
            {
                Decision = decision,
                ReviewNotes = ReviewNotes
            };

            var result = await _apiClient.PutAsync<TimeOffRequestResponse>(
                $"api/timeoff/{request.Id}/review",
                reviewRequest);

            if (result.IsSuccess && result.Data != null)
            {
                // Update the request in the UI
                request.Status = result.Data.Status;
                request.ReviewedDate = result.Data.ReviewedDate;
                request.ReviewedBy = result.Data.ReviewedBy;

                // Send email notification -> for email feat, not ready yet
                //await SendEmailNotificationAsync(request, decision);

                // Recalculate statistics
                CalculateStatistics();

                // Update category counts
                UpdateCategoryCounts();

                // Reapply filter to update the filtered list
                ApplyFilter(SelectedFilter);

                // Close dialog
                IsReviewDialogOpen = false;
                RequestToReview = null;
                ReviewNotes = string.Empty;
            }
            else
            {
                Console.WriteLine($"Failed to review request: {result.ErrorMessage}");
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

    /* for email feat, not ready yet
    private async Task SendEmailNotificationAsync(TimeOffRequestItemViewModel request, string decision)
    {
        try
        {
            // Get intern's email - you might need to fetch this from the API if not available
            // For now, assuming we can construct it or have it in the request
            var internEmail = $"{request.InternName.Replace(" ", ".").ToLower()}@company.com"; // Replace with actual email

            if (decision == "Approved")
            {
                await _emailService.SendTimeOffApprovalEmailAsync(
                    recipientEmail: internEmail,
                    recipientName: request.InternName,
                    requestType: request.RequestType,
                    startDate: request.StartDate.ToString("MM/dd/yyyy"),
                    endDate: request.EndDate.ToString("MM/dd/yyyy"),
                    totalDays: request.TotalDays,
                    reviewedBy: request.ReviewedBy ?? "HR"
                );
            }
            else if (decision == "Denied")
            {
                await _emailService.SendTimeOffDenialEmailAsync(
                    recipientEmail: internEmail,
                    recipientName: request.InternName,
                    requestType: request.RequestType,
                    startDate: request.StartDate.ToString("MM/dd/yyyy"),
                    endDate: request.EndDate.ToString("MM/dd/yyyy"),
                    totalDays: request.TotalDays,
                    reviewedBy: request.ReviewedBy ?? "HR",
                    reason: ReviewNotes
                );
            }

            Console.WriteLine($"Email notification sent to {request.InternName} for {decision} decision");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email notification: {ex.Message}");
            // Don't fail the entire operation if email fails
        }
    } */

    #endregion

    #region Bulk Actions

    [RelayCommand]
    private async Task ApproveAllPending()
    {
        var pendingRequests = AllRequests.Where(r => r.Status == "Pending").ToList();

        if (!pendingRequests.Any())
            return;

        IsLoading = true;
        try
        {
            foreach (var request in pendingRequests)
            {
                var reviewRequest = new ReviewTimeOffRequest
                {
                    Decision = "Approved",
                    ReviewNotes = "Bulk approval"
                };

                await _apiClient.PutAsync<TimeOffRequestResponse>(
                    $"api/timeoff/{request.Id}/review",
                    reviewRequest);

                request.Status = "Approved";
                request.ReviewedDate = DateTime.Now;
                request.ReviewedBy = "HR"; // Should be actual user name
            }

            // Recalculate statistics and update UI
            CalculateStatistics();
            UpdateCategoryCounts();
            ApplyFilter(SelectedFilter);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in bulk approval: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Export Commands

    [RelayCommand]
    private void ExportToExcel()
    {
        // TODO: Implement Excel export functionality
        // This would create an xlsx file with all requests
        Console.WriteLine("Export to Excel functionality to be implemented");
    }

    #endregion
}

/// <summary>
/// Represents a category filter for time off requests
/// </summary>
public partial class RequestCategoryViewModel : ObservableObject
{
    [ObservableProperty]
    private string _categoryId = string.Empty;

    [ObservableProperty]
    private string _categoryName = string.Empty;

    [ObservableProperty]
    private string _filterValue = string.Empty;

    [ObservableProperty]
    private int _requestCount;

    [ObservableProperty]
    private string _countText = "0 requests";

    [ObservableProperty]
    private bool _isSelected;
}

/// <summary>
/// Represents a time off request item for HR review
/// </summary>
public partial class TimeOffRequestItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _internId;

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

    [ObservableProperty]
    private bool _isHovered;

    public string InternInitial => !string.IsNullOrEmpty(InternName) 
        ? InternName[0].ToString().ToUpper() 
        : "?";

    public string DateRange => $"{StartDate:MMM d} - {EndDate:MMM d, yyyy}";

    public string DaysText => TotalDays == 1 ? "1 day" : $"{TotalDays} days";

    public string SubmittedDateDisplay => $"Submitted {SubmittedDate:M/d/yyyy}";

    public string StatusColor => Status switch
    {
        "Approved" => "#22C55E", // Green
        "Denied" => "#EF4444",   // Red
        _ => "#F59E0B"           // Amber for Pending
    };

    public string StatusBadgeBackground => Status switch
    {
        "Approved" => "#DCFCE7", // Light green
        "Denied" => "#FEE2E2",   // Light red
        _ => "#FEF3C7"           // Light amber
    };

    public bool IsPending => Status == "Pending";

    public bool IsReviewed => Status == "Approved" || Status == "Denied";

    public string ReviewedInfo => IsReviewed && ReviewedDate.HasValue
        ? $"{Status} by {ReviewedBy} on {ReviewedDate.Value:M/d/yyyy}"
        : string.Empty;
}