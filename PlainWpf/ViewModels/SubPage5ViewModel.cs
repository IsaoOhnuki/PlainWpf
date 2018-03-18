using Behaviors;
using Mvvm;
using MvvmOption;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        public DrawingAttributes InkPen { get; set; } = new DrawingAttributes { Width = 10, Height = 10, StylusTip = StylusTip.Ellipse };

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

        public bool CanUndo { get { return InkStrokes.Count > 0; } }

        public DelegateCommand Undo { get; set; }
        public DelegateCommand Redo { get; set; }
        public DelegateCommand Load { get; set; }
        public DelegateCommand Save { get; set; }
        public DelegateCommand Draw { get; set; }

        public SubPage5ViewModel()
        {
            InkStrokes.StrokesChanged += InkStrokes_StrokesChanged;
            Draw = new DelegateCommand(() => {
                //ImageSource = InkCanvasBehavior.DrawStrokes(ImageSource as BitmapSource, Strokes, ActualSize);
                //Strokes.Clear();
                ImageSource = ImageSourceUtility.DrawStrokes(ImageSource as BitmapSource, InkStrokes, new Size(ActualWidth, ActualHeight));
                InkStrokes.Clear();
            });
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
                //Strokes.Undo();
                InkStrokes.RemoveAt(InkStrokes.Count - 1);
            });
            Redo = new DelegateCommand(() => {
                //Strokes.Redo();
            });
        }

        private void InkStrokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanUndo));
        }
    }
}
