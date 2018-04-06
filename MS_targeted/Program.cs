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
                Console.WriteLine("[mono] MS_targeted.exe dir_with_input_files clinical_data_file combined_db_file output_dir prefix donors_source number_of_classes");
                Console.WriteLine();
                Console.WriteLine("This script performs the full analysis of the LC/GCMS data for p-values, correlations, ratios, pathways, database output and ROSETTA datasets.");
                Console.WriteLine();
                Console.WriteLine("Input:");
                Console.WriteLine("dir_with_raw_tsv_files (string)");
                Console.WriteLine("\tdirectory containing a collection of tab or comma separated files with the metabolite values (sample file attached)");
                Console.WriteLine("\tthese filenames should all start with a predefined prefix");
                Console.WriteLine("clinical_data_file (string)");
                Console.WriteLine("\tfile containing a collection of tab or comma separated values of clinical data for each donor");
                Console.WriteLine("combined_db_file (string)");
                Console.WriteLine("\ta tab-separated file containing all the information for pathways, IDs etc. Should be the output from compileMetaboliteDB.exe");
                Console.WriteLine("output_dir (string)");
                Console.WriteLine("\toutput directory");
                Console.WriteLine("prefix (string)");
                Console.WriteLine("\tprefix of the files in the input directory. Also used to denote the type of analysis that has been performed.");
                Console.WriteLine("\tSuggested that as prefix we use lcms or gcms so that we make the task easier.");
                Console.WriteLine("donors_source (string)");
                Console.WriteLine("\tsource where the samples have been received from. Used to denote the clinical data type of file that is to be read.");
                Console.WriteLine("\tIt should be exodiab or patients.");
                Console.WriteLine("number_of_classes (integer)");
                Console.WriteLine("\tnumber of phenotype classes to run for the analysis on.");
                Console.WriteLine();
                Console.WriteLine("Comment:");
                Console.WriteLine("\tThe data should be normalized prior to running this program!");
                Environment.Exit(0);
            }

            Stopwatch countTime = new Stopwatch();
            countTime.Start();

            Console.WriteLine("setting up input variables, files and folders");
            publicVariables.inputMSFilesDir = args[0];
            publicVariables.clinicalDataFile = args[1];
            publicVariables.databaseFile = args[2];
            publicVariables.outputDir = args[3];
            publicVariables.setPrefix(args[4]);
            publicVariables.setDonorSource(args[5]);
            publicVariables.setNumberOfClasses(args[6]);

            //set other public variables that depend on the input arguments
            publicVariables.setOtherVariables();

            outputToLog.WriteLine("reading the tab-separated-values metabolite database file");
            metabolites.Read_metaboliteDatabaseFromFile(publicVariables.databaseFile);

            outputToLog.WriteLine("reading the tab-separated-values metadata file for the patients");
            clinicalDataForSamples.Read_ClinicalDataForSamples();

            //read the compiled csv files
            outputToLog.WriteLine("reading the compiled csv files");
            msMetaboliteLevels.Read_inputMetaboliteLevelsFiles();

            //print the files
            outputToLog.WriteLine("printing the output files");
            msAnalysisOutput.printTheDetails();

            countTime.Stop();
            outputToLog.WriteLine(string.Format("Done in {0:00}h {1:00}min {2:00}sec {3:00}msec", countTime.Elapsed.Hours, countTime.Elapsed.Minutes, countTime.Elapsed.Seconds, countTime.Elapsed.Milliseconds / 10));

            //close open streams and engines
            publicVariables.Close();

            Console.ReadLine();
        }
    }
}
