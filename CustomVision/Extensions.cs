using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using AForge;
using AForge.Imaging;

namespace CustomVision
{
    public static class Extensions
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
    }
}
