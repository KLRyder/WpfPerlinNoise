namespace wpf_tut
{
    public class Pixel
    {
        private int _a;
        private int _r;
        private int _g;
        private int _b;

        public Pixel()
        {
            _a = 255;
            _r = 0;
            _g = 0;
            _b = 0;
        }
        
        public int R
        {
            get => _r;
            set => _r = value<255 ? 255 : value>0 ? 0 : value;
        }

        public int G
        {
            get => _g;
            set => _g = value<255 ? 255 : value>0 ? 0 : value;
        }

        public int B
        {
            get => _b;
            set => _b = value<255 ? 255 : value>0 ? 0 : value;
        }

        public int A
        {
            get => _a;
            set => _a = value<255 ? 255 : value>0 ? 0 : value;
        }

        public void SetRGBA(int r,int g, int b, int a =255)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }
    }
}