using System.Collections.Generic;
using System.Linq;

namespace ImageAnalysis.Algorithms
{
    public static class NeighbourhoodConsistent
    {
        public static Dictionary<KeyPoint, KeyPoint> GetConsistentNeighbours(
            Dictionary<KeyPoint, KeyPoint> nearestNeighbours, 
            int cardinalityNeighbour, 
            float minConsistenet)
        {
            var consistentNeighbours = new Dictionary<KeyPoint, KeyPoint>();

            foreach (var nearestNeighbour in nearestNeighbours)
            {
                var firstImageNearestNeighbours = FindNearestNeighbours(nearestNeighbour.Key, nearestNeighbours.Keys.ToList(), cardinalityNeighbour);
                var secondImageNearestNeighbour = FindNearestNeighbours(nearestNeighbour.Value, nearestNeighbours.Values.ToList(), cardinalityNeighbour);
                if((float)(GetNearestNeightboursConsistency(nearestNeighbours, firstImageNearestNeighbours, secondImageNearestNeighbour)/ (float)cardinalityNeighbour) >= minConsistenet)
                {
                    consistentNeighbours.Add(nearestNeighbour.Key, nearestNeighbour.Value);
                }
            }
            return consistentNeighbours;
        }

        private static float GetNearestNeightboursConsistency(Dictionary<KeyPoint, KeyPoint> nearestNeighbours, List<KeyPoint> firstImageNeighbours, List<KeyPoint> secondImageNeighbours)
        {
            float adhesion = 0;
            foreach (var firstImageNeighbour in firstImageNeighbours)
            {
                foreach (var secondImageNeighbour in secondImageNeighbours)
                {
                    if (nearestNeighbours[firstImageNeighbour].Equals(secondImageNeighbour))
                    {
                        adhesion++;
                        break;
                    }
                }
            }
            return adhesion;
        }

        private static List<KeyPoint> FindNearestNeighbours(KeyPoint keyPoint, List<KeyPoint> keyPoints, int cardinalityNeighbour)
        {
            var nearestNeighbours = new List<KeyPoint>();
            for (int i = 0; i < keyPoints.Count; i++)
            {
                if (nearestNeighbours.Count < cardinalityNeighbour)
                {
                    nearestNeighbours.Add(keyPoints[i]);
                }
                else
                {
                        var _distance = keyPoint.GetDistance(keyPoints[i]);
                    for (int j = 0; j < nearestNeighbours.Count; j++)
                    {
                        if (_distance < keyPoint.GetDistance(nearestNeighbours[j]))
                        {
                            nearestNeighbours[j] = keyPoints[i];
                            break;
                        }
                    }
                }
            }
            return nearestNeighbours;
        }
    }
}
