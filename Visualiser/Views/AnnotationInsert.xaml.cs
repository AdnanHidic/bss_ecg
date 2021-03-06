﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visualiser.Models;

namespace Visualiser.Views
{
    /// <summary>
    /// Interaction logic for AnnotationInsert.xaml
    /// </summary>
    public partial class AnnotationInsert : Window
    {
        public delegate void AnnotationInsertRequestedHandler(ECGAnnotation annotation);
        public event AnnotationInsertRequestedHandler annotationInsertRequested;

        private Boolean HasBeenInitialized;

        public AnnotationInsert()
        {
            HasBeenInitialized = false;

            InitializeComponent();

            tb_content.Visibility = Visibility.Hidden;
            cb_standard_content.Visibility = Visibility.Visible;

            ECGAnnotation.StandardAnnotationCodesAndDescs.ForEach(
                tuple =>
                {
                    cb_standard_content.Items.Add(tuple.Item2);
                });
            cb_standard_content.SelectedIndex = 0;

            HasBeenInitialized = true;
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Insert_Button_Click(object sender, RoutedEventArgs e)
        {
            ANNOTATION_TYPE selectedType = (ANNOTATION_TYPE)cb_annotationType.SelectedItem;
            String content = "";
            if (selectedType == ANNOTATION_TYPE.PHYSIONET_STANDARD)
            {
                content = cb_standard_content.SelectedItem as String;
            }
            else
            {
                content = tb_content.Text;
            }

            String[] timestampStrings = tb_timestamp.Text.Split(':');

            if (content.Length == 0)
                MessageBox.Show("There must be some annotation content?");
            else if (timestampStrings.Length==0 || timestampStrings.Length>3)
                MessageBox.Show("Timestamp must be specified correctly ([mm]:[ss]:msec)!");
            else
            {
                try
                {
                    double[] timestampNumbers = timestampStrings.Select(timestampstring =>
                    {
                        return Double.Parse(timestampstring);
                    }).Reverse().ToArray();

                    // first milliseconds (divide by 1000)
                    timestampNumbers[0] = timestampNumbers[0] / 1000;
                    // second should be seconds, no division or multiplication neccessary
                    // thrid should be minutes, multiply by 60
                    if (timestampNumbers.Length > 2)
                        timestampNumbers[2] = timestampNumbers[2]* 60;

                    // now sum for total seconds
                    double timeIndex = 0;
                    for (int i = 0; i < timestampNumbers.Length; i++)
                        timeIndex += timestampNumbers[i];

                    ECGAnnotation annot = new ECGAnnotation()
                    {
                        Text = content,
                        Type = selectedType,
                        TimeIndex = timeIndex
                    };

                    if (annot.Type == ANNOTATION_TYPE.PHYSIONET_STANDARD)
                    {
                        // set the MIT annotation specific values
                        annot.Code = ECGAnnotation.getCodeFromStandardAnnotationText(annot.Text);
                        annot.Aux = "";
                    }

                    MessageBox.Show("Insertion successful!");
                    OnAnnotationInsertRequested(annot);
                }
                catch (Exception)
                {
                    MessageBox.Show("Incorrect timestamp format!");
                }
            }
        }

        private void OnAnnotationInsertRequested(ECGAnnotation annotation)
        {
            if (annotationInsertRequested != null)
                annotationInsertRequested(annotation);
        }

        private void cb_annotationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!HasBeenInitialized)
                return;

            if ((ANNOTATION_TYPE)cb_annotationType.SelectedItem == ANNOTATION_TYPE.PHYSIONET_STANDARD ){
                tb_content.Visibility = Visibility.Hidden;
                cb_standard_content.Visibility = Visibility.Visible;
            }
            else
            {
                tb_content.Visibility = Visibility.Visible;
                cb_standard_content.Visibility = Visibility.Hidden;
            }
        }
    }
}
