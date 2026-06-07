using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using DPFP.Gui.Enrollment;
using proyectoNegocioRflex.Modelo;
using proyectoNegocioRflex.Utilidades;
using RelojControl.Windows;

namespace RelojControl.Controls;

public partial class UcEnrolador : UserControl
{
    private record MarcaRow(string FechaHora, string TipoMarca, string Reloj, string Incidencia);

    private string                _rutSeleccionado    = "";
    private EnrollmentControl?    _ec;
    private int                   _fingerSeleccionado = -1;
    private readonly HashSet<int> _enrolledDedos      = [];

    public int              IdReloj       { get; set; }
    public int              IdEmpresa     { get; set; }
    public int              IdSucursal    { get; set; }
    public string           CodigoEmpresa { get; set; } = "";
    public WindowsFormsHost? EcHost       { get; set; }

    public event System.EventHandler? CloseRequested;

    public UcEnrolador()
    {
        InitializeComponent();
        Unloaded += (_, _) => PararEnrollment();
    }

    private void BuscarPorRut()
    {
        var rut = txtBusqueda.Text.Trim().Replace(".", "").ToUpper();
        if (string.IsNullOrEmpty(rut)) return;

        var enc    = new Encriptacion();
        var rutEnc = enc.Encriptar(rut);
        var dt     = new Persona().traerDatosTrabajadorConConfig(rutEnc);

        if (dt == null || dt.Rows.Count == 0)
        {
            panelLista.Children.Clear();
            panelLista.Children.Add(new TextBlock
            {
                Text       = $"RUT '{txtBusqueda.Text.Trim()}' no encontrado.",
                FontFamily = (System.Windows.Media.FontFamily)FindResource("FontPoppins"),
                FontSize   = 14,
                Foreground = (System.Windows.Media.Brush)FindResource("CoralBrush"),
                Margin     = new Thickness(0, 8, 0, 0),
            });
            return;
        }

        CargarDetalle(rutEnc);
    }

    private void CargarDetalle(string rutEnc)
    {
        _rutSeleccionado = rutEnc;
        try
        {
            var enc = new Encriptacion();
            var dt  = new Persona().traerDatosTrabajadorConConfig(rutEnc);
            if (dt == null || dt.Rows.Count == 0) return;

            var row = dt.Rows[0];
            string nombre = $"{enc.Desencriptar(row[1].ToString()!)} " +
                            $"{enc.Desencriptar(row[3].ToString()!)} " +
                            $"{enc.Desencriptar(row[4].ToString()!)}";
            string rutDec = enc.Desencriptar(rutEnc);

            lblNombreDetalle.Text = nombre.Trim();
            lblRutDetalle.Text    = FormatRut(rutDec);
            lblAvatar.Text        = GetInitials(nombre.Trim());
            lblWorkerPill.Text    = nombre.Trim();
            pillWorker.Visibility = Visibility.Visible;

            lblDatNombre.Text  = nombre.Trim();
            lblDatRut.Text     = FormatRut(rutDec);
            lblDatCorreo.Text  = enc.Desencriptar(row[5].ToString()!);
            lblDatPuesto.Text  = enc.Desencriptar(row[18].ToString()!);
            lblDatCentro.Text  = enc.Desencriptar(row[17].ToString()!);
            lblDatActivo.Text  = row[10].ToString()!;

            chkHabilitada.IsChecked = row[11].ToString() == "1";
            chkEnrolar.IsChecked    = row[12].ToString() == "1";
            chkAsistencia.IsChecked = row[13].ToString() == "1";

            SwitchTab(0);
            scrollPersonas.Visibility = Visibility.Collapsed;
            gridDetalle.Visibility    = Visibility.Visible;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar detalle: {ex.Message}", "Error");
        }
    }

    private void CargarAsistencia()
    {
        try
        {
            string fechaFin    = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string fechaInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
            var dt = new Marca().traerMarcasPorTrabajadorYFecha(_rutSeleccionado, fechaInicio, fechaFin);

            var rows = new List<MarcaRow>();
            if (dt != null)
                foreach (DataRow row in dt.Rows)
                {
                    string fecha = DateTime.TryParse(row[6].ToString(), out var d)
                        ? d.ToString("dd/MM/yyyy HH:mm") : row[6].ToString()!;
                    rows.Add(new MarcaRow(fecha, row[12].ToString()!, row[13].ToString()!, row[15].ToString()!));
                }
            dgMarcas.ItemsSource = rows;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar asistencia: {ex.Message}", "Error");
        }
    }

