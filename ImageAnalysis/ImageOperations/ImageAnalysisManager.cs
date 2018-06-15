using Emgu.CV;
using Emgu.CV.Structure;
using ImageAnalysis.Algorithms;
using ImageAnalysis.FileOperations;
using ImageAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImageAnalysis.ImageOperations
{
    public class ImageAnalysisManager
    {
        public AnalizedImage[] Images { get; private set; }
        public Dictionary<KeyPoint, KeyPoint> NeighbourPairs { get; private set; }
        public Dictionary<KeyPoint, KeyPoint> ConsistencyNeighbourPairs { get; private set; }
        public Dictionary<KeyPoint, KeyPoint> RansacFilterNeighbourPairs { get; private set; }
        public KeyPairsTriangle RansacTriangle { get; private set; }

        public ImageAnalysisManager(string imagePath1, string imagePath2)
        {
            Images = new AnalizedImage[2];
            Images[0] = CreateAnalyseImage(imagePath1);
            Images[1] = CreateAnalyseImage(imagePath2);
        }

        private AnalizedImage CreateAnalyseImage(string picturePath)
        {
            string imagePath = picturePath;
            string pointsPath = imagePath + ".haraff.sift";

            if (!File.Exists(imagePath + ".haraff.sift"))
            {
                ApplicationLauncher.LaunchCreateKeyPointsApp(imagePath);
            }
            var _keyPoints = KeyPointsReader.GetKeyPointsList(pointsPath);

            return new AnalizedImage(imagePath, _keyPoints);
        }

        public void UseNeighbourhoodAlgorithm()
        {
            NeighbourPairs = Neighbourhood.GetNearestNeighbours(Images);
        }

        public void UseConsistencyAlgorithm(int cardinalityNeighbour=30, float minConsistenet=0.35f)
        {
            ConsistencyNeighbourPairs = NeighbourhoodConsistent.GetConsistentNeighbours(NeighbourPairs, cardinalityNeighbour, minConsistenet);
        }

        public void UseRansacAlgorithm(int iterationsCount = 1000, float r = 5, float R = 700, bool secondError= true)
        {
            var min = GetMinSide();
            r = min * 0.05f;
            R = min * 0.3f;
            RansacTriangle = Ransac.RansacAlgorithm(ConsistencyNeighbourPairs, iterationsCount, r, R, secondError);
            RansacFilterNeighbourPairs = Ransac.RansacFilterKeyPoint(ConsistencyNeighbourPairs, RansacTriangle, R);
        }

        public float GetMinSide()
        {
            float minSide;
            using (var secondImage = new Image<Bgr, byte>(Images[1].ImagePath))
            {
                var firstImage = new Image<Bgr, byte>(Images[0].ImagePath);

                minSide = Math.Min(firstImage.Width, firstImage.Height);
                minSide = Math.Min(minSide, secondImage.Height);
                minSide = Math.Min(secondImage.Width, minSide);
            }
            return minSide;
        }
        
        public BitmapSource GetMargedBitmap()
        {
            BitmapSource marged;
            using (var combined = CreateMargedImage(new Image<Bgr, byte>(Images[0].ImagePath), new Image<Bgr, byte>(Images[1].ImagePath), 
                RansacFilterNeighbourPairs,
                RansacTriangle))
            {
                marged = combined.ToBitmapSource();
            }
            return marged;
        }

        private Image<Bgr, byte> CreateMargedImage(
            Image<Bgr, byte> firstImage, Image<Bgr, byte> secondImage,
            Dictionary<KeyPoint, KeyPoint> pairs, KeyPairsTriangle ransacTriangle)
        {

            // Marge image.
            var margedImageWidth = firstImage.Width + secondImage.Width;
            var margedImageHeight = Math.Max(firstImage.Height, secondImage.Height);

            var imageResult = new Image<Bgr, byte>(margedImageWidth, margedImageHeight)
            {
                ROI = new Rectangle(0, 0, firstImage.Width, firstImage.Height)
            };
            firstImage.CopyTo(imageResult);

            imageResult.ROI = new Rectangle(firstImage.Width, 0, secondImage.Width, secondImage.Height);
            secondImage.CopyTo(imageResult);

            imageResult.ROI = Rectangle.Empty;

            // Use Ransac.
            if (ransacTriangle != null)
            {
                var firstTriangle = ransacTriangle.GetFirstTriangle();
                var secondTriangle = ransacTriangle.GetSecondTriangle();

                var triangleColor = new Bgr(Color.Black).MCvScalar;

                CvInvoke.Line(imageResult, firstTriangle[0], firstTriangle[1], triangleColor, 2);
                CvInvoke.Line(imageResult, firstTriangle[1], firstTriangle[2], triangleColor, 2);
                CvInvoke.Line(imageResult, firstTriangle[2], firstTriangle[0], triangleColor, 2);
                CvInvoke.Line(imageResult, secondTriangle[0].ChangeOffset(firstImage.Width),
                    secondTriangle[1].ChangeOffset(firstImage.Width), triangleColor, 2);
                CvInvoke.Line(imageResult, secondTriangle[1].ChangeOffset(firstImage.Width),
                    secondTriangle[2].ChangeOffset(firstImage.Width), triangleColor, 2);
                CvInvoke.Line(imageResult, secondTriangle[2].ChangeOffset(firstImage.Width),
                    secondTriangle[0].ChangeOffset(firstImage.Width), triangleColor, 2);
            }

            // Draw lines.
            foreach (var _pair in pairs)
            {
                var start = new Point((int)_pair.Key.X, (int)_pair.Key.Y);
                var second = new Point((int)_pair.Value.X + firstImage.Width, (int)_pair.Value.Y);

                CvInvoke.Line(imageResult, start, second, new Bgr(_pair.Key.Color).MCvScalar, 1);

            }
            return imageResult;
        }
    }
}