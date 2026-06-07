using System.Windows;
using System.Windows.Input;

namespace RelojControl.Windows;

public partial class WndEnrolador : Window
{
    public int    IdEmpresa     { get => uc.IdEmpresa;     set => uc.IdEmpresa     = value; }
    public int    IdSucursal    { get => uc.IdSucursal;    set => uc.IdSucursal    = value; }
    public string CodigoEmpresa { get => uc.CodigoEmpresa; set => uc.CodigoEmpresa = value; }

    public WndEnrolador(int idReloj = 0)
    {
        InitializeComponent();
        Loaded += (_, __) => uc.SetEnrollmentHost(ecHost);
        uc.SolicitaCerrar += (_, __) => Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close();
    }
}
