namespace PDNWrapper
{
    // Mimics System.Drawing.Size
    internal struct Size
    {
        public static readonly Size Empty = new Size();

        private int width;
        private int height;

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool IsEmpty
        {
            get
            {
                return width == 0 && height == 0;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
    }
}
