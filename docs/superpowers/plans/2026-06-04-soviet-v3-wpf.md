# Soviet v3 — WPF Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a WPF .NET 8 desktop app (`C:\soviet_3\RelojControl.App`) that replicates soviet_v2's business logic with the Front Soviet aesthetic — prioritizing WndMarcaBajoTrafico (attendance kiosk) and WndEnrolador (admin panel).

**Architecture:** New solution `soviet_v3.sln` in `C:\soviet_3\` with a single WPF project `RelojControl.App` (net8.0-windows). Business logic reused via project reference to `soviet_v2\proyectoReloj-master\proyectoNegocioRflex`. MVVM pattern: ViewModels bind to XAML views, all design tokens in a single `Tokens.xaml` ResourceDictionary.

**Tech Stack:** C# .NET 8 WPF, xUnit (ViewModel tests), proyectoNegocioRflex (netstandard2.0), DigitalPersona SDK (local DLLs), System.Windows.Forms (NotifyIcon + DPFP), Poppins + Manrope fonts (embedded).

**Reference paths:**
- Source business logic: `C:\soviet_3\soviet_v2\proyectoReloj-master\proyectoNegocioRflex\`
- Front Soviet prototype: `C:\soviet_3\Front Soviet\RelojControl\entrega\`
- DigitalPersona DLLs: `C:\soviet_3\soviet_v2\proyectoReloj-master\proyectoRelojControlRflex\lib\`

---

## File Map

```
C:\soviet_3\
├── soviet_v3.sln
├── .gitignore
├── RelojControl.App\
│   ├── RelojControl.App.csproj
│   ├── App.xaml / App.xaml.cs
│   ├── Themes\
│   │   └── Tokens.xaml
│   ├── Fonts\
│   │   ├── Poppins-Regular.ttf
│   │   ├── Poppins-Medium.ttf
│   │   ├── Poppins-SemiBold.ttf
│   │   ├── Poppins-Bold.ttf
│   │   ├── Manrope-SemiBold.ttf
│   │   ├── Manrope-Bold.ttf
│   │   └── Manrope-ExtraBold.ttf
│   ├── Infrastructure\
│   │   ├── ViewModelBase.cs
│   │   └── RelayCommand.cs
│   ├── Controls\
│   │   ├── RailControl.xaml / .cs
│   │   ├── NumericKeypad.xaml / .cs
│   │   ├── FingerprintRing.xaml / .cs
│   │   └── NotifOverlay.xaml / .cs
│   ├── Windows\
│   │   ├── WndInicio.xaml / .cs
│   │   ├── WndMarcaBajoTrafico.xaml / .cs
│   │   ├── WndEnrolador.xaml / .cs
│   │   └── WndPanelControl.xaml / .cs
│   ├── ViewModels\
│   │   ├── MarcaBajoTraficoViewModel.cs
│   │   └── EnroladorViewModel.cs
│   └── lib\
│       ├── DPFPGuiNET.dll
│       ├── DPFPShrNET.dll
│       ├── DPFPVerNET.dll
│       ├── AxInterop.DPFPCtlXLib.dll
│       └── Interop.DPFPCtlXLib.dll
└── RelojControl.Tests\
    ├── RelojControl.Tests.csproj
    ├── MarcaBajoTraficoViewModelTests.cs
    └── EnroladorViewModelTests.cs
```

---

## Task 1: Git + Solution Scaffold

**Files:**
- Create: `C:\soviet_3\.gitignore`
- Create: `C:\soviet_3\soviet_v3.sln`
- Create: `C:\soviet_3\RelojControl.App\` (WPF project template)
- Delete: `C:\soviet_3\RelojControl.App\MainWindow.xaml` (generated, not needed)

- [ ] **Step 1: Initialize git repo**

```powershell
cd C:\soviet_3
git init
```

Expected: `Initialized empty Git repository in C:/soviet_3/.git/`

- [ ] **Step 2: Create .gitignore**

```powershell
@"
bin/
obj/
.vs/
*.user
.superpowers/
"@ | Out-File -Encoding utf8 C:\soviet_3\.gitignore
```

- [ ] **Step 3: Create solution and WPF project**

```powershell
cd C:\soviet_3
dotnet new sln -n soviet_v3
dotnet new wpf -n RelojControl.App -o RelojControl.App
dotnet sln soviet_v3.sln add RelojControl.App/RelojControl.App.csproj
```

Expected: three success messages, `soviet_v3.sln` created.

- [ ] **Step 4: Delete generated MainWindow files**

```powershell
Remove-Item C:\soviet_3\RelojControl.App\MainWindow.xaml
Remove-Item C:\soviet_3\RelojControl.App\MainWindow.xaml.cs
```

- [ ] **Step 5: Create test project**

```powershell
cd C:\soviet_3
dotnet new xunit -n RelojControl.Tests -o RelojControl.Tests
dotnet sln soviet_v3.sln add RelojControl.Tests/RelojControl.Tests.csproj
```

- [ ] **Step 6: Initial commit**

```powershell
cd C:\soviet_3
git add soviet_v3.sln .gitignore
git commit -m "chore: init soviet_v3 solution"
```

---

## Task 2: Project Files (csproj)

**Files:**
- Modify: `C:\soviet_3\RelojControl.App\RelojControl.App.csproj`
- Modify: `C:\soviet_3\RelojControl.Tests\RelojControl.Tests.csproj`

- [ ] **Step 1: Replace RelojControl.App.csproj**

Write the full content of `C:\soviet_3\RelojControl.App\RelojControl.App.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationIcon>rfv1.ico</ApplicationIcon>
    <RootNamespace>RelojControl</RootNamespace>
    <AssemblyName>RelojControl</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MySql.Data" Version="8.3.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Google.Protobuf" Version="3.25.3" />
    <PackageReference Include="QRCoder" Version="1.6.0" />
    <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
    <PackageReference Include="System.Management" Version="8.0.0" />
    <PackageReference Include="System.ServiceProcess.ServiceController" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\soviet_v2\proyectoReloj-master\proyectoNegocioRflex\proyectoNegocioRflex.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Reference Include="DPFPShrNET">
      <HintPath>lib\DPFPShrNET.dll</HintPath>
      <Private>true</Private>
    </Reference>
    <Reference Include="DPFPGuiNET">
      <HintPath>lib\DPFPGuiNET.dll</HintPath>
      <Private>true</Private>
    </Reference>
    <Reference Include="DPFPVerNET">
      <HintPath>lib\DPFPVerNET.dll</HintPath>
      <Private>true</Private>
    </Reference>
    <Reference Include="AxInterop.DPFPCtlXLib">
      <HintPath>lib\AxInterop.DPFPCtlXLib.dll</HintPath>
      <Private>true</Private>
    </Reference>
    <Reference Include="Interop.DPFPCtlXLib">
      <HintPath>lib\Interop.DPFPCtlXLib.dll</HintPath>
      <Private>true</Private>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <Resource Include="Fonts\*.ttf" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Replace RelojControl.Tests.csproj**

Write the full content of `C:\soviet_3\RelojControl.Tests\RelojControl.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="xunit" Version="2.7.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.7">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\RelojControl.App\RelojControl.App.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Copy DigitalPersona DLLs**

```powershell
New-Item -ItemType Directory -Force C:\soviet_3\RelojControl.App\lib
$src = "C:\soviet_3\soviet_v2\proyectoReloj-master\proyectoRelojControlRflex\lib"
Copy-Item "$src\DPFPShrNET.dll"           C:\soviet_3\RelojControl.App\lib\
Copy-Item "$src\DPFPGuiNET.dll"           C:\soviet_3\RelojControl.App\lib\
Copy-Item "$src\DPFPVerNET.dll"           C:\soviet_3\RelojControl.App\lib\
Copy-Item "$src\AxInterop.DPFPCtlXLib.dll" C:\soviet_3\RelojControl.App\lib\
Copy-Item "$src\Interop.DPFPCtlXLib.dll"  C:\soviet_3\RelojControl.App\lib\
```

- [ ] **Step 4: Restore and verify build**

```powershell
cd C:\soviet_3
dotnet restore
dotnet build RelojControl.App/RelojControl.App.csproj
```

Expected: `Build succeeded. 0 Warning(s). 0 Error(s).`

- [ ] **Step 5: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/RelojControl.App.csproj RelojControl.Tests/RelojControl.Tests.csproj RelojControl.App/lib/
git commit -m "chore: configure csproj with deps and DigitalPersona DLLs"
```

---

## Task 3: Tokens.xaml (Design System)

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Themes\Tokens.xaml`

- [ ] **Step 1: Create Themes directory and Tokens.xaml**

Create `C:\soviet_3\RelojControl.App\Themes\Tokens.xaml`:

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <!-- ═══ ACCENT ═══ -->
  <Color x:Key="AccentColor">#616EB3</Color>
  <Color x:Key="AccentDeepColor">#3F4690</Color>
  <Color x:Key="AccentSoftColor">#ECEDFB</Color>
  <SolidColorBrush x:Key="AccentBrush"     Color="{StaticResource AccentColor}"/>
  <SolidColorBrush x:Key="AccentDeepBrush" Color="{StaticResource AccentDeepColor}"/>
  <SolidColorBrush x:Key="AccentSoftBrush" Color="{StaticResource AccentSoftColor}"/>

  <!-- ═══ SIGNAL ═══ -->
  <SolidColorBrush x:Key="TealBrush"      Color="#34B3AB"/>
  <SolidColorBrush x:Key="TealSoftBrush"  Color="#D6F3F1"/>
  <SolidColorBrush x:Key="CoralBrush"     Color="#F76D6D"/>
  <SolidColorBrush x:Key="CoralSoftBrush" Color="#FFE3E3"/>
  <SolidColorBrush x:Key="AmberBrush"     Color="#E8910C"/>
  <SolidColorBrush x:Key="AmberSoftBrush" Color="#FDECCD"/>
  <SolidColorBrush x:Key="OkBrush"        Color="#16B364"/>
  <SolidColorBrush x:Key="OkSoftBrush"    Color="#D1FAE5"/>

  <!-- ═══ SURFACES ═══ -->
  <SolidColorBrush x:Key="BgBrush"       Color="#F3F3FA"/>
  <SolidColorBrush x:Key="SurfaceBrush"  Color="#FFFFFF"/>
  <SolidColorBrush x:Key="Surface2Brush" Color="#F7F7FC"/>
  <SolidColorBrush x:Key="InkBrush"      Color="#1B1B27"/>
  <SolidColorBrush x:Key="Ink2Brush"     Color="#5A5A72"/>
  <SolidColorBrush x:Key="Ink3Brush"     Color="#9A9AB0"/>
  <SolidColorBrush x:Key="LineBrush"     Color="#E7E7F1"/>

  <!-- ═══ RAIL GRADIENT ═══ -->
  <LinearGradientBrush x:Key="RailGradientBrush" StartPoint="0.3,0" EndPoint="0.7,1">
    <GradientStop Color="#5A63A8" Offset="0"/>
    <GradientStop Color="#454B8F" Offset="0.55"/>
    <GradientStop Color="#383C6F" Offset="1"/>
  </LinearGradientBrush>

  <!-- ═══ RAIL INK ═══ -->
  <SolidColorBrush x:Key="RailInkBrush"  Color="#FFFFFF"/>
  <SolidColorBrush x:Key="RailInk2Brush" Color="#C7C6EE"/>

  <!-- ═══ CORNER RADII ═══ -->
  <CornerRadius x:Key="RadiusCard"   TopLeft="18" TopRight="18" BottomLeft="18" BottomRight="18"/>
  <CornerRadius x:Key="RadiusInput"  TopLeft="10" TopRight="10" BottomLeft="10" BottomRight="10"/>
  <CornerRadius x:Key="RadiusButton" TopLeft="10" TopRight="10" BottomLeft="10" BottomRight="10"/>
  <CornerRadius x:Key="RadiusKeypad" TopLeft="16" TopRight="16" BottomLeft="16" BottomRight="16"/>
  <CornerRadius x:Key="RadiusBadge"  TopLeft="999" TopRight="999" BottomLeft="999" BottomRight="999"/>

  <!-- ═══ SHADOWS ═══ -->
  <DropShadowEffect x:Key="ShadowCard"
    BlurRadius="28" Direction="270" ShadowDepth="8" Opacity="0.15" Color="#1E1E50"/>
  <DropShadowEffect x:Key="ShadowKey"
    BlurRadius="0"  Direction="270" ShadowDepth="3" Opacity="1"    Color="#3F4690"/>

  <!-- ═══ FONT FAMILIES ═══ -->
  <FontFamily x:Key="FontPoppins">pack://application:,,,/RelojControl;component/Fonts/#Poppins</FontFamily>
  <FontFamily x:Key="FontManrope">pack://application:,,,/RelojControl;component/Fonts/#Manrope</FontFamily>

  <!-- ═══ BASE STYLES ═══ -->
  <Style x:Key="TextBody" TargetType="TextBlock">
    <Setter Property="FontFamily" Value="{StaticResource FontPoppins}"/>
    <Setter Property="FontSize"   Value="14"/>
    <Setter Property="Foreground" Value="{StaticResource InkBrush}"/>
  </Style>

  <Style x:Key="TextSubtitle" TargetType="TextBlock" BasedOn="{StaticResource TextBody}">
    <Setter Property="FontSize"   Value="13"/>
    <Setter Property="Foreground" Value="{StaticResource Ink2Brush}"/>
  </Style>

  <Style x:Key="TextHint" TargetType="TextBlock" BasedOn="{StaticResource TextBody}">
    <Setter Property="FontSize"   Value="12"/>
    <Setter Property="Foreground" Value="{StaticResource Ink3Brush}"/>
  </Style>

  <Style x:Key="TextEyebrow" TargetType="TextBlock">
    <Setter Property="FontFamily"       Value="{StaticResource FontPoppins}"/>
    <Setter Property="FontSize"         Value="11"/>
    <Setter Property="FontWeight"       Value="Bold"/>
    <Setter Property="Foreground"       Value="{StaticResource Ink3Brush}"/>
    <Setter Property="CharacterSpacing" Value="140"/>
  </Style>

  <!-- Button base -->
  <Style x:Key="BtnAccent" TargetType="Button">
    <Setter Property="Background"   Value="{StaticResource AccentBrush}"/>
    <Setter Property="Foreground"   Value="White"/>
    <Setter Property="FontFamily"   Value="{StaticResource FontPoppins}"/>
    <Setter Property="FontWeight"   Value="SemiBold"/>
    <Setter Property="FontSize"     Value="14"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="Padding"      Value="20,10"/>
    <Setter Property="Cursor"       Value="Hand"/>
    <Setter Property="Effect"       Value="{StaticResource ShadowKey}"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="Button">
          <Border Background="{TemplateBinding Background}"
                  CornerRadius="{StaticResource RadiusButton}"
                  Padding="{TemplateBinding Padding}">
            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
          </Border>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
    <Style.Triggers>
      <Trigger Property="IsPressed" Value="True">
        <Setter Property="RenderTransform">
          <Setter.Value><TranslateTransform Y="2"/></Setter.Value>
        </Setter>
        <Setter Property="Effect" Value="{x:Null}"/>
      </Trigger>
    </Style.Triggers>
  </Style>

  <Style x:Key="BtnSecondary" TargetType="Button" BasedOn="{StaticResource BtnAccent}">
    <Setter Property="Background"     Value="{StaticResource Surface2Brush}"/>
    <Setter Property="Foreground"     Value="{StaticResource Ink2Brush}"/>
    <Setter Property="Effect"         Value="{x:Null}"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="Button">
          <Border Background="{TemplateBinding Background}"
                  BorderBrush="{StaticResource LineBrush}"
                  BorderThickness="1"
                  CornerRadius="{StaticResource RadiusButton}"
                  Padding="{TemplateBinding Padding}">
            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
          </Border>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>

  <!-- Input base -->
  <Style x:Key="InputBase" TargetType="TextBox">
    <Setter Property="FontFamily"     Value="{StaticResource FontPoppins}"/>
    <Setter Property="FontSize"       Value="14"/>
    <Setter Property="Foreground"     Value="{StaticResource InkBrush}"/>
    <Setter Property="Background"     Value="{StaticResource SurfaceBrush}"/>
    <Setter Property="BorderBrush"    Value="{StaticResource LineBrush}"/>
    <Setter Property="BorderThickness" Value="1.5"/>
    <Setter Property="Padding"        Value="12,8"/>
    <Setter Property="Template">
      <Setter.Value>
        <ControlTemplate TargetType="TextBox">
          <Border Background="{TemplateBinding Background}"
                  BorderBrush="{TemplateBinding BorderBrush}"
                  BorderThickness="{TemplateBinding BorderThickness}"
                  CornerRadius="{StaticResource RadiusInput}">
            <ScrollViewer x:Name="PART_ContentHost" Margin="{TemplateBinding Padding}"/>
          </Border>
          <ControlTemplate.Triggers>
            <Trigger Property="IsFocused" Value="True">
              <Setter Property="BorderBrush" Value="{StaticResource AccentBrush}"/>
            </Trigger>
          </ControlTemplate.Triggers>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>

</ResourceDictionary>
```

