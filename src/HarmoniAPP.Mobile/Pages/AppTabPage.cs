namespace HarmoniAPP.Mobile.Pages;

public sealed class AppTabPage : TabbedPage
{
    public AppTabPage(
        HomePage homePage,
        PipelinePage pipelinePage,
        TasksPage tasksPage,
        InteractionsPage interactionsPage,
        ProfilePage profilePage)
    {
        Children.Add(CreateTab(homePage, "Resumo"));
        Children.Add(CreateTab(pipelinePage, "Pipeline"));
        Children.Add(CreateTab(tasksPage, "Agenda"));
        Children.Add(CreateTab(interactionsPage, "Interações"));
        Children.Add(CreateTab(profilePage, "Perfil"));
    }

    public void SelectTab(int index)
    {
        if (index >= 0 && index < Children.Count)
        {
            CurrentPage = Children[index];
        }
    }

    private static Page CreateTab(ContentPage rootPage, string title)
    {
        rootPage.Title = title;
        var navigationPage = new NavigationPage(rootPage)
        {
            Title = title
        };

        return navigationPage;
    }
}
