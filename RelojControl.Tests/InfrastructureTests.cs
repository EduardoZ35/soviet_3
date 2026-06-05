using Xunit;
using RelojControl.Infrastructure;

namespace RelojControl.Tests;

public class InfrastructureTests
{
    [Fact]
    public void ViewModelBase_Set_RaisesPropertyChanged()
    {
        var vm = new TestViewModel();
        string? raised = null;
        vm.PropertyChanged += (_, e) => raised = e.PropertyName;
        vm.Name = "test";
        Assert.Equal("Name", raised);
        Assert.Equal("test", vm.Name);
    }

    [Fact]
    public void ViewModelBase_Set_ReturnsFalse_WhenValueUnchanged()
    {
        var vm = new TestViewModel { Name = "same" };
        bool changed = false;
        vm.PropertyChanged += (_, _) => changed = true;
        vm.Name = "same";
        Assert.False(changed);
    }

    [Fact]
    public void RelayCommand_Executes_Action()
    {
        bool executed = false;
        var cmd = new RelayCommand(_ => executed = true);
        cmd.Execute(null);
        Assert.True(executed);
    }

    [Fact]
    public void RelayCommand_CanExecute_UsesDelegate()
    {
        var cmd = new RelayCommand(_ => { }, _ => false);
        Assert.False(cmd.CanExecute(null));
    }

    private class TestViewModel : ViewModelBase
    {
        private string _name = "";
        public string Name { get => _name; set => Set(ref _name, value); }
    }
}
