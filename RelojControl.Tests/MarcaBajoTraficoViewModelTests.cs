using Xunit;
using RelojControl.ViewModels;

namespace RelojControl.Tests;

public class MarcaBajoTraficoViewModelTests
{
    [Fact]
    public void DigitKey_AppendsDigit_ToRutBuffer()
    {
        var vm = new MarcaBajoTraficoViewModel();
        vm.PressKey("1");
        vm.PressKey("2");
        Assert.Equal("12", vm.RutBuffer);
    }

    [Fact]
    public void DelKey_RemovesLastDigit()
    {
        var vm = new MarcaBajoTraficoViewModel();
        vm.PressKey("1"); vm.PressKey("2"); vm.PressKey("DEL");
        Assert.Equal("1", vm.RutBuffer);
    }

    [Fact]
    public void DelKey_OnEmpty_DoesNothing()
    {
        var vm = new MarcaBajoTraficoViewModel();
        vm.PressKey("DEL");
        Assert.Equal("", vm.RutBuffer);
    }

    [Fact]
    public void GoKey_WithEmptyBuffer_DoesNotAdvanceStep()
    {
        var vm = new MarcaBajoTraficoViewModel();
        vm.PressKey("GO");
        Assert.Equal(MarcaStep.IngresoRut, vm.Step);
    }

    [Fact]
    public void GoKey_WithRut_AdvancesToVerificando()
    {
        var vm = new MarcaBajoTraficoViewModel();
        foreach (var d in "12345678") vm.PressKey(d.ToString());
        vm.PressKey("GO");
        Assert.Equal(MarcaStep.VerificandoHuella, vm.Step);
    }

    [Fact]
    public void Reset_ReturnToIngresoRut_ClearsBuffer()
    {
        var vm = new MarcaBajoTraficoViewModel();
        vm.PressKey("1"); vm.PressKey("GO");
        vm.Reset();
        Assert.Equal(MarcaStep.IngresoRut, vm.Step);
        Assert.Equal("", vm.RutBuffer);
    }

    [Fact]
    public void RutDisplay_FormatsWithDots_AndDv()
    {
        var vm = new MarcaBajoTraficoViewModel();
        foreach (var d in "15847233") vm.PressKey(d.ToString());
        vm.PressKey("3");
        Assert.Equal("15.847.233-3", vm.RutDisplay);
    }

    [Fact]
    public void MaxLength_Enforced_At9Digits()
    {
        var vm = new MarcaBajoTraficoViewModel();
        foreach (var d in "123456789") vm.PressKey(d.ToString());
        vm.PressKey("0");
        Assert.Equal(9, vm.RutBuffer.Length);
    }
}
