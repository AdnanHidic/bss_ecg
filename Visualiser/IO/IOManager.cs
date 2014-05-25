using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Visualiser.Models;

namespace Visualiser.IO
{
    /// <summary>
    /// Static class used to perform IO operations on ECG signal model. 
    /// Class methods make it possible to load ECG model from .hea .dat and .atr PhysioNet files,
    /// and save edited ECG model to respective files as well.
    /// </summary>
    static public class IOManager
    {
        static private int Frequency;
        static private long SampleNumber;
        static private int ChannelNumber;

        static public List<String> loadCanals(String signalFileName){
            StreamReader strReader = new StreamReader(signalFileName);
            String line = strReader.ReadLine();
            List<String> firstLine = line.Split(' ').ToList();
            ChannelNumber = Convert.ToInt32(firstLine[1]);
            Frequency = Convert.ToInt32(firstLine[2]);
            SampleNumber = Convert.ToInt32(firstLine[3]);
            List<String> Channels = new List<String>();
            for (int i = 0; i < ChannelNumber; i++) 
            {
                line = strReader.ReadLine();
                Channels.Add(line.Split(' ').ToList().Last());
            }
            return Channels;
        }
        /// <summary>
        /// Used to generate ECG signal model from PhysioNet signal files. Method searches for .HEA, .DAT and .ATR files 
        /// with the name given as method call parameter.
        /// </summary>
        /// <param name="signalFileName">Full path to the HEA file (e.g. C:\100.HEA)</param>
        /// <returns>Generated ECG model.</returns>
        static public ECG loadECGFromSignalFile(String signalFileName, int channelToLoad = 1) 
        {
            String type = signalFileName.Substring(signalFileName.Length - 4);
            if (type == ".dat")
                return loadECGFromSignalBinaryFile(signalFileName, channelToLoad);
            else
                return loadECGFromSignalTextFile(signalFileName, channelToLoad);
        }

        static private ECG loadECGFromSignalBinaryFile(String signalFileName, int channelToLoad=1)
        {
            String signal = signalFileName.Substring(0,signalFileName.Length - 4);
            FileStream file = new FileStream(signal+".dat", FileMode.Open);
            BinaryReader binReader = new BinaryReader(file);
            List<ECGPoint> ecgPoints = new List<ECGPoint>();
            int count = 0;
            short flag = 0;
            long low = 0, high = 0;
            byte[] buf = { 0, 0, 0 };

            for (int i = 0; i < file.Length / 3; i++)
            {
                for (short j = 1; j <= 2; j++)
                {
                    count++;
                    switch (flag)
                    {
                        case 0:
                            try
                            {
                                buf = binReader.ReadBytes(3);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());  
                            }
                            low = buf[1] & 0x0F;
                            high = buf[1] & 0xF0;
                            if (channelToLoad == j)
                                if (low > 7)
                                {
                                    ECGPoint ecgPoint = new ECGPoint();
                                    ecgPoint.TimeIndex = count / Convert.ToDouble(Frequency);
                                    ecgPoint.Value = Convert.ToDouble(buf[0] + (low << 8) - 4096);
                                    ecgPoints.Add(ecgPoint);
                                }
                                else
                                {
                                    ECGPoint ecgPoint = new ECGPoint();
                                    ecgPoint.TimeIndex = count / Convert.ToDouble(Frequency);
                                    ecgPoint.Value = Convert.ToDouble((buf[0] + (low << 8) - 1024) * 0.005);
                                    ecgPoints.Add(ecgPoint);
                                }
                            flag = 1;
                            break;
                        case 1:
                            if (channelToLoad == j)
                                if (high > 127)
                                {
                                    ECGPoint ecgPoint = new ECGPoint();
                                    ecgPoint.TimeIndex = count / Convert.ToDouble(Frequency);
                                    ecgPoint.Value = Convert.ToDouble(buf[2] + (high << 4) - 4096);
                                    ecgPoints.Add(ecgPoint);
                                }
                                else
                                {
                                    ECGPoint ecgPoint = new ECGPoint();
                                    ecgPoint.TimeIndex = count / Convert.ToDouble(Frequency);
                                    ecgPoint.Value = Convert.ToDouble((buf[2] + (high << 4) - 1024) * 0.005);
                                    ecgPoints.Add(ecgPoint);
                                }
                            flag = 0;
                            break;
                    }
                }
            }
            file.Close();
            binReader.Close();
            ECG ecg = new ECG();
            ecg.Name = signalFileName;
            ecg.Points = ecgPoints;
            ecg.SamplingRate = Frequency;
            return ecg;
            // look for HEA ATR DAT & CUST on path etc.
        }

