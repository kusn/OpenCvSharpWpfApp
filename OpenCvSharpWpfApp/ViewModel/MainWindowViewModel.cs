using System;
using System.Windows.Media.Imaging;
using OpenCvSharpWpfApp.Base.ViewModel;

namespace OpenCvSharpWpfApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _openFileName;
        private string _saveFileName;
        private BitmapImage _image;
        private FilterEnum _selecetdFilter;

        public BitmapImage Image
        {
            get => _image;
            set => Set(ref _image, value);
        }

        public FilterEnum SelectedFilter
        {
            get => _selecetdFilter;
            set => Set(ref _selecetdFilter, value);
        }

        public FilterEnum[] AvailableFilters { get; } = Enum.GetValues<FilterEnum>();

        public MainWindowViewModel()
        {
            
        }
    }
}
