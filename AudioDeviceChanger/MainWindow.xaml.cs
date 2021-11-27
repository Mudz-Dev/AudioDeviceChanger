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

namespace AudioDeviceChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AudioDevices Audio { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Audio = new AudioDevices();
            RefreshDevices();
        }


        protected void RefreshDevices()
        {
            Audio.LoadPlaybackDevices();
            lstOutputDevices.ItemsSource = Audio.PlaybackDevices;
            lstOutputDevices.SelectedItem = Audio.DefaultPlaybackDevice;  

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
    }
}
