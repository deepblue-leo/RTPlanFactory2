using System;
using System.Collections.Generic;
using Dicom;
using RTPlanFactoryLib.Implementor;

namespace RTPlanFactoryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pls enter the path of RT Plan file:");
            string rtPlanPath = Console.ReadLine().Trim();
            DicomFile dicomFile = DicomFile.Open(rtPlanPath);
            Dicom.DicomDataset dds = dicomFile.Dataset;
            RTPlanModifier planModifier = new RTPlanModifier(dds);
            //string oldPatientId = planModifier.GetOriginalPatientId();
            //Console.WriteLine("Original Patient Id is " + oldPatientId);
            //Console.WriteLine("Pls set a new patient id:");
            //string newPid = Console.ReadLine().Trim();
            //planModifier.SetNewPatientId(newPid);
            
            List<string> oldStructureSetUids = new List<string>();
            planModifier.GetOriginalReferencedStructureSetUidSeq(ref oldStructureSetUids);

            Console.WriteLine("Original Referenced Structureset uid are：" + string.Join(',',oldStructureSetUids.ToArray()));

            Console.WriteLine("Pls set a new Referenced Structureset uid:");
            List<string> newReferencedStructuresetUids = new List<string>(Console.ReadLine().Trim().Split(","));
            planModifier.SetNewReferencedStructureSetUidSeq(newReferencedStructuresetUids);

            dicomFile.Save("123new.dcm");
        }
    }
}
