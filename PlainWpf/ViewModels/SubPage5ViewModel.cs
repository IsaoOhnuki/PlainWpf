using Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;

namespace PlainWpf.ViewModels
{
    public class SubPage5ViewModel : BindableBase
    {
        public ObservableCollection<Stroke> Strokes { get; set; } = new ObservableCollection<Stroke>();

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

        public Action<Size> ActualSizeChanged { get; set; }
        public DelegateCommand Undo { get; set; }

        public SubPage5ViewModel()
        {
            Strokes.CollectionChanged += Strokes_CollectionChanged;
            Undo = new DelegateCommand(() => {
                if (Strokes.Count > 0)
                {
                    Strokes.Remove(Strokes.Last());
                }
            });
            ActualSizeChanged = (Size sz) => ActualSize = sz;
    }

        private void Strokes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var val in e.NewItems)
            {

            }
        }
    }
}
