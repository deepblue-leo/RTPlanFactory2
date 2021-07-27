using System;
using System.Collections.Generic;
using System.Text;
using RTPlanFactoryLib.Model;
using Dicom;

namespace RTPlanFactoryLib.Interface
{
    interface ICreateNewRPWorkflow
    {
        public RpInfo GetOriginalRpInfo(DicomFile file);
        public RsInfo GetOriginalRsInfo(DicomFile file);
        public CtImgInfo GetOriginalCtImgInfo(DicomFile file);
        public RdInfo GetOriginalRdInfo(DicomFile file);
        public RtImgInfo GetOriginalRtImgInfo(DicomFile file);
        public bool UpdateNewRpInfo(DicomFile file,RpInfo info, string savePath);
        public bool UpdateNewRsInfo(DicomFile file,RsInfo info, string savePath);
        public bool UpdateNewCtImgInfo(DicomFile file,CtImgInfo info, string savePath);
        public bool UpdateNewRdInfo(DicomFile file,RdInfo info, string savePath);
        public bool UpdateNewRtImgInfo(DicomFile file,RtImgInfo info, string savePath); 
        //public bool SaveNewRpFileSet(string folderPath);        
    }
}
