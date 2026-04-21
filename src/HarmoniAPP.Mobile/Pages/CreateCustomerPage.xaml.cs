using System.Globalization;
using HarmoniAPP.Core.Models.Customers;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class CreateCustomerPage : ContentPage
{
    private readonly CustomersApiClient _customersApiClient;
    private readonly IReadOnlyList<FormOption> _healthOptions =
    [
        new("Strong", "Saudável"),
        new("Stable", "Monitorar"),
        new("Risk", "Em risco")
    ];

    private bool _isSaving;

    public CreateCustomerPage(CustomersApiClient customersApiClient)
    {
        _customersApiClient = customersApiClient;
        InitializeComponent();

        HealthPicker.ItemsSource = _healthOptions.ToList();
        HealthPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        HealthPicker.SelectedItem = _healthOptions[0];
        LastInteractionDatePicker.Date = DateTime.Today;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_isSaving)
        {
            return;
        }

        if (!Validate(out var revenue))
        {
            return;
        }

        _isSaving = true;
        SaveButton.IsEnabled = false;

        try
        {
            StatusLabel.IsVisible = false;
            var selectedHealth = (FormOption)HealthPicker.SelectedItem!;

            var request = new CreateCustomerRequest(
                CompanyEntry.Text!.Trim(),
                EmptyAsNull(SegmentEntry.Text),
                PrimaryContactEntry.Text!.Trim(),
                EmailEntry.Text!.Trim(),
                EmptyAsNull(PhoneEntry.Text),
                selectedHealth.Value,
                revenue,
                null,
                null,
                LastInteractionDatePicker.Date,
                EmptyAsNull(NotesEditor.Text));

            await _customersApiClient.CreateAsync(request);
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

    private bool Validate(out decimal? revenue)
    {
        revenue = null;

        if (string.IsNullOrWhiteSpace(CompanyEntry.Text) ||
            string.IsNullOrWhiteSpace(PrimaryContactEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            StatusLabel.Text = "Empresa, contato principal e e-mail são obrigatórios.";
            StatusLabel.IsVisible = true;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(RevenueEntry.Text))
        {
            if (!TryParseDecimal(RevenueEntry.Text, out var parsedRevenue))
            {
                StatusLabel.Text = "Informe uma receita mensal válida.";
                StatusLabel.IsVisible = true;
                return false;
            }

            revenue = parsedRevenue;
        }

        return true;
    }

    private static string? EmptyAsNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool TryParseDecimal(string input, out decimal value) =>
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value) ||
        decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
}
