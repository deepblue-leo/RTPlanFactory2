using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    class RtImgInfo:InfoBase
    {        
        public List<string> ReferencedRpSopInstanceUIDs { get; set; }

        public override string ToString()
        {
            return string.Format("Find A RT Image that SopInstanceUid = {0}, PatientId = {1}",
                this.SopInstanceUID, this.PatientId);
        }
    }
}
