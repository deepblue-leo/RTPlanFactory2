using System;
using System.Collections.Generic;
using Dicom;
using RTPlanFactoryLib.Interface;

namespace RTPlanFactoryLib.Implementor
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

        public void GetOriginalReferencedDoseUidSeq(ref List<string> values)
        {
            DicomModifierBase.GetOriginalTagValues(_dds,new DicomTag[]{DicomTag.ReferencedDoseSequence,
                 DicomTag.ReferencedSOPInstanceUID }, ref values);
        }

        public void GetOriginalReferencedStructureSetUidSeq(ref List<string> values)
        {
            DicomModifierBase.GetOriginalTagValues(_dds, new DicomTag[]{DicomTag.ReferencedStructureSetSequence,
                 DicomTag.ReferencedSOPInstanceUID }, ref values);
        }

        public bool SetNewPlanLabel(string newPlanLabel)
        {
            return DicomModifierBase.AddOrUpdateValue(_dds, Dicom.DicomTag.RTPlanLabel, newPlanLabel);
        }

        public bool SetNewReferencedDoseUidSeq(List<string> uids)
        {
            bool ret = true;
            try
            {
                DicomModifierBase.AddOrUpdateValues(_dds, new DicomTag[]{DicomTag.ReferencedDoseSequence,
                 DicomTag.ReferencedSOPInstanceUID }, uids);
            }
            catch (Exception)
            {
                ret = true;   
            }

            return ret;
        }

        public bool SetNewReferencedStructureSetUidSeq(List<string> uids)
        {
            bool ret = true;
            try
            {
                DicomModifierBase.AddOrUpdateValues(_dds, new DicomTag[]{DicomTag.ReferencedStructureSetSequence,
                 DicomTag.ReferencedSOPInstanceUID }, uids);
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }
    }
}
