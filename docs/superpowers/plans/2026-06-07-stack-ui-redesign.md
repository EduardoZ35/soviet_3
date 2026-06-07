# Stack + UI Redesign — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Migrar el stack a CommunityToolkit.Mvvm + MySqlConnector, agregar MDIX ripple táctil en botones, ícono de huella Lucide en WndInicio, y animaciones de transición de panel.

**Architecture:** Tres fases independientes, cada una compilable y testeable antes de la siguiente. Phase 3 (P1-P8 del spec de rediseño) ya está completamente implementada — el plan cubre solo Phase 1 y Phase 2.

**Tech Stack:** .NET 8, WPF, CommunityToolkit.Mvvm 8.x, MySqlConnector 2.x, MaterialDesignThemes 5.x, Lucide icon paths (inline XAML)

---

## Estado actual (no repetir trabajo ya hecho)

Las siguientes mejoras ya están implementadas en `main`:
- P1–P8 del CODE-REVIEW spec: Viewbox en las 4 ventanas, rail banda superior, logo PNG, badge sync, ResultBadge con draw-on, íconos de sentido Path, tipografía 15/40/92, copy "Marca tu jornada"
- `ResultBadge.xaml.cs`: Kind DP, animación draw-on + pulse, colores de Tokens.xaml

---

## Phase 1 — Stack

### Task 1: CommunityToolkit.Mvvm — NuGet + migrar EnroladorViewModel

**Files:**
- Modify: `RelojControl.App/RelojControl.App.csproj`
- Modify: `RelojControl.App/ViewModels/EnroladorViewModel.cs`

- [ ] **Step 1: Agregar el NuGet**

Editar `RelojControl.App/RelojControl.App.csproj`, agregar dentro del primer `<ItemGroup>` de PackageReference:

```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.0" />
```

- [ ] **Step 2: Verificar que compila**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
```

Esperado: sin errores.

- [ ] **Step 3: Reescribir EnroladorViewModel.cs**

Reemplazar el contenido completo del archivo `RelojControl.App/ViewModels/EnroladorViewModel.cs`:

```csharp
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

    public bool IsWorkerSelected => !string.IsNullOrEmpty(_selectedRut);

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
```

- [ ] **Step 4: Compilar**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
```

Esperado: sin errores.

---

### Task 2: Migrar MarcaBajoTraficoViewModel + eliminar ViewModelBase

**Files:**
- Modify: `RelojControl.App/ViewModels/MarcaBajoTraficoViewModel.cs`
- Delete: `RelojControl.App/Infrastructure/ViewModelBase.cs`

- [ ] **Step 1: Reescribir MarcaBajoTraficoViewModel.cs**

Reemplazar el contenido completo de `RelojControl.App/ViewModels/MarcaBajoTraficoViewModel.cs`:

```csharp
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
```

- [ ] **Step 2: Eliminar ViewModelBase.cs**

```powershell
Remove-Item "RelojControl.App/Infrastructure/ViewModelBase.cs"
```

- [ ] **Step 3: Compilar + tests**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
dotnet test RelojControl.Tests/RelojControl.Tests.csproj
```

Esperado: 0 errores, 21 tests verdes.

- [ ] **Step 4: Commit**

```bash
git add RelojControl.App/RelojControl.App.csproj \
        RelojControl.App/ViewModels/EnroladorViewModel.cs \
        RelojControl.App/ViewModels/MarcaBajoTraficoViewModel.cs \
        RelojControl.App/Infrastructure/ViewModelBase.cs
