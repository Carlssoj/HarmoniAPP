using System.Globalization;
using HarmoniAPP.Core.Models.Opportunities;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class CreateOpportunityPage : ContentPage
{
    private readonly OpportunitiesApiClient _opportunitiesApiClient;
    private readonly IReadOnlyList<FormOption> _stageOptions =
    [
        new("Discovery", "Diagnóstico"),
        new("SolutionDesign", "Desenho da solução"),
        new("Proposal", "Proposta"),
        new("Negotiation", "Negociação"),
        new("ClosedWon", "Ganho"),
        new("ClosedLost", "Perdido")
    ];

    private bool _isSaving;

    public CreateOpportunityPage(OpportunitiesApiClient opportunitiesApiClient)
    {
        _opportunitiesApiClient = opportunitiesApiClient;
        InitializeComponent();

        StagePicker.ItemsSource = _stageOptions.ToList();
        StagePicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        StagePicker.SelectedItem = _stageOptions[0];
        ExpectedCloseDatePicker.Date = DateTime.Today.AddDays(15);
        ProbabilityEntry.Text = "30";
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_isSaving)
        {
            return;
        }

        if (!Validate(out var amount, out var probability))
        {
            return;
        }

        _isSaving = true;
        SaveButton.IsEnabled = false;

        try
        {
            StatusLabel.IsVisible = false;
            var selectedStage = (FormOption)StagePicker.SelectedItem!;

            var request = new CreateOpportunityRequest(
                TitleEntry.Text!.Trim(),
                AccountEntry.Text!.Trim(),
                selectedStage.Value,
                amount,
                probability,
                ExpectedCloseDatePicker.Date,
                null,
                null,
                EmptyAsNull(SummaryEditor.Text),
                StrategicSwitch.IsToggled);

            await _opportunitiesApiClient.CreateAsync(request);
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

    private bool Validate(out decimal? amount, out int? probability)
    {
        amount = null;
        probability = null;

        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(AccountEntry.Text))
        {
            StatusLabel.Text = "Título da oportunidade e conta são obrigatórios.";
            StatusLabel.IsVisible = true;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AmountEntry.Text))
        {
            if (!TryParseDecimal(AmountEntry.Text, out var parsedAmount))
            {
                StatusLabel.Text = "Informe um valor válido.";
                StatusLabel.IsVisible = true;
                return false;
            }

            amount = parsedAmount;
        }

        if (!string.IsNullOrWhiteSpace(ProbabilityEntry.Text))
        {
            if (!int.TryParse(ProbabilityEntry.Text, out var parsedProbability) ||
                parsedProbability < 0 ||
                parsedProbability > 100)
            {
                StatusLabel.Text = "A probabilidade deve ficar entre 0 e 100.";
                StatusLabel.IsVisible = true;
                return false;
            }

            probability = parsedProbability;
        }

        return true;
    }

    private static string? EmptyAsNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool TryParseDecimal(string input, out decimal value) =>
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value) ||
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
}
