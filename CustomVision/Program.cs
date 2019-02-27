namespace CustomVision
{
    using System;
    using Histograms;
    using ImageSimilarity;
    using ComputerVision;
    
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string originalsPath = @"C:\Users\ccarrero.INTERNAL\Desktop\Originals\";
            const string optimizedPath = @"C:\Users\ccarrero.INTERNAL\Desktop\Optimized\";

            var originalFileName = $"{originalsPath}S3.png";
            var optimizedFileName = $"{optimizedPath}S3.png";

            ColorHistogramConsoleHelper.CompareColorHistograms(originalFileName, optimizedFileName);

            ImageSimilarityConsoleHelper.CompareImages(originalFileName, optimizedFileName);

            ComputerVisionConsoleHelper.ComputeVision(originalFileName, optimizedFileName);

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }
    }
}