git commit -m "feat: migrate ViewModels to CommunityToolkit.Mvvm ObservableObject"
```

---

### Task 3: Reemplazar MySql.Data con MySqlConnector

**Files:**
- Modify: `RelojControl.App/RelojControl.App.csproj`
- Modify: `proyectoNegocioRflex/proyectoNegocioRflex.csproj`
- Modify: `proyectoNegocioRflex/Utilidades/ConexionPrincipal.cs`
- Modify: `proyectoNegocioRflex/Utilidades/ConexionServidor.cs`
- Modify: `proyectoNegocioRflex/Utilidades/ConexionIntegracion.cs`
- Modify: `proyectoNegocioRflex/Utilidades/ConexionDinamica.cs`
- Modify: `proyectoNegocioRflex/Utilidades/Conexion.cs`
- Modify: `proyectoNegocioRflex/Modelo/ImagenHuella.cs`

- [ ] **Step 1: Actualizar RelojControl.App.csproj**

Reemplazar:
```xml
<PackageReference Include="MySql.Data" Version="8.3.0" />
```
Con:
```xml
<PackageReference Include="MySqlConnector" Version="2.4.0" />
```

- [ ] **Step 2: Actualizar proyectoNegocioRflex.csproj**

Reemplazar:
```xml
<PackageReference Include="MySql.Data" Version="8.0.30" />
```
Con:
```xml
<PackageReference Include="MySqlConnector" Version="2.4.0" />
```

- [ ] **Step 3: Actualizar los using en los 6 archivos**

En cada uno de estos archivos, reemplazar:
```csharp
using MySql.Data.MySqlClient;
```
Con:
```csharp
using MySqlConnector;
```

Archivos a editar:
- `proyectoNegocioRflex/Utilidades/ConexionPrincipal.cs`
- `proyectoNegocioRflex/Utilidades/ConexionServidor.cs`
- `proyectoNegocioRflex/Utilidades/ConexionIntegracion.cs`
- `proyectoNegocioRflex/Utilidades/ConexionDinamica.cs`
- `proyectoNegocioRflex/Utilidades/Conexion.cs`
- `proyectoNegocioRflex/Modelo/ImagenHuella.cs`

- [ ] **Step 4: Build completo de la solución**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
dotnet test RelojControl.Tests/RelojControl.Tests.csproj
```

Esperado: 0 errores, 21 tests verdes.

- [ ] **Step 5: Commit**

```bash
git add RelojControl.App/RelojControl.App.csproj \
        proyectoNegocioRflex/proyectoNegocioRflex.csproj \
        proyectoNegocioRflex/Utilidades/ConexionPrincipal.cs \
        proyectoNegocioRflex/Utilidades/ConexionServidor.cs \
        proyectoNegocioRflex/Utilidades/ConexionIntegracion.cs \
        proyectoNegocioRflex/Utilidades/ConexionDinamica.cs \
        proyectoNegocioRflex/Utilidades/Conexion.cs \
        proyectoNegocioRflex/Modelo/ImagenHuella.cs
git commit -m "feat: replace MySql.Data with MySqlConnector in all projects"
```

---

## Phase 2 — MDIX + Íconos + Animaciones

### Task 4: MaterialDesignThemes — NuGet + App.xaml

**Files:**
- Modify: `RelojControl.App/RelojControl.App.csproj`
- Modify: `RelojControl.App/App.xaml`

- [ ] **Step 1: Agregar el NuGet**

En `RelojControl.App/RelojControl.App.csproj`, agregar:

```xml
<PackageReference Include="MaterialDesignThemes" Version="5.1.0" />
```

- [ ] **Step 2: Actualizar App.xaml**

Reemplazar el contenido completo de `RelojControl.App/App.xaml`:

```xml
<Application x:Class="RelojControl.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes">
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <!-- MDIX base — primero, sus colores serán sobreescritos por Tokens -->
        <materialDesign:BundledTheme BaseTheme="Light"
                                     PrimaryColor="DeepPurple"
                                     SecondaryColor="Teal"/>
        <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesign2.Defaults.xaml"/>
        <!-- Tokens ÚLTIMO — gana sobre cualquier color/radio/tipografía de MDIX -->
        <ResourceDictionary Source="Themes/Tokens.xaml"/>
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

- [ ] **Step 3: Compilar**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
```

Esperado: sin errores. Si hay advertencias de tema (BundledTheme color names), son ignorables.

---

### Task 5: Ripple en BtnAccent y BtnSecondary

**Files:**
- Modify: `RelojControl.App/Themes/Tokens.xaml`

El ripple de MDIX funciona mediante el control `materialDesign:Ripple` envuelto dentro del `ControlTemplate`. Se agrega dentro del `Border` de cada botón, envolviendo al `ContentPresenter`.

- [ ] **Step 1: Agregar namespace materialDesign en Tokens.xaml**

En `RelojControl.App/Themes/Tokens.xaml`, cambiar la apertura del ResourceDictionary:

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes">
```

- [ ] **Step 2: Actualizar template de BtnAccent**

En `Tokens.xaml`, dentro de `<Style x:Key="BtnAccent" ...>`, reemplazar el `ControlTemplate`:

```xml
<Setter Property="Template">
  <Setter.Value>
    <ControlTemplate TargetType="Button">
      <Border Background="{TemplateBinding Background}"
              CornerRadius="{StaticResource RadiusButton}"
              Padding="{TemplateBinding Padding}">
        <materialDesign:Ripple Feedback="White" Opacity="0.3"
                               HorizontalContentAlignment="Center"
                               VerticalContentAlignment="Center">
          <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
        </materialDesign:Ripple>
      </Border>
    </ControlTemplate>
  </Setter.Value>
