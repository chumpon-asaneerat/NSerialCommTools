#region Using

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
using NLib.Serial.Terminals;

#endregion

namespace NLib.Serial.Terminal.App.Controls
{
    /// <summary>
    /// Interaction logic for WeightQATerminalPage.xaml
    /// </summary>
    public partial class WeightQATerminalPage : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public WeightQATerminalPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Loaded/Unloader

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
