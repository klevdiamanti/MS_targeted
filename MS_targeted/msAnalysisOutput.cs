﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class msAnalysisOutput
    {
        public static void printTheDetails()
        {
            if (publicVariables.printTheMetaboliteDetails)
            {
                //a concentrated list of all the metabolites identified in all the tissues
                //their database IDs, genes associated to and pathways they belong to
                string metabolitesOutputFile = Path.Combine(publicVariables.outputDir, "metabolites" + publicVariables.suffix);
                printTheMetabolitesDetails(metabolitesOutputFile);
            }

            if (publicVariables.printBoxplots)
            {
                //boxplots for the metabolite levels and their significance
                string boxplotDirectory = Path.Combine(publicVariables.outputDir, "boxplots");
                if (Directory.Exists(@"" + boxplotDirectory))
                {
                    Directory.Delete(@"" + boxplotDirectory, true);
                }
                Directory.CreateDirectory(@"" + boxplotDirectory);
                string boxplotImputeFile = Path.Combine(boxplotDirectory, "boxplots.pdf");
                printBoxplots.printMyBoxplot(boxplotImputeFile, msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().Where(x => !publicVariables.excludedPhenotypes.Contains(x)).OrderBy(x => x).ToList());
            }

            if (publicVariables.printScatterplots)
            {
                //scatterplots for the metabolite levels and their correlations to clinical data
                string scatterplotDirectory = Path.Combine(publicVariables.outputDir, "scatterplots");
                if (Directory.Exists(@"" + scatterplotDirectory))
                {
                    Directory.Delete(@"" + scatterplotDirectory, true);
                }
                Directory.CreateDirectory(@"" + scatterplotDirectory);
                string scatterplotImputeFile = Path.Combine(scatterplotDirectory, "scatterplots_");
                printScatterplots.printMyScatterplot(scatterplotImputeFile);
            }

            if (publicVariables.printPathwaysForMetabolites)
            {
                //print enriched pathways
                string pathwaySignificanceDirectory = Path.Combine(publicVariables.outputDir, "pathway_significance");
                if (Directory.Exists(@"" + pathwaySignificanceDirectory))
                {
                    Directory.Delete(@"" + pathwaySignificanceDirectory, true);
                }
                Directory.CreateDirectory(@"" + pathwaySignificanceDirectory);
                string pathwaySignificanceImputeOutputFile = Path.Combine(pathwaySignificanceDirectory, "pathway_significance");
                printPathwaysForMetabolites(pathwaySignificanceImputeOutputFile);
            }

            if (publicVariables.printMetaboliteStatistics)
            {
                //print significant metabolites
                string metaboliteSignificanceDirectory = Path.Combine(publicVariables.outputDir, "metabolite_significance");
                if (Directory.Exists(@"" + metaboliteSignificanceDirectory))
                {
                    Directory.Delete(@"" + metaboliteSignificanceDirectory, true);
                }
                Directory.CreateDirectory(@"" + metaboliteSignificanceDirectory);
                string metabolitesSignificanceImputeFile = Path.Combine(metaboliteSignificanceDirectory, "metabolite_significance");
                printMetaboliteStatistics(metabolitesSignificanceImputeFile);
            }

            if (publicVariables.printCorrelationsMetabolitesToCovariates)
            {
                //print metabolite correlations
                string metaboliteToCovariateCorrelationsDirectory = Path.Combine(publicVariables.outputDir, "metab_to_covar_correlations");
                if (Directory.Exists(@"" + metaboliteToCovariateCorrelationsDirectory))
                {
                    Directory.Delete(@"" + metaboliteToCovariateCorrelationsDirectory, true);
                }
                Directory.CreateDirectory(@"" + metaboliteToCovariateCorrelationsDirectory);
                string metaboliteToCovariateCorrelationsFile = Path.Combine(metaboliteToCovariateCorrelationsDirectory, "metab_to_covar_correlations");
                printCorrelationsMetabolitesToCovariates(metaboliteToCovariateCorrelationsFile);
            }

            if (publicVariables.printMetabolitesForDatabase)
            {
                //print database output
                string databaseDirectory = Path.Combine(publicVariables.outputDir, "database");
                if (Directory.Exists(@"" + databaseDirectory))
                {
                    Directory.Delete(@"" + databaseDirectory, true);
                }
                Directory.CreateDirectory(@"" + databaseDirectory);
                string databaseMetaboliteDetailsFile = Path.Combine(databaseDirectory, publicVariables.prefix.ToString() + "_targeted_metabolite_id" + publicVariables.suffix);
                printMetabolitesForDatabase(databaseMetaboliteDetailsFile);
                string databaseMetaboliteLevelsFile = Path.Combine(databaseDirectory, publicVariables.prefix.ToString() + "_targeted");
                printMetaboliteLevelsForDatabase(databaseMetaboliteLevelsFile);
            }

            if (publicVariables.printRosettaDatasets)
            {
                //print rosetta datasets
                string rosettaDatasetsDirectory = Path.Combine(publicVariables.outputDir, "rosetta_datasets");
                if (Directory.Exists(@"" + rosettaDatasetsDirectory))
                {
                    Directory.Delete(@"" + rosettaDatasetsDirectory, true);
                }
                Directory.CreateDirectory(@"" + rosettaDatasetsDirectory);
                string rosettaDatasetImputeFile = Path.Combine(rosettaDatasetsDirectory, "rosetta_dataset");
                printRosettaDatasets(rosettaDatasetImputeFile);
            }

            if (publicVariables.printCorrelationsMetabolitesToMetabolites)
            {
                //print correlation between metabolites
                //print metabolite correlations
                string metaboliteToMetaboliteCorrelationsDirectory = Path.Combine(publicVariables.outputDir, "metab_to_metab_correlations");
                if (Directory.Exists(@"" + metaboliteToMetaboliteCorrelationsDirectory))
                {
                    Directory.Delete(@"" + metaboliteToMetaboliteCorrelationsDirectory, true);
                }
                Directory.CreateDirectory(@"" + metaboliteToMetaboliteCorrelationsDirectory);
                string metaboliteToMetaboliteCorrelationsFile = Path.Combine(metaboliteToMetaboliteCorrelationsDirectory, "metab_to_metab_correlations");
                printCorrelationsMetabolitesToMetabolites(metaboliteToMetaboliteCorrelationsFile);
            }

            if (publicVariables.printOutputForMoDentify)
            {
                //print data for MoDentify
                string moDentifyFilesDirectory = Path.Combine(publicVariables.outputDir, "MoDentify");
                if (Directory.Exists(@"" + moDentifyFilesDirectory))
                {
                    Directory.Delete(@"" + moDentifyFilesDirectory, true);
                }
                Directory.CreateDirectory(@"" + moDentifyFilesDirectory);
                string moDentifyFilesPrefix = Path.Combine(moDentifyFilesDirectory, "MoDentify");
                printOutputForMoDentify(moDentifyFilesPrefix);
            }

            if (publicVariables.printRatiosOfMetabolites)
            {
                //print data for MoDentify
                string metaboliteRatioDirectory = Path.Combine(publicVariables.outputDir, "metabolite_ratios");
                if (Directory.Exists(@"" + metaboliteRatioDirectory))
                {
                    Directory.Delete(@"" + metaboliteRatioDirectory, true);
                }
                Directory.CreateDirectory(@"" + metaboliteRatioDirectory);
                string metaboliteRatioFilesPrefix = Path.Combine(metaboliteRatioDirectory, "metabolite_ratios");
                printRatiosOfMetabolites(metaboliteRatioFilesPrefix);
            }
        }

        private static void printTheMetabolitesDetails(string outputFile)
        {
            //HEADER
            using (TextWriter output = new StreamWriter(@"" + outputFile))
            {
                output.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}",
                    publicVariables.breakCharInFile,
                    "name",
                    "formula",
                    "custom_ID",
                    "CAS_ID",
                    "KEGG_ID",
                    "HMDB_ID",
                    "PubChem_ID",
                    "ChEBI_ID",
                    "Pathway",
                    "SuperPathway",
                    "SubPathwayID",
                    "SubPathway"
                    ));

                //BODY
                foreach (string mtblid in msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct().OrderBy(x => x))
                {
                    msMetabolite mtbl = msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).First(x => x.mtbltDetails.In_customId == mtblid).mtbltDetails;
                    output.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}",
                        publicVariables.breakCharInFile,
                        mtbl.In_Name,
                        mtbl.In_Formula,
                        mtbl.In_customId,
                        mtbl.Cas_registry_number,
                        mtbl.Kegg_id,
                        mtbl.Hmdb_accession,
                        mtbl.Pubchem_compound_id,
                        mtbl.Chebi_id,
                        (mtbl.List_of_pathways == null) ? "" : string.Join("|", mtbl.List_of_pathways.Select(x => x.pathwayName())),
                        mtbl.My_taxonomy.Kingdom,
                        mtbl.My_taxonomy.Super_class,
                        mtbl.My_taxonomy.Tclass));
                }
            }
        }

        private static void printPathwaysForMetabolites(string outputFilePrefix)
        {
            string line_to_print, pairwiseTest;
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                using (TextWriter output_significant = new StreamWriter(@"" + outputFilePrefix + "_significant_" + tissue + publicVariables.suffix))
                {
                    using (TextWriter output_all = new StreamWriter(@"" + outputFilePrefix + "_" + tissue + publicVariables.suffix))
                    {
                        pairwiseTest = (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) ? "" : "pVal" + publicVariables.breakCharInFile;
                        for (int i = 0; i < msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.Count; i++)
                        {
                            pairwiseTest += msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group2.First() + "v" +
                                msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group1.First() + "r" + publicVariables.breakCharInFile +
                                msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group2.First() + "v" +
                                msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group1.First() + "p" + publicVariables.breakCharInFile;
                        }
                        pairwiseTest = pairwiseTest.Substring(0, pairwiseTest.Length - 1);

                        line_to_print = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                                                      publicVariables.breakCharInFile,
                                                      "SuperPathway",
                                                      "SubPathwayID",
                                                      "SubPathway",
                                                      "PathwayCount",
                                                      "BiochemicalName",
                                                      "Formula",
                                                      "Platform",
                                                      "Charge",
                                                      pairwiseTest,
                                                      "CAS",
                                                      "HMDB",
                                                      "KEGG",
                                                      "ChEBI",
                                                      "PubChem",
                                                      "CustID",
                                                      "Genes"
                                                      );
                        output_significant.WriteLine(line_to_print);
                        output_all.WriteLine(line_to_print);

                        foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                        {
                            foreach (string currPathwayID in msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails)
                                .Where(x => x.List_of_pathways != null && x.List_of_pathways.Count > 0).SelectMany(x => x.List_of_pathways).SelectMany(x => x.pathwayID()).GroupBy(x => x)
                                .OrderByDescending(x => x.Count()).ThenBy(x => x.Key).Select(x => x.Key))
                            {
                                foreach (msMetabolite metabolitePerPathway in msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfMetabolites.Select(x => x.mtbltDetails))
                                {
                                    if (metabolitePerPathway.List_of_pathways != null && metabolitePerPathway.List_of_pathways.Count > 0 && metabolitePerPathway.List_of_pathways.Any(x => x.pathwayID().Any(y => y == currPathwayID)))
                                    {
                                        pairwiseTest = (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) ? "" : metabolitePerPathway.ListOfStats.MultiGroupPvalue.ToString() + publicVariables.breakCharInFile;
                                        for (int i = 0; i < metabolitePerPathway.ListOfStats.PairwiseTestPvalue.Count; i++)
                                        {
                                            pairwiseTest += string.Format("{0}{1}{2}{1}",
                                                metabolitePerPathway.ListOfStats.Ratio.ElementAt(i).pairValue,
                                                publicVariables.breakCharInFile,
                                                metabolitePerPathway.ListOfStats.PairwiseTestPvalue.ElementAt(i).pairValue);
                                        }
                                        pairwiseTest = pairwiseTest.Substring(0, pairwiseTest.Length - 1);

                                        line_to_print = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                                                        publicVariables.breakCharInFile,
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails)
                                                            .Where(x => x.List_of_pathways != null && x.List_of_pathways.Count > 0).SelectMany(x => x.List_of_pathways)
                                                            .First(x => x.pathwayID().Any(y => y == currPathwayID)).Super_class,
                                                        currPathwayID,
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails)
                                                            .Where(x => x.List_of_pathways != null && x.List_of_pathways.Count > 0).SelectMany(x => x.List_of_pathways)
                                                            .First(x => x.pathwayID().Any(y => y == currPathwayID)).pathwayDetails(),
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfMetabolites.Select(x => x.mtbltDetails)
                                                                    .Count(x => x.List_of_pathways.Any(y => y.pathwayID().Any(z => z == currPathwayID))),
                                                        metabolitePerPathway.In_Name,
                                                        metabolitePerPathway.In_Formula,
                                                        publicVariables.prefix.ToString().ToUpper(),
                                                        charge,
                                                        pairwiseTest,
                                                        metabolitePerPathway.In_Cas_id,
                                                        metabolitePerPathway.In_Hmdb_id,
                                                        metabolitePerPathway.In_Kegg_id,
                                                        metabolitePerPathway.In_Chebi_id,
                                                        metabolitePerPathway.In_Pubchem_id,
                                                        metabolitePerPathway.In_customId,
                                                        string.Join("|", metabolitePerPathway.List_of_proteins.Select(x => x.Gene_name))
                                                        );
                                        output_all.WriteLine(line_to_print);

                                        //print significant only
                                        //significant is defined as:
                                        //Anova pvalue corrected OR not <= 0.1
                                        //Individual t test pvalue corrected or not <= 0.1
                                        //Ratio <= 0.5 or >=1.5
                                        //Absolute of Pearson correlation >= 0.5
                                        if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) //if there are only two phenotypic classes
                                        {
                                            if (metabolitePerPathway.ListOfStats.PairwiseTestPvalue.Any(x => x.pairValue <= publicVariables.significanceThreshold) ||
                                                metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue <= publicVariables.ratioLowerThreshold) ||
                                                metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue >= publicVariables.ratioUpperThreshold))
                                            {
                                                line_to_print = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                                                        publicVariables.breakCharInFile,
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails)
                                                            .Where(x => x.List_of_pathways != null && x.List_of_pathways.Count > 0).SelectMany(x => x.List_of_pathways)
                                                            .First(x => x.pathwayID().Any(y => y == currPathwayID)).Super_class,
                                                        currPathwayID,
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails)
                                                            .Where(x => x.List_of_pathways != null && x.List_of_pathways.Count > 0)
                                                            .SelectMany(x => x.List_of_pathways).First(x => x.pathwayID().Any(y => y == currPathwayID)).pathwayDetails(),
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfMetabolites.Select(x => x.mtbltDetails)
                                                                        .Where(x => x.List_of_pathways.Any(y => y.pathwayID().Any(z => z == currPathwayID)))
                                                                        .Count(x => x.ListOfStats.Ratio.Any(y => y.pairValue >= publicVariables.ratioUpperThreshold) || x.ListOfStats.Ratio.Any(y => y.pairValue <= publicVariables.ratioLowerThreshold) ||
                                                                        x.ListOfStats.MultiGroupPvalue <= publicVariables.significanceThreshold || x.ListOfStats.PairwiseTestPvalue.Any(y => y.pairValue <= publicVariables.significanceThreshold)),
                                                        metabolitePerPathway.In_Name,
                                                        metabolitePerPathway.In_Formula,
                                                        publicVariables.prefix.ToString().ToUpper(),
                                                        charge,
                                                        pairwiseTest,
                                                        metabolitePerPathway.In_Cas_id,
                                                        metabolitePerPathway.In_Hmdb_id,
                                                        metabolitePerPathway.In_Kegg_id,
                                                        metabolitePerPathway.In_Chebi_id,
                                                        metabolitePerPathway.In_Pubchem_id,
                                                        metabolitePerPathway.In_customId,
                                                        string.Join("|", metabolitePerPathway.List_of_proteins.Select(x => x.Gene_name))
                                                        );
                                                output_significant.WriteLine(line_to_print);
                                            }
                                        }
                                        else
                                        {
                                            if (metabolitePerPathway.ListOfStats.MultiGroupPvalue <= publicVariables.significanceThreshold ||
                                                metabolitePerPathway.ListOfStats.PairwiseTestPvalue.Any(x => x.pairValue <= publicVariables.significanceThreshold) ||
                                                metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue <= publicVariables.ratioLowerThreshold) ||
                                                metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue >= publicVariables.ratioUpperThreshold))
                                            {
                                                line_to_print = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                                                        publicVariables.breakCharInFile,
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails)
                                                            .Where(x => x.List_of_pathways != null && x.List_of_pathways.Count > 0).SelectMany(x => x.List_of_pathways)
                                                            .First(x => x.pathwayID().Any(y => y == currPathwayID)).Super_class,
                                                        currPathwayID,
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails)
                                                            .Where(x => x.List_of_pathways != null && x.List_of_pathways.Count > 0)
                                                            .SelectMany(x => x.List_of_pathways).First(x => x.pathwayID().Any(y => y == currPathwayID)).pathwayDetails(),
                                                        msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfMetabolites.Select(x => x.mtbltDetails)
                                                                        .Where(x => x.List_of_pathways.Any(y => y.pathwayID().Any(z => z == currPathwayID)))
                                                                        .Count(x => x.ListOfStats.Ratio.Any(y => y.pairValue >= publicVariables.ratioUpperThreshold) || x.ListOfStats.Ratio.Any(y => y.pairValue <= publicVariables.ratioLowerThreshold) ||
                                                                        x.ListOfStats.MultiGroupPvalue <= publicVariables.significanceThreshold || x.ListOfStats.PairwiseTestPvalue.Any(y => y.pairValue <= publicVariables.significanceThreshold)),
                                                        metabolitePerPathway.In_Name,
                                                        metabolitePerPathway.In_Formula,
                                                        publicVariables.prefix.ToString().ToUpper(),
                                                        charge,
                                                        pairwiseTest,
                                                        metabolitePerPathway.In_Cas_id,
                                                        metabolitePerPathway.In_Hmdb_id,
                                                        metabolitePerPathway.In_Kegg_id,
                                                        metabolitePerPathway.In_Chebi_id,
                                                        metabolitePerPathway.In_Pubchem_id,
                                                        metabolitePerPathway.In_customId,
                                                        string.Join("|", metabolitePerPathway.List_of_proteins.Select(x => x.Gene_name))
                                                        );
                                                output_significant.WriteLine(line_to_print);
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (msMetabolite metaboliteWithNoPathway in msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge)
                                .ListOfMetabolites.Select(x => x.mtbltDetails).Where(x => x.List_of_pathways == null || x.List_of_pathways.Count == 0))
                            {
                                pairwiseTest = (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) ? "" : metaboliteWithNoPathway.ListOfStats.MultiGroupPvalue.ToString() + publicVariables.breakCharInFile;
                                for (int i = 0; i < metaboliteWithNoPathway.ListOfStats.PairwiseTestPvalue.Count; i++)
                                {
                                    pairwiseTest += string.Format("{0}{1}{2}{1}",
                                        metaboliteWithNoPathway.ListOfStats.Ratio.ElementAt(i).pairValue,
                                        publicVariables.breakCharInFile,
                                        metaboliteWithNoPathway.ListOfStats.PairwiseTestPvalue.ElementAt(i).pairValue);
                                }
                                pairwiseTest = pairwiseTest.Substring(0, pairwiseTest.Length - 1);

                                line_to_print = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                                                  publicVariables.breakCharInFile,
                                                  "Unknown",
                                                  "Unknown",
                                                  "Unknown",
                                                  "0",
                                                  metaboliteWithNoPathway.In_Name,
                                                  metaboliteWithNoPathway.In_Formula,
                                                  publicVariables.prefix.ToString().ToUpper(),
                                                  charge,
                                                  pairwiseTest,
                                                  metaboliteWithNoPathway.In_Cas_id,
                                                  metaboliteWithNoPathway.In_Hmdb_id,
                                                  metaboliteWithNoPathway.In_Kegg_id,
                                                  metaboliteWithNoPathway.In_Chebi_id,
                                                  metaboliteWithNoPathway.In_Pubchem_id,
                                                  metaboliteWithNoPathway.In_customId,
                                                  string.Join("|", metaboliteWithNoPathway.List_of_proteins.Select(x => x.Gene_name))
                                                  );
                                output_all.WriteLine(line_to_print);

                                //print significant only
                                //significant is defined as:
                                //Anova pvalue corrected OR not <= 0.1
                                //Individual t test pvalue corrected or not <= 0.1
                                //Ratio <= 0.5 or >=1.5
                                //Absolute of Pearson correlation >= 0.5
                                if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) //if there are only two phenotypic classes
                                {
                                    if (metaboliteWithNoPathway.ListOfStats.PairwiseTestPvalue.Any(x => x.pairValue <= publicVariables.significanceThreshold) ||
                                        metaboliteWithNoPathway.ListOfStats.Ratio.Any(x => x.pairValue <= publicVariables.ratioLowerThreshold) ||
                                        metaboliteWithNoPathway.ListOfStats.Ratio.Any(x => x.pairValue >= publicVariables.ratioUpperThreshold))
                                    {
                                        output_significant.WriteLine(line_to_print);
                                    }
                                }
                                else
                                {
                                    if (metaboliteWithNoPathway.ListOfStats.MultiGroupPvalue <= publicVariables.significanceThreshold || 
                                        metaboliteWithNoPathway.ListOfStats.PairwiseTestPvalue.Any(x => x.pairValue <= publicVariables.significanceThreshold) || 
                                        metaboliteWithNoPathway.ListOfStats.Ratio.Any(x => x.pairValue <= publicVariables.ratioLowerThreshold) || 
                                        metaboliteWithNoPathway.ListOfStats.Ratio.Any(x => x.pairValue >= publicVariables.ratioUpperThreshold))
                                    {
                                        output_significant.WriteLine(line_to_print);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void printMetaboliteStatistics(string detailsFilePrefix)
        {
            string line_to_print, pairwiseTest;
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                using (TextWriter output_significant = new StreamWriter(@"" + detailsFilePrefix + "_significant_" + tissue + publicVariables.suffix))
                {
                    using (TextWriter output_all = new StreamWriter(@"" + detailsFilePrefix + "_" + tissue + publicVariables.suffix))
                    {
                        pairwiseTest = (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) ? "" : "pVal" + publicVariables.breakCharInFile;
                        for (int i = 0; i < msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.Count; i++)
                        {
                            pairwiseTest += msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group2.First() + "v" +
                                msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group1.First() + "r" + publicVariables.breakCharInFile +
                                msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group2.First() + "v" +
                                msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfMetabolites.First().mtbltDetails.ListOfStats.PairwiseTestPvalue.ElementAt(i).group1.First() + "p" + publicVariables.breakCharInFile;
                        }
                        pairwiseTest = pairwiseTest.Substring(0, pairwiseTest.Length - 1);

                        line_to_print = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                                                      publicVariables.breakCharInFile,
                                                      "BiochemicalName",
                                                      "Formula",
                                                      "Kingdom",
                                                      "SuperClass",
                                                      "Class",
                                                      "Platform",
                                                      "Charge",
                                                      pairwiseTest,
                                                      "CAS",
                                                      "HMDB",
                                                      "KEGG",
                                                      "ChEBI",
                                                      "PubChem",
                                                      "CustID",
                                                      "Genes",
                                                      "PathwayIDs"
                                                      );
                        output_significant.WriteLine(line_to_print);
                        output_all.WriteLine(line_to_print);

                        foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                        {
                            foreach (msMetabolite metabolitePerPathway in msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfMetabolites.Select(x => x.mtbltDetails))
                            {
                                pairwiseTest = (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) ? "" : metabolitePerPathway.ListOfStats.MultiGroupPvalue.ToString() + publicVariables.breakCharInFile;
                                for (int i = 0; i < metabolitePerPathway.ListOfStats.PairwiseTestPvalue.Count; i++)
                                {
                                    pairwiseTest += string.Format("{1}{0}{2}{0}",
                                        publicVariables.breakCharInFile,
                                        metabolitePerPathway.ListOfStats.Ratio.ElementAt(i).pairValue,
                                        metabolitePerPathway.ListOfStats.PairwiseTestPvalue.ElementAt(i).pairValue);
                                }
                                pairwiseTest = pairwiseTest.Substring(0, pairwiseTest.Length - 1);

                                line_to_print = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                                                  publicVariables.breakCharInFile,
                                                  metabolitePerPathway.In_Name,
                                                  metabolitePerPathway.In_Formula,
                                                  metabolitePerPathway.My_taxonomy.Kingdom,
                                                  metabolitePerPathway.My_taxonomy.Super_class,
                                                  metabolitePerPathway.My_taxonomy.Tclass,
                                                  publicVariables.prefix.ToString().ToUpper(),
                                                  charge,
                                                  pairwiseTest,
                                                  metabolitePerPathway.In_Cas_id,
                                                  metabolitePerPathway.In_Hmdb_id,
                                                  metabolitePerPathway.In_Kegg_id,
                                                  metabolitePerPathway.In_Chebi_id,
                                                  metabolitePerPathway.In_Pubchem_id,
                                                  metabolitePerPathway.In_customId,
                                                  string.Join("|", metabolitePerPathway.List_of_proteins.Select(x => x.Gene_name)),
                                                  (metabolitePerPathway.List_of_pathways != null && metabolitePerPathway.List_of_pathways.Count > 0)
                                                        ? string.Join("|", metabolitePerPathway.List_of_pathways.SelectMany(x => x.pathwayID()).Distinct()) : "Unknown"
                                                  );
                                output_all.WriteLine(line_to_print);

                                //print significant only
                                //significant is defined as:
                                //Anova pvalue corrected OR not <= 0.1
                                //Individual t test pvalue corrected or not <= 0.1
                                //Ratio <= 0.5 or >=1.5
                                //Absolute of Pearson correlation >= 0.5
                                if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two) //if there are only two phenotypic classes
                                {
                                    if (metabolitePerPathway.ListOfStats.PairwiseTestPvalue.Any(x => x.pairValue <= publicVariables.significanceThreshold) || 
                                        metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue <= publicVariables.ratioLowerThreshold) || 
                                        metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue >= publicVariables.ratioUpperThreshold))
                                    {
                                        output_significant.WriteLine(line_to_print);
                                    }
                                }
                                else
                                {
                                    if (metabolitePerPathway.ListOfStats.MultiGroupPvalue <= publicVariables.significanceThreshold ||
                                        metabolitePerPathway.ListOfStats.PairwiseTestPvalue.Any(x => x.pairValue <= publicVariables.significanceThreshold) ||
                                        metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue <= publicVariables.ratioLowerThreshold) ||
                                        metabolitePerPathway.ListOfStats.Ratio.Any(x => x.pairValue >= publicVariables.ratioUpperThreshold))
                                    {
                                        output_significant.WriteLine(line_to_print);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void printCorrelationsMetabolitesToCovariates(string corrsCovsFilePrefix)
        {
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                using (TextWriter output_all = new StreamWriter(@"" + corrsCovsFilePrefix + "_" + tissue + publicVariables.suffix))
                {
                    output_all.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}",
                        publicVariables.breakCharInFile,
                        "BiochemicalName",
                        "Formula",
                        "Platform",
                        "Charge",
                        string.Join(publicVariables.breakCharInFile.ToString(),
                            msMetaboliteLevels.List_SampleForTissueAndCharge.First().ListOfNumClinicalData
                            .Select(x => x.name + "_c" + publicVariables.breakCharInFile + x.name + "_p" + publicVariables.breakCharInFile + x.name + "_pAdjust")),
                        "CAS",
                        "HMDB",
                        "KEGG",
                        "ChEBI",
                        "PubChem",
                        "CustID"
                        ));

                    foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                    {
                        foreach (msMetabolite mtbl in msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfMetabolites.Select(x => x.mtbltDetails))
                        {
                            output_all.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}",
                                publicVariables.breakCharInFile,
                                mtbl.In_Name,
                                mtbl.In_Formula,
                                publicVariables.prefix.ToString().ToUpper(),
                                charge,
                                string.Join(publicVariables.breakCharInFile.ToString(),
                                    mtbl.ListOfStats.CorrelationValues
                                    .Select(x => x.corr_value.ToString() + publicVariables.breakCharInFile + x.pValueUnadjust.ToString() + publicVariables.breakCharInFile + x.pValueAdjust.ToString())),
                                mtbl.In_Cas_id,
                                mtbl.In_Hmdb_id,
                                mtbl.In_Kegg_id,
                                mtbl.In_Chebi_id,
                                mtbl.In_Pubchem_id,
                                mtbl.In_customId
                                ));
                        }
                    }
                }
            }
        }

        private static void printMetabolitesForDatabase(string metabolitesOutputFile)
        {
            //HEADER
            using (TextWriter output = new StreamWriter(@"" + metabolitesOutputFile))
            {
                output.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}",
                    publicVariables.breakCharInFile,
                    "Name",
                    "formula",
                    "weight",
                    "AZmID",
                    "CAS_ID",
                    "found_CAS_ID",
                    "found_CAS_ID_other",
                    "KEGG_ID",
                    "found_KEGG_ID",
                    "found_KEGG_ID_other",
                    "HMDB_ID",
                    "found_HMDB_ID",
                    "found_HMDB_ID_other",
                    "ChEBI_ID",
                    "found_ChEBI_ID",
                    "found_ChEBI_ID_other",
                    "PubChem",
                    "found_PubChem",
                    "found_PubChem_other",
                    "Problematic",
                    "Comment"));

                //BODY
                foreach (string mtblid in msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct().OrderBy(x => x))
                {
                    msMetabolite mtbl = msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).First(x => x.mtbltDetails.In_customId == mtblid).mtbltDetails;
                    output.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}",
                        publicVariables.breakCharInFile,
                        mtbl.In_Name,
                        mtbl.In_Formula,
                        mtbl.In_Mass,
                        mtbl.In_customId,
                        mtbl.In_Cas_id,
                        mtbl.Cas_registry_number,
                        string.Join("|", mtbl.In_add_Cas_id),
                        mtbl.In_Kegg_id,
                        mtbl.Kegg_id,
                        string.Join("|", mtbl.In_add_Kegg_id),
                        mtbl.In_Hmdb_id,
                        mtbl.Hmdb_accession,
                        string.Join("|", mtbl.In_add_Hmdb_id),
                        mtbl.In_Chebi_id,
                        mtbl.Chebi_id,
                        string.Join("|", mtbl.In_add_Chebi_id),
                        mtbl.In_Pubchem_id,
                        mtbl.Pubchem_compound_id,
                        "",
                        mtbl.In_isProblematic,
                        ""));
                }
            }
        }

        private static void printMetaboliteLevelsForDatabase(string metabolitesLevelsOutputFile)
        {
            List<string> listOfCustId;
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Charge).Distinct())
                {
                    using (TextWriter output = new StreamWriter(@"" + metabolitesLevelsOutputFile + "_" + tissue + "_" + charge + publicVariables.suffix))
                    {
                        //HEADER
                        listOfCustId = msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge).SelectMany(x => x.ListOfMetabolites)
                            .Select(x => x.mtbltDetails).Select(x => x.In_customId).Distinct().ToList();
                        output.WriteLine("Patient_ID" + publicVariables.breakCharInFile + string.Join(publicVariables.breakCharInFile.ToString(), listOfCustId));

                        //BODY
                        foreach (sampleForTissueAndCharge smtac in msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge))
                        {
                            output.Write(smtac.Id);
                            foreach (string cid in listOfCustId)
                            {
                                if (smtac.ListOfMetabolites.First(x => x.mtbltDetails.In_customId == cid).mtbltVals.Non_imputed == -1)
                                {
                                    output.Write(publicVariables.breakCharInFile.ToString());
                                }
                                else
                                {
                                    output.Write(publicVariables.breakCharInFile.ToString() + smtac.ListOfMetabolites.First(x => x.mtbltDetails.In_customId == cid).mtbltVals.Non_imputed.ToString());
                                }
                            }
                            output.WriteLine();
                        }
                    }
                }
            }
        }

        private static void printRosettaDatasets(string rosettaDatasetImputeFile)
        {
            List<string> listOfCustId;
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Charge).Distinct())
                {
                    using (TextWriter output = new StreamWriter(@"" + rosettaDatasetImputeFile + "_" + tissue + "_" + charge + publicVariables.suffix))
                    {
                        //HEADER
                        listOfCustId = msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge).SelectMany(x => x.ListOfMetabolites)
                            .Select(x => x.mtbltDetails).Select(x => x.In_customId).Distinct().ToList();
                        output.WriteLine(string.Join(publicVariables.breakCharInFile.ToString(), listOfCustId) + publicVariables.breakCharInFile + "Phenotype");
                        //output.WriteLine(string.Join(publicVariables.breakCharInFile.ToString(), Enumerable.Repeat("float(0)", listOfCustId.Count)) + publicVariables.breakCharInFile + "string");

                        //BODY
                        foreach (sampleForTissueAndCharge sftac in msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge))
                        {
                            if (!publicVariables.excludedPhenotypes.Contains(sftac.Phenotype))
                            {
                                foreach (string cid in listOfCustId)
                                {
                                    output.Write(sftac.ListOfMetabolites.First(x => x.mtbltDetails.In_customId == cid).mtbltVals.Imputed.ToString() + publicVariables.breakCharInFile.ToString());
                                }
                                output.WriteLine(sftac.Phenotype);
                            }
                        }
                    }
                }
            }
        }

        private static void printCorrelationsMetabolitesToMetabolites(string corrsMetabsFilePrefix)
        {
            string line_to_print;
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                using (TextWriter output_pval = new StreamWriter(@"" + corrsMetabsFilePrefix + "_pValueAdjust_" + tissue + publicVariables.suffix))
                {
                    using (TextWriter output_corrVal = new StreamWriter(@"" + corrsMetabsFilePrefix + "_corrValue_" + tissue + publicVariables.suffix))
                    {
                        line_to_print = publicVariables.breakCharInFile.ToString();
                        foreach (string mid in msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue).ListOfMetabolites
                                .Select(x => x.mtbltDetails.ListOfStats.CorrelationMetabolites).First().Select(x => x.metab_id.Split('_').First()))
                        {
                            line_to_print += publicVariables.breakCharInFile.ToString() + msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue)
                                .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails).First(x => x.In_customId == mid).In_Name;
                        }
                        output_pval.WriteLine(line_to_print);
                        output_corrVal.WriteLine(line_to_print);

                        line_to_print =publicVariables.breakCharInFile.ToString() + publicVariables.breakCharInFile.ToString() +
                            string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue).ListOfMetabolites
                                .Select(x => x.mtbltDetails.ListOfStats.CorrelationMetabolites).First().Select(x => x.metab_id.Split('_').First()));
                        output_pval.WriteLine(line_to_print);
                        output_corrVal.WriteLine(line_to_print);

                        foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                        {
                            foreach (msMetabolite msm in msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge)
                                .ListOfMetabolites.Select(x => x.mtbltDetails))
                            {
                                output_pval.WriteLine(msm.In_Name + publicVariables.breakCharInFile.ToString() + msm.In_customId + publicVariables.breakCharInFile.ToString() +
                                string.Join(publicVariables.breakCharInFile.ToString(), msm.ListOfStats.CorrelationMetabolites.Select(x => x.pValueAdjust)));

                                output_corrVal.WriteLine(msm.In_Name + publicVariables.breakCharInFile.ToString() + msm.In_customId + publicVariables.breakCharInFile.ToString() +
                                string.Join(publicVariables.breakCharInFile.ToString(), msm.ListOfStats.CorrelationMetabolites.Select(x => x.corr_value)));
                            }
                        }
                    }
                }
            }
        }

        private static void printOutputForMoDentify(string modentifyFilesPrefix)
        {
            //Data
            using (TextWriter output = new StreamWriter(@"" + modentifyFilesPrefix + "_AZm_data" + publicVariables.suffix))
            {
                //NAMES
                string line_to_print = publicVariables.breakCharInFile.ToString();
                foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
                {
                    foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                    {
                        line_to_print += publicVariables.breakCharInFile.ToString() + string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge)
                            .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                            .Select(x => tissue + "::" + x.In_AZmNameFixed));
                    }
                    
                }
                output.WriteLine(line_to_print);

                //CUSTOM IDs
                line_to_print = publicVariables.breakCharInFile.ToString();
                foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
                {
                    foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                    {
                        line_to_print += publicVariables.breakCharInFile.ToString() + string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge)
                            .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                            .Select(x => tissue + "::" + x.In_customId));
                    }
                }
                output.WriteLine(line_to_print);

                //Data
                foreach (string sampleID in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Id).Distinct().OrderBy(x => x))
                {
                    line_to_print = msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Id == sampleID).Phenotype + publicVariables.breakCharInFile.ToString() + sampleID;
                    foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
                    {
                        foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                        {
                            if (msMetaboliteLevels.List_SampleForTissueAndCharge.Count(x => x.Id == sampleID && x.Tissue == tissue && x.Charge == charge) == 0)
                            {
                                line_to_print += publicVariables.breakCharInFile.ToString() + string.Join(publicVariables.breakCharInFile.ToString(), Enumerable.Repeat("", 
                                    msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge).First(x => x.ListOfMetabolites.Count != 0)
                                    .ListOfMetabolites.Count));
                            }
                            else if (msMetaboliteLevels.List_SampleForTissueAndCharge.Count(x => x.Id == sampleID && x.Tissue == tissue && x.Charge == charge) == 1)
                            {
                                line_to_print += publicVariables.breakCharInFile.ToString() +
                                    string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.
                                        First(x => x.Id == sampleID && x.Tissue == tissue && x.Charge == charge)
                                            .ListOfMetabolites.OrderBy(x => x.mtbltDetails.In_customId).Select(x => x.mtbltVals.Imputed));
                            }
                            else
                            {
                                outputToLog.WriteLine("ERROR! There are more than one samples with the same ID!");
                                Environment.Exit(0);
                            }
                        }
                    }
                    output.WriteLine(line_to_print);
                }
            }

            //Annotations
            using (TextWriter output = new StreamWriter(@"" + modentifyFilesPrefix + "_AZm_annotations" + publicVariables.suffix))
            {
                //Header
                output.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}",
                        publicVariables.breakCharInFile.ToString(), "AZmID", "tAZmID", "HmdbID", "HmdbSecondaryID", "AZmName", "HmdbName", "AZmHmdbName", "tAZmHmdbName",
                        "AZmSuperClass", "tAZmSuperClass", "AZmClass", "tAZmClass", "HmdbDirectParent", "tHmdbDirectParent", "HmdbKingdom", "tHmdbKingdom", "HmdbSuperClass",
                        "tHmdbSuperClass", "AZmHmdbSuperClass", "tAZmHmdbSuperClass", "HmdbClass", "tHmdbClass", "AZmHmdbClass", "tAZmHmdbClass", "AZmPlatform", "AZmCharge"));

                //Body
                string cSuperClass = "", cTClass = "";
                foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
                {
                    foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                    {
                        foreach (msMetabolite msm in msMetaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfMetabolites
                            .Select(x => x.mtbltDetails).OrderBy(x => x.In_customId))
                        {
                            cSuperClass = (msm.My_taxonomy.Super_class == "Organic compounds") ? msm.My_taxonomy.Tclass : msm.My_taxonomy.Super_class;
                            cTClass = (msm.In_AZmSuperClass == "Other") ? "Unknown" : msm.My_taxonomy.Tclass;
                            output.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}",
                                publicVariables.breakCharInFile.ToString(),
                                msm.In_customId,
                                tissue + "::" + msm.In_customId,
                                msm.In_Hmdb_id,
                                string.Join("|", msm.In_add_Hmdb_id),
                                msm.In_Name,
                                msm.Name,
                                msm.In_AZmNameFixed,
                                tissue + "::" + msm.In_AZmNameFixed,
                                msm.In_AZmSuperClass,
                                tissue + "::" + msm.In_AZmSuperClass,
                                msm.In_AZmClass,
                                tissue + "::" + msm.In_AZmClass,
                                msm.My_taxonomy.Direct_parent,
                                tissue + "::" + msm.My_taxonomy.Direct_parent,
                                msm.My_taxonomy.Kingdom,
                                tissue + "::" + msm.My_taxonomy.Kingdom,
                                msm.My_taxonomy.Super_class,
                                tissue + "::" + msm.My_taxonomy.Super_class,
                                cSuperClass,
                                tissue + "::" + cSuperClass,
                                msm.My_taxonomy.Tclass,
                                tissue + "::" + msm.My_taxonomy.Tclass,
                                cTClass,
                                tissue + "::" + cTClass,
                                publicVariables.prefix.ToString().ToUpper(),
                                charge));
                        }
                    }
                }
            }

            //Phenotypes
            using (TextWriter output = new StreamWriter(@"" + modentifyFilesPrefix + "_AZm_phenotypes" + publicVariables.suffix))
            {
                Dictionary<string, Dictionary<string, int>> discToNumDict = new Dictionary<string, Dictionary<string, int>>();
                discToNumDict.Add("CurrPhenotype", new Dictionary<string, int>());
                int phenoCnt = 0;
                foreach (string cp in clinicalDataForSamples.List_clinicalDataForSamples.Select(x => x.Phenotype).Distinct().OrderBy(x => x))
                {
                    discToNumDict["CurrPhenotype"].Add(cp, phenoCnt++);
                }
                foreach (var discToNumPenos in clinicalDataForSamples.List_clinicalDataForSamples.SelectMany(x => x.OtherPhenotypes))
                {
                    if (discToNumDict.ContainsKey(discToNumPenos.Key))
                    {
                        if (!discToNumDict[discToNumPenos.Key].ContainsKey(discToNumPenos.Value))
                        {
                            discToNumDict[discToNumPenos.Key].Add(discToNumPenos.Value, discToNumDict[discToNumPenos.Key].Last().Value + 1);
                        }
                    }
                    else
                    {
                        discToNumDict.Add(discToNumPenos.Key, new Dictionary<string, int>() { { discToNumPenos.Value, 0} });
                    }
                }

                //Header
                output.WriteLine(string.Format("{0}{1}{0}{2}{0}{3}{0}{4}{0}{5}",
                    publicVariables.breakCharInFile.ToString(),
                    string.Join(publicVariables.breakCharInFile.ToString(), clinicalDataForSamples.List_clinicalDataForSamples.First().NormalizationVars.Select(x => x.Key).OrderBy(x => x)),
                    string.Join(publicVariables.breakCharInFile.ToString(), clinicalDataForSamples.List_clinicalDataForSamples.First().OtherPhenotypes.Select(x => x.Key).OrderBy(x => x)),
                    string.Join(publicVariables.breakCharInFile.ToString(), clinicalDataForSamples.List_clinicalDataForSamples.OrderBy(x => x.TissueChargeWeightProblematic.Count).Last().TissueChargeWeightProblematic.Select(x => x.tissue + "_" + x.charge).OrderBy(x => x)),
                    "Phenotype", "Phenotype" + publicVariables.numberOfClasses));

                string line_to_print;
                foreach (clinicalDataForSample cdfs in clinicalDataForSamples.List_clinicalDataForSamples.OrderBy(x => x.Id))
                {
                    line_to_print = cdfs.Id;
                    foreach (KeyValuePair<string, imputedValues> nv in cdfs.NormalizationVars.OrderBy(x => x.Key))
                    {
                        line_to_print += publicVariables.breakCharInFile.ToString() + nv.Value.Imputed;
                    }
                    foreach (KeyValuePair<string, string> op in cdfs.OtherPhenotypes.OrderBy(x => x.Key))
                    {
                        line_to_print += publicVariables.breakCharInFile.ToString() + discToNumDict[op.Key][op.Value];
                    }
                    foreach (clinicalDataForSample.sampleWeight sw in clinicalDataForSamples.List_clinicalDataForSamples.OrderBy(x => x.TissueChargeWeightProblematic.Count).Last().TissueChargeWeightProblematic)
                    {
                        if (cdfs.TissueChargeWeightProblematic.Any(x => x.tissue == sw.tissue && x.charge == sw.charge))
                        {
                            line_to_print += publicVariables.breakCharInFile.ToString() + cdfs.TissueChargeWeightProblematic.First(x => x.tissue == sw.tissue && x.charge == sw.charge).weight;
                        }
                        else
                        {
                            line_to_print += publicVariables.breakCharInFile.ToString();
                        }
                    }
                    line_to_print += publicVariables.breakCharInFile.ToString() + discToNumDict["CurrPhenotype"][cdfs.Phenotype] + publicVariables.breakCharInFile.ToString() + cdfs.Phenotype;
                    output.WriteLine(line_to_print);
                }
            }
        }

        private static void printRatiosOfMetabolites(string metaboliteRatioFilesPrefix)
        {
            string line_to_print;
            //Ratio
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
            {
                using (TextWriter output = new StreamWriter(@"" + metaboliteRatioFilesPrefix + "_" + tissue + publicVariables.suffix))
                {
                    foreach (string phenotype in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().OrderBy(x => x))
                    {
                        if (msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().OrderBy(x => x).First() != phenotype)
                        {
                            output.WriteLine();
                        }

                        //HEADER line 1 metabolite biochemichal names
                        line_to_print = "";
                        foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                        {
                            if (msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x).First() == charge)
                            {
                                line_to_print += publicVariables.breakCharInFile.ToString() + phenotype + publicVariables.breakCharInFile.ToString() +
                                    string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge)
                                    .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                                    .Select(x => x.In_AZmNameFixed));
                            }
                            else
                            {
                                line_to_print += publicVariables.breakCharInFile.ToString() + 
                                    string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge)
                                    .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                                    .Select(x => x.In_AZmNameFixed));
                            }
                            
                        }
                        output.WriteLine(line_to_print);

                        //HEADER line 2 metabolite custom IDs
                        line_to_print = "";
                        foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                        {
                            if (msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x).First() == charge)
                            {
                                line_to_print += publicVariables.breakCharInFile.ToString() + phenotype + publicVariables.breakCharInFile.ToString() +
                                    string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge)
                                    .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                                    .Select(x => x.In_customId));
                            }
                            else
                            {
                                line_to_print += publicVariables.breakCharInFile.ToString() + 
                                    string.Join(publicVariables.breakCharInFile.ToString(), msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge)
                                    .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                                    .Select(x => x.In_customId));
                            }
                        }
                        output.WriteLine(line_to_print);

                        //RATIOS
                        foreach (string charge_nom in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                        {
                            foreach (string cid_nom in msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_nom && x.Phenotype == phenotype)
                                .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                                .Select(x => x.In_customId))
                            {
                                //name and id of the nominator
                                line_to_print = msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_nom && x.Phenotype == phenotype)
                                    .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails).First(x => x.In_customId == cid_nom).In_Name + publicVariables.breakCharInFile.ToString() + cid_nom;
                                foreach (string charge_denom in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                                {
                                    foreach (string cid_denom in msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_denom && x.Phenotype == phenotype)
                                        .Select(x => x.ListOfMetabolites).First().Select(x => x.mtbltDetails).OrderBy(x => x.In_customId)
                                        .Select(x => x.In_customId))
                                    {
                                        if (msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_denom && x.Phenotype == phenotype)
                                            .SelectMany(x => x.ListOfMetabolites).Count(x => x.mtbltDetails.In_customId == cid_denom) == 0)
                                        {
                                            continue;
                                        }

                                        line_to_print += publicVariables.breakCharInFile.ToString() + 
                                            Convert.ToString(decimal.Round(
                                            Convert.ToDecimal(msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_nom && x.Phenotype == phenotype)
                                            .SelectMany(x => x.ListOfMetabolites).Where(x => x.mtbltDetails.In_customId == cid_nom).Select(x => x.mtbltVals.Imputed).Average() /
                                            msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_denom && x.Phenotype == phenotype)
                                            .SelectMany(x => x.ListOfMetabolites).Where(x => x.mtbltDetails.In_customId == cid_denom).Select(x => x.mtbltVals.Imputed).Average()), 2, MidpointRounding.AwayFromZero));
                                    }
                                }
                                output.WriteLine(line_to_print);
                            }
                        }
                    }
                }
            }

            //Ratio statistical significance
            foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
            {
                using (TextWriter output = new StreamWriter(@"" + metaboliteRatioFilesPrefix + "_significance_" + tissue + publicVariables.suffix))
                {
                    List<string> phenotypes = interMetaboliteConnections.ListOfInterMetaboliteConnections.Select(x => x.Group1).Distinct().OrderBy(x => x).ToList();
                    phenotypes.AddRange(interMetaboliteConnections.ListOfInterMetaboliteConnections.Select(x => x.Group2).Distinct().OrderBy(x => x));
                    phenotypes = phenotypes.Distinct().ToList();

                    for (int i = 0; i < phenotypes.Count; i++)
                    {
                        for (int j = i + 1; j < phenotypes.Count; j++)
                        {
                            //HEADER
                            line_to_print = phenotypes.ElementAt(i) + publicVariables.breakCharInFile + phenotypes.ElementAt(j);
                            foreach (string charge in interMetaboliteConnections.ListOfInterMetaboliteConnections.Select(x => x.Charge_nominator).OrderBy(x => x).Distinct())
                            {
                                foreach (string cid in interMetaboliteConnections.ListOfInterMetaboliteConnections
                                            .Where(x => x.Tissue == tissue && x.Charge_nominator == charge && x.Group1 == phenotypes.ElementAt(i))
                                            .Select(x => x.CustomId_nominator).Distinct().OrderBy(x => x))
                                {
                                    line_to_print += publicVariables.breakCharInFile + interMetaboliteConnections.ListOfInterMetaboliteConnections
                                        .First(x => x.Tissue == tissue && x.Charge_nominator == charge && x.CustomId_nominator == cid).Name_nominator;
                                }
                            }
                            output.WriteLine(line_to_print);

                            line_to_print = phenotypes.ElementAt(i) + publicVariables.breakCharInFile + phenotypes.ElementAt(j);
                            foreach (string charge in interMetaboliteConnections.ListOfInterMetaboliteConnections.Select(x => x.Charge_nominator).OrderBy(x => x).Distinct())
                            {
                                line_to_print += publicVariables.breakCharInFile +
                                        string.Join(publicVariables.breakCharInFile.ToString(), interMetaboliteConnections.ListOfInterMetaboliteConnections
                                            .Where(x => x.Tissue == tissue && x.Charge_nominator == charge && x.Group1 == phenotypes.ElementAt(i))
                                            .Select(x => x.CustomId_nominator).Distinct().OrderBy(x => x));
                            }
                            output.WriteLine(line_to_print);

                            //BODY
                            foreach (string charge_nom in interMetaboliteConnections.ListOfInterMetaboliteConnections.Select(x => x.Charge_nominator).OrderBy(x => x).Distinct())
                            {
                                foreach (string cid_nom in interMetaboliteConnections.ListOfInterMetaboliteConnections
                                    .Where(x => x.Tissue == tissue && x.Charge_nominator == charge_nom && x.Group1 == phenotypes.ElementAt(i))
                                    .Select(x => x.CustomId_nominator).Distinct().OrderBy(x => x))
                                {
                                    //name and id of the nominator
                                    line_to_print = interMetaboliteConnections.ListOfInterMetaboliteConnections.First(x => x.Tissue == tissue && x.Charge_nominator == charge_nom &&
                                                        x.CustomId_nominator == cid_nom).Name_nominator + publicVariables.breakCharInFile + cid_nom;
                                    foreach (string charge_denom in interMetaboliteConnections.ListOfInterMetaboliteConnections.Select(x => x.Charge_denominator).OrderBy(x => x).Distinct())
                                    {
                                        foreach (string cid_denom in interMetaboliteConnections.ListOfInterMetaboliteConnections
                                            .Where(x => x.Tissue == tissue && x.Charge_denominator == charge_denom && x.Group2 == phenotypes.ElementAt(j))
                                            .Select(x => x.CustomId_denominator).Distinct().OrderBy(x => x))
                                        {
                                            line_to_print += publicVariables.breakCharInFile.ToString() + interMetaboliteConnections.ListOfInterMetaboliteConnections
                                                .First(x => x.Tissue == tissue && x.Group1 == phenotypes.ElementAt(i) && x.Group2 == phenotypes.ElementAt(j) &&
                                                x.CustomId_nominator == cid_nom && x.CustomId_denominator == cid_denom && x.Charge_nominator == charge_nom &&
                                                x.Charge_denominator == charge_denom).PValue;
                                        }
                                    }
                                    output.WriteLine(line_to_print);
                                }
                            }
                            output.WriteLine();
                        }
                        output.WriteLine();
                    }
                }
            }
        }
    }
}
