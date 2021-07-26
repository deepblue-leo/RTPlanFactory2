using System;
using System.Collections.Generic;
using System.Text;
using Dicom;
using RTPlanFactoryLib.Interface;
using RTPlanFactoryLib.Model;
using RTPlanFactoryLib.Utility;

namespace RTPlanFactoryLib.Implementor
{
    class NewRpWorkflowImplementer : INewRPWorkflow
    {
        public CtImgInfo GetOriginalCtImgInfo(DicomFile file)
        {
            CtImgInfo ret = null;
            Dicom.DicomDataset dds = file.Dataset;

            if (SopTypeClassifier.GetInstance().GetSopType(dds) == EnumSopType.CT_IMG)
            {
                ret.PatientId = dds.GetString(DicomTag.PatientID);
                ret.PatientName = dds.GetString(DicomTag.PatientName);
                ret.SopInstanceUID = dds.GetString(DicomTag.SOPInstanceUID);                
            }

            return ret;
        }

        public RdInfo GetOriginalRdInfo(DicomFile file)
        {
            RdInfo ret = null;
            Dicom.DicomDataset dds = file.Dataset;

            if (SopTypeClassifier.GetInstance().GetSopType(dds) == EnumSopType.RT_PLAN)
            {
                ret.PatientId = dds.GetString(DicomTag.PatientID);
                ret.PatientName = dds.GetString(DicomTag.PatientName);
                ret.SopInstanceUID = dds.GetString(DicomTag.SOPInstanceUID);                
            }

            return ret;
        }

        public RpInfo GetOriginalRpInfo(DicomFile file)
        {
            RpInfo ret = null;
            Dicom.DicomDataset dds = file.Dataset;

            if (SopTypeClassifier.GetInstance().GetSopType(dds) == EnumSopType.RT_PLAN)
            {
                ret.PatientId = dds.GetString(DicomTag.PatientID);
                ret.PatientName = dds.GetString(DicomTag.PatientName);
                ret.SopInstanceUID = dds.GetString(DicomTag.SOPInstanceUID);
                ret.PlanLabel = dds.GetString(DicomTag.RTPlanLabel);
            }

            return ret;
        }

        public RsInfo GetOriginalRsInfo(DicomFile file)
        {
            throw new NotImplementedException();
        }

        public void NewCtImgInfo(CtImgInfo info)
        {
            throw new NotImplementedException();
        }

        public void NewRdInfo(RdInfo info)
        {
            throw new NotImplementedException();
        }

        public void NewRpInfo(RpInfo info)
        {
            throw new NotImplementedException();
        }

        public void NewRsInfo(RsInfo info)
        {
            throw new NotImplementedException();
        }

        public void SaveNewRpFileSet(string folderPath)
        {
            throw new NotImplementedException();
        }
    }
}
