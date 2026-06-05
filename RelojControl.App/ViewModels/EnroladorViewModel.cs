using RelojControl.Infrastructure;

namespace RelojControl.ViewModels;

public enum EnrollStep { Idle, Scanning, Complete, Error }

public class EnroladorViewModel : ViewModelBase
{
    private string     _searchQuery          = "";
    private int        _activeTab;
    private EnrollStep _enrollStep           = EnrollStep.Idle;
    private int        _samplesCollected;
    private int        _fingerIndex          = 1;
    private bool       _isEnrolling;
    private string     _mensajeEnrolamiento  = "";
    private string     _selectedRut          = "";
    private string     _nombreTrabajador     = "";

    public string     SearchQuery         { get => _searchQuery;         set => Set(ref _searchQuery, value); }
    public int        ActiveTab           { get => _activeTab;           set => Set(ref _activeTab, value); }
    public EnrollStep EnrollStep          { get => _enrollStep;          set => Set(ref _enrollStep, value); }
    public int        SamplesCollected    { get => _samplesCollected;    set => Set(ref _samplesCollected, value); }
    public int        FingerIndex         { get => _fingerIndex;         set => Set(ref _fingerIndex, value); }
    public bool       IsEnrolling         { get => _isEnrolling;         set => Set(ref _isEnrolling, value); }
    public string     MensajeEnrolamiento { get => _mensajeEnrolamiento; set => Set(ref _mensajeEnrolamiento, value); }
    public string     SelectedRut         { get => _selectedRut;         set => Set(ref _selectedRut, value); }
    public string     NombreTrabajador    { get => _nombreTrabajador;    set => Set(ref _nombreTrabajador, value); }

    public bool IsWorkerSelected => !string.IsNullOrEmpty(_selectedRut);

    public void SelectWorker(string rut, string nombre)
    {
        SelectedRut       = rut;
        NombreTrabajador  = nombre;
        ActiveTab         = 0;
        OnPropertyChanged(nameof(IsWorkerSelected));
    }

    public void ClearWorker()
    {
        SelectedRut       = "";
        NombreTrabajador  = "";
        ResetEnrollment();
        OnPropertyChanged(nameof(IsWorkerSelected));
    }

    public void StartEnrollment(int fingerIndex)
    {
        FingerIndex          = fingerIndex;
        SamplesCollected     = 0;
        EnrollStep           = EnrollStep.Scanning;
        IsEnrolling          = true;
        MensajeEnrolamiento  = $"Escanea el dedo {fingerIndex} — muestra 1 de 4";
    }

    public bool AddSample()
    {
        if (_enrollStep != EnrollStep.Scanning) return false;
        SamplesCollected++;
        if (_samplesCollected >= 4)
        {
            EnrollStep          = EnrollStep.Complete;
            IsEnrolling         = false;
            MensajeEnrolamiento = "Huella registrada exitosamente.";
            return true;
        }
        MensajeEnrolamiento = $"Escanea el dedo {_fingerIndex} — muestra {_samplesCollected + 1} de 4";
        return false;
    }

    public void SetEnrollError(string mensaje)
    {
        EnrollStep          = EnrollStep.Error;
        IsEnrolling         = false;
        MensajeEnrolamiento = mensaje;
    }

    public void ResetEnrollment()
    {
        SamplesCollected    = 0;
        EnrollStep          = EnrollStep.Idle;
        IsEnrolling         = false;
        MensajeEnrolamiento = "";
    }
}
