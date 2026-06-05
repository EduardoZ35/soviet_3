using RelojControl.Infrastructure;

namespace RelojControl.ViewModels;

public enum MarcaStep { IngresoRut, VerificandoHuella, SeleccionSentido, Exito, Error }

public class MarcaBajoTraficoViewModel : ViewModelBase
{
    private MarcaStep _step = MarcaStep.IngresoRut;
    private string    _rutBuffer = "";
    private string    _nombreTrabajador = "";
    private string    _ultimaMarca = "";
    private string    _mensajeError = "";
    private bool      _isScanning;

    public MarcaStep Step             { get => _step;             set => Set(ref _step, value); }
    public string    RutBuffer        { get => _rutBuffer;        set => Set(ref _rutBuffer, value); }
    public string    NombreTrabajador { get => _nombreTrabajador; set => Set(ref _nombreTrabajador, value); }
    public string    UltimaMarca      { get => _ultimaMarca;      set => Set(ref _ultimaMarca, value); }
    public string    MensajeError     { get => _mensajeError;     set => Set(ref _mensajeError, value); }
    public bool      IsScanning       { get => _isScanning;       set => Set(ref _isScanning, value); }

    public string RutDisplay
    {
        get
        {
            if (_rutBuffer.Length == 0) return "";
            if (_rutBuffer.Length <= 1) return _rutBuffer;
            var num = _rutBuffer[..^1];
            var dv  = _rutBuffer[^1];
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
                if (_rutBuffer.Length > 0)
                {
                    RutBuffer = _rutBuffer[..^1];
                    OnPropertyChanged(nameof(RutDisplay));
                }
                break;
            case "GO":
                if (_rutBuffer.Length >= 2) Step = MarcaStep.VerificandoHuella;
                break;
            default:
                if (_rutBuffer.Length < 9)
                {
                    RutBuffer += key;
                    OnPropertyChanged(nameof(RutDisplay));
                }
                break;
        }
    }

    public void SetWorkerInfo(string nombre, string ultimaMarca)
    {
        NombreTrabajador = nombre;
        UltimaMarca      = ultimaMarca;
        // Step stays at VerificandoHuella — fingerprint must verify before proceeding
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
        OnPropertyChanged(nameof(RutDisplay));
    }
}
