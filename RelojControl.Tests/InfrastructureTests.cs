using Xunit;
using RelojControl.Infrastructure;

namespace RelojControl.Tests;

public class InfrastructureTests
{
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
}
