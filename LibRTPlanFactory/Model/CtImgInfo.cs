using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public class CtImgInfo : InfoBase
    {
        public override string ToString()
        {
            return string.Format("Find A CT Image that SopInstanceUid = {0}, PatientId = {1}",
                this.SopInstanceUID,this.PatientId);
        }
    }
}
