using HarmoniAPP.Core.Models.Tasks;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class EditTaskPage : ContentPage
{
    private readonly TasksApiClient _tasksApiClient;
    private readonly IReadOnlyList<FormOption> _priorityOptions =
    [
        new("Low", "Baixa"),
        new("Medium", "Média"),
        new("High", "Alta")
    ];

    private Guid _taskId;
    private bool _isSaving;

    public EditTaskPage(TasksApiClient tasksApiClient)
    {
        _tasksApiClient = tasksApiClient;
        InitializeComponent();

        PriorityPicker.ItemsSource = _priorityOptions.ToList();
        PriorityPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        PriorityPicker.SelectedItem = _priorityOptions[1];
    }

    public void Initialize(TaskItemResponse task)
    {
        _taskId = task.Id;
        TitleEntry.Text = task.Titulo;
        RelatedToEntry.Text = task.RelacionadoA;
        NotesEditor.Text = task.Observacoes;
        DueDatePicker.Date = task.DataVencimento;
        CompletedSwitch.IsToggled = task.Concluida;
        PriorityPicker.SelectedItem = _priorityOptions.FirstOrDefault(option => option.Label == task.Prioridade) ?? _priorityOptions[1];
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

            var request = new UpdateTaskRequest(
                TitleEntry.Text!.Trim(),
                RelatedToEntry.Text!.Trim(),
                null,
                null,
                selectedPriority.Value,
                DueDatePicker.Date,
                string.IsNullOrWhiteSpace(NotesEditor.Text) ? null : NotesEditor.Text.Trim(),
                CompletedSwitch.IsToggled);

            await _tasksApiClient.UpdateAsync(_taskId, request);
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
        if (_taskId == Guid.Empty)
        {
            StatusLabel.Text = "Nenhuma tarefa foi selecionada para edição.";
            StatusLabel.IsVisible = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(RelatedToEntry.Text))
        {
            StatusLabel.Text = "Título e contexto relacionado são obrigatórios.";
            StatusLabel.IsVisible = true;
            return false;
        }

        return true;
    }
}
