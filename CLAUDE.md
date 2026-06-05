# RelojControl (soviet_3) — Contexto del proyecto

## Qué es esto

Reescritura en **WPF + .NET 8** del sistema de control de asistencia rFlex (anterior: WinForms .NET Framework, repositorio `soviet_v2`). Los "relojes" son PCs con Windows que los funcionarios usan para marcar asistencia y enrolarse con huella digital.

**Stack:** C# .NET 8, WPF (`net8.0-windows`), `UseWPF=true`, `UseWindowsForms=true`, MySQL 8, DPFP SDK (DigitalPersona U.are.U 4500), MVVM con ViewModelBase.

---

## Estructura del proyecto

```
soviet_3/
├── RelojControl.App/          # App WPF principal
│   ├── Windows/               # Ventanas (WndInicio, WndMarcaBajoTrafico, WndEnrolador, WndPanelControl)
│   ├── ViewModels/            # MVVM ViewModels
│   ├── Controls/              # Controles custom (FingerprintRing, NumericKeypad, RailControl, etc.)
│   ├── Infrastructure/        # ViewModelBase, ConfiguracionesConstantes
│   └── lib/                   # DLLs DPFP (ver sección crítica abajo)
├── proyectoNegocioRflex/      # Business logic (netstandard2.0, compartida con soviet_v2)
├── RelojControl.Tests/        # xUnit tests (net8.0-windows)
└── publish/                   # Output de dotnet publish (gitignoreado)
```

**Gitignore crítico:** `soviet_v2/` y `Front Soviet/` están en `.gitignore`. Nunca commitear contenido de esos directorios.

---

## Tipos de reloj

### `tipoReloj_idtipoReloj` (tabla `tipoReloj`, FK en `reloj`)

| ID | Nombre | Uso |
|----|--------|-----|
| 1 | Enrolador | Solo registro de huella digital |
| 2 | Asistencia Bajo Tráfico | Marcas jornada, <100 personas |
| 3 | Asistencia Alto Tráfico | Marcas jornada, >100 personas |
| 4 | Centro Médico Alto Tráfico | UTI/UCI/urgencias |
| 5 | Centro Médico Bajo Tráfico | Clínicas menores |
| 6 | Casino | Registro de raciones |
| 7 | Pruebas | Dev/QA |

⚠️ **La query `traerDatosRelojPorNombre` en v3 NO incluye `tipoReloj_idtipoReloj`** — debe agregarse al SELECT si se necesita en lógica de negocio.

### `soloEnrolador` (bool, columna [18] de la query v3)

Campo booleano de nivel dispositivo — **NO es 0/1/2**:
- `0` / `false` = dispositivo normal (va a pantalla de marca si `iniciarDesdeMarca=1`)
- `1` / `true` = solo enrolador (muestra cardLogin, nunca va a pantalla de marca)

### `mostrar_login_inicio` (bool, columna [11] de la query v3)

Fuerza cardLogin visible al iniciar, independiente de `soloEnrolador`. Usar este campo para dispositivos tipo 2 (Asistencia Bajo Tráfico) que también necesitan acceso a enrolamiento.

### Lógica actual en `WndInicio.OnLoaded`

```csharp
if (_soloEnrolar >= 1 || _mostrarLoginInicio)
    cardLogin.Visibility = Visibility.Visible;
if (_soloEnrolar == 1) return;          // solo enrolador: no ir a marca
if (_iniciarDesdeMarca && _permitidoUso) AbrirMarca();
```

Para un dispositivo "Asistencia Bajo Tráfico" que también debe mostrar admin: configurar `mostrar_login_inicio = 1` en BD.

---

## DPFP SDK — DLLs requeridas (.NET 8) ⚠️ CRÍTICO

### Todas las DLLs deben estar en `lib/` Y en el `.csproj`

En .NET 8 **no hay GAC**. Si una DLL no está referenciada en el `.csproj` con `<Private>true</Private>`, no entra a `deps.json` y .NET 8 no la resuelve en runtime aunque exista físicamente en disco.

**DLLs actualmente referenciadas en `RelojControl.App.csproj`:**

```xml
<Reference Include="DPFPShrNET">
  <HintPath>lib\DPFPShrNET.dll</HintPath><Private>true</Private>
</Reference>
<Reference Include="DPFPGuiNET">
  <HintPath>lib\DPFPGuiNET.dll</HintPath><Private>true</Private>
</Reference>
<Reference Include="DPFPVerNET">
  <HintPath>lib\DPFPVerNET.dll</HintPath><Private>true</Private>
</Reference>
<Reference Include="AxInterop.DPFPCtlXLib">
  <HintPath>lib\AxInterop.DPFPCtlXLib.dll</HintPath><Private>true</Private>
</Reference>
<Reference Include="Interop.DPFPCtlXLib">
  <HintPath>lib\Interop.DPFPCtlXLib.dll</HintPath><Private>true</Private>
</Reference>
<Reference Include="DPFPCtlXTypeLibNET">
  <HintPath>lib\DPFPCtlXTypeLibNET.dll</HintPath><Private>true</Private>
</Reference>
<Reference Include="DPFPCtlXWrapperNET">
  <HintPath>lib\DPFPCtlXWrapperNET.dll</HintPath><Private>true</Private>
</Reference>
<Reference Include="DPFPShrXTypeLibNET">
  <HintPath>lib\DPFPShrXTypeLibNET.dll</HintPath><Private>true</Private>
</Reference>
```

