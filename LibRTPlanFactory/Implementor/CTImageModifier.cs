using System;
using LibRTPlanFactory.Interface;

namespace LibRTPlanFactory.Implementor
{
    public class CTImageModifier:DicomModifierBase,ICTImageModifier
    {
        public CTImageModifier(Dicom.DicomDataset dds):base(dds)
        {
        }
    }
}
