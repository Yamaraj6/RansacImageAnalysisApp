using ImageAnalysis.ImageOperations;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageAnalysisGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            ImageAnalysisManager _image = new ImageAnalysisManager(textBox1.Text, textBox2.Text);

            var task = Task.Run(() =>
            {
                _image.UseNeighbourhoodAlgorithm();
                _image.UseConsistencyAlgorithm(10, 0.5f);
                _image.UseRansacAlgorithm();
            });
            await task;
            imgInput.Source = _image.GetMargedBitmap();
            ((Button)sender).IsEnabled = true;
        }


        private void ButtonFilePick_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;

                if (((Button)sender).Name == "btn_file_pick")
                {
                    textBox1.Text = filename;
                }
                else
                {
                    textBox2.Text = filename;
                }
            }
        }
    }
}