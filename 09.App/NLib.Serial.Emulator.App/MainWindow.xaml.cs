using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NLib.Serial.Devices;
using NLib.Serial.Emulators;

namespace NLib.Serial.Emulator.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TFO1Device.Instance.Config.PortName = "COM4";
            TFO1Device.Instance.Start();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            TFO1Device.Instance.Shutdown();
        }

        private void cmdTFO1Send_Click(object sender, RoutedEventArgs e)
        {
            var data = TFO1Device.Instance.Value;
            data.F = 0;
            data.A = 366;
            data.W0 = 23;
            data.W4 = (decimal)343.5;
            var buffers = data.ToByteArray();
            TFO1Device.Instance.Send(buffers);
        }
    }
}
