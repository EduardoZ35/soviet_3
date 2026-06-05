using System;
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
        scanArc.Visibility        = Visibility.Collapsed;
        successOverlay.Visibility = Visibility.Collapsed;
        bgCircle.Fill             = (System.Windows.Media.Brush)FindResource("AccentSoftBrush");
        fpIcon.Stroke             = (System.Windows.Media.Brush)FindResource("AccentBrush");

        switch (state)
        {
            case FingerprintState.Scanning:
                scanArc.Visibility = Visibility.Visible;
                break;
            case FingerprintState.Success:
                bgCircle.Fill             = (System.Windows.Media.Brush)FindResource("OkSoftBrush");
                fpIcon.Stroke             = (System.Windows.Media.Brush)FindResource("OkBrush");
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
        var anim = new DoubleAnimation(120, 0, new Duration(TimeSpan.FromMilliseconds(500)))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        checkPath.BeginAnimation(System.Windows.Shapes.Path.StrokeDashOffsetProperty, anim);
    }
}
