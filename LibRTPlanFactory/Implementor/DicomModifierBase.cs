using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using RTPlanFactoryLib.Interface;

namespace RTPlanFactoryLib.Implementor
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

        static protected string GetOriginalTagValue(Dicom.DicomDataset dds, Dicom.DicomTag tag)
        {
            return dds.GetValueOrDefault<string>(tag, 0, null);
        }

        /// <summary>
        /// 获取Sequence下某个tag的所有值
        /// </summary>
        /// <param name="dds">Dicom Dataset</param>
        /// <param name="tags">按级别放入tag，前面的tag都应该是sequence类型，只有最后一个tag才是要具体取值的</param>
        /// <returns></returns>
        static public void GetOriginalTagValues(Dicom.DicomDataset dds, Dicom.DicomTag[] tags, ref List<string> values)
        { 
            for (int i = 0; i < tags.Length; i++)
            {
                Dicom.DicomSequence seq;
                if (dds.TryGetSequence(tags[i], out seq) && seq.Items.Count > 0)
                {
                    foreach (var item in seq.Items)
                    {
                        GetOriginalTagValues(item, GetPartOfTagArray(tags, i + 1, tags.Length - 1 - i), ref values);
                    }                    
                }
                else
                {
                    if (dds.TryGetSingleValue<string>(tags[i], out string value))
                    {
                        values.Add(value);
                    }
                    //values.Add(dds.GetSingleValueOrDefault<string>(tags[i],null));
                }                
            }       
        }

        static private Dicom.DicomTag[] GetPartOfTagArray(Dicom.DicomTag[] tags, int start, int length)
        {
            Dicom.DicomTag[] newTagArray = new Dicom.DicomTag[length];
            for (int i = 0; i < length; i++)
            {
                newTagArray[i] = tags[start + i];
            }

            return newTagArray;
        }

        static protected bool AddOrUpdateValue(Dicom.DicomDataset dds, Dicom.DicomTag tag, string value)
        {
            bool ret = false;
            try
            {
                dds.AddOrUpdate<string>(tag, value);
                ret = true;
            }
            catch (Exception)
            {

            }

            return ret;
        }

        static public void AddOrUpdateValues(Dicom.DicomDataset dds, Dicom.DicomTag[] tags, List<string> values)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (dds.TryGetSequence(tags[i], out Dicom.DicomSequence seq) /*&& seq.Items.Count > 0*/)
                {
                    foreach (var item in seq.Items)
                    {
                        try
                        {
                            AddOrUpdateValues(item, GetPartOfTagArray(tags, i + 1, tags.Length - 1 - i), values);
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }                        
                    }
                }
                else
                {
                    //只针对最后一个Tag进行值的更新
                    if (tags.Length == 1)
                    {
                        try
                        {
                            dds.AddOrUpdate(tags[i], values[0]);
                            if (values.Count > 1)
                            {
                                values.RemoveAt(0);
                            }
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }                                              
                    }
                }
            }
        }

        public static string GuidToUidStringUsingStringAndParse(Guid value)
        {

            var guidBytes = string.Format("0{0:N}", value);

            var bigInteger = BigInteger.Parse(guidBytes, NumberStyles.HexNumber);

            return string.Format(CultureInfo.InvariantCulture, "2.25.{0}", bigInteger);
        }
    }
}
