using HarmoniAPP.Core.Models.Interactions;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class CreateInteractionPage : ContentPage
{
    private readonly InteractionsApiClient _interactionsApiClient;
    private readonly IReadOnlyList<FormOption> _typeOptions =
    [
        new("Call", "Ligação"),
        new("Meeting", "Reunião"),
        new("Email", "E-mail"),
        new("Proposal", "Proposta"),
        new("QuarterlyBusinessReview", "QBR"),
        new("Onboarding", "Onboarding")
    ];

    private readonly IReadOnlyList<FormOption> _recordTypeOptions =
    [
        new("Lead", "Lead"),
        new("Customer", "Cliente"),
        new("Opportunity", "Oportunidade"),
        new("General", "Conta geral")
    ];

    private bool _isSaving;

    public CreateInteractionPage(InteractionsApiClient interactionsApiClient)
    {
        _interactionsApiClient = interactionsApiClient;
        InitializeComponent();

        TypePicker.ItemsSource = _typeOptions.ToList();
        TypePicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        TypePicker.SelectedItem = _typeOptions[0];

        RecordTypePicker.ItemsSource = _recordTypeOptions.ToList();
        RecordTypePicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        RecordTypePicker.SelectedItem = _recordTypeOptions[1];

        OccurredOnDatePicker.Date = DateTime.Today;
        NextActionDatePicker.Date = DateTime.Today.AddDays(2);
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

            var type = (FormOption)TypePicker.SelectedItem!;
            var recordType = (FormOption)RecordTypePicker.SelectedItem!;
            var nextAction = string.IsNullOrWhiteSpace(NextActionEntry.Text) ? null : NextActionEntry.Text.Trim();

            var request = new CreateInteractionRequest(
                SubjectEntry.Text!.Trim(),
                type.Value,
                recordType.Value,
                RelatedToEntry.Text!.Trim(),
                null,
                null,
                OccurredOnDatePicker.Date,
                SummaryEditor.Text!.Trim(),
                nextAction,
                nextAction is null ? null : NextActionDatePicker.Date);

            await _interactionsApiClient.CreateAsync(request);
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
        if (string.IsNullOrWhiteSpace(SubjectEntry.Text) ||
            string.IsNullOrWhiteSpace(RelatedToEntry.Text) ||
            string.IsNullOrWhiteSpace(SummaryEditor.Text))
        {
            StatusLabel.Text = "Assunto, contexto e resumo são obrigatórios.";
            StatusLabel.IsVisible = true;
            return false;
        }

        return true;
    }
}
