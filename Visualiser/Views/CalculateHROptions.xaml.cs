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
using System.Windows.Shapes;

namespace Visualiser.Views
{
    /// <summary>
    /// Interaction logic for CalculateHROptions.xaml
    /// </summary>
    public partial class CalculateHROptions : Window
    {
        public delegate void HRCalculationRequestedHandler(double lower, double upper);
        public event HRCalculationRequestedHandler HRCalculationRequested;

        public CalculateHROptions()
        {
            InitializeComponent();
        }

        private void b_ok_Click(object sender, RoutedEventArgs e)
        {
            String[] lowerTimeIndexStrings = tb_lowerTimeIndex.Text.Split(':');
            String[] upperTimeIndexStrings = tb_upperTimeIndex.Text.Split(':');

            if (lowerTimeIndexStrings.Length == 0 || lowerTimeIndexStrings.Length > 3)
                MessageBox.Show("Lower TimeIndex must be specified correctly ([mm]:[ss]:msec)!");
            else if (upperTimeIndexStrings.Length == 0 || upperTimeIndexStrings.Length > 3)
                MessageBox.Show("Uppert TimeIndex must be specified correctly ([mm]:[ss]:msec)!");
            else
            {
                try
                {
                    double[] lowerTimeIndexNumbers = lowerTimeIndexStrings.Select(timestampstring =>
                    {
                        return Double.Parse(timestampstring);
                    }).Reverse().ToArray();
                    double[] upperTimeIndexNumbers = upperTimeIndexStrings.Select(timestampstring => {
                        return Double.Parse(timestampstring);
                    }).Reverse().ToArray();

                    // first milliseconds (divide by 1000)
                    lowerTimeIndexNumbers[0] = lowerTimeIndexNumbers[0] / 1000;
                    upperTimeIndexNumbers[0] = upperTimeIndexNumbers[0] / 1000;

                    // second should be seconds, no division or multiplication neccessary
                    // thrid should be minutes, multiply by 60
                    if (lowerTimeIndexNumbers.Length > 2)
                        lowerTimeIndexNumbers[2] = lowerTimeIndexNumbers[2] * 60;
                    if (upperTimeIndexNumbers.Length > 2)
                        upperTimeIndexNumbers[2] = upperTimeIndexNumbers[2] * 60;

                    // now sum for total seconds
                    double lowerTimeIndex = 0;
                    for (int i = 0; i < lowerTimeIndexNumbers.Length; i++)
                        lowerTimeIndex += lowerTimeIndexNumbers[i];

                    double upperTimeIndex = 0;
                    for (int i = 0; i < upperTimeIndexNumbers.Length; i++)
                        upperTimeIndex += upperTimeIndexNumbers[i];

                    OnHRCalculationRequested(lowerTimeIndex, upperTimeIndex);
                }
                catch (Exception)
                {
                    MessageBox.Show("Incorrect timestamp format!");
                }
            }
        }

        private void OnHRCalculationRequested(double lower, double upper)
        {
            if (HRCalculationRequested != null)
                HRCalculationRequested(lower, upper);
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
