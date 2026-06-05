using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using proyectoNegocioRflex.Modelo;
using proyectoNegocioRflex.Utilidades;

namespace RelojControl.Windows;

public partial class WndInicio : Window
{
    private int    _idReloj, _idEmpresa, _idSucursal, _idResolucionMarca;
    private string _rutEmpresa = "", _nombreEmpresa = "", _codigoEmpresa = "";
    private string _nombreSucursal = "", _ubicacionReloj = "", _direccionSucursal = "";
    private bool   _permiteComida, _permiteJornada, _habilitado, _bloqueado;
    private bool   _iniciarDesdeMarca, _permitidoUso, _mostrarLoginInicio;
    private int    _soloEnrolar, _idTipoRolUsuario;

    private readonly DispatcherTimer _reloj = new() { Interval = TimeSpan.FromSeconds(1) };

    public WndInicio()
    {
        InitializeComponent();
_reloj.Tick += (_, _) => ActualizarReloj();
        _reloj.Start();
        ActualizarReloj();
        Loaded  += OnLoaded;
        KeyDown += OnKeyDown;
        txtPass.PasswordChanged += (s, e) =>
            lblPassPlaceholder.Visibility = txtPass.Password.Length == 0
                ? Visibility.Visible : Visibility.Collapsed;
        txtPass.GotFocus  += (s, e) => lblPassPlaceholder.Visibility = Visibility.Collapsed;
        txtPass.LostFocus += (s, e) => {
            if (txtPass.Password.Length == 0)
                lblPassPlaceholder.Visibility = Visibility.Visible;
        };
    }

    private void ActualizarReloj()
    {
        var now = DateTime.Now;
        lblHoraInicio.Text  = now.ToString("HH:mm");
        lblFechaInicio.Text = now.ToString("dddd, d 'de' MMMM",
            System.Globalization.CultureInfo.GetCultureInfo("es-CL"));
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var h = new Herramientas();
            if (!h.comprobarArchivosBase(false))
            {
                MostrarMsg("Reloj mal configurado", "Faltan archivos de configuración.");
                return;
            }
            ComprobarReloj();
            if (_bloqueado)
            {
                MostrarMsg("Reloj bloqueado", "Contactar con soporte rFlex.");
                return;
            }
            // Mostrar login admin si el reloj lo requiere (tipo 1=solo enrolador,
            // tipo 2=enrolador+asistencia) o si está configurado explícitamente
            if (_soloEnrolar >= 1 || _mostrarLoginInicio)
                cardLogin.Visibility = Visibility.Visible;
            if (_soloEnrolar == 1) return; // solo enrolador: no ir a marca
            if (_iniciarDesdeMarca && _permitidoUso) AbrirMarca();
        }
        catch (Exception ex)
        {
            MostrarMsg("Error de inicio", ex.Message);
        }
    }

    private void ComprobarReloj()
    {
        var r  = new Reloj();
        var dt = r.traerDatosRelojPorNombre(Environment.MachineName);
        if (dt == null || dt.Rows.Count == 0)
        {
            MostrarMsg("Sin configuración", "El reloj no está registrado.");
            _permitidoUso = false;
            return;
        }
        var row = dt.Rows[0];
        _idReloj           = int.Parse(row[0].ToString()!);
        _idEmpresa         = int.Parse(row[1].ToString()!);
        _ubicacionReloj    = row[4].ToString()!;
        _habilitado         = row[5].ToString()  == "1";
        _bloqueado          = row[14].ToString() == "1";
        _permiteComida      = row[9].ToString()  == "1";
        _permiteJornada     = row[10].ToString() == "1";
        _mostrarLoginInicio = row[11].ToString() == "1";
        _iniciarDesdeMarca  = row[12].ToString() == "1";
        _soloEnrolar        = int.Parse(row[18].ToString()!);
        _idResolucionMarca = int.Parse(row[20].ToString()!);

        lblTituloReloj.Text = $"Registro de Asistencia · {_ubicacionReloj}";

        ComprobarEmpresa();
        ComprobarSucursal();
        _permitidoUso = ComprobarTipoMarca() && ComprobarTipoRechazo() && ComprobarTipoInhabilitacion();
        cardMarca.Visibility = _permitidoUso ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ComprobarEmpresa()
    {
        var dt = new Empresa().traerDatosEmpresaPorID(_idEmpresa);
        if (dt == null || dt.Rows.Count == 0) return;
        _rutEmpresa    = dt.Rows[0][1].ToString()!;
        _nombreEmpresa = dt.Rows[0][2].ToString()!;
        _codigoEmpresa = dt.Rows[0][11].ToString()!;
    }

    private void ComprobarSucursal()
    {
        var dt = new Sucursal().traerSucursalPorIDReloj(_idReloj);
        if (dt == null || dt.Rows.Count == 0) return;
        _idSucursal        = int.Parse(dt.Rows[0][0].ToString()!);
        _nombreSucursal    = dt.Rows[0][2].ToString()!;
        _direccionSucursal = $"{dt.Rows[0][5]} {dt.Rows[0][6]} {dt.Rows[0][7]} {dt.Rows[0][8]}";
    }

    private bool ComprobarTipoMarca()          => new TipoMarca().traerTipoMarca()?.Rows.Count > 0;
    private bool ComprobarTipoRechazo()        => new TipoRechazo().traerTipoRechazo()?.Rows.Count > 0;
    private bool ComprobarTipoInhabilitacion() => new TipoInhabilitacionMarca().traerTiposInhabilitacionMarca()?.Rows.Count > 0;

    private void CardMarca_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => AbrirMarca();

    private void AbrirMarca()
    {
        var frm = new WndMarcaBajoTrafico
        {
            IdReloj           = _idReloj,
            IdEmpresa         = _idEmpresa,
            IdSucursal        = _idSucursal,
            PermiteComida     = _permiteComida,
            PermiteJornada    = _permiteJornada,
            NombreEmpresa     = _nombreEmpresa,
            NombreSucursal    = _nombreSucursal,
            UbicacionReloj    = _ubicacionReloj,
            DireccionSucursal = _direccionSucursal,
        };
        frm.Show();
        Hide();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPass.Password))
        { MostrarMsg("Iniciar sesión", "Ingrese usuario y contraseña."); return; }

        var enc = new Encriptacion();
        var p   = new Persona();
        var dt  = p.loginEnrolamiento(enc.Encriptar(txtUsuario.Text.ToUpper()), enc.Encriptar(txtPass.Password));
        if (dt == null || dt.Rows.Count == 0)
        { MostrarMsg("Iniciar sesión", "Usuario o contraseña incorrectos."); return; }

        _idTipoRolUsuario = int.Parse(dt.Rows[0][5].ToString()!);
        var panel = new WndPanelControl(_idTipoRolUsuario) { CodigoEmpresa = _codigoEmpresa };
        panel.Owner = this;
        panel.Show();
        Hide();
    }

    private void TxtPass_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return) BtnLogin_Click(sender, e);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.E && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            cardLogin.Visibility = cardLogin.Visibility == Visibility.Visible
                ? Visibility.Collapsed : Visibility.Visible;
    }

    private static void MostrarMsg(string titulo, string msg)
        => MessageBox.Show(msg, titulo);

    public void OnChildClosed() => Show();
}
