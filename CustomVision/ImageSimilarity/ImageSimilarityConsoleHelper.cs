namespace CustomVision.ImageSimilarity
{
    using System;
    using System.Diagnostics;

    internal static class ImageSimilarityConsoleHelper
    {
        public static void CompareImages(string originalFileName, string optimizedFileName)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var s1Result = ImageSimilarity.AreImagesSimilar(originalFileName, optimizedFileName);

            Console.WriteLine($"\n****************************************************************************");
            Console.WriteLine($"Images Similarity with 95% Threshold: {s1Result}");

            stopWatch.Stop();
            Console.WriteLine($"Execution Time: {stopWatch.Elapsed.TotalSeconds} Seconds");
            Console.WriteLine("****************************************************************************");
        }
    }
}
