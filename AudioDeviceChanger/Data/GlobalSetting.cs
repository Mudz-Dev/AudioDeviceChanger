using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioDeviceChanger.Data
{
    public class GlobalSetting
    {

        public bool MinimizeToTray { get; set; }
        public bool RunWhenPCStarts { get; set; }

        public GlobalSetting()
        {
            MinimizeToTray = true;
            RunWhenPCStarts = true;
        }


    }
}
