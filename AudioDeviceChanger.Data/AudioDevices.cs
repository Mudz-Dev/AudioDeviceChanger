using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AudioDeviceChanger.Data
{
    public class AudioDevices
    {
        private List<CoreAudioDevice> _playbackDevices;
        private CoreAudioController? _audioController;

        public int DefaultDeviceIndex { get; set; }

        public CoreAudioController AudioController
        {
            get {
                if (_audioController == null) _audioController = new CoreAudioController();
                return _audioController;
            }
            set { 
                _audioController = value; 
            }
        }

        public List<CoreAudioDevice> PlaybackDevices
        {
            get
            {
                return _playbackDevices;
            }
            set
            {
                _playbackDevices = value;
            }
        }

        public CoreAudioDevice DefaultPlaybackDevice
        {
            get
            {
                return AudioController.DefaultPlaybackDevice;
            }
        }

        public AudioDevices()
        {
            LoadPlaybackDevices();      
        }

        public void LoadPlaybackDevices()
        {
            PlaybackDevices = GetPlaybackDevices();
        }

        public List<CoreAudioDevice> GetPlaybackDevices()
        {
            List<CoreAudioDevice> devices = new List<CoreAudioDevice>();
            int count = 0;
            foreach(CoreAudioDevice device in AudioController.GetPlaybackDevices().Where(x => x.State == AudioSwitcher.AudioApi.DeviceState.Active))
            {
                if (device.IsDefaultDevice)
                {
                    DefaultDeviceIndex = count;
                }

                devices.Add(device);

                count++;
            }

            return devices;
            
        }

        public void IncrementPlaybackDevice()
        {
            CoreAudioDevice device = null;
            if (DefaultDeviceIndex ==  PlaybackDevices.Count - 1)
            {
                DefaultDeviceIndex = 0;
            }
            else
            {
                DefaultDeviceIndex++;
            }

            device = PlaybackDevices[DefaultDeviceIndex];
            device.SetAsDefault();
        }

        public void DecrementPlaybackDevice()
        {
            CoreAudioDevice device = null;
            if (DefaultDeviceIndex <= 0)
            {
                DefaultDeviceIndex = PlaybackDevices.Count - 1;
            }
            else
            {
                DefaultDeviceIndex--;
            }

            device = PlaybackDevices[DefaultDeviceIndex];
            device.SetAsDefault();
        }

    }
}
