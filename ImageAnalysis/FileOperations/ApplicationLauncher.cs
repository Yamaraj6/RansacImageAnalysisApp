using System;
using System.Diagnostics;

namespace ImageAnalysis.FileOperations
{
    public static class ApplicationLauncher
    {
        public static void LaunchCreateKeyPointsApp(string picturePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "KeyPointsApp\\extract_features_32bit.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "-haraff -sift -i " + picturePath + " -DE";
            
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }
    }
}
