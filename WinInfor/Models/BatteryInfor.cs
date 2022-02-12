using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinInfor
{
    internal class BatteryInfor
    {
        public string BatteryLifeRemaining, BatteryLifePercent, PowerStatus, WearLevel = "0", DesignedCapacity = "Cannot identify", Health;
        PowerStatus status = SystemInformation.PowerStatus;
        public BatteryInfor()
        {
            DesignedCapacity = get_DesignedCapacity();
            if (DesignedCapacity == "Cannot identify")
            {
                BatteryLifeRemaining = "Unlimited";
                BatteryLifePercent = "100%";
                PowerStatus = get_PowerStatus();
                WearLevel = "0%";
                Health = "Unknown";
            }
            else
            {
                BatteryLifeRemaining = get_BatteryLifeRemaining();
                BatteryLifePercent = get_BatteryLifePercent();
                PowerStatus = get_PowerStatus();
                Health = get_Health();
                WearLevel = String.Format("About {0}%", get_WearLevel());
                DesignedCapacity += " Wh";
            }
        }
        string get_BatteryLifeRemaining()
        {
            try
            {
                
                if (status.BatteryLifeRemaining != -1)
                {
                    if (status.BatteryLifeRemaining / 3600 > 0)
                    {
                        return String.Format("{0} hours and {1} minutes", status.BatteryLifeRemaining / 3600, status.BatteryLifeRemaining / 60 - (status.BatteryLifeRemaining / 3600) * 60);
                    }
                    else
                    {
                        return String.Format("About {0} minutes", status.BatteryLifeRemaining / 60);
                    }
                }
                return "Unlimited";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot estimate battery life remaining.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot estimate";
        }
        string get_BatteryLifePercent()
        {
            try
            {
                if (status.BatteryLifePercent < 1)
                {
                    return String.Format("About {0}%", status.BatteryLifePercent * 100);
                }
                return "100%";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot estimate battery life percent.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot estimate";
        }
        string get_PowerStatus()
        {
            try
            {
                if (status.PowerLineStatus == PowerLineStatus.Offline)
                {
                    return "Running on battery";
                }
                else if (status.PowerLineStatus == PowerLineStatus.Online)
                {
                    return "Plugged in";
                }
                return "Cannot identify";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify power status.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Cannot identify";
        }
        string get_WearLevel()
        {
            try
            {
                
                double res = 0;
                ManagementObjectSearcher mos = new ManagementObjectSearcher(@"\\localhost\root\wmi", "Select FullChargedCapacity From BatteryFullChargedCapacity");
                foreach (ManagementObject mo in mos.Get())
                {
                    res = double.Parse(mo["FullChargedCapacity"].ToString()) / double.Parse(DesignedCapacity) / 1000 * 100;
                    break;
                }
                return Math.Round(100 - res).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot estimate battery weal level.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "0";
        }
        string get_DesignedCapacity()
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher(@"\\localhost\root\wmi", "Select DesignedCapacity From BatteryStaticData");
                foreach (ManagementObject mo in mos.Get())
                {
                    double res = double.Parse(mo["DesignedCapacity"].ToString());
                    return Math.Round(res / 1000).ToString();
                }
                return "Cannot identify";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify designed capacity of the battery.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Cannot identify";
            }
        }
        string get_Health()
        {
            try
            {
                if (double.Parse(WearLevel) <= 15)
                {
                    return "Excellent";
                }
                else if (double.Parse(WearLevel) > 15 && double.Parse(WearLevel) < 40)
                {
                    return "Good";
                }
                return "Bad";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot identify battery health.\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Unknown";
        }
    }
}
