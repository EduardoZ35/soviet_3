# Design: Stack + UI Redesign — RelojControl (soviet_3)

**Fecha:** 2026-06-07  
**Proyecto:** `c:\soviet_3` — RelojControl.App (WPF .NET 8)  
**Autor:** sesión de brainstorming con Claude Code

---

## Contexto

RelojControl es un sistema de control de asistencia para tótems Windows (WPF .NET 8, DPFP SDK biométrico, MySQL 8). El stack está bloqueado en C# + WPF por el SDK propietario de DigitalPersona y 50.000 templates enrolados en formato DPFP. Este diseño mejora el stack existente y cierra la brecha visual con el prototipo de rediseño, sin tocar el SDK ni los templates.

---

## Decisiones de diseño

| Decisión | Elegida | Descartada |
|----------|---------|------------|
| Profundidad MDIX | Solo ripple en botones (B) | Iconos PackIcon (A), tema completo (C) |
| Ícono de huella | Lucide `fingerprint-pattern` stroke 1.5px | MDI filled, custom path, wifi-arcs actuales |
| Enfoque | 3 fases independientes | Big-bang único |
| SDK huella | DPFP sin cambios | SourceAFIS (bloqueado por 50k templates) |

---

## Fase 1 — Stack

**Objetivo:** infraestructura sin impacto visual. Build limpio, 21 tests pasan.

### 1a. CommunityToolkit.Mvvm

Reemplaza `ViewModelBase.cs` custom.

```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*"/>
```

- `ViewModelBase.cs` eliminado
- ViewModels pasan a heredar de `ObservableObject`
- Properties: `[ObservableProperty]` + source generator
- Commands: `[RelayCommand]` reemplaza `ICommand` manual

Archivos afectados: todos los `*ViewModel.cs` + `Infrastructure/ViewModelBase.cs` (eliminado).

### 1b. MySqlConnector

Reemplaza `MySql.Data 8.3.0` (Oracle).

```xml
<!-- Quitar: -->
<PackageReference Include="MySql.Data" Version="8.3.0"/>
<!-- Agregar: -->
<PackageReference Include="MySqlConnector" Version="2.*"/>
```

Drop-in: mismo namespace `MySqlConnector`, misma API pública. Async genuino, pool de conexiones correcto.

Archivos afectados: cualquier `using MySql.Data.MySqlClient` → `using MySqlConnector`.

**Commit:** `feat: migrate to CommunityToolkit.Mvvm and MySqlConnector`

---

## Fase 2 — MDIX + Íconos + Animaciones

**Objetivo:** ripple táctil, ícono de huella real, transiciones de panel.

### 2a. MaterialDesignInXaml — solo ripple

```xml
<PackageReference Include="MaterialDesignThemes" Version="5.*"/>
```

`App.xaml` — orden crítico (Tokens ÚLTIMO, gana siempre):

```xml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <materialDesign:BundledTheme BaseTheme="Light"
                                   PrimaryColor="DeepPurple"
                                   SecondaryColor="Teal"/>
      <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesign2.Defaults.xaml"/>
      <!-- Tokens ÚLTIMO — sobreescribe colores/radios/tipografía de MDIX -->
      <ResourceDictionary Source="Themes/Tokens.xaml"/>
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

Botones existentes mantienen estilo visual de `Tokens.xaml` y ganan ripple via:
```xml
materialDesign:RippleAssist.IsDisabled="False"
```

### 2b. Ícono de huella — Lucide fingerprint-pattern

Reemplaza los arcos tipo wifi en `WndInicio` cardMarca. 9 paths Lucide, stroke 1.5px, color `AccentBrush`.

```xml
<Canvas Width="24" Height="24">
  <!-- 9 paths de Lucide fingerprint-pattern -->
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M12 10a2 2 0 0 0-2 2c0 1.02-.1 2.51-.26 4"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M14 13.12c0 2.38 0 6.38-1 8.88"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M17.29 21.02c.12-.6.43-2.3.5-3.02"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M2 12a10 10 0 0 1 18-6"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M2 16h.01"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M21.8 16c.2-2 .131-5.354 0-6"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M5 19.5C5.5 18 6 15 6 12a6 6 0 0 1 .34-2"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M8.65 22c.21-.66.45-1.32.57-2"/>
  <Path Stroke="{StaticResource AccentBrush}" StrokeThickness="1.5"
        StrokeStartLineCap="Round" StrokeEndLineCap="Round"
        Data="M9 6.8a6 6 0 0 1 9 5.2v2"/>
</Canvas>
```

Envolver en `Viewbox` para escalar al tamaño que necesite el contenedor.

### 2c. Animaciones WPF built-in

**Transición entre cards (WndInicio):**
```xml
<DoubleAnimation Storyboard.TargetProperty="Opacity"
                 From="0" To="1" Duration="0:0:0.25"/>
<DoubleAnimation Storyboard.TargetProperty="(TranslateTransform.Y)"
                 From="20" To="0" Duration="0:0:0.25">
  <DoubleAnimation.EasingFunction>
    <CubicEase EasingMode="EaseOut"/>
  </DoubleAnimation.EasingFunction>
