using System;
using RTPlanFactoryLib.Interface;

namespace RTPlanFactoryLib.Implementor
{
    public class RTImageModifier: DicomModifierBase, IRTImageModifier
    {
        public RTImageModifier(Dicom.DicomDataset dds):base(dds)
        {
        }
    }
}
