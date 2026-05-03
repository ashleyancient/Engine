using System.Windows;
using System.Windows.Input;
using Engine.ViewModels;

namespace Engine.Views;

public partial class AuthWindow : Window
{
    private AuthViewModel? _vm;

    public AuthWindow()
    {
        InitializeComponent();
        Loaded += AuthWindow_Loaded;
    }

    private void AuthWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _vm = (AuthViewModel)DataContext;
        LoginPasswordBox.Password = _vm.LoginPassword;
        _vm.AuthenticationSucceeded += OnAuthSucceeded;
    }

    private void OnAuthSucceeded(Models.User user)
    {
        var main = new MainWindow();
        main.Show();
        Close();
    }

    private void OnDragHeader(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
        {
            try { DragMove(); } catch { }
        }
    }

    private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void OnClose(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    private void OnLoginPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_vm is not null) _vm.LoginPassword = LoginPasswordBox.Password;
    }

    private void OnRegPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_vm is not null) _vm.RegPassword = RegPasswordBox.Password;
    }

    private void OnRegConfirmChanged(object sender, RoutedEventArgs e)
    {
        if (_vm is not null) _vm.RegConfirm = RegConfirmBox.Password;
    }
}
