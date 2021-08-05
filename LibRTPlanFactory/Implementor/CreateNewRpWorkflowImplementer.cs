using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dicom;
using RTPlanFactoryLib.Interface;
using RTPlanFactoryLib.Model;
using RTPlanFactoryLib.Utility;

namespace RTPlanFactoryLib.Implementor
{
    public class CreateNewRpWorkflowImplementer : ICreateNewRPWorkflow
    {                
        private List<DicomFileInfo> _originalDicomFileList = new List<DicomFileInfo>();

        /// <summary>
        /// 构造函数
        /// </summary>        
        public CreateNewRpWorkflowImplementer()
        {           
        }

        /// <summary>
        /// 遍历原计划文件集所在的目录，将其中所有dcm文件进行解析，解析出的信息装入_originalFileSet
        /// </summary>
        /// <param name="handleSopInfo">在遍历文件过程中所执行的额外动作，例如将info信息输出到界面</param>
        public void LoopOriginalFileSetFolder(string originalFileSetFolder, Action<DicomFileInfo> handleSopInfo)
        {
            DirectoryInfo root = new DirectoryInfo(originalFileSetFolder);
            FileInfo[] files = root.GetFiles("*.dcm");

            foreach (var file in files)
            {
                DicomFile dFile = DicomFile.Open(file.FullName);
                //InfoBase sopInfo = null;
                DicomFileInfo fileInfo = null;
                switch (SopTypeClassifier.GetInstance().GetSopType(dFile.Dataset))
                {
                    case EnumSopType.CT_IMG:
                        fileInfo = new DicomFileInfo
                        {
                            OriginalFilePath = file.FullName,
                            SopType = SopTypeClassifier.GetInstance().GetSopType(dFile.Dataset),
                            OrginalSopInfo = GetOriginalCtImgInfo(dFile)
                        };
                        _originalDicomFileList.Add(fileInfo);
                        break;
                    case EnumSopType.RT_IMG:
                        fileInfo = new DicomFileInfo
                        {
                            OriginalFilePath = file.FullName,
                            SopType = SopTypeClassifier.GetInstance().GetSopType(dFile.Dataset),
                            OrginalSopInfo = GetOriginalRtImgInfo(dFile)
                        };
                        _originalDicomFileList.Add(fileInfo);
                        break;
                    case EnumSopType.RT_PLAN:
                        fileInfo = new DicomFileInfo
                        {
                            OriginalFilePath = file.FullName,
                            SopType = SopTypeClassifier.GetInstance().GetSopType(dFile.Dataset),
                            OrginalSopInfo = GetOriginalRpInfo(dFile)
                        };
                        _originalDicomFileList.Add(fileInfo);
                        break;
                    case EnumSopType.RT_STRUCTURE_SET:
                        fileInfo = new DicomFileInfo
                        {
                            OriginalFilePath = file.FullName,
                            SopType = SopTypeClassifier.GetInstance().GetSopType(dFile.Dataset),
                            OrginalSopInfo = GetOriginalRsInfo(dFile)
                        };
                        _originalDicomFileList.Add(fileInfo);
                        break;
                    case EnumSopType.RT_DOSE:
                        fileInfo = new DicomFileInfo
                        {
                            OriginalFilePath = file.FullName,
                            SopType = SopTypeClassifier.GetInstance().GetSopType(dFile.Dataset),
                            OrginalSopInfo = GetOriginalRdInfo(dFile)
                        };
                        _originalDicomFileList.Add(fileInfo);
                        break;
                    case EnumSopType.UNKNOWN:
                        break;
                    default:
                        break;
                }

                if (null != fileInfo)
                {
                    handleSopInfo(fileInfo);
                }
            }
        }        

