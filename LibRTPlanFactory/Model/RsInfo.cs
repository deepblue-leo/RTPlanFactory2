using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public class RsInfo:InfoBase
    {        
        public List<string> ReferencedCtImgSopInstanceUIDs { get; set; }

        public override string ToString()
        {
            return string.Format("Find A RT Structure Set that SopInstanceUid = {0}, PatientId = {1}, ReferencedCtImageCount = {2}",
                this.SopInstanceUID, this.PatientId, this.ReferencedCtImgSopInstanceUIDs.Count);
        }
    }
}
