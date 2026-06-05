using System.Windows;
namespace RelojControl.Windows;
public partial class WndMarcaBajoTrafico : Window
{
    public int    IdReloj           { get; set; }
    public int    IdEmpresa         { get; set; }
    public int    IdSucursal        { get; set; }
    public bool   PermiteComida     { get; set; }
    public bool   PermiteJornada    { get; set; }
    public string NombreEmpresa     { get; set; } = "";
    public string NombreSucursal    { get; set; } = "";
    public string UbicacionReloj    { get; set; } = "";
    public string DireccionSucursal { get; set; } = "";
    public WndMarcaBajoTrafico() => InitializeComponent();
}
