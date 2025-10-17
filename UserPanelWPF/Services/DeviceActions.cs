using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserPanelWPF.Models;
using static UserPanelWPF.Models.Enumerators;


namespace UserPanelWPF.Services
{
    public static class DeviceActions
    {
        //public static bool RunningState { get; private set; } = false;
        public static async Task<string> ToggleArmedState()
        {
            int userPanelAction = (int)DeviceAction.ArmDevice;
            string logMessage = await VDClientService.SetVDAsync(0, userPanelAction);
            return logMessage;

            //VibrationDetector.AlarmArmed = !VibrationDetector.AlarmArmed;
        }
        public static async Task<string> ToggleTriggedState()
        {
            int userPanelAction = (int)DeviceAction.TriggerDevice;
            string logMessage = await VDClientService.SetVDAsync(0, userPanelAction);
            return logMessage;

            //VibrationDetector.AlarmTriggered = !VibrationDetector.AlarmTriggered;
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

        public async static Task<string> SetThresholdLevel(double sliderValue)
        {
            int userPanelAction = (int)DeviceAction.SetThreshold;
            string logMessage = await VDClientService.SetVDAsync(sliderValue,userPanelAction);
            return logMessage;
        }
    }
}
