using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Modbus.Device;

namespace TestModbusServer
{
    public partial class MainWindow2 : Window
    {
        private ModbusTcpSlave slave;
        private ModbusIpMaster master;
        private TcpListener slaveListener;
        private TcpClient masterClient;
        private Task updateInputsTask;
        private bool isMasterConnected;
        private bool isSlaveRunning;
        private CancellationTokenSource monitorCoilCts; // For canceling coil monitoring

        public MainWindow2()
        {
            InitializeComponent();
            // Initialize UI with default IP, port, and slave ID
            SlaveIpTextBox.Text = "127.0.0.1";
            SlavePortTextBox.Text = "502";
            SlaveIdTextBox.Text = "1";
            MasterIpTextBox.Text = "127.0.0.1";
            MasterPortTextBox.Text = "502";
            MasterSlaveIdTextBox.Text = "1";
            // Populate CoilAddressComboBox with addresses 0-7
            for (int i = 0; i <= 7; i++)
            {
                CoilAddressComboBox.Items.Add(i.ToString());
            }
            CoilAddressComboBox.SelectedIndex = 0; // Default to coil 0
        }

        private void StartSlaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isSlaveRunning)
            {
                StatusTextBlock.Text = "Slave is already running";
                return;
            }

            // Read UI inputs on the UI thread
            string slaveIp = SlaveIpTextBox.Text;
            string slavePort = SlavePortTextBox.Text;
            string slaveIdText = SlaveIdTextBox.Text;

            Task.Run(() =>
            {
                try
                {
                    // Validate inputs
                    if (!IPAddress.TryParse(slaveIp, out IPAddress ipAddress))
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid slave IP address");
                        return;
                    }
                    if (!int.TryParse(slavePort, out int port) || port < 1 || port > 65535)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid slave port");
                        return;
                    }
                    if (!byte.TryParse(slaveIdText, out byte slaveId) || slaveId < 1 || slaveId > 247)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid slave ID (must be 1-247)");
                        return;
                    }

                    slaveListener = new TcpListener(ipAddress, port);
                    slaveListener.Start();
                    slave = ModbusTcpSlave.CreateTcp(slaveId, slaveListener);
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
                    dataStore.InputDiscretes[1] = false;  // Discrete input address 0
                    dataStore.InputDiscretes[2] = false; // Discrete input address 1

                    slave.DataStore = dataStore;

                    // Run listener in a background thread
                    Task.Factory.StartNew(() => slave.Listen(), TaskCreationOptions.LongRunning);

                    // Start a task to simulate updating InputDiscretes
                    updateInputsTask = Task.Factory.StartNew(() => UpdateInputDiscretes(dataStore), TaskCreationOptions.LongRunning);

