using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Behaviors
{
    public class ActualSizeBehavior
    {
        public static Size GetActualSize(DependencyObject target)
        {
            return (Size)target.GetValue(ActualSizeProperty);
        }

        public static void SetActualSize(DependencyObject target, Size value)
        {
            target.SetValue(ActualSizeProperty, value);
        }

        private static readonly DependencyProperty ActualSizeProperty = DependencyProperty.RegisterAttached(
            "ActualSize",
            typeof(Size),
            typeof(ActualSizeBehavior),
            // デフォルト値がdefault(Size)だとバインディング先の初期値に悩むため違う値にする必要がある、でも負値は使用できない
            new PropertyMetadata(new Size(double.NaN, double.NaN), OnActualSizeChanged));

        private static void OnActualSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (e.OldValue != null)
                {
                    element.SizeChanged -= Element_SizeChanged;
                }
                if (e.NewValue != null)
                {
                    element.SizeChanged += Element_SizeChanged;
                }
            }
        }

        private static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((FrameworkElement)sender).SetCurrentValue(ActualSizeProperty, e.NewSize);
        }
    }
}
