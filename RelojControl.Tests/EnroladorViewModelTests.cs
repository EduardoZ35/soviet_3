using Xunit;
using RelojControl.ViewModels;

namespace RelojControl.Tests;

public class EnroladorViewModelTests
{
    [Fact]
    public void DefaultTab_IsZero()
    {
        var vm = new EnroladorViewModel();
        Assert.Equal(0, vm.ActiveTab);
    }

    [Fact]
    public void ActiveTab_CanBeChanged()
    {
        var vm = new EnroladorViewModel();
        vm.ActiveTab = 3;
        Assert.Equal(3, vm.ActiveTab);
    }

    [Fact]
    public void SamplesCollected_StartsAtZero()
    {
        var vm = new EnroladorViewModel();
        vm.StartEnrollment(1);
        Assert.Equal(0, vm.SamplesCollected);
        Assert.Equal(EnrollStep.Scanning, vm.EnrollStep);
    }

    [Fact]
    public void AddSample_IncrementsSamplesCollected()
    {
        var vm = new EnroladorViewModel();
        vm.StartEnrollment(2);
        vm.AddSample();
        vm.AddSample();
        Assert.Equal(2, vm.SamplesCollected);
    }

    [Fact]
    public void FourSamples_CompletesEnrollment()
    {
        var vm = new EnroladorViewModel();
        vm.StartEnrollment(1);
        vm.AddSample(); vm.AddSample(); vm.AddSample();
        bool done = vm.AddSample();
        Assert.True(done);
        Assert.Equal(4, vm.SamplesCollected);
        Assert.Equal(EnrollStep.Complete, vm.EnrollStep);
        Assert.False(vm.IsEnrolling);
    }

    [Fact]
    public void ResetEnrollment_RestoresIdle()
    {
        var vm = new EnroladorViewModel();
        vm.StartEnrollment(1);
        vm.AddSample(); vm.AddSample();
        vm.ResetEnrollment();
        Assert.Equal(EnrollStep.Idle, vm.EnrollStep);
        Assert.Equal(0, vm.SamplesCollected);
        Assert.False(vm.IsEnrolling);
    }

    [Fact]
    public void SetEnrollError_SetsErrorState()
    {
        var vm = new EnroladorViewModel();
        vm.StartEnrollment(1);
        vm.SetEnrollError("Lector no detectado.");
        Assert.Equal(EnrollStep.Error, vm.EnrollStep);
        Assert.Equal("Lector no detectado.", vm.MensajeEnrolamiento);
        Assert.False(vm.IsEnrolling);
    }

    [Fact]
    public void SelectWorker_SetsRutAndNombre()
    {
        var vm = new EnroladorViewModel();
        vm.SelectWorker("rutEncriptado", "Juan Pérez");
        Assert.Equal("rutEncriptado", vm.SelectedRut);
        Assert.Equal("Juan Pérez", vm.NombreTrabajador);
        Assert.True(vm.IsWorkerSelected);
    }

    [Fact]
    public void ClearWorker_ResetsSelection()
    {
        var vm = new EnroladorViewModel();
        vm.SelectWorker("rut", "Juan");
        vm.ClearWorker();
        Assert.Equal("", vm.SelectedRut);
        Assert.False(vm.IsWorkerSelected);
        Assert.Equal(EnrollStep.Idle, vm.EnrollStep);
    }
}
