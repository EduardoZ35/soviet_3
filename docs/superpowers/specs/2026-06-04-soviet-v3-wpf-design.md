# Soviet v3 — Diseño WPF con estética Front Soviet

**Fecha:** 2026-06-04  
**Estado:** Aprobado

---

## Contexto

Soviet v2 (`C:\soviet_3\soviet_v2\`) es una app WinForms .NET 8 que gestiona marcaje de asistencia en kioscos físicos. Usa MaterialSkin + MetroCompat shims para UI. El proyecto `Front Soviet` (`C:\soviet_3\Front Soviet\`) contiene un rediseño visual completo para WPF con tokens de diseño, prototipos HTML/JSX y un handoff técnico WPF.

El objetivo es crear **soviet_v3**: una nueva solución WPF en `C:\soviet_3\` que reutiliza la lógica de negocio de soviet_v2 y aplica la estética de Front Soviet. Soviet_v2 y Front Soviet se mantienen intactos como referencia.

---

## Decisiones de diseño

| Decisión | Elección | Razón |
|---|---|---|
| Tema visual | **Light/Claro** | Máxima legibilidad en kioscos con luz ambiente |
| Estructura | **Nueva solución + project reference** | Aislamiento total, sin tocar soviet_v2 |
| Ventanas | **WPF nativo** (sin ElementHost como host principal) | WPF ya habilitado en soviet_v2, ElementHost frágil |
| Resolución ventanas | **Idéntica a soviet_v2** | Comportamiento probado en producción |
| Alcance v1 | **App completa** con 2 pantallas prioritarias con nueva estética | Enfoque incremental |
| DigitalPersona | **WindowsFormsHost** para control DPFP | SDK solo expone controles WinForms |

---

## Estructura de solución

```
C:\soviet_3\
├── soviet_v2\                          ← referencia, no se toca
├── Front Soviet\                       ← referencia, no se toca
├── soviet_v3.sln                       ← nueva solución
│
└── RelojControl.App\                   ← WPF app (net8.0-windows)
    ├── App.xaml / App.xaml.cs          ← arranque, tray icon sincronizador
    ├── Themes\
    │   └── Tokens.xaml                 ← ResourceDictionary tokens Front Soviet
    ├── Windows\
    │   ├── WndInicio.xaml              ← orquestador invisible (≈ frmInicio)
    │   ├── WndMarcaBajoTrafico.xaml    ← ★ PRIORIDAD 1 — nueva estética
    │   ├── WndEnrolador.xaml           ← ★ PRIORIDAD 2 — nueva estética
    │   └── WndPanelControl.xaml        ← WPF básico funcional (sin nueva estética)
    ├── ViewModels\
    │   ├── MarcaBajoTraficoViewModel.cs
    │   └── EnroladorViewModel.cs
    ├── Controls\                        ← UserControls reutilizables
    │   ├── RailControl.xaml            ← barra lateral con gradiente + reloj
    │   ├── NumericKeypad.xaml          ← teclado numérico 3×4
    │   ├── FingerprintRing.xaml        ← anillo huella con animaciones
    │   └── NotifOverlay.xaml           ← modal de notificaciones
    └── lib\                             ← DLLs DigitalPersona (copiadas de soviet_v2)
        ├── DPFPGuiNET.dll
        ├── DPFPShrNET.dll
        ├── DPFPVerNET.dll
        ├── AxInterop.DPFPCtlXLib.dll
        └── Interop.DPFPCtlXLib.dll

← Project reference directa:
   ..\soviet_v2\proyectoReloj-master\proyectoNegocioRflex\proyectoNegocioRflex.csproj
```

---

## Tokens de diseño — Tokens.xaml

Todos los valores de Front Soviet como recursos WPF. Ninguna pantalla hardcodea colores.

### Colores

| Token | Valor | Uso |
|---|---|---|
| `AccentBrush` | `#616EB3` | Botones CTA, bordes activos, acentos |
| `AccentDeepBrush` | `#3F4690` | Sombra 3D botones, overlays |
| `AccentSoftBrush` | `#ECEDFB` | Fondos hover, fondo anillo huella |
| `TealBrush` | `#34B3AB` | Entrada/inicio jornada |
| `TealSoftBrush` | `#D6F3F1` | Fondo badge entrada |
| `CoralBrush` | `#F76D6D` | Salida/error |
| `CoralSoftBrush` | `#FFE3E3` | Fondo badge salida/error |
| `AmberBrush` | `#E8910C` | Comida/avisos |
| `AmberSoftBrush` | `#FDECCD` | Fondo badge comida |
| `OkBrush` | `#16B364` | Éxito |
| `BgBrush` | `#F3F3FA` | Fondo ventana principal |
| `SurfaceBrush` | `#FFFFFF` | Tarjetas, paneles |
| `Surface2Brush` | `#F7F7FC` | Teclas keypad, inputs secundarios |
| `InkBrush` | `#1B1B27` | Texto principal |
| `Ink2Brush` | `#5A5A72` | Subtítulos, labels |
| `Ink3Brush` | `#9A9AB0` | Hints, placeholders |
| `LineBrush` | `#E7E7F1` | Bordes, separadores |

### Rail (barra lateral)

```xml
<LinearGradientBrush x:Key="RailGradientBrush" StartPoint="0,0" EndPoint="0.3,1">
    <GradientStop Color="#5A63A8" Offset="0"/>
    <GradientStop Color="#454B8F" Offset="0.55"/>
    <GradientStop Color="#383C6F" Offset="1"/>
</LinearGradientBrush>
```

### Tipografía

| Token | Familia | Peso | Uso |
|---|---|---|---|
| `FontUI` | Poppins | 400/500/600/700 | Toda la UI general |
| `FontNum` | Manrope | 600/700/800 | Reloj, RUT, PIN, números |

Fonts instaladas como recursos embedded en `RelojControl.App/Fonts/`.

### Radios y sombras

| Token | Valor |
|---|---|
| `RadiusCard` | 18 |
| `RadiusInput` | 10 |
| `RadiusButton` | 10 |
| `RadiusKeypad` | 16 |
| `ShadowCard` | `DropShadowEffect BlurRadius=28 Direction=270 ShadowDepth=8 Opacity=.15` |
| `ShadowKey` | `DropShadowEffect BlurRadius=0 Direction=270 ShadowDepth=3 Color=#3F4690` (tecla CTA) |

---

## WndInicio — Lobby (con UI propia)

Equivalente a `frmInicio`. **Tiene UI propia** — pantalla tipo "lobby" con top header y dos cards.

**Tamaño:** `WindowStyle=None`, `Topmost=True`, fullscreen o tamaño igual a resolución actual  
**Fondo:** `RailGradientBrush` (indigo) cubre toda la ventana

### Layout

```
┌─────────────────────────────────────────────────┐
│ [● Sincronizado]  Registro · Ubicación   09:19  │  ← top bar
│                                    Miérc 3 Jun  │
├──────────────────────┬──────────────────────────┤
│  Iniciar sesión      │   📶  Realizar marcas    │  ← dos cards blancas
│  [Usuario________]   │                          │
│  [Contraseña_____]   │     Registrar marca      │
│  [Iniciar sesión ]   │                          │
├──────────────────────┴──────────────────────────┤
│                  rFlex.io · · · ·               │  ← footer
└─────────────────────────────────────────────────┘
```

### Lógica

1. Al cargar: inicializa tray icon sincronizador + llama `comprobarReloj()`
2. Si `IniciarDesdePantallaDeMarca == true` → salta directo a pantalla de marca (sin mostrar lobby)
3. Si `SoloEnrolar == 1` → muestra solo card de login (oculta card de marca)
4. Card "Registrar marca" → lee `idresolucionMarca`:
   - `3` (default) → `WndMarcaBajoTrafico`
   - `2` → `WndMarca1360x768` (fase posterior)
   - `1` → `WndMarca1080x1920` (fase posterior)
5. Login exitoso → `WndEnrolador` o `WndPanelControl` según `IdTipoRolUsuario`
6. `Ctrl+E` → activa/desactiva visibilidad card login (igual que soviet_v2)

---

## WndMarcaBajoTrafico ★ Prioridad 1

**Tamaño:** `Width=733 Height=1061`  
**Comportamiento:** `WindowStyle=None`, `Topmost=True`, `WindowStartupLocation=CenterScreen`

### Layout

Dos columnas en `Grid` (validado contra captura `shot-bajo.png` / prototipo JSX):
- **Col 0 (380px):** `RailControl` — gradiente + logo + reloj + info empresa + sync status
- **Col 1 (flex):** área principal con los estados

### Estados (enum `MarcaStep`)

```csharp
enum MarcaStep
{
    IngresoRut,         // keypad activo, display RUT con caret
    VerificandoHuella,  // anillo animado, nombre trabajador, última marca
    SeleccionSentido,   // botones Entrada/Salida/Comida
    Exito,              // badge verde animado, auto-reset 4s
    Error,              // badge rojo, mensaje, auto-reset 3s
}
```

### Componentes

**RailControl:**
- Fondo: `RailGradientBrush`
- Logo RFLEX (texto o imagen)
- Reloj: Manrope 800, `DispatcherTimer` 1s, tabular nums
- Fecha: Poppins 600
- Info empresa (nombre, sucursal) al pie
- Círculo decorativo pseudo (Ellipse con opacidad radial)

**NumericKeypad:**
- Grid 3×4 (1–9, ⌫, 0, IR→)
- Teclas normales: `Surface2Brush`, `LineBrush` borde, `RadiusKeypad`
- Tecla IR→: `AccentBrush`, `ShadowKey`, offset 3px en `:active`
- Tecla ⌫: color `CoralBrush`
- Display RUT: border `LineBrush`, Manrope 700 48px, caret animado

**FingerprintRing:**
- Ellipse 192×192, fondo `AccentSoftBrush`
- Ícono huella SVG Path centrado
- `RotateTransform` animado en estado `VerificandoHuella`
- `StrokeThickness` arc animado (DoubleAnimation en `StrokeDashOffset`)
- Estado error: fondo `CoralSoftBrush`

**TarjetaTrabajador:**
- Avatar 62×62, `RadiusButton`, gradiente `AccentBrush→AccentDeepBrush`
- Iniciales del nombre
- Nombre Poppins 700 22px
- Subtítulo RUT Poppins 400 15px `Ink2Brush`
- Última marca: `Ink3Brush` + valor bold

**BotonesSentido:**
- Border-left 5px (`TealBrush` para Entrada, `CoralBrush` para Salida, `AmberBrush` para Comida)
- Ícono 56×56, `RadiusButton`, soft background
- Hover: `TranslateTransform Y=-3`

**NotifOverlay:**
- Overlay `rgba(15,15,35,.5)` + blur (WPF: semi-transparent panel sobre viewport)
- Top bar 6px con color según tipo
- CountdownProgressBar: 6s auto-cierre
- Tipos: info, warning, error, success

### ViewModel

```csharp
class MarcaBajoTraficoViewModel : INotifyPropertyChanged
{
    MarcaStep Step          // estado actual
    string RutDisplay       // "12.345.67█" (con caret)
    string NombreTrabajador
    string UltimaMarca
    bool   IsScanning       // activa animación anillo
    string MensajeError
    
    ICommand KeyPressCommand
    ICommand SeleccionarSentidoCommand
    ICommand ResetCommand    // vuelve a IngresoRut
}
```

---

## WndEnrolador ★ Prioridad 2

**Comportamiento:** `WindowState=Maximized`, `WindowStyle=None`, `Topmost=True`

> Validado contra captura `shot-enrol.png`. Layout es **admin panel desktop**, NO wizard modal.

### Layout

```
┌────────┬────────────────────────────────────────────────┐
│ rFlex  │ Panel de Control · Admin Usuarios   [EZ] Salir │  ← header
│        ├────────────────────────────────────────────────┤
│ [👤]   │ [15847233-3____] [🔍 Buscar] [+ Nuevo] [Limpiar│  ← buscador
│Enrolar │ ┌────────────────────────────────────── Guardado┤
│        │ │ [VS] VALENTINA SOTO HERRERA          15.847  │  ← tarjeta
│        │ │      Enfermera Clínica · Urgencia Adulto      │
│        │ └───────────────────────────────────────────────┤
│        │ 1.Datos  2.Asistencia  3.Huellas  4.Comidas  5. │  ← tabs
│        ├────────────────────────────────────────────────┤
│        │  [campos del tab activo — scrollable]           │  ← form
│        │                                  [Siguiente →]  │
└────────┴────────────────────────────────────────────────┘
```

### Sidebar izquierda

- Fondo `Surface2Brush` (`#F7F7FC`), borde derecho `LineBrush` — **NO gradiente**
- Logo rFlex.io (texto con puntos)
- Botón "Enrolar" como nav item con ícono persona

### Header

- Título "Panel de Control · Administración de Usuarios"
- Subtítulo estación/reloj (`Ink3Brush`)
- Avatar usuario logueado (iniciales en círculo `AccentBrush`) + nombre
- Botón "Salir" con ícono

### Buscador

- TextBox RUT con botón "Buscar" (`AccentBrush`)
- Botón "+ Funcionario nuevo" (`Surface2Brush` con borde)
- Botón "Limpiar"

### Tarjeta trabajador activo

- Avatar 40×40, iniciales, gradiente `AccentBrush→AccentDeepBrush`, `RadiusButton`
- Nombre (Poppins 700), cargo (Ink2), RUT (Ink3)
- Badge "● Guardado" (`TealSoftBrush`/`TealBrush`) o "Sin guardar" (`AmberSoftBrush`)

### Tabs (5 pestañas)

Tab activo: `AccentBrush` subrayado, texto `AccentBrush` bold  
Tabs inactivos: `Ink3Brush`

**Tab 1 — Datos generales:**  
Secciones IDENTIFICACIÓN / CONTACTO / ORGANIZACIÓN. Campos: Nombre, Segundo nombre, Alias, Apellido paterno, Apellido materno, Fecha nacimiento, Teléfono, Correo, Empresa.

**Tab 2 — Asistencia:**  
Checkboxes: permite comida, permite jornada, habilitado, tipo rol usuario.

**Tab 3 — Huellas:**  
- `WindowsFormsHost` con control DPFP activo
- Indicadores de progreso (0–4 capturas)
- Anillo `FingerprintRing` animado durante captura

**Tab 4 — Comidas:**  
Lista `SucursalTipoComida` con checkboxes + configuración ración.

**Tab 5 — Relojes:**  
Lista relojes disponibles en sucursal, checkbox por reloj.

### Navegación tabs

- Botón "Siguiente →" (`AccentBrush`, `ShadowKey`) avanza al siguiente tab
- Al llegar al tab 5: botón "Guardar" → persiste via `proyectoNegocioRflex`
- Cambios no guardados → badge "Sin guardar" en tarjeta trabajador

### ViewModel

```csharp
class EnroladorViewModel : INotifyPropertyChanged
{
    int            WizardStep          // 1–5
    ObservableCollection<PersonaVM> Personas
    PersonaVM      PersonaActual
    List<ImagenHuella> HuellasCapturadas
    bool           IsCapturandoHuella
    
    ICommand NuevoCommand
    ICommand EditarCommand
    ICommand SiguienteCommand
    ICommand VolverCommand
    ICommand GuardarCommand
    ICommand CancelarCommand
    ICommand CapturarHuellaCommand
}
```

---

## Comportamiento de ventanas (igual a soviet_v2)

| Ventana | `WindowStyle` | `Topmost` | `StartupLocation` | `WindowState` |
|---|---|---|---|---|
| `WndMarcaBajoTrafico` | `None` | `True` | `CenterScreen` | Normal (733×1061) |
| `WndEnrolador` | `None` | `True` | `CenterScreen` | `Maximized` |
| `WndPanelControl` | `None` | `True` | `CenterScreen` | `Maximized` |

---

## Tray icon sincronizador

Misma lógica que `frmInicio.cs` `InitSyncMonitor()`:
- Polling cada 30s a `C:\rflexapps\sync_status.txt`
- Ícono verde/amarillo/rojo según estado
- Menú contextual "Reiniciar sincronizador"
- Implementado en `App.xaml.cs` con `System.Windows.Forms.NotifyIcon`

---

## Fonts embebidas

Poppins y Manrope se descargan de Google Fonts y se incluyen como recursos en `RelojControl.App/Fonts/`. Registradas en `App.xaml`:

```xml
<FontFamily x:Key="FontPoppins">pack://application:,,,/RelojControl.App;component/Fonts/#Poppins</FontFamily>
<FontFamily x:Key="FontManrope">pack://application:,,,/RelojControl.App;component/Fonts/#Manrope</FontFamily>
```

---

## Dependencias del proyecto

```xml
<!-- RelojControl.App.csproj -->
<TargetFramework>net8.0-windows</TargetFramework>
<UseWPF>true</UseWPF>
<UseWindowsForms>true</UseWindowsForms>  <!-- para NotifyIcon + WindowsFormsHost DPFP -->

<ProjectReference Include="..\..\soviet_v2\proyectoReloj-master\proyectoNegocioRflex\proyectoNegocioRflex.csproj" />

<PackageReference Include="MySql.Data" Version="8.3.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

DLLs DigitalPersona: copiadas a `lib/`, referenciadas con `<Reference>` + `<CopyToOutputDirectory>Always`.

---

## Fuera de alcance v1

- `WndMarca1360x768` (horizontal) con nueva estética
- `WndMarca1080x1920` (vertical grande) con nueva estética  
- Dashboard `syncMonitor` web
- App `rflex-diagnostico`
- Temas indigo/dark (solo light en v1)
- Ticket de casino
