using System.Windows;
using Engine.Services;
using Engine.Views;

namespace Engine;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeService.Instance.Apply(ThemeMode.Dark);
        var auth = new AuthWindow();
        auth.Show();
    }
}
