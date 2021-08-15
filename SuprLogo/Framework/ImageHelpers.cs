using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace SuprLogo.Framework
{
    public class ImageHelpers
    {
        public static ImageSource AutoCropBitmap(BitmapSource source, Color backgroundColor)
        {
            if (source == null)
                throw new ArgumentException("source");

            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source,
                                    PixelFormats.Bgra32, null, 0);

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int bytesPerPixel = source.Format.BitsPerPixel / 8;
            int stride = width * bytesPerPixel;

            var pixelBuffer = new byte[height * stride];
            source.CopyPixels(pixelBuffer, stride, 0);

            int cropTop = height, cropBottom = 0, cropLeft = width, cropRight = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = (y * stride + x * bytesPerPixel);
                    byte blue = pixelBuffer[offset];
                    byte green = pixelBuffer[offset + 1];
                    byte red = pixelBuffer[offset + 2];
                    byte alpha = pixelBuffer[offset + 3];

                    //TODO: Define a threshold when a pixel has a content
                    bool hasContent = alpha > 0 && Color.FromArgb(alpha, red, green, blue) != backgroundColor;

                    if (hasContent)
                    {
                        cropLeft = Math.Min(x, cropLeft);
                        cropRight = Math.Max(x, cropRight);
                        cropTop = Math.Min(y, cropTop);
                        cropBottom = Math.Max(y, cropBottom);
                    }
                }
            }

            return new CroppedBitmap(source,
                     new Int32Rect(cropLeft, cropTop, cropRight - cropLeft,
                                   cropBottom - cropTop));
        }

    }
}