- [ ] **Step 2: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Themes/Tokens.xaml
git commit -m "feat: add Tokens.xaml design system (Front Soviet tokens)"
```

---

## Task 4: Fonts (Poppins + Manrope)

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Fonts\` (7 TTF files)

- [ ] **Step 1: Create Fonts directory**

```powershell
New-Item -ItemType Directory -Force C:\soviet_3\RelojControl.App\Fonts
```

- [ ] **Step 2: Download Poppins fonts**

Download these files from Google Fonts (https://fonts.google.com/specimen/Poppins) and place in `C:\soviet_3\RelojControl.App\Fonts\`:
- `Poppins-Regular.ttf`
- `Poppins-Medium.ttf`
- `Poppins-SemiBold.ttf`
- `Poppins-Bold.ttf`

Alternatively, download via PowerShell:

```powershell
$base = "https://fonts.gstatic.com/s/poppins/v21"
$dest = "C:\soviet_3\RelojControl.App\Fonts"
Invoke-WebRequest "$base/pxiEyp8kv8JHgZlgi5qAaWgz.woff2" -OutFile "$dest\Poppins-Regular.ttf"
```

> **Note:** Google Fonts woff2 URLs change. The safest approach is to visit https://fonts.google.com/specimen/Poppins, click "Download family", extract the ZIP, and copy the 4 TTF files above to `Fonts\`. Do the same for Manrope.

- [ ] **Step 3: Download Manrope fonts**

Download from https://fonts.google.com/specimen/Manrope and place in `C:\soviet_3\RelojControl.App\Fonts\`:
- `Manrope-SemiBold.ttf`
- `Manrope-Bold.ttf`
- `Manrope-ExtraBold.ttf`

- [ ] **Step 4: Verify font files exist**

```powershell
Get-ChildItem C:\soviet_3\RelojControl.App\Fonts\
```

Expected: 7 `.ttf` files listed.

- [ ] **Step 5: Build to verify font embedding**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Fonts/
git commit -m "feat: embed Poppins and Manrope fonts"
```

---

## Task 5: Infrastructure (ViewModelBase, RelayCommand, App.xaml)

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Infrastructure\ViewModelBase.cs`
- Create: `C:\soviet_3\RelojControl.App\Infrastructure\RelayCommand.cs`
- Modify: `C:\soviet_3\RelojControl.App\App.xaml`
- Modify: `C:\soviet_3\RelojControl.App\App.xaml.cs`
- Create: `C:\soviet_3\RelojControl.Tests\InfrastructureTests.cs`

- [ ] **Step 1: Create ViewModelBase.cs**

```csharp
// C:\soviet_3\RelojControl.App\Infrastructure\ViewModelBase.cs
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RelojControl.Infrastructure;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
```

- [ ] **Step 2: Create RelayCommand.cs**

```csharp
// C:\soviet_3\RelojControl.App\Infrastructure\RelayCommand.cs
using System;
using System.Windows.Input;

namespace RelojControl.Infrastructure;

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add    => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter)    => _execute(parameter);
    public void RaiseCanExecuteChanged()      => CommandManager.InvalidateRequerySuggested();
}
```

- [ ] **Step 3: Replace App.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\App.xaml -->
<Application x:Class="RelojControl.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="Themes/Tokens.xaml"/>
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

- [ ] **Step 4: Replace App.xaml.cs (minimal startup — tray icon wired in Task 10)**

```csharp
// C:\soviet_3\RelojControl.App\App.xaml.cs
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
```

- [ ] **Step 5: Create stub WndInicio so the app compiles (full UI in Task 11)**

Create `C:\soviet_3\RelojControl.App\Windows\WndInicio.xaml`:

```xml
<Window x:Class="RelojControl.Windows.WndInicio"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="WndInicio" Width="800" Height="600">
  <Grid Background="{StaticResource BgBrush}">
    <TextBlock Text="Soviet v3 — cargando..." HorizontalAlignment="Center" VerticalAlignment="Center"
               Style="{StaticResource TextBody}"/>
  </Grid>
</Window>
```

Create `C:\soviet_3\RelojControl.App\Windows\WndInicio.xaml.cs`:

```csharp
using System.Windows;

namespace RelojControl.Windows;

public partial class WndInicio : Window
{
    public WndInicio() => InitializeComponent();
}
```

- [ ] **Step 6: Write infrastructure test**

```csharp
// C:\soviet_3\RelojControl.Tests\InfrastructureTests.cs
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
```

- [ ] **Step 7: Run tests**

```powershell
cd C:\soviet_3
dotnet test RelojControl.Tests/RelojControl.Tests.csproj --no-build -v normal
```

Expected: `Passed: 4, Failed: 0`

(Run `dotnet build` first if needed.)

- [ ] **Step 8: Build app**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 9: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Infrastructure/ RelojControl.App/App.xaml RelojControl.App/App.xaml.cs RelojControl.App/Windows/WndInicio.xaml RelojControl.App/Windows/WndInicio.xaml.cs RelojControl.Tests/InfrastructureTests.cs
git commit -m "feat: add ViewModelBase, RelayCommand, App startup stub"
```

---

## Task 6: RailControl UserControl

The rail appears on both WndMarcaBajoTrafico (left sidebar, 380px wide) and has a simplified equivalent in other screens. This UserControl encapsulates the gradient rail with clock, sync status, and location info.

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Controls\RailControl.xaml`
- Create: `C:\soviet_3\RelojControl.App\Controls\RailControl.xaml.cs`

- [ ] **Step 1: Create RailControl.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\Controls\RailControl.xaml -->
<UserControl x:Class="RelojControl.Controls.RailControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Name="rail">
  <Border Background="{StaticResource RailGradientBrush}" ClipToBounds="True">
    <!-- Decorative circle top-right -->
    <Grid>
      <Ellipse Width="320" Height="320" HorizontalAlignment="Right" VerticalAlignment="Top"
               Margin="0,-120,-120,0" Opacity="0.08">
        <Ellipse.Fill>
          <RadialGradientBrush>
            <GradientStop Color="White" Offset="0"/>
            <GradientStop Color="Transparent" Offset="1"/>
          </RadialGradientBrush>
        </Ellipse.Fill>
      </Ellipse>

      <DockPanel Margin="28,32,28,28">
        <!-- Logo top -->
        <TextBlock DockPanel.Dock="Top" Text="rFlex.io"
                   FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="14"
                   Foreground="{StaticResource RailInkBrush}" LetterSpacing="1"/>

        <!-- Clock -->
        <StackPanel DockPanel.Dock="Top" Margin="0,24,0,0">
          <StackPanel Orientation="Horizontal" VerticalAlignment="Bottom">
            <TextBlock x:Name="lblHora"
                       FontFamily="{StaticResource FontManrope}" FontWeight="ExtraBold" FontSize="80"
                       Foreground="{StaticResource RailInkBrush}" LineHeight="1"
                       VerticalAlignment="Bottom"/>
            <TextBlock x:Name="lblSegundos"
                       FontFamily="{StaticResource FontManrope}" FontWeight="SemiBold" FontSize="28"
                       Foreground="{StaticResource RailInk2Brush}" VerticalAlignment="Bottom"
                       Margin="6,0,0,10"/>
          </StackPanel>
          <TextBlock x:Name="lblFecha"
                     FontFamily="{StaticResource FontPoppins}" FontSize="13"
                     Foreground="{StaticResource RailInk2Brush}" Margin="0,4,0,0"/>
        </StackPanel>

        <!-- Sync pill -->
        <Border DockPanel.Dock="Top" Margin="0,20,0,0" HorizontalAlignment="Left"
                CornerRadius="{StaticResource RadiusBadge}" Padding="10,4">
          <Border.Background>
            <SolidColorBrush Color="#34B3AB" Opacity="0.2"/>
          </Border.Background>
          <StackPanel Orientation="Horizontal">
            <Ellipse x:Name="syncDot" Width="8" Height="8" Fill="#34B3AB" VerticalAlignment="Center"/>
            <TextBlock x:Name="lblSync" Text="Sincronizado"
                       FontFamily="{StaticResource FontPoppins}" FontSize="11"
                       Foreground="{StaticResource TealSoftBrush}" Margin="6,0,0,0"/>
          </StackPanel>
        </Border>

        <!-- Bottom info: empresa / sucursal / ubicacion -->
        <StackPanel DockPanel.Dock="Bottom" VerticalAlignment="Bottom">
          <TextBlock x:Name="lblUbicacion"
                     FontFamily="{StaticResource FontPoppins}" FontSize="11"
                     Foreground="{StaticResource RailInk2Brush}" TextWrapping="Wrap"/>
          <TextBlock x:Name="lblEmpresa"
                     FontFamily="{StaticResource FontPoppins}" FontWeight="SemiBold" FontSize="12"
                     Foreground="{StaticResource RailInkBrush}" TextWrapping="Wrap" Margin="0,4,0,0"/>
        </StackPanel>

        <Grid/>
      </DockPanel>
    </Grid>
  </Border>
</UserControl>
```

- [ ] **Step 2: Create RailControl.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Controls\RailControl.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RelojControl.Controls;

public partial class RailControl : UserControl
{
    private readonly DispatcherTimer _clock = new() { Interval = TimeSpan.FromSeconds(1) };

    public static readonly DependencyProperty EmpresaProperty =
        DependencyProperty.Register(nameof(Empresa), typeof(string), typeof(RailControl),
            new PropertyMetadata("", (d, _) => ((RailControl)d).lblEmpresa.Text = (string)d.GetValue(EmpresaProperty)));

    public static readonly DependencyProperty UbicacionProperty =
        DependencyProperty.Register(nameof(Ubicacion), typeof(string), typeof(RailControl),
            new PropertyMetadata("", (d, _) => ((RailControl)d).lblUbicacion.Text = (string)d.GetValue(UbicacionProperty)));

    public string Empresa   { get => (string)GetValue(EmpresaProperty);   set => SetValue(EmpresaProperty, value); }
    public string Ubicacion { get => (string)GetValue(UbicacionProperty); set => SetValue(UbicacionProperty, value); }

    public RailControl()
    {
        InitializeComponent();
        _clock.Tick += (_, _) => ActualizarReloj();
        Loaded   += (_, _) => { ActualizarReloj(); _clock.Start(); };
        Unloaded += (_, _) => _clock.Stop();
    }

    private void ActualizarReloj()
    {
        var now = DateTime.Now;
        lblHora.Text     = now.ToString("HH:mm");
        lblSegundos.Text = now.ToString("ss");
        lblFecha.Text    = now.ToString("dddd, d 'de' MMMM",
                               System.Globalization.CultureInfo.GetCultureInfo("es-CL"));
    }

    public void SetSyncStatus(bool online)
    {
        syncDot.Fill = new System.Windows.Media.SolidColorBrush(
            online ? System.Windows.Media.Color.FromRgb(0x34, 0xB3, 0xAB)
                   : System.Windows.Media.Color.FromRgb(0xF7, 0x6D, 0x6D));
        lblSync.Text = online ? "Sincronizado" : "Sin conexión";
    }
}
```

- [ ] **Step 3: Build to verify**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Controls/RailControl.xaml RelojControl.App/Controls/RailControl.xaml.cs
git commit -m "feat: add RailControl (gradient sidebar with clock)"
```

---

## Task 7: NumericKeypad UserControl

3×4 grid (1–9, ⌫, 0, IR→) used in WndMarcaBajoTrafico. Raises a `KeyPressed` event with the digit or action.

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Controls\NumericKeypad.xaml`
- Create: `C:\soviet_3\RelojControl.App\Controls\NumericKeypad.xaml.cs`

- [ ] **Step 1: Create NumericKeypad.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\Controls\NumericKeypad.xaml -->
<UserControl x:Class="RelojControl.Controls.NumericKeypad"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <UniformGrid Columns="3" Rows="4">

    <Button Content="1"  Tag="1"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="2"  Tag="2"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="3"  Tag="3"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="4"  Tag="4"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="5"  Tag="5"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="6"  Tag="6"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="7"  Tag="7"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="8"  Tag="8"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="9"  Tag="9"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="⌫"  Tag="DEL" Click="Key_Click" Style="{StaticResource KeyBtnDel}"/>
    <Button Content="0"  Tag="0"   Click="Key_Click" Style="{StaticResource KeyBtn}"/>
    <Button Content="IR →" Tag="GO" Click="Key_Click" Style="{StaticResource KeyBtnGo}"/>

  </UniformGrid>

  <UserControl.Resources>
    <!-- Normal key -->
    <Style x:Key="KeyBtn" TargetType="Button">
      <Setter Property="Background"     Value="{StaticResource SurfaceBrush}"/>
      <Setter Property="Foreground"     Value="{StaticResource InkBrush}"/>
      <Setter Property="FontFamily"     Value="{StaticResource FontManrope}"/>
      <Setter Property="FontWeight"     Value="Bold"/>
      <Setter Property="FontSize"       Value="26"/>
      <Setter Property="BorderThickness" Value="1"/>
      <Setter Property="BorderBrush"    Value="{StaticResource LineBrush}"/>
      <Setter Property="Margin"         Value="4"/>
      <Setter Property="Cursor"         Value="Hand"/>
      <Setter Property="Template">
        <Setter.Value>
          <ControlTemplate TargetType="Button">
            <Border x:Name="bd" Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    CornerRadius="{StaticResource RadiusKeypad}">
              <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
            </Border>
            <ControlTemplate.Triggers>
              <Trigger Property="IsPressed" Value="True">
                <Setter TargetName="bd" Property="Background" Value="{StaticResource AccentSoftBrush}"/>
                <Setter TargetName="bd" Property="RenderTransform">
                  <Setter.Value><TranslateTransform Y="2"/></Setter.Value>
                </Setter>
              </Trigger>
              <Trigger Property="IsMouseOver" Value="True">
                <Setter TargetName="bd" Property="Background" Value="{StaticResource Surface2Brush}"/>
              </Trigger>
            </ControlTemplate.Triggers>
          </ControlTemplate>
        </Setter.Value>
      </Setter>
    </Style>

    <!-- Delete key -->
    <Style x:Key="KeyBtnDel" TargetType="Button" BasedOn="{StaticResource KeyBtn}">
      <Setter Property="Foreground" Value="{StaticResource CoralBrush}"/>
      <Setter Property="FontSize"   Value="20"/>
    </Style>

    <!-- Go key -->
    <Style x:Key="KeyBtnGo" TargetType="Button" BasedOn="{StaticResource KeyBtn}">
      <Setter Property="Background" Value="{StaticResource AccentBrush}"/>
      <Setter Property="Foreground" Value="White"/>
      <Setter Property="FontSize"   Value="15"/>
      <Setter Property="Effect"     Value="{StaticResource ShadowKey}"/>
      <Setter Property="BorderThickness" Value="0"/>
    </Style>
  </UserControl.Resources>
</UserControl>
```

- [ ] **Step 2: Create NumericKeypad.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Controls\NumericKeypad.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;

namespace RelojControl.Controls;

public partial class NumericKeypad : UserControl
{
    public event EventHandler<string>? KeyPressed;

    public NumericKeypad() => InitializeComponent();

    private void Key_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
            KeyPressed?.Invoke(this, tag);
    }
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Controls/NumericKeypad.xaml RelojControl.App/Controls/NumericKeypad.xaml.cs
git commit -m "feat: add NumericKeypad UserControl (3x4 grid)"
```

---

## Task 8: FingerprintRing UserControl

Circular ring with fingerprint icon. Shows idle, scanning (spinning arc), success, and error states.

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Controls\FingerprintRing.xaml`
- Create: `C:\soviet_3\RelojControl.App\Controls\FingerprintRing.xaml.cs`

