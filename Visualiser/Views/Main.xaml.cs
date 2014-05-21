using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
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
            AfterInitializeComponent();
        }

        private void BeforeInitializeComponent()
        {

        }
        private ECG signal;

        private void AfterInitializeComponent()
        {
            signal = new ECG();
            signal.SamplingRate = 360;
            signal.HeartRate = 70;
            signal.Name = "100";
            signal.Points.AddRange(
                new List<ECGPoint>()
                {
                    new ECGPoint(){
                        TimeIndex = 0.04,
                        Value = 0.5
                    },
                    new ECGPoint(){
                        TimeIndex = 0.12,
                        Value = 0.8
                    },
                    new ECGPoint(){
                        TimeIndex = 0.2,
                        Value = 0.6
                    },
                    new ECGPoint(){
                        TimeIndex = 0.28,
                        Value = 1.5
                    },
                    new ECGPoint(){
                        TimeIndex = 0.36,
                        Value = 2.5
                    },
                }
            );
            ecgView.ECGSignal = signal;
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
    }
}
