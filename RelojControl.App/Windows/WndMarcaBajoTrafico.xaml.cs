using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DPFP.Gui.Verification;
using DPFP.Verification;
using proyectoNegocioRflex.Modelo;
using proyectoNegocioRflex.Utilidades;
using RelojControl.Controls;
using RelojControl.ViewModels;

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

    private readonly MarcaBajoTraficoViewModel _vm = new();
    private VerificationControl? _vc;
    private string _rutEncriptado  = "";
    private int    _numeroDedo;
    private readonly DispatcherTimer _resetTimer = new() { Interval = TimeSpan.FromSeconds(5) };

    public WndMarcaBajoTrafico()
    {
        InitializeComponent();
        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MarcaBajoTraficoViewModel.Step)) UpdatePanel();
        };
        _resetTimer.Tick += (_, _) => { _resetTimer.Stop(); _vm.Reset(); };
        DataContext = _vm;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        rail.Empresa   = NombreEmpresa;
        rail.Ubicacion = $"{NombreSucursal} · {UbicacionReloj}";
        IniciarCaptura();
        UpdatePanel();
    }

    private void Keypad_KeyPressed(object? sender, string key)
    {
        _vm.PressKey(key);
        lblRutDisplay.Text = _vm.RutDisplay;
        if (_vm.Step == MarcaStep.VerificandoHuella) BuscarTrabajador();
    }

    private void BuscarTrabajador()
    {
        fpRing.SetState(FingerprintState.Scanning);
        _vm.IsScanning = true;

        var enc = new Encriptacion();
        _rutEncriptado = enc.Encriptar(_vm.RutBuffer.ToUpper());

        var dt = new Persona().traerDatosTrabajadorConConfig(_rutEncriptado);
        if (dt == null || dt.Rows.Count == 0)
        {
            _vm.SetError("RUT no encontrado en el sistema.");
            return;
        }

        string nombre     = $"{enc.Desencriptar(dt.Rows[0][1].ToString()!)} " +
                            $"{enc.Desencriptar(dt.Rows[0][3].ToString()!)} " +
                            $"{enc.Desencriptar(dt.Rows[0][4].ToString()!)}";
        string ultimaMarca = ObtenerUltimaMarca();

        lblNombre.Text      = nombre;
        lblRutCard.Text     = FormatRut(_vm.RutBuffer);
        lblAvatar.Text      = GetInitials(nombre);
        lblUltimaMarca.Text = ultimaMarca;

        _vm.SetWorkerInfo(nombre, ultimaMarca);
    }

    private string ObtenerUltimaMarca()
    {
        try
        {
            var dt = new Marca().traerUltimaMarca(_rutEncriptado);
            if (dt == null || dt.Rows.Count == 0) return "Sin marcas previas";
            var tipo = dt.Rows[0][12].ToString()!;
            var hora = DateTime.Parse(dt.Rows[0][6].ToString()!).ToString("HH:mm");
            return $"Hoy {hora} · {tipo}";
        }
        catch { return ""; }
    }

    private void UpdatePanel()
    {
        Dispatcher.Invoke(() =>
        {
            panelRut.Visibility     = _vm.Step == MarcaStep.IngresoRut        ? Visibility.Visible : Visibility.Collapsed;
            panelHuella.Visibility  = _vm.Step == MarcaStep.VerificandoHuella ? Visibility.Visible : Visibility.Collapsed;
            panelSentido.Visibility = _vm.Step == MarcaStep.SeleccionSentido  ? Visibility.Visible : Visibility.Collapsed;
            panelExito.Visibility   = _vm.Step == MarcaStep.Exito             ? Visibility.Visible : Visibility.Collapsed;
            panelError.Visibility   = _vm.Step == MarcaStep.Error             ? Visibility.Visible : Visibility.Collapsed;

            if (_vm.Step == MarcaStep.Error)
            {
                lblMensajeError.Text = _vm.MensajeError;
            }
            if (_vm.Step == MarcaStep.IngresoRut) lblRutDisplay.Text = "";
        });
    }

    private void BtnSentido_Click(object sender, RoutedEventArgs e)
    {
        var tag     = ((System.Windows.Controls.Button)sender).Tag?.ToString() ?? "";
        int tipoId  = tag == "Entrada" ? ConfiguracionesConstantes.MARCA_ENTRADA : ConfiguracionesConstantes.MARCA_SALIDA;
        RegistrarMarca(tag, tipoId);
    }

    private void RegistrarMarca(string sentido, int tipoMarcaId)
    {
        try
        {
            var h    = new Herramientas();
            var fechaArr = h.obtenerHoraServidorConTimeZoneParaMarca(false);

            bool ok = new Marca().agregarMarca(
                tipoMarcaId,
                _rutEncriptado,
                IdEmpresa,
                IdSucursal,
                _numeroDedo,
                _rutEncriptado,
                _rutEncriptado,
                IdReloj,
                0,
                0,
                fechaArr);

            if (!ok) { _vm.SetError("Error al guardar la marca en la base de datos."); return; }

            lblNombreExito.Text  = _vm.NombreTrabajador;
            lblSentidoExito.Text = $"Jornada · {sentido} · {DateTime.Now:HH:mm}";
            _vm.SetExito();
            _resetTimer.Start();
        }
        catch (Exception ex)
        {
            _vm.SetError($"Error al guardar: {ex.Message}");
        }
    }

    private void BtnVolver_Click(object sender, RoutedEventArgs e)     => _vm.Reset();
    private void BtnReintentar_Click(object sender, RoutedEventArgs e) => _vm.Reset();

    private void IniciarCaptura()
    {
        if (_vc != null) return;
        try
        {
            _vc = new VerificationControl();
            _vc.OnComplete += new VerificationControl._OnComplete(OnFingerprintComplete);
            fpHost.Child = _vc;  // HWND created first
            _vc.Active   = true; // activate after HWND exists
        }
        catch { }
    }

    private void DetenerCaptura()
    {
        if (_vc == null) return;
        try { _vc.Active = false; } catch { }
        fpHost.Child = null;
        _vc = null;
    }


    private void OnFingerprintComplete(object Control, DPFP.FeatureSet featureSet,
                                       ref DPFP.Gui.EventHandlerStatus status)
    {
        Dispatcher.BeginInvoke(() =>
        {
            if (_vm.Step != MarcaStep.VerificandoHuella) return;
            VerificarHuella(featureSet);
        });
    }

    private void VerificarHuella(DPFP.FeatureSet featureSet)
    {
        try
        {
            var huellas = new ImagenHuella().traerHuellaPorRutPersona(_rutEncriptado);
            if (huellas == null || huellas.Rows.Count == 0)
            { _vm.SetError("Sin huellas registradas. Contacte al administrador."); return; }

            var verificador = new Verification();
            foreach (DataRow row in huellas.Rows)
            {
                using var ms     = new MemoryStream((byte[])row[2]);
                var template     = new DPFP.Template(ms);
                var result       = new Verification.Result();
                verificador.Verify(featureSet, template, ref result);
                if (result.Verified)
                {
                    _numeroDedo = int.Parse(row[1].ToString()!);
                    _vm.SetTrabajador(_vm.NombreTrabajador, _vm.UltimaMarca);
                    return;
                }
            }
            _vm.SetError("Huella no reconocida. Intente nuevamente.");
        }
        catch (Exception ex) { _vm.SetError(ex.Message); }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
        { Close(); return; }

        if (_vm.Step != MarcaStep.IngresoRut) return;

        string? key = e.Key switch
        {
            Key.D0 or Key.NumPad0 => "0",
            Key.D1 or Key.NumPad1 => "1",
            Key.D2 or Key.NumPad2 => "2",
            Key.D3 or Key.NumPad3 => "3",
            Key.D4 or Key.NumPad4 => "4",
            Key.D5 or Key.NumPad5 => "5",
            Key.D6 or Key.NumPad6 => "6",
            Key.D7 or Key.NumPad7 => "7",
            Key.D8 or Key.NumPad8 => "8",
            Key.D9 or Key.NumPad9 => "9",
            Key.K                 => "K",
            Key.Back or Key.Delete => "DEL",
            Key.Return or Key.Enter => "GO",
            _ => null
        };

        if (key == null) return;
        e.Handled = true;
        Keypad_KeyPressed(null, key);
    }

    protected override void OnClosed(EventArgs e)
    {
        DetenerCaptura();
        base.OnClosed(e);
    }

    private static string FormatRut(string raw)
    {
        if (raw.Length < 2) return raw;
        var num = raw[..^1]; var dv = raw[^1];
        return long.TryParse(num, out var n)
            ? $"{n:N0}-{dv}".Replace(",", ".")
            : $"{num}-{dv}";
    }

    private static string GetInitials(string nombre)
    {
        var parts = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? $"{parts[0][0]}{parts[1][0]}" : nombre[..Math.Min(2, nombre.Length)];
    }
}
