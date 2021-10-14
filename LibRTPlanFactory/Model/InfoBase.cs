using System;
using System.Collections.Generic;
using System.Text;

namespace RTPlanFactoryLib.Model
{
    public abstract class InfoBase
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string SopInstanceUID { get; set; }
        public string StudyInstanceUID { get; set; }
        public string SeriesInstanceUID { get; set; }
        public abstract override string ToString();        
    }
}
