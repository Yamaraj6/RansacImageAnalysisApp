using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using ImageAnalysis.ImageOperations;

namespace ImageAnalysis.Algorithms
{
    public static class Ransac
    {
        public static KeyPairsTriangle RansacAlgorithm(
            Dictionary<KeyPoint, KeyPoint> pointPairs,
            int iterationsCount = 100,
            float r = 10,
            float R = 180,
            bool countSecondError = false)
        {
            KeyPairsTriangle bestTriangle = null;
            Mat bestCurrentTransform = null;
            var minError = float.MaxValue;
            var minSecondError = float.MaxValue;

            for (int i = 0; i < iterationsCount; i++)
            {
                var triangle = ChooseTriangle(pointPairs, r, R);
                if (triangle == null)
                {
                    continue;
                }

                // For print image
                var _currentTransform = CvInvoke.GetAffineTransform(triangle.GetFirstImageTriangle(), triangle.GetSecondImageTriangle());
                if (_currentTransform.IsEmpty)
                {
                    continue;
                }

                var error = CalculateError(_currentTransform, pointPairs);
                if (error <= minError)
                {
                    if (countSecondError)
                    {
                        var secondError = CalculateSecondError(triangle);
                        if (secondError <= minSecondError)
                        {
                            bestCurrentTransform = _currentTransform;
                            minError = error;
                            bestTriangle = triangle;
                        }
                    }
                    else
                    {
                        bestCurrentTransform = _currentTransform;
                        minError = error;
                        bestTriangle = triangle;
                    }
                }
            }
            return bestTriangle;
        }

        private static float CalculateSecondError(KeyPairsTriangle triangle)
        {         
            var dist1 = triangle.FirstPair.Key.GetDistance(triangle.SecondPair.Key);
            var dist2 = triangle.SecondPair.Key.GetDistance(triangle.ThirdPair.Key);
            var dist3 = triangle.ThirdPair.Key.GetDistance(triangle.FirstPair.Key);
            return Math.Abs(dist1 - dist2) + Math.Abs(dist1 - dist3);
        }

        public static Dictionary<KeyPoint, KeyPoint> RansacFilterKeyPoint(Dictionary<KeyPoint, KeyPoint> pointPairs, KeyPairsTriangle traingle, float R)
        {
            var ransacKeyPoints = new Dictionary<KeyPoint, KeyPoint>();

            foreach(var _keyPoint in pointPairs)
            {
                int i = 0;
                foreach (var _pairs in traingle.GetTrianglePairs())
                {
                    var _dist1 = _keyPoint.Key.GetDistance(_pairs.Key);
                    var _dist2 = _keyPoint.Value.GetDistance(_pairs.Value);
                    if (_dist1 < R && _dist2 < R)
                    {
                        i++;
                    }
                }
                if (i == 3)
                {
                    ransacKeyPoints.Add(_keyPoint.Key, _keyPoint.Value);
                }
            }

            return ransacKeyPoints;
        }
        
        private static float CalculateError(Mat _affineTransform, Dictionary<KeyPoint, KeyPoint> pointPairs)
        {
            var error = 0f;

            foreach (var pair in pointPairs)
            {
                var pointBMatrix = pair.Value.GetPointMatrix();
                var transformedPointBMatrix = new Matrix<int>(2, 1);
                CvInvoke.WarpAffine(pointBMatrix, transformedPointBMatrix, _affineTransform, new System.Drawing.Size(1, 1));
                var transformedPointB = new KeyPoint(transformedPointBMatrix[0, 0], transformedPointBMatrix[1, 0], pair.Value.Features);
                error += pair.Key.GetDistance(transformedPointB);
            }
            return error;
        }

        private static KeyPairsTriangle ChooseTriangle(Dictionary<KeyPoint, KeyPoint> pointPairs, float r, float R)
        {
            Random rand = new Random();
            var _randomNumber = rand.Next(pointPairs.Count);

            var firstPair = new KeyValuePair<KeyPoint, KeyPoint>(
                pointPairs.Keys.ToList()[_randomNumber],
                pointPairs.Values.ToList()[_randomNumber]);


            var closePoints = FilterFarPoints(pointPairs, r, R, firstPair);
            if (!closePoints.Any())
            {
                return null;
            }
            _randomNumber = rand.Next(closePoints.Count);
            var secondPair = new KeyValuePair<KeyPoint, KeyPoint>(
                closePoints.Keys.ToList()[_randomNumber],
                closePoints.Values.ToList()[_randomNumber]);


            closePoints = FilterFarPoints(pointPairs, r, R, firstPair, secondPair);
            if (!closePoints.Any())
            {
                return null;
            }

            _randomNumber = rand.Next(closePoints.Count);
            var thirdPair = new KeyValuePair<KeyPoint, KeyPoint>(
                closePoints.Keys.ToList()[_randomNumber],
                closePoints.Values.ToList()[_randomNumber]);
            return new KeyPairsTriangle(firstPair, secondPair, thirdPair);
        }

        private static Dictionary<KeyPoint, KeyPoint> FilterFarPoints(Dictionary<KeyPoint, KeyPoint> pointPairs,
            float r, float R, params KeyValuePair<KeyPoint, KeyPoint>[] previousPairs)
        {
            return pointPairs.Where(pair => previousPairs.All(prevPair => IsClosePair(prevPair, pair, r, R)))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static bool IsClosePair(KeyValuePair<KeyPoint, KeyPoint> originPair,
            KeyValuePair<KeyPoint, KeyPoint> destPair, float r, float R)
        {
            var firstDistance = originPair.Key.GetDistance(destPair.Key);
            var secondDistance = originPair.Value.GetDistance(destPair.Value);
            return r < firstDistance && firstDistance < R &&
                   r < secondDistance && secondDistance < R;
        }
    }
}