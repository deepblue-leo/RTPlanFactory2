using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTPlanFactoryLib.Utility
{
    sealed class SopTypeClassifier
    {
        private static Dictionary<EnumSopType, List<string>> _dicSopType = new Dictionary<EnumSopType, List<string>>();
        private static SopTypeClassifier _Instance = null;
        private static readonly object syncRoot = new object();

        private SopTypeClassifier()
        {
            _dicSopType.Add(EnumSopType.CT_IMG, new List<string> { "1.2.840.10008.5.1.4.1.1.2" });
            _dicSopType.Add(EnumSopType.RT_DOSE, new List<string> { "1.2.840.10008.5.1.4.1.1.481.2" });
            _dicSopType.Add(EnumSopType.RT_IMG, new List<string> { "1.2.840.10008.5.1.4.1.1.481.1" });
            _dicSopType.Add(EnumSopType.RT_PLAN, new List<string> { "1.2.840.10008.5.1.4.1.1.481.5", "1.2.246.352.70.1.70" });
            _dicSopType.Add(EnumSopType.RT_STRUCTURE_SET, new List<string> { "1.2.840.10008.5.1.4.1.1.481.3" });
        }

        public static SopTypeClassifier GetInstance()
        {
            lock(syncRoot)
            {
                if (null == _Instance)
                {
                    _Instance = new SopTypeClassifier();
                }
                
                return _Instance;
            }            
        }

        public EnumSopType GetSopType(Dicom.DicomDataset dds)
        {
            EnumSopType ret = EnumSopType.UNKNOWN;

            if (dds.TryGetString(Dicom.DicomTag.SOPClassUID, out string ddsClassUid))
            {
                ret = _dicSopType.FirstOrDefault(q => q.Value.Contains(ddsClassUid)).Key;                                
            }

            return ret;
        }
    }
}