### Síntoma de DLL faltante

- Lector enciende (LED activo), captura imagen visible en control DPFP
- `OnComplete` / `OnEnroll` **nunca dispara**
- App queda colgada en "Coloque su dedo" indefinidamente
- Sin error visible en UI

### Causa raíz

El SDK lanza `FileNotFoundException` internamente al intentar cargar la DLL por strong-name. Esa excepción es tragada por el runtime del SDK → `OnComplete` nunca se invoca.

### Cómo diagnosticar

Instrumentar `AppDomain.CurrentDomain.FirstChanceException` al inicio de la app:

```csharp
AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
    System.Diagnostics.Debug.WriteLine($"FIRST-CHANCE: {e.Exception.GetType().Name}: {e.Exception.Message}");
```

Buscar `FileNotFoundException` con `DPFPShr*` o `DPFPCtlX*` en el mensaje.

### Historia del diagnóstico (2026-06-04, soviet_v2 → soviet_v3)

**Síntoma:** Lector U.are.U 4500 enciende, captura imagen, pero `OnComplete` nunca dispara → marcaje y enrolamiento colgados.

**Hipótesis descartadas (NO eran el problema):**
- ❌ Arquitectura x64/x86
- ❌ Timing de HWND / `Active=true` antes de crear HWND
- ❌ `ReaderSerialNumber` GUID nulo
- ❌ Tamaño del control (1×1 px vs 300×200 px)
- ❌ `Visibility="Hidden"` en WindowsFormsHost (afecta `IsWindowVisible` pero no fue la causa raíz)
- ❌ Multi-instancia robando el reader
- ❌ `UseWPF=true` en csproj
- ❌ DLLs stale en `C:\rflexapps\`

**Fix real:** `DPFPShrXTypeLibNET.dll` no estaba referenciada en el `.csproj`.
- Copiada a `lib\DPFPShrXTypeLibNET.dll` desde `C:\rflexapps\`
- Agregada como `<Reference>` con `<Private>true</Private>`
- Resultado: entra a `deps.json` → .NET 8 la resuelve → `OnComplete` dispara ✅

**Lección:** Ante cualquier callback DPFP que no dispara, instrumentar `FirstChanceException` primero. Habría tomado 20 minutos en lugar de una tarde.

---

## WindowsFormsHost y DPFP — patrón correcto

### WndMarcaBajoTrafico (VerificationControl)

`fpHost` vive en el root Grid con márgenes negativos (`Margin="-400,-300,0,0"`):
- Control fuera de pantalla pero Win32-visible (`IsWindowVisible=true`)
- Creado una vez en `OnLoaded`, vive durante toda la sesión
- `_vc.Active = true` se establece DESPUÉS de `fpHost.Child = _vc` (HWND debe existir primero)

### WndEnrolador (EnrollmentControl)

`ecHost` vive dentro del tab de huellas. Lifecycle por tab:
- Al mostrar tab huellas: `IniciarEnrollment()` → crea EC, asigna a `ecHost.Child`, activa
- Al salir del tab: `PararEnrollment()` → `ecHost.Child = null; _ec = null` ANTES de colapsar el tab
- **Nunca** hacer `Visibility=Collapsed` en un WindowsFormsHost con child activo — destruye el HWND con el control adentro

---

## Encriptación de RUT

Todos los RUTs se almacenan encriptados. `Encriptacion.Encriptar(rut.ToUpper())` antes de cualquier consulta SQL. El RUT debe estar en mayúsculas antes de encriptar (el dígito verificador `K` debe ser `K`, no `k`).

---

## Publicación

```powershell
# Matar instancias previas primero
Stop-Process -Name "RelojControl" -Force -ErrorAction SilentlyContinue
# Publicar framework-dependent (requiere .NET 8 runtime en máquina destino)
dotnet publish RelojControl.App/RelojControl.App.csproj -c Release -r win-x64 --no-self-contained -o publish/
```

Output: ~15MB en `publish/`. Las máquinas de producción ya tienen .NET 8 runtime instalado.

Config de conexión: `C:\rflexapps\config\conexionLocal.txt`

---

## Tests

```bash
dotnet test RelojControl.Tests/RelojControl.Tests.csproj
```

21 tests, todos deben pasar antes de cualquier commit a main.
