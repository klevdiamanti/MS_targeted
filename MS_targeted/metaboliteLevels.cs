using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class metaboliteLevels
    {
        public static List<sampleForTissueAndCharge> List_SampleForTissueAndCharge = new List<sampleForTissueAndCharge>();
        private static List<string> listOfMetaboliteIDs = new List<string>();

        public static void ReadInputMetabolites()
        {
            foreach (string csvFile in Directory.GetFiles(@"" + publicVariables.inputMSFilesDir).Where(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(publicVariables.prefix.ToString()) && x.EndsWith(publicVariables.suffix)))
            {
                string line;
                List<msMetabolite> listOfMetabolitesPerTissueAndCharge = new List<msMetabolite>();
                List<string> listOfCsvLines = new List<string>();
                using (TextReader input = new StreamReader(@"" + csvFile))
                {
                    while ((line = input.ReadLine()) != null)
                    {
                        if (line.StartsWith("Meta") || line.StartsWith("Type")) //header or metabolites data
                        {
                            listOfCsvLines.Add(line);
                        }
                        else if (line.StartsWith("Sample")) //metabolites measurements
                        {
                            if (listOfMetabolitesPerTissueAndCharge.Count == 0)
                            {
                                listOfMetabolitesPerTissueAndCharge = ReadInputMetabolitesFromDatabase(listOfCsvLines);
                            }
                            List_SampleForTissueAndCharge.Add(new sampleForTissueAndCharge(line.Split(publicVariables.breakCharInFile).ToList(), listOfMetabolitesPerTissueAndCharge));
                        }
                        else
	                    {
                            outputToLog.WriteLine("Metabolite levels file " + csvFile.Split(Path.DirectorySeparatorChar).Last() + " contains unexpected entries in column 1!");
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }

        private static List<msMetabolite> ReadInputMetabolitesFromDatabase(List<string> listOfCsvLines)
        {
            List<msMetabolite> listOfMetabolitesPerTissueAndCharge = new List<msMetabolite>();
            msMetabolite msMetab;
            bool addToList = false, isDuplicate = false;
            for (int i = publicVariables.indexToStartFrom; i < listOfCsvLines.First().Split(publicVariables.breakCharInFile).Length; i++)
            {
                msMetab = new msMetabolite()
                {
                    In_Index = i,
                    In_Name = listOfCsvLines.First().Split(publicVariables.breakCharInFile).ElementAt(i).Trim(),
                    In_Type = listOfCsvLines.ElementAt(1).Split(publicVariables.breakCharInFile).ElementAt(i).Trim(),
                    In_Formula = listOfCsvLines.ElementAt(2).Split(publicVariables.breakCharInFile).ElementAt(i).Replace(" ", ""),
                    In_Mass = Convert.ToDouble(listOfCsvLines.ElementAt(3).Split(publicVariables.breakCharInFile).ElementAt(i)),
                    In_Rt = Convert.ToDouble(listOfCsvLines.ElementAt(4).Split(publicVariables.breakCharInFile).ElementAt(i)),
                    In_customId = listOfCsvLines.ElementAt(5).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_Cas_id = listOfCsvLines.ElementAt(6).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Cas_id = listOfCsvLines.ElementAt(7).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_Hmdb_id = listOfCsvLines.ElementAt(8).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Hmdb_id = listOfCsvLines.ElementAt(9).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_Kegg_id = listOfCsvLines.ElementAt(10).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Kegg_id = listOfCsvLines.ElementAt(11).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_Chebi_id = listOfCsvLines.ElementAt(12).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Chebi_id = listOfCsvLines.ElementAt(13).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_Pubchem_id = listOfCsvLines.ElementAt(14).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Pubchem_id = listOfCsvLines.ElementAt(15).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_Chemspider_id = listOfCsvLines.ElementAt(16).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Chemspider_id = listOfCsvLines.ElementAt(17).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_Lipidmaps_id = listOfCsvLines.ElementAt(18).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Lipidmaps_id = listOfCsvLines.ElementAt(19).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_Metlin_id = listOfCsvLines.ElementAt(20).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_add_Metlin_id = listOfCsvLines.ElementAt(21).Split(publicVariables.breakCharInFile).ElementAt(i).Split('|').ToList(),
                    In_isProblematic = Convert.ToBoolean(listOfCsvLines.ElementAt(22).Split(publicVariables.breakCharInFile).ElementAt(i)),
                    In_msMsConfirmed = Convert.ToBoolean(listOfCsvLines.ElementAt(23).Split(publicVariables.breakCharInFile).ElementAt(i)),
                    In_inBlank = Convert.ToBoolean(listOfCsvLines.ElementAt(24).Split(publicVariables.breakCharInFile).ElementAt(i)),
                    In_msProblematic = Convert.ToBoolean(listOfCsvLines.ElementAt(25).Split(publicVariables.breakCharInFile).ElementAt(i)),
                    In_AZmSuperClass = listOfCsvLines.ElementAt(26).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_AZmClass = listOfCsvLines.ElementAt(27).Split(publicVariables.breakCharInFile).ElementAt(i),
                    In_AZmNameFixed = listOfCsvLines.ElementAt(28).Split(publicVariables.breakCharInFile).ElementAt(i)
                };

                if (!string.IsNullOrEmpty(msMetab.In_customId) && !string.IsNullOrWhiteSpace(msMetab.In_customId) && msMetab.In_Type == "Metabolite")
                {
                    if (listOfMetaboliteIDs.Any(x => x.Split('_').First() == msMetab.In_customId))
                    {
                        if (listOfMetabolitesPerTissueAndCharge.Any(x => x.In_customId.Split('_').First() == msMetab.In_customId))
                        {
                            msMetab.ToHMDB_metabolite(listOfMetabolitesPerTissueAndCharge.First(x => x.In_customId.Split('_').First() == msMetab.In_customId));
                            if (listOfMetabolitesPerTissueAndCharge.Count(x => x.In_customId.Split('_').First() == msMetab.In_customId) == 1)
                            {
                                msMetab.In_customId = msMetab.In_customId + "_1";
                            }
                            else
                            {
                                msMetab.In_customId = msMetab.In_customId + "_" + Convert.ToString(listOfMetabolitesPerTissueAndCharge.Where(x => x.In_customId.Split('_').First() == msMetab.In_customId)
                                    .Select(x => x.In_customId).Where(x => x.Split('_').Length > 1).Select(x => Convert.ToInt32(x.Split('_').Last())).Max() + 1);
                            }
                            addToList = true;
                        }
                        else
                        {
                            msMetab.ToHMDB_metabolite(List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).First(x => x.mtbltDetails.In_customId == msMetab.In_customId).mtbltDetails);
                            addToList = false;
                        }
                        isDuplicate = false;
                    }
                    else
                    {
                        msMetab.getFromMetaboliteDB();
                        addToList = true;
                        isDuplicate = false;
                    }
                }
                else if (msMetab.In_Type == "IS")
                {
                    addToList = false;
                    isDuplicate = true;
                }

                if (addToList)
                {
                    listOfMetaboliteIDs.Add(msMetab.In_customId);
                }

                if (!isDuplicate)
                {
                    listOfMetabolitesPerTissueAndCharge.Add(msMetab);
                }
            }

            return listOfMetabolitesPerTissueAndCharge;
        }
    }
}
