using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visualiser.Models;
using Visualiser.Processing;

namespace Visualiser.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            BeforeInitializeComponent();
            InitializeComponent();
            //AfterInitializeComponent();
            //StaticAfterInitializeComponent();
        }

        private void BeforeInitializeComponent()
        {

        }
        private ECG signal;

        private void StaticAfterInitializeComponent()
        {
            String[] values = File.ReadAllText(@"C:\Users\XZone\Desktop\moar.csv").Replace("\r\n",";").Split(';');
            List<double> points = new List<double>();
            for (int i = 1; i < values.Length; i += 3)
            {
                points.Add(Double.Parse(values[i], CultureInfo.InvariantCulture));
            }

            signal = new ECG();
            signal.SamplingRate = 360;
            signal.Name = "100";
            for (int i = 0; i < points.Count; i++)
            {
                signal.Points.Add(new ECGPoint(){TimeIndex= (double)(i+1)/360, Value = points[i]});
            }
            var result = QRSDetector.QRS_Detect(signal);
            signal.HeartRate = result.Item2;
            signal.Spikes = result.Item1;
            ecgView.ECGSignal = signal;
        }

        private void AfterInitializeComponent()
        {
            
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("Simple utility application for displaying PhysioNet-compliant ECG signals.\nAuthors: Hajdarević Adnan, Hidić Adnan, Zubanović Damir");
        }

        private void InsertAnnotation_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            signal.Annotations.AddRange(new List<ECGAnnotation>(){
                new ECGAnnotation(){
                    Text = "ANSWER",
                    TimeIndex = 0.2,
                    Type = ECGAnnotation.TYPE.ANSWER
                },
                new ECGAnnotation(){
                    Text = "SOLUTION",
                    TimeIndex = 0.2,
                    Type = ECGAnnotation.TYPE.SOLUTION
                },
                new ECGAnnotation(){
                    Text = "STANDARD",
                    TimeIndex = 0.36,
                    Type = ECGAnnotation.TYPE.PHYSIONET_STANDARD
                }
            });

            ecgView.refresh();
        }

        private void EditAnnotations_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteAnnotations_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleAllAnnotations_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSolutionAnnotations_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DetectQRS_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSpikes_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