</Setter>
```

- [ ] **Step 3: Actualizar template de BtnSecondary**

En `Tokens.xaml`, dentro de `<Style x:Key="BtnSecondary" ...>`, reemplazar el `ControlTemplate`:

```xml
<Setter Property="Template">
  <Setter.Value>
    <ControlTemplate TargetType="Button">
      <Border Background="{TemplateBinding Background}"
              BorderBrush="{StaticResource LineBrush}"
              BorderThickness="1"
              CornerRadius="{StaticResource RadiusButton}"
              Padding="{TemplateBinding Padding}">
        <materialDesign:Ripple Feedback="{StaticResource AccentBrush}" Opacity="0.12"
                               HorizontalContentAlignment="Center"
                               VerticalContentAlignment="Center">
          <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
        </materialDesign:Ripple>
      </Border>
    </ControlTemplate>
  </Setter.Value>
</Setter>
```

- [ ] **Step 4: Compilar**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
```

Esperado: sin errores.

- [ ] **Step 5: Verificar ripple visualmente**

```powershell
Stop-Process -Name "RelojControl" -Force -ErrorAction SilentlyContinue
dotnet publish RelojControl.App/RelojControl.App.csproj -c Release -r win-x64 --no-self-contained -o publish/
./publish/RelojControl.exe
```

Hacer click en un botón → debe aparecer efecto de onda (ripple) al tocar. Si el botón es invisible o no renderiza, verificar que `Tokens.xaml` sea el último en `MergedDictionaries` de `App.xaml`.

---

### Task 6: Ícono de huella Lucide en WndInicio cardMarca

**Files:**
- Modify: `RelojControl.App/Windows/WndInicio.xaml`

La card "Registrar marca" en `WndInicio.xaml` actualmente solo tiene un TextBlock. Se agrega el ícono Lucide `fingerprint-pattern` (9 paths, stroke 1.5px) encima del texto.

- [ ] **Step 1: Localizar el StackPanel de cardMarca**

En `RelojControl.App/Windows/WndInicio.xaml`, la sección cardMarca (alrededor de línea 151) tiene:

```xml
<Border x:Name="cardMarca" ...>
  <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
    <TextBlock Text="Registrar marca" .../>
  </StackPanel>
</Border>
```

- [ ] **Step 2: Agregar ícono de huella antes del TextBlock**

Reemplazar el `StackPanel` dentro de `cardMarca`:

```xml
<StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
  <Viewbox Width="72" Height="72" HorizontalAlignment="Center" Margin="0,0,0,16">
    <Canvas Width="24" Height="24">
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M12 10a2 2 0 0 0-2 2c0 1.02-.1 2.51-.26 4"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M14 13.12c0 2.38 0 6.38-1 8.88"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M17.29 21.02c.12-.6.43-2.3.5-3.02"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M2 12a10 10 0 0 1 18-6"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M2 16h.01"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M21.8 16c.2-2 .131-5.354 0-6"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M5 19.5C5.5 18 6 15 6 12a6 6 0 0 1 .34-2"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M8.65 22c.21-.66.45-1.32.57-2"/>
      <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M9 6.8a6 6 0 0 1 9 5.2v2"/>
    </Canvas>
  </Viewbox>
  <TextBlock Text="Registrar marca"
             FontFamily="{StaticResource FontPoppins}" FontWeight="Bold" FontSize="28"
             Foreground="{StaticResource AccentDeepBrush}" HorizontalAlignment="Center"/>
</StackPanel>
```

- [ ] **Step 3: Compilar**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
```

Esperado: sin errores.

- [ ] **Step 4: Verificar visualmente**

Lanzar la app y confirmar que cardMarca muestra el ícono de huella (9 arcos concéntricos) sobre el texto "Registrar marca".

- [ ] **Step 5: Commit parcial**

```bash
git add RelojControl.App/RelojControl.App.csproj \
        RelojControl.App/App.xaml \
        RelojControl.App/Themes/Tokens.xaml \
        RelojControl.App/Windows/WndInicio.xaml
