using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public class RdInfo : InfoBase
    {       
        public override string ToString()
        {
            return string.Format("Find A RT Dose that SopInstanceUid = {0}, PatientId = {1}",
                this.SopInstanceUID, this.PatientId);
        }
    }
}