</DoubleAnimation>
```

**Draw-on del check (ResultBadge éxito):**
```xml
<Path x:Name="checkPath" StrokeDashArray="60" StrokeDashOffset="60"
      Data="M16 33 l11 11 l21 -23"/>
<DoubleAnimation Storyboard.TargetName="checkPath"
                 Storyboard.TargetProperty="StrokeDashOffset"
                 From="60" To="0" Duration="0:0:0.4"/>
```

Archivos afectados: `App.xaml`, `RelojControl.App.csproj`, `WndInicio.xaml`, `WndMarcaBajoTrafico.xaml`, `Controls/ResultBadge.xaml`.

**Commit:** `feat: MDIX ripple, Lucide fingerprint icon, panel animations`

---

## Fase 3 — CODE-REVIEW P1-P8

**Objetivo:** cierre de brecha visual con prototipo. Todos los ítems del spec `CODE-REVIEW-wpf-rediseño.md`.

### P1 — Viewbox scaling ⚠️ CRÍTICO

Cada ventana envuelta en `Viewbox Stretch="Uniform"`. El `fpHost`/`ecHost` (`WindowsFormsHost` DPFP) debe quedar **fuera del Viewbox**, como hermano directo del Grid raíz del Window.

```xml
<Grid>  <!-- raíz directa del Window -->
  <Viewbox Stretch="Uniform">
    <Grid Width="1366" Height="768">  <!-- lienzo fijo horizontal -->
      <!-- todo el contenido visual -->
    </Grid>
  </Viewbox>
  <!-- DPFP host FUERA del Viewbox -->
  <WindowsFormsHost x:Name="fpHost" Width="300" Height="200"
                    HorizontalAlignment="Left" VerticalAlignment="Top"
                    Margin="-400,-300,0,0"/>
</Grid>
```

Lienzos:
- `WndMarcaBajoTrafico`: 768×1366 (vertical)
- `WndInicio`, `WndEnrolador`, `WndPanelControl`: 1366×768 (horizontal)

Window attrs: `WindowStyle="None" ResizeMode="NoResize" WindowState="Maximized" Topmost="True"`

### P2 — Rail orientación

`WndMarcaBajoTrafico` (vertical): rail como banda superior (Row Height=280) en vez de columna lateral.

### P3 — Logo real en rail

`logo-rflex-white.png` ya existe en `RelojControl.App/Assets/` y ya está declarada como `<Resource Include="Assets\*.png"/>` en el `.csproj`. Solo requiere cambio XAML en `RailControl.xaml`.

### P4 — Badge sync legible

`RailControl.xaml`:
- `syncDot`: `Fill="#4ADE80"`
- `lblSync`: `Foreground="White"`
- Badge background: `SolidColorBrush Color="White" Opacity="0.14"`

### P5 — SuccessBadge / ErrorBadge

`Controls/ResultBadge.xaml` — reemplaza reutilización de `FingerprintRing`:
- Éxito: `Ellipse` `OkSoftBrush` + `Path` check Lucide + animación draw-on
- Error: `Ellipse` `CoralSoftBrush` + `Path` alerta Lucide

### P6 — Íconos sentido

`WndMarcaBajoTrafico.xaml` `panelSentido`: `Path` Lucide entrada/salida reemplaza `TextBlock Text="↗"/"↙"`.

### P7 — Tipografía

| Elemento | Antes | Después |
|----------|-------|---------|
| Eyebrow | 11 | 15 |
| Título | 34 | 40 |
| Reloj | 80 | 92 |
| Subtítulo sentido | 13 | 14.5 |

### P8 — Copy

Panel sentido: "Marca tu jornada". Mensajes de estado/resultado (intentos huella, "Marca ya registrada", etc.) según tabla maestra del handoff WPF §13 si está disponible; si no, mantener textos actuales y posponer P8 para cuando se entregue el handoff.

Archivos afectados: `WndInicio.xaml`, `WndMarcaBajoTrafico.xaml`, `WndEnrolador.xaml`, `WndPanelControl.xaml`, `Controls/RailControl.xaml`, `Controls/ResultBadge.xaml`.

**Commit:** `feat: viewbox scaling, rail layout, typography, icons — P1-P8`

---

## Checklist de verificación (por fase)

- [ ] `dotnet build` sin errores ni warnings
- [ ] `dotnet test` — 21 tests pasan
- [ ] App inicia sin excepciones
- [ ] Fase 1: ningún cambio visual observable
- [ ] Fase 2: ripple en botones, huella Lucide visible, animaciones en transiciones
- [ ] Fase 3: app escala correctamente en resolución distinta a la de diseño; DPFP sigue funcionando (fpHost fuera del Viewbox)

---

## Restricciones inamovibles

- DPFP SDK: no reemplazar — 50.000 templates en formato propietario
- `Tokens.xaml`: siempre el último en `MergedDictionaries` — nunca hardcodear hex en XAML
- `fpHost`/`ecHost`: siempre fuera del Viewbox — destruye el HWND si se mete adentro
