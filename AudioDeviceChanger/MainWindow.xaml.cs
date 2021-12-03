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

namespace AudioDeviceChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int ADDHOTKEY_ID = 9000;
        private const int SUBHOTKEY_ID = 9001;

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

        AudioDevices Audio { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Audio = new AudioDevices();
        }


        protected void RefreshDevices()
        {
            Audio.LoadPlaybackDevices();
            lstOutputDevices.ItemsSource = Audio.PlaybackDevices;
            lstOutputDevices.SelectedItem = Audio.DefaultPlaybackDevice;
            lstOutputDevices.SelectionMode = SelectionMode.Single;

        }

        protected void SetDefaultDevice()
        {
            var selectedDevice = lstOutputDevices.SelectedItem as CoreAudioDevice;
            if (selectedDevice != null)
            {

                selectedDevice.SetAsDefault();
                RefreshDevices();
            }
        }


        private void lstOutputDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDefaultDevice();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Audio.IncrementPlaybackDevice();
            RefreshDevices();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Audio.DecrementPlaybackDevice();
            RefreshDevices();
        }

        private IntPtr _windowHandle;
        private HwndSource _source;
        private void Window_SourceInitialized(object sender, EventArgs e)
        {

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, ADDHOTKEY_ID, MOD_CONTROL, VK_ADD); //CTRL + ADD
            RegisterHotKey(_windowHandle, SUBHOTKEY_ID, MOD_CONTROL, VK_SUB); //CTRL + SUB

            RefreshDevices();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, ADDHOTKEY_ID);
            UnregisterHotKey(_windowHandle, SUBHOTKEY_ID);
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
                        case ADDHOTKEY_ID:
                            vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_ADD)
                            {
                                Console.WriteLine("Add Key Combo Pressed");
                                Audio.IncrementPlaybackDevice();
                                RefreshDevices();
                            }
                            handled = true;
                            break;
                        case SUBHOTKEY_ID:
                            vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_SUB)
                            {
                                Console.WriteLine("Subtract Key Combo Pressed");
                                Audio.DecrementPlaybackDevice();
                                RefreshDevices();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
