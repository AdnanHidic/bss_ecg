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
            currentSignal = new ECG();
            DataContext = currentSignal;
        }

        private void AfterInitializeComponent()
        {
            
        }

        private ECG currentSignal;

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Simple utility application for displaying PhysioNet-compliant ECG signals.\nAuthors: Hajdarević Adnan, Hidić Adnan, Zubanović Damir");
        }
    }
}
