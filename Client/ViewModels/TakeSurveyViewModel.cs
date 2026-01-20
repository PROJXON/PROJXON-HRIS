using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class TakeSurveyViewModel : ViewModelBase
{
    private readonly IApiClient _apiClient;
    private readonly INavigationService _navigationService;
    private int _assignmentId;

    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    
    public ObservableCollection<SurveyAnswerViewModel> Questions { get; } = new();

    public TakeSurveyViewModel(IApiClient apiClient, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _navigationService = navigationService;
    }

    public async Task LoadFromAssignmentAsync(int assignmentId)
    {
        _assignmentId = assignmentId;
        IsLoading = true;
        ErrorMessage = string.Empty;

        try 
        {
            var response = await _apiClient.GetByIdAsync<SurveyAssignmentResponse>("api/Survey/assignment", assignmentId);
            
            if (response.IsSuccess && response.Data != null && response.Data.Survey != null)
            {
                var survey = response.Data.Survey;
                
                Title = survey.Title;
                Description = survey.Description;
                
                if (!string.IsNullOrEmpty(survey.QuestionsJson))
                {
                    ParseQuestions(survey.QuestionsJson);
                }
            }
            else
            {
                ErrorMessage = "Failed to load survey details: " + response.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error loading survey: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ParseQuestions(string json)
    {
        try 
        {
            var parsedQuestions = JsonSerializer.Deserialize<JsonElement>(json);
            Questions.Clear();
            
            if (parsedQuestions.ValueKind == JsonValueKind.Array)
            {
                foreach (var q in parsedQuestions.EnumerateArray())
                {
                    // Handle case where properties might be "Id" or "id"
                    int qId = 0;
                    if (q.TryGetProperty("id", out var idProp)) qId = idProp.GetInt32();
                    else if (q.TryGetProperty("Id", out var idPropUpper)) qId = idPropUpper.GetInt32();

                    string qText = "";
                    if (q.TryGetProperty("text", out var textProp)) qText = textProp.GetString() ?? "";
                    else if (q.TryGetProperty("Text", out var textPropUpper)) qText = textPropUpper.GetString() ?? "";
                    else if (q.TryGetProperty("questionText", out var qTextProp)) qText = qTextProp.GetString() ?? "";

                    Questions.Add(new SurveyAnswerViewModel
                    {
                        QuestionId = qId,
                        QuestionText = qText,
                        Answer = ""
                    });
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error parsing questions: " + ex.Message;
        }
    }

    [RelayCommand]
    private async Task SubmitSurvey()
    {
        IsLoading = true;
        try
        {
            var answers = Questions.Select(q => new { id = q.QuestionId, answer = q.Answer }).ToList();
            var jsonAnswers = JsonSerializer.Serialize(answers);

            var response = await _apiClient.PostAsync<object>($"api/Survey/complete/{_assignmentId}", jsonAnswers); // Send raw string, handled by backend
            
            if (response.IsSuccess)
            {
                await _navigationService.NavigateTo(ViewModelType.Tasks);
            }
            else 
            {
                ErrorMessage = response.ErrorMessage;
            }
        }
        catch(Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await _navigationService.NavigateTo(ViewModelType.Tasks);
    }
}

public partial class SurveyAnswerViewModel : ObservableObject
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    [ObservableProperty] private string _answer = string.Empty;
}