    private void SwitchTab(int idx)
    {
        if (tabHuellas.Visibility == Visibility.Visible && idx != 2)
            PararEnrollment();

        tabDatos.Visibility      = idx == 0 ? Visibility.Visible : Visibility.Collapsed;
        tabAsistencia.Visibility = idx == 1 ? Visibility.Visible : Visibility.Collapsed;
        tabHuellas.Visibility    = idx == 2 ? Visibility.Visible : Visibility.Collapsed;
        tabComidas.Visibility    = idx == 3 ? Visibility.Visible : Visibility.Collapsed;
        tabRelojes.Visibility    = idx == 4 ? Visibility.Visible : Visibility.Collapsed;

        btnTab0.Style = (Style)FindResource(idx == 0 ? "TabBtnActive" : "TabBtn");
        btnTab1.Style = (Style)FindResource(idx == 1 ? "TabBtnActive" : "TabBtn");
        btnTab2.Style = (Style)FindResource(idx == 2 ? "TabBtnActive" : "TabBtn");
        btnTab3.Style = (Style)FindResource(idx == 3 ? "TabBtnActive" : "TabBtn");
        btnTab4.Style = (Style)FindResource(idx == 4 ? "TabBtnActive" : "TabBtn");

        if (idx == 1) CargarAsistencia();
        if (idx == 2) IniciarEnrollment();
        if (idx == 3) CargarComidas();
        if (idx == 4) CargarRelojes();
    }

    private void CargarComidas()
    {
        var items = new System.Collections.ObjectModel.ObservableCollection<TipoComidaItem>();
        try
        {
            var dt = new SucursalTipoComida().traerSucursalTipoComidaLocalPorEstadoYSucursal(1, IdSucursal);
            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    items.Add(new TipoComidaItem
                    {
                        Id        = int.Parse(row[0].ToString()!),
                        Nombre    = row[5].ToString()!,
                        Horario   = $"{row[2]} — {row[3]}",
                        Valor     = $"{row[6]} días",
                        Habilitado= row[4].ToString() == "1",
                    });
        }
        catch { }
        listaTiposComida.ItemsSource = items;
    }

    private void CargarRelojes()
    {
        var items = new System.Collections.ObjectModel.ObservableCollection<RelojItem>();
        try
        {
            var dt = new Reloj().traerRelojesEmpresa();
            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    items.Add(new RelojItem
                    {
                        Id        = int.Parse(row[0].ToString()!),
                        Nombre    = row[3].ToString()!,
                        Ubicacion = row[4].ToString()!,
                    });
        }
        catch { }
        listaRelojes.ItemsSource = items;
    }

    private void CargarHuellasEnroladas()
    {
        _enrolledDedos.Clear();
        try
        {
            var dt = new ImagenHuella().traerHuellaPorRutPersona(_rutSeleccionado);
            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    if (int.TryParse(row[1].ToString(), out int nd))
                        _enrolledDedos.Add(nd);
        }
        catch { }
        UpdateFingerButtons(_fingerSeleccionado);
        lblHuellaCount.Text = $"{_enrolledDedos.Count}/10 huellas enroladas";
    }

    private void IniciarEnrollment()
    {
        if (EcHost == null) return;
        CargarHuellasEnroladas();
        _fingerSeleccionado  = -1;
        lblEnrollStatus.Text = "← Selecciona un dedo para comenzar.";

        try
        {
            _ec = new EnrollmentControl();
            _ec.EnrolledFingerMask   = 0;
            _ec.MaxEnrollFingerCount = 1;
            _ec.OnEnroll        += new EnrollmentControl._OnEnroll(OnEnrollComplete);
            _ec.OnSampleQuality += new EnrollmentControl._OnSampleQuality(OnSampleQuality);
            EcHost.Child = _ec;
            EcHost.HorizontalAlignment = HorizontalAlignment.Right;
            EcHost.VerticalAlignment   = VerticalAlignment.Bottom;
            EcHost.Margin              = new Thickness(0, 0, 20, 20);
        }
        catch (Exception ex)
        {
            lblEnrollStatus.Text = $"Error al iniciar lector: {ex.Message}";
        }
    }

    private void PararEnrollment()
    {
        if (_ec != null && EcHost != null)
        {
            EcHost.Child = null;
            _ec          = null;
        }
        if (EcHost != null)
        {
            EcHost.HorizontalAlignment = HorizontalAlignment.Left;
            EcHost.VerticalAlignment   = VerticalAlignment.Top;
            EcHost.Margin              = new Thickness(-1000, -1000, 0, 0);
        }
    }

