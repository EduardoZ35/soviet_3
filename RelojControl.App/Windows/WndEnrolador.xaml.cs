using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DPFP.Gui.Enrollment;
using proyectoNegocioRflex.Modelo;
using proyectoNegocioRflex.Utilidades;

namespace RelojControl.Windows;

public partial class WndEnrolador : Window
{
    private record PersonaItem(string RutEnc, string RutDisplay, string Nombre, string Puesto);
    private record MarcaRow(string FechaHora, string TipoMarca, string Reloj, string Incidencia);

    private readonly List<PersonaItem> _allPersonas = [];
    private string                   _rutSeleccionado    = "";
    private EnrollmentControl?       _ec;
    private int                      _fingerSeleccionado = -1;

    public int IdEmpresa { get; set; }

    public WndEnrolador()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CargarPersonas();
    }

    private void CargarPersonas()
    {
        try
        {
            var enc = new Encriptacion();
            var dt  = new Persona().traerDatosPersonasDesdeLocal();
            if (dt == null) return;

            _allPersonas.Clear();
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    string rutEnc    = row[1].ToString()!;
                    string rutDec    = enc.Desencriptar(rutEnc);
                    string nombre    = $"{enc.Desencriptar(row[2].ToString()!)} " +
                                       $"{enc.Desencriptar(row[4].ToString()!)} " +
                                       $"{enc.Desencriptar(row[5].ToString()!)}";
                    string puesto    = enc.Desencriptar(row[7].ToString()!);
                    _allPersonas.Add(new PersonaItem(rutEnc, FormatRut(rutDec), nombre.Trim(), puesto));
                }
                catch { }
            }
            MostrarLista(_allPersonas);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar personas: {ex.Message}", "Error");
        }
    }

    private void MostrarLista(IEnumerable<PersonaItem> items)
    {
        panelLista.Children.Clear();
        foreach (var p in items)
        {
            var card = BuildPersonaCard(p);
            panelLista.Children.Add(card);
        }
    }

    private Border BuildPersonaCard(PersonaItem p)
    {
        var border = new Border
        {
            Background    = (System.Windows.Media.Brush)FindResource("SurfaceBrush"),
            CornerRadius  = new CornerRadius(14),
            Margin        = new Thickness(0, 0, 0, 8),
            Padding       = new Thickness(20, 14, 20, 14),
            Effect        = (System.Windows.Media.Effects.Effect)FindResource("ShadowCard"),
            Cursor        = Cursors.Hand,
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var lblNombre = new TextBlock
        {
            Text        = p.Nombre,
            FontFamily  = (System.Windows.Media.FontFamily)FindResource("FontPoppins"),
            FontWeight  = FontWeights.SemiBold,
            FontSize    = 14,
            Foreground  = (System.Windows.Media.Brush)FindResource("InkBrush"),
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(lblNombre, 0);

        var lblRut = new TextBlock
        {
            Text       = p.RutDisplay,
            FontFamily = (System.Windows.Media.FontFamily)FindResource("FontManrope"),
            FontSize   = 13,
            Foreground = (System.Windows.Media.Brush)FindResource("Ink3Brush"),
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(lblRut, 1);

        var btnVer = new Button
        {
            Content   = "Ver detalle →",
            Tag       = p.RutEnc,
            Style     = (Style)FindResource("BtnAccent"),
        };
        btnVer.Click += BtnVerDetalle_Click;
        Grid.SetColumn(btnVer, 2);

        grid.Children.Add(lblNombre);
        grid.Children.Add(lblRut);
        grid.Children.Add(btnVer);
        border.Child = grid;
        return border;
    }

    private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
    {
        var rutEnc = ((Button)sender).Tag?.ToString() ?? "";
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

            lblNombreDetalle.Text    = nombre.Trim();
            lblRutDetalle.Text       = FormatRut(rutDec);
            lblAvatar.Text           = GetInitials(nombre.Trim());
            lblWorkerPill.Text       = nombre.Trim();
            pillWorker.Visibility    = Visibility.Visible;

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
            {
                foreach (DataRow row in dt.Rows)
                {
                    string fecha       = DateTime.TryParse(row[6].ToString(), out var d)
                                         ? d.ToString("dd/MM/yyyy HH:mm") : row[6].ToString()!;
                    string incidencia  = row[15].ToString()!;
                    rows.Add(new MarcaRow(fecha, row[12].ToString()!, row[13].ToString()!, incidencia));
                }
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
    }

    private void IniciarEnrollment()
    {
        try
        {
            _fingerSeleccionado = -1;
            UpdateFingerButtons(-1);
            lblEnrollStatus.Text = "Selecciona un dedo para comenzar el enrolamiento.";

            _ec = new EnrollmentControl();
            _ec.EnrolledFingerMask   = 0;
            _ec.MaxEnrollFingerCount = 1;
            _ec.OnEnroll        += new EnrollmentControl._OnEnroll(OnEnrollComplete);
            _ec.OnSampleQuality += new EnrollmentControl._OnSampleQuality(OnSampleQuality);
            ecHost.Child = _ec;
        }
        catch (Exception ex)
        {
            lblEnrollStatus.Text = $"Error al iniciar lector: {ex.Message}";
        }
    }

    private void PararEnrollment()
    {
        if (_ec != null)
        {
            ecHost.Child = null;
            _ec = null;
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
        lblEnrollStatus.Text = ok
            ? $"Dedo {numeroDedo + 1} enrolado correctamente."
            : "Error al guardar la huella en la base de datos.";
        UpdateFingerButtons(-1);
        _fingerSeleccionado = -1;
    }

    private void BtnDedo_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_rutSeleccionado) || _ec == null) return;
        int finger = int.Parse(((Button)sender).Tag?.ToString() ?? "1");
        _fingerSeleccionado = finger;
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
            if (btn != null)
                btn.Style = (Style)FindResource(i == activeFinger ? "FingerBtnActive" : "FingerBtn");
        }
    }

    private void BtnTab_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(((Button)sender).Tag?.ToString(), out int idx))
            SwitchTab(idx);
    }

    private void BtnVolver_Click(object sender, RoutedEventArgs e)
    {
        _rutSeleccionado           = "";
        pillWorker.Visibility      = Visibility.Collapsed;
        gridDetalle.Visibility     = Visibility.Collapsed;
        scrollPersonas.Visibility  = Visibility.Visible;
    }

    private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
    {
        lblHintBusqueda.Visibility = string.IsNullOrEmpty(txtBusqueda.Text)
            ? Visibility.Visible : Visibility.Collapsed;
        FiltrarLista();
    }

    private void FiltrarLista()
    {
        var q = txtBusqueda.Text.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(q)) { MostrarLista(_allPersonas); return; }
        var filtered = _allPersonas.FindAll(p =>
            p.Nombre.ToLowerInvariant().Contains(q) ||
            p.RutDisplay.ToLowerInvariant().Contains(q));
        MostrarLista(filtered);
    }

    private void BtnCerrar_Click(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        PararEnrollment();
        base.OnClosed(e);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close();
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
        return parts.Length >= 2 ? $"{parts[0][0]}{parts[1][0]}" : nombre[..Math.Min(2, nombre.Length)];
    }
}
