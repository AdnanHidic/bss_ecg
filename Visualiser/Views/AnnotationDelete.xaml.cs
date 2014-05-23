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
using Visualiser.Models;

namespace Visualiser.Views
{
    /// <summary>
    /// Interaction logic for AnnotationDelete.xaml
    /// </summary>
    public partial class AnnotationDelete : Window
    {
        public delegate void AnnotationDeletionRequestedHandler(ECGAnnotation annotation);
        public event AnnotationDeletionRequestedHandler annotationDeletionRequested;

        public AnnotationDelete()
        {
            InitializeComponent();
        }

        public AnnotationDelete(List<ECGAnnotation> annotations)
        {
            InitializeComponent();
            annotations.ForEach(annotation =>
            {
                lb_annotations.Items.Add(annotation);
            });
        }

        private void b_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void b_delete_Click(object sender, RoutedEventArgs e)
        {
            if (lb_annotations.SelectedIndex != -1)
            {
                OnAnnotationDeletionRequested(lb_annotations.SelectedItem as ECGAnnotation);
                lb_annotations.Items.RemoveAt(lb_annotations.SelectedIndex);               
            }
            else
                MessageBox.Show("Nothing selected to delete!");
        }

        private void OnAnnotationDeletionRequested(ECGAnnotation annotation)
        {
            if (annotationDeletionRequested != null)
                annotationDeletionRequested(annotation);
        }
    }
}
