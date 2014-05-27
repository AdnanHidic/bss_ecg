using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualiser.Models;

namespace Visualiser.Processing
{
    /// <summary>
    /// Static class that implements So and Chan's QRS detection algorithm.
    /// </summary>
    public static class QRSDetector
    {
        public static double Determine_HeartRate(ECG signal, double lowerTimeIndex, double upperTimeIndex)
        {
            List<ECGPoint> spikesWithinBounds = signal.Spikes.Where(ecgpoint =>
            {
                return ecgpoint.TimeIndex >= lowerTimeIndex && ecgpoint.TimeIndex <= upperTimeIndex;
            }).ToList();

            // get the seconds between first and the last spike
            double sumTimesBetweenSpikes = spikesWithinBounds[spikesWithinBounds.Count - 1].TimeIndex - spikesWithinBounds[0].TimeIndex;
            // determine the HR
            double heartRate = 60 * spikesWithinBounds.Count / sumTimesBetweenSpikes;

            return heartRate;
        }


        /// <summary>
        /// Performs QRS-complex detection on ECG signal provided as call argument.
        /// </summary>
        /// <param name="signal">ECG signal object</param>
        /// <returns>List of ECG points determined to be R spikes and Decimal value for measured heart-rate</returns>
        public static List<ECGPoint> QRS_Detect(ECG signal)
        {
            // extract voltages from ecg signal points
            double[] voltages = signal.Points.Select(point => point.Value).ToArray();
            // perform QRS detection
            List<int> spikeIndices = SoAndChan(voltages);
            // map indices of R spikes into corresponding ecg signal points
            List<ECGPoint> spikes = spikeIndices.Select(index => signal.Points[index]).ToList();

            return spikes;
        
        }

        private const double THRESHOLD_PARAM = 8;
        private const double FILTER_PARAMETER = 16;
        private const int SAMPLE_RATE = 250;


        // originalni rad: http://ieeexplore.ieee.org/xpl/login.jsp?tp=&arnumber=754529&url=http%3A%2F%2Fieeexplore.ieee.org%2Fxpls%2Fabs_all.jsp%3Farnumber%3D754529
        // izvorni kod: http://blog.kenliao.info/2012/03/c-so-and-chan-method-r-peak-detection.html
        // obrazloženje algoritma: http://researcher.most.gov.tw/public/8905780/Attachment/86518531071.ppt
        // prilagodio: Hidić Adnan
        public static List<int> SoAndChan(double[] voltages)
        {
            // initial maxi should be the max slope of the first SAMPLE_RATE points.
            double initial_maxi = -2 * voltages[0] - voltages[1] + voltages[3] + 2 * voltages[4];
            for (int i = 2; i < SAMPLE_RATE; i++)
            {
                double slope = -2 * voltages[i - 2] - voltages[i - 1] + voltages[i + 1] + 2 * voltages[i + 2];
                if (slope > initial_maxi)
                    initial_maxi = slope;
            }

            List<int> rIndices = new List<int>();

            // set initial maxi
            double maxi = initial_maxi;
            bool first_satisfy = false;
            bool second_satisfy = false;
            int onset_point = 0;
            int R_point = 0;

            // I want a way to plot all the r dots that are found...
            int[] rExist = new int[voltages.Length];
            // First two voltages should be ignored because we need rom length
            for (int i = 2; i < voltages.Length - 2; i++)
            {

                // Last two voltages should be ignored too
                if (!first_satisfy || !second_satisfy)
                {
                    // Get Slope:
                    double slope = -2 * voltages[i - 2] - voltages[i - 1] + voltages[i + 1] + 2 * voltages[i + 2];

                    // Get slope threshold
                    double slope_threshold = (THRESHOLD_PARAM / 16) * maxi;

                    // We need two consecutive data that satisfy slope > slope_threshold
                    if (slope > slope_threshold)
                    {
                        if (!first_satisfy)
                        {
                            first_satisfy = true;
                            onset_point = i;
                        }
                        else
                        {
                            if (!second_satisfy)
                            {
                                second_satisfy = true;
                            }
                        }
                    }
                }
                // We found the ONSET already, now we find the R point
                else
                {

                    if (voltages[i] < voltages[i - 1])
                    {
                        rIndices.Add(i - 1);
                        R_point = i - 1;

                        // Since we have the R, we should reset
                        first_satisfy = false;
                        second_satisfy = false;

                        // and update maxi
                        double first_maxi = voltages[R_point] - voltages[onset_point];
                        maxi = ((first_maxi - maxi) / FILTER_PARAMETER) + maxi;
                    }
                }
            }
            return rIndices;
        }
    }
}
