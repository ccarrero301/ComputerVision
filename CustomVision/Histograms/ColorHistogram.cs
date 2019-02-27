namespace CustomVision.Histograms
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    internal static class ColorHistogram
    {
        public static double CompareColorHistograms(string sourceImagePath, string optimizedImagePath)
        {
            using (Stream sourceImageStream = File.OpenRead(sourceImagePath))
            using (Stream optimizedImageStream = File.OpenRead(optimizedImagePath))
            {
                var sourceImageBitmap = new Bitmap(sourceImageStream);
                var optimizedImageBitmap = new Bitmap(optimizedImageStream);

                var sourceImageHistogram = GenerateColorHistogram(sourceImageBitmap);

                var optimizedImageHistogram = GenerateColorHistogram(optimizedImageBitmap);

                return (sourceImageHistogram.Select((t, i) => Math.Abs(t - optimizedImageHistogram[i])).Sum()) * 100;
            }
        }

        private static double[] GenerateColorHistogram(Bitmap sourceImage)
        {
            var rgbColor = new double[512];

            var width = sourceImage.Width;
            var height = sourceImage.Height;

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var pixelColor = sourceImage.GetPixel(x, y);

                    var red = pixelColor.R;
                    var green = pixelColor.G;
                    var blue = pixelColor.B;

                    var quantColor = ((red / 32) * 64) + ((green / 32) * 8) + (blue / 32);

                    ++rgbColor[quantColor];
                }
            }

            double normalizationFactor = width * height;

            for (var i = 0; i < rgbColor.Length; i++)
            {
                rgbColor[i] = rgbColor[i] / normalizationFactor;
            }

            return rgbColor;
        }
    }
}