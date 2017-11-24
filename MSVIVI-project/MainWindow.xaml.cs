using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
using AForge.Imaging.Filters;

namespace MSVIVI_project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void ImportButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select picture",
                Filter = "All supported formats (*.jpg,*.png,*.bmp,*.gif,*.tif)|*.jpg;*.png;*.bmp;*.gif;*.tif"
            };

            if (fileDialog.ShowDialog() == true)
            {
                var uri = new Uri(fileDialog.FileName);
                this.DefaultImage.Source = new BitmapImage(uri);
            }
        }

        private void BilinearInterpolationChecked(object sender, RoutedEventArgs e)
        {
            var filter = new BayerFilter();
            var bitmapImage = this.Convert(this.DefaultImage);
            var rgbImage = filter.Apply(bitmapImage);

            this.ModifiedImage.Source = this.Make(rgbImage);
        }

        private Bitmap Convert(System.Windows.Controls.Image img)
        {
            var ms = new MemoryStream();
            var bbe = new BmpBitmapEncoder();
            bbe.Frames.Add(BitmapFrame.Create(new Uri(img.Source.ToString(), UriKind.RelativeOrAbsolute)));

            bbe.Save(ms);
            var img2 = Image.FromStream(ms);
            return img2 as Bitmap;
        }

        private BitmapImage Make(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();

                return bi;
            }
        }

        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "BMP (*.bmp)|*.bmp;|TIF (*.tif)|*.tif"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)this.ModifiedImage.Source));
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
        }

        private void ApplyBayerFilter(object sender, RoutedEventArgs e)
        {
            var filter = new BayerFilter();
            var bitmapImage = this.Convert(this.DefaultImage);
            var rgbImage = filter.Apply(bitmapImage);

            this.ModifiedImage.Source = this.Make(rgbImage);
        }
    }
}
