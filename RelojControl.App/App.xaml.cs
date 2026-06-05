using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using RelojControl.Windows;

namespace RelojControl;

public partial class App : System.Windows.Application
{
    private NotifyIcon?      _tray;
    private DispatcherTimer? _syncTimer;
    private Icon?            _iconGreen, _iconYellow, _iconRed;

    private const string SyncStatusFile = @"C:\rflexapps\sync_status.txt";
    private const string ServiceName    = "SincronizadorRflex";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        InitSyncMonitor();
        var inicio = new WndInicio();
        inicio.Show();
    }

    private void InitSyncMonitor()
    {
        try
        {
            _iconGreen  = CreateColorIcon(Color.FromArgb(0, 180, 0));
            _iconYellow = CreateColorIcon(Color.FromArgb(220, 180, 0));
            _iconRed    = CreateColorIcon(Color.FromArgb(200, 0, 0));

            var menu        = new ContextMenuStrip();
            var itemRestart = new ToolStripMenuItem("Reiniciar sincronizador");
            itemRestart.Click += (_, _) => ReiniciarSincronizador();
            menu.Items.Add(itemRestart);

            _tray = new NotifyIcon
            {
                Icon             = _iconGreen,
                Text             = "rFlex Sincronizador — verificando...",
                ContextMenuStrip = menu,
                Visible          = true,
            };

            _syncTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _syncTimer.Tick += (_, _) => ActualizarEstadoSync();
            _syncTimer.Start();
            ActualizarEstadoSync();
        }
        catch { }
    }

    private void ActualizarEstadoSync()
    {
        try
        {
            bool procesoVivo     = Process.GetProcessesByName("sincronizadorServicio").Length > 0;
            string estado        = File.Exists(SyncStatusFile) ? File.ReadAllText(SyncStatusFile).Trim() : "";
            DateTime ultimaEscri = File.Exists(SyncStatusFile) ? File.GetLastWriteTime(SyncStatusFile) : DateTime.MinValue;
            double minSinActiv   = (DateTime.Now - ultimaEscri).TotalMinutes;
            bool detenido        = estado.StartsWith("Detenido") || !procesoVivo;
            bool bloqueado       = procesoVivo && minSinActiv > 10;

            var (icono, texto) = detenido  ? (_iconRed!,    "rFlex Sinc — DETENIDO")
                               : bloqueado ? (_iconYellow!, $"rFlex Sinc — SIN ACTIV {minSinActiv:F0}min")
                               :             (_iconGreen!,  "rFlex Sinc — OK");

            if (_tray != null)
            {
                _tray.Icon = icono;
                _tray.Text = texto.Length > 63 ? texto[..60] + "..." : texto;
            }
        }
        catch { }
    }

    private void ReiniciarSincronizador()
    {
        try
        {
            Process.Start(new ProcessStartInfo("cmd.exe")
            {
                Arguments       = $"/c net stop {ServiceName} & net start {ServiceName}",
                Verb            = "runas",
                UseShellExecute = true,
                WindowStyle     = ProcessWindowStyle.Hidden,
            });
        }
        catch { }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _syncTimer?.Stop();
        if (_tray != null) { _tray.Visible = false; _tray.Dispose(); }
        _iconGreen?.Dispose(); _iconYellow?.Dispose(); _iconRed?.Dispose();
        base.OnExit(e);
    }

    private static Icon CreateColorIcon(Color c)
    {
        using var bmp = new Bitmap(16, 16);
        using var g   = Graphics.FromImage(bmp);
        g.Clear(Color.Transparent);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(c);
        g.FillEllipse(brush, 1, 1, 13, 13);
        return Icon.FromHandle(bmp.GetHicon());
    }
}
