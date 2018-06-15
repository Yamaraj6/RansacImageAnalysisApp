using ImageAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageAnalysis.Algorithms
{
    public static class Neighbourhood
    {
        public static Dictionary<KeyPoint, KeyPoint> GetNearestNeighbours(AnalizedImage[] analizedImages)
        {
            return FindAllNearestNeighbours(analizedImages);
        }
        
        private static Dictionary<KeyPoint, KeyPoint> FindAllNearestNeighbours(AnalizedImage[] analizedImages)
        {
            var keyPointsPairs = new Dictionary<KeyPoint, KeyPoint>();
            Parallel.ForEach(analizedImages[0].KeyPoints, point =>
            {
                var nearestNeighbour = FindNearestNeighbour(point, analizedImages[1].KeyPoints);
                var nearestNeighbourForCheck = FindNearestNeighbour(nearestNeighbour, analizedImages[0].KeyPoints);
                if (point.Equals(nearestNeighbourForCheck))
                {
                    keyPointsPairs.Add(point, nearestNeighbour);
                }
            });
            return keyPointsPairs;
        }

        public static KeyPoint FindNearestNeighbour(KeyPoint point, List<KeyPoint> neighbours)
        {
            var distances = new Dictionary<KeyPoint, float>();
            foreach(var neighbour in neighbours)
            {
                distances.Add(neighbour, GetDistance(point, neighbour));
            }
            KeyValuePair<KeyPoint, float> min = new KeyValuePair<KeyPoint, float>(null, 0);
            foreach (var distance in distances)
            {
                if (min.Key == null || distance.Value < min.Value)
                {
                    min = distance;
                }
            }

            return min.Key;
        }

        private static float GetDistance(KeyPoint point1, KeyPoint point2)
        {
            float distance = 0f;
            for (int i = 0; i < point1.Features.Count; i++)
            {
                distance += Math.Abs(point1.Features[i] - point2.Features[i]);
            }
            return distance;
        }
    }
}