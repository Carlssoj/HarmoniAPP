using HarmoniAPP.Core.Models.Tasks;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class CreateTaskPage : ContentPage
{
    private readonly TasksApiClient _tasksApiClient;
    private readonly IReadOnlyList<FormOption> _priorityOptions =
    [
        new("Low", "Baixa"),
        new("Medium", "Média"),
        new("High", "Alta")
    ];

    private bool _isSaving;

    public CreateTaskPage(TasksApiClient tasksApiClient)
    {
        _tasksApiClient = tasksApiClient;
        InitializeComponent();

        PriorityPicker.ItemsSource = _priorityOptions.ToList();
        PriorityPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        PriorityPicker.SelectedItem = _priorityOptions[1];
        DueDatePicker.Date = DateTime.Today.AddDays(1);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_isSaving)
        {
            return;
        }

        if (!Validate())
        {
            return;
        }

        _isSaving = true;
        SaveButton.IsEnabled = false;

        try
        {
            StatusLabel.IsVisible = false;
            var selectedPriority = (FormOption)PriorityPicker.SelectedItem!;

            var request = new CreateTaskRequest(
                TitleEntry.Text!.Trim(),
                RelatedToEntry.Text!.Trim(),
                null,
                null,
                selectedPriority.Value,
                DueDatePicker.Date,
                string.IsNullOrWhiteSpace(NotesEditor.Text) ? null : NotesEditor.Text.Trim());

            await _tasksApiClient.CreateAsync(request);
            await Navigation.PopModalAsync();
        }
        catch (Exception exception)
        {
            StatusLabel.Text = MobileErrorFormatter.Format(exception);
            StatusLabel.IsVisible = true;
        }
        finally
        {
            SaveButton.IsEnabled = true;
            _isSaving = false;
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(RelatedToEntry.Text))
        {
            StatusLabel.Text = "Título e contexto relacionado são obrigatórios.";
            StatusLabel.IsVisible = true;
            return false;
        }

        return true;
    }
}
