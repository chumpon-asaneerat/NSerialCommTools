using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using Modbus.Device;

namespace TestModbus
{
    /// <summary>
    /// The Modbus Client (Application)
    /// </summary>
    public class ModbusMaster
    {
        public string IPAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 502;
        public ushort StartAddress { get; set; } = 0;

        #region Coils

        public void WriteCoils(params bool[] coilValues)
        {
            try
            {
                TcpClient client = new TcpClient(IPAddress, Port);
                using (ModbusIpMaster master = ModbusIpMaster.CreateIp(client))
                {
                    master.WriteMultipleCoils(StartAddress, coilValues);
                }
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool[] ReadCoils(ushort noOfPoints)
        {
            bool[] rets = new bool[noOfPoints];
            try
            {
                TcpClient client = new TcpClient(IPAddress, Port);
                using (ModbusIpMaster master = ModbusIpMaster.CreateIp(client))
                {
                    rets = master.ReadCoils(StartAddress, noOfPoints);
                }
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return rets;
        }

        #endregion

        #region Inputs

        public bool[] ReadInputs(ushort noOfInputs)
        {
            bool[] rets = new bool[noOfInputs];
            try
            {
                TcpClient client = new TcpClient(IPAddress, Port);
                using (ModbusIpMaster master = ModbusIpMaster.CreateIp(client))
                {
                    rets = master.ReadInputs(StartAddress, noOfInputs);
                }
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return rets;
        }

        #endregion
    }

    /// <summary>
    /// The Modbus Device Emulator.
    /// </summary>
    public class ModbusSlave
    {
        public int SlaveId { get; set; } = 1;
    }
}
