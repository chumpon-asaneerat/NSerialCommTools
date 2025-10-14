using Modbus.Device;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TestModbusServer;

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
        private bool isSlaveRunning;
        public ObservableCollection<ModbusItem> Coils { get; } = new ObservableCollection<ModbusItem>();
        public ObservableCollection<ModbusItem> DiscreteInputs { get; } = new ObservableCollection<ModbusItem>();
        public bool InputBit0 { get; set; }
        public bool InputBit1 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            // Initialize UI with default IP, port, and slave ID
            SlaveIpTextBox.Text = "127.0.0.1";
            SlavePortTextBox.Text = "502";
            SlaveIdTextBox.Text = "1";
            MasterIpTextBox.Text = "127.0.0.1";
            MasterPortTextBox.Text = "502";
            MasterSlaveIdTextBox.Text = "1";
            // Initialize coils and discrete inputs
            for (int i = 0; i < 8; i++)
            {
                Coils.Add(new ModbusItem { Address = i, Name = $"Coil {i}", Value = i < 2 || i == 4 || i == 6 });
            }
            for (int i = 0; i < 2; i++)
            {
                DiscreteInputs.Add(new ModbusItem { Address = i, Name = $"Input {i}", Value = i == 0 });
            }
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
                    //dataStore.CoilDiscretes[7] = true;  // Coil address 6
                    //dataStore.CoilDiscretes[8] = false; // Coil address 7

                    // Initialize 2 discrete input values (InputDiscretes, addresses 0-1)
                    dataStore.InputDiscretes[1] = true;  // Discrete input address 0
                    dataStore.InputDiscretes[2] = false; // Discrete input address 1

                    // Initialize holding register for input simulation (address 0)
                    dataStore.HoldingRegisters[1] = 0; // Register address 0, bits for inputs

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

        private void UpdateInputDiscretes(Modbus.Data.DataStore dataStore)
        {
            // Update discrete inputs based on holding register
            while (slaveListener?.Server.IsBound == true)
            {
                try
                {
                    // Read holding register 0 to update discrete inputs
                    ushort registerValue = dataStore.HoldingRegisters[1];
                    dataStore.InputDiscretes[1] = (registerValue & 1) == 1; // Bit 0 for input 0
                    dataStore.InputDiscretes[2] = (registerValue & 2) == 2; // Bit 1 for input 1

                    Dispatcher.Invoke(() =>
                    {
                        DiscreteInputs[0].Value = dataStore.InputDiscretes[1];
                        DiscreteInputs[1].Value = dataStore.InputDiscretes[2];
                        StatusTextBlock.Text = $"Updated discrete inputs: {dataStore.InputDiscretes[1]}, {dataStore.InputDiscretes[2]}";
                    });
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
                        for (int i = 0; i < 8; i++)
                        {
                            Coils[i].Value = coils[i];
                        }
                        StatusTextBlock.Text = "Read coils successful";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Read coils error: {ex.Message}");
                }
            });
        }

        private void CoilCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            // Read UI inputs on the UI thread
            string masterSlaveId = MasterSlaveIdTextBox.Text;
            var checkBox = sender as System.Windows.Controls.CheckBox;
            var item = checkBox?.DataContext as ModbusItem;

            if (item == null) return;

            Task.Run(() =>
            {
                try
                {
                    if (!byte.TryParse(masterSlaveId, out byte slaveId) || slaveId < 1 || slaveId > 247)
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Invalid master slave ID (must be 1-247)");
                        return;
                    }

                    master.WriteSingleCoil(slaveId, (ushort)item.Address, item.Value);
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Wrote coil value {item.Value} to coil {item.Address}");
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
                        for (int i = 0; i < 2; i++)
                        {
                            DiscreteInputs[i].Value = inputs[i];
                        }
                        StatusTextBlock.Text = "Read discrete inputs successful";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Read discrete inputs error: {ex.Message}");
                }
            });
        }

        private void WriteInputRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMasterConnected || master == null)
            {
                StatusTextBlock.Text = "Master not connected";
                return;
            }

            // Read UI inputs on the UI thread
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

                    // Calculate register value from bit checkboxes
                    ushort registerValue = (ushort)((InputBit0 ? 1 : 0) | (InputBit1 ? 2 : 0));
                    master.WriteSingleRegister(slaveId, 0, registerValue);
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Wrote input register value {registerValue} to register 0");
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Write input register error: {ex.Message}");
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
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