using CommunityToolkit.Mvvm.ComponentModel;

namespace RelojControl.ViewModels;

public enum MarcaStep { IngresoRut, VerificandoHuella, SeleccionSentido, Exito, Error }

public partial class MarcaBajoTraficoViewModel : ObservableObject
{
    [ObservableProperty] private MarcaStep _step            = MarcaStep.IngresoRut;
    [ObservableProperty] private string    _nombreTrabajador = "";
    [ObservableProperty] private string    _ultimaMarca      = "";
    [ObservableProperty] private string    _mensajeError     = "";
    [ObservableProperty] private bool      _isScanning;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RutDisplay))]
    private string _rutBuffer = "";

    public string RutDisplay
    {
        get
        {
            if (_rutBuffer.Length == 0) return "";
            if (_rutBuffer.Length <= 1) return _rutBuffer;
            var num       = _rutBuffer[..^1];
            var dv        = _rutBuffer[^1];
            var formatted = long.TryParse(num, out var n)
                ? n.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-CL"))
                : num;
            return $"{formatted}-{dv}";
        }
    }

    public void PressKey(string key)
    {
        switch (key)
        {
            case "DEL":
                if (_rutBuffer.Length > 0) RutBuffer = _rutBuffer[..^1];
                break;
            case "GO":
                if (_rutBuffer.Length >= 2) Step = MarcaStep.VerificandoHuella;
                break;
            default:
                if (_rutBuffer.Length < 9) RutBuffer += key;
                break;
        }
    }

    public void SetWorkerInfo(string nombre, string ultimaMarca)
    {
        NombreTrabajador = nombre;
        UltimaMarca      = ultimaMarca;
    }

    public void SetTrabajador(string nombre, string ultimaMarca)
    {
        NombreTrabajador = nombre;
        UltimaMarca      = ultimaMarca;
        Step             = MarcaStep.SeleccionSentido;
    }

    public void SetExito() => Step = MarcaStep.Exito;

    public void SetError(string mensaje)
    {
        MensajeError = mensaje;
        Step         = MarcaStep.Error;
    }

    public void Reset()
    {
        RutBuffer        = "";
        NombreTrabajador = "";
        UltimaMarca      = "";
        MensajeError     = "";
        IsScanning       = false;
        Step             = MarcaStep.IngresoRut;
    }
}
