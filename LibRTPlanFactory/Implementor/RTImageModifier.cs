using System;
using LibRTPlanFactory.Interface;

namespace LibRTPlanFactory.Implementor
{
    public class RTImageModifier: DicomModifierBase, IRTImageModifier
    {
        public RTImageModifier(Dicom.DicomDataset dds):base(dds)
        {
        }
    }
}