- [ ] **Step 1: Create FingerprintRing.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\Controls\FingerprintRing.xaml -->
<UserControl x:Class="RelojControl.Controls.FingerprintRing"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Width="192" Height="192">
  <Grid>
    <!-- Background circle -->
    <Ellipse x:Name="bgCircle" Width="192" Height="192"
             Fill="{StaticResource AccentSoftBrush}"/>

    <!-- Spinning arc (visible when scanning) -->
    <Ellipse x:Name="scanArc" Width="192" Height="192"
             Stroke="{StaticResource AccentBrush}" StrokeThickness="3"
             StrokeDashArray="60 140" Visibility="Collapsed"
             RenderTransformOrigin="0.5,0.5">
      <Ellipse.RenderTransform>
        <RotateTransform x:Name="arcRotation" Angle="0"/>
      </Ellipse.RenderTransform>
      <Ellipse.Triggers>
        <EventTrigger RoutedEvent="Loaded">
          <BeginStoryboard x:Name="spinStoryboard">
            <Storyboard RepeatBehavior="Forever">
              <DoubleAnimation Storyboard.TargetName="arcRotation"
                               Storyboard.TargetProperty="Angle"
                               From="0" To="360" Duration="0:0:1.4"/>
            </Storyboard>
          </BeginStoryboard>
        </EventTrigger>
      </Ellipse.Triggers>
    </Ellipse>

    <!-- Fingerprint / WiFi icon (SVG path) -->
    <Viewbox Width="96" Height="96">
      <Canvas Width="24" Height="24">
        <Path x:Name="fpIcon" Fill="Transparent"
              Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
              StrokeLineCap="Round" StrokeLineJoin="Round"
              Data="M12,2 C9.8,2 7.8,2.8 6.3,4.2 M12,2 C14.2,2 16.2,2.8 17.7,4.2
                    M4.2,6.3 C2.8,7.8 2,9.8 2,12
                    M19.8,6.3 C21.2,7.8 22,9.8 22,12
                    M2,12 C2,14.2 2.8,16.2 4.2,17.7
                    M22,12 C22,14.2 21.2,16.2 19.8,17.7
                    M6.3,19.8 C7.8,21.2 9.8,22 12,22
                    M17.7,19.8 C16.2,21.2 14.2,22 12,22
                    M12,7 C9.2,7 7,9.2 7,12 M12,7 C14.8,7 17,9.2 17,12
                    M9,12 C9,13.7 10.3,15 12,15 M15,12 C15,13.7 13.7,15 12,15
                    M12,17 L12,19"/>
      </Canvas>
    </Viewbox>

    <!-- Success check overlay -->
    <Grid x:Name="successOverlay" Visibility="Collapsed">
      <Ellipse Fill="{StaticResource OkSoftBrush}" Width="192" Height="192"/>
      <Viewbox Width="80" Height="80">
        <Canvas Width="100" Height="100">
          <Path x:Name="checkPath" Stroke="{StaticResource OkBrush}" StrokeThickness="6"
                StrokeLineCap="Round" StrokeLineJoin="Round" Fill="Transparent"
                Data="M 15,50 L 40,75 L 85,20"
                StrokeDashArray="120" StrokeDashOffset="120">
            <Path.Triggers>
              <EventTrigger RoutedEvent="Path.Loaded"/>
            </Path.Triggers>
          </Path>
        </Canvas>
      </Viewbox>
    </Grid>
  </Grid>
</UserControl>
```

- [ ] **Step 2: Create FingerprintRing.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Controls\FingerprintRing.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace RelojControl.Controls;

public enum FingerprintState { Idle, Scanning, Success, Error }

public partial class FingerprintRing : UserControl
{
    public FingerprintRing() => InitializeComponent();

    public void SetState(FingerprintState state)
    {
        scanArc.Visibility      = Visibility.Collapsed;
        successOverlay.Visibility = Visibility.Collapsed;
        bgCircle.Fill           = (System.Windows.Media.Brush)FindResource("AccentSoftBrush");
        fpIcon.Stroke           = (System.Windows.Media.Brush)FindResource("AccentBrush");

        switch (state)
        {
            case FingerprintState.Scanning:
                scanArc.Visibility = Visibility.Visible;
                break;

            case FingerprintState.Success:
                bgCircle.Fill = (System.Windows.Media.Brush)FindResource("OkSoftBrush");
                fpIcon.Stroke = (System.Windows.Media.Brush)FindResource("OkBrush");
                successOverlay.Visibility = Visibility.Visible;
                AnimateCheck();
                break;

            case FingerprintState.Error:
                bgCircle.Fill = (System.Windows.Media.Brush)FindResource("CoralSoftBrush");
                fpIcon.Stroke = (System.Windows.Media.Brush)FindResource("CoralBrush");
                break;
        }
    }

    private void AnimateCheck()
    {
        var anim = new DoubleAnimation(120, 0, new Duration(System.TimeSpan.FromMilliseconds(500)))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        checkPath.BeginAnimation(System.Windows.Shapes.Path.StrokeDashOffsetProperty, anim);
    }
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Controls/FingerprintRing.xaml RelojControl.App/Controls/FingerprintRing.xaml.cs
git commit -m "feat: add FingerprintRing UserControl with state animations"
```

---

## Task 9: NotifOverlay UserControl

Modal notification overlay with semi-transparent backdrop, colored top bar, message, and auto-close countdown.

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Controls\NotifOverlay.xaml`
- Create: `C:\soviet_3\RelojControl.App\Controls\NotifOverlay.xaml.cs`

- [ ] **Step 1: Create NotifOverlay.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\Controls\NotifOverlay.xaml -->
<UserControl x:Class="RelojControl.Controls.NotifOverlay"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Visibility="Collapsed">
  <Grid>
    <!-- Backdrop -->
    <Rectangle Fill="#80000000"/>

    <!-- Card -->
    <Border MaxWidth="480" MinWidth="320" HorizontalAlignment="Center" VerticalAlignment="Center"
            Background="{StaticResource SurfaceBrush}" CornerRadius="{StaticResource RadiusCard}"
            Effect="{StaticResource ShadowCard}" ClipToBounds="True">
      <Grid>
        <Grid.RowDefinitions>
          <RowDefinition Height="6"/>
          <RowDefinition/>
          <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Color bar -->
        <Rectangle x:Name="colorBar" Grid.Row="0" Fill="{StaticResource AccentBrush}"/>

        <!-- Body -->
        <StackPanel Grid.Row="1" Margin="28,20,28,16" Spacing="8">
          <TextBlock x:Name="lblTitulo" FontFamily="{StaticResource FontPoppins}"
                     FontWeight="Bold" FontSize="16" Foreground="{StaticResource InkBrush}"
                     TextWrapping="Wrap"/>
          <TextBlock x:Name="lblMensaje" Style="{StaticResource TextSubtitle}"
                     TextWrapping="Wrap"/>
        </StackPanel>

        <!-- Countdown bar -->
        <Grid Grid.Row="2">
          <Rectangle Height="3" Fill="{StaticResource LineBrush}"/>
          <Rectangle x:Name="progressBar" Height="3" HorizontalAlignment="Left"
                     Fill="{StaticResource AccentBrush}"/>
        </Grid>
      </Grid>
    </Border>
  </Grid>
</UserControl>
```

- [ ] **Step 2: Create NotifOverlay.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Controls\NotifOverlay.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace RelojControl.Controls;

public enum NotifType { Info, Success, Warning, Error }

public partial class NotifOverlay : UserControl
{
    private DispatcherTimer? _timer;
    private double           _totalSeconds;

    public NotifOverlay() => InitializeComponent();

