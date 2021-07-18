using System;
namespace RTPlanFactory.Interface
{
    public interface IDicomModifier
    {
        string GetOriginalPatientId();
        string GetOriginalPatientName();
        string GetOriginalStudyInstanceUid();
        string GetOriginalSeriesInstanceUid();
        string GetOriginalSopInstanceUid();

        bool SetNewPatientId(string newPid);
        bool SetNewPatientName(string newPatientName);        
        bool SetNewStudyInstanceUid(string newStudyInstanceUid);
        bool SetNewSeriesInstanceUid(string newSeriesInstanceUid);
        bool SetNewSopInstanceUid(string newSopInstanceUid);
    }
}
