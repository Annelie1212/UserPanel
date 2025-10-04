using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPanelWPF.Models
{
    public class VDChangeValueResponse
    {
        public int VibrationDetectorId { get; set; }
        public bool RequestSuccessful { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;


    }
}
