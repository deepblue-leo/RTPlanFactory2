using System;
using System.Collections.Generic;

namespace RTPlanFactoryLib.Interface
{
    public interface IRTStructureSetModifier:IDicomModifier
    {
        string GetOriginalStructureSetLabel();
        bool SetNewStructureSetLabel(string newLabel);
        string GetOriginalStructureSetName();
        bool SetNewStructureSetName(string newName);
        string[] GetOriginalReferencedImageSopInstanceUidSeq();
        bool SetNewReferencedImageSopInstanceUidSeq(string[] newImgUids);
    }
}
