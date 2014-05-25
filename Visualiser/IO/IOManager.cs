using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Visualiser.IO.Exceptions;
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
            if(!File.Exists(signalFileName)){
                List<RequiredFilesMissingException.RequiredFiles> requiredFiles = new List<RequiredFilesMissingException.RequiredFiles>
                {
                    RequiredFilesMissingException.RequiredFiles.HEA
                };
                throw new RequiredFilesMissingException(requiredFiles);
            }

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
            String route = signalFileName.Substring(0, signalFileName.Length - 4);
            if (!File.Exists(route + ".dat") && !File.Exists(route + ".txt")) 
            {
                List<RequiredFilesMissingException.RequiredFiles> requiredFiles = new List<RequiredFilesMissingException.RequiredFiles>
                {
                    RequiredFilesMissingException.RequiredFiles.DAT,
                    RequiredFilesMissingException.RequiredFiles.TXT
                };
                
                throw new RequiredFilesMissingException(requiredFiles);
            }

            ECG ecg = null;

            if (File.Exists(route + ".dat"))
                ecg = loadECGFromSignalBinaryFile(signalFileName, channelToLoad);
            else
                ecg = loadECGFromSignalTextFile(signalFileName, channelToLoad);

            List<ECGAnnotation> standardAnnotations = null;
            List<ECGAnnotation> customAnnotations = null;

            if (File.Exists(route + ".atr"))
            {
                standardAnnotations = loadStandardAnnotations(route + ".atr", channelToLoad);
            }

            if (File.Exists(route + ".cust"))
            {
                customAnnotations = loadCustomAnnotations(route + ".cust", channelToLoad);
            }

            if (standardAnnotations != null)
            {
                if (customAnnotations != null)
                    standardAnnotations.Concat(customAnnotations);

                ecg.Annotations = standardAnnotations;
            }
            else
            {
                if (customAnnotations != null)
                    ecg.Annotations = customAnnotations;
            }
            
            return ecg;
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
            ecg.Channel = channelToLoad;
            return ecg;
            // look for HEA ATR DAT & CUST on path etc.
        }

        static private List<ECGAnnotation> loadStandardAnnotations(String atrFileName, int channelToLoad = 1)
        {
            List<ECGAnnotation> standardAnnotations = new List<ECGAnnotation>();
            byte[] bytes = File.ReadAllBytes(atrFileName);
            int sampleTime = 0;
            int num = 0;
            int subtyp = 0;
            int chan = 0;
            bool increaseSampleTime = true;
            ECGAnnotation annotation = null;
            String aux = "";

            for (int i = 0; i < bytes.Length; i += 2)
            {
                aux = "";
                subtyp = 0;
                increaseSampleTime = true;

                int A = bytes[i + 1] >> 2;
                int I = (((bytes[i + 1] & 0x3) << 8) | bytes[i]);

                switch (A)
                {
                    // SKIP
                    case 59:
                        if (I == 0)
                        {
                            // if I is 0, we should skip the next 4 bytes
                            i += 4;
                        }
                        else
                        {
                            // otherwise we have to skip next I bytes
                            i += I;
                        }
                        increaseSampleTime = false;
                        break;
                    // NUM
                    case 60:
                        num = I;
                        annotation.Num = num;
                        increaseSampleTime = false;
                        break;
                    // SUB      
                    case 61:
                        subtyp = I;
                        annotation.SubTyp = subtyp;
                        increaseSampleTime = false;
                        break;
                    // CHAN
                    case 62:
                        chan = I;
                        annotation.Chan = chan;
                        increaseSampleTime = false;
                        break;
                    // AUX
                    case 63:
                        for (int j = 0; j < I; j++)
                        {
                            // next I bytes are the aux string letters.
                            // since we are reading two bytes at a time in main for loop, the aux string starts at (i+1)+1 position hence the: i+2+offset formula
                            char c = Convert.ToChar(bytes[i + 2 + j]);
                            if (c == '\0')
                                continue;
                            aux += c;
                        }

                        // if I is odd, there is a null byte pad appended to make the byte count even, but the null byte is not included in the byte count represented by I
                        i += I + (I % 2 == 0 ? 0 : 1);

                        annotation.Aux = aux;
                        increaseSampleTime = false;
                        break;
                    // EOF
                    case 0:
                        if (I == 0)
                        {
                            return standardAnnotations;
                        }
                        break;
                }

                if (increaseSampleTime)
                {
                    sampleTime += I;

                    annotation = new ECGAnnotation();
                    annotation.Type = ANNOTATION_TYPE.PHYSIONET_STANDARD;
                    annotation.TimeIndex = sampleTime * (1.0 / Frequency);
                    annotation.Text = ECGAnnotation.getStandardAnnotationTextFromCode(A);
                    annotation.Code = A;
                    annotation.Aux = aux;
                    annotation.Chan = chan;
                    annotation.Num = num;
                    annotation.SubTyp = subtyp;

                    if (annotation.Chan == (channelToLoad - 1))
                        standardAnnotations.Add(annotation);
                }
                else
                {
                    if (aux.Length > 0)
                        annotation.Text += " " + aux;
                }
            }

            return standardAnnotations;
        }

        static private void saveStandardAnnotations(ECG ecg, String atrFileName)
        {
            int sampleTime = 0;
            int num = 0;
            int chan = 0;
            
            FileStream fs = new FileStream(atrFileName+".atr", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);
            byte buf = Convert.ToByte(0x0);
            byte nul = Convert.ToByte(0x0);
            int previousSampleTime = 0;

            ecg.Annotations = ecg.Annotations.OrderBy(an => an.TimeIndex).ToList();

            for (int i = 0; i < ecg.Annotations.Count; i++)
            {
                ECGAnnotation annotation = ecg.Annotations.ElementAt(i);
                if (annotation.Type == ANNOTATION_TYPE.PHYSIONET_STANDARD)
                {
                    sampleTime = Convert.ToInt32(annotation.TimeIndex * Frequency) - previousSampleTime;
                    previousSampleTime = Convert.ToInt32(annotation.TimeIndex * Frequency);

                    buf = Convert.ToByte(sampleTime & 0xFF);
                    writer.Write(buf);
                    buf = Convert.ToByte(Convert.ToByte((annotation.Code << 2) & 0xFF) | (Convert.ToByte((sampleTime >> 8) & 0xFF) & 0x3));
                    writer.Write(buf);

                    if (annotation.Aux != null && annotation.Aux.Length > 0)
                    {
                        buf = Convert.ToByte((annotation.Aux.Length+1) & 0xFF);
                        writer.Write(buf);
                        buf = Convert.ToByte(Convert.ToByte((63 << 2) & 0xFF) | (Convert.ToByte(((annotation.Aux.Length + 1) >> 8) & 0xFF) & 0x3));
                        writer.Write(buf);

                        // write down AUX
                        for (int j = 0; j < annotation.Aux.Length; j++)
                        {
                            writer.Write(Convert.ToByte(annotation.Aux[j] & 0xFF));
                        }

                        writer.Write(nul); // string termination...

                        if (annotation.Aux.Length+1 % 2 != 0)
                        {
                            writer.Write(nul);
                        }
                    }

                    if (annotation.SubTyp != 0)
                    {
                        // write down SUB
                        buf = Convert.ToByte(annotation.SubTyp & 0xFF);
                        writer.Write(buf);
                        buf = Convert.ToByte(Convert.ToByte((61 << 2) & 0xFF) | (Convert.ToByte((annotation.SubTyp >> 8) & 0xFF) & 0x3));
                        writer.Write(buf);
                    }

                    if ((ecg.Channel-1) != chan)
                    {
                        chan = (ecg.Channel - 1);
                        // write down new CHAN

                        buf = Convert.ToByte(chan & 0xFF);
                        writer.Write(buf);
                        buf = Convert.ToByte(Convert.ToByte((62 << 2) & 0xFF) | (Convert.ToByte((chan >> 8) & 0xFF) & 0x3));
                        writer.Write(buf);
                    }

                    if (annotation.Num != num)
                    {
                        num = annotation.Num;
                        // write down new NUM

                        buf = Convert.ToByte(annotation.Num & 0xFF);
                        writer.Write(buf);
                        buf = Convert.ToByte(Convert.ToByte((60 << 2) & 0xFF) | (Convert.ToByte((annotation.Num >> 8) & 0xFF) & 0x3));
                        writer.Write(buf);
                    }
                }
            }

            // write EOF
            writer.Write(nul);
            writer.Write(nul);
            writer.Close();
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
            ecg.Channel = channelToLoad;
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

            // save standard annotations
            saveStandardAnnotations(signal, signalFileName.Substring(0, signalFileName.Length-4));
            
            //save custom annotations
            saveCustomAnnotations(signal, signalFileName.Substring(0, signalFileName.Length - 4));
        }

        static private void saveCustomAnnotations(ECG signal, String signalFileName) 
        {
            FileStream fileStream = new FileStream(signalFileName + ".cust", FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            List<ECGAnnotation> annotations = signal.Annotations;
            foreach (var annotation in annotations) 
            {
                if (annotation.Type == ANNOTATION_TYPE.ANSWER || annotation.Type == ANNOTATION_TYPE.SOLUTION) 
                {
                    streamWriter.WriteLine("{0} {1} {2} {3}", annotation.Type,signal.Channel, annotation.TimeIndex, annotation.Text);
                }
            }
            streamWriter.Close();
        }

        static private List<ECGAnnotation> loadCustomAnnotations(String signalFileName, int channel) 
        {
            if(!File.Exists(signalFileName))
            {
                return new List<ECGAnnotation>();
            }
            FileStream fileStream = new FileStream (signalFileName,FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            String line = streamReader.ReadLine();
            List<ECGAnnotation> annotations = new List<ECGAnnotation>();
            
            while(line != null) 
            {
                List<String> list = line.Split(' ').ToList();
                if (list[1] == channel.ToString())
                {
                    List<String> help = list.GetRange(3, list.Count - 3);
                    ECGAnnotation annotation = new ECGAnnotation()
                    {
                        Type = (ANNOTATION_TYPE)Enum.Parse(typeof(ANNOTATION_TYPE), list[0].ToString()),
                        TimeIndex = Convert.ToDouble(list[2]),
                        Text = String.Join(String.Empty,help)
                    };
                    annotations.Add(annotation);
                }

                // read next line
                line = streamReader.ReadLine();
            }
            streamReader.Close();
            return annotations;
        }
    }
}
