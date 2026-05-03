using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Engine.Services;
using Engine.ViewModels;

namespace Engine.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var fade = new DoubleAnimation(0, 1, new System.Windows.Duration(System.TimeSpan.FromMilliseconds(250)));
        BeginAnimation(OpacityProperty, fade);
    }

    private void OnDragHeader(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
        {
            try
            {
                if (e.ClickCount == 2) ToggleMaximize();
                else DragMove();
            }
            catch { }
        }
    }

    private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void OnMaximize(object sender, RoutedEventArgs e) => ToggleMaximize();

    private void ToggleMaximize()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        var auth = new AuthWindow();
        auth.Show();
        Close();
    }

    private void OnNavigate(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is AppSection section && DataContext is MainViewModel vm)
        {
            vm.CurrentSection = section;
            AnimatePageIn();
        }
    }

    private void AnimatePageIn()
    {
        var slide = new DoubleAnimation(-20, 0, new System.Windows.Duration(System.TimeSpan.FromMilliseconds(200)))
        {
            EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = EasingMode.EaseOut }
        };
        PageT.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, slide);

        var fade = new DoubleAnimation(0, 1, new System.Windows.Duration(System.TimeSpan.FromMilliseconds(220)));
        PageRoot.BeginAnimation(OpacityProperty, fade);
    }

    private void OnGlobalKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
        {
            if (e.Key == Key.F)
            {
                vm.IsGlobalSearchOpen = true;
                e.Handled = true;
                return;
            }
            if (e.Key == Key.N)
            {
                vm.CurrentSection = AppSection.Contacts;
                e.Handled = true;
                return;
            }
        }
        if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
        {
            switch (e.Key)
            {
                case Key.D1: vm.CurrentSection = AppSection.Chats; AnimatePageIn(); e.Handled = true; break;
                case Key.D2: vm.CurrentSection = AppSection.Contacts; AnimatePageIn(); e.Handled = true; break;
                case Key.D3: vm.CurrentSection = AppSection.Stories; AnimatePageIn(); e.Handled = true; break;
                case Key.D4: vm.CurrentSection = AppSection.Channels; AnimatePageIn(); e.Handled = true; break;
                case Key.D5: vm.CurrentSection = AppSection.Favorites; AnimatePageIn(); e.Handled = true; break;
                case Key.D6: vm.CurrentSection = AppSection.Notifications; AnimatePageIn(); e.Handled = true; break;
            }
        }
        if (e.Key == Key.Escape)
        {
            if (vm.IsStoryOverlayVisible) vm.ActiveStory = null;
            else if (vm.IsGlobalSearchOpen) vm.IsGlobalSearchOpen = false;
            else if (vm.ChatVM.HasReply) vm.ChatVM.ReplyTo = null;
            e.Handled = true;
        }
    }
}