        public /*async*/ void CreateNewRpSets(string newFileSetFolder, string newPatientName, string newPatientId, string newPlanLabe, Action<DicomFileInfo> handleSopInfo)
        {
            List<string> ctImageSopInstanceUidList = new List<string>();
            List<string> rsSopInstanceUidList = new List<string>();
            List<string> rdSopInstanceUidList = new List<string>();
            List<string> rpSopInstanceUidList = new List<string>();

            if (!Directory.Exists(newFileSetFolder))
                Directory.CreateDirectory(newFileSetFolder);

            //获取所有的CT Image，并修改CtImgInfo
            //这里重点是创建新的CT Image SopInstanceUID
            foreach (var item in _originalDicomFileList.Where(a => a.SopType == EnumSopType.CT_IMG))
            {
                item.NewSopInfo = new CtImgInfo
                {
                    PatientId = newPatientId,
                    PatientName = newPatientName,
                    SopInstanceUID = DicomModifierBase.GuidToUidStringUsingStringAndParse(Guid.NewGuid()),                    
                };
                ctImageSopInstanceUidList.Add(item.NewSopInfo.SopInstanceUID);                                

                item.NewFilePath = newFileSetFolder + "CI" + item.NewSopInfo.SopInstanceUID + ".dcm";                
                CopyandUpdateNewCtImgInfo(item);

                if (null != item)
                {                    
                    handleSopInfo(item);                 
                }
            }

            //获取RS，并修改RsInfo
            //这里重点是创建新的RS SopInstanceUID，以及将CT Image SopInstanceUID更新进来
            foreach (var item in _originalDicomFileList.Where(a => a.SopType == EnumSopType.RT_STRUCTURE_SET))
            {
                item.NewSopInfo = new RsInfo
                {
                    PatientId = newPatientId,
                    PatientName = newPatientName,
                    SopInstanceUID = DicomModifierBase.GuidToUidStringUsingStringAndParse(Guid.NewGuid()),
                    ReferencedCtImgSopInstanceUIDs = ctImageSopInstanceUidList
                };
                rsSopInstanceUidList.Add(item.NewSopInfo.SopInstanceUID);
                item.NewFilePath = newFileSetFolder + "RS" + item.NewSopInfo.SopInstanceUID + ".dcm";
                CopyandUpdateNewRsInfo(item);

                if (null != item)
                {
                    handleSopInfo(item);
                }
            }

            //获取RD，并修改RdInfo
            //这里重点是创建新的RD SopInstanceUID
            foreach (var item in _originalDicomFileList.Where(a => a.SopType == EnumSopType.RT_DOSE))
            {
                item.NewSopInfo = new RdInfo
                {
                    PatientId = newPatientId,
                    PatientName = newPatientName,
                    SopInstanceUID = DicomModifierBase.GuidToUidStringUsingStringAndParse(Guid.NewGuid()),                    
                };
                rdSopInstanceUidList.Add(item.NewSopInfo.SopInstanceUID);
                item.NewFilePath = newFileSetFolder + "RD" + item.NewSopInfo.SopInstanceUID + ".dcm";
                CopyandUpdateNewRdInfo(item);

                if (null != item)
                {
                    handleSopInfo(item);
                }
            }

            //获取RP，并修改RpInfo
            //这里重点是创建新的RP SopInstanceUID，以及将新的RS SopInstanceUID，RD SopInstanceUID更新进来
            foreach (var item in _originalDicomFileList.Where(a => a.SopType == EnumSopType.RT_PLAN))
            {
                item.NewSopInfo = new RpInfo
                {
                    PatientId = newPatientId,
                    PatientName = newPatientName,
                    SopInstanceUID = DicomModifierBase.GuidToUidStringUsingStringAndParse(Guid.NewGuid()),
                    PlanLabel = newPlanLabe,
                    ReferencedRsSopInstanceUIDs = rsSopInstanceUidList,
                    ReferencedRdSopInstanceUIDs = rdSopInstanceUidList,
                };
                rpSopInstanceUidList.Add(item.NewSopInfo.SopInstanceUID);
                item.NewFilePath = newFileSetFolder + "RP" + item.NewSopInfo.SopInstanceUID + ".dcm";
                CopyandUpdateNewRpInfo(item);

                if (null != item)
                {
                    handleSopInfo(item);
                }
            }

            //获取Rt Img，并修改RtImgInfo
            //这里重点是创建新的Rt Img SopInstanceUID，以及将新的RP SopInstanceUID更新进来
            foreach (var item in _originalDicomFileList.Where(a => a.SopType == EnumSopType.RT_IMG))
            {
                item.NewSopInfo = new RtImgInfo
                {
                    PatientId = newPatientId,
                    PatientName = newPatientName,
                    SopInstanceUID = DicomModifierBase.GuidToUidStringUsingStringAndParse(Guid.NewGuid()),
                    ReferencedRpSopInstanceUIDs = rpSopInstanceUidList,
                };                
                item.NewFilePath = newFileSetFolder + "RI" + item.NewSopInfo.SopInstanceUID + ".dcm";
                CopyandUpdateNewRtImgInfo(item);

                if (null != item)
                {
                    handleSopInfo(item);
                }
            }            
        }

