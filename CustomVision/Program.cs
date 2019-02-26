namespace CustomVision
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Drawing;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

    internal class Program
    {
        private const string SubscriptionKey = "4be5bc2a09a94d46899ad655be147cb5";

        private const string RemoteImageUrl = "http://upload.wikimedia.org/wikipedia/commons/3/3c/Shaki_waterfall.jpg";

        private static string _accentColor;

        private const string CheetahAssetsOriginalsPath = @"C:\Users\ccarrero.INTERNAL\Desktop\Originals\";
        private const string CheetahAssetsOptimizedPath = @"C:\Users\ccarrero.INTERNAL\Desktop\Optimized\";

        // Specify the features to return
        private static readonly List<VisualFeatureTypes> Features =
            new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Adult,
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Color,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces,
                VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Objects,
                VisualFeatureTypes.Tags
            };

        private static void Main(string[] args)
        {
            ColorHistogramComparision();

            ImagesSimilarity();

            ComputeVision();

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static void ColorHistogramComparision()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var histogramDistanceS1 = CompareColorHistograms($"{CheetahAssetsOriginalsPath}S14.png", $"{CheetahAssetsOptimizedPath}S14.png");

            Console.WriteLine($"\n****************************************************************************");
            Console.WriteLine($"Histogram Distance : {histogramDistanceS1}%");

            stopWatch.Stop();
            Console.WriteLine($"Execution Time : {stopWatch.Elapsed.TotalSeconds} Seconds");
            Console.WriteLine("****************************************************************************");
        }

        private static double CompareColorHistograms(string sourceImagePath, string optimizedImagePath)
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

        private static void ImagesSimilarity()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var s1Result = AreImagesSimilar($"{CheetahAssetsOriginalsPath}S14.png", $"{CheetahAssetsOptimizedPath}S14.png");

            Console.WriteLine($"\n****************************************************************************");
            Console.WriteLine($"Images Similarity with 95% Threshold: {s1Result}");

            stopWatch.Stop();
            Console.WriteLine($"Execution Time: {stopWatch.Elapsed.TotalSeconds} Seconds");
            Console.WriteLine("****************************************************************************");
        }

        private static bool AreImagesSimilar(string sourceImagePath, string optimizedImagePath)
        {
            using (Stream sourceImageStream = File.OpenRead(sourceImagePath))
            using (Stream optimizedImageStream = File.OpenRead(optimizedImagePath))
            {
                var sourceImageBitmap = new Bitmap(sourceImageStream);
                var optimizedImageBitmap = new Bitmap(optimizedImageStream);

                return optimizedImageBitmap.Compare(sourceImageBitmap, 0.95f);
            }
        }

        private static void ComputeVision()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            CalculateComputerVision($"{CheetahAssetsOriginalsPath}S14.png", $"{CheetahAssetsOptimizedPath}S14.png");

            stopWatch.Stop();
            Console.WriteLine($"Execution Time : {stopWatch.Elapsed.TotalSeconds} Seconds");
            Console.WriteLine("****************************************************************************");
        }

        private static void CalculateComputerVision(string sourceImagePath, string optimizedImagePath)
        {
            var computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(SubscriptionKey))
            {
                Endpoint = "https://eastus.api.cognitive.microsoft.com"
            };

            //Console.WriteLine("Images being analyzed ...");

            Console.WriteLine($"\n****************************************************************************");
            AnalyzeLocalAsync(computerVision, sourceImagePath).Wait();
            var originalAccentColor = _accentColor;
            //Console.WriteLine("\n***************************************************************");

            //Console.WriteLine("\n***************************************************************");
            AnalyzeLocalAsync(computerVision, optimizedImagePath).Wait();
            var optimizedAccentColor = _accentColor;
            //Console.WriteLine("\n***************************************************************");

            //Console.WriteLine("\n***************************************************************");
            var colorComparision = CompareHexColors(originalAccentColor, optimizedAccentColor);

            Console.WriteLine($"Original Accent Color : {originalAccentColor}");
            Console.WriteLine($"Optimized Accent Color : {optimizedAccentColor}");
            Console.WriteLine($"Color Comparision : {colorComparision}");
        }

        private static float GetImageDifference(string sourceImagePath, string optimizedImagePath)
        {
            using (Stream sourceImageStream = File.OpenRead(sourceImagePath))
            using (Stream optimizedImageStream = File.OpenRead(optimizedImagePath))
            {
                var sourceImageBitmap = new Bitmap(sourceImageStream);
                var optimizedImageBitmap = new Bitmap(optimizedImageStream);

                float diff = 0;

                for (var y = 0; y < sourceImageBitmap.Height; y++)
                {
                    for (var x = 0; x < sourceImageBitmap.Width; x++)
                    {
                        var pixel1 = sourceImageBitmap.GetPixel(x, y);
                        var pixel2 = optimizedImageBitmap.GetPixel(x, y);

                        diff += Math.Abs(pixel1.R - pixel2.R);
                        diff += Math.Abs(pixel1.G - pixel2.G);
                        diff += Math.Abs(pixel1.B - pixel2.B);
                    }
                }

                return 100 * (diff / 255) / (sourceImageBitmap.Width * sourceImageBitmap.Height * 3);
            }
        }

        // Analyze a remote image
        private static async Task AnalyzeRemoteAsync(IComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            var analysis = await computerVision.AnalyzeImageAsync(imageUrl, Features).ConfigureAwait(false);

            DisplayResults(analysis, imageUrl);
        }

        // Analyze a local image
        private static async Task AnalyzeLocalAsync(IComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                var analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, Features).ConfigureAwait(false);

                DisplayResults(analysis, imagePath);
            }
        }

        // Display the most relevant caption for the image
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            //Console.WriteLine(imageUri);

            //Console.WriteLine("\tColors");
            ProcessColors(analysis);

            //Console.WriteLine("\tMetaData");
            //ProcessMetaData(analysis);

            //Console.WriteLine("\tObjects");
            //ProcessObjects(analysis);

            //Console.WriteLine("\tCategories");
            //ProcessCategories(analysis);

            //Console.WriteLine("\tDescriptions");
            //ProcessDescriptions(analysis);
        }

        private static void ProcessColors(ImageAnalysis analysis)
        {
            if (analysis.Color == null)
            {
                Console.WriteLine("\t\tNo Detected Colors");
            }

            else if (analysis.Color != null)
            {
                //Console.WriteLine($"\t\tAccent Color : {analysis.Color.AccentColor}");
                //Console.WriteLine($"\t\tDominant Color Foreground : {analysis.Color.DominantColorForeground}");
                //Console.WriteLine($"\t\tDominant Color Background : {analysis.Color.DominantColorBackground}");

                //foreach (var dominantColor in analysis.Color.DominantColors)
                //{
                //    Console.WriteLine($"\t\tDominant Color : {dominantColor}");
                //}

                //Console.WriteLine($"\t\tIs BWImg : {analysis.Color.IsBWImg}");

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