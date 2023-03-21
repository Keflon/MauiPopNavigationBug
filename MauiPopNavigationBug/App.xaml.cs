using System.Diagnostics;

namespace MauiPopNavigationBug;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		//MainPage = new AppShell();
		MainPage = new NavigationPage(new MainPage());

        App.Current.PageAppearing += Current_PageAppearing;
        App.Current.PageDisappearing += Current_PageDisappearing;
	}

    private void Current_PageDisappearing(object sender, Page e)
    {
        Debug.WriteLine($"Current_PageDisappearing {e}");
    }

    private void Current_PageAppearing(object sender, Page e)
    {
        Debug.WriteLine($"Current_PageAppearing {e}");
    }
}
