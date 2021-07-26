using System;
using System.Collections.Generic;
using System.Text;
using RTPlanFactoryLib.Model;
using Dicom;

namespace RTPlanFactoryLib.Interface
{
    interface INewRPWorkflow
    {
        public RpInfo GetOriginalRpInfo(DicomFile file);
        public RsInfo GetOriginalRsInfo(DicomFile file);
        public CtImgInfo GetOriginalCtImgInfo(DicomFile file);
        public RdInfo GetOriginalRdInfo(DicomFile file);
        public void NewRpInfo(RpInfo info);
        public void NewRsInfo(RsInfo info);
        public void NewCtImgInfo(CtImgInfo info);
        public void NewRdInfo(RdInfo info);
        public void SaveNewRpFileSet(string folderPath);
    }
}
