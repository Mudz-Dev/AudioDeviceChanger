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
        private List<CoreAudioDevice> _captureDevices;
        private CoreAudioController? _audioController;

        public int DefaultPlaybackDeviceIndex { get; set; }
        public int DefaultCaptureDeviceIndex { get; set; }

        public CoreAudioController AudioController
        {
            get
            {
                if (_audioController == null) _audioController = new CoreAudioController();
                return _audioController;
            }
            set
            {
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

        public List<CoreAudioDevice> CaptureDevices
        {
            get
            {
                return _captureDevices;
            }
            set
            {
                _captureDevices = value;
            }
        }

        public CoreAudioDevice DefaultPlaybackDevice
        {
            get
            {
                return AudioController.DefaultPlaybackDevice;
            }
        }

        public CoreAudioDevice DefaultCaptureDevice
        {
            get
            {
                return AudioController.DefaultCaptureDevice;
            }
        }

        public AudioDevices()
        {
            LoadPlaybackDevices();
            LoadCaptureDevices();
        }

        #region Playback Devices

        public void LoadPlaybackDevices()
        {
            PlaybackDevices = GetPlaybackDevices();
        }

        public List<CoreAudioDevice> GetPlaybackDevices()
        {
            List<CoreAudioDevice> devices = new List<CoreAudioDevice>();
            int count = 0;
            foreach (CoreAudioDevice device in AudioController.GetPlaybackDevices().Where(x => x.State == AudioSwitcher.AudioApi.DeviceState.Active))
            {
                if (device.IsDefaultDevice)
                {
                    DefaultPlaybackDeviceIndex = count;
                }

                devices.Add(device);

                count++;
            }

            return devices;

        }

        public void IncrementPlaybackDevice()
        {
            CoreAudioDevice device = null;
            if (DefaultPlaybackDeviceIndex == PlaybackDevices.Count - 1)
            {
                DefaultPlaybackDeviceIndex = 0;
            }
            else
            {
                DefaultPlaybackDeviceIndex++;
            }

            device = PlaybackDevices[DefaultPlaybackDeviceIndex];
            device.SetAsDefault();
        }

        public void DecrementPlaybackDevice()
        {
            CoreAudioDevice device = null;
            if (DefaultPlaybackDeviceIndex <= 0)
            {
                DefaultPlaybackDeviceIndex = PlaybackDevices.Count - 1;
            }
            else
            {
                DefaultPlaybackDeviceIndex--;
            }

            device = PlaybackDevices[DefaultPlaybackDeviceIndex];
            device.SetAsDefault();
        }

        #endregion

        #region Capture Devices

        public void LoadCaptureDevices()
        {
            CaptureDevices = GetCaptureDevices();
        }

        public List<CoreAudioDevice> GetCaptureDevices()
        {
            List<CoreAudioDevice> devices = new List<CoreAudioDevice>();
            int count = 0;
            foreach (CoreAudioDevice device in AudioController.GetCaptureDevices().Where(x => x.State == AudioSwitcher.AudioApi.DeviceState.Active))
            {
                if (device.IsDefaultDevice)
                {
                    DefaultCaptureDeviceIndex = count;
                }

                devices.Add(device);

                count++;
            }

            return devices;


        }


        public void IncrementCaptureDevice()
        {
            CoreAudioDevice device = null;
            if (DefaultCaptureDeviceIndex == CaptureDevices.Count - 1)
            {
                DefaultCaptureDeviceIndex = 0;
            }
            else
            {
                DefaultCaptureDeviceIndex++;
            }

            device = CaptureDevices[DefaultCaptureDeviceIndex];
            device.SetAsDefault();
        }

        public void DecrementCaptureDevice()
        {
            CoreAudioDevice device = null;
            if (DefaultCaptureDeviceIndex <= 0)
            {
                DefaultCaptureDeviceIndex = CaptureDevices.Count - 1;
            }
            else
            {
                DefaultCaptureDeviceIndex--;
            }

            device = CaptureDevices[DefaultCaptureDeviceIndex];
            device.SetAsDefault();
        }
        #endregion
    }
}
