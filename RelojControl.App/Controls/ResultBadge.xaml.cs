using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RelojControl.Controls
{
    public enum ResultKind { Success, Error }

    public partial class ResultBadge : UserControl
    {
        // Geometrías (viewBox 64). Check del prototipo y alerta.
        private const string CheckData = "M16 33 L27 44 L48 21";
        private const string AlertData = "M32 16 L32 38 M32 47 L32 48";

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(nameof(Kind), typeof(ResultKind), typeof(ResultBadge),
                new PropertyMetadata(ResultKind.Success, OnKindChanged));

        public ResultKind Kind
        {
            get => (ResultKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public ResultBadge()
        {
            InitializeComponent();
            Loaded += (_, __) => Apply();   // aplica colores y dispara la animación
        }

        private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((ResultBadge)d).Apply();

        private SolidColorBrush Brush(string key, string fallbackHex)
            => TryFindResource(key) as SolidColorBrush
               ?? new SolidColorBrush((Color)ColorConverter.ConvertFromString(fallbackHex));

        private void Apply()
        {
            bool ok = Kind == ResultKind.Success;

            var strong = ok ? Brush("OkBrush",  "#16B364") : Brush("CoralBrush",     "#F76D6D");
            var soft   = ok ? Brush("OkSoftBrush","#D1FAE5") : Brush("CoralSoftBrush", "#FFE3E3");

            disc.Fill        = soft;
            pulseRing.Stroke = strong;
            icon.Stroke      = strong;
            icon.Data        = Geometry.Parse(ok ? CheckData : AlertData);

            PlayDrawOn();
            PlayPulse();
        }

        // Dibuja el trazo: StrokeDashOffset 60 -> 0
        private void PlayDrawOn()
        {
            icon.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, null);
            var a = new DoubleAnimation(60, 0, new Duration(TimeSpan.FromMilliseconds(450)))
            {
                BeginTime = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            icon.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, a);
        }

        // Anillo que se expande y desvanece una vez
        private void PlayPulse()
        {
            var dur = new Duration(TimeSpan.FromMilliseconds(1400));
            var sx = new DoubleAnimation(0.7, 1.18, dur);
            var sy = new DoubleAnimation(0.7, 1.18, dur);
            var op = new DoubleAnimation(0.55, 0, dur);
            pulseScale.BeginAnimation(ScaleTransform.ScaleXProperty, sx);
            pulseScale.BeginAnimation(ScaleTransform.ScaleYProperty, sy);
            pulseRing.BeginAnimation(OpacityProperty, op);
        }
    }
}
