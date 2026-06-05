using System;
using System.Windows;
using System.Windows.Controls;

namespace RelojControl.Controls;

public partial class NumericKeypad : UserControl
{
    public event EventHandler<string>? KeyPressed;

    public NumericKeypad() => InitializeComponent();

    private void Key_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
            KeyPressed?.Invoke(this, tag);
    }
}
