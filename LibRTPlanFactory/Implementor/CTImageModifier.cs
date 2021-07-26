using System;
using RTPlanFactoryLib.Interface;

namespace RTPlanFactoryLib.Implementor
{
    public class CTImageModifier:DicomModifierBase,ICTImageModifier
    {
        public CTImageModifier(Dicom.DicomDataset dds):base(dds)
        {
        }
    }
}
