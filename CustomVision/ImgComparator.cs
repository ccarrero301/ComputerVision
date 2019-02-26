namespace CustomVision
{
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    public class ImgComparator
    {
        private readonly int _hashSize;
        private readonly List<ImgHash> _hashLib = new List<ImgHash>();

        public ImgComparator(int hashSize = 16) => _hashSize = hashSize;

        public List<List<ImgHash>> FindDuplicatesWithTolerance(int minSimilarity = 90)
        {
            var alreadyMarkedAsDuplicate = new List<ImgHash>();

            var duplicatesFound = new List<List<ImgHash>>();

            foreach (var hash in _hashLib)
            {
                if (alreadyMarkedAsDuplicate.Contains(hash))
                    continue;

                var singleImgDuplicates = FindDuplicatesTo(hash, minSimilarity, ref alreadyMarkedAsDuplicate);

                duplicatesFound.Add(singleImgDuplicates);
            }

            return duplicatesFound;
        }

        private List<ImgHash> FindDuplicatesTo(ImgHash hash, int minSimilarity, ref List<ImgHash> alreadyMarkedAsDuplicate)
        {
            var currentHashDuplicate = new List<ImgHash>();

            foreach (var hashCompareWith in _hashLib)
            {
                if (!(hash.CompareWith(hashCompareWith) >= minSimilarity))
                    continue;

                if (!alreadyMarkedAsDuplicate.Contains(hash))
                {
                    alreadyMarkedAsDuplicate.Add(hash);

                    currentHashDuplicate.Add(hash);
                }

                if (alreadyMarkedAsDuplicate.Contains(hashCompareWith))
                    continue;

                alreadyMarkedAsDuplicate.Add(hashCompareWith);

                currentHashDuplicate.Add(hashCompareWith);
            }

            return currentHashDuplicate;
        }

        public void AddPicByPath(string path)
        {
            var hash = new ImgHash(_hashSize);
            hash.GenerateFromPath(path);

            _hashLib.Add(hash);
        }
    }
}