using System.Globalization;
using HarmoniAPP.Core.Models.Leads;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class CreateLeadPage : ContentPage
{
    private readonly LeadsApiClient _leadsApiClient;
    private readonly IReadOnlyList<FormOption> _statusOptions =
    [
        new("New", "Novo"),
        new("Contacted", "Contato inicial"),
        new("Qualified", "Qualificado"),
        new("ProposalSent", "Proposta enviada"),
        new("Won", "Ganho"),
        new("Lost", "Perdido")
    ];

    private bool _isSaving;

    public CreateLeadPage(LeadsApiClient leadsApiClient)
    {
        _leadsApiClient = leadsApiClient;
        InitializeComponent();

        StatusPicker.ItemsSource = _statusOptions.ToList();
        StatusPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        StatusPicker.SelectedItem = _statusOptions[0];
        FollowUpDatePicker.Date = DateTime.Today.AddDays(2);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_isSaving)
        {
            return;
        }

        if (!Validate(out var potentialValue))
        {
            return;
        }

        _isSaving = true;
        SaveButton.IsEnabled = false;

        try
        {
            StatusLabel.IsVisible = false;
            var selectedStatus = (FormOption)StatusPicker.SelectedItem!;

            var request = new CreateLeadRequest(
                NameEntry.Text!.Trim(),
                CompanyEntry.Text!.Trim(),
                EmailEntry.Text!.Trim(),
                EmptyAsNull(PhoneEntry.Text),
                selectedStatus.Value,
                potentialValue,
                EmptyAsNull(SourceEntry.Text),
                null,
                null,
                FollowUpDatePicker.Date,
                EmptyAsNull(NotesEditor.Text));

            await _leadsApiClient.CreateAsync(request);
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

    private bool Validate(out decimal? potentialValue)
    {
        potentialValue = null;

        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(CompanyEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            StatusLabel.Text = "Nome, empresa e e-mail são obrigatórios.";
            StatusLabel.IsVisible = true;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(PotentialValueEntry.Text))
        {
            if (!TryParseDecimal(PotentialValueEntry.Text, out var parsedPotentialValue))
            {
                StatusLabel.Text = "Informe um valor potencial válido.";
                StatusLabel.IsVisible = true;
                return false;
            }

            potentialValue = parsedPotentialValue;
        }

        return true;
    }

    private static string? EmptyAsNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool TryParseDecimal(string input, out decimal value) =>
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value) ||
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
}
