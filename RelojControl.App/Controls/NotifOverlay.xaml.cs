using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace RelojControl.Controls;

public enum NotifType { Info, Success, Warning, Error }

public partial class NotifOverlay : UserControl
{
    private DispatcherTimer? _timer;

    public NotifOverlay() => InitializeComponent();

    public void Show(string titulo, string mensaje, NotifType tipo = NotifType.Info, double segundos = 6)
    {
        lblTitulo.Text  = titulo;
        lblMensaje.Text = mensaje;

        colorBar.Fill = tipo switch
        {
            NotifType.Success => (Brush)FindResource("TealBrush"),
            NotifType.Warning => (Brush)FindResource("AmberBrush"),
            NotifType.Error   => (Brush)FindResource("CoralBrush"),
            _                 => (Brush)FindResource("AccentBrush"),
        };

        progressBar.Width = ActualWidth > 0 ? ActualWidth : 480;
        Visibility        = Visibility.Visible;

        var anim = new DoubleAnimation(progressBar.Width, 0, TimeSpan.FromSeconds(segundos));
        progressBar.BeginAnimation(FrameworkElement.WidthProperty, anim);

        _timer?.Stop();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(segundos) };
        _timer.Tick += (_, _) => { _timer.Stop(); Visibility = Visibility.Collapsed; };
        _timer.Start();
    }

    public void Hide()
    {
        _timer?.Stop();
        Visibility = Visibility.Collapsed;
    }
}
