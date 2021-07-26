using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public class RsInfo
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string SopInstanceUID { get; set; }
        public List<string> ReferencedCtImgSopInstanceUIDs { get; set; }
    }
}
