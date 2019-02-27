namespace CustomVision.ComputerVision
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

    internal static class ComputerVisionConsoleHelper
    {
        private static string _accentColor;

        public static void ComputeVision(string originalFileName, string optimizedFileName)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            ComputerVisionConsoleHelper.CalculateComputerVision(originalFileName, optimizedFileName);

            stopWatch.Stop();
            Console.WriteLine($"Execution Time : {stopWatch.Elapsed.TotalSeconds} Seconds");
            Console.WriteLine("****************************************************************************");
        }

        public static void CalculateComputerVision(string sourceImagePath, string optimizedImagePath)
        {
            Console.WriteLine("Images being analyzed ...");
            const string subscriptionKey = "4be5bc2a09a94d46899ad655be147cb5";
            var azureComputerVision = new AzureComputerVision(subscriptionKey);

            Console.WriteLine($"\n****************************************************************************");
            AnalyzeLocalAsync(azureComputerVision, sourceImagePath).Wait();
            var originalAccentColor = _accentColor;
            Console.WriteLine("\n***************************************************************");

            Console.WriteLine("\n***************************************************************");
            AnalyzeLocalAsync(azureComputerVision, optimizedImagePath).Wait();
            var optimizedAccentColor = _accentColor;
            Console.WriteLine("\n***************************************************************");

            Console.WriteLine("\n***************************************************************");
            var colorComparision = CompareHexColors(originalAccentColor, optimizedAccentColor);

            Console.WriteLine($"Original Accent Color : {originalAccentColor}");
            Console.WriteLine($"Optimized Accent Color : {optimizedAccentColor}");
            Console.WriteLine($"Color Comparision : {colorComparision}");
        }

        private static async Task AnalyzeLocalAsync(AzureComputerVision azureComputerVision, string imagePath)
        {
            var analysis = await azureComputerVision.AnalyzeLocalAsync(imagePath).ConfigureAwait(false);

            DisplayResults(analysis, imagePath);
        }

        // Display the most relevant caption for the image
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            Console.WriteLine(imageUri);

            Console.WriteLine("\tColors");
            ProcessColors(analysis);

            Console.WriteLine("\tMetaData");
            ProcessMetaData(analysis);

            Console.WriteLine("\tObjects");
            ProcessObjects(analysis);

            Console.WriteLine("\tCategories");
            ProcessCategories(analysis);

            Console.WriteLine("\tDescriptions");
            ProcessDescriptions(analysis);
        }

        private static void ProcessColors(ImageAnalysis analysis)
        {
            if (analysis.Color == null)
            {
                Console.WriteLine("\t\tNo Detected Colors");
            }

            else if (analysis.Color != null)
            {
                Console.WriteLine($"\t\tAccent Color : {analysis.Color.AccentColor}");
                Console.WriteLine($"\t\tDominant Color Foreground : {analysis.Color.DominantColorForeground}");
                Console.WriteLine($"\t\tDominant Color Background : {analysis.Color.DominantColorBackground}");

                foreach (var dominantColor in analysis.Color.DominantColors)
                {
                    Console.WriteLine($"\t\tDominant Color : {dominantColor}");
                }

                Console.WriteLine($"\t\tIs BWImg : {analysis.Color.IsBWImg}");

                _accentColor = analysis.Color.AccentColor;
            }
        }

        private static void ProcessMetaData(ImageAnalysis analysis)
        {
            Console.WriteLine($"\t\tMetadata Format : {analysis.Metadata.Format}");
            Console.WriteLine($"\t\tMetadata Height : {analysis.Metadata.Height}");
            Console.WriteLine($"\t\tMetadata Width : {analysis.Metadata.Width}");
        }

        private static void ProcessObjects(ImageAnalysis analysis)
        {
            if (analysis.Objects.Count == 0)
            {
                Console.WriteLine("\t\tNo Detected Objects");
            }

            else
            {
                foreach (var detectedObject in analysis.Objects)
                {
                    Console.WriteLine($"\t\tDetected Object Confidence : {detectedObject.Confidence}");
                    Console.WriteLine($"\t\tDetected Object Property : {detectedObject.ObjectProperty}");
                    Console.WriteLine($"\t\tDetected Object Rectangle H : {detectedObject.Rectangle.H}");
                    Console.WriteLine($"\t\tDetected Object Rectangle W : {detectedObject.Rectangle.W}");
                    Console.WriteLine($"\t\tDetected Object Rectangle X : {detectedObject.Rectangle.X}");
                    Console.WriteLine($"\t\tDetected Object Rectangle Y : {detectedObject.Rectangle.Y}");
                }
            }
        }

        private static void ProcessCategories(ImageAnalysis analysis)
        {
            if (analysis.Categories.Count == 0)
            {
                Console.WriteLine("\t\tNo Detected Categories");
            }

            else
            {
                foreach (var category in analysis.Categories)
                {
                    Console.WriteLine($"\t\tCategory Name : {category.Name}");
                    Console.WriteLine($"\t\tCategory Score : {category.Score}");
                }
            }
        }

        private static void ProcessDescriptions(ImageAnalysis analysis)
        {
            if (analysis.Description.Captions.Count == 0)
            {
                Console.WriteLine("\t\tNo Detected Captions");
            }

            else
            {
                foreach (var caption in analysis.Description.Captions)
                {
                    Console.WriteLine($"\t\tCaption Confidence : {caption.Confidence}");
                    Console.WriteLine($"\t\tCaption Text : {caption.Text}");
                }
            }
        }

        private static int CompareHexColors(string firstColor, string secondColor)
        {
            if (firstColor.IndexOf("#", StringComparison.Ordinal) != -1)
                firstColor = firstColor.Replace("#", "");

            var r1 = int.Parse(firstColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g1 = int.Parse(firstColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b1 = int.Parse(firstColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            if (secondColor.IndexOf("#", StringComparison.Ordinal) != -1)
                secondColor = secondColor.Replace("#", "");

            var r2 = int.Parse(secondColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g2 = int.Parse(secondColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b2 = int.Parse(secondColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            var result = ColorFormulas.DoFullCompare(r1, g1, b1, r2, g2, b2);

            return result;
        }
    }
}
