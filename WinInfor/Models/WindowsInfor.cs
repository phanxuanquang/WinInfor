using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinInfor
{
    using SLID = Guid;
    internal class WindowsInfor
    {
        public string ComputerName, Version, WindowsArchitechture, Activation, Defender, InstallTime;       
        public WindowsInfor()
        {
            ComputerName = Environment.MachineName.ToString();
            Version = Environment.OSVersion.Version.ToString();
            WindowsArchitechture = RuntimeInformation.OSArchitecture.ToString().ToUpper();
            Activation = get_Activation();
            Defender = get_DefenderStatus();
            InstallTime = get_InstallTime();
        }

        public enum SL_GENUINE_STATE
        {
            SL_GEN_STATE_IS_GENUINE = 0,
            SL_GEN_STATE_INVALID_LICENSE = 1,
            SL_GEN_STATE_TAMPERED = 2,
            SL_GEN_STATE_OFFLINE = 3,
            SL_GEN_STATE_LAST = 4
        }
        [DllImportAttribute("Slwga.dll", EntryPoint = "SLIsGenuineLocal", CharSet = CharSet.None, ExactSpelling = false, SetLastError = false, PreserveSig = true, CallingConvention = CallingConvention.Winapi, BestFitMapping = false, ThrowOnUnmappableChar = false)]
        [PreserveSigAttribute()]
        static extern uint SLIsGenuineLocal(ref SLID slid, [In, Out] ref SL_GENUINE_STATE genuineState, IntPtr val3);
        string get_Activation()
        {
            bool _IsGenuineWindows = false;
            Guid ApplicationID = new Guid("55c92734-d682-4d71-983e-d6ec3f16059f");
            SLID windowsSlid = (Guid)ApplicationID;
            try
            {
                SL_GENUINE_STATE genuineState = SL_GENUINE_STATE.SL_GEN_STATE_LAST;
                uint ResultInt = SLIsGenuineLocal(ref windowsSlid, ref genuineState, IntPtr.Zero);
                if (ResultInt == 0)
                {
                    _IsGenuineWindows = (genuineState == SL_GENUINE_STATE.SL_GEN_STATE_IS_GENUINE);
                }
                else
                {
                    MessageBox.Show("Error getting activation information {0}", ResultInt.ToString());
                }
                if (_IsGenuineWindows)
                {
                    return "Activated";
                }
                return "Not activated";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Activation status unknown.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
        string get_DefenderStatus()
        {
            try
            {
                bool isDefenderServicesRunning(string[] servicesList)
                {
                    ServiceController sc;
                    foreach (string service in servicesList)
                    {
                        sc = new ServiceController(service);
                        if (sc != null && sc.Status != ServiceControllerStatus.Running)
                        {
                            return false;
                        }
                    }
                    return true;
                }

                string[] sercives = { "WdNisSvc", "WinDefend", "mpssvc" };
                if (isDefenderServicesRunning(sercives))
                {
                    return "On";
                }
                return "Some services are not running";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify Windows Defender status.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
        private string get_InstallTime()
        {
            try
            {
                var key = RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Environment.MachineName, RegistryView.Registry64);

                key = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);
                if (key != null)
                {
                    DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0);
                    object objValue = key.GetValue("InstallDate");
                    string stringValue = objValue.ToString();
                    Int64 regVal = Convert.ToInt64(stringValue);

                    return startDate.AddSeconds(regVal).ToString("HH:mm  on dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Windows installed time unknown.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
        
    }
}
