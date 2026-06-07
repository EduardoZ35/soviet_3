using System.Windows;
using System.Windows.Controls;

namespace RelojControl.Windows;

public partial class WndPanelControl : Window
{
    private readonly int _idReloj;
    public string CodigoEmpresa { get; set; } = "";

    public WndPanelControl(int idRol = 0, int idReloj = 0)
    {
        InitializeComponent();
        _idReloj = idReloj;
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
            mainContent.Content = new TextBlock
            {
                Text = "Selecciona una opción del menú lateral",
                FontFamily = (System.Windows.Media.FontFamily)FindResource("FontPoppins"),
                FontSize = 15,
                Foreground = (System.Windows.Media.Brush)FindResource("Ink3Brush"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
            };
        };
        mainContent.Content = uc;
    }
}
