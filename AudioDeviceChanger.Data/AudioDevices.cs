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

        private static CoreAudioController? _audioController;

        public static int DefaultDeviceIndex = -1;

        public static CoreAudioController AudioController
        {
            get {
                if (_audioController == null) _audioController = new CoreAudioController();
                return _audioController;
            }
            set { 
                _audioController = value; 
            }
        }

        public static IEnumerable<CoreAudioDevice> GetPlaybackDevices()
        {
            return AudioController.GetPlaybackDevices().Where(x => x.State == AudioSwitcher.AudioApi.DeviceState.Active);
        }

    }
}
