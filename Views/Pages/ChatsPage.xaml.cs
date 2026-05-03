using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Engine.ViewModels;

namespace Engine.Views.Pages;

public partial class ChatsPage : UserControl
{
    public ChatsPage()
    {
        InitializeComponent();
    }

    private void OnInputKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
        {
            if (sender is TextBox tb && tb.DataContext is ChatViewModel cvm)
            {
                e.Handled = true;
                if (cvm.SendMessageCommand.CanExecute(null))
                    cvm.SendMessageCommand.Execute(null);
            }
        }
    }
}