git commit -m "feat: MDIX ripple on buttons + Lucide fingerprint icon in cardMarca"
```

---

### Task 7: Animación fade-in de cards al aparecer en WndInicio

**Files:**
- Modify: `RelojControl.App/Windows/WndInicio.xaml.cs`

`cardLogin` y `cardMarca` son cards independientes (no se alternan entre sí — coexisten en columnas 0 y 2 del Grid). Ambas pasan de `Collapsed` a `Visible` en `OnLoaded`/`ComprobarReloj`. Se agrega fade+slide de 250ms cuando aparecen.

Context del code-behind actual:
- Línea 66: `cardLogin.Visibility = Visibility.Visible;` — en `OnLoaded`
- Línea 104: `cardMarca.Visibility = _permitidoUso ? Visibility.Visible : Visibility.Collapsed;` — en `ComprobarReloj`
- Línea 175: toggle de `cardLogin` con Ctrl+E (debug shortcut)

- [ ] **Step 1: Agregar helper FadeIn en WndInicio.xaml.cs**

Agregar el siguiente método privado al final de la clase `WndInicio`, antes de la llave de cierre `}` final:

```csharp
private static void FadeIn(FrameworkElement el)
{
    el.Opacity = 0;
    var t = new System.Windows.Media.TranslateTransform(0, 16);
    el.RenderTransform = t;

    var dur = new Duration(TimeSpan.FromMilliseconds(250));
    var ease = new System.Windows.Media.Animation.CubicEase
        { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut };

    el.BeginAnimation(OpacityProperty,
        new System.Windows.Media.Animation.DoubleAnimation(0, 1, dur) { EasingFunction = ease });
    t.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty,
        new System.Windows.Media.Animation.DoubleAnimation(16, 0, dur) { EasingFunction = ease });
}
```

- [ ] **Step 2: Aplicar FadeIn donde las cards se hacen visibles**

En `OnLoaded` (línea ~66), reemplazar:
```csharp
cardLogin.Visibility = Visibility.Visible;
```
Con:
```csharp
cardLogin.Visibility = Visibility.Visible;
FadeIn(cardLogin);
```

En `ComprobarReloj` (línea ~104), reemplazar:
```csharp
cardMarca.Visibility = _permitidoUso ? Visibility.Visible : Visibility.Collapsed;
```
Con:
```csharp
if (_permitidoUso)
{
    cardMarca.Visibility = Visibility.Visible;
    FadeIn(cardMarca);
}
else
{
    cardMarca.Visibility = Visibility.Collapsed;
}
```

- [ ] **Step 3: Compilar + tests**

```powershell
dotnet build RelojControl.App/RelojControl.App.csproj
dotnet test RelojControl.Tests/RelojControl.Tests.csproj
```

Esperado: 0 errores, 21 tests verdes.

- [ ] **Step 4: Verificar visualmente**

Lanzar la app → ambas cards deben aparecer con fade+slide suave al cargar la pantalla de inicio.

- [ ] **Step 5: Commit final Phase 2**

```bash
git add RelojControl.App/Windows/WndInicio.xaml \
        RelojControl.App/Windows/WndInicio.xaml.cs
git commit -m "feat: fade+slide card entrance animation in WndInicio"
```

---

## Notas de deploy

```powershell
Stop-Process -Name "RelojControl" -Force -ErrorAction SilentlyContinue
dotnet publish RelojControl.App/RelojControl.App.csproj -c Release -r win-x64 --no-self-contained -o publish/
```

**Kiosk mode (producción):** WndInicio, WndEnrolador y WndPanelControl tienen `WindowStyle="SingleBorderWindow"` — intencional para dev/testing. Para producción, cambiar a `WindowStyle="None" ResizeMode="NoResize" WindowState="Maximized" Topmost="True"` en las 3 ventanas.

---

## Phase 3 — Status

**Completada en sesiones anteriores.** Todos los ítems del CODE-REVIEW spec están implementados:
- ✅ P1: Viewbox en las 4 ventanas (fpHost fuera del Viewbox en WndMarcaBajoTrafico y WndEnrolador)
- ✅ P2: Rail banda superior 280px en WndMarcaBajoTrafico
- ✅ P3: Logo PNG en RailControl
- ✅ P4: Badge sync — texto blanco, punto #4ADE80, fondo white 0.14
- ✅ P5: ResultBadge con draw-on (StrokeDashOffset 60→0) + pulse ring
- ✅ P6: Íconos Lucide Path para entrada/salida en panelSentido
- ✅ P7: Tipografía eyebrow=15, título=40, reloj=92
- ✅ P8: Copy "Marca tu jornada"
