namespace CustomVision
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;

    public class ImgHash
    {
        private readonly int _hashSide;

        public bool[] HashData { get; private set; }
        public string ImgSize { get; private set; }

        public Image Img => Image.FromFile(FilePath);

        public string FilePath { get; private set; }

        public string FileName => Path.GetFileName(FilePath);

        public string FileLocation => Path.GetDirectoryName(FilePath);

        public ImgHash(int hashSideSize = 16)
        {
            _hashSide = hashSideSize;

            HashData = new bool[hashSideSize * hashSideSize];
        }

        /// <summary>
        /// Method to compare 2 image hashes
        /// </summary>
        /// <returns>% of similarity</returns>
        public double CompareWith(ImgHash compareWith)
        {
            if (HashData.Length != compareWith.HashData.Length)
            {
                throw new Exception("Cannot compare hashes with different sizes");
            }

            var differenceCounter = HashData.Where((t, i) => t != compareWith.HashData[i]).Count();

            var result = 100 - differenceCounter / 100.0 * HashData.Length / 2.0;

            return result;
        }

        public void GenerateFromPath(string path)
        {
            FilePath = path;

            var image = (Bitmap) Image.FromFile(path, true);

            ImgSize = $"{image.Size.Width}x{image.Size.Height}";

            GenerateFromImage(image);

            image.Dispose();
        }

        private void GenerateFromImage(Image img)
        {
            var lResult = new List<bool>();

            //resize img to 16x16px (by default) or with configured size 
            var bmpMin = new Bitmap(img, new Size(_hashSide, _hashSide));

            for (var j = 0; j < bmpMin.Height; j++)
            {
                for (var i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true and false
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }

            HashData = lResult.ToArray();

            bmpMin.Dispose();
        }
    }
}