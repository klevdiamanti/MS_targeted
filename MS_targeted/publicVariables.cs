using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class publicVariables
    {
        //input arguments
        public static string inputMSFilesDir { get; set; }
        public static string clinicalDataFile { get; set; }
        public static string databaseFile { get; set; }
        public static string outputDir { get; set; }
        public enum prefixValues
        {
            lcms,
            gcms,
            mixed
        }
        public static prefixValues prefix { get; set; }
        public enum numberOfClassesValues
        {
            two,
            three
        }
        public static numberOfClassesValues numberOfClasses { get; set; }
        public static int numberOfPermutations { get; set; }

        //other variables
        public static string suffix { get; set; }
        public static char breakCharInFile { get; set; }
        public static int indexToStartFrom { get; set; }
        public static int numOfCharges { get; set; }
        public static string logFile { get; set; }
        public static List<string> excludedPhenotypes { get; set; }
        public static List<string> normCovars { get; set; }

        //what to print
        public static bool printTheMetaboliteDetails { get; set; }
        public static bool printBoxplots { get; set; }
        public static bool printScatterplots { get; set; }
        public static bool printPathwaysForMetabolites { get; set; }
        public static bool printMetaboliteStatistics { get; set; }
        public static bool printCorrelationsMetabolitesToCovariates { get; set; }
        public static bool printMetabolitesForDatabase { get; set; }
        public static bool printRosettaDatasets { get; set; }
        public static bool printCorrelationsMetabolitesToMetabolites { get; set; }
        public static bool printOutputForMoDentify { get; set; }
        public static bool printRatiosOfMetabolites { get; set; }
        public static bool printRegressionStatistics { get; set; }

        public static void setOtherVariables()
        {
            numberOfPermutations = 100000;
            suffix = Path.GetExtension(@"" + Directory.GetFiles(@"" + inputMSFilesDir).Where(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString())).First());
            breakCharInFile = (suffix == ".tsv") ? '\t' : ',';
            indexToStartFrom = getIndexWhereToStartFrom();
            numOfCharges = (prefix == prefixValues.gcms) ? 1 : ((prefix == prefixValues.mixed) ? 3 : 2);
            checkInputVariables();
            logFile = outputDir + "metaboliteDetails.log";
            outputToLog.initializeLogFile(logFile);
            rEngineInstance.initializeREngine("", "");
            excludedPhenotypes = new List<string>() { "U" };
            normCovars = new List<string>() { "HbA1c", "GSIS", "sampleWeight" }.Select(x => x.ToLower()).ToList();

            printTheMetaboliteDetails = true;
            printBoxplots = false;
            printScatterplots = false;
            printPathwaysForMetabolites = false;
            printMetaboliteStatistics = true;
            printCorrelationsMetabolitesToCovariates = false;
            printMetabolitesForDatabase = false;
            printRosettaDatasets = false;
            printCorrelationsMetabolitesToMetabolites = false;
            printOutputForMoDentify = false;
            printRatiosOfMetabolites = false;
            printRegressionStatistics = true;
        }

        public static void Close()
        {
            rEngineInstance.disposeREngine();
            outputToLog.closeLogFile();
        }

        private static int getIndexWhereToStartFrom()
        {
            using (TextReader input = new StreamReader(@"" + Directory.GetFiles(@"" + inputMSFilesDir).First(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString()) && x.EndsWith(suffix))))
            {
                input.ReadLine();
                return (input.ReadLine().Split(breakCharInFile).ToList().IndexOf("Metabolite"));
            }
        }

        public static void setPrefix(string p)
        {
            switch (p.ToLower())
            {
                case "lcms":
                    prefix = prefixValues.lcms;
                    break;
                case "gcms":
                    prefix = prefixValues.gcms;
                    break;
                case "mixed":
                    prefix = prefixValues.mixed;
                    break;
                default:
                    Console.WriteLine("The prefix was incorrectly defined!");
                    Environment.Exit(0);
                    break;
            }
        }

        public static void setNumberOfClasses()
        {
            switch (clinicalData.List_clinicalData.Select(x => x.Phenotype).Where(x => !excludedPhenotypes.Contains(x)).Distinct().Count())
            {
                case 2:
                    numberOfClasses = numberOfClassesValues.two;
                    break;
                case 3:
                    numberOfClasses = numberOfClassesValues.three;
                    break;
                default:
                    Console.WriteLine("Number of classes was incorrectly defined as " +
                        clinicalData.List_clinicalData.Select(x => x.Phenotype).Where(x => !excludedPhenotypes.Contains(x)).Distinct().Count().ToString() + "!");
                    Environment.Exit(0);
                    break;
            }
        }

        private static void checkInputVariables()
        {
            if (Directory.GetFiles(@"" + inputMSFilesDir).Where(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString()) && x.EndsWith(suffix)).Count() == 0)
            {
                outputToLog.WriteErrorLine("No input files detected with the given input (prefix and csv files directory)!");
            }

            if (!File.Exists(@"" + clinicalDataFile))
            {
                outputToLog.WriteErrorLine("No clinical data file detected!");
            }

            if (!File.Exists(@"" + databaseFile))
            {
                outputToLog.WriteErrorLine("No clinical data file detected!");
            }

            if (!Directory.Exists(@"" + outputDir))
            {
                Directory.CreateDirectory(@"" + outputDir);
            }
            else
            {
                if (Directory.GetFiles(@"" + outputDir).Count() > 0 || Directory.GetDirectories(@"" + outputDir).Count() > 0)
                {
                    foreach (string f in Directory.GetFiles(@"" + outputDir))
                    {
                        File.Delete(@"" + f);
                    }
                    foreach (string d in Directory.GetDirectories(@"" + outputDir))
                    {
                        Directory.Delete(@"" + d, true);
                    }
                }
            }

            if (numberOfPermutations > 1000000 || numberOfPermutations < 100)
            {
                outputToLog.WriteErrorLine("The number of permutations was incorrectly set!");
            }
        }
    }
}
