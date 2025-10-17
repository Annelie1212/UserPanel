using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPanelWPF.Models
{
    public class Enumerators
    {
        public enum DeviceAction
        {
            ArmDevice = 0,
            TriggerDevice = 1,
            SetThreshold = 2,
            Error = 99
        }
    }
}
