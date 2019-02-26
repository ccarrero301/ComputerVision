namespace CustomVision
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using AForge.Imaging;

    internal static class ImageSimilarity
    {
        public static bool Compare(this Bitmap image1, Bitmap image2, double threshold)
        {
            return new ExhaustiveTemplateMatching(0)
                       .ProcessImage(image1.To24bppRgbFormat(), image2.To24bppRgbFormat())[0]
                       .Similarity >= threshold;
        }

        public static Bitmap To24bppRgbFormat(this Bitmap img)
        {
            return img.Clone(new Rectangle(0, 0, img.Width, img.Height),
                PixelFormat.Format24bppRgb);
        }

        public static bool AreImagesSimilar(string sourceImagePath, string optimizedImagePath)
        {
            using (Stream sourceImageStream = File.OpenRead(sourceImagePath))
            using (Stream optimizedImageStream = File.OpenRead(optimizedImagePath))
            {
                var sourceImageBitmap = new Bitmap(sourceImageStream);
                var optimizedImageBitmap = new Bitmap(optimizedImageStream);

                return optimizedImageBitmap.Compare(sourceImageBitmap, 0.95f);
            }
        }
    }
}
