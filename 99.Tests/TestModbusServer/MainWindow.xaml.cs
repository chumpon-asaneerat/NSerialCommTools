using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Modbus.Device;

namespace TestModbusServer
{
    public partial class MainWindow : Window
    {
        private ModbusTcpSlave slave;
        private ModbusIpMaster master;
        private TcpListener slaveListener;
        private TcpClient masterClient;
        private Task updateInputsTask;
        private bool isMasterConnected;

        public MainWindow()
        {
            InitializeComponent();
            StartSlaveAsync(); // Fire and forget
        }

        private void StartSlaveAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    slaveListener = new TcpListener(IPAddress.Any, 502);
                    slaveListener.Start();
                    slave = ModbusTcpSlave.CreateTcp(1, slaveListener);
                    var dataStore = Modbus.Data.DataStoreFactory.CreateDefaultDataStore();

                    // Initialize 8 coil values (CoilDiscretes, addresses 0-7)
                    dataStore.CoilDiscretes[1] = true;  // Coil address 0
                    dataStore.CoilDiscretes[2] = true;  // Coil address 1
                    dataStore.CoilDiscretes[3] = false; // Coil address 2
                    dataStore.CoilDiscretes[4] = false; // Coil address 3
                    dataStore.CoilDiscretes[5] = true;  // Coil address 4
                    dataStore.CoilDiscretes[6] = false; // Coil address 5
                    dataStore.CoilDiscretes[7] = true;  // Coil address 6
                    dataStore.CoilDiscretes[8] = false; // Coil address 7

                    // Initialize 2 discrete input values (InputDiscretes, addresses 0-1)
                    dataStore.InputDiscretes[1] = true;  // Discrete input address 0
                    dataStore.InputDiscretes[2] = false; // Discrete input address 1

                    slave.DataStore = dataStore;

                    // Run listener in a background thread
                    Task.Factory.StartNew(() => slave.Listen(), TaskCreationOptions.LongRunning);

                    // Start a task to simulate updating InputDiscretes
                    updateInputsTask = Task.Factory.StartNew(() => UpdateInputDiscretes(dataStore), TaskCreationOptions.LongRunning);

                    Dispatcher.Invoke(() => StatusTextBlock.Text = "Slave running on port 502");
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Slave error: {ex.Message}");
                }
            });
        }

        private void UpdateInputDiscretes(Modbus.Data.DataStore dataStore)
        {
            // Simulate changing discrete inputs periodically
            while (slaveListener.Server.IsBound) // Stop if listener is closed
            {
                try
                {
                    // Toggle discrete input 0 every 5 seconds
                    dataStore.InputDiscretes[1] = !dataStore.InputDiscretes[1];
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Updated discrete input 0 to {dataStore.InputDiscretes[1]}");
                    Thread.Sleep(5000); // Wait 5 seconds
                }
                catch
                {
                    // Exit loop if slave is disposed
                    break;
                }
            }
        }

        private void ToggleMasterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected)
            {
                // Connect to master
                try
                {
                    masterClient = new TcpClient("localhost", 502);
                    master = ModbusIpMaster.CreateIp(masterClient);
                    isMasterConnected = true;
                    Dispatcher.Invoke(() =>
                    {
                        ToggleMasterButton.Content = "Disconnect Master";
                        StatusTextBlock.Text = "Master connected to slave";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Master connection error: {ex.Message}");
                }
            }
            else
            {
                // Disconnect master
                try
                {
                    master?.Dispose();
                    masterClient?.Close();
                    master = null;
                    masterClient = null;
                    isMasterConnected = false;
                    Dispatcher.Invoke(() =>
                    {
                        ToggleMasterButton.Content = "Connect Master";
                        StatusTextBlock.Text = "Master disconnected";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Master disconnection error: {ex.Message}");
                }
            }
        }

        private void ReadCoilsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    bool[] coils = master.ReadCoils(1, 0, 8); // Read coils 0-7
                    Dispatcher.Invoke(() =>
                    {
                        CoilsTextBlock.Text = string.Join(", ", coils);
                        StatusTextBlock.Text = "Read coils successful";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Read coils error: {ex.Message}");
                }
            });
        }

        private void WriteCoilButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            bool value = CoilValueCheckBox.IsChecked ?? false;
            Task.Run(() =>
            {
                try
                {
                    master.WriteSingleCoil(1, 0, value); // Write to coil 0
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Wrote coil value {value} to coil 0");
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Write coil error: {ex.Message}");
                }
            });
        }

        private void ReadDiscreteInputsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    bool[] inputs = master.ReadInputs(1, 0, 2); // Read discrete inputs 0-1
                    Dispatcher.Invoke(() =>
                    {
                        DiscreteInputsTextBlock.Text = string.Join(", ", inputs);
                        StatusTextBlock.Text = "Read discrete inputs successful";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Read discrete inputs error: {ex.Message}");
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                slave?.Dispose(); // Stops the listener
                master?.Dispose();
                masterClient?.Close();
                slaveListener?.Stop();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Shutdown error: {ex.Message}";
            }
        }
    }
}