#region Using

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

#endregion

namespace TestModbus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Internal Variables

        private ModbusMaster master = new ModbusMaster();

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = master;
        }

        private void cmdReadCoils_Click(object sender, RoutedEventArgs e)
        {
            if (null == master)
            {
                txtCoils.Text = string.Empty;
                return;
            }
            ushort noOfPoints = ushort.Parse(txtNoOfPoints.Text);
            bool[] coils = master.ReadCoils(noOfPoints);
            txtCoils.Text = string.Join(", ", coils);
        }

        private void cmdWriteCoils_Click(object sender, RoutedEventArgs e)
        {
            if (null == master)
            {
                txtCoils.Text = string.Empty;
                return;
            }
            string[] values = txtCoils.Text.Split(',');
            List<bool> coils = new List<bool>();
            foreach (var val in values)
            {
                coils.Add(bool.Parse(val));
            }
            master.WriteCoils(coils.ToArray());
        }

        private void cmdReadInputs_Click(object sender, RoutedEventArgs e)
        {
            if (null == master)
            {
                txtInputs.Text = string.Empty;
                return;
            }
            ushort noOfInputs = ushort.Parse(txtNoOfInputs.Text);
            bool[] coils = master.ReadInputs(noOfInputs);
            txtInputs.Text = string.Join(", ", coils);
        }
    }
}