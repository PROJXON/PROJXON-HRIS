using System;
using System.Collections.Generic;
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
    
    // Read Only Property
    [ObservableProperty] private bool _isReadOnly;
    
    // Dynamic return destination - defaults to Tasks (for Interns)
    public ViewModelType ReturnDestination { get; set; } = ViewModelType.Tasks;
    
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
                var data = response.Data;
                var survey = data.Survey;
                
                Title = survey.Title;
                Description = survey.Description;
                
                // Determine if Read Only
                IsReadOnly = data.IsCompleted;

                // Parse Questions
                if (!string.IsNullOrEmpty(survey.QuestionsJson))
                {
                    ParseQuestions(survey.QuestionsJson);
                }

                // If Completed, Parse Answers and fill them in
                if (IsReadOnly && !string.IsNullOrEmpty(data.ResponseJson))
                {
                    ParseAnswers(data.ResponseJson);
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

    // Helper to parse answers
    private void ParseAnswers(string json)
    {
        try
        {
            var answers = JsonSerializer.Deserialize<List<AnswerDto>>(json);
            if (answers != null)
            {
                foreach (var ans in answers)
                {
                    var question = Questions.FirstOrDefault(q => q.QuestionId == ans.id);
                    if (question != null)
                    {
                        question.Answer = ans.answer ?? "";
                    }
                }
            }
        }
        catch
        {
            // Fail silently or log if answer parsing fails
        }
    }

    // Private DTO for deserialization
    private class AnswerDto
    {
        public int id { get; set; }
        public string? answer { get; set; }
    }

    [RelayCommand]
    private async Task SubmitSurvey()
    {
        IsLoading = true;
        try
        {
            var answers = Questions.Select(q => new { id = q.QuestionId, answer = q.Answer }).ToList();
            var jsonAnswers = JsonSerializer.Serialize(answers);

            var response = await _apiClient.PostAsync<object>($"api/Survey/complete/{_assignmentId}", jsonAnswers);
            
            if (response.IsSuccess)
            {
                // Use dynamic destination instead of hardcoding Tasks
                await _navigationService.NavigateTo(ReturnDestination);
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
        // Use the dynamic destination instead of hardcoding Tasks
        await _navigationService.NavigateTo(ReturnDestination);
    }
}

public partial class SurveyAnswerViewModel : ObservableObject
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    [ObservableProperty] private string _answer = string.Empty;
}