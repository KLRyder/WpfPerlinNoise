using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace wpf_tut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private PerlinNoiseGenerator2D _pnoise;
        private const double PnoiseScaler = 0.75;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _pnoise = new PerlinNoiseGenerator2D(4000, 4000);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message+"\n"+"Source: MainWindow()");
            }

            var uri = new Uri(@"\cat.png", UriKind.Relative);
            ImgDynamic.Source = new BitmapImage(uri);
        }

        private void GenerateStatic(object sender, RoutedEventArgs e)
        {
            _pnoise = new PerlinNoiseGenerator2D(4000, 4000);
            GenStaticMain();
        }


        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GenStaticMain();
        }

        private void GenStaticMain()
        {
            var b = new Bitmap(256, 256);

            try
            {
                int x, y;
                var max = 0.0;
                var min = 0.0;

                var tempVals = new double[256, 256];

                // Loop through the images pixels to reset color.
                for (x = 0; x < b.Width; x++)
                {
                    for (y = 0; y < b.Height; y++)
                    {
                        double noiselvl;

                        noiselvl = _pnoise.GenerateOctavesValue(x+int.Parse(OffsetBox.Text), y+int.Parse(OffsetBox.Text), SliderScale.Value, int.Parse(OctaveBox.Text),
                            SliderPersistance.Value, SliderLacunarity.Value);

                        if (noiselvl > max)
                        {
                            max = noiselvl;
                        }
                        else if (noiselvl < min)
                        {
                            min = noiselvl;
                        }

                        tempVals[x, y] = noiselvl;
                    }
                }

                var variation = max - min;

                for (x = 0; x < b.Width; x++)
                {
                    for (y = 0; y < b.Height; y++)
                    {
                        var pixelVal = tempVals[x, y];
                        pixelVal -= min;
                        pixelVal /= variation;
                        var noise = Convert.ToInt32(255 * pixelVal);
                        b.SetPixel(x, y, Color.FromArgb(255, noise, noise, noise));
                    }
                }

                ImgDynamic.Source = ConvertImageToImgSrc(b);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message+"\n"+"Source: GenStaticMain");
            }
        }

        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        private static BitmapImage ConvertImageToImgSrc(Image src)
        {
            var ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        // Unlikely to use this again, was used while tweaking the PerlinNoiseGenerator to work out the required scaler.
//        private void Test(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                var max = 0.0;
//                var min = 0.0;
//                for (var i = 0; i < 10000; i++)
//                {
//                    for (var j = 0; j < 10000; j++)
//                    {
//                        var num =_pnoise.GenerateValue(i, j, 3.4234);
//                        if (num>max)
//                        {
//                            max = num;
//                        }
//                        else if (min>num)
//                        {
//                            min = num;
//                        }
//                    }
//                }
//
//                MessageBox.Show(min + " " + max);
//            }
//            catch (Exception exception)
//            {
//                MessageBox.Show(exception.Message);
//            }
//        }

        private static readonly Regex Regex = new Regex("[^0-9]+");

        private void ForceNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text);
        }

        private void Octave_box_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            GenStaticMain();
        }
    }
}