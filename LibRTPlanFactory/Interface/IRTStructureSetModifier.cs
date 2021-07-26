using System;
using System.Collections.Generic;

namespace RTPlanFactoryLib.Interface
{
    public interface IRTStructureSetModifier:IDicomModifier
    {
        public string GetOriginalStructureSetLabel();
        public bool SetNewStructureSetLabel(string newLabel);
        public string GetOriginalStructureSetName();
        public bool SetNewStructureSetName(string newName);
        public string[] GetOriginalReferencedImageSopInstanceUidSeq();
        public bool SetNewReferencedImageSopInstanceUidSeq(string[] newImgUids);
    }
}
