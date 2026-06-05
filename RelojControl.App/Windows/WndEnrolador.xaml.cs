using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using proyectoNegocioRflex.Modelo;
using proyectoNegocioRflex.Utilidades;

namespace RelojControl.Windows;

public partial class WndEnrolador : Window
{
    private record PersonaItem(string RutEnc, string RutDisplay, string Nombre, string Puesto);
    private record MarcaRow(string FechaHora, string TipoMarca, string Reloj, string Incidencia);

    private readonly List<PersonaItem> _allPersonas = [];
    private string _rutSeleccionado = "";

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
