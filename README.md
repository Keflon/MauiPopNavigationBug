# MauiPopNavigationBug

## Bug report here:
https://github.com/dotnet/maui/issues/14092

## 

### Description

App.Current.PageDisappearing is not raised when a page is popped from the navigation stack.


### Steps to Reproduce

1. Create a File->New MAUI app.
2. Replace the constructor in App.xaml.cs with this:
```csharp
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
```
3. Replace the click-handler in MainPage.xaml.cs with this:
```csharp
    private void OnCounterClicked(object sender, EventArgs e)
    {
        App.Current.MainPage.Navigation.PushAsync(new ContentPage { Title="Child Page"});
    }
```
4. Run the app.
5. Look at the debug output from the new `Current_PageDisappearing` and `Current_PageAppearing` event handlers.
6. Observe that MainPage and NavigationPage are reported as appearing.
```
[0:] Current_PageAppearing MauiPopNavigationBug.MainPage
[0:] Current_PageAppearing Microsoft.Maui.Controls.NavigationPage
```
8. Tap the button.
9. Observe `MainPage` disappears and `ContentPage` appears.
```
[0:] Current_PageDisappearing MauiPopNavigationBug.MainPage
[0:] Current_PageAppearing Microsoft.Maui.Controls.ContentPage
```
10. Tap the back button
11. [BUG] Observe the ContentPage is popped, the MainPage appearing event is raised but the ContentPage PageDisappearing event is not raised.
```
[0:] Current_PageAppearing MauiPopNavigationBug.MainPage
```



### Link to public reproduction project repository

https://github.com/Keflon/MauiPopNavigationBug

### Version with bug

7.0 (current)

### Last version that worked well

Unknown/Other

### Affected platforms

iOS, Android, Windows, I was *not* able test on other platforms

### Affected platform versions

All current / latest.

### Did you find any workaround?

Subscribe to the following events:

```csharp
App.Current.DescendantAdded += CurrentApplication_DescendantAdded;
App.Current.DescendantRemoved += CurrentApplication_DescendantRemoved;
```
Do something like this:
```csharp
private void CurrentApplication_DescendantAdded(object sender, ElementEventArgs e)
{
    if (e.Element is Page cp)
    {
        cp.Disappearing += PageDisappearing;
        cp.Appearing += PageAppearing;
    }
}

private async void CurrentApplication_DescendantRemoved(object sender, ElementEventArgs e)
{
    if (e.Element is Page cp)
    {
        cp.Appearing -= PageAppearing;

        await Task.Yield();     // Reason: The disappearing event was/is raised after the DescendantRemoved event.
        cp.Disappearing -= PageDisappearing;
    }
}

```
The PageAppearing and PageDisappearing events are correctly raised.  

The above is not tested, however it is a subset of working `CurrentApplication_DescendantAdded` and `CurrentApplication_DescendantRemoved` implementations found here: https://github.com/Keflon/Maui.MvvmZero/blob/eada8a7059fadc248de13456b3be627373384bd8/Maui.MvvmZero/Implementation/PageServiceZero.cs  



### Relevant log output

```shell
[0:] Current_PageAppearing MauiPopNavigationBug.*
[0:] Current_PageDisappearing MauiPopNavigationBug.*
```