        /// <summary>
        /// 获取文件中Ct Img的主要信息
        /// </summary>
        /// <param name="file">CT Img文件</param>
        /// <returns>CtImgInfo</returns>
        public CtImgInfo GetOriginalCtImgInfo(DicomFile file)
        {
            CtImgInfo ret = new CtImgInfo();
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
            RdInfo ret = new RdInfo();
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
            RpInfo ret = new RpInfo();
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
            RsInfo ret = new RsInfo();
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
            RtImgInfo ret = new RtImgInfo();
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
        public bool CopyandUpdateNewCtImgInfo(DicomFileInfo info)
        {
            bool ret;
            try
            {
                //File.Copy(info.OriginalFilePath, info.NewFilePath);

                //DicomFile dFile = DicomFile.Open(info.NewFilePath);
                DicomFile dFile = DicomFile.Open(info.OriginalFilePath);
                DicomDataset dds = dFile.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.NewSopInfo.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.NewSopInfo.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.NewSopInfo.SopInstanceUID);

                dFile.SaveAsync(info.NewFilePath);
                
                //dFile.Save(info.NewFilePath);
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
        public bool CopyandUpdateNewRdInfo(DicomFileInfo info)
        {
            bool ret;
            try
            {
                //File.Copy(info.OriginalFilePath, info.NewFilePath);

                DicomFile dFile = DicomFile.Open(info.OriginalFilePath);
                DicomDataset dds = dFile.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.NewSopInfo.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.NewSopInfo.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.NewSopInfo.SopInstanceUID);

                dFile.SaveAsync(info.NewFilePath);                
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
        public bool CopyandUpdateNewRpInfo(DicomFileInfo info)
        {
            bool ret;
            try
            {
                //File.Copy(info.OriginalFilePath, info.NewFilePath);

                DicomFile dFile = DicomFile.Open(info.OriginalFilePath);
                DicomDataset dds = dFile.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.NewSopInfo.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.NewSopInfo.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.NewSopInfo.SopInstanceUID);
                                
                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedDoseSequence, DicomTag.ReferencedSOPInstanceUID },
                    ((RpInfo)info.NewSopInfo).ReferencedRdSopInstanceUIDs);

                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedStructureSetSequence, DicomTag.ReferencedSOPInstanceUID },
                    ((RpInfo)info.NewSopInfo).ReferencedRsSopInstanceUIDs);

                dFile.SaveAsync(info.NewFilePath);
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
        public bool CopyandUpdateNewRsInfo(DicomFileInfo info)
        {
            bool ret = false;

            try
            {
                //File.Copy(info.OriginalFilePath, info.NewFilePath);

                DicomFile dFile = DicomFile.Open(info.OriginalFilePath);
                DicomDataset dds = dFile.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.NewSopInfo.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.NewSopInfo.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.NewSopInfo.SopInstanceUID);

                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] {
                        DicomTag.ROIContourSequence,
                        DicomTag.ContourSequence,
                        DicomTag.ContourImageSequence, 
                        DicomTag.ReferencedSOPInstanceUID },
                    ((RsInfo)info.NewSopInfo).ReferencedCtImgSopInstanceUIDs);

                dFile.SaveAsync(info.NewFilePath);
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
        public bool CopyandUpdateNewRtImgInfo(DicomFileInfo info)
        {
            bool ret = false;

            try
            {
                //File.Copy(info.OriginalFilePath, info.NewFilePath);

                DicomFile dFile = DicomFile.Open(info.OriginalFilePath);
                DicomDataset dds = dFile.Dataset;

                dds.AddOrUpdate<string>(DicomTag.PatientID, info.NewSopInfo.PatientId);
                dds.AddOrUpdate<string>(DicomTag.PatientName, info.NewSopInfo.PatientName);
                dds.AddOrUpdate<string>(DicomTag.SOPInstanceUID, info.NewSopInfo.SopInstanceUID);

                DicomModifierBase.AddOrUpdateValues(
                    dds,
                    new DicomTag[] { DicomTag.ReferencedRTPlanSequence, DicomTag.ReferencedSOPInstanceUID },
                    ((RtImgInfo)info.NewSopInfo).ReferencedRpSopInstanceUIDs);

                dFile.SaveAsync(info.NewFilePath);
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
