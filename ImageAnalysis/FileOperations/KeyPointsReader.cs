using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ImageAnalysis.FileOperations
{
    public static class KeyPointsReader
    {
        private static string LoadFile(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    String fileText = sr.ReadToEnd();
                    return fileText;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;                   
            }
        }
        
        public static List<KeyPoint> GetKeyPointsList(string pointsPath)
        {
            string pointsData = LoadFile(pointsPath);

            var list = new List<KeyPoint>();

            int counter = 0;
            var tempX = "";
            var tempY = "";
            var feature = "";

            for (int i = 0; i < 2; i++)
            {
                while (pointsData[counter] != '\n')
                {
                    counter++;
                }
                counter++;
            }

            while (pointsData.Length > counter)
            {
                tempX = "";
                tempY = "";

                // read X
                while (Char.IsNumber(pointsData[counter]) || pointsData[counter] == '.')
                {
                    if (pointsData[counter] == '.')
                    {
                        tempX += ',';
                    }
                    else
                    {
                        tempX += pointsData[counter];
                    }
                    counter++;
                }

                counter++;
                // read Y
                while (Char.IsNumber(pointsData[counter]) || pointsData[counter] == '.')
                {
                    if (pointsData[counter] == '.')
                    {
                        tempY += ',';
                    }
                    else
                    {
                        tempY += pointsData[counter];
                    }
                    counter++;
                }
                list.Add(new KeyPoint((float)Convert.ToDouble(tempX), (float)Convert.ToDouble(tempY)));

                // skip elipse parameters
                for (int i = 0; i <= 3; i++)
                {
                    while (pointsData[counter] != ' ')
                    {
                        counter++;
                    }
                    counter++;
                }

                // add features
                var features = new List<int>();
                while (pointsData[counter] != '\n')
                {
                    feature = "";
                    if (pointsData[counter] == ' ')
                    {
                        counter++;
                    }
                    while (Char.IsNumber(pointsData[counter]))
                    {
                        feature += pointsData[counter];
                        counter++;
                    }
                    features.Add(Convert.ToInt32(feature));
                }
                list.Last().AddFeatures(features);
                counter++;
            }
            return list;
        }
    }
}