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
            /*
            //string ret = Encoding.ASCII.GetString(new byte[] { 0x83 });
            string ret = char.ConvertFromUtf32(0x83);
            byte[] rets = Encoding.ASCII.GetBytes(ret);
            Console.WriteLine(rets);
            */
            TFO1Data data = new TFO1Data();
            data.F = 0;
            data.A = 366;
            data.W0 = 23;
            data.W4 = (decimal)343.5;
            var buffers = data.ToByteArray();
            Console.WriteLine(ByteArrayHelper.ToHexString(buffers));
        }
    }
}
