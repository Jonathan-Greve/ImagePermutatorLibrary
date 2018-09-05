using System.Drawing;

namespace ImagePermutatorLibrary
{
    public class ImageSheet
    {
        private int numRows, numCols;
        private int borderThicknessW, borderThicknessH;
        private bool willRotateCroppedImage, willRotateOutputImage = false;

        public Image SourceImage { get; private set; }
        public Image CroppedImage { get; private set; }
        public Image OutputImage { get; private set; }
        public SheetFormat SheetFormat { get; private set; }
        public ImageFormat ImageFormat { get; private set; }
        public Rectangle CropArea { get; private set; }
        public RotateFlipType CroppedImageRotation { get; private set; }
        public RotateFlipType OutputImageRotation { get; private set; }

        public void SetSheetFormat(SheetFormat sheetFormat) => SheetFormat = sheetFormat;
        public void SetImageFormat(ImageFormat imageFormat) => ImageFormat = imageFormat;
        public void SetCropArea(int xStart, int yStart, int xEnd, int yEnd) =>
            CropArea = new Rectangle(new Point(xStart, yStart), new Size(xEnd - xStart, yEnd - yStart));
        public void SetCroppedImageRotation(RotateFlipType rotateFlipType)
        {
            CroppedImageRotation = rotateFlipType;
            willRotateCroppedImage = true;
        }
        public void SetOutputImageRotation(RotateFlipType rotateFlipType)
        {
            OutputImageRotation = rotateFlipType;
            willRotateOutputImage = true;
        }

        public void Create()
        {
            CreateOutputImage();
            DrawOutputImage();
        }

        private void DrawOutputImage()
        {
            CreateCroppedImage();

            using (Graphics imageHelper = Graphics.FromImage(OutputImage))
            {
                imageHelper.FillRectangle(new SolidBrush(Color.White), 0, 0, OutputImage.Width, OutputImage.Height);

                for (int col = 0; col < numCols; col++)
                {
                    for (int row = 0; row < numRows; row++)
                    {
                        Rectangle insertPosition = new Rectangle(
                            borderThicknessW + col * (CropArea.Width + borderThicknessW),
                            borderThicknessH + row * (CropArea.Height + borderThicknessH),
                            CropArea.Width,
                            CropArea.Height);
                        imageHelper.DrawImage(CroppedImage, insertPosition, new Rectangle(0, 0, CroppedImage.Width, CroppedImage.Height), GraphicsUnit.Pixel);
                    }
                }
            }
            if (willRotateOutputImage)
            {
                OutputImage.RotateFlip(OutputImageRotation);
            }
        }

        private void CreateCroppedImage()
        {
            CroppedImage = new Bitmap(CropArea.Width, CropArea.Height);
            using (Graphics croppedImageHelper = Graphics.FromImage(CroppedImage))
            {
                croppedImageHelper.DrawImage(SourceImage, new Rectangle(0, 0, CroppedImage.Width, CroppedImage.Height), CropArea, GraphicsUnit.Pixel);
            }
            if (willRotateCroppedImage)
                CroppedImage.RotateFlip(CroppedImageRotation);
        }

        private void CalcNumberOfRowsAndColumns()
        {
            numRows = SheetFormat.Height / ImageFormat.Height;
            numCols = SheetFormat.Width / ImageFormat.Width;
        }

        private void CreateOutputImage()
        {
            CalcNumberOfRowsAndColumns();
            CalcBorderThickness();
            double aspectRatio = (double)SheetFormat.Width / SheetFormat.Height;
            int OutputImageWidth = numCols * CropArea.Width + borderThicknessW * (numCols + 1);
            int OutputImageHeight = (int)(OutputImageWidth / aspectRatio);
            OutputImage = new Bitmap(OutputImageWidth, OutputImageHeight);
        }

        private void CalcBorderThickness()
        {
            borderThicknessW = (int)((double)(SheetFormat.Width % ImageFormat.Width) / ImageFormat.Width * CropArea.Width / (numCols + 1));
            borderThicknessH = (int)((double)(SheetFormat.Height % ImageFormat.Height) / ImageFormat.Height * CropArea.Height / (numRows + 1));
        }

        static public ImageSheet FromImage(Image sourceImage)
        {
            return new ImageSheet(sourceImage);
        }

        private ImageSheet(Image SourceImage)
        {
            this.SourceImage = SourceImage;
        }
    }

    public abstract class SheetFormat
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
    }

    public class A4 : SheetFormat
    {
        override public int Width { get; } = 210;
        override public int Height { get; } = 297;
    }
    public class CustomSheet : SheetFormat
    {
        override public int Width { get; }
        override public int Height { get; }

        public CustomSheet(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    public abstract class ImageFormat
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
    }
    public class PassportPhotoFormat : ImageFormat
    {
        override public int Width { get; } = 51;
        override public int Height { get; } = 51;
    }

    public class ChineseVisaPhotoFormat : ImageFormat
    {
        override public int Width { get; } = 38;
        override public int Height { get; } = 48;
    }

    public class CustomImageFormat : ImageFormat
    {
        override public int Width { get; }
        override public int Height { get; }

        public CustomImageFormat(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
