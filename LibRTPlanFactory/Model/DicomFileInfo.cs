using RTPlanFactoryLib.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public class DicomFileInfo
    {
        public EnumSopType SopType { get; set; }
        public string OriginalFilePath { get; set; }       
        public InfoBase OrginalSopInfo { get; set; }
        public string NewFilePath { get; set; }
        public InfoBase NewSopInfo { get; set; }
    }
}