    private void OnEnrollComplete(object Control, int Finger, DPFP.Template Template,
                                  ref DPFP.Gui.EventHandlerStatus Status)
    {
        Dispatcher.Invoke(() =>
        {
            try
            {
                int numeroDedo = Finger - 1;
                using var ms = new MemoryStream();
                Template.Serialize(ms);
                GuardarHuella(numeroDedo, ms.ToArray());
            }
            catch (Exception ex)
            {
                lblEnrollStatus.Text = $"Error al guardar huella: {ex.Message}";
            }
        });
    }

    private void OnSampleQuality(object Control, string ReaderSerialNumber, int Finger,
                                 DPFP.Capture.CaptureFeedback CaptureFeedback)
    {
        Dispatcher.Invoke(() =>
        {
            lblEnrollStatus.Text = CaptureFeedback == DPFP.Capture.CaptureFeedback.Good
                ? $"Muestra capturada. Sigue escaneando el dedo {_fingerSeleccionado}."
                : $"Calidad insuficiente ({CaptureFeedback}). Vuelve a colocar el dedo.";
        });
    }

    private void GuardarHuella(int numeroDedo, byte[] bytes)
    {
        var ih       = new ImagenHuella();
        var existing = ih.traerHuellaPorRutYNumeroDedo(_rutSeleccionado, numeroDedo);
        bool ok;
        if (existing != null && existing.Rows.Count > 0)
        {
            int id = int.Parse(existing.Rows[0][0].ToString()!);
            ok = ih.editarHuella(bytes, _rutSeleccionado, 0, id);
        }
        else
        {
            ok = ih.guardarHuella(
                IdEmpresa,
                _rutSeleccionado,
                numeroDedo,
                bytes,
                _rutSeleccionado,
                _rutSeleccionado,
                0,
                Environment.MachineName,
                DateTime.Now,
                DateTime.Now);
        }
        if (ok) _enrolledDedos.Add(numeroDedo);
        lblEnrollStatus.Text     = ok
            ? $"✓ Dedo {numeroDedo + 1} enrolado. Selecciona otro dedo o cierra."
            : "Error al guardar la huella en la base de datos.";
        _fingerSeleccionado  = -1;
        lblHuellaCount.Text  = $"{_enrolledDedos.Count}/10 huellas enroladas";
        UpdateFingerButtons(-1);
    }

    private void BtnDedo_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_rutSeleccionado) || _ec == null) return;
        int finger = int.Parse(((Button)sender).Tag?.ToString() ?? "1");
        _fingerSeleccionado  = finger;
        UpdateFingerButtons(finger);
        lblEnrollStatus.Text = $"Dedo {finger} seleccionado — escanea 4 veces para enrolar.";
    }

    private void BtnReiniciarEnroll_Click(object sender, RoutedEventArgs e)
    {
        PararEnrollment();
        IniciarEnrollment();
    }

    private void UpdateFingerButtons(int activeFinger)
    {
        for (int i = 1; i <= 10; i++)
        {
            var btn = (Button?)FindName($"btnDedo{i}");
            if (btn == null) continue;
            bool enrolled = _enrolledDedos.Contains(i - 1);
            string style = (i == activeFinger)
                ? (enrolled ? "FingerBtnEnrolledActive" : "FingerBtnActive")
                : (enrolled ? "FingerBtnEnrolled"       : "FingerBtn");
            btn.Style = (Style)FindResource(style);
        }
    }

    private void BtnTab_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(((Button)sender).Tag?.ToString(), out int idx))
            SwitchTab(idx);
    }

    private void BtnVolver_Click(object sender, RoutedEventArgs e)
    {
        PararEnrollment();
        _rutSeleccionado          = "";
        pillWorker.Visibility     = Visibility.Collapsed;
        gridDetalle.Visibility    = Visibility.Collapsed;
        scrollPersonas.Visibility = Visibility.Visible;
    }

    private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
    {
        lblHintBusqueda.Visibility = string.IsNullOrEmpty(txtBusqueda.Text)
            ? Visibility.Visible : Visibility.Collapsed;
    }

    private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return) BuscarPorRut();
    }

    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        PararEnrollment();
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private static string FormatRut(string raw)
    {
        if (string.IsNullOrEmpty(raw) || raw.Length < 2) return raw;
        if (raw.Contains('-'))
        {
            var parts = raw.Split('-');
            if (long.TryParse(parts[0], out var n))
                return $"{n:N0}-{parts[1]}".Replace(",", ".");
            return raw;
        }
        var num = raw[..^1]; var dv = raw[^1];
        return long.TryParse(num, out var nn)
            ? $"{nn:N0}-{dv}".Replace(",", ".")
            : $"{num}-{dv}";
    }

    private static string GetInitials(string nombre)
    {
        var parts = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}"
            : nombre[..Math.Min(2, nombre.Length)];
    }
}
