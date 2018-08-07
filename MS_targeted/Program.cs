using System;
using System.Diagnostics;
using metabolomicsDB;

namespace MS_targeted
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("[mono] MS_targeted.exe dir_with_input_files clinical_data_file combined_db_file output_dir prefix");
                Console.WriteLine();
                Console.WriteLine("This script performs the full analysis of the LC/GCMS data for p-values, correlations, ratios, pathways, database output and ROSETTA datasets.");
                Console.WriteLine();
                Console.WriteLine("Input:");
                Console.WriteLine("dir_with_input_files (string)");
                Console.WriteLine("\tdirectory containing a collection of tab or comma separated files with the metabolite values");
                Console.WriteLine("\tthese files should be tab seprated, should all start with the same prefix and have the same suffix");
                Console.WriteLine("\tthe metabolites values should be normalized prior to running this pipeline");
                Console.WriteLine("clinical_data_file (string)");
                Console.WriteLine("\tfile containing a collection of tab or comma separated values of clinical data for each donor");
                Console.WriteLine("combined_db_file (string)");
                Console.WriteLine("\ta tab-separated file containing all the information for pathways, IDs etc. Should be the output from metabolomicsDB.exe");
                Console.WriteLine("output_dir (string)");
                Console.WriteLine("\toutput directory");
                Console.WriteLine("prefix (string)");
                Console.WriteLine("\tprefix of the files in the input directory.");
                Console.WriteLine("\tsuggested that as prefix we use lcms, gcms or mixed so that we make the task easier.");
                Environment.Exit(0);
            }

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

            outputToLog.WriteLine("reading the tab-separated metadata file for the patients");
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
        }
    }
}
