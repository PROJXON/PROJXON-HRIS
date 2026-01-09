using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Create Survey view
/// Allows users to create new surveys with questions
/// </summary>
public partial class CreateSurveyViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    #region Sidebar User Profile

    [ObservableProperty]
    private string _userName = "John Smith";

    [ObservableProperty]
    private string _userRole = "HR Manager";

    #endregion

    #region Survey Details

    [ObservableProperty]
    private string _surveyTitle = string.Empty;

    [ObservableProperty]
    private string _surveyDescription = string.Empty;

    [ObservableProperty]
    private string _newQuestionText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<SurveyQuestionViewModel> _questions = new();

    #endregion

    #region Survey Summary

    [ObservableProperty]
    private string _status = "Draft";

    public int TotalQuestions => Questions.Count;

    #endregion

    #region State

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    #endregion

    public CreateSurveyViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Questions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalQuestions));
    }

    // Parameterless constructor for design-time support
    public CreateSurveyViewModel() : this(null!)
    {
    }

    #region Question Commands

    [RelayCommand]
    private void AddQuestion()
    {
        if (string.IsNullOrWhiteSpace(NewQuestionText))
            return;

        var questionNumber = Questions.Count + 1;
        Questions.Add(new SurveyQuestionViewModel
        {
            QuestionNumber = questionNumber,
            QuestionText = NewQuestionText.Trim()
        });

        NewQuestionText = string.Empty;
    }

    [RelayCommand]
    private void DeleteQuestion(SurveyQuestionViewModel? question)
    {
        if (question == null) return;

        Questions.Remove(question);

        // Renumber remaining questions
        for (int i = 0; i < Questions.Count; i++)
        {
            Questions[i].QuestionNumber = i + 1;
        }
    }

    #endregion

    #region Save/Cancel Commands

    [RelayCommand]
    private async Task SaveAsDraft()
    {
        if (string.IsNullOrWhiteSpace(SurveyTitle))
        {
            ErrorMessage = "Please enter a survey title";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            // TODO: Call API to save survey as draft
            await Task.Delay(500); // Simulate API call

            // Navigate back to forms list
            await _navigationService.NavigateTo(ViewModelType.Forms);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save survey: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    #endregion

    #region Navigation Commands

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        await _navigationService.NavigateTo(ViewModelType.HRDashboard);
    }

    [RelayCommand]
    private async Task NavigateToProfile()
    {
        await _navigationService.NavigateTo(ViewModelType.Profile);
    }

    [RelayCommand]
    private async Task NavigateToTimeOff()
    {
        // TODO: Navigate to time off view when implemented
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToAttendance()
    {
        await _navigationService.NavigateTo(ViewModelType.Attendance);
    }

    [RelayCommand]
    private async Task NavigateToEmployees()
    {
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

    [RelayCommand]
    private async Task NavigateToRecruitment()
    {
        await _navigationService.NavigateTo(ViewModelType.Recruitment);
    }

    [RelayCommand]
    private async Task NavigateToForms()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        // Reset form when navigating to create new survey
        SurveyTitle = string.Empty;
        SurveyDescription = string.Empty;
        NewQuestionText = string.Empty;
        Questions.Clear();
        Status = "Draft";
        ErrorMessage = string.Empty;

        await base.OnNavigatedToAsync();
    }
}

/// <summary>
/// Represents a question in a survey
/// </summary>
public partial class SurveyQuestionViewModel : ObservableObject
{
    [ObservableProperty]
    private int _questionNumber;

    [ObservableProperty]
    private string _questionText = string.Empty;

    public string QuestionLabel => $"Question {QuestionNumber}";
}
