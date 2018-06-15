using System.Collections.Generic;
using System.Drawing;

namespace ImageAnalysis.ImageOperations
{
    public class KeyPairsTriangle
    {
        public KeyValuePair<KeyPoint, KeyPoint> FirstPair { get; }
        public KeyValuePair<KeyPoint, KeyPoint> SecondPair { get; }
        public KeyValuePair<KeyPoint, KeyPoint> ThirdPair { get; }

        public KeyPairsTriangle(
            KeyValuePair<KeyPoint, KeyPoint> firstPair,
            KeyValuePair<KeyPoint, KeyPoint> secondPair,
            KeyValuePair<KeyPoint, KeyPoint> thirdPair)
        {
            FirstPair = firstPair;
            SecondPair = secondPair;
            ThirdPair = thirdPair;
        }

        public PointF[] GetFirstImageTriangle() => new PointF[]
        {
            new PointF(FirstPair.Key.X, FirstPair.Key.Y),
            new PointF(SecondPair.Key.X, SecondPair.Key.Y),
            new PointF(ThirdPair.Key.X, ThirdPair.Key.Y),
        };

        public PointF[] GetSecondImageTriangle() => new PointF[]
        {
            new PointF(FirstPair.Value.X, FirstPair.Value.Y),
            new PointF(SecondPair.Value.X, SecondPair.Value.Y),
            new PointF(ThirdPair.Value.X, ThirdPair.Value.Y),
        };

        public Point[] GetFirstTriangle() => new Point[]
        {
            new Point((int)FirstPair.Key.X, (int)FirstPair.Key.Y),
            new Point((int)SecondPair.Key.X, (int)SecondPair.Key.Y),
            new Point((int)ThirdPair.Key.X, (int)ThirdPair.Key.Y),
        };

        public Point[] GetSecondTriangle() => new Point[]
        {
            new Point((int)FirstPair.Value.X, (int)FirstPair.Value.Y),
            new Point((int)SecondPair.Value.X, (int)SecondPair.Value.Y),
            new Point((int)ThirdPair.Value.X, (int)ThirdPair.Value.Y),
        };

        public KeyValuePair<KeyPoint, KeyPoint>[] GetTrianglePairs()
        {
            KeyValuePair<KeyPoint, KeyPoint>[] trianglePairs = { FirstPair, SecondPair, ThirdPair };
            return trianglePairs;
        }

        public override string ToString()
        {
            return FirstPair.ToString() + " ; " + SecondPair.ToString() + " ; " + ThirdPair.ToString();
        }
    }
}