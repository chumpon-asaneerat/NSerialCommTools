#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-19
=================
- Netwrok framework.
  - NetworkUtils class is move from namespace NLib.Networks to NLib.Utils.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using System.Threading;
using System.Net.NetworkInformation;

using NLib.Networks;

#endregion

namespace NLib.Utils
{
    /// <summary>
    /// Network Utilities class. Provide Network's related functions
    /// </summary>
    public class NetworkUtils
    {
        #region Find Network Computers

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Find Network Computers. Uses the DllImport : NetServerEnum with all its required parameters
        /// (see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/netmgmt/netmgmt/netserverenum.asp
        /// for full details or method signature) to retrieve a list of domain SV_TYPE_WORKSTATION
        /// and SV_TYPE_SERVER PC's.
        /// </summary>
        /// <returns> A list of string that represents all the PC's in the Domain. </returns>
        /// ------------------------------------------------------------------------------------
        public static List<string> FindNetworkComputers()
        {
            #region Const

            const int MAX_PREFERRED_LENGTH = -1;
            const int SV_TYPE_WORKSTATION = 1;
            const int SV_TYPE_SERVER = 2;

            #endregion

            List<string> networkComputers = new List<string>();
            IntPtr buffer = IntPtr.Zero;
            IntPtr tmpBuffer = IntPtr.Zero;

            int sizeofINFO = Marshal.SizeOf(typeof(NetworkAPI.SERVER_INFO_100));
            int entriesRead = 0;
            int totalEntries = 0;
            int resHandle = 0;

            try
            {
                // Call the DllImport
                // See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/netmgmt/netmgmt/netserverenum.asp
                int ret = NetworkAPI.NetServerEnum(null, 100, ref buffer,
                    MAX_PREFERRED_LENGTH, out entriesRead, out totalEntries,
                    SV_TYPE_WORKSTATION | SV_TYPE_SERVER,
                    null, out resHandle);

                if (ret == 0)
                {
                    // Loop through all SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's
                    for (int i = 0; i < totalEntries; i++)
                    {
                        // Get pointer to, Pointer to the buffer that received the data from the call to NetServerEnum. Must ensure to use
                        // correct size of STRUCTURE to ensure correct location in memory is pointed to
                        tmpBuffer = new IntPtr((int)buffer + (i * sizeofINFO));
                        // Have now got a pointer to the list of SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's, which is unmanaged memory needs to
                        // Marshal data from an unmanaged block of memory to a managed object, again using STRUCTURE to ensure the correct data
                        // is marshalled
                        NetworkAPI.SERVER_INFO_100 svrInfo = (NetworkAPI.SERVER_INFO_100)Marshal.PtrToStructure(tmpBuffer, typeof(NetworkAPI.SERVER_INFO_100));
                        // Add the PC names to the ArrayList
                        networkComputers.Add(svrInfo.sv100_name.ToUpper());
                    }
                }
            }
            finally
            {
                // Free the memory that the NetApiBufferAllocate function allocates
                NetworkAPI.NetApiBufferFree(buffer);
            }
            // Return the computers
            return networkComputers;
        }

        #endregion

        #region Is Other Computer Connected

        /// <summary>
        /// Check if a computer is on the network.
        /// </summary>
        public static bool IsOtherComputerConnected
        {
            get
            {
                string computerName = string.Empty;

                try
                {
                    computerName =
                        Dns.GetHostEntry(Dns.GetHostName()).HostName.ToUpper();
                }
                catch (Exception)
                {
                    computerName = string.Empty;
                }
                if (string.IsNullOrWhiteSpace(computerName))
                {
                    return false;
                }

                List<string> computersName = FindNetworkComputers();
                // Remove the computer on which the service is running
                if (computersName.Contains(computerName))
                    computersName.Remove(computerName);
                return computersName.Count > 0;
            }
        }

        #endregion

        #region IP/Host