                    isSlaveRunning = true;
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBlock.Text = $"Slave running on {ipAddress}:{port} with ID {slaveId}";
                        StartSlaveButton.IsEnabled = false;
                        StopSlaveButton.IsEnabled = true;
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBlock.Text = $"Slave start error: {ex.Message}";
                        StartSlaveButton.IsEnabled = true;
                        StopSlaveButton.IsEnabled = false;
                    });
                    if (slaveListener != null)
                    {
                        slaveListener.Stop();
                        slaveListener = null;
                    }
                }
            });
        }

        private void StopSlaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isSlaveRunning)
            {
                StatusTextBlock.Text = "Slave is not running";
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    slave?.Dispose();
                    slaveListener?.Stop();
                    slave = null;
                    slaveListener = null;
                    isSlaveRunning = false;
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBlock.Text = "Slave stopped";
                        StartSlaveButton.IsEnabled = true;
                        StopSlaveButton.IsEnabled = false;
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Slave stop error: {ex.Message}");
                }
            });
        }

        private bool input1 = false;
        private bool input2 = false;
        private DateTime input1Time = DateTime.Now;
        private DateTime input2Time = DateTime.Now;

        private void cmdPressInput1_Click(object sender, RoutedEventArgs e)
        {
            input1 = true;
            input1Time = DateTime.Now;
        }

        private void cmdPressInput2_Click(object sender, RoutedEventArgs e)
        {
            input2 = true;
            input2Time = DateTime.Now;
        }

        private void UpdateInputDiscretes(Modbus.Data.DataStore dataStore)
        {
            // Simulate changing discrete inputs periodically
            while (slaveListener?.Server.IsBound == true)
            {
                try
                {
                    /*
                    // Toggle discrete input 0 every 5 seconds
                    dataStore.InputDiscretes[1] = !dataStore.InputDiscretes[1];
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Updated discrete input 0 to {dataStore.InputDiscretes[1]}");
                    Thread.Sleep(5000); // Wait 5 seconds
                    */
                    dataStore.InputDiscretes[1] = input1;
                    dataStore.InputDiscretes[2] = input2;
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Check discrete input 0: {dataStore.InputDiscretes[1]}, input 1: {dataStore.InputDiscretes[2]}");
                    Thread.Sleep(500); // Wait 0.5 seconds
                    // Reset inputs after 5 seconds
                    var ts1 = DateTime.Now - input1Time;
                    if (ts1.TotalSeconds > 5 && input1)
                    {
                        input1 = false;
                    }
                    var ts2 = DateTime.Now - input2Time;
                    if (ts2.TotalSeconds > 5 && input2)
                    {
                        input2 = false;
                    }
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
            // Read UI inputs on the UI thread
            string masterIp = MasterIpTextBox.Text;
            string masterPort = MasterPortTextBox.Text;
            string masterSlaveId = MasterSlaveIdTextBox.Text;

            if (!isMasterConnected)
            {
                // Connect to master
                Task.Run(() =>
                {
                    try
                    {
                        if (!IPAddress.TryParse(masterIp, out IPAddress ipAddress))
                        {
                            Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid master IP address");
                            return;
                        }
                        if (!int.TryParse(masterPort, out int port) || port < 1 || port > 65535)
                        {
                            Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid master port");
                            return;
                        }
                        if (!byte.TryParse(masterSlaveId, out byte slaveId) || slaveId < 1 || slaveId > 247)
                        {
                            Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid master slave ID (must be 1-247)");
                            return;
                        }

                        masterClient = new TcpClient(ipAddress.ToString(), port);
                        master = ModbusIpMaster.CreateIp(masterClient);
                        isMasterConnected = true;
                        Dispatcher.Invoke(() =>
                        {
                            ToggleMasterButton.Content = "Disconnect Master";
                            StatusTextBlock.Text = $"Master connected to {ipAddress}:{port} targeting slave ID {slaveId}";
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = $"Master connection error: {ex.Message}");
                    }
                });
            }
            else
            {
                // Disconnect master
                Task.Run(() =>
                {
                    try
                    {
                        monitorCoilCts?.Cancel(); // Stop monitoring if active
                        master?.Dispose();
                        masterClient?.Close();
                        master = null;
                        masterClient = null;
                        isMasterConnected = false;
                        Dispatcher.Invoke(() =>
                        {
                            ToggleMasterButton.Content = "Connect Master";
                            MonitorCoilButton.Content = "Monitor Coil";
                            CoilValueTextBlock.Text = "Coil Value: ";
                            StatusTextBlock.Text = "Master disconnected";
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = $"Master disconnection error: {ex.Message}");
                    }
                });
            }
        }

        private void ReadCoilsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            // Read UI input on the UI thread
            string masterSlaveId = MasterSlaveIdTextBox.Text;

            Task.Run(() =>
            {
                try
                {
                    if (!byte.TryParse(masterSlaveId, out byte slaveId) || slaveId < 1 || slaveId > 247)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid master slave ID (must be 1-247)");
                        return;
                    }

                    bool[] coils = master.ReadCoils(slaveId, 0, 8); // Read coils 0-7
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

            // Read UI inputs on the UI thread
            string masterSlaveId = MasterSlaveIdTextBox.Text;
            string coilAddressText = CoilAddressComboBox.SelectedItem?.ToString();
            bool value = CoilValueCheckBox.IsChecked ?? false;

            Task.Run(() =>
            {
                try
                {
                    if (!byte.TryParse(masterSlaveId, out byte slaveId) || slaveId < 1 || slaveId > 247)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid master slave ID (must be 1-247)");
                        return;
                    }
                    if (!int.TryParse(coilAddressText, out int address) || address < 0 || address > 7)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid coil address (must be 0-7)");
                        return;
                    }

                    master.WriteSingleCoil(slaveId, (ushort)address, value); // Write to selected coil
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBlock.Text = $"Wrote coil value {value} to coil {address} at {DateTime.Now:HH:mm:ss.fff}";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Write coil error: {ex.Message}");
                }
            });
        }

        private void MonitorCoilButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            // Read UI input on the UI thread
            string masterSlaveId = MasterSlaveIdTextBox.Text;
            string coilAddressText = CoilAddressComboBox.SelectedItem?.ToString();

            if (monitorCoilCts != null)
            {
                // Stop monitoring
                monitorCoilCts.Cancel();
                monitorCoilCts.Dispose();
                monitorCoilCts = null;
                Dispatcher.Invoke(() =>
                {
                    MonitorCoilButton.Content = "Monitor Coil";
                    CoilValueTextBlock.Text = "Coil Value: ";
                    StatusTextBlock.Text = "Coil monitoring stopped";
                });
            }
            else
            {
                // Start monitoring
                if (!byte.TryParse(masterSlaveId, out byte slaveId) || slaveId < 1 || slaveId > 247)
                {
                    StatusTextBlock.Text = "Invalid master slave ID (must be 1-247)";
                    return;
                }
                if (!int.TryParse(coilAddressText, out int address) || address < 0 || address > 7)
                {
                    StatusTextBlock.Text = "Invalid coil address (must be 0-7)";
                    return;
                }

                monitorCoilCts = new CancellationTokenSource();
                Task.Run(() => MonitorCoil(slaveId, (ushort)address, monitorCoilCts.Token));
                Dispatcher.Invoke(() =>
                {
                    MonitorCoilButton.Content = "Stop Monitoring";
                    StatusTextBlock.Text = $"Monitoring coil {address}";
                });
            }
        }

        private void MonitorCoil(byte slaveId, ushort address, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    bool[] coil = master.ReadCoils(slaveId, address, 1); // Read single coil
                    Dispatcher.Invoke(() =>
                    {
                        CoilValueTextBlock.Text = $"Coil Value: {coil[0]}";
                        StatusTextBlock.Text = $"Read coil {address} value {coil[0]} at {DateTime.Now:HH:mm:ss.fff}";
                    });
                    Thread.Sleep(500); // Poll every 500ms
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBlock.Text = $"Monitor coil error: {ex.Message}";
                        MonitorCoilButton.Content = "Monitor Coil";
                        CoilValueTextBlock.Text = "Coil Value: ";
                    });
                    monitorCoilCts?.Cancel();
                    monitorCoilCts?.Dispose();
                    monitorCoilCts = null;
                    break;
                }
            }
        }

        private void ReadDiscreteInputsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            // Read UI input on the UI thread
            string masterSlaveId = MasterSlaveIdTextBox.Text;

            Task.Run(() =>
            {
                try
                {
                    if (!byte.TryParse(masterSlaveId, out byte slaveId) || slaveId < 1 || slaveId > 247)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid master slave ID (must be 1-247)");
                        return;
                    }

                    bool[] inputs = master.ReadInputs(slaveId, 0, 2); // Read discrete inputs 0-1
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
                monitorCoilCts?.Cancel();
                slave?.Dispose();
                master?.Dispose();
                masterClient?.Close();
                slaveListener?.Stop();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => StatusTextBlock.Text = $"Shutdown error: {ex.Message}");
            }
        }
    }
}