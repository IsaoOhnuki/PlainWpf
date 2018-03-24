using Behaviors;
using Mvvm;
using MvvmOption;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilitys;

namespace PlainWpf.ViewModels
{
    public class SubPage5ViewModel : BindableBase
    {
        public StrokeCollection InkStrokes { get; set; } = new StrokeCollection();
        public StrokeCollection InkStrokesB { get; set; } = new StrokeCollection();

        private double actualWidth;
        public double ActualWidth
        {
            get { return actualWidth; }
            set
            {
                actualWidth = value;
                OnPropertyChanged(nameof(ActualWidth));
            }
        }

        private double actualHeight;
        public double ActualHeight
        {
            get { return actualHeight; }
            set
            {
                actualHeight = value;
                OnPropertyChanged(nameof(ActualHeight));
            }
        }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged();
            }
        }

        private Color penColor;
        public Color PenColor
        {
            get { return penColor; }
            set
            {
                penColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(InkPen));
            }
        }

        private StylusTip penTip;
        public StylusTip PenTip
        {
            get { return penTip; }
            set
            {
                penTip = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEllipsePen));
                OnPropertyChanged(nameof(IsRectanglePen));
                OnPropertyChanged(nameof(InkPen));
            }
        }

        public bool IsEllipsePen { get { return PenTip == StylusTip.Ellipse; } }
        public bool IsRectanglePen { get { return PenTip == StylusTip.Rectangle; } }

        private PenSizeType penSize;
        public PenSizeType PenSize
        {
            get { return penSize; }
            set
            {
                penSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLagePen));
                OnPropertyChanged(nameof(IsMediumPen));
                OnPropertyChanged(nameof(IsSmallPen));
                OnPropertyChanged(nameof(InkPen));
            }
        }

        public bool IsLagePen { get { return PenSize == PenSizeType.Lage; } }
        public bool IsMediumPen { get { return PenSize == PenSizeType.Medium; } }
        public bool IsSmallPen { get { return PenSize == PenSizeType.Small; } }

        public DrawingAttributes InkPen
        {
            get
            {
                switch (PenSize)
                {
                    case PenSizeType.Lage:
                        return new DrawingAttributes { Width = 30, Height = 30, StylusTip = PenTip, Color = PenColor };
                    case PenSizeType.Medium:
                        return new DrawingAttributes { Width = 20, Height = 20, StylusTip = PenTip, Color = PenColor };
                    case PenSizeType.Small:
                        return new DrawingAttributes { Width = 10, Height = 10, StylusTip = PenTip, Color = PenColor };
                    default:
                        return null;
                }
            }
        }

        public bool CanUndo { get { return InkStrokes.Count > 0; } }
        public bool CanRedo { get { return InkStrokesB.Count > 0; } }

        public DelegateCommand Undo { get; set; }
        public DelegateCommand Redo { get; set; }
        public DelegateCommand Load { get; set; }
        public DelegateCommand Save { get; set; }
        public DelegateCommand Draw { get; set; }
        public DelegateCommand<object> PenTipCommand { get; set; }
        public DelegateCommand<object> PenSizeCommand { get; set; }

        public SubPage5ViewModel()
        {
            InkStrokes.StrokesChanged += InkStrokes_StrokesChanged;
            PenSizeCommand = new DelegateCommand<object>(x => PenSize = x.ToString() == PenSizeType.Lage.ToString() ? PenSizeType.Lage : x.ToString() == PenSizeType.Medium.ToString() ? PenSizeType.Medium : x.ToString() == PenSizeType.Small.ToString() ? PenSizeType.Small : (PenSizeType)x);
            PenTipCommand = new DelegateCommand<object>(x => PenTip = x.ToString() == StylusTip.Ellipse.ToString() ? StylusTip.Ellipse : x.ToString() == StylusTip.Rectangle.ToString() ? StylusTip.Rectangle : (StylusTip)x);
            Load = new DelegateCommand(() => {
                var dialogMessage = new OpenFileDialogMessage() { Title = "画像選択", Filter = "ビットマップ|*.bmp|JPEG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif|全てのファイル|*.*", FilterIndex = 5 };
                var result = MessengerService.SendMessage(typeof(SubPage5ViewModel), dialogMessage);
                if (((OpenFileDialogMessage)result).DialogResult)
                {
                    ImageSource = new BitmapImage(new Uri(dialogMessage.FileName));
                }
            });
            Save = new DelegateCommand(() => {
                var dialogMessage = new SaveFileDialogMessage() { Title = "画像保存", Filter = "ビットマップ|*.bmp|JPEG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif|全てのファイル|*.*", FilterIndex = 5 };
                var result = MessengerService.SendMessage(typeof(SubPage5ViewModel), dialogMessage);
                if (((SaveFileDialogMessage)result).DialogResult)
                {
                    ImageSourceUtility.BitmapSourceToFile(((SaveFileDialogMessage)result).FileName, ImageSource as BitmapSource);
                }
            });
            Undo = new DelegateCommand(() => {
                historied = true;
                var stroke = InkStrokes.Last();
                InkStrokesB.Add(stroke);
                InkStrokes.Remove(stroke);
                historied = false;
            });
            Redo = new DelegateCommand(() => {
                historied = true;
                var stroke = InkStrokesB.Last();
                InkStrokesB.Remove(stroke);
                InkStrokes.Add(stroke);
                historied = false;
            });
            Draw = new DelegateCommand(() => {
                ImageSource = ImageSourceUtility.DrawStrokes(ImageSource as BitmapSource, InkStrokes, new Size(ActualWidth, ActualHeight));
                historied = true;
                InkStrokes.Clear();
                InkStrokesB.Clear();
                historied = false;
            });
        }

        bool historied;
        private void InkStrokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (!historied)
            {
                if (e.Added != null)
                {
                    InkStrokesB.Clear();
                }
                if (e.Removed != null)
                {
                    foreach (var val in e.Removed.Reverse())
                    {
                        InkStrokesB.Add(val);
                    }
                }
            }
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }
    }

    //[TypeConverter(typeof(StylusTip))]
    //public sealed class StylusTipTypeConverter : TypeConverter
    //{
    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        return sourceType == typeof(StylusTip);
    //    }
    //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    //    {
    //        return base.ConvertFrom(context, culture, value);
    //    }
    //}

    public enum PenSizeType
    {
        Lage,
        Medium,
        Small,
    }

    [TypeConverter(typeof(PenSizeType))]
    public sealed class PenSizeTypeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(PenSizeType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }
    }
}
