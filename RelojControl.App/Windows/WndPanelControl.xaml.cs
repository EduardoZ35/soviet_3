using System.Windows;

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
        var enrolador = new WndEnrolador(_idReloj) { CodigoEmpresa = CodigoEmpresa, Owner = this };
        enrolador.Show();
        Hide();
    }
}
