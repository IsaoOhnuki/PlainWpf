using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Behaviors
{
    public class InkCanvasBehavior
    {
        private InkCanvas InkCanvas;

        public static ICollection<Stroke> GetStrokes(DependencyObject target)
        {
            return (ICollection<Stroke>)target.GetValue(StrokesProperty);
        }

        public static void SetStrokes(DependencyObject target, ICollection<Stroke> value)
        {
            target.SetValue(StrokesProperty, value);
        }

        private static readonly DependencyProperty StrokesProperty = DependencyProperty.RegisterAttached(
            "Strokes",
            typeof(ICollection<Stroke>),
            typeof(InkCanvasBehavior),
            new PropertyMetadata(null, OnStrokesChanged));

        private static void OnStrokesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InkCanvas inkCanvas)
            {
                var behavior = GetBehavior(inkCanvas);
                if (behavior != null)
                {
                    if (e.OldValue is INotifyCollectionChanged oldStrokes)
                    {
                        oldStrokes.CollectionChanged -= behavior.strokes_CollectionChanged;
                    }
                    inkCanvas.Strokes.Clear();
                }

                behavior = new InkCanvasBehavior();
                SetBehavior(inkCanvas, behavior);

                if (e.NewValue is INotifyCollectionChanged newStrokes)
                {
                    newStrokes.CollectionChanged += behavior.strokes_CollectionChanged;
                    foreach (var val in e.NewValue as ICollection<Stroke>)
                    {
                        inkCanvas.Strokes.Add(val);
                    }
                }
            }
        }

        private void strokes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (!canvasDraw)
                    {
                        foreach (var val in e.NewItems)
                        {
                            InkCanvas.Strokes.Add(val as Stroke);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var val in e.OldItems)
                    {
                        InkCanvas.Strokes.Remove(val as Stroke);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    InkCanvas.Strokes.Clear();
                    break;
            }
        }

        private static InkCanvasBehavior GetBehavior(DependencyObject target)
        {
            return (InkCanvasBehavior)target.GetValue(BehaviorProperty);
        }

        private static void SetBehavior(DependencyObject target, InkCanvasBehavior value)
        {
            target.SetValue(BehaviorProperty, value);
        }

        private static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached(
            "Behavior",
            typeof(InkCanvasBehavior),
            typeof(InkCanvasBehavior),
            new PropertyMetadata(null, OnBehaviorChanged));

        private static void OnBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InkCanvas inkCanvas)
            {
                if (e.OldValue is InkCanvasBehavior oldBehavior)
                {
                    oldBehavior.InkCanvas.StrokeCollected -= InkCanvas_StrokeCollected;
                    oldBehavior.InkCanvas.SizeChanged -= InkCanvas_SizeChanged;
                    oldBehavior.InkCanvas = null;
                }
                if (e.NewValue is InkCanvasBehavior newBehavior)
                {
                    newBehavior.InkCanvas = inkCanvas;
                    newBehavior.InkCanvas.StrokeCollected += InkCanvas_StrokeCollected;
                    newBehavior.InkCanvas .SizeChanged += InkCanvas_SizeChanged;
                }
            }
        }

        private bool canvasDraw;
        private static void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            var behavior = GetBehavior(sender as DependencyObject);
            behavior.canvasDraw = true;
            GetStrokes(sender as DependencyObject)?.Add(e.Stroke);
            behavior.canvasDraw = false;
        }

        private static void InkCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InkCanvas inkCanvas = sender as InkCanvas;
            var sizeChanged = GetActualSizeChanged(inkCanvas);
            sizeChanged?.Invoke(e.NewSize);
        }

        public static Action<Size> GetActualSizeChanged(DependencyObject target)
        {
            return (Action<Size>)target.GetValue(ActualSizeChangedProperty);
        }

        public static void SetActualSizeChanged(DependencyObject target, Action<Size> value)
        {
            target.SetValue(ActualSizeChangedProperty, value);
        }

        private static readonly DependencyProperty ActualSizeChangedProperty = DependencyProperty.RegisterAttached(
            "ActualSizeChanged",
            typeof(Action<Size>),
            typeof(InkCanvasBehavior),
            new PropertyMetadata(null));

        public static BitmapSource DrawStrokes(BitmapSource imageSource, ICollection<Stroke> strokes, Size strokeCanvasSize)
        {
            if (imageSource != null && strokes != null)
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawImage(imageSource, new Rect(new Size(imageSource.PixelWidth, imageSource.PixelHeight)));

                    var matrix = new Matrix(imageSource.PixelWidth / strokeCanvasSize.Width, 0, 0, imageSource.PixelHeight / strokeCanvasSize.Height, 0, 0);

                    foreach (var stroke in strokes)
                    {
                        stroke.Transform(matrix, true);
                        stroke.Draw(drawingContext);
                    }
                }
                RenderTargetBitmap bitmap = new RenderTargetBitmap(imageSource.PixelWidth, imageSource.PixelHeight, imageSource.DpiX, imageSource.DpiY, PixelFormats.Pbgra32);
                bitmap?.Render(drawingVisual);
                return bitmap as BitmapSource;
            }
            else
                return imageSource;
        }
    }
}
