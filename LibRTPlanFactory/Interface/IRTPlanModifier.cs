using System;
using System.Collections.Generic;

namespace LibRTPlanFactory.Interface
{
    public interface IRTPlanModifier
    {
        public string GetOriginalPlanLabel();
        public bool SetNewPlanLabel(string newPlanLabel);
        public void GetOriginalReferencedStructureSetUidSeq(ref List<string> values);
        public bool SetNewReferencedStructureSetUidSeq(List<string> uids);
        public void GetOriginalReferencedDoseUidSeq(ref List<string> values);
        public bool SetNewReferencedDoseUidSeq(List<string> uids);
    }
}
