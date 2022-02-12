using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WinInfor.Program;

namespace WinInfor
{
    public partial class MainUI : Form
    {
        BatteryInfor battery = new BatteryInfor();
        public MainUI()
        {
            InitializeComponent();
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            //ShadowForm.SetShadowForm(this);
            this.Icon = Properties.Resources.icon;
        }
        // Anti flicker
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; 
                return handleParam;
            }
        }

        private void HomeTab_Load(object sender, EventArgs e)
        {
            this.DesignedCapacity.Text = battery.DesignedCapacity;
            LoadDataFrom(new SystemInfor(), battery, new DisplayInfor(this.DesignedCapacity.Text), new WindowsInfor());
        }
        private void LoadDataFrom(SystemInfor systemInfor, BatteryInfor batteryInfor, DisplayInfor displayInfor, WindowsInfor windowsInfor)
        {
            this.SystemModel.Text = systemInfor.SystemModel;
            this.OperatingSystem.Text = systemInfor.OperatingSystem;
            this.CPU.Text = systemInfor.CPU;
            this.GraphicCard.Text = systemInfor.GraphicCard;
            this.RAM.Text = systemInfor.RAM;
            this.HardDiscSpace.Text = systemInfor.HardDiscSpace;

            //this.DesignedCapacity.Text = batteryInfor.DesignedCapacity;
            this.WearLevel.Text = batteryInfor.WearLevel;
            this.BatteryLifeRemaining.Text = batteryInfor.BatteryLifeRemaining;
            this.PowerStatus.Text = batteryInfor.PowerStatus;
            this.CurrentPercent.Text = batteryInfor.BatteryLifePercent;
            this.Health.Text = batteryInfor.Health;

            this.Resolution.Text = displayInfor.Resolution;
            this.RefreshRate.Text = displayInfor.RefreshRate;
            this.Brightness.Text = displayInfor.Brightness;
            this.ScreenScale.Text = displayInfor.scale;
            this.NightLight.Text = displayInfor.NightLight;
            this.HDR.Text = displayInfor.HDRforPlayback;

            this.ComputerName.Text = windowsInfor.ComputerName;
            this.WindowsVersion.Text = windowsInfor.Version;
            this.Architechture.Text = windowsInfor.Architechture;
            this.Activation.Text = windowsInfor.Activation;
            this.DefenderStatus.Text = windowsInfor.Defender;
        }
        private void ReLoadDataFrom(BatteryInfor batteryInfor, DisplayInfor displayInfor, WindowsInfor windowsInfor)
        {
            this.BatteryLifeRemaining.Text = batteryInfor.BatteryLifeRemaining;
            this.PowerStatus.Text = batteryInfor.PowerStatus;
            this.CurrentPercent.Text = batteryInfor.BatteryLifePercent;

            this.Resolution.Text = displayInfor.Resolution;
            this.RefreshRate.Text = displayInfor.RefreshRate;
            this.Brightness.Text = displayInfor.Brightness;
            this.NightLight.Text = displayInfor.NightLight;
            this.HDR.Text = displayInfor.HDRforPlayback;

            this.ComputerName.Text = windowsInfor.ComputerName;
            this.DefenderStatus.Text = windowsInfor.Defender;
        }

        #region Button Events
        private void CheckUpdateButton_Click(object sender, EventArgs e)
        {
            Program.runCommand_Advanced("explorer ms-settings:windowsupdate");
        }
        private void Activation_Click(object sender, EventArgs e)
        {
            Program.runCommand_Advanced("explorer ms-settings:activation");
        }
        private void RefreshInformation_Button_Click(object sender, EventArgs e)
        {
            ReLoadDataFrom(new BatteryInfor(), new DisplayInfor(this.DesignedCapacity.Text), new WindowsInfor());
        }
        private void GithubButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/phanxuanquang/WinInfor");
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }
        #endregion
    }
}
