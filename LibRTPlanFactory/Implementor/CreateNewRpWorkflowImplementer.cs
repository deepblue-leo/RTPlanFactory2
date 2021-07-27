using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dicom;
using RTPlanFactoryLib.Interface;
using RTPlanFactoryLib.Model;
using RTPlanFactoryLib.Utility;

namespace RTPlanFactoryLib.Implementor
{
    class CreateNewRpWorkflowImplementer : ICreateNewRPWorkflow
    {
        private string _originalFileSetFolder;
        private string _newFileSetFolder;
        private string _newPatientName;
        private string _newPatientId;
        private string _newPlanLabel;
        private int _newPlanCount;
        private Dictionary<string, InfoBase> _originalFileSet = new Dictionary<string, InfoBase>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="OriginalFileSetFolder">原始文件集路径</param>
        /// <param name="newFileSetFolder">新文件急路径</param>
        /// <param name="newPatientName">新患者姓名</param>
        /// <param name="newPlanLabel">新计划标签</param>
        /// <param name="newPlanCount">创建新计划数量</param>
        public CreateNewRpWorkflowImplementer(string originalFileSetFolder, string newFileSetFolder, string newPatientName, string newPlanLabel, int newPlanCount)
        {
            _originalFileSetFolder = originalFileSetFolder;
            _newFileSetFolder = newFileSetFolder;
            _newPatientName = newPatientName;
            //_newPatientId = newPatientId;
            _newPlanLabel = newPlanLabel;
            _newPlanCount = newPlanCount;
        }

        /// <summary>
        /// 遍历原计划文件集所在的目录，将其中所有dcm文件进行解析，解析出的信息装入_originalFileSet
        /// </summary>
        /// <param name="handleSopInfo">在遍历文件过程中所执行的额外动作，例如将info信息输出到界面</param>
        public void LoopOriginalFileSetFolder(Action<InfoBase> handleSopInfo)
        {
            DirectoryInfo root = new DirectoryInfo(_originalFileSetFolder);
            FileInfo[] files = root.GetFiles("(*.dcm)");

            foreach (var file in files)
            {
                DicomFile dFile = DicomFile.Open(file.FullName);
                InfoBase sopInfo = null;
                switch (SopTypeClassifier.GetInstance().GetSopType(dFile.Dataset))
                {
                    case EnumSopType.CT_IMG:
                        sopInfo = GetOriginalCtImgInfo(dFile);
                        _originalFileSet.Add(file.FullName, sopInfo);                        
                        break;
                    case EnumSopType.RT_IMG:
                        sopInfo = GetOriginalRtImgInfo(dFile);
                        _originalFileSet.Add(file.FullName, sopInfo);
                        break;
                    case EnumSopType.RT_PLAN:
                        sopInfo = GetOriginalRpInfo(dFile);
                        _originalFileSet.Add(file.FullName, sopInfo);
                        break;
                    case EnumSopType.RT_STRUCTURE_SET:
                        sopInfo = GetOriginalRsInfo(dFile);
                        _originalFileSet.Add(file.FullName, sopInfo);
                        break;
                    case EnumSopType.RT_DOSE:
                        sopInfo = GetOriginalRdInfo(dFile);
                        _originalFileSet.Add(file.FullName, sopInfo);
                        break;
                    case EnumSopType.UNKNOWN:
                        break;
                    default:
                        break;
                }

                if (null != sopInfo)
                {
                    handleSopInfo(sopInfo);
                }
            }
        }

        public void CreateNewRpSets(Action<InfoBase> handleSopInfo)
        {
            //获取所有的CT Image，并修改CtImgInfo
            //这里重点是创建新的CT Image SopInstanceUID

            //获取RS，并修改RsInfo
            //这里重点是创建新的RS SopInstanceUID，以及将CT Image SopInstanceUID更新进来

            //获取RD，并修改RdInfo
            //这里重点是创建新的RD SopInstanceUID

            //获取RP，并修改RdInfo
            //这里重点是创建新的RP SopInstanceUID，以及将新的RS SopInstanceUID，RD SopInstanceUID更新进来

            //获取Rt Img，并修改RtImgInfo
            //这里重点是创建新的Rt Img SopInstanceUID，以及将新的RP SopInstanceUID更新进来
        }

        /// <summary>
        /// 获取文件中Ct Img的主要信息
        /// </summary>
        /// <param name="file">CT Img文件</param>
        /// <returns>CtImgInfo</returns>
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
        /// <summary>
        /// 获取文件中RT Dose的主要信息
        /// </summary>
        /// <param name="file">RT Dose文件</param>
        /// <returns>RdInfo</returns>
        public RdInfo GetOriginalRdInfo(DicomFile file)
        {
            RdInfo ret = null;
            Dicom.DicomDataset dds = file.Dataset;

            if (SopTypeClassifier.GetInstance().GetSopType(dds) == EnumSopType.RT_DOSE)
            {
                ret.PatientId = dds.GetString(DicomTag.PatientID);
                ret.PatientName = dds.GetString(DicomTag.PatientName);
                ret.SopInstanceUID = dds.GetString(DicomTag.SOPInstanceUID);                
            }

            return ret;
        }
        /// <summary>
        /// 获取文件中RT Plan的主要信息
        /// </summary>
        /// <param name="file">RT Plan文件</param>
        /// <returns>RpInfo</returns>
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

