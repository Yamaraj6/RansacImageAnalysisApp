using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ImageAnalysis.Models
{
    public class AnalizedImage
    {
        private readonly Lazy<BitmapSource> imageBitmapSourceLazy;
        public List<KeyPoint> KeyPoints { get; private set; }
        public string ImagePath{ get; }

        public AnalizedImage(string imagePath, List<KeyPoint> keyPoints)
        {
            this.imageBitmapSourceLazy =  new Lazy<BitmapSource>(() => new BitmapImage(new Uri(imagePath)));
            this.ImagePath = imagePath;
            this.KeyPoints = keyPoints;
        }
    }
}
