using System;
using System.Collections.Generic;

namespace RTPlanFactoryLib.Interface
{
    public interface IRTPlanModifier
    {
        string GetOriginalPlanLabel();
        bool SetNewPlanLabel(string newPlanLabel);
        void GetOriginalReferencedStructureSetUidSeq(ref List<string> values);
        bool SetNewReferencedStructureSetUidSeq(List<string> uids);
        void GetOriginalReferencedDoseUidSeq(ref List<string> values);
        bool SetNewReferencedDoseUidSeq(List<string> uids);
    }
}