                //获得RP中refer的Dose文件的Instance UID
                List<string> temList = new List<string>();
                DicomModifierBase.GetOriginalTagValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedDoseSequence, DicomTag.ReferencedSOPInstanceUID },
                    ref temList);
                ret.ReferencedRdSopInstanceUIDs = temList;

                //获得RP中refer的RS文件的Instance UID
                temList.Clear();
                DicomModifierBase.GetOriginalTagValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedStructureSetSequence, DicomTag.ReferencedSOPInstanceUID },
                    ref temList);
                ret.ReferencedRsSopInstanceUIDs = temList;
            }

            return ret;
        }
        /// <summary>
        /// 获取文件中RT Structure Set的主要信息
        /// </summary>
        /// <param name="file">RT Structure Set文件</param>
        /// <returns>RsInfo</returns>
        public RsInfo GetOriginalRsInfo(DicomFile file)
        {
            RsInfo ret = null;
            Dicom.DicomDataset dds = file.Dataset;

            if (SopTypeClassifier.GetInstance().GetSopType(dds) == EnumSopType.RT_STRUCTURE_SET)
            {
                ret.PatientId = dds.GetString(DicomTag.PatientID);
                ret.PatientName = dds.GetString(DicomTag.PatientName);
                ret.SopInstanceUID = dds.GetString(DicomTag.SOPInstanceUID);
                //ret.ReferencedCtImgSopInstanceUIDs = dds.GetString(DicomTag.RTPlanLabel);

                //获得RS中refer的CT IMG文件的Instance UID
                List<string> temList = new List<string>();
                DicomModifierBase.GetOriginalTagValues(
                    dds,
                    new DicomTag[] { DicomTag.ContourImageSequence, DicomTag.ReferencedSOPInstanceUID },
                    ref temList);
                ret.ReferencedCtImgSopInstanceUIDs = temList;
            }

            return ret;
        }
        /// <summary>
        /// 获取文件中RT Img的主要信息
        /// </summary>
        /// <param name="file">RT Image文件</param>
        /// <returns>RtImgInfo</returns>
        public RtImgInfo GetOriginalRtImgInfo(DicomFile file)
        {
            RtImgInfo ret = null;
            Dicom.DicomDataset dds = file.Dataset;

            if (SopTypeClassifier.GetInstance().GetSopType(dds) == EnumSopType.RT_IMG)
            {
                ret.PatientId = dds.GetString(DicomTag.PatientID);
                ret.PatientName = dds.GetString(DicomTag.PatientName);
                ret.SopInstanceUID = dds.GetString(DicomTag.SOPInstanceUID);
                //ret.ReferencedCtImgSopInstanceUIDs = dds.GetString(DicomTag.RTPlanLabel);

                //获得RS中refer的CT IMG文件的Instance UID
                List<string> temList = new List<string>();
                DicomModifierBase.GetOriginalTagValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedRTPlanSequence, DicomTag.ReferencedSOPInstanceUID },
                    ref temList);
                ret.ReferencedRpSopInstanceUIDs = temList;
            }

            return ret;
        }
        /// <summary>
        /// 更新CT Image信息并保存
        /// </summary>
        /// <param name="file">CT Image</param>
        /// <param name="info">CtImgInfo</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>bool</returns>
        public bool UpdateNewCtImgInfo(DicomFile file, CtImgInfo info, string savePath)
        {
            bool ret;
            try
            {
                DicomDataset dds = file.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.SopInstanceUID);

                file.Save(savePath);
                ret = true;
            }
            catch (Exception)
            {
                throw;
            }

            return ret;            
        }
        /// <summary>
        /// 更新RT Dose信息并保存
        /// </summary>
        /// <param name="file">RT Dose</param>
        /// <param name="info">RdInfo</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>bool</returns>
        public bool UpdateNewRdInfo(DicomFile file, RdInfo info, string savePath)
        {
            bool ret;
            try
            {
                DicomDataset dds = file.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.SopInstanceUID);                

                file.Save(savePath);
                ret = true;
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }
        /// <summary>
        /// 更新RT Plan信息并保存
        /// </summary>
        /// <param name="file">RT Plan</param>
        /// <param name="info">RpInfo</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>bool</returns>
        public bool UpdateNewRpInfo(DicomFile file, RpInfo info, string savePath)
        {
            bool ret;
            try
            {
                DicomDataset dds = file.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.SopInstanceUID);
                                
                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedDoseSequence, DicomTag.ReferencedSOPInstanceUID },
                    info.ReferencedRdSopInstanceUIDs);

                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedStructureSetSequence, DicomTag.ReferencedSOPInstanceUID },
                    info.ReferencedRsSopInstanceUIDs);

                file.Save(savePath);
                ret = true;
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }
        /// <summary>
        /// 更新RT Structure Set信息并保存
        /// </summary>
        /// <param name="file">RT Structure Set</param>
        /// <param name="info">RsInfo</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>bool</returns>
        public bool UpdateNewRsInfo(DicomFile file, RsInfo info, string savePath)
        {
            bool ret = false;

            try
            {
                DicomDataset dds = file.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.SopInstanceUID);

                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] { DicomTag.ContourImageSequence, DicomTag.ReferencedSOPInstanceUID },
                    info.ReferencedCtImgSopInstanceUIDs);

                file.Save(savePath);
                ret = true;
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }
        /// <summary>
        /// 更新RT Image信息并保存
        /// </summary>
        /// <param name="file">RT Image</param>
        /// <param name="info">RtImgInfo</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>bool</returns>
        public bool UpdateNewRtImgInfo(DicomFile file, RtImgInfo info, string savePath)
        {
            bool ret = false;

            try
            {
                DicomDataset dds = file.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.SopInstanceUID);

                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedRTPlanSequence, DicomTag.ReferencedSOPInstanceUID },
                    info.ReferencedRpSopInstanceUIDs);

                file.Save(savePath);
                ret = true;
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }
    }
}
