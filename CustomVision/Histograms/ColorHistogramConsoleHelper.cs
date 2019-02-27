namespace CustomVision.Histograms
{
    using System;
    using System.Diagnostics;

    internal class ColorHistogramConsoleHelper
    {
        public static void CompareColorHistograms(string originalFileName, string optimizedFileName)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var histogramDistanceS1 = ColorHistogram.CompareColorHistograms(originalFileName, optimizedFileName);

            Console.WriteLine($"\n****************************************************************************");
            Console.WriteLine($"Histogram Distance : {histogramDistanceS1}%");

            stopWatch.Stop();
            Console.WriteLine($"Execution Time : {stopWatch.Elapsed.TotalSeconds} Seconds");
            Console.WriteLine("****************************************************************************");
        }
    }
}
