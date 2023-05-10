using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharpWpfApp.Base.Command;
using OpenCvSharpWpfApp.Base.ViewModel;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OpenCvSharpWpfApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _openFileName;
        private string _saveFileName;
        private Bitmap _bmp;
        private BitmapImage _image;
        private FilterEnum _selecetdFilter = FilterEnum.None;
        private bool _isEnabled = false;
        
        public BitmapImage Image
        {
            get => _image;
            set => Set(ref _image, value);
        }

        public FilterEnum SelectedFilter
        {
            get => _selecetdFilter;
            set
            {
                Set(ref _selecetdFilter, value);
                Image = Convert(_openFileName, SelectedFilter);
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Set(ref _isEnabled, value);
        }

        public FilterEnum[] AvailableFilters { get; } = Enum.GetValues<FilterEnum>();

        #region OpenImageCommand

        private LambdaCommand _openImageCommand;
        public LambdaCommand OpenImageCommand => _openImageCommand
            ??= new LambdaCommand(OnOpenImageCommandExecuted, CanOpenImageCommandExecute);
        private void OnOpenImageCommandExecuted(object p)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.bmp;*.jpg;*.png";

            try
            {
                if (ofd.ShowDialog() == true)
                {
                    _openFileName = ofd.FileName;
                    Image = new BitmapImage(new Uri(_openFileName));
                    IsEnabled = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private bool CanOpenImageCommandExecute(object p) => true;

        #endregion

        #region SaveImageCommand

        private LambdaCommand _saveImageCommand;
        public LambdaCommand SaveImageCommand => _saveImageCommand
            ??= new LambdaCommand(OnSaveImageCommandExecuted, CanSaveImageCommandExecute);

        private void OnSaveImageCommandExecuted(object p)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP files (*.bmp)|*.bmp|JPEG files (*.jpg)|*.jpg|PNG files (*.png)|*.png";
            if (sfd.ShowDialog() == true)
            {
                int index = sfd.FilterIndex;
                _saveFileName = sfd.FileName;
                Save(Image, _saveFileName, index);
            }
        }
        private bool CanSaveImageCommandExecute(object p) => SelectedFilter != FilterEnum.None;

        #endregion

        public MainWindowViewModel()
        {
            
        }

        private BitmapImage Convert(string filename, FilterEnum filter)
        {
            BitmapImage bImg = new BitmapImage();
            ImreadModes imreadModes = ImreadModes.Unchanged;
            Mat mat = new Mat();

            using (var t = new ResourcesTracker())
            {
                if (filter == FilterEnum.None)
                {
                    bImg = new BitmapImage(new Uri(_openFileName));
                    return bImg;
                }
                else if (filter == FilterEnum.GrayScale)
                {
                    imreadModes = ImreadModes.Grayscale;
                    mat = t.T(new Mat(filename, imreadModes));
                }
                else if (filter == FilterEnum.Median)
                {
                    var src = t.T(new Mat(filename, ImreadModes.AnyColor));
                    Cv2.MedianBlur(src, mat, 11);
                }

                var bitmap = BitmapConverter.ToBitmap(mat);

                Bitmap ImageOriginalBase = new Bitmap(bitmap);
                
                using (MemoryStream ms = new MemoryStream())
                {
                    ImageOriginalBase.Save(ms, ImageFormat.Png);
                    bImg.BeginInit();
                    bImg.StreamSource = ms;
                    bImg.CacheOption = BitmapCacheOption.OnLoad;
                    bImg.EndInit();
                    bImg.Freeze();
                }
            }
            return bImg;
        }

        private static void Save(BitmapImage image, string filePath, int filterIndex)
        {
            BitmapEncoder encoder = null;

            if (filterIndex == 1)
                encoder = new BmpBitmapEncoder();
            else if (filterIndex == 2)
                encoder = new JpegBitmapEncoder();
            else if (filterIndex == 3)
                encoder = new PngBitmapEncoder();
            
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
    }
}
