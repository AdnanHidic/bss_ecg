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
using Visualiser.IO;
using Visualiser.IO.Exceptions;
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
            AfterInitializeComponent();
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
            if (signal == null)
                return;

            AnnotationInsert annotationInsertWindow = new AnnotationInsert();
            annotationInsertWindow.annotationInsertRequested += annotationInsertWindow_annotationInsertRequested;
            annotationInsertWindow.Show();            
        }

        void annotationInsertWindow_annotationInsertRequested(ECGAnnotation annotation)
        {
            signal.Annotations.Add(annotation);
            ecgView.refresh();
        }

        private void DeleteAnnotation_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (signal == null)
                return;

            AnnotationDelete annotationDeleteWindow = new AnnotationDelete(signal.Annotations);
            annotationDeleteWindow.annotationDeletionRequested += annotationDeleteWindow_annotationDeletionRequested;
            annotationDeleteWindow.Show();
        }

        void annotationDeleteWindow_annotationDeletionRequested(ECGAnnotation annotation)
        {
            signal.Annotations.Remove(annotation);
            ecgView.refresh();
        }

        private void ToggleAllAnnotations_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ecgView.AreAnnotationsDisplayed = !ecgView.AreAnnotationsDisplayed;
        }

        private void ToggleSolutionAnnotations_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ecgView.AreCustomSolutionAnnotationsDisplayed = !ecgView.AreCustomSolutionAnnotationsDisplayed;
        }

        private void DetectQRS_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (signal == null)
                return;

            var result = QRSDetector.QRS_Detect(signal);
            signal.HeartRate = result.Item2;
            signal.Spikes = result.Item1;
            ecgView.refresh();
        }

        private void ToggleSpikes_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ecgView.IsQRSDisplayed = !ecgView.IsQRSDisplayed;
        }


        private void Open_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".HEA",
                Filter="HEA Files (*.HEA)|*.HEA"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name
            if (result == true)
            {
                // Open file name
                string filename = dlg.FileName;
                try
                {
                    List<String> channelNames = IOManager.loadCanals(filename);

                    ChannelSelector channelSelector = new ChannelSelector(channelNames);
                    channelSelector.ShowDialog();

                    int selectedChannelIndex = channelSelector.SelectedChannel;
                    if (selectedChannelIndex == -1)
                    {
                        MessageBox.Show("No channel selected. Aborting signal load.");
                        return;
                    }

                    signal = IOManager.loadECGFromSignalFile(filename,selectedChannelIndex+1);
                    ecgView.ECGSignal = signal ;
                }
                catch (RequiredFilesMissingException ex)
                {
                    String msg = "Signal files missing: ";
                    foreach (RequiredFilesMissingException.RequiredFiles rf in ex.MissingFiles)
                        msg += rf + " ";
                    MessageBox.Show(msg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error occurred: "+ex.Message);
                }
            }
            // else just ignore it
        }


        private void Save_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (signal == null)
                MessageBox.Show("Nothing to save..");
            else
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = signal.Name; // Default file name
                dlg.DefaultExt = ".HEA"; // Default file extension
                dlg.Filter = "HEA Files (.HEA)|*.HEA"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;
                    try
                    {
                        IOManager.saveECGToSignalFiles(signal, filename);
                        MessageBox.Show("Saved successfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }


        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
