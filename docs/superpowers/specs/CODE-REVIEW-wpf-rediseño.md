# Code Review · Alinear el WPF con el prototipo de rediseño

> **Para Claude Code (VSCode).** Este documento es una guía de cambios sobre `RelojControl.App`.
> Aplica los ítems en orden de prioridad. Cada uno indica archivo, problema y el cambio exacto.
> La fuente de verdad visual es el prototipo HTML del rediseño (paleta, tipografía, espaciado, comportamiento).
> **Los tokens (`Themes/Tokens.xaml`) ya están correctos** — NO cambiar colores/radios/sombras salvo lo indicado.

---

## Contexto rápido

- Stack: WPF .NET 8, MVVM. Tokens en `Themes/Tokens.xaml` (fieles al prototipo).
- Ventanas: `WndInicio`, `WndMarcaBajoTrafico`, `WndEnrolador`, `WndPanelControl`.
- Controles: `RailControl`, `NumericKeypad`, `FingerprintRing`, `NotifOverlay`.
- Objetivo de este review: cerrar la brecha visual con el prototipo. El esqueleto ya está bien; faltan escalado, marca y algunos detalles de iconografía/tipografía.

---

## 🔴 P1 · Escalado con Viewbox (impacto máximo)

**Problema.** Las ventanas usan tamaños fijos en px sin `Viewbox`. En un tótem real (768×1366, 1080×1920, 1360×768) el contenido no escala: queda chico o cortado. Esta es la causa #1 de que "no se vea como el prototipo".

**Regla de diseño.** Cada ventana se maqueta a su **resolución de diseño** y se escala con un `Viewbox Uniform` que llena la pantalla física.

- `WndMarcaBajoTrafico` (asistencia/casino vertical): lienzo **768 × 1366**.
- `WndInicio` / `WndEnrolador` / `WndPanelControl` (horizontal): lienzo **1366 × 768**.

**Cambio (aplicar en cada Window).** Envolver el contenido raíz:

```xml
<!-- Antes: <Grid> ... </Grid> directo en el Window -->
<Viewbox Stretch="Uniform" Background="{StaticResource BgBrush}">
  <Grid Width="768" Height="1366">   <!-- usar 1366×768 en las horizontales -->
    <!-- ...contenido actual sin tocar... -->
  </Grid>
</Viewbox>
```

En el `<Window>`: quitar `Width`/`Height` fijos del contenido y abrir a pantalla completa del tótem:
```xml
WindowStyle="None" ResizeMode="NoResize" WindowState="Maximized" Topmost="True"
```

**Verificación.** Correr en una resolución distinta a la de diseño (ej. 1080×1920) → el layout debe llenar la pantalla manteniendo proporción, con letterbox sobre `BgBrush` si la relación de aspecto difiere.

> Nota: el lector DPFP (`fpHost`/`ecHost` en `WindowsFormsHost` con márgenes negativos) **debe quedar FUERA del Viewbox** — un WindowsFormsHost no se escala y el SDK necesita su HWND real. Mantenerlo como hijo directo del Grid raíz del Window, hermano del Viewbox.

```xml
<Grid>  <!-- raíz directa del Window -->
  <Viewbox Stretch="Uniform"> ... contenido visual ... </Viewbox>
  <WindowsFormsHost x:Name="fpHost" Width="300" Height="200"
                    HorizontalAlignment="Left" VerticalAlignment="Top"
                    Margin="-400,-300,0,0"/>
</Grid>
```

---

## 🔴 P2 · Orientación del rail por tipo de reloj

**Problema.** `WndMarcaBajoTrafico` usa un **rail lateral izquierdo** (columna 280) en un lienzo vertical. En el prototipo:

- **Vertical** (alto tráfico / casino, 768×1366): el rail es una **banda superior** (alto ~280), no una columna lateral.
- **Horizontal** (bajo tráfico, 1366×768): rail **lateral** izquierdo (ancho ~380).

**Cambio.** Si esta ventana corre en tótems verticales, el rail debe ir arriba:

```xml
<Grid>
  <Grid.RowDefinitions>
    <RowDefinition Height="280"/>   <!-- rail banda superior -->
    <RowDefinition/>                <!-- main -->
  </Grid.RowDefinitions>
  <ctrl:RailControl x:Name="rail" Grid.Row="0"/>
  <Grid Grid.Row="1"> ...panels... </Grid>
</Grid>
```

`RailControl` necesita soportar ambos modos (ver P3). Si el bajo tráfico horizontal usa otra ventana, mantener ahí el rail lateral 380.

---

## 🔴 P3 · Logo de marca en el rail (hoy es texto plano)

**Archivo:** `Controls/RailControl.xaml`

**Problema.** El rail muestra `<TextBlock Text="rFlex.io">`. Pierde identidad: el prototipo usa el **logo real con los puntos de colores**.

