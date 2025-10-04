using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserPanelWPF.Models;


namespace UserPanelWPF.Services
{
    public static class DeviceActions
    {
        //public static bool RunningState { get; private set; } = false;
        public static void ToggleArmedState()
        {
            VibrationDetector.AlarmArmed = !VibrationDetector.AlarmArmed;
        }
        public static void ToggleTriggedState()
        {
            VibrationDetector.AlarmTriggered = !VibrationDetector.AlarmTriggered;
        }
        public static string GetDeviceName()
        {
            return VibrationDetector.DeviceName;
        }
        public static bool GetArmedState()
        {
            return VibrationDetector.AlarmArmed;
        }
        public static bool GetTriggedState()
        {
            return VibrationDetector.AlarmTriggered;
        }

        public async static Task<string> SetVibrationLevel()
        {
            string someMessage = await VDClientService.SetVDAsync();
            return someMessage;
        }
    }
}
