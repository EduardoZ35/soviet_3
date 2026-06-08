using System.Windows;
using System.Windows.Controls;

namespace RelojControl.Windows;

public partial class WndPanelControl : Window
{
    private readonly int _idReloj;
    public string CodigoEmpresa  { get; set; } = "";
    public string OperadorNombre { get; set; } = "Administrador";

    public WndPanelControl(int idRol = 0, int idReloj = 0)
    {
        InitializeComponent();
        _idReloj = idReloj;
        Loaded += (_, __) =>
        {
            lblChipIniciales.Text = GetInitials(OperadorNombre);
            BtnEnrolamiento_Click(this, new RoutedEventArgs());
        };
    }

    private static string GetInitials(string nombre)
    {
        var parts = nombre.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}"
            : nombre[..System.Math.Min(2, nombre.Length)];
    }

    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
        if (Owner is WndInicio inicio) inicio.OnChildClosed();
    }

    private void BtnEnrolamiento_Click(object sender, RoutedEventArgs e)
    {
        var uc = new RelojControl.Controls.UcEnrolador();
        uc.CodigoEmpresa = CodigoEmpresa;
        uc.SetEnrollmentHost(ecHost);
        uc.SolicitaCerrar += (_, __) =>
        {
            ecHost.Child = null;
            ecHost.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            ecHost.VerticalAlignment   = System.Windows.VerticalAlignment.Top;
            ecHost.Margin              = new System.Windows.Thickness(-1000, -1000, 0, 0);
            mainContent.Content        = BuildPlaceholder();
        };
        var vb = new Viewbox { Stretch = System.Windows.Media.Stretch.Uniform };
        vb.Child = uc;
        mainContent.Content = vb;
    }

    private object BuildPlaceholder()
    {
        var icon = new System.Windows.Shapes.Path
        {
            Stroke          = (System.Windows.Media.Brush)FindResource("AccentBrush"),
            StrokeThickness = 1.5,
            StrokeStartLineCap = System.Windows.Media.PenLineCap.Round,
            StrokeEndLineCap   = System.Windows.Media.PenLineCap.Round,
            Width   = 28,
            Height  = 28,
            Stretch = System.Windows.Media.Stretch.Uniform,
            Data    = System.Windows.Media.Geometry.Parse(
                "M12 12C14.2 12 16 10.2 16 8C16 5.8 14.2 4 12 4C9.8 4 8 5.8 8 8C8 10.2 9.8 12 12 12Z " +
                "M5 20C5 16.4 8.1 14.4 12 14.4C15.9 14.4 19 16.4 19 20"),
        };
        var canvas = new System.Windows.Controls.Canvas { Width = 24, Height = 24 };
        canvas.Children.Add(icon);
        var vb = new Viewbox { Width = 28, Height = 28, Margin = new System.Windows.Thickness(0, 0, 0, 14) };
        var bg = new Border
        {
            Width        = 64,
            Height       = 64,
            CornerRadius = new CornerRadius(20),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            Margin = new System.Windows.Thickness(0, 0, 0, 14),
            Background = new System.Windows.Media.LinearGradientBrush(
                System.Windows.Media.Color.FromRgb(0xEC, 0xED, 0xFB),
                System.Windows.Media.Color.FromRgb(0xD6, 0xD8, 0xF8), 45),
            Child = new Viewbox { Width = 28, Height = 28, Child = canvas },
        };
        var lbl = new TextBlock
        {
            Text      = "Selecciona una opción del menú lateral",
            FontFamily= (System.Windows.Media.FontFamily)FindResource("FontPoppins"),
            FontSize  = 15,
            Foreground= (System.Windows.Media.Brush)FindResource("Ink3Brush"),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
        };
        var sp = new StackPanel
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            VerticalAlignment   = System.Windows.VerticalAlignment.Center,
        };
        sp.Children.Add(bg);
        sp.Children.Add(lbl);
        return sp;
    }
}
