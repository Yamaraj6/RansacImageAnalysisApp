using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageAnalysis
{
    public class KeyPoint
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public List<int> Features { get; private set; }
        public Color Color { get; private set; }

        public KeyPoint(float x, float y)
        {
            this.X = x;
            this.Y = y;
            Features = new List<int>();
        }

        public KeyPoint(float x, float y, List<int> features)
        {
            this.X = x;
            this.Y = y;
            this.Features = new List<int>(features);
        }

        public void AddFeature(int feature)
        {
            Features.Add(feature);
            ChooseColor();
        }

        private void ChooseColor()
        {
            var _average = (int)Features.Average();
            switch (_average)
            {
                case int n when (n >= 35):
                    Color = Color.Red;
                    break;
                case int n when (n >= 32):
                    Color = Color.Orange;
                    break;
                case int n when (n >= 29):
                    Color = Color.Yellow;
                    break;
                case int n when (n >= 26):
                    Color = Color.Green;
                    break;
                case int n when (n >= 23):
                    Color = Color.SeaGreen;
                    break;
                case int n when (n >= 20):
                    Color = Color.Blue;
                    break;
                case int n when (n >= 17):
                    Color = Color.Purple;
                    break;
                case int n when (n >= 14):
                    Color = Color.Pink;
                    break;
                default:
                    Color = Color.Black;
                    break;
            }
        }

        public override string ToString()
        {
            var features = "";
            foreach (var feature in Features)
            { features += feature + " "; }
            return (int)X + ";" + (int)Y + " " + features;
        }

        internal void AddFeatures(List<int> features)
        {
            this.Features = features;
            ChooseColor();
        }

        public float GetDistance(KeyPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2));
        }

        public Matrix<double> GetPointMatrix()
        {
            return new Matrix<double>(new double[] { X, Y });
        }
    }
}