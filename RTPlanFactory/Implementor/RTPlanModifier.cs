using System;
using System.Collections.Generic;
using Dicom;
using RTPlanFactory.Implementor;

namespace RTPlanFactory.Interface
{
    public class RTPlanModifier : DicomModifierBase, IRTPlanModifier
    {

        public RTPlanModifier(DicomDataset dds) : base(dds)
        {
                     
        }

        public string GetOriginalPlanLabel()
        {
            return DicomModifierBase.GetOriginalTagValue(_dds,Dicom.DicomTag.RTPlanLabel);
        }

        public string[] GetOriginalReferencedDoseUidSeq()
        {
            return DicomModifierBase.GetOriginalTagValues(_dds, Dicom.DicomTag.ReferencedDoseSequence);
        }

        public string[] GetOriginalReferencedStructureSetUidSeq()
        {
            return DicomModifierBase.GetOriginalTagValues(_dds, Dicom.DicomTag.ReferencedStructureSetSequence);
        }

        public bool SetNewPlanLabel(string newPlanLabel)
        {
            return DicomModifierBase.AddOrUpdateValue(_dds, Dicom.DicomTag.RTPlanLabel, newPlanLabel);
        }

        public bool SetNewReferencedDoseUidSeq(string[] uids)
        {
            throw new NotImplementedException();
        }

        public bool SetNewReferencedStructureSetUidSeq(string[] uids)
        {
            throw new NotImplementedException();
        }
    }
}
