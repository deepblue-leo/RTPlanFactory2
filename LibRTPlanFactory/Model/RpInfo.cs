using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public class RpInfo
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }        
        public string SopInstanceUID { get; set; }
        public string PlanLabel { get; set; }
        public List<string> ReferencedRsSopInstanceUIDs { get; set; }
        public List<string> ReferencedRdSopInstanceUIDs { get; set; }
    }
}