**Cambio.**
1. Agregar el PNG `logo-rflex-white.png` (wordmark blanco + puntos de colores) a `Assets/` con `Build Action = Resource`.
2. Reemplazar el TextBlock:
```xml
<Image Source="pack://application:,,,/RelojControl;component/Assets/logo-rflex-white.png"
       Height="30" HorizontalAlignment="Left" DockPanel.Dock="Top"/>
```
> El PNG está disponible en el paquete de entrega del diseño (`assets/logo-rflex-white.png`). Si se prefiere vectorial, recrear el wordmark + 4 puntos (#8EA2DC, #FFD848, #8E4B90, #34B3AB) como `Path`/`Ellipse`.

---

## 🔴 P4 · Badge "Sincronizado" ilegible

**Archivo:** `Controls/RailControl.xaml`

**Problema.** `lblSync` usa `Foreground="{StaticResource TealSoftBrush}"` (#D6F3F1, casi blanco) sobre fondo teal claro → contraste nulo. El punto también es teal sobre teal.

**Cambio.**
```xml
<Ellipse x:Name="syncDot" Width="8" Height="8" Fill="#4ADE80" VerticalAlignment="Center"/>
<TextBlock x:Name="lblSync" Text="Sincronizado · en línea"
           Foreground="White" .../>
```
Y subir la opacidad del fondo del badge a ~0.16–0.20 de blanco (no teal) para que sea el "pill" translúcido del prototipo:
```xml
<Border.Background><SolidColorBrush Color="White" Opacity="0.14"/></Border.Background>
```
Estado offline: punto `#FF9A9A`, texto "Sin conexión · guardando local".

---

## 🟠 P5 · Iconografía de éxito / error

**Archivo:** `Windows/WndMarcaBajoTrafico.xaml` (panelExito / panelError)

**Problema.** `panelExito` y `panelError` reutilizan `FingerprintRing` (`fpSuccess`, `fpError`). El prototipo usa:
- **Éxito:** círculo verde (`OkBrush` 14%) con **check** dibujándose.
- **Error:** círculo coral con **ícono de alerta**.

**Cambio.** Crear un `SuccessBadge`/`StatusBadge` (UserControl) o inline con `Path`:
```xml
<!-- Éxito: badge 150×150, check verde -->
<Grid Width="150" Height="150">
  <Ellipse Fill="{StaticResource OkSoftBrush}"/>
  <Viewbox Width="82" Height="82">
    <Canvas Width="64" Height="64">
      <Path Stroke="{StaticResource OkBrush}" StrokeThickness="7"
            StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
            Data="M16 33 l11 11 l21 -23"/>
    </Canvas>
  </Viewbox>
</Grid>
```
Animación draw-on (opcional, fiel al prototipo): animar `StrokeDashOffset` de 60→0 sobre el `Path` con `StrokeDashArray="60"`.

Error: mismo patrón con `CoralSoftBrush` + `Path` de alerta `Data="M12 8 v5 M12 16.5 v.5 M12 21 a9 9 0 1 0 0 -18 a9 9 0 0 0 0 18"`.

---

## 🟠 P6 · Flechas de sentido → íconos de línea

**Archivo:** `Windows/WndMarcaBajoTrafico.xaml` (panelSentido)

**Problema.** Usa texto `↗` / `↙`. Se ven toscos. El prototipo usa íconos de línea entrar/salir.

**Cambio.** Reemplazar cada `<TextBlock Text="↗">` por un `Path` (viewBox 24, dentro de Viewbox 30×30):
```xml
<!-- Entrada (entrar) -->
<Path Stroke="{StaticResource TealBrush}" StrokeThickness="2"
      StrokeStartLineCap="Round" StrokeEndLineCap="Round" StrokeLineJoin="Round"
      Data="M14 4 h4 a2 2 0 0 1 2 2 v12 a2 2 0 0 1 -2 2 h-4 M3 12 h11 M9 7 l5 5 l-5 5"/>
<!-- Salida (salir): Data="M10 4 H6 a2 2 0 0 0 -2 2 v12 a2 2 0 0 0 2 2 h4 M21 12 H10 M16 7 l5 5 l-5 5" -->
```
(El catálogo completo de íconos como `Path` está en el handoff WPF, sección 12.)

---

## 🟠 P7 · Escala tipográfica

**Archivos:** `WndMarcaBajoTrafico.xaml`, `RailControl.xaml`

**Problema.** Varios tamaños quedaron por debajo del diseño, debilitando la jerarquía.

| Elemento | Hoy | Diseño |
|---|---|---|
| Eyebrow ("MARCA TU ASISTENCIA") | 11 | **15** |
| Título ("Ingresa tu RUT") | 34 | **40** |
| Reloj (hora) | 80 | **92** (vertical: 96) |
| Subtítulo de sentido | 13 | **14.5** |

Ajustar `FontSize` a la columna "Diseño". (Con el Viewbox de P1 todo escala parejo después.)

---

## 🟢 P8 · Copy / textos

- panelSentido título: "¿Qué tipo de marca?" → **"Marca tu jornada"** (o mantener si negocio lo prefiere; el prototipo usa este).
- Verificar que los mensajes de estado/resultado usen los textos exactos del prototipo (tabla maestra en el handoff WPF, sección 13): intentos de huella, "Marca ya registrada", "Ud no tiene permitido el marcaje en este reloj", casino sin tickets / fuera de horario, etc.

---

## Checklist de aplicación

- [ ] P1 · Viewbox + lienzo a resolución de diseño en las 4 ventanas (DPFP host fuera del Viewbox)
- [ ] P2 · Rail banda-superior en verticales / lateral en horizontales
- [ ] P3 · Logo real en el rail (Assets/logo-rflex-white.png)
- [ ] P4 · Badge "Sincronizado" legible (texto blanco, punto verde, fondo blanco translúcido)
- [ ] P5 · SuccessBadge (check verde) + ErrorBadge (alerta coral) — dejar de reusar FingerprintRing
- [ ] P6 · Íconos de línea en sentido (Path) en vez de flechas de texto
- [ ] P7 · Escala tipográfica (eyebrow 15, título 40, reloj 92/96)
- [ ] P8 · Copy y textos según tabla maestra
- [ ] Compilar + `dotnet test` (21 tests deben pasar) antes de commit

---

## Referencias

- **Prototipo HTML** del rediseño → fuente de verdad visual (medidas, estados, comportamiento).
- **Handoff WPF** → §11 medidas exactas por pantalla · §12 catálogo de íconos `Path` · §13 tabla maestra de textos.
- **Tokens** (`Themes/Tokens.xaml`) → ya alineados; reutilizar siempre `{StaticResource ...}`, no hardcodear hex.
