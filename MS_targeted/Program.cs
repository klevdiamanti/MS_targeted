using System;
using System.Collections;
using System.Collections.Generic;
using metabolomicsDB;

namespace MS_targeted
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("[mono] MS_targeted.exe dir_with_input_files clinical_data_file combined_db_file configuration_file output_dir");
                Console.WriteLine();
                Console.WriteLine("This pipeleine performs the full analysis of the LC/GCMS data for p-values, correlations, ratios, pathways and machine-learning datasets.");
                Console.WriteLine();
                Console.WriteLine("Input:");
                Console.WriteLine("dir_with_input_files (string)");
                Console.WriteLine("\tdirectory containing a collection of tab or comma separated files with the metabolite values.");
                Console.WriteLine("\tthese files should all start with the same prefix and have the same suffix.");
                Console.WriteLine("\tthe metabolites values should be normalized prior to running this pipeline.");
                Console.WriteLine("clinical_data_file (string)");
                Console.WriteLine("\tfile containing a collection of tab or comma separated values of clinical data for each sample");
                Console.WriteLine("combined_db_file (string)");
                Console.WriteLine("\ta tab-separated file containing all the information for pathways, IDs etc. Should be the output from metabolomicsDB.exe");
                Console.WriteLine("configuration_file (string)");
                Console.WriteLine("\tconfiguration file for the pipeline");
                Console.WriteLine("output_dir (string)");
                Console.WriteLine("\toutput directory");
                Console.WriteLine("");
                Console.WriteLine("Note that tab-separated files are preferred over comma-separated.");
                Environment.Exit(0);
            }

            //IEnumerable[] e = new IEnumerable[3];
            //List<int> a = new List<int>() { 1, 2, 3, 4, 5 };
            //List<int> b = new List<int>() { 4, 5, 6, 7, 8 };
            ////IEnumerable[] e = new IEnumerable[] { a.ToArray(), b.ToArray() };
            //List<int> c = new List<int>() { 4, 5, 6, 7, 8 };
            //e.SetValue(c.ToArray(), 2);
            ////e = new IEnumerable[] { e., c.ToArray() };

            Console.WriteLine("**** MS_targeted ****");
            Console.WriteLine("setting up input variables, files and folders");
            publicVariables.inputMSFilesDir = args[0];
            publicVariables.clinicalDataFile = args[1];
            publicVariables.databaseFile = args[2];
            publicVariables.configurationFile = args[3];
            publicVariables.outputDir = args[4];
            //set other public variables that depend on the input arguments
            publicVariables.setOtherVariables();

            outputToLog.WriteLine("reading the tab-separated metabolite database file");
            metabolites.Read_metaboliteDatabaseFromFile(publicVariables.databaseFile);

            outputToLog.WriteLine("reading the tab-separated metadata file");
            clinicalData.Read_ClinicalDataForSamples();

            //set number of phenotypic classes
            outputToLog.WriteLine("setting number of phenotypic classes");
            publicVariables.setNumberOfPhenotypicClasses();

            //read the compiled csv files
            outputToLog.WriteLine("reading the compiled csv files");
            metaboliteLevels.ReadInputMetabolites();

            //calculate statistics
            outputToLog.WriteLine("calculating metabolite statistics");
            metaboliteStats.StartMetaboliteStats();

            //print the files
            outputToLog.WriteLine("printing the output files");
            analysisOutput.printTheDetails();

            //close open streams and engines
            publicVariables.Close();
            Console.WriteLine("Done!");
        }
    }
}
