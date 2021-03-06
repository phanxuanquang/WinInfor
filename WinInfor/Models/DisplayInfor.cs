using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinInfor
{
    internal class DisplayInfor
    {
        public string Resolution, RefreshRate, Brightness, scale, NightLight, HDRforPlayback;
        public DisplayInfor(string BatteryDesignedCapacity)
        {
            RefreshRate = get_RefreshRate();
            Resolution = get_Resolution();
            scale = get_Scale() + "%";
            NightLight = get_NightLightStatus();
            HDRforPlayback = get_HDRforPlayback();

            if (BatteryDesignedCapacity != "Cannot identify")
            {
                Brightness = get_Brightness().ToString() + "%";
            }
            else
            {
                Brightness = "100%";
            }
        }
        private enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }
        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);
        private static void SetDpiAwareness()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
            }
        }

        string get_Resolution()
        {
            try
            {
                SetDpiAwareness();
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;
                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                return String.Format("{0}  x {1}", screenWidth, screenHeight); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot detect resolution of the monitor.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
        string get_RefreshRate()
        {
            int RefreshRatecount = 1;
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT CurrentRefreshRate FROM Win32_VideoController");
                foreach (ManagementObject mo in mos.Get())
                {
                    if (mo["CurrentRefreshRate"] != null)
                    {
                        RefreshRatecount++;
                    }
                    if (mo["CurrentRefreshRate"] != null && RefreshRatecount > 1 && mo["CurrentRefreshRate"].ToString() != "1")
                    {
                        return mo["CurrentRefreshRate"].ToString() + "Hz"; 
                    }
                }
                return "Cannot identify";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dedicated graphic card information not found.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
        int get_Brightness()
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher(@"\\.\root\wmi", "Select CurrentBrightness From WmiMonitorBrightness");
                foreach (ManagementObject mo in mos.Get())
                {
                    return (byte)mo["CurrentBrightness"];
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify brightness level.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 0;
        }
        double get_Scale()
        {
            var currentDPI = (int)Registry.GetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop", "LogPixels", 96);
            double scale = Math.Round((double)currentDPI / 96 * 100, 0);
            return scale;
        }
        string get_NightLightStatus()
        {
            try
            {
                const string BlueLightReductionStateKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CloudStore\Store\DefaultAccount\Current\default$windows.data.bluelightreduction.bluelightreductionstate\windows.data.bluelightreduction.bluelightreductionstate";
                using (var key = Registry.CurrentUser.OpenSubKey(BlueLightReductionStateKey))
                {
                    var data = key?.GetValue("Data");
                    if (data is null)
                        return "Disable";
                    var byteData = (byte[])data;
                    if (byteData.Length > 24 && byteData[23] == 0x10 && byteData[24] == 0x00)
                        return "Enable";
                    return "Disable";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify Night Light status.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
        string get_HDRforPlayback()
        {
            try
            {
                var key = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\VideoSettings", "EnableHDRForPlayback", null);
                if (key != null)
                {
                    if (key.ToString() == "1")
                        return "Enable";
                }
                return "Disable";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify HDR support.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
    }
}
