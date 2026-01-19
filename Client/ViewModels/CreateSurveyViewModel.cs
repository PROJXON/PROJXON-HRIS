using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Create Survey view
/// Allows users to create new surveys with questions
/// </summary>
public partial class CreateSurveyViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IApiClient _apiClient;

    public SidebarViewModel Sidebar { get; }

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

    public CreateSurveyViewModel(
        INavigationService navigationService, 
        SidebarViewModel sidebarViewModel,
        IApiClient apiClient)
    {
        _navigationService = navigationService;
        Sidebar = sidebarViewModel;
        _apiClient = apiClient;
        Questions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalQuestions));
    }

    // Parameterless constructor for design-time support
    public CreateSurveyViewModel() : this(null!, new SidebarViewModel(), null!)
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
            // Serialize questions
            var questionsData = Questions.Select(q => new { id = q.QuestionNumber, text = q.QuestionText });
            var jsonQuestions = JsonSerializer.Serialize(questionsData);

            var request = new CreateSurveyRequest
            {
                Title = SurveyTitle,
                Description = SurveyDescription,
                QuestionsJson = jsonQuestions
            };

            var response = await _apiClient.PostAsync<SurveyResponse>("api/Survey", request);

            if (response.IsSuccess)
            {
                await _navigationService.NavigateTo(ViewModelType.Forms);
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }
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

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Forms";
        Sidebar.SetPortalMode(false);

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