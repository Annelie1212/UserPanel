using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserPanelWPF.Services;

namespace UserPanelWPF.Models
{
    public static class VibrationDetector
    {
        public static int VibrationDetectorId { get; set; }
        public static string DeviceName { get; set; } = "Vibration Detector";
        public static string Location { get; set; }

        //Larmad eller inte larmad
        public static bool AlarmArmed { get; set; } = false;

        //Alarm utlöst eller inte
        public static bool AlarmTriggered { get; set; } = false;

        public static int VibrationLevel { get; set; }

        public static int VibrationLevelThreshold { get; set; }

        public static void Btn_Armed()
        {
            DeviceActions.ToggleArmedState();

            if (DeviceActions.GetTriggedState() == true)
            {
                DeviceActions.ToggleTriggedState();
            }

        }

        public static void Btn_Trigged()
        {
            if (!DeviceActions.GetArmedState())
            {
                //make sure the button does nothing if the device is not armed
            }
            else
            {
                DeviceActions.ToggleTriggedState();
            }
        }


        public static void Update(VDFetchStatusResponse statusResponse)
        {
            VibrationDetectorId = statusResponse.VibrationDetectorId;
            DeviceName = statusResponse.DeviceName;
            Location = statusResponse.Location;
            AlarmArmed = statusResponse.AlarmArmed;
            AlarmTriggered = statusResponse.AlarmTriggered;
            VibrationLevel = statusResponse.VibrationLevel;
            VibrationLevelThreshold = statusResponse.VibrationLevelThreshold;

        }
    }
}
