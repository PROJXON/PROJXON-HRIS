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
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class SurveyResponsesViewModel : ViewModelBase
{
    private readonly IApiClient _apiClient;
    private readonly INavigationService _navigationService;
    
    // We need the sidebar to set the active page
    public SidebarViewModel Sidebar { get; }

    [ObservableProperty] private string _surveyTitle = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private ObservableCollection<SurveyAssignmentResponse> _assignments = new();

    public SurveyResponsesViewModel(
        IApiClient apiClient, 
        INavigationService navigationService,
        SidebarViewModel sidebarViewModel)
    {
        _apiClient = apiClient;
        _navigationService = navigationService;
        Sidebar = sidebarViewModel;
    }

    public async Task LoadResponsesAsync(int surveyId)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        Assignments.Clear();

        try
        {
            // Get Survey Details for the Title
            var surveyResult = await _apiClient.GetByIdAsync<SurveyResponse>("api/Survey", surveyId);
            if (surveyResult.IsSuccess && surveyResult.Data != null)
            {
                SurveyTitle = surveyResult.Data.Title;
            }

            // Get Assignments specific to this survey
            var result = await _apiClient.GetAllAsync<IEnumerable<SurveyAssignmentResponse>>($"api/Survey/{surveyId}/assignments");

            if (result.IsSuccess && result.Data != null)
            {
                foreach (var item in result.Data)
                {
                    Assignments.Add(item);
                }
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ViewDetails(SurveyAssignmentResponse? assignment)
    {
        if (assignment == null) return;
        
        // Navigate to the same view used for taking surveys.
        // It will detect it's completed and show Read-Only mode.
        await _navigationService.NavigateTo(ViewModelType.TakeSurvey, assignment.Id);
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }
    
    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Forms";
        Sidebar.SetPortalMode(false);
        await base.OnNavigatedToAsync();
    }
}