        static private void loadStandardECGAnnotations(String atrFileName)
        {
            byte[] bytes = File.ReadAllBytes(@"C:\Projects\bss_ecg\Signals\100.atr");
            int sampleTime = 0;
            int num = 0;
            int subtyp = 0;
            int chan = 0;
            bool increaseSampleTime = true;
            String aux = "";

            for (int i = 0; i < bytes.Length; i += 2)
            {
                aux = "";
                subtyp = 0;
                increaseSampleTime = true;

                int A = bytes[i + 1] >> 2;
                int I = (((bytes[i + 1] & 0x03) << 6) | bytes[i]);

                switch (A)
                {
                    // SKIP
                    case 59:
                        if (I == 0)
                        {
                            // if I is 0, we should skip the next 4 bytes
                            i += 2;
                        }
                        else
                        {
                            // otherwise we have to skip next I bytes
                            i += I - 2;
                        }
                        increaseSampleTime = false;
                        break;
                    // NUM
                    case 60:
                        num = I;
                        increaseSampleTime = false;
                        break;
                    // SUB      
                    case 61:
                        subtyp = I;
                        increaseSampleTime = false;
                        break;
                    // CHAN
                    case 62:
                        chan = I;
                        increaseSampleTime = false;
                        break;
                    // AUX
                    case 63:
                        for (int j = 0; j < I; j++)
                        {
                            // next I bytes are the aux string letters.
                            // since we are reading two bytes at a time in main for loop, the aux string starts at (i+1)+1 position hence the: i+2+offset formula
                            aux += Convert.ToChar(bytes[i + 2 + j]);
                        }

                        // if I is odd, there is a null byte pad appended to make the byte count even, but the null byte is not included in the byte count represented by I
                        i += I + (I % 2 == 0 ? 0 : 1) - 2;

                        increaseSampleTime = false;
                        break;
                    // EOF
                    case 0:
                        if (I == 0)
                        {
                            Console.WriteLine("End of signal");
                            sampleTime = 0;
                            increaseSampleTime = false;
                        }
                        break;
                }

                if (increaseSampleTime)
                    sampleTime += I;

                Console.WriteLine("A: " + A + " I: " + I + " Sample: " + sampleTime + " Chan: " + chan + " Subtyp: " + subtyp + " Num: " + num + " Aux: " + aux);
            }
        }

        static private ECG loadECGFromSignalTextFile(String signalFileName, int channelToLoad = 1)
        {
            String signal = signalFileName.Substring(0, signalFileName.Length - 4);
            FileStream file = new FileStream(signal + ".txt", FileMode.Open);
            StreamReader streamReader = new StreamReader(file);
            List<ECGPoint> ecgPoints = new List<ECGPoint>();
            string line = "";
            line = streamReader.ReadLine();
            int count = 0;
            while (!(line.Contains("0.000")))
            {
                if (line == null)
                    break;
                try
                {
                    line = streamReader.ReadLine();
                }
                catch (Exception)
                {
                    Console.WriteLine(ecgPoints.ToString());
                }
            }

            do
            {
                ECGPoint ecgPoint = new ECGPoint();
                ecgPoint.Value = (double.Parse(line.Split('\t')[channelToLoad]));
                ecgPoint.TimeIndex = count / Convert.ToDouble(Frequency);
                ecgPoints.Add(ecgPoint);
            } while ((line = streamReader.ReadLine()) != null);
            file.Close();
            streamReader.Close();
            ECG ecg = new ECG();
            ecg.Name = signalFileName;
            ecg.Points = ecgPoints;
            ecg.SamplingRate = Frequency;
            return ecg;
        }
        /// <summary>
        /// Used to save ECG signal model to PhysioNet signal files. Method generates .HEA, .DAT and .ATR files 
        /// with the name given as method call parameter.
        /// </summary>
        /// <param name="signalFileName">Path + signal name (e.g. C:\100)</param>
        static public void saveECGToSignalFiles(ECG signal, String signalFileName)
        {
            // save HEA ATR DAT & CUST
            throw new NotImplementedException();
        }
    }
}
