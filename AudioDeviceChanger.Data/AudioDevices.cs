using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace AudioDeviceChanger.Data
{
    public class AudioDevices
    {

        public static MMDevice GetDefaultOutputDevice()
        {
            var enumerator = new MMDeviceEnumerator();

            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            
            return device;
        }

        public static List<WaveOutCapabilities> GetOutputDevices()
        {
            List<WaveOutCapabilities> devices = new List<WaveOutCapabilities>();

            for(int deviceId = 0; deviceId < WaveOut.DeviceCount; deviceId++)
            {
                var deviceInfo = WaveOut.GetCapabilities(deviceId);
                Console.WriteLine($"{deviceId}: {deviceInfo.ProductName}");
                devices.Add(deviceInfo);
            }

            return devices;
        }

        public static List<WaveInCapabilities> GetInputDevices()
        {

            List<WaveInCapabilities> devices = new List<WaveInCapabilities>();

            for (int deviceId = 0; deviceId < WaveOut.DeviceCount; deviceId++)
            {
                var deviceInfo = WaveIn.GetCapabilities(deviceId);

                devices.Add(deviceInfo);
            }

            return devices;
        }

        public static bool SetDefaultDevice(string id)
        {
            try
            {

                var enumerator = new MMDeviceEnumerator();
                var audioOutputDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(new string[]
                {
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AudioDeviceCmdlets.dll")
                });

                Runspace runspace = RunspaceFactory.CreateRunspace(iss);
                runspace.Open();

                Pipeline pipeline = runspace.CreatePipeline();

                Command command_set = new Command("Set-AudioDevice");
                CommandParameter param_set = new CommandParameter("ID", audioOutputDevice.ID);
                command_set.Parameters.Add(param_set);
                pipeline.Commands.Add(command_set);

                // Execute PowerShell script
                var results = pipeline.Invoke();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
