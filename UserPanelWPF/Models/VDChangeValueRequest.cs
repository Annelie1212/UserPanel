using System.ComponentModel.DataAnnotations;
using static UserPanelWPF.Models.Enumerators;


namespace UserPanelWPF.Models
{
    public class VDChangeValueRequest
    {
        [Required]
        public int VibrationDetectorId { get; set; }

        [Required]
        public DeviceAction UserPanelAction { get; set; } = (DeviceAction)99;

        [Required]
        public double NewValue { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime UserPanelActionDate { get; set; }


    }
}