    public void Show(string titulo, string mensaje, NotifType tipo = NotifType.Info, double segundos = 6)
    {
        lblTitulo.Text  = titulo;
        lblMensaje.Text = mensaje;

        colorBar.Fill = tipo switch
        {
            NotifType.Success => (Brush)FindResource("TealBrush"),
            NotifType.Warning => (Brush)FindResource("AmberBrush"),
            NotifType.Error   => (Brush)FindResource("CoralBrush"),
            _                 => (Brush)FindResource("AccentBrush"),
        };

        progressBar.Width = ActualWidth > 0 ? ActualWidth : 480;
        _totalSeconds     = segundos;
        Visibility        = Visibility.Visible;

        var anim = new DoubleAnimation(progressBar.Width, 0, TimeSpan.FromSeconds(segundos));
        progressBar.BeginAnimation(FrameworkElement.WidthProperty, anim);

        _timer?.Stop();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(segundos) };
        _timer.Tick += (_, _) => { _timer.Stop(); Visibility = Visibility.Collapsed; };
        _timer.Start();
    }

    public void Hide()
    {
        _timer?.Stop();
        Visibility = Visibility.Collapsed;
    }
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Controls/NotifOverlay.xaml RelojControl.App/Controls/NotifOverlay.xaml.cs
git commit -m "feat: add NotifOverlay UserControl with countdown"
```

---

## Task 10: App.xaml.cs — Tray Icon Sincronizador

Replicates the `InitSyncMonitor` logic from `frmInicio.cs` in soviet_v2. Uses `System.Windows.Forms.NotifyIcon` (WinForms interop).

**Files:**
- Modify: `C:\soviet_3\RelojControl.App\App.xaml.cs`

- [ ] **Step 1: Replace App.xaml.cs with tray icon logic**

```csharp
// C:\soviet_3\RelojControl.App\App.xaml.cs
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
    private NotifyIcon?     _tray;
    private DispatcherTimer? _syncTimer;
    private Icon?           _iconGreen, _iconYellow, _iconRed;

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
        catch { /* no bloquear inicio */ }
    }

    private void ActualizarEstadoSync()
    {
        try
        {
            bool procesoVivo      = Process.GetProcessesByName("sincronizadorServicio").Length > 0;
            string estado         = File.Exists(SyncStatusFile) ? File.ReadAllText(SyncStatusFile).Trim() : "";
            DateTime ultimaEscri  = File.Exists(SyncStatusFile) ? File.GetLastWriteTime(SyncStatusFile) : DateTime.MinValue;
            double minSinActiv    = (DateTime.Now - ultimaEscri).TotalMinutes;
            bool detenido         = estado.StartsWith("Detenido") || !procesoVivo;
            bool bloqueado        = procesoVivo && minSinActiv > 10;

            var (icono, texto) = detenido  ? (_iconRed!,    "rFlex Sinc — DETENIDO")
                               : bloqueado ? (_iconYellow!, $"rFlex Sinc — SIN ACTIV {minSinActiv:F0}min")
                               :             (_iconGreen!,  $"rFlex Sinc — OK");

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
                Arguments        = $"/c net stop {ServiceName} & net start {ServiceName}",
                Verb             = "runas",
                UseShellExecute  = true,
                WindowStyle      = ProcessWindowStyle.Hidden,
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
```

- [ ] **Step 2: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 3: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/App.xaml.cs
git commit -m "feat: add tray icon sync monitor in App.xaml.cs"
```

---

## Task 11: WndInicio (Lobby Screen)

Replicates frmInicio behavior: loads reloj config from DB, shows lobby (login card + marca card), navigates based on DB flags.

**Files:**
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndInicio.xaml`
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndInicio.xaml.cs`

- [ ] **Step 1: Create WndInicio.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\Windows\WndInicio.xaml -->
<Window x:Class="RelojControl.Windows.WndInicio"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        WindowStyle="None" AllowsTransparency="True"
        WindowStartupLocation="CenterScreen"
        WindowState="Maximized"
        Background="Transparent">
  <Border>
    <Border.Background>
      <LinearGradientBrush StartPoint="0.3,0" EndPoint="0.7,1">
        <GradientStop Color="#5A63A8" Offset="0"/>
        <GradientStop Color="#454B8F" Offset="0.55"/>
        <GradientStop Color="#383C6F" Offset="1"/>
      </LinearGradientBrush>
    </Border.Background>

    <DockPanel>
      <!-- TOP BAR -->
      <Grid DockPanel.Dock="Top" Margin="40,28,40,0">
        <Grid.ColumnDefinitions>
          <ColumnDefinition Width="Auto"/>
          <ColumnDefinition/>
          <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>

        <!-- Sync pill -->
        <Border Grid.Column="0" CornerRadius="{StaticResource RadiusBadge}" Padding="14,6"
                Background="#3034B3AB">
          <StackPanel Orientation="Horizontal">
            <Ellipse x:Name="syncDotInicio" Width="8" Height="8" Fill="#34B3AB" VerticalAlignment="Center"/>
            <TextBlock x:Name="lblSyncInicio" Text="Sincronizado · en línea"
                       FontFamily="{StaticResource FontPoppins}" FontSize="12"
                       Foreground="#D6F3F1" Margin="8,0,0,0"/>
          </StackPanel>
        </Border>

        <!-- Title -->
        <TextBlock x:Name="lblTituloReloj" Grid.Column="1" Text="Registro de Asistencia"
                   FontFamily="{StaticResource FontPoppins}" FontSize="16" FontWeight="Medium"
                   Foreground="White" HorizontalAlignment="Center" VerticalAlignment="Center"/>

        <!-- Clock -->
        <StackPanel Grid.Column="2" HorizontalAlignment="Right">
          <TextBlock x:Name="lblHoraInicio"
                     FontFamily="{StaticResource FontManrope}" FontWeight="ExtraBold" FontSize="32"
                     Foreground="White" HorizontalAlignment="Right" LineHeight="1"/>
          <TextBlock x:Name="lblFechaInicio"
                     FontFamily="{StaticResource FontPoppins}" FontSize="12"
                     Foreground="#C7C6EE" HorizontalAlignment="Right"/>
        </StackPanel>
      </Grid>

      <!-- FOOTER -->
      <TextBlock DockPanel.Dock="Bottom" Text="rFlex.io  ·  ·  ·  ·"
                 FontFamily="{StaticResource FontPoppins}" FontSize="13"
                 Foreground="#80C7C6EE" HorizontalAlignment="Center" Margin="0,0,0,24"/>

      <!-- CARDS CENTER -->
      <Grid VerticalAlignment="Center" HorizontalAlignment="Center" Margin="40,32">
        <Grid.ColumnDefinitions>
          <ColumnDefinition Width="380"/>
          <ColumnDefinition Width="24"/>
          <ColumnDefinition Width="380"/>
        </Grid.ColumnDefinitions>

        <!-- Login card -->
        <Border x:Name="cardLogin" Grid.Column="0"
                Background="{StaticResource SurfaceBrush}"
                CornerRadius="{StaticResource RadiusCard}" Padding="32"
                Effect="{StaticResource ShadowCard}" Visibility="Collapsed">
          <StackPanel Spacing="16">
            <StackPanel Orientation="Horizontal" Spacing="10">
              <TextBlock Text="🔒" FontSize="18" VerticalAlignment="Center"/>
              <TextBlock Text="Iniciar sesión administrador"
                         FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="17"
                         Foreground="{StaticResource InkBrush}" VerticalAlignment="Center"/>
            </StackPanel>

            <StackPanel Spacing="8">
              <TextBlock Text="Usuario" Style="{StaticResource TextHint}"/>
              <TextBox x:Name="txtUsuario" Style="{StaticResource InputBase}"/>
              <TextBlock Text="Contraseña" Style="{StaticResource TextHint}"/>
              <PasswordBox x:Name="txtPass" FontFamily="{StaticResource FontPoppins}"
                           FontSize="14" Padding="12,8" BorderThickness="1.5"
                           BorderBrush="{StaticResource LineBrush}"
                           Background="{StaticResource SurfaceBrush}"/>
            </StackPanel>

            <Button Content="Iniciar sesión" Style="{StaticResource BtnAccent}"
                    Background="{StaticResource TealBrush}" Click="BtnLogin_Click"
                    HorizontalAlignment="Stretch"/>
          </StackPanel>
        </Border>

        <!-- Marca card -->
        <Border x:Name="cardMarca" Grid.Column="2"
                Background="{StaticResource SurfaceBrush}"
                CornerRadius="{StaticResource RadiusCard}" Padding="32"
                Effect="{StaticResource ShadowCard}" Cursor="Hand"
                MouseLeftButtonUp="CardMarca_Click">
          <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="16">
            <TextBlock Text="📶" FontSize="48" HorizontalAlignment="Center"/>
            <TextBlock Text="Registrar marca"
                       FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="20"
                       Foreground="{StaticResource InkBrush}" HorizontalAlignment="Center"/>
          </StackPanel>
        </Border>
      </Grid>
    </DockPanel>
  </Border>
</Window>
```

- [ ] **Step 2: Create WndInicio.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Windows\WndInicio.xaml.cs
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using proyectoNegocioRflex.Modelo;

namespace RelojControl.Windows;

public partial class WndInicio : Window
{
    private int    _idReloj, _idEmpresa, _idSucursal, _idResolucionMarca;
    private string _rutEmpresa = "", _nombreEmpresa = "", _codigoEmpresa = "";
    private string _nombreSucursal = "", _ubicacionReloj = "", _direccionSucursal = "";
    private bool   _permiteComida, _permiteJornada, _habilitado, _bloqueado;
    private bool   _iniciarDesdeMarca, _permitidoUso;
    private int    _soloEnrolar, _idTipoRolUsuario;

    private readonly DispatcherTimer _reloj = new() { Interval = TimeSpan.FromSeconds(1) };

    public WndInicio()
    {
        InitializeComponent();
        _reloj.Tick += (_, _) => ActualizarReloj();
        _reloj.Start();
        ActualizarReloj();
        Loaded  += OnLoaded;
        KeyDown += OnKeyDown;
    }

    private void ActualizarReloj()
    {
        var now = DateTime.Now;
        lblHoraInicio.Text  = now.ToString("HH:mm");
        lblFechaInicio.Text = now.ToString("dddd, d 'de' MMMM",
            System.Globalization.CultureInfo.GetCultureInfo("es-CL"));
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var h = new Herramientas();
            if (!h.comprobarArchivosBase(false))
            {
                MostrarNotif("Reloj mal configurado", "Faltan archivos de configuración.", NotifType.Error);
                return;
            }
            ComprobarReloj();
            if (_bloqueado)
            {
                MostrarNotif("Reloj bloqueado", "Contactar con soporte rFlex.", NotifType.Warning);
                return;
            }
            if (_soloEnrolar == 1) { cardLogin.Visibility = Visibility.Visible; return; }
            if (_iniciarDesdeMarca && _permitidoUso) AbrirMarca();
        }
        catch (Exception ex)
        {
            MostrarNotif("Error de inicio", ex.Message, NotifType.Error);
        }
    }

    private void ComprobarReloj()
    {
        var r  = new Reloj();
        var dt = r.traerDatosRelojPorNombre(Environment.MachineName);
        if (dt == null || dt.Rows.Count == 0)
        {
            MostrarNotif("Sin configuración", "El reloj no está registrado.", NotifType.Warning);
            _permitidoUso = false;
            return;
        }
        var row = dt.Rows[0];
        _idReloj           = int.Parse(row[0].ToString()!);
        _idEmpresa         = int.Parse(row[1].ToString()!);
        _ubicacionReloj    = row[4].ToString()!;
        _habilitado        = row[5].ToString() == "1";
        _bloqueado         = row[14].ToString() == "1";
        _permiteComida     = row[9].ToString()  == "1";
        _permiteJornada    = row[10].ToString() == "1";
        _iniciarDesdeMarca = row[12].ToString() == "1";
        _soloEnrolar       = int.Parse(row[18].ToString()!);
        _idResolucionMarca = int.Parse(row[20].ToString()!);

        lblTituloReloj.Text = $"Registro de Asistencia · {_ubicacionReloj}";

        ComprobarEmpresa();
        ComprobarSucursal();
        _permitidoUso = ComprobarTipoMarca() && ComprobarTipoRechazo() && ComprobarTipoInhabilitacion();
        cardMarca.Visibility = _permitidoUso ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ComprobarEmpresa()
    {
        var dt = new Empresa().traerDatosEmpresaPorID(_idEmpresa);
        if (dt == null || dt.Rows.Count == 0) return;
        _rutEmpresa    = dt.Rows[0][1].ToString()!;
        _nombreEmpresa = dt.Rows[0][2].ToString()!;
        _codigoEmpresa = dt.Rows[0][11].ToString()!;
    }

    private void ComprobarSucursal()
    {
        var dt = new Sucursal().traerSucursalPorIDReloj(_idReloj);
        if (dt == null || dt.Rows.Count == 0) return;
        _idSucursal        = int.Parse(dt.Rows[0][0].ToString()!);
        _nombreSucursal    = dt.Rows[0][2].ToString()!;
        _direccionSucursal = $"{dt.Rows[0][5]} {dt.Rows[0][6]} {dt.Rows[0][7]} {dt.Rows[0][8]}";
    }

    private bool ComprobarTipoMarca()         => new TipoMarca().traerTipoMarca()?.Rows.Count > 0;
    private bool ComprobarTipoRechazo()       => new TipoRechazo().traerTipoRechazo()?.Rows.Count > 0;
    private bool ComprobarTipoInhabilitacion()=> new TipoInhabilitacionMarca().traerTiposInhabilitacionMarca()?.Rows.Count > 0;

    private void CardMarca_Click(object sender, MouseButtonEventArgs e) => AbrirMarca();

    private void AbrirMarca()
    {
        var frm = new WndMarcaBajoTrafico
        {
            IdReloj          = _idReloj,
            IdEmpresa        = _idEmpresa,
            IdSucursal       = _idSucursal,
            PermiteComida    = _permiteComida,
            PermiteJornada   = _permiteJornada,
            NombreEmpresa    = _nombreEmpresa,
            NombreSucursal   = _nombreSucursal,
            UbicacionReloj   = _ubicacionReloj,
            DireccionSucursal= _direccionSucursal,
        };
        frm.Show();
        Hide();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPass.Password))
        { MostrarNotif("Iniciar sesión", "Ingrese usuario y contraseña.", NotifType.Warning); return; }

        var enc  = new Encriptacion();
        var p    = new Persona();
        var dt   = p.loginEnrolamiento(enc.Encriptar(txtUsuario.Text.ToUpper()), enc.Encriptar(txtPass.Password));
        if (dt == null || dt.Rows.Count == 0)
        { MostrarNotif("Iniciar sesión", "Usuario o contraseña incorrectos.", NotifType.Error); return; }

        _idTipoRolUsuario = int.Parse(dt.Rows[0][5].ToString()!);
        var panel = new WndPanelControl(_idTipoRolUsuario) { CodigoEmpresa = _codigoEmpresa };
        panel.Show();
        Hide();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.E && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            cardLogin.Visibility = cardLogin.Visibility == Visibility.Visible
                ? Visibility.Collapsed : Visibility.Visible;
    }

    private void MostrarNotif(string titulo, string msg, NotifType tipo)
        => MessageBox.Show(msg, titulo); // replaced by NotifOverlay in WndMarcaBajoTrafico

    // Called from child windows when they close
    public void OnChildClosed() => Show();
}
```

> **Note:** `WndMarcaBajoTrafico` and `WndPanelControl` are stub classes until Task 13 and Task 18. Build will fail until stubs exist — create them next.

- [ ] **Step 3: Create WndPanelControl stub**

Create `C:\soviet_3\RelojControl.App\Windows\WndPanelControl.xaml`:

```xml
<Window x:Class="RelojControl.Windows.WndPanelControl"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Panel de Control" WindowState="Maximized" WindowStyle="None">
  <Grid Background="{StaticResource BgBrush}">
    <TextBlock Text="Panel de Control — próximamente"
               HorizontalAlignment="Center" VerticalAlignment="Center"
               Style="{StaticResource TextBody}"/>
  </Grid>
</Window>
```

Create `C:\soviet_3\RelojControl.App\Windows\WndPanelControl.xaml.cs`:

```csharp
using System.Windows;
namespace RelojControl.Windows;
public partial class WndPanelControl : Window
{
    public string CodigoEmpresa { get; set; } = "";
    public WndPanelControl(int idRol) => InitializeComponent();
}
```

- [ ] **Step 4: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 5: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Windows/
git commit -m "feat: implement WndInicio lobby with DB config and navigation"
```

---

## Task 12: MarcaBajoTraficoViewModel + Tests

Pure C# ViewModel — testable without UI.

**Files:**
- Create: `C:\soviet_3\RelojControl.App\ViewModels\MarcaBajoTraficoViewModel.cs`
- Create: `C:\soviet_3\RelojControl.Tests\MarcaBajoTraficoViewModelTests.cs`

- [ ] **Step 1: Write failing tests first**

```csharp
// C:\soviet_3\RelojControl.Tests\MarcaBajoTraficoViewModelTests.cs
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
    public void RutDisplay_FormatsWithDots_AndK()
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
        vm.PressKey("0"); // 10th digit
        Assert.Equal(9, vm.RutBuffer.Length);
    }
}
```

- [ ] **Step 2: Run tests — expect FAIL**

```powershell
cd C:\soviet_3
dotnet build
dotnet test RelojControl.Tests/RelojControl.Tests.csproj --no-build
```

Expected: compile error — `MarcaBajoTraficoViewModel` not found.

- [ ] **Step 3: Implement MarcaBajoTraficoViewModel.cs**

```csharp
// C:\soviet_3\RelojControl.App\ViewModels\MarcaBajoTraficoViewModel.cs
using System;
using System.Windows.Input;
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

    public MarcaStep Step            { get => _step;             set => Set(ref _step, value); }
    public string    RutBuffer       { get => _rutBuffer;        set => Set(ref _rutBuffer, value); }
    public string    NombreTrabajador{ get => _nombreTrabajador; set => Set(ref _nombreTrabajador, value); }
    public string    UltimaMarca     { get => _ultimaMarca;      set => Set(ref _ultimaMarca, value); }
    public string    MensajeError    { get => _mensajeError;     set => Set(ref _mensajeError, value); }
    public bool      IsScanning      { get => _isScanning;       set => Set(ref _isScanning, value); }

    // Formatted RUT: "15.847.233-3" — last digit/K after dash, dots on number part
    public string RutDisplay
    {
        get
        {
            if (_rutBuffer.Length == 0) return "";
            if (_rutBuffer.Length <= 1) return _rutBuffer;
            var num = _rutBuffer[..^1];
            var dv  = _rutBuffer[^1];
            // insert dots: 12345678 → 12.345.678
            var formatted = long.TryParse(num, out var n)
                ? n.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-CL")).Replace(".", ".")
                : num;
            return $"{formatted}-{dv}";
        }
    }

    public void PressKey(string key)
    {
        switch (key)
        {
            case "DEL":
                if (_rutBuffer.Length > 0) { RutBuffer = _rutBuffer[..^1]; OnPropertyChanged(nameof(RutDisplay)); }
                break;
            case "GO":
                if (_rutBuffer.Length >= 2) Step = MarcaStep.VerificandoHuella;
                break;
            default:
                if (_rutBuffer.Length < 9) { RutBuffer += key; OnPropertyChanged(nameof(RutDisplay)); }
                break;
        }
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
```

- [ ] **Step 4: Run tests — expect PASS**

```powershell
cd C:\soviet_3
dotnet build
dotnet test RelojControl.Tests/RelojControl.Tests.csproj --no-build -v normal
```

Expected: `Passed: 8, Failed: 0`

- [ ] **Step 5: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/ViewModels/MarcaBajoTraficoViewModel.cs RelojControl.Tests/MarcaBajoTraficoViewModelTests.cs
git commit -m "feat: MarcaBajoTraficoViewModel with RUT input logic (TDD)"
```

---

## Task 13: WndMarcaBajoTrafico — Full UI

Kiosk window: left rail (380px) + main area switching between all 5 MarcaStep states.

**Files:**
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndMarcaBajoTrafico.xaml`
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndMarcaBajoTrafico.xaml.cs`

- [ ] **Step 1: Create WndMarcaBajoTrafico.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\Windows\WndMarcaBajoTrafico.xaml -->
<Window x:Class="RelojControl.Windows.WndMarcaBajoTrafico"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:ctrl="clr-namespace:RelojControl.Controls"
        Width="733" Height="1061"
        WindowStyle="None" Topmost="True"
        WindowStartupLocation="CenterScreen"
        Background="{StaticResource BgBrush}"
        KeyDown="Window_KeyDown">

  <Grid>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="280"/>
      <ColumnDefinition/>
    </Grid.ColumnDefinitions>

    <!-- RAIL -->
    <ctrl:RailControl x:Name="rail" Grid.Column="0"
                      Empresa="{Binding NombreEmpresa, RelativeSource={RelativeSource AncestorType=Window}}"
                      Ubicacion="{Binding UbicacionReloj, RelativeSource={RelativeSource AncestorType=Window}}"/>

    <!-- MAIN AREA -->
    <Grid Grid.Column="1" Background="{StaticResource BgBrush}">

      <!-- Overlay de notificaciones -->
      <ctrl:NotifOverlay x:Name="notif" Panel.ZIndex="100"/>

      <!-- ═══ ESTADO: IngresoRut ═══ -->
      <Grid x:Name="panelRut" Margin="40,48,40,40">
        <Grid.RowDefinitions>
          <RowDefinition Height="Auto"/>
          <RowDefinition Height="Auto"/>
          <RowDefinition Height="Auto"/>
          <RowDefinition/>
        </Grid.RowDefinitions>

        <TextBlock Grid.Row="0" Text="MARCA TU ASISTENCIA"
                   Style="{StaticResource TextEyebrow}" Margin="0,0,0,8"/>
        <TextBlock Grid.Row="1" Text="Ingresa tu RUT"
                   FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="34"
                   Foreground="{StaticResource InkBrush}" Margin="0,0,0,24"/>

        <!-- Display RUT -->
        <Border Grid.Row="2" Background="{StaticResource SurfaceBrush}"
                BorderBrush="{StaticResource LineBrush}" BorderThickness="1.5"
                CornerRadius="{StaticResource RadiusInput}" Padding="20,14"
                Margin="0,0,0,20" Effect="{StaticResource ShadowCard}">
          <Grid>
            <Grid.ColumnDefinitions>
              <ColumnDefinition/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <TextBlock x:Name="lblRutDisplay" Grid.Column="0"
                       FontFamily="{StaticResource FontManrope}" FontWeight="ExtraBold" FontSize="42"
                       Foreground="{StaticResource InkBrush}" LetterSpacing="2"
                       VerticalAlignment="Center"/>
            <!-- Caret -->
            <Rectangle x:Name="caret" Grid.Column="1" Width="3" Fill="{StaticResource AccentBrush}"
                       Height="42" VerticalAlignment="Center" Margin="4,0,0,0">
              <Rectangle.Triggers>
                <EventTrigger RoutedEvent="Loaded">
                  <BeginStoryboard>
                    <Storyboard RepeatBehavior="Forever">
                      <DoubleAnimation Storyboard.TargetProperty="Opacity"
                                       From="1" To="0" Duration="0:0:0.6" AutoReverse="True"/>
                    </Storyboard>
                  </BeginStoryboard>
                </EventTrigger>
              </Rectangle.Triggers>
            </Rectangle>
          </Grid>
        </Border>

        <!-- Keypad -->
        <ctrl:NumericKeypad x:Name="keypad" Grid.Row="3" KeyPressed="Keypad_KeyPressed"/>
      </Grid>

      <!-- ═══ ESTADO: VerificandoHuella ═══ -->
      <Grid x:Name="panelHuella" Margin="40,48,40,40" Visibility="Collapsed">
        <Grid.RowDefinitions>
          <RowDefinition Height="Auto"/>
          <RowDefinition Height="Auto"/>
          <RowDefinition Height="Auto"/>
          <RowDefinition/>
          <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Tarjeta trabajador -->
        <Border Grid.Row="0" Background="{StaticResource SurfaceBrush}"
                BorderBrush="{StaticResource LineBrush}" BorderThickness="1"
                CornerRadius="{StaticResource RadiusCard}" Padding="20,16"
                Effect="{StaticResource ShadowCard}" Margin="0,0,0,24">
          <Grid>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <!-- Avatar -->
            <Border Grid.Column="0" Width="56" Height="56" CornerRadius="{StaticResource RadiusButton}"
                    Margin="0,0,14,0">
              <Border.Background>
                <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                  <GradientStop Color="#616EB3" Offset="0"/>
                  <GradientStop Color="#3F4690" Offset="1"/>
                </LinearGradientBrush>
              </Border.Background>
              <TextBlock x:Name="lblAvatar" FontFamily="{StaticResource FontPoppins}" FontWeight="Bold"
                         FontSize="18" Foreground="White" HorizontalAlignment="Center"
                         VerticalAlignment="Center"/>
            </Border>
            <!-- Name / RUT -->
            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="4">
              <TextBlock x:Name="lblNombre" FontFamily="{StaticResource FontPoppins}" FontWeight="Bold"
                         FontSize="18" Foreground="{StaticResource InkBrush}"/>
              <TextBlock x:Name="lblRutCard" Style="{StaticResource TextSubtitle}"/>
            </StackPanel>
            <!-- Last mark -->
            <StackPanel Grid.Column="2" VerticalAlignment="Center" HorizontalAlignment="Right">
              <TextBlock Text="ÚLTIMA MARCA" Style="{StaticResource TextEyebrow}" HorizontalAlignment="Right"/>
              <TextBlock x:Name="lblUltimaMarca" Style="{StaticResource TextHint}" HorizontalAlignment="Right"/>
            </StackPanel>
          </Grid>
        </Border>

        <!-- Sentido chip -->
        <Border Grid.Row="1" x:Name="chipSentido" CornerRadius="{StaticResource RadiusBadge}"
                Padding="16,8" HorizontalAlignment="Left" Margin="0,0,0,32"
                Background="{StaticResource TealSoftBrush}">
          <TextBlock x:Name="lblSentido" Text="Jornada · Inicio"
                     FontFamily="{StaticResource FontPoppins}" FontWeight="SemiBold" FontSize="14"
                     Foreground="{StaticResource TealBrush}"/>
        </Border>

        <!-- Fingerprint ring centered -->
        <ctrl:FingerprintRing x:Name="fpRing" Grid.Row="2"
                              HorizontalAlignment="Center" Margin="0,0,0,16"/>

        <!-- Scanning text -->
        <TextBlock Grid.Row="3" x:Name="lblEsperandoHuella" Text="Verificando huella..."
                   FontFamily="{StaticResource FontPoppins}" FontWeight="SemiBold" FontSize="16"
                   Foreground="{StaticResource AccentBrush}" HorizontalAlignment="Center"/>

        <!-- Volver button -->
        <Button Grid.Row="4" Content="← Volver" Style="{StaticResource BtnSecondary}"
                HorizontalAlignment="Left" Click="BtnVolver_Click"/>
      </Grid>

      <!-- ═══ ESTADO: SeleccionSentido ═══ -->
      <Grid x:Name="panelSentido" Margin="40,48,40,40" Visibility="Collapsed">
        <Grid.RowDefinitions>
          <RowDefinition Height="Auto"/>
          <RowDefinition Height="Auto"/>
          <RowDefinition/>
        </Grid.RowDefinitions>
        <TextBlock Grid.Row="0" Text="¿Qué tipo de marca?"
                   FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="28"
                   Foreground="{StaticResource InkBrush}" Margin="0,0,0,24"/>
        <StackPanel Grid.Row="1" Spacing="12">
          <Button x:Name="btnEntrada"  Tag="Entrada"  Click="BtnSentido_Click" Style="{StaticResource SentidoBtn}" BorderBrush="{StaticResource TealBrush}">
            <StackPanel Orientation="Horizontal" Spacing="16">
              <Border Width="56" Height="56" CornerRadius="{StaticResource RadiusButton}" Background="{StaticResource TealSoftBrush}">
                <TextBlock Text="↗" FontSize="26" Foreground="{StaticResource TealBrush}" HorizontalAlignment="Center" VerticalAlignment="Center"/>
              </Border>
              <StackPanel VerticalAlignment="Center">
                <TextBlock Text="ENTRADA" FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="14" Foreground="{StaticResource TealBrush}"/>
                <TextBlock Text="Inicio de jornada" Style="{StaticResource TextSubtitle}"/>
              </StackPanel>
            </StackPanel>
          </Button>
          <Button x:Name="btnSalida"   Tag="Salida"   Click="BtnSentido_Click" Style="{StaticResource SentidoBtn}" BorderBrush="{StaticResource CoralBrush}">
            <StackPanel Orientation="Horizontal" Spacing="16">
              <Border Width="56" Height="56" CornerRadius="{StaticResource RadiusButton}" Background="{StaticResource CoralSoftBrush}">
                <TextBlock Text="↙" FontSize="26" Foreground="{StaticResource CoralBrush}" HorizontalAlignment="Center" VerticalAlignment="Center"/>
              </Border>
              <StackPanel VerticalAlignment="Center">
                <TextBlock Text="SALIDA" FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="14" Foreground="{StaticResource CoralBrush}"/>
                <TextBlock Text="Término de jornada" Style="{StaticResource TextSubtitle}"/>
              </StackPanel>
            </StackPanel>
          </Button>
        </StackPanel>
      </Grid>

      <!-- ═══ ESTADO: Exito ═══ -->
      <Grid x:Name="panelExito" Visibility="Collapsed">
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="20">
          <ctrl:FingerprintRing x:Name="fpSuccess" HorizontalAlignment="Center"/>
          <TextBlock x:Name="lblExito" Text="¡Marca registrada!"
                     FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="32"
                     Foreground="{StaticResource InkBrush}" HorizontalAlignment="Center"/>
          <TextBlock x:Name="lblNombreExito" Style="{StaticResource TextSubtitle}" HorizontalAlignment="Center" FontSize="18"/>
          <Border x:Name="chipExito" CornerRadius="{StaticResource RadiusBadge}" Padding="20,10"
                  HorizontalAlignment="Center" Background="{StaticResource TealSoftBrush}">
            <TextBlock x:Name="lblSentidoExito" FontFamily="{StaticResource FontPoppins}" FontWeight="SemiBold"
                       FontSize="16" Foreground="{StaticResource TealBrush}"/>
          </Border>
          <TextBlock x:Name="lblVolviendo" Text="Volviendo al inicio en 5s..."
                     Style="{StaticResource TextHint}" HorizontalAlignment="Center"/>
        </StackPanel>
      </Grid>

      <!-- ═══ ESTADO: Error ═══ -->
      <Grid x:Name="panelError" Visibility="Collapsed">
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Spacing="20">
          <ctrl:FingerprintRing x:Name="fpError" HorizontalAlignment="Center"/>
          <TextBlock Text="No se pudo registrar"
                     FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="28"
                     Foreground="{StaticResource CoralBrush}" HorizontalAlignment="Center"/>
          <TextBlock x:Name="lblMensajeError" Style="{StaticResource TextSubtitle}"
                     HorizontalAlignment="Center" TextAlignment="Center"/>
          <Button Content="Intentar de nuevo" Style="{StaticResource BtnAccent}"
                  HorizontalAlignment="Center" Click="BtnReintentar_Click"/>
        </StackPanel>
      </Grid>

    </Grid>
  </Grid>

  <Window.Resources>
    <Style x:Key="SentidoBtn" TargetType="Button">
      <Setter Property="Background"     Value="{StaticResource SurfaceBrush}"/>
      <Setter Property="BorderThickness" Value="0,0,0,0,5,0"/>
      <Setter Property="Padding"        Value="22,18"/>
      <Setter Property="Cursor"         Value="Hand"/>
      <Setter Property="Effect"         Value="{StaticResource ShadowCard}"/>
      <Setter Property="Template">
        <Setter.Value>
          <ControlTemplate TargetType="Button">
            <Border Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="0,0,5,0"
                    CornerRadius="{StaticResource RadiusCard}"
                    Padding="{TemplateBinding Padding}">
              <ContentPresenter/>
            </Border>
          </ControlTemplate>
        </Setter.Value>
      </Setter>
      <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
          <Setter Property="RenderTransform">
            <Setter.Value><TranslateTransform Y="-3"/></Setter.Value>
          </Setter>
        </Trigger>
      </Style.Triggers>
    </Style>
  </Window.Resources>
</Window>
```

- [ ] **Step 2: Create WndMarcaBajoTrafico.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Windows\WndMarcaBajoTrafico.xaml.cs
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DPFP.Capture;
using proyectoNegocioRflex.Modelo;
using RelojControl.Controls;
using RelojControl.ViewModels;

namespace RelojControl.Windows;

public partial class WndMarcaBajoTrafico : Window, DPFP.Capture.EventHandler
{
    public int    IdReloj           { get; set; }
    public int    IdEmpresa         { get; set; }
    public int    IdSucursal        { get; set; }
    public bool   PermiteComida     { get; set; }
    public bool   PermiteJornada    { get; set; }
    public string NombreEmpresa     { get; set; } = "";
    public string NombreSucursal    { get; set; } = "";
    public string UbicacionReloj    { get; set; } = "";
    public string DireccionSucursal { get; set; } = "";

    private readonly MarcaBajoTraficoViewModel _vm = new();
    private Capture? _capture;
    private int      _idPersona;
    private readonly DispatcherTimer _resetTimer = new() { Interval = TimeSpan.FromSeconds(5) };

    public WndMarcaBajoTrafico()
    {
        InitializeComponent();
        _vm.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(MarcaBajoTraficoViewModel.Step)) UpdatePanel(); };
        _resetTimer.Tick += (_, _) => { _resetTimer.Stop(); _vm.Reset(); };
        DataContext = _vm;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        rail.Empresa    = NombreEmpresa;
        rail.Ubicacion  = $"{NombreSucursal} · {UbicacionReloj}";
        IniciarCaptura();
        UpdatePanel();
    }

    // ── Keypad ──────────────────────────────────────────────────────────────
    private void Keypad_KeyPressed(object? sender, string key)
    {
        _vm.PressKey(key);
        lblRutDisplay.Text = _vm.RutDisplay;

        if (_vm.Step == MarcaStep.VerificandoHuella)
            BuscarTrabajador();
    }

    private void BuscarTrabajador()
    {
        fpRing.SetState(FingerprintState.Scanning);
        _vm.IsScanning = true;

        var rut = _vm.RutBuffer;
        // Query DB
        var p  = new Persona();
        var enc = new Encriptacion();
        var dt  = p.traerDatosPersonaPorRut(enc.Encriptar(rut));

        if (dt == null || dt.Rows.Count == 0)
        {
            _vm.SetError("RUT no encontrado en el sistema.");
            return;
        }

        _idPersona        = int.Parse(dt.Rows[0][0].ToString()!);
        string nombre     = $"{dt.Rows[0][2]} {dt.Rows[0][4]} {dt.Rows[0][5]}";
        string ultimaMarca = ObtenerUltimaMarca(_idPersona);

        lblNombre.Text    = nombre;
        lblRutCard.Text   = FormatRut(rut);
        lblAvatar.Text    = GetInitials(nombre);
        lblUltimaMarca.Text = ultimaMarca;

        _vm.SetTrabajador(nombre, ultimaMarca);
    }

    private string ObtenerUltimaMarca(int idPersona)
    {
        try
        {
            var m  = new Marca();
            var dt = m.traerUltimaMarcaPorPersona(idPersona, IdReloj);
            if (dt == null || dt.Rows.Count == 0) return "Sin marcas previas";
            var row  = dt.Rows[0];
            var tipo = row[1].ToString()!;
            var hora = DateTime.Parse(row[0].ToString()!).ToString("HH:mm");
            return $"Hoy {hora} · {tipo}";
        }
        catch { return ""; }
    }

    // ── Panel switching ──────────────────────────────────────────────────────
    private void UpdatePanel()
    {
        Dispatcher.Invoke(() =>
        {
            panelRut.Visibility    = _vm.Step == MarcaStep.IngresoRut         ? Visibility.Visible : Visibility.Collapsed;
            panelHuella.Visibility = _vm.Step == MarcaStep.VerificandoHuella  ? Visibility.Visible : Visibility.Collapsed;
            panelSentido.Visibility= _vm.Step == MarcaStep.SeleccionSentido   ? Visibility.Visible : Visibility.Collapsed;
            panelExito.Visibility  = _vm.Step == MarcaStep.Exito              ? Visibility.Visible : Visibility.Collapsed;
            panelError.Visibility  = _vm.Step == MarcaStep.Error              ? Visibility.Visible : Visibility.Collapsed;

            if (_vm.Step == MarcaStep.Error)
            {
                fpError.SetState(FingerprintState.Error);
                lblMensajeError.Text = _vm.MensajeError;
            }
            if (_vm.Step == MarcaStep.IngresoRut) lblRutDisplay.Text = "";
        });
    }

    // ── Sentido buttons ──────────────────────────────────────────────────────
    private void BtnSentido_Click(object sender, RoutedEventArgs e)
    {
        var sentido = ((System.Windows.Controls.Button)sender).Tag?.ToString() ?? "";
        RegistrarMarca(sentido);
    }

    private void RegistrarMarca(string sentido)
    {
        try
        {
            var marca = new Marca
            {
                persona_idpersona     = _idPersona,
                reloj_idreloj         = IdReloj,
                empresa_idempresa     = IdEmpresa,
                sucursal_idsucursal   = IdSucursal,
                tipo_marcacion        = sentido,
                fecha_hora_marcacion  = DateTime.Now,
            };
            marca.guardarMarcaLocal();

            fpSuccess.SetState(FingerprintState.Success);
            lblNombreExito.Text  = _vm.NombreTrabajador;
            lblSentidoExito.Text = $"Jornada · {sentido} · {DateTime.Now:HH:mm}";
            _vm.SetExito();
            _resetTimer.Start();
        }
        catch (Exception ex)
        {
            _vm.SetError($"Error al guardar: {ex.Message}");
        }
    }

    // ── Navigation ───────────────────────────────────────────────────────────
    private void BtnVolver_Click(object sender, RoutedEventArgs e)  => _vm.Reset();
    private void BtnReintentar_Click(object sender, RoutedEventArgs e) => _vm.Reset();

    // ── DigitalPersona ───────────────────────────────────────────────────────
    private void IniciarCaptura()
    {
        try
        {
            _capture = new Capture(DataPurpose.Verification);
            _capture.EventHandler = this;
            _capture.StartCapture();
        }
        catch { /* SDK no disponible en dev */ }
    }

    void DPFP.Capture.EventHandler.OnComplete(object capture, string sn, DPFP.Sample sample)
    {
        Dispatcher.Invoke(() =>
        {
            if (_vm.Step != MarcaStep.VerificandoHuella) return;
            fpRing.SetState(FingerprintState.Scanning);
            VerificarHuella(sample);
        });
    }

    private void VerificarHuella(DPFP.Sample sample)
    {
        try
        {
            var extractor = new DPFP.Processing.FeatureExtraction();
            var feedback  = DPFP.Capture.CaptureFeedback.None;
            var features  = new DPFP.FeatureSet();
            extractor.CreateFeatureSet(sample, DataPurpose.Verification, ref feedback, ref features);
            if (feedback != DPFP.Capture.CaptureFeedback.Good) return;

            var huellas = new ImagenHuella().traerHuellasPorPersona(_idPersona);
            if (huellas == null || huellas.Rows.Count == 0)
            { _vm.SetTrabajador(_vm.NombreTrabajador, _vm.UltimaMarca); return; }

            var verificador = new DPFP.Verification.Verification();
            foreach (DataRow row in huellas.Rows)
            {
                var template = new DPFP.Template();
                template.DeSerialize((byte[])row["huella"]);
                var result = new DPFP.Verification.Verification.Result();
                verificador.Verify(features, template, ref result);
                if (result.Verified) { _vm.SetTrabajador(_vm.NombreTrabajador, _vm.UltimaMarca); return; }
            }
            _vm.SetError("Huella no reconocida. Intente nuevamente.");
        }
        catch (Exception ex) { _vm.SetError(ex.Message); }
    }

    void DPFP.Capture.EventHandler.OnFingerGone(object c, string sn)    { }
    void DPFP.Capture.EventHandler.OnFingerTouch(object c, string sn)   { }
    void DPFP.Capture.EventHandler.OnImageQuality(object c, string sn, DPFP.Capture.CaptureFeedback f) { }
    void DPFP.Capture.EventHandler.OnReaderConnect(object c, string sn) { }
    void DPFP.Capture.EventHandler.OnReaderDisconnect(object c, string sn) { }
    void DPFP.Capture.EventHandler.OnSampleQuality(object c, string sn, DPFP.Capture.CaptureFeedback f) { }

    // ── Window close ─────────────────────────────────────────────────────────
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        _capture?.StopCapture();
        base.OnClosed(e);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static string FormatRut(string raw)
    {
        if (raw.Length < 2) return raw;
        var num = raw[..^1]; var dv = raw[^1];
        return long.TryParse(num, out var n)
            ? $"{n:N0}-{dv}".Replace(",", ".")
            : $"{num}-{dv}";
    }

    private static string GetInitials(string nombre)
    {
        var parts = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? $"{parts[0][0]}{parts[1][0]}" : nombre[..Math.Min(2, nombre.Length)];
    }
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 4: Smoke test — run app and verify WndMarcaBajoTrafico opens**

```powershell
cd C:\soviet_3\RelojControl.App
dotnet run
```

Click "Registrar marca" in WndInicio. WndMarcaBajoTrafico should open at 733×1061, centered, TopMost, with rail visible and keypad active. Verify clock ticking in rail. Close with Ctrl+Esc.

- [ ] **Step 5: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Windows/WndMarcaBajoTrafico.xaml RelojControl.App/Windows/WndMarcaBajoTrafico.xaml.cs
git commit -m "feat: WndMarcaBajoTrafico with all states, DPFP integration, Front Soviet design"
```

---

## Task 14: EnroladorViewModel + Tests

**Files:**
- Create: `C:\soviet_3\RelojControl.App\ViewModels\EnroladorViewModel.cs`
- Create: `C:\soviet_3\RelojControl.Tests\EnroladorViewModelTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
// C:\soviet_3\RelojControl.Tests\EnroladorViewModelTests.cs
using Xunit;
using RelojControl.ViewModels;

namespace RelojControl.Tests;

public class EnroladorViewModelTests
{
    [Fact]
    public void WizardStep_StartsAt_1()
    {
        var vm = new EnroladorViewModel();
        Assert.Equal(1, vm.WizardStep);
    }

    [Fact]
    public void Siguiente_AdvancesStep()
    {
        var vm = new EnroladorViewModel();
        vm.Siguiente();
        Assert.Equal(2, vm.WizardStep);
    }

    [Fact]
    public void Volver_DecrementsStep()
    {
        var vm = new EnroladorViewModel { WizardStep = 3 };
        vm.Volver();
        Assert.Equal(2, vm.WizardStep);
    }

    [Fact]
    public void Siguiente_DoesNotExceed_5()
    {
        var vm = new EnroladorViewModel { WizardStep = 5 };
        vm.Siguiente();
        Assert.Equal(5, vm.WizardStep);
    }

    [Fact]
    public void Volver_DoesNotGoBelowOne()
    {
        var vm = new EnroladorViewModel { WizardStep = 1 };
        vm.Volver();
        Assert.Equal(1, vm.WizardStep);
    }

    [Fact]
    public void NuevaPersna_ResetsWizard_AndClearsFields()
    {
        var vm = new EnroladorViewModel { WizardStep = 3, RutBusqueda = "12345" };
        vm.NuevaPersona();
        Assert.Equal(1, vm.WizardStep);
        Assert.True(vm.IsNuevoModo);
    }

    [Fact]
    public void HuellasCapturadasCount_StartsAt_Zero()
    {
        var vm = new EnroladorViewModel();
        Assert.Equal(0, vm.HuellasCapturadasCount);
    }

    [Fact]
    public void IncrementarHuella_IncrementsCount()
    {
        var vm = new EnroladorViewModel();
        vm.IncrementarHuella();
        Assert.Equal(1, vm.HuellasCapturadasCount);
    }
}
```

- [ ] **Step 2: Run tests — expect FAIL (type not found)**

```powershell
dotnet build C:\soviet_3 ; dotnet test C:\soviet_3\RelojControl.Tests\RelojControl.Tests.csproj --no-build
```

- [ ] **Step 3: Implement EnroladorViewModel.cs**

```csharp
// C:\soviet_3\RelojControl.App\ViewModels\EnroladorViewModel.cs
using System.Collections.ObjectModel;
using RelojControl.Infrastructure;

namespace RelojControl.ViewModels;

public class PersonaItem
{
    public int    Id            { get; set; }
    public string Nombre        { get; set; } = "";
    public string Rut           { get; set; } = "";
    public string Cargo         { get; set; } = "";
    public int    HuellaCount   { get; set; }
    public bool   Enrolado      => HuellaCount > 0;
    public string Initials      => Nombre.Length >= 2 ? Nombre[..2].ToUpper() : Nombre.ToUpper();
}

public class EnroladorViewModel : ViewModelBase
{
    private int    _wizardStep = 1;
    private string _rutBusqueda = "";
    private bool   _isNuevoModo;
    private int    _huellasCapturadasCount;
    private PersonaItem? _personaActual;

    public int    WizardStep            { get => _wizardStep;             set => Set(ref _wizardStep, value); }
    public string RutBusqueda           { get => _rutBusqueda;            set => Set(ref _rutBusqueda, value); }
    public bool   IsNuevoModo           { get => _isNuevoModo;            set => Set(ref _isNuevoModo, value); }
    public int    HuellasCapturadasCount{ get => _huellasCapturadasCount; set => Set(ref _huellasCapturadasCount, value); }
    public PersonaItem? PersonaActual   { get => _personaActual;          set => Set(ref _personaActual, value); }

    public ObservableCollection<PersonaItem> Personas { get; } = new();

    // Wizard navigation
    public void Siguiente() { if (WizardStep < 5) WizardStep++; }
    public void Volver()    { if (WizardStep > 1) WizardStep--; }

    public void NuevaPersona()
    {
        PersonaActual            = new PersonaItem();
        WizardStep               = 1;
        IsNuevoModo              = true;
        HuellasCapturadasCount   = 0;
    }

    public void EditarPersona(PersonaItem p)
    {
        PersonaActual          = p;
        WizardStep             = 1;
        IsNuevoModo            = false;
        HuellasCapturadasCount = p.HuellaCount;
    }

    public void IncrementarHuella() => HuellasCapturadasCount++;

    public bool IsLastStep => WizardStep == 5;
}
```

- [ ] **Step 4: Run tests — expect PASS**

```powershell
dotnet build C:\soviet_3
dotnet test C:\soviet_3\RelojControl.Tests\RelojControl.Tests.csproj --no-build -v normal
```

Expected: `Passed: 16, Failed: 0` (8 infra + 8 enrolador)

- [ ] **Step 5: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/ViewModels/EnroladorViewModel.cs RelojControl.Tests/EnroladorViewModelTests.cs
git commit -m "feat: EnroladorViewModel with wizard navigation (TDD)"
```

---

## Task 15: WndEnrolador — Layout, Sidebar, Tabs 1–2

**Files:**
- Create: `C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml`
- Create: `C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml.cs`

- [ ] **Step 1: Create WndEnrolador.xaml**

```xml
<!-- C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml -->
<Window x:Class="RelojControl.Windows.WndEnrolador"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:ctrl="clr-namespace:RelojControl.Controls"
        WindowStyle="None" Topmost="True" WindowState="Maximized"
        WindowStartupLocation="CenterScreen"
        Background="{StaticResource BgBrush}">

  <Grid>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="88"/>
      <ColumnDefinition/>
    </Grid.ColumnDefinitions>

    <!-- ═══ WHITE SIDEBAR ═══ -->
    <Border Grid.Column="0" Background="{StaticResource Surface2Brush}"
            BorderBrush="{StaticResource LineBrush}" BorderThickness="0,0,1,0">
      <DockPanel>
        <TextBlock DockPanel.Dock="Top" Margin="0,20,0,0" HorizontalAlignment="Center"
                   FontFamily="{StaticResource FontPoppins}" FontWeight="ExtraBold" FontSize="11"
                   Foreground="{StaticResource AccentBrush}" LetterSpacing="1">
          rFlex<LineBreak/>
          <Run FontSize="8" FontWeight="Normal" Foreground="{StaticResource Ink3Brush}">· · · ·</Run>
        </TextBlock>

        <Button DockPanel.Dock="Top" Margin="12,24,12,0" Padding="0,12"
                Style="{StaticResource BtnAccent}" Background="{StaticResource AccentSoftBrush}"
                Click="BtnNuevo_Click">
          <StackPanel HorizontalAlignment="Center" Spacing="4">
            <TextBlock Text="👤" FontSize="20" HorizontalAlignment="Center"/>
            <TextBlock Text="Enrolar" FontFamily="{StaticResource FontPoppins}" FontSize="10"
                       FontWeight="SemiBold" Foreground="{StaticResource AccentBrush}"
                       HorizontalAlignment="Center"/>
          </StackPanel>
        </Button>

        <Grid/>
      </DockPanel>
    </Border>

    <!-- ═══ MAIN AREA ═══ -->
    <Grid Grid.Column="1">
      <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition/>
        <RowDefinition Height="Auto"/>
      </Grid.RowDefinitions>

      <!-- HEADER -->
      <Border Grid.Row="0" BorderBrush="{StaticResource LineBrush}" BorderThickness="0,0,0,1"
              Padding="32,16">
        <Grid>
          <Grid.ColumnDefinitions>
            <ColumnDefinition/>
            <ColumnDefinition Width="Auto"/>
          </Grid.ColumnDefinitions>
          <StackPanel Grid.Column="0">
            <TextBlock Text="Panel de Control · Administración de Usuarios"
                       FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="17"
                       Foreground="{StaticResource InkBrush}"/>
            <TextBlock x:Name="lblEstacion"
                       FontFamily="{StaticResource FontPoppins}" FontSize="12"
                       Foreground="{StaticResource Ink3Brush}"/>
          </StackPanel>
          <StackPanel Grid.Column="1" Orientation="Horizontal" Spacing="12" VerticalAlignment="Center">
            <Border Width="34" Height="34" CornerRadius="{StaticResource RadiusBadge}"
                    Background="{StaticResource AccentBrush}">
              <TextBlock x:Name="lblUserInitials" FontFamily="{StaticResource FontPoppins}"
                         FontWeight="Bold" FontSize="12" Foreground="White"
                         HorizontalAlignment="Center" VerticalAlignment="Center"/>
            </Border>
            <TextBlock x:Name="lblUserName" Style="{StaticResource TextSubtitle}" VerticalAlignment="Center"/>
            <Button Content="↩ Salir" Style="{StaticResource BtnSecondary}"
                    Padding="12,6" Click="BtnSalir_Click"/>
          </StackPanel>
        </Grid>
      </Border>

      <!-- BUSCADOR -->
      <Grid Grid.Row="1" Margin="32,16,32,0">
        <Grid.ColumnDefinitions>
          <ColumnDefinition Width="280"/>
          <ColumnDefinition Width="12"/>
          <ColumnDefinition Width="Auto"/>
          <ColumnDefinition Width="12"/>
          <ColumnDefinition Width="Auto"/>
          <ColumnDefinition Width="12"/>
          <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>
        <TextBox Grid.Column="0" x:Name="txtRutBusqueda" Style="{StaticResource InputBase}"
                 Text="{Binding RutBusqueda, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                 KeyDown="TxtRut_KeyDown"/>
        <Button Grid.Column="2" Content="🔍 Buscar" Style="{StaticResource BtnAccent}"
                Padding="16,10" Click="BtnBuscar_Click"/>
        <Button Grid.Column="4" Content="+ Funcionario nuevo" Style="{StaticResource BtnSecondary}"
                Padding="16,10" Click="BtnNuevo_Click"/>
        <Button Grid.Column="6" Content="Limpiar" Style="{StaticResource BtnSecondary}"
                Padding="16,10" Click="BtnLimpiar_Click"/>
      </Grid>

      <!-- TARJETA TRABAJADOR -->
      <Border Grid.Row="2" x:Name="cardPersona" Margin="32,16,32,0"
              Background="{StaticResource SurfaceBrush}"
              BorderBrush="{StaticResource LineBrush}" BorderThickness="1"
              CornerRadius="{StaticResource RadiusCard}" Padding="20,16"
              Visibility="Collapsed">
        <Grid>
          <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto"/>
            <ColumnDefinition/>
            <ColumnDefinition Width="Auto"/>
          </Grid.ColumnDefinitions>
          <Border Grid.Column="0" Width="48" Height="48" CornerRadius="{StaticResource RadiusButton}"
                  Margin="0,0,14,0">
            <Border.Background>
              <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                <GradientStop Color="#616EB3" Offset="0"/>
                <GradientStop Color="#3F4690" Offset="1"/>
              </LinearGradientBrush>
            </Border.Background>
            <TextBlock x:Name="lblPersonaInitials" FontFamily="{StaticResource FontPoppins}"
                       FontWeight="Bold" FontSize="16" Foreground="White"
                       HorizontalAlignment="Center" VerticalAlignment="Center"/>
          </Border>
          <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="3">
            <TextBlock x:Name="lblPersonaNombre" FontFamily="{StaticResource FontPoppins}"
                       FontWeight="Bold" FontSize="16" Foreground="{StaticResource InkBrush}"/>
            <TextBlock x:Name="lblPersonaInfo" Style="{StaticResource TextSubtitle}"/>
          </StackPanel>
          <Border Grid.Column="2" x:Name="badgeGuardado" CornerRadius="{StaticResource RadiusBadge}"
                  Padding="12,5" Background="{StaticResource TealSoftBrush}" VerticalAlignment="Center">
            <TextBlock Text="● Guardado" FontFamily="{StaticResource FontPoppins}"
                       FontWeight="SemiBold" FontSize="12" Foreground="{StaticResource TealBrush}"/>
          </Border>
        </Grid>
      </Border>

      <!-- TABS -->
      <Border Grid.Row="3" x:Name="panelTabs" BorderBrush="{StaticResource LineBrush}"
              BorderThickness="0,0,0,1" Margin="32,12,32,0" Visibility="Collapsed">
        <StackPanel Orientation="Horizontal">
          <Button x:Name="tab1" Content="① Datos generales" Style="{StaticResource TabBtn}"
                  Tag="1" Click="Tab_Click"/>
          <Button x:Name="tab2" Content="② Asistencia"      Style="{StaticResource TabBtn}"
                  Tag="2" Click="Tab_Click"/>
          <Button x:Name="tab3" Content="③ Huellas"          Style="{StaticResource TabBtn}"
                  Tag="3" Click="Tab_Click"/>
          <Button x:Name="tab4" Content="④ Comidas"          Style="{StaticResource TabBtn}"
                  Tag="4" Click="Tab_Click"/>
          <Button x:Name="tab5" Content="⑤ Relojes"          Style="{StaticResource TabBtn}"
                  Tag="5" Click="Tab_Click"/>
        </StackPanel>
      </Border>

      <!-- TAB CONTENT -->
      <ScrollViewer Grid.Row="4" x:Name="scrollContent" Margin="32,20,32,0"
                    VerticalScrollBarVisibility="Auto" Visibility="Collapsed">
        <StackPanel>

          <!-- TAB 1: DATOS GENERALES -->
          <StackPanel x:Name="contentTab1" Spacing="20">
            <TextBlock Text="IDENTIFICACIÓN" Style="{StaticResource TextEyebrow}"/>
            <Grid>
              <Grid.ColumnDefinitions>
                <ColumnDefinition/><ColumnDefinition Width="16"/>
                <ColumnDefinition/><ColumnDefinition Width="16"/>
                <ColumnDefinition/>
              </Grid.ColumnDefinitions>
              <StackPanel Grid.Column="0" Spacing="6">
                <TextBlock Text="Nombre *" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtNombre" Style="{StaticResource InputBase}"/>
              </StackPanel>
              <StackPanel Grid.Column="2" Spacing="6">
                <TextBlock Text="Segundo nombre" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtSegundoNombre" Style="{StaticResource InputBase}"/>
              </StackPanel>
              <StackPanel Grid.Column="4" Spacing="6">
                <TextBlock Text="Alias" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtAlias" Style="{StaticResource InputBase}"/>
              </StackPanel>
            </Grid>
            <Grid>
              <Grid.ColumnDefinitions>
                <ColumnDefinition/><ColumnDefinition Width="16"/>
                <ColumnDefinition/><ColumnDefinition Width="16"/>
                <ColumnDefinition/>
              </Grid.ColumnDefinitions>
              <StackPanel Grid.Column="0" Spacing="6">
                <TextBlock Text="Apellido paterno *" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtApellidoPaterno" Style="{StaticResource InputBase}"/>
              </StackPanel>
              <StackPanel Grid.Column="2" Spacing="6">
                <TextBlock Text="Apellido materno *" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtApellidoMaterno" Style="{StaticResource InputBase}"/>
              </StackPanel>
              <StackPanel Grid.Column="4" Spacing="6">
                <TextBlock Text="Fecha de nacimiento" Style="{StaticResource TextHint}"/>
                <DatePicker x:Name="dpFechaNac" FontFamily="{StaticResource FontPoppins}"/>
              </StackPanel>
            </Grid>
            <TextBlock Text="CONTACTO" Style="{StaticResource TextEyebrow}" Margin="0,8,0,0"/>
            <Grid>
              <Grid.ColumnDefinitions>
                <ColumnDefinition/><ColumnDefinition Width="16"/>
                <ColumnDefinition/><ColumnDefinition Width="16"/>
                <ColumnDefinition/>
              </Grid.ColumnDefinitions>
              <StackPanel Grid.Column="0" Spacing="6">
                <TextBlock Text="Teléfono" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtTelefono" Style="{StaticResource InputBase}"/>
              </StackPanel>
              <StackPanel Grid.Column="2" Spacing="6">
                <TextBlock Text="Teléfono alternativo" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtTelefonoAlt" Style="{StaticResource InputBase}"/>
              </StackPanel>
              <StackPanel Grid.Column="4" Spacing="6">
                <TextBlock Text="Correo electrónico *" Style="{StaticResource TextHint}"/>
                <TextBox x:Name="txtCorreo" Style="{StaticResource InputBase}"/>
              </StackPanel>
            </Grid>
            <TextBlock Text="ORGANIZACIÓN" Style="{StaticResource TextEyebrow}" Margin="0,8,0,0"/>
            <StackPanel Spacing="6" MaxWidth="400" HorizontalAlignment="Left">
              <TextBlock Text="Empresa" Style="{StaticResource TextHint}"/>
              <TextBox x:Name="txtEmpresa" Style="{StaticResource InputBase}" IsReadOnly="True"
                       Background="{StaticResource Surface2Brush}"/>
            </StackPanel>
          </StackPanel>

          <!-- TAB 2: ASISTENCIA -->
          <StackPanel x:Name="contentTab2" Spacing="16" Visibility="Collapsed">
            <TextBlock Text="ACCESOS Y PERMISOS" Style="{StaticResource TextEyebrow}"/>
            <CheckBox x:Name="chkPermiteJornada"  Content="Permite marca de jornada (entrada/salida)"
                      FontFamily="{StaticResource FontPoppins}" FontSize="14" Foreground="{StaticResource InkBrush}"/>
            <CheckBox x:Name="chkPermiteComida"   Content="Permite marca de comida/casino"
                      FontFamily="{StaticResource FontPoppins}" FontSize="14" Foreground="{StaticResource InkBrush}"/>
            <CheckBox x:Name="chkHabilitado"      Content="Funcionario habilitado"
                      FontFamily="{StaticResource FontPoppins}" FontSize="14" Foreground="{StaticResource InkBrush}" IsChecked="True"/>
            <TextBlock Text="ROL DE USUARIO" Style="{StaticResource TextEyebrow}" Margin="0,8,0,0"/>
            <StackPanel Spacing="6" MaxWidth="300" HorizontalAlignment="Left">
              <TextBlock Text="Tipo de rol" Style="{StaticResource TextHint}"/>
              <ComboBox x:Name="cboRol" FontFamily="{StaticResource FontPoppins}" FontSize="14"
                        Padding="12,8" Background="{StaticResource SurfaceBrush}"/>
            </StackPanel>
          </StackPanel>

          <!-- TAB 3: HUELLAS (placeholder — fully implemented in Task 16) -->
          <StackPanel x:Name="contentTab3" Visibility="Collapsed" HorizontalAlignment="Center"
                      VerticalAlignment="Center" Spacing="20">
            <TextBlock Text="Captura de huellas — se implementa en Task 16"
                       Style="{StaticResource TextSubtitle}" HorizontalAlignment="Center"/>
          </StackPanel>

          <!-- TAB 4: COMIDAS (placeholder — Task 17) -->
          <StackPanel x:Name="contentTab4" Visibility="Collapsed">
            <TextBlock Text="Configuración de comidas — Task 17" Style="{StaticResource TextSubtitle}"/>
          </StackPanel>

          <!-- TAB 5: RELOJES (placeholder — Task 17) -->
          <StackPanel x:Name="contentTab5" Visibility="Collapsed">
            <TextBlock Text="Asignación de relojes — Task 17" Style="{StaticResource TextSubtitle}"/>
          </StackPanel>

        </StackPanel>
      </ScrollViewer>

      <!-- FOOTER NAVIGATION -->
      <Border Grid.Row="5" x:Name="panelNav" BorderBrush="{StaticResource LineBrush}"
              BorderThickness="0,1,0,0" Padding="32,16" Visibility="Collapsed">
        <Grid>
          <Button x:Name="btnVolver" Content="← Volver" Style="{StaticResource BtnSecondary}"
                  HorizontalAlignment="Left" Padding="20,10" Click="BtnWizardVolver_Click"/>
          <Button x:Name="btnSiguiente" Content="Siguiente →" Style="{StaticResource BtnAccent}"
                  HorizontalAlignment="Right" Padding="20,10" Click="BtnWizardSiguiente_Click"/>
        </Grid>
      </Border>
    </Grid>
  </Grid>

  <Window.Resources>
    <Style x:Key="TabBtn" TargetType="Button">
      <Setter Property="Background"     Value="Transparent"/>
      <Setter Property="Foreground"     Value="{StaticResource Ink3Brush}"/>
      <Setter Property="FontFamily"     Value="{StaticResource FontPoppins}"/>
      <Setter Property="FontSize"       Value="13"/>
      <Setter Property="BorderThickness" Value="0,0,0,2"/>
      <Setter Property="BorderBrush"    Value="Transparent"/>
      <Setter Property="Padding"        Value="16,10"/>
      <Setter Property="Cursor"         Value="Hand"/>
      <Setter Property="Template">
        <Setter.Value>
          <ControlTemplate TargetType="Button">
            <Border BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    Padding="{TemplateBinding Padding}">
              <ContentPresenter/>
            </Border>
          </ControlTemplate>
        </Setter.Value>
      </Setter>
    </Style>
  </Window.Resources>
</Window>
```

- [ ] **Step 2: Create WndEnrolador.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using proyectoNegocioRflex.Modelo;
using RelojControl.ViewModels;

namespace RelojControl.Windows;

public partial class WndEnrolador : Window
{
    private readonly EnroladorViewModel _vm = new();
    private int _idReloj;

    public string RutAdmin     { get; set; } = "";
    public string CodigoEmpresa{ get; set; } = "";

    public WndEnrolador(int idReloj = 0)
    {
        InitializeComponent();
        _idReloj    = idReloj;
        DataContext = _vm;
        lblEstacion.Text = $"Estación {Environment.MachineName} · Reloj Control";
        lblUserInitials.Text = RutAdmin.Length >= 2 ? RutAdmin[..2] : RutAdmin;
        lblUserName.Text     = RutAdmin;
        txtEmpresa.Text      = CodigoEmpresa;
    }

    // ── Búsqueda ──────────────────────────────────────────────────────────────
    private void TxtRut_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Return) BuscarPersona(); }
    private void BtnBuscar_Click(object s, RoutedEventArgs e) => BuscarPersona();

    private void BuscarPersona()
    {
        var enc = new Encriptacion();
        var p   = new Persona();
        var dt  = p.traerDatosPersonaPorRut(enc.Encriptar(_vm.RutBusqueda.Replace(".", "").Replace("-", "")));
        if (dt == null || dt.Rows.Count == 0)
        {
            MessageBox.Show("Persona no encontrada.", "Búsqueda");
            return;
        }
        CargarPersona(dt);
    }

    private void CargarPersona(System.Data.DataTable dt)
    {
        var row = dt.Rows[0];
        var item = new PersonaItem
        {
            Id     = int.Parse(row[0].ToString()!),
            Nombre = $"{row[2]} {row[3]} {row[4]} {row[5]}",
            Rut    = row[1].ToString()!,
            Cargo  = row[6].ToString() ?? "",
        };
        _vm.EditarPersona(item);
        MostrarFormulario(item);
    }

    private void MostrarFormulario(PersonaItem item)
    {
        cardPersona.Visibility  = Visibility.Visible;
        panelTabs.Visibility    = Visibility.Visible;
        scrollContent.Visibility= Visibility.Visible;
        panelNav.Visibility     = Visibility.Visible;

        lblPersonaNombre.Text   = item.Nombre.ToUpper();
        lblPersonaInfo.Text     = $"{item.Cargo} · {item.Rut}";
        lblPersonaInitials.Text = item.Initials;

        txtNombre.Text         = item.Nombre.Split(' ').Length > 0 ? item.Nombre.Split(' ')[0] : "";
        txtApellidoPaterno.Text= item.Nombre.Split(' ').Length > 2 ? item.Nombre.Split(' ')[2] : "";
        txtEmpresa.Text        = CodigoEmpresa;

        ActivarTab(1);
    }

    private void BtnNuevo_Click(object s, RoutedEventArgs e)
    {
        _vm.NuevaPersona();
        cardPersona.Visibility   = Visibility.Visible;
        panelTabs.Visibility     = Visibility.Visible;
        scrollContent.Visibility = Visibility.Visible;
        panelNav.Visibility      = Visibility.Visible;
        LimpiarFormulario();
        lblPersonaNombre.Text    = "NUEVO FUNCIONARIO";
        lblPersonaInfo.Text      = "Sin guardar";
        lblPersonaInitials.Text  = "NF";
        ActivarTab(1);
    }

    private void BtnLimpiar_Click(object s, RoutedEventArgs e)
    {
        _vm.RutBusqueda = "";
        cardPersona.Visibility   = Visibility.Collapsed;
        panelTabs.Visibility     = Visibility.Collapsed;
        scrollContent.Visibility = Visibility.Collapsed;
        panelNav.Visibility      = Visibility.Collapsed;
    }

    // ── Tabs ──────────────────────────────────────────────────────────────────
    private void Tab_Click(object s, RoutedEventArgs e)
    {
        if (s is Button btn && int.TryParse(btn.Tag?.ToString(), out int t))
            ActivarTab(t);
    }

    private void ActivarTab(int tab)
    {
        _vm.WizardStep = tab;
        contentTab1.Visibility = tab == 1 ? Visibility.Visible : Visibility.Collapsed;
        contentTab2.Visibility = tab == 2 ? Visibility.Visible : Visibility.Collapsed;
        contentTab3.Visibility = tab == 3 ? Visibility.Visible : Visibility.Collapsed;
        contentTab4.Visibility = tab == 4 ? Visibility.Visible : Visibility.Collapsed;
        contentTab5.Visibility = tab == 5 ? Visibility.Visible : Visibility.Collapsed;

        foreach (var btn in new[] { tab1, tab2, tab3, tab4, tab5 })
        {
            bool active = btn.Tag?.ToString() == tab.ToString();
            btn.Foreground = active
                ? (System.Windows.Media.Brush)FindResource("AccentBrush")
                : (System.Windows.Media.Brush)FindResource("Ink3Brush");
            btn.BorderBrush = active
                ? (System.Windows.Media.Brush)FindResource("AccentBrush")
                : System.Windows.Media.Brushes.Transparent;
            btn.FontWeight = active ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal;
        }

        btnVolver.IsEnabled    = tab > 1;
        btnSiguiente.Content   = tab == 5 ? "Guardar" : "Siguiente →";
    }

    private void BtnWizardVolver_Click(object s, RoutedEventArgs e)
    {
        _vm.Volver(); ActivarTab(_vm.WizardStep);
    }

    private void BtnWizardSiguiente_Click(object s, RoutedEventArgs e)
    {
        if (_vm.IsLastStep) { Guardar(); return; }
        _vm.Siguiente(); ActivarTab(_vm.WizardStep);
    }

    private void Guardar()
    {
        MessageBox.Show("Guardado correctamente.", "Enrolador");
        badgeGuardado.Visibility = Visibility.Visible;
    }

    private void LimpiarFormulario()
    {
        txtNombre.Text = txtSegundoNombre.Text = txtAlias.Text = "";
        txtApellidoPaterno.Text = txtApellidoMaterno.Text = "";
        txtTelefono.Text = txtTelefonoAlt.Text = txtCorreo.Text = "";
    }

    private void BtnSalir_Click(object s, RoutedEventArgs e) => Close();

    protected override void OnClosed(System.EventArgs e)
    {
        base.OnClosed(e);
        if (Owner is WndInicio inicio) inicio.OnChildClosed();
    }
}
```

- [ ] **Step 3: Fix WndInicio BtnLogin to pass RutAdmin to WndEnrolador**

In `WndInicio.xaml.cs`, replace the `frmPanelDeControl` instantiation with:

```csharp
var enrolador = new WndEnrolador(_idReloj)
{
    RutAdmin      = txtUsuario.Text,
    CodigoEmpresa = _codigoEmpresa,
    Owner         = this,
};
enrolador.Show();
Hide();
```

> Only update the `BtnLogin_Click` block — do NOT change anything else in the file.

- [ ] **Step 4: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 5: Smoke test — run app, login, verify WndEnrolador opens**

```powershell
cd C:\soviet_3\RelojControl.App && dotnet run
```

Enter `admin` / `rflex`. WndEnrolador should open fullscreen with white sidebar, header, search bar.

- [ ] **Step 6: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Windows/WndEnrolador.xaml RelojControl.App/Windows/WndEnrolador.xaml.cs RelojControl.App/Windows/WndInicio.xaml.cs
git commit -m "feat: WndEnrolador with sidebar, header, tabs 1-2 and search"
```

---

## Task 16: WndEnrolador Tab 3 — Captura de Huellas (DPFP)

Replace the placeholder `contentTab3` with real DigitalPersona fingerprint capture.

**Files:**
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml` (contentTab3)
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml.cs`

- [ ] **Step 1: Replace contentTab3 in WndEnrolador.xaml**

Find and replace the `contentTab3` StackPanel with:

```xml
<!-- TAB 3: HUELLAS -->
<Grid x:Name="contentTab3" Visibility="Collapsed">
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="Auto"/>
    <ColumnDefinition/>
  </Grid.ColumnDefinitions>

  <!-- Capture area left -->
  <StackPanel Grid.Column="0" Width="320" Spacing="20" Margin="0,0,40,0">
    <TextBlock Text="CAPTURA DE HUELLAS" Style="{StaticResource TextEyebrow}"/>
    <TextBlock Text="Apoye el mismo dedo 4 veces para registrar la huella."
               Style="{StaticResource TextSubtitle}" TextWrapping="Wrap"/>

    <ctrl:FingerprintRing x:Name="fpEnrolRing" HorizontalAlignment="Left"/>

    <!-- Progress dots -->
    <StackPanel Orientation="Horizontal" Spacing="10">
      <Ellipse x:Name="dot1" Width="18" Height="18" Fill="{StaticResource LineBrush}"/>
      <Ellipse x:Name="dot2" Width="18" Height="18" Fill="{StaticResource LineBrush}"/>
      <Ellipse x:Name="dot3" Width="18" Height="18" Fill="{StaticResource LineBrush}"/>
      <Ellipse x:Name="dot4" Width="18" Height="18" Fill="{StaticResource LineBrush}"/>
    </StackPanel>
    <TextBlock x:Name="lblHuellaStatus" Text="Apoye el dedo en el lector..."
               FontFamily="{StaticResource FontPoppins}" FontSize="14"
               Foreground="{StaticResource AccentBrush}"/>

    <Button Content="Saltar este paso" Style="{StaticResource BtnSecondary}"
            HorizontalAlignment="Left" Padding="16,8" Click="BtnSkipHuellas_Click"/>
  </StackPanel>

  <!-- Huellas registradas list right -->
  <StackPanel Grid.Column="1" Spacing="12">
    <TextBlock Text="HUELLAS REGISTRADAS" Style="{StaticResource TextEyebrow}"/>
    <ItemsControl x:Name="listaHuellas">
      <ItemsControl.ItemTemplate>
        <DataTemplate>
          <Border Background="{StaticResource Surface2Brush}" CornerRadius="{StaticResource RadiusInput}"
                  Padding="14,10" Margin="0,0,0,6">
            <Grid>
              <TextBlock Text="{Binding}" FontFamily="{StaticResource FontPoppins}" FontSize="13"
                         Foreground="{StaticResource InkBrush}"/>
              <TextBlock Text="✓" HorizontalAlignment="Right"
                         FontFamily="{StaticResource FontPoppins}" FontWeight="Bold"
                         Foreground="{StaticResource TealBrush}"/>
            </Grid>
          </Border>
        </DataTemplate>
      </ItemsControl.ItemTemplate>
    </ItemsControl>
  </StackPanel>
</Grid>
```

- [ ] **Step 2: Add DPFP capture logic to WndEnrolador.xaml.cs**

Add these members and methods to `WndEnrolador` class:

```csharp
// Add at top of class:
private DPFP.Capture.Capture? _captureEnrol;
private DPFP.Processing.Enrollment? _enrollment;
private readonly System.Collections.ObjectModel.ObservableCollection<string> _huellaLabels = new();
private int _dedoActual = 1;

// Add to constructor (after InitializeComponent):
listaHuellas.ItemsSource = _huellaLabels;

// Add these methods:
private void IniciarCapturaEnrol()
{
    try
    {
        _enrollment   = new DPFP.Processing.Enrollment();
        _captureEnrol = new DPFP.Capture.Capture(DPFP.Capture.DataPurpose.Enrollment);
        _captureEnrol.EventHandler = new EnrolCapHandler(this);
        _captureEnrol.StartCapture();
        fpEnrolRing.SetState(FingerprintState.Scanning);
    }
    catch { lblHuellaStatus.Text = "Lector no disponible."; }
}

private void StopCapturaEnrol()
{
    _captureEnrol?.StopCapture();
}

internal void OnEnrolSample(DPFP.Sample sample)
{
    Dispatcher.Invoke(() =>
    {
        var ext      = new DPFP.Processing.FeatureExtraction();
        var feedback = DPFP.Capture.CaptureFeedback.None;
        var features = new DPFP.FeatureSet();
        ext.CreateFeatureSet(sample, DPFP.Capture.DataPurpose.Enrollment, ref feedback, ref features);
        if (feedback != DPFP.Capture.CaptureFeedback.Good)
        {
            lblHuellaStatus.Text = "Calidad insuficiente. Intente nuevamente.";
            return;
        }

        _enrollment!.AddFeatures(features);
        _vm.IncrementarHuella();
        ActualizarDots(_vm.HuellasCapturadasCount);

        if (_enrollment.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
        {
            GuardarHuella();
        }
        else
        {
            lblHuellaStatus.Text = $"Capture {4 - _vm.HuellasCapturadasCount % 4} veces más...";
        }
    });
}

private void GuardarHuella()
{
    var template = new System.IO.MemoryStream();
    _enrollment!.Template.Serialize(template);

    var ih = new ImagenHuella
    {
        persona_idpersona = _vm.PersonaActual?.Id ?? 0,
        imagen_huella     = template.ToArray(),
    };
    ih.guardarImagenHuella();

    _huellaLabels.Add($"Huella {_dedoActual}");
    _dedoActual++;
    _vm.HuellasCapturadasCount = (_dedoActual - 1) * 4;
    ActualizarDots(0);

    _enrollment = new DPFP.Processing.Enrollment();
    fpEnrolRing.SetState(FingerprintState.Success);
    lblHuellaStatus.Text = "¡Huella guardada! Puede capturar otra o continuar.";
    badgeGuardado.Visibility = Visibility.Collapsed; // mark unsaved
}

private void ActualizarDots(int count)
{
    var dots = new[] { dot1, dot2, dot3, dot4 };
    var filled = (System.Windows.Media.Brush)FindResource("AccentBrush");
    var empty  = (System.Windows.Media.Brush)FindResource("LineBrush");
    for (int i = 0; i < 4; i++)
        dots[i].Fill = i < (count % 4 == 0 && count > 0 ? 4 : count % 4) ? filled : empty;
}

private void BtnSkipHuellas_Click(object s, RoutedEventArgs e)
{
    StopCapturaEnrol(); _vm.Siguiente(); ActivarTab(_vm.WizardStep);
}
```

Add when Tab 3 is activated (inside `ActivarTab`):

```csharp
if (tab == 3) IniciarCapturaEnrol();
else          StopCapturaEnrol();
```

Add inner handler class at end of file (outside WndEnrolador, inside namespace):

```csharp
internal class EnrolCapHandler : DPFP.Capture.EventHandler
{
    private readonly WndEnrolador _parent;
    public EnrolCapHandler(WndEnrolador p) => _parent = p;
    public void OnComplete(object c, string sn, DPFP.Sample s)         => _parent.OnEnrolSample(s);
    public void OnFingerGone(object c, string sn)                       { }
    public void OnFingerTouch(object c, string sn)                      { }
    public void OnImageQuality(object c, string sn, DPFP.Capture.CaptureFeedback f) { }
    public void OnReaderConnect(object c, string sn)                    { }
    public void OnReaderDisconnect(object c, string sn)                 { }
    public void OnSampleQuality(object c, string sn, DPFP.Capture.CaptureFeedback f) { }
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Windows/WndEnrolador.xaml RelojControl.App/Windows/WndEnrolador.xaml.cs
git commit -m "feat: WndEnrolador Tab 3 fingerprint capture with DPFP SDK"
```

---

## Task 17: WndEnrolador Tabs 4–5 (Comidas + Relojes)

**Files:**
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml` (contentTab4, contentTab5)
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndEnrolador.xaml.cs`

- [ ] **Step 1: Replace contentTab4 placeholder in WndEnrolador.xaml**

```xml
<!-- TAB 4: COMIDAS -->
<StackPanel x:Name="contentTab4" Visibility="Collapsed" Spacing="16">
  <TextBlock Text="TIPOS DE COMIDA HABILITADOS" Style="{StaticResource TextEyebrow}"/>
  <ItemsControl x:Name="listaTiposComida">
    <ItemsControl.ItemTemplate>
      <DataTemplate>
        <Border Background="{StaticResource SurfaceBrush}" BorderBrush="{StaticResource LineBrush}"
                BorderThickness="1" CornerRadius="{StaticResource RadiusInput}"
                Padding="16,12" Margin="0,0,0,8">
          <Grid>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <CheckBox Grid.Column="0" IsChecked="{Binding Habilitado}" VerticalAlignment="Center"
                      Margin="0,0,14,0"/>
            <StackPanel Grid.Column="1" VerticalAlignment="Center">
              <TextBlock Text="{Binding Nombre}" FontFamily="{StaticResource FontPoppins}"
                         FontWeight="SemiBold" FontSize="14" Foreground="{StaticResource InkBrush}"/>
              <TextBlock Text="{Binding Horario}" Style="{StaticResource TextHint}"/>
            </StackPanel>
            <Border Grid.Column="2" CornerRadius="{StaticResource RadiusBadge}" Padding="10,4"
                    Background="{StaticResource AmberSoftBrush}">
              <TextBlock Text="{Binding Valor}" FontFamily="{StaticResource FontPoppins}"
                         FontWeight="SemiBold" FontSize="12" Foreground="{StaticResource AmberBrush}"/>
            </Border>
          </Grid>
        </Border>
      </DataTemplate>
    </ItemsControl.ItemTemplate>
  </ItemsControl>
</StackPanel>
```

- [ ] **Step 2: Replace contentTab5 placeholder in WndEnrolador.xaml**

```xml
<!-- TAB 5: RELOJES -->
<StackPanel x:Name="contentTab5" Visibility="Collapsed" Spacing="16">
  <TextBlock Text="RELOJES ASIGNADOS" Style="{StaticResource TextEyebrow}"/>
  <ItemsControl x:Name="listaRelojes">
    <ItemsControl.ItemTemplate>
      <DataTemplate>
        <Border Background="{StaticResource SurfaceBrush}" BorderBrush="{StaticResource LineBrush}"
                BorderThickness="1" CornerRadius="{StaticResource RadiusInput}"
                Padding="16,12" Margin="0,0,0,8">
          <Grid>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <CheckBox Grid.Column="0" IsChecked="{Binding Asignado}" VerticalAlignment="Center"
                      Margin="0,0,14,0"/>
            <StackPanel Grid.Column="1" VerticalAlignment="Center">
              <TextBlock Text="{Binding Nombre}" FontFamily="{StaticResource FontPoppins}"
                         FontWeight="SemiBold" FontSize="14" Foreground="{StaticResource InkBrush}"/>
              <TextBlock Text="{Binding Ubicacion}" Style="{StaticResource TextHint}"/>
            </StackPanel>
          </Grid>
        </Border>
      </DataTemplate>
    </ItemsControl.ItemTemplate>
  </ItemsControl>
</StackPanel>
```

- [ ] **Step 3: Add data loading for tabs 4–5 in WndEnrolador.xaml.cs**

Add inner DTOs and loading logic:

```csharp
// Add inner DTOs inside namespace (not inside class):
public class TipoComidaItem
{
    public string Nombre    { get; set; } = "";
    public string Horario   { get; set; } = "";
    public string Valor     { get; set; } = "";
    public bool   Habilitado{ get; set; }
    public int    Id        { get; set; }
}

public class RelojItem
{
    public int    Id       { get; set; }
    public string Nombre   { get; set; } = "";
    public string Ubicacion{ get; set; } = "";
    public bool   Asignado { get; set; }
}
```

Add method to `WndEnrolador` class:

```csharp
private void CargarTab4(int idPersona, int idSucursal)
{
    var items = new System.Collections.ObjectModel.ObservableCollection<TipoComidaItem>();
    try
    {
        var tc = new SucursalTipoComida();
        var dt = tc.traerTiposComidaPorSucursal(idSucursal);
        if (dt != null)
            foreach (System.Data.DataRow r in dt.Rows)
                items.Add(new TipoComidaItem
                {
                    Id       = int.Parse(r[0].ToString()!),
                    Nombre   = r[1].ToString()!,
                    Horario  = $"{r[3]} - {r[4]}",
                    Valor    = r[2].ToString()!,
                    Habilitado = false,
                });
    }
    catch { }
    listaTiposComida.ItemsSource = items;
}

private void CargarTab5(int idPersona, int idSucursal)
{
    var items = new System.Collections.ObjectModel.ObservableCollection<RelojItem>();
    try
    {
        var r  = new Reloj();
        var dt = r.traerRelojesPorSucursal(idSucursal);
        if (dt != null)
            foreach (System.Data.DataRow row in dt.Rows)
                items.Add(new RelojItem
                {
                    Id       = int.Parse(row[0].ToString()!),
                    Nombre   = row[3].ToString()!,
                    Ubicacion= row[4].ToString()!,
                    Asignado = false,
                });
    }
    catch { }
    listaRelojes.ItemsSource = items;
}
```

In `ActivarTab`, add:

```csharp
if (tab == 4 && _vm.PersonaActual != null) CargarTab4(_vm.PersonaActual.Id, _idReloj);
if (tab == 5 && _vm.PersonaActual != null) CargarTab5(_vm.PersonaActual.Id, _idReloj);
```

- [ ] **Step 4: Build**

```powershell
dotnet build C:\soviet_3\RelojControl.App\RelojControl.App.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 5: Commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Windows/WndEnrolador.xaml RelojControl.App/Windows/WndEnrolador.xaml.cs
git commit -m "feat: WndEnrolador Tabs 4-5 comidas and relojes with DB loading"
```

---

## Task 18: WndPanelControl (Basic WPF — no new aesthetic yet)

Functional replacement for frmPanelDeControl. Basic WPF with existing functionality stubbed.

**Files:**
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndPanelControl.xaml`
- Modify: `C:\soviet_3\RelojControl.App\Windows\WndPanelControl.xaml.cs`

- [ ] **Step 1: Replace WndPanelControl.xaml with basic functional layout**

```xml
<!-- C:\soviet_3\RelojControl.App\Windows\WndPanelControl.xaml -->
<Window x:Class="RelojControl.Windows.WndPanelControl"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        WindowStyle="None" Topmost="True" WindowState="Maximized"
        Background="{StaticResource BgBrush}">
  <Grid>
    <!-- Header -->
    <DockPanel>
      <Border DockPanel.Dock="Top" Background="{StaticResource AccentBrush}" Padding="28,16">
        <Grid>
          <TextBlock Text="Panel de Control · rFlex"
                     FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="18"
                     Foreground="White" VerticalAlignment="Center"/>
          <Button Content="Cerrar" HorizontalAlignment="Right" Style="{StaticResource BtnSecondary}"
                  Background="Transparent" Foreground="White" BorderBrush="Transparent"
                  Click="BtnCerrar_Click"/>
        </Grid>
      </Border>
      <StackPanel DockPanel.Dock="Left" Width="200" Background="{StaticResource Surface2Brush}"
                  BorderBrush="{StaticResource LineBrush}">
        <Button Content="Enrolamiento" Padding="20,14" Click="BtnEnrolamiento_Click"
                HorizontalAlignment="Stretch" BorderThickness="0"
                Background="Transparent" FontFamily="{StaticResource FontPoppins}"
                Foreground="{StaticResource InkBrush}" HorizontalContentAlignment="Left"/>
      </StackPanel>
      <Grid Background="{StaticResource BgBrush}">
        <TextBlock Text="Panel de Control — funcionalidad completa en próxima versión"
                   Style="{StaticResource TextSubtitle}" HorizontalAlignment="Center"
                   VerticalAlignment="Center"/>
      </Grid>
    </DockPanel>
  </Grid>
</Window>
```

- [ ] **Step 2: Replace WndPanelControl.xaml.cs**

```csharp
// C:\soviet_3\RelojControl.App\Windows\WndPanelControl.xaml.cs
using System.Windows;

namespace RelojControl.Windows;

public partial class WndPanelControl : Window
{
    private int    _idReloj;
    public string CodigoEmpresa { get; set; } = "";

    public WndPanelControl(int idRol = 0, int idReloj = 0)
    {
        InitializeComponent();
        _idReloj = idReloj;
    }

    private void BtnCerrar_Click(object s, RoutedEventArgs e)
    {
        Close();
        if (Owner is WndInicio inicio) inicio.OnChildClosed();
    }

    private void BtnEnrolamiento_Click(object s, RoutedEventArgs e)
    {
        var enrolador = new WndEnrolador(_idReloj) { CodigoEmpresa = CodigoEmpresa, Owner = this };
        enrolador.Show();
        Hide();
    }
}
```

- [ ] **Step 3: Final build — all projects**

```powershell
cd C:\soviet_3
dotnet build
```

Expected: `Build succeeded. 0 Error(s).`

- [ ] **Step 4: Run all tests**

```powershell
dotnet test C:\soviet_3\RelojControl.Tests\RelojControl.Tests.csproj -v normal
```

Expected: `Passed: 16, Failed: 0`

- [ ] **Step 5: Final smoke test**

```powershell
cd C:\soviet_3\RelojControl.App && dotnet run
```

Verify:
1. WndInicio opens with gradient background, sync pill, clock, two cards
2. "Registrar marca" opens WndMarcaBajoTrafico (733×1061, TopMost, rail visible, keypad active)
3. Ctrl+E on WndInicio shows login card
4. Login with valid credentials opens WndEnrolador fullscreen
5. WndEnrolador shows white sidebar, header with user, search bar, tabs 1–5
6. Ctrl+Esc closes WndMarcaBajoTrafico

- [ ] **Step 6: Final commit**

```powershell
cd C:\soviet_3
git add RelojControl.App/Windows/WndPanelControl.xaml RelojControl.App/Windows/WndPanelControl.xaml.cs
git commit -m "feat: WndPanelControl basic functional WPF, complete soviet_v3 v1"
git tag v1.0.0-soviet-v3
```

---

## Out of Scope (Future Tasks)

- WndMarca1360x768 (horizontal 1360×768) with Front Soviet aesthetic
- WndMarca1080x1920 (vertical 1080×1920) with Front Soviet aesthetic
- Dark/Indigo theme variants
- Ticket de casino WPF
- WndPanelControl full feature parity with frmPanelDeControl
- rflex-diagnostico WPF port
