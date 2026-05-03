using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Views.Controls;

public partial class EmojiPicker : UserControl
{
    private static readonly string[] _symbols =
    {
        ":)", ":(", ":D", ";)", ":P", ":o", "<3", "</3",
        "+1", "-1", "ok", "..", "?!", "!!", "==", "~~",
        "[*]", "[+]", "[-]", "[!]", "[?]", "[#]", "[@]", "[$]",
        "(c)", "(r)", "(tm)", "...", "->", "<-", "/\\", "\\/",
        "v", "vv", "x", "v.", "..", "::", ";;", "@@",
        "##", "&&", "%%", "**", "''", "\"\"", "()", "[]",
        "{}", "<>", "//", "\\\\", "||", "+-", "-+", "=>",
        "<=", ">=", "!=", "<>", "++", "--", "::", ".."
    };

    public event EventHandler<string>? SymbolPicked;

    public EmojiPicker()
    {
        InitializeComponent();
        ItemsHost.ItemsSource = _symbols;
    }

    private void OnSymbolClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Content: string s })
            SymbolPicked?.Invoke(this, s);
    }
}
