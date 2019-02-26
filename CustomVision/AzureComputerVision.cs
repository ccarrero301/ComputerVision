namespace CustomVision
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

    internal class AzureComputerVision
    {
        // Specify the features to return
        private readonly List<VisualFeatureTypes> _features;
        private readonly ComputerVisionClient _computerVision;

        public AzureComputerVision(string SubscriptionKey)
        {
            _computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(SubscriptionKey))
            {
                Endpoint = "https://eastus.api.cognitive.microsoft.com"
            };

            _features = new List<VisualFeatureTypes>()
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
        }

        // Analyze a local image
        public async Task<ImageAnalysis> AnalyzeLocalAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
                throw new Exception($"Unable to open or read localImagePath: {imagePath}");

            using (var imageStream = File.OpenRead(imagePath))
                return await _computerVision.AnalyzeImageInStreamAsync(imageStream, _features).ConfigureAwait(false);
        }

        // Analyze a remote image
        public Task<ImageAnalysis> AnalyzeRemoteAsync(IComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                throw new Exception($"Invalid Remote Image Url: {imageUrl}");

            return _computerVision.AnalyzeImageAsync(imageUrl, _features);
        }
    }
}