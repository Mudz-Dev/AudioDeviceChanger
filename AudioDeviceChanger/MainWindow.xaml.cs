using AudioDeviceChanger.Data;
using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Hardcodet.Wpf.TaskbarNotification;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;

namespace AudioDeviceChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TaskbarIcon tb { get; set; }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int ADDPBHOTKEY_ID = 9000;
        private const int SUBPBHOTKEY_ID = 9001;
        private const int ADDCHOTKEY_ID = 9002;
        private const int SUBCHOTKEY_ID = 9003;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        private const uint MOD_WIN = 0x0008; //WINDOWS
        
        //Add Key:
        private const uint VK_ADD = 0x6B;

        //Subtract Key:
        private const uint VK_SUB = 0x6D;

        protected string SettingsPath
        {
            get
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AudioChanger");
            }
        }
        protected string SettingsFilename
        {
            get
            {
                return System.IO.Path.Combine(SettingsPath, "AppSettings.json");
            }
        }

        AudioDevices Audio { get; set; }
        GlobalSetting Settings { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Audio = new AudioDevices();
            tb = (TaskbarIcon)FindResource("MyNotifyIcon");
            InitializeAppSettings();
        }

        protected void InitializeAppSettings()
        {
            if (File.Exists(SettingsFilename)) {
                string appFile = File.ReadAllText(SettingsFilename);
                Settings = JsonConvert.DeserializeObject<GlobalSetting>(appFile);
            }
            else
            {
                Settings = new GlobalSetting();
            }

            SetStartup();

        }
        protected void LoadSettings()
        {

            tglMinimize.IsChecked = Settings.MinimizeToTray;
            tglRunOnPCStart.IsChecked = Settings.RunWhenPCStarts;

        }
        protected void SaveSettings()
        {
            if (!(Directory.Exists(SettingsPath))) {
                Directory.CreateDirectory(SettingsPath);
            }

            Settings.MinimizeToTray = tglMinimize.IsChecked ?? true;
            Settings.RunWhenPCStarts = tglRunOnPCStart.IsChecked ?? true;

            string appFile = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(SettingsFilename, appFile);
        }

        protected void RefreshDevices()
        {
            LoadOutputDevices();
            LoadInputDevices();

        }

        protected void LoadOutputDevices()
        {
            Audio.LoadPlaybackDevices();
            lstOutputDevices.ItemsSource = Audio.PlaybackDevices;
            lstOutputDevices.SelectedItem = Audio.DefaultPlaybackDevice;
            lstOutputDevices.SelectionMode = SelectionMode.Single;
        }

        protected void LoadInputDevices()
        {
            Audio.LoadCaptureDevices();
            lstInputDevices.ItemsSource = Audio.CaptureDevices;
            lstInputDevices.SelectedItem = Audio.DefaultCaptureDevice;
            lstInputDevices.SelectionMode = SelectionMode.Single;
        }

        protected void SetDefaultOutputDevice()
        {
            var selectedDevice = lstOutputDevices.SelectedItem as CoreAudioDevice;
            if (selectedDevice != null)
            {

                selectedDevice.SetAsDefault();
                RefreshDevices();
            }
        }

        protected void SetDefaultInputDevice()
        {
            var selectedDevice = lstInputDevices.SelectedItem as CoreAudioDevice;
            if (selectedDevice != null)
            {

                selectedDevice.SetAsDefault();
                RefreshDevices();
            }
        }

        private void lstOutputDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDefaultOutputDevice();
        }

        private void IncrementOutput()
        {
            Audio.IncrementPlaybackDevice();
            RefreshDevices();
            ShowNotification("Output Device Changed", Audio.DefaultPlaybackDevice.FullName);
        }

        private void DecrementOutput()
        {
            Audio.DecrementPlaybackDevice();
            RefreshDevices();
            ShowNotification("Output Device Changed", Audio.DefaultPlaybackDevice.FullName);
        }

        private void IncrementInput()
        {
            Audio.IncrementCaptureDevice();
            RefreshDevices();
            ShowNotification("Input Device Changed", Audio.DefaultCaptureDevice.FullName);
        }

        private void DecrementInput()
        {
            Audio.DecrementCaptureDevice();
            RefreshDevices();
            ShowNotification("Input Device Changed", Audio.DefaultCaptureDevice.FullName);
        }

        private void ShowNotification(string title, string message)
        {
            var currentApp = Application.Current as App;

            if (currentApp != null)
                currentApp.tb.ShowBalloonTip(title, message, BalloonIcon.Info);
        }

        private IntPtr _windowHandle;
        private HwndSource _source;
        private void Window_SourceInitialized(object sender, EventArgs e)
        {

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, ADDPBHOTKEY_ID, MOD_CONTROL, VK_ADD); //CTRL + ADD
            RegisterHotKey(_windowHandle, SUBPBHOTKEY_ID, MOD_CONTROL, VK_SUB); //CTRL + SUB

            RegisterHotKey(_windowHandle, ADDCHOTKEY_ID, MOD_ALT, VK_ADD); //ALT + ADD
            RegisterHotKey(_windowHandle, SUBCHOTKEY_ID, MOD_ALT, VK_SUB); //ALT + SUB

            RefreshDevices();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, ADDPBHOTKEY_ID);
            UnregisterHotKey(_windowHandle, SUBPBHOTKEY_ID);
            UnregisterHotKey(_windowHandle, ADDCHOTKEY_ID);
            UnregisterHotKey(_windowHandle, SUBCHOTKEY_ID);
            base.OnClosed(e);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int vkey;
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case ADDPBHOTKEY_ID:
                            vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_ADD)
                            {
                                Console.WriteLine("Add Key Combo Pressed");
                                IncrementOutput();
                            }
                            handled = true;
                            break;
                        case SUBPBHOTKEY_ID:
                            vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_SUB)
                            {
                                Console.WriteLine("Subtract Key Combo Pressed");
                                DecrementOutput();
                            }
                            handled = true;
                            break;
                        case ADDCHOTKEY_ID:
                            vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_ADD)
                            {
                                Console.WriteLine("Add Key Combo Pressed");
                                IncrementInput();
                            }
                            handled = true;
                            break;
                        case SUBCHOTKEY_ID:
                            vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_SUB)
                            {
                                Console.WriteLine("Subtract Key Combo Pressed");
                                DecrementInput();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void SetStartup()
        {

            string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(runKey);


            if (Settings.RunWhenPCStarts)
            {
                rk.Close();
                rk = Registry.CurrentUser.OpenSubKey(runKey, true);
                string executableStr = Process.GetCurrentProcess().MainModule.FileName;
                rk.SetValue("AudioChanger", executableStr);
                rk.Close();
            }
            else
            {
                rk = Registry.CurrentUser.OpenSubKey(runKey, true);
                rk.DeleteValue("AudioChanger", false);
                rk.Close();
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();

            if (Settings.MinimizeToTray)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void lstInputDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDefaultInputDevice();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = new NavigationWindow();
            window.Source = new Uri("Settings.xaml", UriKind.Relative);
            window.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void DrawerHost_DrawerOpened(object sender, MaterialDesignThemes.Wpf.DrawerOpenedEventArgs e)
        {
            LoadSettings();
        }

        private void tglRunOnPCStart_Checked(object sender, RoutedEventArgs e)
        {
            Settings.RunWhenPCStarts = true;
            SetStartup();
        }

        private void tglRunOnPCStart_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.RunWhenPCStarts = false;
            SetStartup();
        }
    }
}
