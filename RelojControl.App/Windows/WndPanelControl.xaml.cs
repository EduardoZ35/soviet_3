using System.Windows;
namespace RelojControl.Windows;
public partial class WndPanelControl : Window
{
    public string CodigoEmpresa { get; set; } = "";
    public WndPanelControl(int idRol = 0, int idReloj = 0) => InitializeComponent();
}
