using System;
using System.Collections.Generic;
using System.Text;
using RTPlanFactoryLib.Model;
using Dicom;

namespace RTPlanFactoryLib.Interface
{
    public interface ICreateNewRPWorkflow
    {
        RpInfo GetOriginalRpInfo(DicomFile file);
        RsInfo GetOriginalRsInfo(DicomFile file);
        CtImgInfo GetOriginalCtImgInfo(DicomFile file);
        RdInfo GetOriginalRdInfo(DicomFile file);
        RtImgInfo GetOriginalRtImgInfo(DicomFile file);
        bool CopyandUpdateNewRpInfo(DicomFileInfo info);
        bool CopyandUpdateNewRsInfo(DicomFileInfo info);
        bool CopyandUpdateNewCtImgInfo(DicomFileInfo info);
        bool CopyandUpdateNewRdInfo(DicomFileInfo info);
        bool CopyandUpdateNewRtImgInfo(DicomFileInfo info); 
        //public bool SaveNewRpFileSet(string folderPath);        
    }
}
