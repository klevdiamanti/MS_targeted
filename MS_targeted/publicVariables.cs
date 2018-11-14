using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class publicVariables
    {
        #region command line input arguments
        public static string inputMSFilesDir { get; set; }
        public static string clinicalDataFile { get; set; }
        public static string databaseFile { get; set; }
        public static string outputDir { get; set; }
        public static string configurationFile { get; set; }
        #endregion

        #region variables from configuration file
        //number of permutations
        public static int numberOfPermutations { get; set; }
        private static bool numberOfPermutationsIsSet = false;

        //prefix
        public enum prefixValues
        {
            lcms,
            gcms,
            mixed
        }
        public static prefixValues prefix { get; set; }
        private static bool prefixIsSet = false;

        //suffix
        public static string suffix { get; set; }
        private static bool suffixIsSet = false;

        //FileTabSeparator
        public static char breakCharInFile { get; set; }
        private static bool breakCharInFileIsSet = false;

        //number of charges
        public static int numOfCharges { get; set; }
        private static bool numOfChargesIsSet = false;

        //index to start from
        public static int indexToStartFrom { get; set; }
        public static bool indexToStartFromIsSet = false;

        //R_HOME
        private static string R_HOME { get; set; }
        private static bool R_HOMEisSet = false;

        //R_DLL
        private static string R_DLL { get; set; }
        private static bool R_DLLisSet = false;

        //R_PACKAGES
        private static string R_packages { get; set; }
        private static bool R_packagesisSet = false;

        //excluded phenotypes
        public static List<string> excludedPhenotypes { get; set; }
        private static bool excludedPhenotypesIsSet = false;

        //normalization covariates
        public static List<string> normCovars { get; set; }
        private static bool normCovarsIsSet = false;

        //cofounder covars
        public static List<string> cofCovars { get; set; }
        private static bool cofCovarsIsSet = false;

        //sample weight covariates
        public static List<Tuple<string, string, string>> sampleWeight { get; set; }
        private static bool sampleWeightIsSet = false;

        //sample ID in metadata file
        public static string sampleId { get; set; }
        private static bool sampleIdIsSet = false;

        //decision/phenotype in metadata file
        public static string phenotype { get; set; }
        private static bool phenotypeIsSet = false;

        public static int parametricTestThreshold { get; set; }
        private static bool parametricTestThresholdIsSet = false;

        //what to print
        public static bool printTheMetaboliteDetails = false;
        public static bool printBoxplots = false;
        public static bool printScatterplots = false;
        public static bool printPathwaysForMetabolites = false;
        public static bool printMetaboliteStatistics = false;
        public static bool printCorrelationsMetabolitesToCovariates = false;
        public static bool printMetabolitesForDatabase = false;
        public static bool printRosettaDatasets = false;
        public static bool printCorrelationsMetabolitesToMetabolites = false;
        public static bool printOutputForMoDentify = false;
        public static bool printRatiosOfMetabolites = false;
        public static bool printRegressionStatistics = false;
        #endregion

        #region additional variables
        //number of phenotypic classes
        public enum numberOfClassesValues
        {
            two,
            more
        }
        public static numberOfClassesValues numberOfClasses { get; set; }

        //initialize REngine
        private static bool initializeREngineIsSet = false;
        #endregion

        public static void setOtherVariables()
        {
            checkInputFilesAndDirs();

            #region read variables from configuration file
            string R_HOME = "", R_DLL = "";
            using (TextReader input = new StreamReader(@"" + configurationFile))
            {
                string line;
                int tmpInt;
                while ((line = input.ReadLine()) != null)
                {
                    //ignore comment lines
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    else
                    {
                        switch (line.Split('\t').First())
                        {
                            case "NumberOfPermutations":
                                #region set numberOfPermutations
                                if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0)
                                {
                                    numberOfPermutations = tmpInt;
                                    numberOfPermutationsIsSet = true;
                                    outputToLog.WriteLine("NumberOfPermutations was successfully set as " + numberOfPermutations);
                                }
                                else
                                {
                                    numberOfPermutationsIsSet = false;
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not integer or positive");
                                }
                                #endregion
                                break;
                            case "ParametricTestThreshold":
                                #region set ParametricTestThreshold
                                if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0)
                                {
                                    parametricTestThreshold = tmpInt;
                                    parametricTestThresholdIsSet = true;
                                    outputToLog.WriteLine("ParametricTestThreshold was successfully set as " + parametricTestThreshold);
                                }
                                else
                                {
                                    parametricTestThresholdIsSet = false;
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not integer or positive");
                                }
                                #endregion
                                break;
                            case "Prefix":
                                #region set prefix
                                if (line.Split('\t').ElementAt(1).ToLower() == "lcms")
                                {
                                    prefix = prefixValues.lcms;
                                    prefixIsSet = true;
                                    outputToLog.WriteLine("Prefix was successfully set as " + prefix);
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "gcms")
                                {
                                    prefix = prefixValues.gcms;
                                    prefixIsSet = true;
                                    outputToLog.WriteLine("Prefix was successfully set as " + prefix);
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "mixed")
                                {
                                    prefix = prefixValues.mixed;
                                    prefixIsSet = true;
                                    outputToLog.WriteLine("Prefix was successfully set as " + prefix);
                                }
                                else
                                {
                                    prefixIsSet = false;
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not lcms, gcms or mixed");
                                }

                                //Check wether there is at least one file with the given prefix
                                if (Directory.GetFiles(@"" + inputMSFilesDir).Count(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString())) == 0)
                                {
                                    outputToLog.WriteErrorLine("There are 0 files in the input directory for the given prefix (" + prefix + ")");
                                }
                                else
                                {
                                    outputToLog.WriteLine("Detected "+ Directory.GetFiles(@"" + inputMSFilesDir).Count(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString())).ToString()
                                        + " files in the input directory for the given prefix (" + prefix + ")");
                                }
                                #endregion
                                #region set suffix
                                //Try to set file suffix from existing input files
                                if (!suffixIsSet)
                                {
                                    if (Directory.GetFiles(@"" + inputMSFilesDir).Where(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString()))
                                    .Select(x => Path.GetExtension(x)).Distinct().Count() == 1)
                                    {
                                        suffix = Path.GetExtension(@"" + Directory.GetFiles(@"" + inputMSFilesDir)
                                            .Where(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString())).First());
                                        suffixIsSet = true;
                                        outputToLog.WriteLine("Suffix was successfully set as " + suffix);
                                    }
                                    else
                                    {
                                        suffixIsSet = false;
                                        outputToLog.WriteWarningLine("Suffix could not be set automatically. Should be set from configuration file.");
                                    }

                                    #region set breakCharInFile
                                    if (!breakCharInFileIsSet)
                                    {
                                        if (suffixIsSet)
                                        {
                                            //Try to set breakCharInFile
                                            if (suffix == ".tsv")
                                            {
                                                breakCharInFile = '\t';
                                                breakCharInFileIsSet = true;
                                                outputToLog.WriteLine("FileTabSeparator was successfully set as [tab]");
                                            }
                                            else if (suffix == ".csv")
                                            {
                                                breakCharInFile = ',';
                                                breakCharInFileIsSet = true;
                                                outputToLog.WriteLine("FileTabSeparator was successfully set as " + breakCharInFile.ToString());
                                            }
                                            else
                                            {
                                                breakCharInFileIsSet = false;
                                                outputToLog.WriteWarningLine("FileTabSeparator could not be set automatically. Should be set from configuration file.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        breakCharInFileIsSet = true;
                                        if (breakCharInFile == '\t')
                                        {
                                            outputToLog.WriteLine("FileTabSeparator was successfully set as [tab]");
                                        }
                                        else
                                        {
                                            outputToLog.WriteLine("FileTabSeparator has been already set to " + breakCharInFile.ToString());
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    suffixIsSet = true;
                                    outputToLog.WriteLine("Suffix has been already set as " + suffix);
                                }
                                #endregion
                                #region set numOfCharges
                                if (numOfCharges == 0)
                                {
                                    if (prefix == prefixValues.gcms)
                                    {
                                        numOfCharges = 1;
                                        outputToLog.WriteLine("NumberOfCharges was successfully set as " + numOfCharges);
                                    }
                                    else if (prefix == prefixValues.lcms)
                                    {
                                        numOfCharges = 2;
                                        outputToLog.WriteLine("NumberOfCharges was successfully set as " + numOfCharges);
                                    }
                                    else if (prefix == prefixValues.mixed)
                                    {
                                        numOfCharges = 3;
                                        outputToLog.WriteLine("NumberOfCharges was successfully set as " + numOfCharges);
                                    }
                                    else
                                    {
                                        outputToLog.WriteWarningLine("NumberOfCharges could not be set automatically. Should be set from configuration file.");
                                    }
                                }
                                #endregion
                                #region set index of numerical values
                                if (prefixIsSet && suffixIsSet)
                                {
                                    using (TextReader inputTmp = new StreamReader(@"" + Directory.GetFiles(@"" + inputMSFilesDir).First(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix.ToString()) && x.EndsWith(suffix))))
                                    {
                                        inputTmp.ReadLine();
                                        string lineTmp = inputTmp.ReadLine();
                                        if (lineTmp.Split(breakCharInFile).ToList().IndexOf("Metabolite") == -1)
                                        {
                                            indexToStartFromIsSet = false;
                                            outputToLog.WriteWarningLine("Index of the column where numerical values start could not be automatically defined!");
                                        }
                                        else
                                        {
                                            indexToStartFrom = lineTmp.Split(breakCharInFile).ToList().IndexOf("Metabolite");
                                            indexToStartFromIsSet = true;
                                            outputToLog.WriteLine("Index of the column where numerical values start was successfully set as " + indexToStartFrom);
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case "Suffix":
                                #region set suffix
                                if (suffixIsSet)
                                {
                                    if (suffix == line.Split('\t').ElementAt(1))
                                    {
                                        suffixIsSet = true;
                                        outputToLog.WriteLine("Suffix was confirmed as " + suffix);
                                    }
                                    else
                                    {
                                        suffix = line.Split('\t').ElementAt(1);
                                        suffixIsSet = true;
                                        outputToLog.WriteWarningLine("Automatically detected suffix and the one from the configuration file do not match. Keeping the one from the configuration file!");
                                    }
                                }
                                else
                                {
                                    suffix = line.Split('\t').ElementAt(1);
                                    suffixIsSet = true;
                                    outputToLog.WriteLine("Suffix was successfully set as " + suffix);
                                }
                                #endregion
                                #region set breakCharInFile
                                if (!breakCharInFileIsSet)
                                {
                                    if (suffixIsSet)
                                    {
                                        //Try to set breakCharInFile
                                        if (suffix == ".tsv")
                                        {
                                            breakCharInFile = '\t';
                                            breakCharInFileIsSet = true;
                                            outputToLog.WriteLine("FileTabSeparator was successfully set as [tab]");
                                        }
                                        else if (suffix == ".csv")
                                        {
                                            breakCharInFile = ',';
                                            breakCharInFileIsSet = true;
                                            outputToLog.WriteLine("FileTabSeparator was successfully set as " + breakCharInFile.ToString());
                                        }
                                        else
                                        {
                                            breakCharInFileIsSet = false;
                                            outputToLog.WriteWarningLine("FileTabSeparator could not be set automatically. Should be set from configuration file.");
                                        }
                                    }
                                }
                                else
                                {
                                    breakCharInFileIsSet = true;
                                    if (breakCharInFile == '\t')
                                    {
                                        outputToLog.WriteLine("FileTabSeparator has been already set to [tab]");
                                    }
                                    else
                                    {
                                        outputToLog.WriteLine("FileTabSeparator has been already set to " + breakCharInFile.ToString());
                                    }
                                }
                                #endregion
                                break;
                            case "FileTabSeparator":
                                #region set breakCharInFile
                                if (breakCharInFileIsSet)
                                {
                                    if (breakCharInFile == '\t' && line.Split('\t').ElementAt(1) == "tab")
                                    {
                                        breakCharInFileIsSet = true;
                                        outputToLog.WriteLine("FileTabSeparator was confirmed as [tab]");
                                    }
                                    else if (breakCharInFile == line.Split('\t').ElementAt(1).First())
                                    {
                                        breakCharInFileIsSet = true;
                                        outputToLog.WriteLine("FileTabSeparator was confirmed as " + breakCharInFile.ToString());
                                    }
                                    else
                                    {
                                        if (line.Split('\t').ElementAt(1) == "tab")
                                        {
                                            breakCharInFile = '\t';
                                        }
                                        else
                                        {
                                            breakCharInFile = line.Split('\t').ElementAt(1).First();
                                        }
                                        breakCharInFileIsSet = true;
                                        outputToLog.WriteWarningLine("Automatically detected FileTabSeparator and the one from the configuration file do not match. Keeping the one from the configuration file!");
                                    }
                                }
                                else
                                {
                                    if (line.Split('\t').ElementAt(1) == "tab")
                                    {
                                        breakCharInFile = '\t';
                                        breakCharInFileIsSet = true;
                                        outputToLog.WriteLine("FileTabSeparator was confirmed as [tab]");
                                    }
                                    else
                                    {
                                        breakCharInFile = line.Split('\t').ElementAt(1).First();
                                        breakCharInFileIsSet = true;
                                        outputToLog.WriteLine("FileTabSeparator was confirmed as " + breakCharInFile.ToString());
                                    }
                                }
                                #endregion
                                break;
                            case "NumberOfCharges":
                                #region set numOfCharges
                                if (numOfChargesIsSet)
                                {
                                    if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0 && numOfCharges == tmpInt)
                                    {
                                        numOfChargesIsSet = true;
                                        outputToLog.WriteLine("NumberOfCharges was confirmed as " + numOfCharges);
                                    }
                                    else if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0)
                                    {
                                        numOfCharges = tmpInt;
                                        numOfChargesIsSet = true;
                                        outputToLog.WriteWarningLine("Automatically detected NumberOfCharges and the one from the configuration file do not match. Keeping the one from the configuration file!");
                                    }
                                    else
                                    {
                                        numOfChargesIsSet = false;
                                        outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not integer or positive");
                                    }
                                }
                                else
                                {
                                    if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0)
                                    {
                                        numOfCharges = tmpInt;
                                        numOfChargesIsSet = true;
                                        outputToLog.WriteLine("NumberOfCharges was successfully set as " + numOfCharges);
                                    }
                                    else
                                    {
                                        numOfChargesIsSet = false;
                                        outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not integer or positive");
                                    }
                                }
                                #endregion
                                break;
                            case "IndexOfNumericValues":
                                #region set IndexOfNumericValues
                                if (indexToStartFromIsSet)
                                {
                                    if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0 && indexToStartFrom == tmpInt)
                                    {
                                        indexToStartFromIsSet = true;
                                        outputToLog.WriteLine("IndexOfNumericValues was confirmed as " + indexToStartFrom);
                                    }
                                    else if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0)
                                    {
                                        indexToStartFrom = tmpInt;
                                        indexToStartFromIsSet = true;
                                        outputToLog.WriteWarningLine("Automatically detected IndexOfNumericValues and the one from the configuration file do not match. Keeping the one from the configuration file!");
                                    }
                                    else
                                    {
                                        indexToStartFromIsSet = false;
                                        outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not integer or positive");
                                    }
                                }
                                else
                                {
                                    if (int.TryParse(line.Split('\t').ElementAt(1), out tmpInt) && tmpInt > 0)
                                    {
                                        indexToStartFrom = tmpInt;
                                        indexToStartFromIsSet = true;
                                        outputToLog.WriteLine("IndexOfNumericValues was successfully set as " + indexToStartFrom);
                                    }
                                    else
                                    {
                                        indexToStartFromIsSet = false;
                                        outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not integer or positive");
                                    }
                                }
                                #endregion
                                break;
                            case "R_HOME":
                                #region set R_HOME
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1))
                                    && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1))
                                    && !Directory.Exists(@"" + line.Split('\t').ElementAt(1)))
                                {
                                    R_HOMEisSet = false;
                                    outputToLog.WriteErrorLine("R_HOME directory does not exist");
                                }
                                else
                                {
                                    R_HOME = line.Split('\t').ElementAt(1);
                                    R_HOMEisSet = true;
                                    outputToLog.WriteLine("R_HOME was successfully set as " + R_HOME);
                                }
                                #endregion
                                break;
                            case "R_DLL":
                                #region set R_DLL
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1))
                                    && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1))
                                    && !File.Exists(@"" + line.Split('\t').ElementAt(1)))
                                {
                                    R_DLLisSet = false;
                                    outputToLog.WriteErrorLine("R_DLL file does not exist");
                                }
                                else
                                {
                                    R_DLL = line.Split('\t').ElementAt(1);
                                    R_DLLisSet = true;
                                    outputToLog.WriteLine("R_DLL was successfully set as " + R_DLL);
                                }
                                #endregion
                                break;
                            case "R_PACKAGES":
                                #region set R_PACKAGES
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1))
                                    && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1))
                                    && !Directory.Exists(@"" + line.Split('\t').ElementAt(1)))
                                {
                                    R_packagesisSet = false;
                                    outputToLog.WriteErrorLine("R_PACKAGES directory does not exist");
                                }
                                else
                                {
                                    R_packages = line.Split('\t').ElementAt(1);
                                    R_packagesisSet = true;
                                    outputToLog.WriteLine("R_PACKAGES was successfully set as " + R_packages);
                                }
                                #endregion
                                break;
                            case "ExcludedPhenotypes":
                                #region set ExcludedPhenotypes
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1)) && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1)))
                                {
                                    excludedPhenotypes = line.Split('\t').ElementAt(1).Split(',').ToList();
                                    excludedPhenotypesIsSet = true;
                                    outputToLog.WriteLine("ExcludedPhenotypes was successfully set");
                                }
                                else
                                {
                                    excludedPhenotypesIsSet = true;
                                    excludedPhenotypes = new List<string>();
                                    outputToLog.WriteWarningLine("ExcludedPhenotypes was successfully set as empty");
                                }
                                #endregion
                                break;
                            case "CovarCorrelate":
                                #region set CovarCorrelate
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1)) && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1)))
                                {
                                    normCovars = line.Split('\t').ElementAt(1).Split(',').Select(x => x.ToLower().Replace("-", "_").Replace(" ", "_").Replace("=", "_")).ToList();
                                    normCovarsIsSet = true;
                                    outputToLog.WriteLine("CovarCorrelate was successfully set");
                                }
                                else
                                {
                                    normCovarsIsSet = true;
                                    normCovars = new List<string>();
                                    outputToLog.WriteWarningLine("CovarCorrelate was successfully set as empty");
                                }
                                #endregion
                                break;
                            case "CovarSampleWeight":
                                #region set CovarSampleWeight
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1)) && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1)))
                                {
                                    sampleWeight = line.Split('\t').ElementAt(1).Split(',').Select(x => new Tuple<string, string, string>
                                    (x.Split('#').First().ToLower().Replace("-", "_").Replace(" ", "_").Replace("=", "_"), x.Split('#').Last().Split('_').First().ToLower(),
                                    x.Split('#').Last().Split('_').Last().ToLower()))
                                    .ToList();
                                    sampleWeightIsSet = true;
                                    outputToLog.WriteLine("CovarSampleWeight was successfully set");
                                }
                                else
                                {
                                    sampleWeightIsSet = true;
                                    sampleWeight = new List<Tuple<string, string, string>>();
                                    outputToLog.WriteWarningLine("CovarSampleWeight was successfully set as empty");
                                }
                                #endregion
                                break;
                            case "Cofounders":
                                #region set Cofounders
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1)) && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1)))
                                {
                                    cofCovars = line.Split('\t').ElementAt(1).Split(',').Select(x => x.ToLower().Replace("-", "_").Replace(" ", "_").Replace("=", "_")).ToList();
                                    cofCovarsIsSet = true;
                                    outputToLog.WriteLine("Cofounders was successfully set");
                                }
                                else
                                {
                                    cofCovarsIsSet = true;
                                    cofCovars = new List<string>();
                                    outputToLog.WriteWarningLine("Cofounders was successfully set as empty");
                                }
                                #endregion
                                break;
                            case "CovarId":
                                #region set CovarId
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1)) && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1)))
                                {
                                    sampleId = line.Split('\t').ElementAt(1).ToLower();
                                    sampleIdIsSet = true;
                                    outputToLog.WriteLine("CovarId was successfully set as " + sampleId);
                                }
                                else
                                {
                                    sampleIdIsSet = false;
                                    outputToLog.WriteWarningLine("CovarId cannot be set as empty");
                                }
                                #endregion
                                break;
                            case "CovarDecision":
                                #region set CovarDecision
                                if (!string.IsNullOrEmpty(line.Split('\t').ElementAt(1)) && !string.IsNullOrWhiteSpace(line.Split('\t').ElementAt(1)))
                                {
                                    phenotype = line.Split('\t').ElementAt(1).ToLower();
                                    phenotypeIsSet = true;
                                    outputToLog.WriteLine("CovarDecision was successfully set as " + phenotype);
                                }
                                else
                                {
                                    phenotypeIsSet = false;
                                    outputToLog.WriteWarningLine("CovarDecision cannot be set as empty");
                                }
                                
                                #endregion
                                break;
                            case "PrintMetaboliteDetails":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printTheMetaboliteDetails = true;
                                    outputToLog.WriteLine("PrintMetaboliteDetails was successfully set as " + printTheMetaboliteDetails.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printTheMetaboliteDetails = false;
                                    outputToLog.WriteLine("PrintMetaboliteDetails was successfully set as " + printTheMetaboliteDetails.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintBoxplots":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printBoxplots = true;
                                    outputToLog.WriteLine("PrintBoxplots was successfully set as " + printBoxplots.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printBoxplots = false;
                                    outputToLog.WriteLine("PrintBoxplots was successfully set as " + printBoxplots.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintScatterplots":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printScatterplots = true;
                                    outputToLog.WriteLine("PrintScatterplots was successfully set as " + printScatterplots.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printScatterplots = false;
                                    outputToLog.WriteLine("PrintScatterplots was successfully set as " + printScatterplots.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintPathwaysForMetabolites":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printPathwaysForMetabolites = true;
                                    outputToLog.WriteLine("PrintPathwaysForMetabolites was successfully set as " + printPathwaysForMetabolites.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printPathwaysForMetabolites = false;
                                    outputToLog.WriteLine("PrintPathwaysForMetabolites was successfully set as " + printPathwaysForMetabolites.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintMetaboliteStatistics":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printMetaboliteStatistics = true;
                                    outputToLog.WriteLine("PrintMetaboliteStatistics was successfully set as " + printMetaboliteStatistics.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printMetaboliteStatistics = false;
                                    outputToLog.WriteLine("PrintMetaboliteStatistics was successfully set as " + printMetaboliteStatistics.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintCorrelationsMetabolitesToCovariates":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printCorrelationsMetabolitesToCovariates = true;
                                    outputToLog.WriteLine("PrintCorrelationsMetabolitesToCovariates was successfully set as " + printCorrelationsMetabolitesToCovariates.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printCorrelationsMetabolitesToCovariates = false;
                                    outputToLog.WriteLine("PrintCorrelationsMetabolitesToCovariates was successfully set as " + printCorrelationsMetabolitesToCovariates.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintMetabolitesForDatabase":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printMetabolitesForDatabase = true;
                                    outputToLog.WriteLine("PrintMetabolitesForDatabase was successfully set as " + printMetabolitesForDatabase.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printMetabolitesForDatabase = false;
                                    outputToLog.WriteLine("PrintMetabolitesForDatabase was successfully set as " + printMetabolitesForDatabase.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintMachineLearningDatasets":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printRosettaDatasets = true;
                                    outputToLog.WriteLine("PrintMachineLearningDatasets was successfully set as " + printRosettaDatasets.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printRosettaDatasets = false;
                                    outputToLog.WriteLine("PrintMachineLearningDatasets was successfully set as " + printRosettaDatasets.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintCorrelationsMetabolitesToMetabolites":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printCorrelationsMetabolitesToMetabolites = true;
                                    outputToLog.WriteLine("PrintCorrelationsMetabolitesToMetabolites was successfully set as " + printCorrelationsMetabolitesToMetabolites.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printCorrelationsMetabolitesToMetabolites = false;
                                    outputToLog.WriteLine("PrintCorrelationsMetabolitesToMetabolites was successfully set as " + printCorrelationsMetabolitesToMetabolites.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintOutputForMoDentify":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printOutputForMoDentify = true;
                                    outputToLog.WriteLine("PrintOutputForMoDentify was successfully set as " + printOutputForMoDentify.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printOutputForMoDentify = false;
                                    outputToLog.WriteLine("PrintOutputForMoDentify was successfully set as " + printOutputForMoDentify.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintRatiosOfMetabolites":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printRatiosOfMetabolites = true;
                                    outputToLog.WriteLine("PrintRatiosOfMetabolites was successfully set as " + printRatiosOfMetabolites.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printRatiosOfMetabolites = false;
                                    outputToLog.WriteLine("PrintRatiosOfMetabolites was successfully set as " + printRatiosOfMetabolites.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "PrintRegressionStatistics":
                                #region
                                if (line.Split('\t').ElementAt(1).ToLower() == "true")
                                {
                                    printRegressionStatistics = true;
                                    outputToLog.WriteLine("PrintRegressionStatistics was successfully set as " + printRegressionStatistics.ToString().ToUpper());
                                }
                                else if (line.Split('\t').ElementAt(1).ToLower() == "false")
                                {
                                    printRegressionStatistics = false;
                                    outputToLog.WriteLine("PrintRegressionStatistics was successfully set as " + printRegressionStatistics.ToString().ToUpper());
                                }
                                else
                                {
                                    outputToLog.WriteErrorLine("Parameter " + line.Split('\t').First() + " from configuration file was not TRUE or FALSE");
                                }
                                #endregion
                                break;
                            case "LogFileName":
                                #region set LogFileName
                                outputToLog.WriteLine("LogFileName was successfully set as " + outputToLog.logFile);
                                #endregion
                                break;
                            default:
                                outputToLog.WriteErrorLine("The term " + line.Split('\t').First() + " from configuration file was cannot be recognized");
                                break;
                        }
                    }
                }
            }
            #endregion

            //initialize REngine
            if (R_HOMEisSet && R_DLLisSet && R_packagesisSet)
            {
                rEngineInstance.initializeREngine(R_HOME, R_DLL, R_packages);
                initializeREngineIsSet = true;
            }
            else
            {
                outputToLog.WriteErrorLine("R_HOME, R_DLL or R_PACKAGES have not been set in the configuration file");
            }

            #region check if initialization were done properly
            if (!numberOfPermutationsIsSet)
            {
                outputToLog.WriteErrorLine("NumberOfPermutations has not been set in the configuration file");
            }
            else if (!prefixIsSet)
            {
                outputToLog.WriteErrorLine("Prefix has not been set in the configuration file");
            }
            else if (!suffixIsSet)
            {
                outputToLog.WriteErrorLine("Suffix has not been set in the configuration file");
            }
            else if (!breakCharInFileIsSet)
            {
                outputToLog.WriteErrorLine("FileTabSeparator has not been set in the configuration file");
            }
            else if (!numOfChargesIsSet)
            {
                outputToLog.WriteErrorLine("NumberOfCharges has not been set in the configuration file");
            }
            else if (!indexToStartFromIsSet)
            {
                outputToLog.WriteErrorLine("IndexOfNumericValues has not been set in the configuration file");
            }
            else if (!excludedPhenotypesIsSet)
            {
                outputToLog.WriteErrorLine("ExcludedPhenotypes has not been set in the configuration file. It can be set to empty/NULL too");
            }
            else if (!normCovarsIsSet)
            {
                outputToLog.WriteErrorLine("CorrelationCovariates has not been set in the configuration file. It can be set to empty/NULL too");
            }
            else if (!sampleWeightIsSet)
            {
                outputToLog.WriteErrorLine("CovarSampleWeight has not been set in the configuration file. It can be set to empty/NULL too");
            }
            else if (!cofCovarsIsSet)
            {
                outputToLog.WriteErrorLine("Cofounders has not been set in the configuration file. It can be set to empty/NULL too");
            }
            else if (!sampleIdIsSet)
            {
                outputToLog.WriteErrorLine("CovarId has not been set in the configuration file");
            }
            else if (!phenotypeIsSet)
            {
                outputToLog.WriteErrorLine("CovarDecision has not been set in the configuration file");
            }
            else if (!initializeREngineIsSet)
            {
                outputToLog.WriteErrorLine("REngine could not be initialized because R_HOME or R_DLL are not in the configuration file");
            }
            else if (!parametricTestThresholdIsSet)
            {
                outputToLog.WriteErrorLine("ParametricTestThreshold has not been set in the configuration file");
            }
            #endregion

            checkCovariateInputs();
        }

        private static void checkInputFilesAndDirs()
        {
            //check if input MS files directory exists
            if (!Directory.Exists(@"" + inputMSFilesDir))
            {
                Console.WriteLine("Clinical data file does not exist!");
                Environment.Exit(0);
            }

            //check if clinical data file exists
            if (!File.Exists(@"" + clinicalDataFile))
            {
                Console.WriteLine("Clinical data file does not exist!");
                Environment.Exit(0);
            }

            //check if database file exists
            if (!File.Exists(@"" + databaseFile))
            {
                Console.WriteLine("Database file does not exist!");
                Environment.Exit(0);
            }

            //check if output direcotry exists
            //if yes empty it
            //if no create it
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

            //check if configuration file exists
            if (!File.Exists(@"" + configurationFile))
            {
                Console.WriteLine("Configuration file does not exist!");
                Environment.Exit(0);
            }

            //create log file
            using (TextReader input = new StreamReader(@"" + configurationFile))
            {
                string line, logFile = "";
                while ((line = input.ReadLine()) != null)
                {
                    if (!line.StartsWith("#") && line.Split('\t').First() == "LogFileName")
                    {
                        if (!File.Exists(@"" + outputDir + Path.DirectorySeparatorChar + line.Split('\t').ElementAt(1)))
                        {
                            logFile = outputDir + Path.DirectorySeparatorChar + line.Split('\t').ElementAt(1);
                            outputToLog.initializeLogFile(outputDir + Path.DirectorySeparatorChar + line.Split('\t').ElementAt(1));
                        }
                        else
                        {
                            Console.WriteLine("Log file could not be created!");
                            Environment.Exit(0);
                        }
                    }
                }
                if (string.IsNullOrEmpty(logFile) || string.IsNullOrWhiteSpace(logFile) || !File.Exists(@"" + logFile))
                {
                    Console.WriteLine("Log file could not be created!");
                    Environment.Exit(0);
                }
            }
        }

        private static void checkCovariateInputs()
        {
            #region check for duplicates
            if (normCovars.Count > 0 && normCovars.Count != normCovars.Distinct().Count())
            {
                outputToLog.WriteErrorLine("CovarCorrelate contains duplicates in the configuration file");
            }

            if (sampleWeight.Count > 0 && sampleWeight.Count != sampleWeight.Select(x => x.Item1).Distinct().Count())
            {
                outputToLog.WriteErrorLine("CovarSampleWeight contains duplicates (names before hashtag) in the configuration file");
            }

            if (sampleWeight.Count > 0 && sampleWeight.Count != sampleWeight.Select(x => x.Item2 + "_" + x.Item3).Distinct().Count())
            {
                outputToLog.WriteErrorLine("CovarSampleWeight contains duplicates (names after hashtag) in the configuration file");
            }
            #endregion

            #region check sample ID
            if (sampleId == phenotype)
            {
                outputToLog.WriteErrorLine("CovarId cannot be the same as the CovarDecision in the configuration file");
            }

            if (normCovars.Count > 0 && normCovars.Contains(sampleId))
            {
                outputToLog.WriteErrorLine("CovarId cannot be among the CovarCorrelate covariates in the configuration file");
            }

            if (sampleWeight.Count > 0 && sampleWeight.Any(x => x.Item1 == sampleId))
            {
                outputToLog.WriteErrorLine("CovarId cannot be among the CovarSampleWeight covariates in the configuration file");
            }
            #endregion

            #region check decision
            if (normCovars.Count > 0 && normCovars.Contains(phenotype))
            {
                outputToLog.WriteErrorLine("CovarDecision cannot be among the CovarCorrelate covariates in the configuration file");
            }

            if (sampleWeight.Count > 0 && sampleWeight.Any(x => x.Item1 == phenotype))
            {
                outputToLog.WriteErrorLine("CovarDecision cannot be among the CovarSampleWeight covariates in the configuration file");
            }
            #endregion

            #region check sample weight
            if (sampleWeight.Count == 0 && normCovars.Contains("sampleweight"))
            {
                outputToLog.WriteErrorLine("CovarCorrelate cannot contain sampleWeight as a covariate since it is not defined in the configuration file");
            }
            #endregion
        }

        public static void setNumberOfPhenotypicClasses()
        {
            switch (clinicalData.List_clinicalData.Select(x => x.Phenotype).Where(x => !excludedPhenotypes.Contains(x)).Distinct().Count())
            {
                case 2:
                    numberOfClasses = numberOfClassesValues.two;
                    break;
                case 3:
                    numberOfClasses = numberOfClassesValues.more;
                    break;
                default:
                    outputToLog.WriteErrorLine("Number of classes was incorrectly defined as " +
                        clinicalData.List_clinicalData.Select(x => x.Phenotype).Where(x => !excludedPhenotypes.Contains(x)).Distinct().Count().ToString());
                    break;
            }
        }

        public static void Close()
        {
            rEngineInstance.disposeREngine();
            outputToLog.closeLogFile();
        }
    }
}
