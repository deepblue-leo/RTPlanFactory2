using System;
using LibRTPlanFactory.Interface;

namespace LibRTPlanFactory.Implementor
{
    public class RTStructureSetModifier:DicomModifierBase,IRTStructureSetModifier
    {
        public RTStructureSetModifier(Dicom.DicomDataset dds):base(dds)
        {
        }

        public string[] GetOriginalReferencedImageSopInstanceUidSeq()
        {
            throw new NotImplementedException();
        }

        public string GetOriginalStructureSetLabel()
        {
            throw new NotImplementedException();
        }

        public string GetOriginalStructureSetName()
        {
            throw new NotImplementedException();
        }

        public bool SetNewReferencedImageSopInstanceUidSeq(string[] newImgUids)
        {
            throw new NotImplementedException();
        }

        public bool SetNewStructureSetLabel(string newLabel)
        {
            throw new NotImplementedException();
        }

        public bool SetNewStructureSetName(string newName)
        {
            throw new NotImplementedException();
        }
    }
}
