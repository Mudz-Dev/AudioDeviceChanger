using AudioDeviceChanger.Data;
using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioDeviceChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            LoadDevices();
        }

        protected void LoadDevices()
        {
            var defDefault = new CoreAudioController().GetDefaultDevice(AudioSwitcher.AudioApi.DeviceType.Playback, AudioSwitcher.AudioApi.Role.Multimedia);

            lstOutputDevices.ItemsSource = new CoreAudioController().GetPlaybackDevices().Where(x => x.State == AudioSwitcher.AudioApi.DeviceState.Active);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedDevice = lstOutputDevices.SelectedItem as CoreAudioDevice;
            if (selectedDevice != null)
            {
                
                selectedDevice.SetAsDefault();
                lstOutputDevices.ItemsSource = new CoreAudioController().GetPlaybackDevices().Where(x => x.State == AudioSwitcher.AudioApi.DeviceState.Active);
            }

        }
    }
}
