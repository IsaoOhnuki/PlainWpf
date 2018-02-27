using Mvvm;
using Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainWpf.ViewModels
{
    public class SubPage4ViewModel : BindableBase
    {
        public SubPage4ViewModel()
        {
            pathBuilder = new PathBuilder(((SingleBootableApp)App.Current).StartupPath.FullPath);
            pathBuilder.PropertyChanged += PathBuilder_PropertyChanged;
        }

        private void PathBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName == nameof(FilePath))
            {
                if (IsFilePathVerified || pathBuilder.FilePath.Length == 0)
                    ResetError(e.PropertyName);
                else
                    SetError("", e.PropertyName);
            }
            else if (e.PropertyName == nameof(FileName))
            {
                if (IsFileNameVerified || pathBuilder.FileName.Length == 0)
                    ResetError(e.PropertyName);
                else
                    SetError("", e.PropertyName);
            }
            else if (e.PropertyName == nameof(FullPath))
            {
                if (IsFullPathVerified || pathBuilder.FullPath.Length == 0)
                    ResetError(e.PropertyName);
                else
                    SetError("", e.PropertyName);
            }
        }

        private PathBuilder pathBuilder;
        public bool IsFullPathVerified { get { return pathBuilder.IsFullPathVerified; } }
        public bool IsFilePathVerified { get { return pathBuilder.IsFilePathVerified; } }
        public bool IsFileNameVerified { get { return pathBuilder.IsFileNameVerified; } }
        public string FilePath
        {
            get { return pathBuilder.FilePath; }
            set { pathBuilder.FilePath = value; }
        }
        public string FileName
        {
            get { return pathBuilder.FileName; }
            set { pathBuilder.FileName = value; }
        }
        public string FullPath
        {
            get { return pathBuilder.FullPath; }
            set { pathBuilder.FullPath = value; }
        }
        public string Extension
        {
            get { return pathBuilder.Extension; }
            set { pathBuilder.Extension = value; }
        }
    }
}
