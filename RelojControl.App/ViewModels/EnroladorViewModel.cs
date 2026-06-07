using CommunityToolkit.Mvvm.ComponentModel;

namespace RelojControl.ViewModels;

public enum EnrollStep { Idle, Scanning, Complete, Error }

public partial class EnroladorViewModel : ObservableObject
{
    [ObservableProperty] private string     _searchQuery         = "";
    [ObservableProperty] private int        _activeTab;
    [ObservableProperty] private EnrollStep _enrollStep          = EnrollStep.Idle;
    [ObservableProperty] private int        _samplesCollected;
    [ObservableProperty] private int        _fingerIndex         = 1;
    [ObservableProperty] private bool       _isEnrolling;
    [ObservableProperty] private string     _mensajeEnrolamiento = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWorkerSelected))]
    private string _selectedRut = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWorkerSelected))]
    private string _nombreTrabajador = "";

    public bool IsWorkerSelected => !string.IsNullOrEmpty(SelectedRut);

    public void SelectWorker(string rut, string nombre)
    {
        SelectedRut      = rut;
        NombreTrabajador = nombre;
        ActiveTab        = 0;
    }

    public void ClearWorker()
    {
        SelectedRut      = "";
        NombreTrabajador = "";
        ResetEnrollment();
    }

    public void StartEnrollment(int fingerIndex)
    {
        FingerIndex         = fingerIndex;
        SamplesCollected    = 0;
        EnrollStep          = EnrollStep.Scanning;
        IsEnrolling         = true;
        MensajeEnrolamiento = $"Escanea el dedo {fingerIndex} — muestra 1 de 4";
    }

    public bool AddSample()
    {
        if (EnrollStep != EnrollStep.Scanning) return false;
        SamplesCollected++;
        if (SamplesCollected >= 4)
        {
            EnrollStep          = EnrollStep.Complete;
            IsEnrolling         = false;
            MensajeEnrolamiento = "Huella registrada exitosamente.";
            return true;
        }
        MensajeEnrolamiento = $"Escanea el dedo {FingerIndex} — muestra {SamplesCollected + 1} de 4";
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
