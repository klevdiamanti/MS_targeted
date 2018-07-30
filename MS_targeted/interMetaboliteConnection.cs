
using System.Collections.Generic;

namespace MS_targeted
{
    public class interMetaboliteConnection
    {
        public string Tissue { get; set; }
        public string CustomId_nominator { get; set; }
        public string Name_nominator { get; set; }
        public string Charge_nominator { get; set; }
        public string CustomId_denominator { get; set; }
        public string Name_denominator { get; set; }
        public string Charge_denominator { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public double PValue { get; set; }
        public List<perSampleRatio> ListOfPerSampleRatios { get; set; }

        public void fillInListOfPerSampleRatios(List<string> sid, List<double> r)
        {
            ListOfPerSampleRatios = new List<perSampleRatio>();
            for (int i = 0; i < sid.Count; i++)
            {
                ListOfPerSampleRatios.Add(new perSampleRatio() { sampleID = sid[i], ratio = r[i] });
            }
        }

        public class perSampleRatio
        {
            public string sampleID { get; set; }
            public double ratio { get; set; }
        }
    }
}
