using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTPlanFactoryLib.Utility
{
    sealed class SopTypeClassifier
    {
        private static Dictionary<EnumSopType, string> _dicSopType = new Dictionary<EnumSopType, string>();
        private static SopTypeClassifier _Instance = null;
        private static readonly object syncRoot = new object();

        private SopTypeClassifier()
        {
            _dicSopType.Add(EnumSopType.CT_IMG, "1.2.840.10008.5.1.4.1.1.2");
            _dicSopType.Add(EnumSopType.RT_DOSE, "1.2.840.10008.5.1.4.1.1.481.2");
            _dicSopType.Add(EnumSopType.RT_IMG, "1.2.840.10008.5.1.4.1.1.481.1");
            _dicSopType.Add(EnumSopType.RT_PLAN, "1.2.840.10008.5.1.4.1.1.481.5");
            _dicSopType.Add(EnumSopType.RT_STRUCTURE_SET, "1.2.840.10008.5.1.4.1.1.481.3");
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
                ret = _dicSopType.FirstOrDefault(q => q.Value == ddsClassUid).Key;                                
            }

            return ret;
        }
    }
}
