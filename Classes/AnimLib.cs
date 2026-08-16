using System.Windows;
using System.Windows.Media.Animation;
class AnimLib
{
    public static readonly IEasingFunction Quad = new QuadraticEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Sine = new SineEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Back = new BackEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Bounce = new BounceEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Circle = new CircleEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Cubic = new CubicEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Elastic = new ElasticEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Exponential = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Power = new PowerEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Quartic = new QuarticEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Quintic = new QuinticEase { EasingMode = EasingMode.EaseInOut };
    private static readonly IEasingFunction DefaultEase = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

    public static Task FadeAsync(
        UIElement target,
        double from = 0,
        double to = 1,
        int milliseconds = 500,
        IEasingFunction? easingFunction = null)
    {
        if (target == null) return Task.CompletedTask;

        var tcs = new TaskCompletionSource<bool>();

        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(milliseconds),
            EasingFunction = easingFunction ?? new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };

        var storyboard = new Storyboard();
        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

        storyboard.Completed += (s, e) =>
        {
            target.Opacity = to;
            tcs.TrySetResult(true);
        };

        storyboard.Children.Add(animation);
        storyboard.Begin();

        return tcs.Task;
    }

    public static Task MoveMarginAsync(
        FrameworkElement target,
        Thickness from,
        Thickness to,
        int milliseconds = 750,
        IEasingFunction? easingFunction = null)
    {
        if (target == null) return Task.CompletedTask;

        var tcs = new TaskCompletionSource<bool>();

        var animation = new ThicknessAnimation
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(milliseconds),
            EasingFunction = easingFunction ?? DefaultEase
        };

        var storyboard = new Storyboard();
        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(FrameworkElement.MarginProperty));

        storyboard.Completed += (s, e) =>
        {
            target.Margin = to;
            tcs.TrySetResult(true);
        };

        storyboard.Children.Add(animation);
        storyboard.Begin();

        return tcs.Task;
    }

    public static void MoveMargin(
            FrameworkElement target,
            Thickness from,
            Thickness to,
            int milliseconds = 750,
            IEasingFunction? easingFunction = null)
    {
        _ = MoveMarginAsync(target, from, to, milliseconds, easingFunction);
    }

    public static void Fade(
            UIElement target,
            double from = 0,
            double to = 1,
            int milliseconds = 500)
    {
        _ = FadeAsync(target, from, to, milliseconds);
    }

    public static void DoubleAnimation(
            DependencyObject target,
            string propertyPath,
            double from = 0,
            double to = 1,
            int milliseconds = 500,
            IEasingFunction? easingFunction = null,
            Action? onCompleted = null)
    {
        if (target == null || string.IsNullOrEmpty(propertyPath)) return;

        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new TimeSpan(0, 0, 0, 0, milliseconds),
            EasingFunction = easingFunction
        };

        var storyboard = new Storyboard();
        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));

        if (onCompleted != null)
        {
            storyboard.Completed += (s, e) => onCompleted();
        }

        storyboard.Children.Add(animation);
        storyboard.Begin();
    }
}