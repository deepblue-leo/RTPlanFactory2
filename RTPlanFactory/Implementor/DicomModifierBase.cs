using System;
using RTPlanFactory.Interface;

namespace RTPlanFactory.Implementor
{
    public class DicomModifierBase:IDicomModifier
    {
        protected Dicom.DicomDataset _dds;

        public DicomModifierBase(Dicom.DicomDataset dds)
        {
            if (null != dds)
            {
                _dds = dds;
            }
        }

        public string GetOriginalPatientId()
        {
            return GetOriginalTagValue(_dds,Dicom.DicomTag.PatientID);
        }      

        public string GetOriginalPatientName()
        {
            return GetOriginalTagValue(_dds, Dicom.DicomTag.PatientName);            
        }

        public string GetOriginalSeriesInstanceUid()
        {
            return GetOriginalTagValue(_dds, Dicom.DicomTag.SeriesInstanceUID);           
        }

        public string GetOriginalSopInstanceUid()
        {
            return GetOriginalTagValue(_dds, Dicom.DicomTag.SOPInstanceUID);           
        }

        public string GetOriginalStudyInstanceUid()
        {
            return GetOriginalTagValue(_dds, Dicom.DicomTag.StudyInstanceUID);
        }

        public bool SetNewPatientId(string newPid)
        {
            return AddOrUpdateValue(_dds, Dicom.DicomTag.PatientID, newPid);
        }        

        public bool SetNewPatientName(string newPatientName)
        {
            return AddOrUpdateValue(_dds, Dicom.DicomTag.PatientName, newPatientName);
        }

        public bool SetNewSeriesInstanceUid(string newSeriesInstanceUid)
        {
            return AddOrUpdateValue(_dds, Dicom.DicomTag.SeriesInstanceUID, newSeriesInstanceUid);
        }

        public bool SetNewSopInstanceUid(string newSopInstanceUid)
        {
            return AddOrUpdateValue(_dds, Dicom.DicomTag.SOPInstanceUID, newSopInstanceUid);
        }

        public bool SetNewStudyInstanceUid(string newStudyInstanceUid)
        {
            return AddOrUpdateValue(_dds, Dicom.DicomTag.StudyInstanceUID, newStudyInstanceUid);
        }

        static public string GetOriginalTagValue(Dicom.DicomDataset dds, Dicom.DicomTag tag)
        {
            return dds.GetValueOrDefault<string>(tag, 0, null);
        }

        /// <summary>
        /// 获取Sequence下某个tag的所有值
        /// </summary>
        /// <param name="dds">Dicom Dataset</param>
        /// <param name="tags">按级别放入tag，前面的tag都应该是sequence类型，只有最后一个tag才是要具体取值的</param>
        /// <returns></returns>
        static public string[] GetOriginalTagValues(Dicom.DicomDataset dds, Dicom.DicomTag[] tags)
        {
            Dicom.DicomSequence seq = null;

            for (int i = 0; i < tags.Length - 1; i++)
            {
                seq = dds.GetSequence(tags[i]);
            }       
        }

        static public bool AddOrUpdateValue(Dicom.DicomDataset dds, Dicom.DicomTag tag, string value)
        {
            bool ret = false;

            try
            {
                dds.AddOrUpdate<string>(tag, value);
                ret = true;
            }
            catch (Exception ex)
            {

            }

            return ret;
        }

        static public bool AddOrUpdateValues(Dicom.DicomDataset dds, Dicom.DicomTag tag, string[] values)
        {
            bool ret = false;

            try
            {
                dds.
            }
            catch (Exception ex)
            {

            }
        }
    }
}
