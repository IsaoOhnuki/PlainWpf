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
        public HistoryStack<Stroke> Strokes { get; set; } = new HistoryStack<Stroke>(false);

        private Size actualSize;
        public Size ActualSize
        {
            get { return actualSize; }
            set
            {
                actualSize = value;
                OnPropertyChanged(nameof(ActualSize));
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

        public Action<Size> ActualSizeChanged { get; set; }
        public DelegateCommand Undo { get; set; }
        public DelegateCommand Redo { get; set; }
        public DelegateCommand Load { get; set; }
        public DelegateCommand Draw { get; set; }

        public SubPage5ViewModel()
        {
            Draw = new DelegateCommand(() => {
                ImageSource = InkCanvasBehavior.DrawStrokes(ImageSource as BitmapSource, Strokes, ActualSize);
                Strokes.Clear();
            });
            Load = new DelegateCommand(() => {
                var dialogMessage = new OpenFileDialogMessage() { Title = "" };
                var result = MessengerService.SendMessage(typeof(SubPage5ViewModel), dialogMessage);
                if (((OpenFileDialogMessage)result).DialogResult)
                {
                    ImageSource = new BitmapImage(new Uri(dialogMessage.FileName));
                }
            });
            Undo = new DelegateCommand(() => {
                Strokes.Undo();
            });
            Redo = new DelegateCommand(() => {
                Strokes.Redo();
            });
            ActualSizeChanged = (Size sz) => ActualSize = sz;
        }
    }
}