        /// <summary>
        /// Get Local Host's Name
        /// </summary>
        /// <returns>Return The local host name.</returns>
        public static string GetLocalHostName()
        {
            string hostName = string.Empty;
            try
            {
                hostName = Environment.MachineName;
            }
            catch { hostName = "localhost"; }
            return hostName;
        }
        /// <summary>
        /// Get Host Address
        /// </summary>
        /// <param name="hostName">The host name to solve IPAddress.</param>
        /// <returns>Return List of IPAddress on specificed host.</returns>
        public static IPAddress[] GetHostAddress(string hostName)
        {
            IPHostEntry entry = null;
            try
            {
                entry = Dns.GetHostEntry(hostName);
            }
            catch //(Exception ex)
            {
                // no such host known.
                entry = null;
            }

            if (entry != null && entry.AddressList != null)
            {
                return entry.AddressList;
            }
            else return null;
        }
        /// <summary>
        /// Gets all active network adaptors not include loopback adaptor.
        /// </summary>
        /// <returns>Returns array of network interface adaptors.</returns>
        public static NetworkInterface[] GetActiveNetworkInterfaces()
        {
            List<NetworkInterface> Interfaces = new List<NetworkInterface>();

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    Interfaces.Add(nic);
                }
            }

            return Interfaces.ToArray();
        }
        /// <summary>
        /// Gets current active network adaptor not include loopback adaptor.
        /// </summary>
        /// <returns>Returns current active network interface adaptor.</returns>
        public static NetworkInterface GetActiveInterface()
        {
            List<NetworkInterface> Interfaces = new List<NetworkInterface>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    Interfaces.Add(nic);
                }
            }

            NetworkInterface result = null;
            foreach (NetworkInterface nic in Interfaces)
            {
                if (result == null)
                {
                    result = nic;
                }
                else
                {
                    if (nic.GetIPProperties().GetIPv4Properties() != null)
                    {
                        if (nic.GetIPProperties().GetIPv4Properties().Index <
                            result.GetIPProperties().GetIPv4Properties().Index)
                            result = nic;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Gets IP Address from active network adaptor.
        /// </summary>
        /// <returns>Returns IP Address of current active network interface adaptor.</returns>
        public static IPAddress GetLocalIPAddress()
        {
            return GetLocalIPAddress(GetActiveInterface());
        }
        /// <summary>
        /// Gets IP Address from specificed network adaptor.
        /// </summary>
        /// <param name="nic">The network interface adaptor.</param>
        /// <returns>Returns IP Address of specificed network interface adaptor.</returns>
        public static IPAddress GetLocalIPAddress(NetworkInterface nic)
        {
            IPAddress result = null;
            if (null == nic)
                return result;
            IPInterfaceProperties ipProps = nic.GetIPProperties();
            foreach (UnicastIPAddressInformation uni in ipProps.UnicastAddresses)
            {
                if (uni != null && uni.Address != null &&
                    uni.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    // valid
                    result = uni.Address;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// Gets the IP address provide by ISP that used in internet.
        /// When call the respose speed is depend on the internet connect speed so when call
        /// UI may be freeze until the response is returns.
        /// </summary>
        /// <returns>Returns external IP address that used in internet.</returns>
        public static IPAddress GetExternalIPAddress()
        {
            IPAddress result = null;
            try
            {
                // Use a web page that displays the IP of the request.  In this case,
                // I use network-tools.com.  This page has been around for years
                // and is always up when I have tried it.  You could use others or
                // your own. 
                WebRequest myRequest = WebRequest.Create("http://checkip.dyndns.org/");

                // Send request, get response, and parse out the IP address on the page.
                using (WebResponse res = myRequest.GetResponse())
                {
                    using (Stream s = res.GetResponseStream())
                    {
                        using (StreamReader sr =
                            new StreamReader(s, System.Text.Encoding.UTF8))
                        {
                            string html = sr.ReadToEnd();
                            Regex regex =
                                new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                            string ipString = regex.Match(html).Value;

                            //Console.WriteLine("Public IP: " + ipString);
                            if (!IPAddress.TryParse(ipString, out result))
                            {
                                result = null;
                            }
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
                //Console.WriteLine("Error getting IP Address:\n" + ex.Message);
                result = null;
            }

            return result;
        }

        #endregion
    }
}