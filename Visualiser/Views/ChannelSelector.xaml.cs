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
    /// Interaction logic for ChannelSelector.xaml
    /// </summary>
    public partial class ChannelSelector : Window
    {
        public ChannelSelector()
        {
            InitializeComponent();
        }

        public ChannelSelector(List<String> channels)
        {
            InitializeComponent();
            channels.ForEach(channel => { lb_channels.Items.Add(channel); });
        }

        public int SelectedChannel
        {
            get
            {
                return lb_channels.SelectedIndex;
            }
        }

        private void b_ok_Click(object sender, RoutedEventArgs e)
        {
            if (lb_channels.SelectedIndex == -1)
            {
                MessageBox.Show("No channel selected.");
            }
            else
            {
                this.Close();
            }
        }
    }
}
