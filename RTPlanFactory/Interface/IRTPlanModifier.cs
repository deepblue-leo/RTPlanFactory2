using System;
using System.Collections.Generic;

namespace RTPlanFactory.Interface
{
    public interface IRTPlanModifier
    {
        public string GetOriginalPlanLabel();
        public bool SetNewPlanLabel(string newPlanLabel);
        public string[] GetOriginalReferencedStructureSetUidSeq();
        public bool SetNewReferencedStructureSetUidSeq(string[] uids);
        public string[] GetOriginalReferencedDoseUidSeq();
        public bool SetNewReferencedDoseUidSeq(string[] uids);
    }
}
