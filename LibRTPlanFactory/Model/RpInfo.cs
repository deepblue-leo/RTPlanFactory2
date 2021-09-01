using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public class RpInfo:InfoBase
    {        
        public string PlanLabel { get; set; }
        public List<string> ReferencedRsSopInstanceUIDs { get; set; }
        public List<string> ReferencedRdSopInstanceUIDs { get; set; }
        public List<string> TreatmentMachineNames { get; set; } 
        public int BeamCount { get; set; }

        public override string ToString()
        {
            return string.Format("Find A RT Plan that SopInstanceUid = {0}, PatientId = {1}, PlanLabel = {2}",
                this.SopInstanceUID, this.PatientId,this.PlanLabel);
        }
    }
}
