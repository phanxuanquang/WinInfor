using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinInfor
{
    internal class WindowsInfor
    {
        public string ComputerName, Version, Architechture, Activation, Defender;
        public WindowsInfor()
        {
            ComputerName = Environment.MachineName.ToString();
            Version = Environment.OSVersion.Version.ToString();
            Architechture = Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit";
            Activation = get_Activation();
            Defender = get_DefenderStatus();
        }
        string get_Activation()
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject mo in mos.Get())
                {
                    if (mo["SerialNumber"].ToString() != "" || mo["SerialNumber"] != null || mo["SerialNumber"].ToString().Contains("XXXXX") == false)
                    {
                        return "Activated";
                    }
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
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "CMD.exe";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;

                bool isDefenderOn(string[] keyList)
                {
                    foreach (string key in keyList)
                    {
                        p.StartInfo.Arguments = String.Format("/C PowerShell \"Get-MpComputerStatus | select {0}\"", key);
                        p.Start();
                        if (!p.StandardOutput.ReadToEnd().Contains("True"))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                if (isDefenderOn(new string[] { "AntispywareEnabled", "AntivirusEnabled", "IoavProtectionEnabled", "RealTimeProtectionEnabled", "IsTamperProtected" }))
                {
                    return "On";
                }
                return "Off";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify Windows Defender status.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
    }
}
