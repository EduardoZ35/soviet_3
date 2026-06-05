using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RelojControl.Controls;

public partial class RailControl : UserControl
{
    private readonly DispatcherTimer _clock = new() { Interval = TimeSpan.FromSeconds(1) };

    public static readonly DependencyProperty EmpresaProperty =
        DependencyProperty.Register(nameof(Empresa), typeof(string), typeof(RailControl),
            new PropertyMetadata("", (d, _) => ((RailControl)d).lblEmpresa.Text = (string)d.GetValue(EmpresaProperty)));

    public static readonly DependencyProperty UbicacionProperty =
        DependencyProperty.Register(nameof(Ubicacion), typeof(string), typeof(RailControl),
            new PropertyMetadata("", (d, _) => ((RailControl)d).lblUbicacion.Text = (string)d.GetValue(UbicacionProperty)));

    public string Empresa   { get => (string)GetValue(EmpresaProperty);   set => SetValue(EmpresaProperty, value); }
    public string Ubicacion { get => (string)GetValue(UbicacionProperty); set => SetValue(UbicacionProperty, value); }

    public RailControl()
    {
        InitializeComponent();
        _clock.Tick += (_, _) => ActualizarReloj();
        Loaded   += (_, _) => { ActualizarReloj(); _clock.Start(); };
        Unloaded += (_, _) => _clock.Stop();
    }

    private void ActualizarReloj()
    {
        var now = DateTime.Now;
        lblHora.Text     = now.ToString("HH:mm");
        lblSegundos.Text = now.ToString("ss");
        lblFecha.Text    = now.ToString("dddd, d 'de' MMMM",
                               System.Globalization.CultureInfo.GetCultureInfo("es-CL"));
    }

    public void SetSyncStatus(bool online)
    {
        syncDot.Fill = new System.Windows.Media.SolidColorBrush(
            online ? System.Windows.Media.Color.FromRgb(0x34, 0xB3, 0xAB)
                   : System.Windows.Media.Color.FromRgb(0xF7, 0x6D, 0x6D));
        lblSync.Text = online ? "Sincronizado" : "Sin conexión";
    }
}
