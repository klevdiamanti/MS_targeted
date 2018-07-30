using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class msMetaboliteLevels
    {
        public static List<sampleForTissueAndCharge> List_SampleForTissueAndCharge = new List<sampleForTissueAndCharge>();
        private static List<string> listOfMetaboliteIDs = new List<string>();

        public static void Read_inputMetaboliteLevelsFiles()
        {
            foreach (string csvFile in Directory.GetFiles(@"" + publicVariables.inputMSFilesDir).Where(x => x.Split(Path.DirectorySeparatorChar).Last().StartsWith(publicVariables.prefix.ToString()) && x.EndsWith(publicVariables.suffix)))
            {
                string line;
                int tmpInt;
                List<msMetabolite> listOfMetabolitesPerTissueAndCharge = new List<msMetabolite>();
                List<string> listOfCsvLines = new List<string>();
                using (TextReader input = new StreamReader(@"" + csvFile))
                {
                    while ((line = input.ReadLine()) != null)
                    {
                        if (!(line.StartsWith("H") && int.TryParse(line.Substring(1, 4), out tmpInt))) //header or metabolites data
                        {
                            listOfCsvLines.Add(line);
                        }
                        else //metabolites measurements
                        {
                            if (listOfMetabolitesPerTissueAndCharge.Count == 0)
                            {
                                listOfMetabolitesPerTissueAndCharge = Metabolites_FromInputMetaboliteLevelsFile(listOfCsvLines);
                            }

                            List_SampleForTissueAndCharge.Add(new sampleForTissueAndCharge(line.Split(publicVariables.breakCharInFile).ToList(), listOfMetabolitesPerTissueAndCharge));
                        }
                    }
                }
            }

            outputToLog.WriteLine("calculating metabolite statistics");
            imputeMissingMetaboliteValues();
            if (publicVariables.printRatiosOfMetabolites)
            {
                calculateRatiosOfMetabolitesForEachSample();
            }
            calculatePvaluesFromResamplingStatistics();
        }

        private static List<msMetabolite> Metabolites_FromInputMetaboliteLevelsFile(List<string> listOfCsvLines)
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

        private static void imputeMissingMetaboliteValues()
        {
            List<string> phenotypes = List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().OrderBy(x => x).ToList();
            foreach (string tissue in List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                foreach (string charge in List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                {
                    foreach (string pheno in phenotypes)
                    {
                        foreach (sampleForTissueAndCharge.parentMetabolite dm in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno).SelectMany(x => x.ListOfMetabolites))
                        {
                            if (dm.mtbltVals.Imputed == -1)
                            {
                                if (List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)
                                    .SelectMany(x => x.ListOfMetabolites).Count(x => x.mtbltDetails.In_customId == dm.mtbltDetails.In_customId) == 0 ||
                                    List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)
                                    .SelectMany(x => x.ListOfMetabolites).Where(x => x.mtbltDetails.In_customId == dm.mtbltDetails.In_customId).Select(x => x.mtbltVals.Non_imputed).Count(x => x != -1) == 0)
                                {
                                    dm.mtbltVals.Imputed = 0;
                                }
                                else
                                {
                                    dm.mtbltVals.Imputed = List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)
                                    .SelectMany(x => x.ListOfMetabolites).Where(x => x.mtbltDetails.In_customId == dm.mtbltDetails.In_customId).Select(x => x.mtbltVals.Non_imputed).Where(x => x != -1).Min();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void calculateRatiosOfMetabolitesForEachSample()
        {
            List<string> phenotypes = List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().Where(x => !publicVariables.excludedPhenotypes.Contains(x)).OrderBy(x => x).ToList();
            List<tmpPerSampleRatios> tmpListOfPerSampleRatios; //ID nominator, charge nominator, ID denominator, charge denominator, phenotype, ratio
            foreach (string tissue in List_SampleForTissueAndCharge.Select(x => x.Tissue).OrderBy(x => x).Distinct())
            {
                //calculate per sample ratios and store them in a list of tuples that contains
                //custom ID nominator, charge nominator, custom ID denominator, charge denominator, phenotype, ratio
                //charges for nominator and denominator are used for cases such as LCMS where we need to check the ratio for metabolites that come
                //from different charges
                tmpListOfPerSampleRatios = new List<tmpPerSampleRatios>();
                foreach (string sampleID in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Id).Distinct())
                {
                    foreach (string charge_nom in List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                    {
                        foreach (string cid_nom in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_nom && x.Id == sampleID)
                                                                                    .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                        {
                            foreach (string charge_denom in List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                            {
                                foreach (string cid_denom in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_denom && x.Id == sampleID)
                                                                                        .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                                {
                                    tmpListOfPerSampleRatios.Add(new tmpPerSampleRatios
                                    {
                                        sampleID = sampleID,
                                        cid_nom = cid_nom,
                                        charge_nom = charge_nom,
                                        cid_denom = cid_denom,
                                        charge_denom = charge_denom,
                                        phenotype = List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge_nom && x.Id == sampleID).Phenotype,
                                        ratio = List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge_nom && x.Id == sampleID).ListOfMetabolites
                                                .First(x => x.mtbltDetails.In_customId == cid_nom).mtbltVals.Imputed /
                                        List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge_denom && x.Id == sampleID).ListOfMetabolites
                                                .First(x => x.mtbltDetails.In_customId == cid_denom).mtbltVals.Imputed
                                    });
                                }
                            }
                        }
                    }
                }

                //loop through each pair of phenotypes to perform the statistical test for the ratios
                for (int i = 0; i < phenotypes.Count; i++)
                {
                    for (int j = i + 1; j < phenotypes.Count; j++)
                    {
                        foreach (string charge_nom in List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                        {
                            foreach (string cid_nom in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_nom && x.Phenotype == phenotypes.ElementAt(i))
                                                                                .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                            {
                                foreach (string charge_denom in List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                                {
                                    foreach (string cid_denom in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_denom && x.Phenotype == phenotypes.ElementAt(j))
                                                                                            .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                                    {
                                        permutationTest.returnIEnurable(new List<string>() { phenotypes.ElementAt(i), phenotypes.ElementAt(j) },
                                            new List<double[]>()
                                            {
                                                tmpListOfPerSampleRatios.Where(x => x.cid_nom == cid_nom && x.charge_nom == charge_nom && x.cid_denom == cid_denom && x.charge_denom == charge_denom && x.phenotype ==  phenotypes.ElementAt(i))
                                                    .Select(x => x.ratio).ToArray(),
                                                tmpListOfPerSampleRatios.Where(x => x.cid_nom == cid_nom && x.charge_nom == charge_nom && x.cid_denom == cid_denom && x.charge_denom == charge_denom && x.phenotype ==  phenotypes.ElementAt(j))
                                                    .Select(x => x.ratio).ToArray(),
                                            });
                                        interMetaboliteConnections.ListOfInterMetaboliteConnections.Add(new interMetaboliteConnection()
                                        {
                                            Tissue = tissue,
                                            CustomId_nominator = cid_nom,
                                            Name_nominator = List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Phenotype == phenotypes.ElementAt(i))
                                                                                          .SelectMany(x => x.ListOfMetabolites)
                                                                                          .First(x => x.mtbltDetails.In_customId == cid_nom)
                                                                                          .mtbltDetails.In_Name,
                                            Charge_nominator = charge_nom,
                                            CustomId_denominator = cid_denom,
                                            Name_denominator = List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Phenotype == phenotypes.ElementAt(j))
                                                                                            .SelectMany(x => x.ListOfMetabolites)
                                                                                            .First(x => x.mtbltDetails.In_customId == cid_denom)
                                                                                            .mtbltDetails.In_Name,
                                            Charge_denominator = charge_denom,
                                            Group1 = phenotypes.ElementAt(i),
                                            Group2 = phenotypes.ElementAt(j),
                                            PValue = permutationTest.wilcoxonMannWhitneyPermutationTest(new string[] { "Phenotype", "Ratio" })
                                        });
                                        interMetaboliteConnections.ListOfInterMetaboliteConnections.Last().fillInListOfPerSampleRatios(tmpListOfPerSampleRatios.Where(x => x.cid_nom == cid_nom && x.charge_nom == charge_nom && x.cid_denom == cid_denom && x.charge_denom == charge_denom && (x.phenotype == phenotypes.ElementAt(i) || x.phenotype == phenotypes.ElementAt(j)))
                                                    .Select(x => x.sampleID).ToList(), tmpListOfPerSampleRatios.Where(x => x.cid_nom == cid_nom && x.charge_nom == charge_nom && x.cid_denom == cid_denom && x.charge_denom == charge_denom && (x.phenotype == phenotypes.ElementAt(i) || x.phenotype == phenotypes.ElementAt(j)))
                                                    .Select(x => x.ratio).ToList());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void calculatePvaluesFromResamplingStatistics()
        {
            //select all phenotypes and add them in a list
            //we do it in order to avoid querying the list listOfPatients every time
            //we want to increase efficiency
            List<string> phenotypes = List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().Where(x => !publicVariables.excludedPhenotypes.Contains(x)).OrderBy(x => x).ToList();

            //create two list of arrays for imputed and non-imputed values
            //we need these lists to compute p-values
            //ANOVA and t-test methods require arrays
            List<double[]> metabolite_values;

            //create list of arrays for imputed and non-imputed values
            //we need these lists to compute p-values
            //ANOVA and t-test methods require arrays
            List<normalizationVals> listOfNormValues;

            //create two list of tuples for imputed and non-imputed values
            //we need these lists to compute ratios between average values of pairs of phenotypes
            //in each tuple we store the two phenotypes between which the ratio was computed and the ratio value
            List<msMetabolite.stats.pairwiseRatioValues> ratio;

            //create two list of tuples for imputed and non-imputed values
            //we need these lists to store the ANOVA or t-test p-values for all phenotypes or for pairs of phenotypes
            //in each tuple we store the two phenotypes between which the p-value was computed and the p-value itself
            List<msMetabolite.stats.pairwiseTestValues> statTestPvalue;

            //keep the anova p-values in these variable to avoid recalculating them for each patient
            double multiGroupTestPvalue = -1;
            List<msMetabolite.stats.regressValues> regressionVals;

            //headers for the permutationTest dataframe
            string[] phenotypeColumnNames;

            //Keep correlations for metabolites to covariates
            List<msMetabolite.stats.corrVars> correlationToCovariatesVals;

            //Keep correlations for metabolites to metabolites
            Dictionary<string, List<msMetabolite.stats.corrMetabs>> dictOfCorrelationToMetabsVals;

            //clinical data names
            List<string> clinicalDataNames = List_SampleForTissueAndCharge.First().ListOfNumClinicalData.Select(x => x.name).ToList();

            //loop through each tissue
            foreach (string tissue in List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                dictOfCorrelationToMetabsVals = correlations.correlateMetabsToMetabs(tissue);
                //loop through each charge
                foreach (string charge in List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                {
                    //loop through each metabolite by selecting unique custom IDs
                    foreach (string custid in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge).SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                    {
                        //initialize the lists and arrays
                        metabolite_values = new List<double[]>();
                        listOfNormValues = new List<normalizationVals>();
                        ratio = new List<msMetabolite.stats.pairwiseRatioValues>();
                        statTestPvalue = new List<msMetabolite.stats.pairwiseTestValues>();
                        phenotypeColumnNames = new string[] { "Phenotype", custid };
                        correlationToCovariatesVals = new List<msMetabolite.stats.corrVars>();
                        regressionVals = new List<msMetabolite.stats.regressValues>();

                        //loop over the phenotypes to keep info for numerical covariates for which we need to compute regressions
                        //each of the arrays contains as many elements as there are for each phenotype
                        foreach (string pheno in phenotypes)
                        {
                            double[] tmpArrayMetabVal = new double[List_SampleForTissueAndCharge.Count(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)];
                            int i = 0;
                            foreach (string sampleId in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno).Select(x => x.Id))
                            {
                                tmpArrayMetabVal[i] = List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno && x.Id == sampleId)
                                    .ListOfMetabolites.First(x => x.mtbltDetails.In_customId == custid).mtbltVals.Imputed;
                                foreach (string ncv in publicVariables.normCovars)
                                {
                                    if (listOfNormValues.Any(x => x.covarName == ncv))
                                    {
                                        if (listOfNormValues.First(x => x.covarName == ncv).listOfVals.ContainsKey(pheno))
                                        {
                                            listOfNormValues.First(x => x.covarName == ncv).listOfVals[pheno]
                                                .Add(List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno && x.Id == sampleId).ListOfNumClinicalData.First(x => x.name == ncv).value);
                                        }
                                        else
                                        {
                                            listOfNormValues.First(x => x.covarName == ncv).listOfVals.Add(pheno,
                                                    new List<double>() { List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno && x.Id == sampleId).ListOfNumClinicalData.First(x => x.name == ncv).value });
                                        }
                                    }
                                    else
                                    {
                                        listOfNormValues.Add(new normalizationVals()
                                        {
                                            covarName = ncv,
                                            listOfVals = new Dictionary<string, List<double>>()
                                            {
                                                {
                                                    pheno,
                                                    new List<double>(){ List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno && x.Id == sampleId).ListOfNumClinicalData.First(x => x.name == ncv).value }
                                                }
                                            }
                                        });
                                    }
                                }
                                i++;
                            }
                            metabolite_values.Add(tmpArrayMetabVal);
                        }

                        //loop over the phenotypes in order to get each pair of phenotype
                        //calculate the ratio for imputed and non-imputed values for pair-wise phenotypes, and store them in the corresponding lists of tuples
                        //calculate the p-value for imputed and non-imputed values for pair-wise phenotypes, and store them in the corresponding lists of tuples
                        //it does not matter that we use an ANOVA instead of a t-test for the permutation statistic here since they should coincide because an ANOVA for two classes is equal to a t-test
                        for (int i = 0; i < phenotypes.Count; i++)
                        {
                            for (int j = i + 1; j < phenotypes.Count; j++)
                            {
                                returnFCandCI rfcaci = foldChangeCI.calculateFoldChangeAndCI(metabolite_values.ElementAt(j), metabolite_values.ElementAt(i));
                                ratio.Add(new msMetabolite.stats.pairwiseRatioValues()
                                {
                                    group1 = phenotypes.ElementAt(i),
                                    group2 = phenotypes.ElementAt(j),
                                    fold_change = rfcaci.fc,
                                    ci_lower = rfcaci.lower,
                                    ci_upper = rfcaci.upper
                                });

                                permutationTest.returnIEnurable(new List<string>() { phenotypes.ElementAt(i), phenotypes.ElementAt(j) }, new List<double[]>() { metabolite_values.ElementAt(i), metabolite_values.ElementAt(j) });
                                statTestPvalue.Add(new msMetabolite.stats.pairwiseTestValues()
                                {
                                    group1 = phenotypes.ElementAt(i),
                                    group2 = phenotypes.ElementAt(j),
                                    pairValue = permutationTest.wilcoxonMannWhitneyPermutationTest(phenotypeColumnNames)
                                });
                            }
                        }

                        //calculate the two-group or multi-group p-values
                        permutationTest.returnIEnurable(phenotypes, metabolite_values);
                        if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two)
                        {
                            multiGroupTestPvalue = permutationTest.wilcoxonMannWhitneyPermutationTest(phenotypeColumnNames);
                        }
                        else
                        {
                            multiGroupTestPvalue = permutationTest.kruskalWallisPermutationTest(phenotypeColumnNames);
                        }

                        //linear regression model between metabolite levels and HbA1c
                        foreach (normalizationVals nv in listOfNormValues)
                        {
                            permutationTest.returnIEnurableNumeric(nv.listOfVals.Select(x => x.Value.ToArray()).ToList(), metabolite_values);
                            regressionVals.Add(permutationTest.linearRegressionTest(new string[] { nv.covarName, custid }));
                        }

                        //calculate spearman correlations
                        correlationToCovariatesVals = correlations.correlateMetabsToCovariates(tissue, charge, custid, clinicalDataNames);

                        foreach (sampleForTissueAndCharge sftac in List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge))
                        {
                            sftac.ListOfMetabolites.First(x => x.mtbltDetails.In_customId == custid).mtbltDetails.ListOfStats = new msMetabolite.stats()
                            {
                                Tissue = sftac.Tissue,
                                Charge = sftac.Charge,
                                MultiGroupPvalue = multiGroupTestPvalue,
                                RegressionValues = regressionVals,
                                Ratio = ratio,
                                PairwiseTestPvalue = statTestPvalue,
                                CorrelationValues = correlationToCovariatesVals,
                                CorrelationMetabolites = dictOfCorrelationToMetabsVals
                                    .First(x => string.Join("_", x.Key.Split('_').ToList().GetRange(0, x.Key.Split('_').Length - 1)) == custid && x.Key.Split('_').Last() == charge).Value
                            };
                        }
                    }
                }
            }
        }

        private class normalizationVals
        {
            public Dictionary<string, List<double>> listOfVals;
            public string covarName;
        }
    }

    public class tmpPerSampleRatios
    {
        public string sampleID { get; set; }
        public string cid_nom { get; set; }
        public string charge_nom { get; set; }
        public string cid_denom { get; set; }
        public string charge_denom { get; set; }
        public string phenotype { get; set; }
        public double ratio { get; set; }
    }
}
