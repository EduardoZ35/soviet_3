using System.Windows;
using RelojControl.Windows;

namespace RelojControl;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        var inicio = new WndInicio();
        inicio.Show();
    }
}
