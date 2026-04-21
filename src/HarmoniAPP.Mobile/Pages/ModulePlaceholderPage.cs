namespace HarmoniAPP.Mobile.Pages;

public class ModulePlaceholderPage : ContentPage
{
    public ModulePlaceholderPage(string title, string description)
    {
        Title = title;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 16,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold
                    },
                    new Label
                    {
                        Text = description,
                        FontSize = 16
                    },
                    new Label
                    {
                        Text = "A estrutura do módulo já está pronta no app. O próximo passo é conectar a listagem e as ações completas à API.",
                        FontSize = 14,
                        TextColor = Colors.Gray
                    }
                }
            }
        };
    }
}
