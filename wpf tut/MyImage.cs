using System;

namespace wpf_tut
{
    public class MyImage
    {
        private Pixel[,] _pixels;
        private readonly int _hight;
        private readonly int _width;

        public int Hight => _hight;
        public int Width => _width;

        public MyImage(int hight, int width)
        {
            _pixels = new Pixel[hight, width];
            _hight = hight;
            _width = width;
            for (var y = 0; y < _hight; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    _pixels[x, y] = new Pixel();
                }
            }
        }

        public void SetPixel(int x, int y, int r, int g, int b, int a = 255)
        {
            if (x > _width || y > _hight || x < 0 || y < 0)
            {
                throw new Exception("pixel location outside of image");
            }

            _pixels[x, y].SetRGBA(r, g, b, a);
        }

        public void FillImage(int r, int g, int b, int a)
        {
            foreach (var pixel in _pixels)
            {
                pixel.SetRGBA(r, g, b, a);
            }
        }
    }
}