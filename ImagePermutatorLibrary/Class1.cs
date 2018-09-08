using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ImagePermutatorLibrary
{

    public class ImageSheet
    {
        private int numRows, numCols;
        private int borderThicknessW, borderThicknessH;

        public Image<Rgba32> SourceImage { get; private set; }
        public Image<Rgba32> CroppedImage { get; private set; }
        public Image<Rgba32> OutputImage { get; private set; }
        public SheetFormat SheetFormat { get; private set; }
        public ImageFormat ImageFormat { get; private set; }
        public Rectangle CropArea { get; private set; }
        public float CroppedImageRotationDegrees { get; private set; }
        public float OutputImageRotationDegrees { get; private set; }

        public void SetSheetFormat(SheetFormat sheetFormat) => SheetFormat = sheetFormat;
        public void SetImageFormat(ImageFormat imageFormat) => ImageFormat = imageFormat;
        public void SetCropArea(int xStart, int yStart, int xEnd, int yEnd) =>
            CropArea = new Rectangle(new Point(xStart, yStart), new Size(xEnd - xStart, yEnd - yStart));
        public void SetCroppedImageRotation(float rotateDegrees)
        {
            CroppedImageRotationDegrees = rotateDegrees;
        }
        public void SetOutputImageRotation(float rotateDegrees)
        {
            OutputImageRotationDegrees = rotateDegrees;
        }

        public void Create()
        {
            CreateOutputImage();
            CreateCroppedImage();
            DrawOutputImage();
        }

        private void DrawOutputImage()
        {

            for (int col = 0; col < numCols; col++)
            {
                for (int row = 0; row < numRows; row++)
                {
                    Point insertPosition = new Point(borderThicknessW + col * (CropArea.Width + borderThicknessW), borderThicknessH + row * (CropArea.Height + borderThicknessH));
                    OutputImage.Mutate(i => i
                    .DrawImage(CroppedImage, 1, insertPosition));
                }
            }
            if (OutputImageRotationDegrees > 0.0)
                OutputImage.Mutate(i => i.Rotate(OutputImageRotationDegrees));
        }

        private void CreateCroppedImage()
        {
            CroppedImage = new Image<Rgba32>(CropArea.Width, CropArea.Height);
            CroppedImage = SourceImage.Clone(
                ctx => ctx.Resize(CropArea.Width, CropArea.Height, new SixLabors.ImageSharp.Processing.Processors.Transforms.BicubicResampler(), CropArea, new Rectangle(0, 0, CroppedImage.Width, CroppedImage.Height), true));
            if (CroppedImageRotationDegrees > 0.0)
                CroppedImage.Mutate(i => i.Rotate(CroppedImageRotationDegrees));
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
            OutputImage = new Image<Rgba32>(OutputImageWidth, OutputImageHeight);
            OutputImage.Mutate(i => i.BackgroundColor(Rgba32.White));
        }

        private void CalcBorderThickness()
        {
            borderThicknessW = (int)((double)(SheetFormat.Width % ImageFormat.Width) / ImageFormat.Width * CropArea.Width / (numCols + 1));
            borderThicknessH = (int)((double)(SheetFormat.Height % ImageFormat.Height) / ImageFormat.Height * CropArea.Height / (numRows + 1));
        }

        static public ImageSheet FromImage(Image<Rgba32> sourceImage)
        {
            return new ImageSheet(sourceImage);
        }

        private ImageSheet(Image<Rgba32> SourceImage)
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
