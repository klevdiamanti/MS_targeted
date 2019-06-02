using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MS_targeted
{
    public static class metaboliteStats
    {
        public static void StartMetaboliteStats()
        {
            
            ImputeMissingMetaboliteValues();
            if (publicVariables.printRatiosOfMetabolites)
            {
                CalculateMetaboliteRatios();
            }
            CalculatePvalues();
        }

        private static void ImputeMissingMetaboliteValues()
        {
            List<string> phenotypes = metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().OrderBy(x => x).ToList();
            foreach (string tissue in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                foreach (string charge in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                {
                    foreach (string pheno in phenotypes)
                    {
                        foreach (sampleForTissueAndCharge.parentMetabolite dm in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno).SelectMany(x => x.ListOfMetabolites))
                        {
                            if (dm.mtbltVals.Imputed == -1)
                            {
                                if (metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)
                                    .SelectMany(x => x.ListOfMetabolites).Count(x => x.mtbltDetails.In_customId == dm.mtbltDetails.In_customId) == 0 ||
                                    metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)
                                    .SelectMany(x => x.ListOfMetabolites).Where(x => x.mtbltDetails.In_customId == dm.mtbltDetails.In_customId).Select(x => x.mtbltVals.Non_imputed).Count(x => x != -1) == 0)
                                {
                                    dm.mtbltVals.Imputed = 0;
                                }
                                else
                                {
                                    dm.mtbltVals.Imputed = metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)
                                    .SelectMany(x => x.ListOfMetabolites).Where(x => x.mtbltDetails.In_customId == dm.mtbltDetails.In_customId).Select(x => x.mtbltVals.Non_imputed).Where(x => x != -1).Min();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CalculateMetaboliteRatios()
        {
            List<string> phenotypes = metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().Where(x => !publicVariables.excludedPhenotypes.Contains(x)).OrderBy(x => x).ToList();
            List<tmpPerSampleRatios> tmpListOfPerSampleRatios; //ID nominator, charge nominator, ID denominator, charge denominator, phenotype, ratio
            foreach (string tissue in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).OrderBy(x => x).Distinct())
            {
                //calculate per sample ratios and store them in a list of tuples that contains
                //custom ID nominator, charge nominator, custom ID denominator, charge denominator, phenotype, ratio
                //charges for nominator and denominator are used for cases such as LCMS where we need to check the ratio for metabolites that come
                //from different charges
                tmpListOfPerSampleRatios = new List<tmpPerSampleRatios>();
                foreach (string sampleID in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Id).Distinct())
                {
                    foreach (string charge_nom in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                    {
                        foreach (string cid_nom in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_nom && x.Id == sampleID)
                                                                                    .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                        {
                            foreach (string charge_denom in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                            {
                                foreach (string cid_denom in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_denom && x.Id == sampleID)
                                                                                        .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                                {
                                    tmpListOfPerSampleRatios.Add(new tmpPerSampleRatios
                                    {
                                        sampleID = sampleID,
                                        cid_nom = cid_nom,
                                        charge_nom = charge_nom,
                                        cid_denom = cid_denom,
                                        charge_denom = charge_denom,
                                        phenotype = metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge_nom && x.Id == sampleID).Phenotype,
                                        ratio = metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge_nom && x.Id == sampleID).ListOfMetabolites
                                                .First(x => x.mtbltDetails.In_customId == cid_nom).mtbltVals.Imputed /
                                        metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge_denom && x.Id == sampleID).ListOfMetabolites
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
                        foreach (string charge_nom in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                        {
                            foreach (string cid_nom in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_nom && x.Phenotype == phenotypes.ElementAt(i))
                                                                                .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                            {
                                foreach (string charge_denom in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).OrderBy(x => x).Distinct())
                                {
                                    foreach (string cid_denom in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge_denom && x.Phenotype == phenotypes.ElementAt(j))
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
                                        metaboliteRatios.ListOfInterMetaboliteConnections.Add(new metaboliteRatio()
                                        {
                                            Tissue = tissue,
                                            CustomId_nominator = cid_nom,
                                            Name_nominator = metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Phenotype == phenotypes.ElementAt(i))
                                                                                          .SelectMany(x => x.ListOfMetabolites)
                                                                                          .First(x => x.mtbltDetails.In_customId == cid_nom)
                                                                                          .mtbltDetails.In_Name,
                                            Charge_nominator = charge_nom,
                                            CustomId_denominator = cid_denom,
                                            Name_denominator = metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Phenotype == phenotypes.ElementAt(j))
                                                                                            .SelectMany(x => x.ListOfMetabolites)
                                                                                            .First(x => x.mtbltDetails.In_customId == cid_denom)
                                                                                            .mtbltDetails.In_Name,
                                            Charge_denominator = charge_denom,
                                            Group1 = phenotypes.ElementAt(i),
                                            Group2 = phenotypes.ElementAt(j),
                                            PValue = permutationTest.wilcoxonMannWhitneyPermutationTest(new string[] { "Phenotype", "Ratio" }, new IEnumerable[0], new List<string>())
                                        });
                                        metaboliteRatios.ListOfInterMetaboliteConnections.Last().fillInListOfPerSampleRatios(tmpListOfPerSampleRatios.Where(x => x.cid_nom == cid_nom && x.charge_nom == charge_nom && x.cid_denom == cid_denom && x.charge_denom == charge_denom && (x.phenotype == phenotypes.ElementAt(i) || x.phenotype == phenotypes.ElementAt(j)))
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

        private static void CalculatePvalues()
        {
            //select all phenotypes and add them in a list
            //we do it in order to avoid querying the list listOfPatients every time
            //we want to increase efficiency
            List<string> phenotypes = metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Phenotype).Distinct().Where(x => !publicVariables.excludedPhenotypes.Contains(x)).OrderBy(x => x).ToList();

            //create two list of arrays for imputed and non-imputed values
            //we need these lists to compute p-values
            //ANOVA and t-test methods require arrays
            List<double[]> metabolite_values;

            //create list of arrays for imputed and non-imputed values
            //we need these lists to compute p-values
            //ANOVA and t-test methods require arrays
            List<clinicalDataVals> listOfCovars;

            //create two list of tuples for imputed and non-imputed values
            //we need these lists to compute ratios between average values of pairs of phenotypes
            //in each tuple we store the two phenotypes between which the ratio was computed and the ratio value
            List<msMetabolite.stats.pairwiseFoldChangeValues> ratio;

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
            List<string> numClinicalDataNames = metaboliteLevels.List_SampleForTissueAndCharge.First()
                .ListOfNumClinicalData.Where(x => x.typeOf == sampleForTissueAndCharge.sampleClinicalData.type.numeric)
                .Select(x => x.name).ToList();

            //decide if you run a parametric or non-parametric test. Parametric is decided if there are at least 30 samples per phenotype.
            //true for parametric; false for non-parametric
            bool? paramOrNonParam;

            //tmp vals
            sampleForTissueAndCharge.sampleClinicalData scd;

            //loop through each tissue
            foreach (string tissue in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct())
            {
                dictOfCorrelationToMetabsVals = correlations.correlateMetabsToMetabs(tissue);
                //loop through each charge
                foreach (string charge in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct())
                {
                    paramOrNonParam = null;
                    //loop through each metabolite by selecting unique custom IDs
                    foreach (string custid in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge).SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct())
                    {
                        //initialize the lists and arrays
                        metabolite_values = new List<double[]>();
                        listOfCovars = new List<clinicalDataVals>();
                        ratio = new List<msMetabolite.stats.pairwiseFoldChangeValues>();
                        statTestPvalue = new List<msMetabolite.stats.pairwiseTestValues>();
                        phenotypeColumnNames = new string[] { "Phenotype", custid };
                        correlationToCovariatesVals = new List<msMetabolite.stats.corrVars>();
                        regressionVals = new List<msMetabolite.stats.regressValues>();

                        //loop over the phenotypes to keep info for numerical covariates for which we need to compute regressions
                        //each of the arrays contains as many elements as there are for each phenotype
                        //
                        //add metabolite level values in 
                        foreach (string pheno in phenotypes)
                        {
                            double[] tmpArrayMetabVal = new double[metaboliteLevels.List_SampleForTissueAndCharge.Count(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno)];
                            int i = 0;
                            foreach (string sampleId in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno).Select(x => x.Id))
                            {
                                tmpArrayMetabVal[i] = metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno && x.Id == sampleId)
                                    .ListOfMetabolites.First(x => x.mtbltDetails.In_customId == custid).mtbltVals.Imputed;
                                foreach (string ncv in publicVariables.normCovars)
                                {
                                    scd = metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge && x.Phenotype == pheno && x.Id == sampleId).ListOfNumClinicalData.First(x => x.name == ncv);
                                    if (listOfCovars.Any(x => x.covarName == ncv))
                                    {
                                        // add a new value for the covariate
                                        if (scd.typeOf == sampleForTissueAndCharge.sampleClinicalData.type.numeric)
                                        {
                                            // both covariate and phenotype exists
                                            if (listOfCovars.First(x => x.covarName == ncv).n_dictOfVals.ContainsKey(pheno))
                                            {
                                                // add a new value for the covariate
                                                listOfCovars.First(x => x.covarName == ncv).n_dictOfVals[pheno].Add(scd.n_value);
                                            }
                                            else // the current phenotype does not exist for the given covariate
                                            {
                                                // add a new phenotype on the covariate
                                                listOfCovars.First(x => x.covarName == ncv).n_dictOfVals.Add(pheno, new List<double>() { scd.n_value });
                                            }
                                        }
                                        else if (scd.typeOf == sampleForTissueAndCharge.sampleClinicalData.type.categorical)
                                        {
                                            // both covariate and phenotype exists
                                            if (listOfCovars.First(x => x.covarName == ncv).c_dictOfVals.ContainsKey(pheno))
                                            {
                                                // add a new value for the covariate
                                                listOfCovars.First(x => x.covarName == ncv).c_dictOfVals[pheno].Add(scd.c_value);
                                            }
                                            else // the current phenotype does not exist for the given covariate
                                            {
                                                // add a new phenotype on the covariate
                                                listOfCovars.First(x => x.covarName == ncv).c_dictOfVals.Add(pheno, new List<string>() { scd.c_value });
                                            }
                                        }
                                    }
                                    else //neither the covariate nor the phenotype exist
                                    {
                                        // add a new covariate
                                        if (scd.typeOf == sampleForTissueAndCharge.sampleClinicalData.type.numeric)
                                        {
                                            listOfCovars.Add(new clinicalDataVals(ncv, pheno, scd.n_value, null, sampleId));
                                        }
                                        else if (scd.typeOf == sampleForTissueAndCharge.sampleClinicalData.type.categorical)
                                        {
                                            listOfCovars.Add(new clinicalDataVals(ncv, pheno, -1, scd.c_value, sampleId));
                                        }
                                    }
                                }
                                i++;
                            }
                            metabolite_values.Add(tmpArrayMetabVal);
                        }

                        //decide whether we apply parametric or non parametric test
                        if (metabolite_values.All(x => x.Length >= publicVariables.parametricTestThreshold))
                        {
                            if (paramOrNonParam == null) //if paramOrNonParam has not been set yet, then just set it
                            {
                                paramOrNonParam = true;
                            }
                            else if (paramOrNonParam == false) //if before we dicided non-parametric and now parametric within the same tissue and charge then we have a problem
                            {
                                outputToLog.WriteErrorLine("Inconsistent number of samples for tissue: " + tissue + " and charge: " + charge + " for metabolite " + custid);
                            }
                            else //NO PROBLEM: paramOrNonParam remains true for parametric test
                            {
                                paramOrNonParam = true;
                            }
                        }
                        else
                        {
                            if (paramOrNonParam == null) //if paramOrNonParam has not been set yet, then just set it
                            {
                                paramOrNonParam = false;
                            }
                            else if (paramOrNonParam == true) //if before we dicided parametric and now non-parametric within the same tissue and charge then we have a problem
                            {
                                outputToLog.WriteErrorLine("Inconsistent number of samples for tissue: " + tissue + " and charge: " + charge + " for metabolite " + custid);
                            }
                            else //NO PROBLEM: paramOrNonParam remains false for non-parametric test
                            {
                                paramOrNonParam = false;
                            }
                        }

                        // initialize
                        IEnumerable[] s = new IEnumerable[publicVariables.cofCovars.Count];
                        List<tmpPerPhenoConfounders> s_pheno = new List<tmpPerPhenoConfounders>();
                        for (int i = 0; i < phenotypes.Count; i++)
                        {
                            for (int j = i + 1; j < phenotypes.Count; j++)
                            {
                                s_pheno.Add(new tmpPerPhenoConfounders()
                                {
                                    group1 = phenotypes[i],
                                    group2 = phenotypes[j],
                                    s_p = new IEnumerable[publicVariables.cofCovars.Count],
                                    stype = new List<string>()
                                });
                            }
                        }
                        List<string> stype = new List<string>();
                        int cnt = 0;
                        foreach (string cf in publicVariables.cofCovars)
                        {
                            if (listOfCovars.Any(x => x.covarName == cf))
                            {
                                if (listOfCovars.First(x => x.covarName == cf).typeOf == clinicalDataVals.type.numeric)
                                {
                                    List<double> sAddNum = new List<double>();
                                    foreach (List<double> san in listOfCovars.First(x => x.covarName == cf).n_dictOfVals.Select(x => x.Value.ToList()).ToList())
                                    {
                                        sAddNum.AddRange(san);
                                    }
                                    s.SetValue(sAddNum.ToArray(), cnt);
                                    stype.Add("number");

                                    for (int i = 0; i < phenotypes.Count; i++)
                                    {
                                        for (int j = i + 1; j < phenotypes.Count; j++)
                                        {
                                            sAddNum = listOfCovars.First(x => x.covarName == cf).n_dictOfVals
                                                .Where(x => x.Key == phenotypes[i]).Select(x => x.Value.ToList()).ToList().First();
                                            sAddNum.AddRange(listOfCovars.First(x => x.covarName == cf).n_dictOfVals
                                                .Where(x => x.Key == phenotypes[j]).Select(x => x.Value.ToList()).ToList().First());
                                            s_pheno.First(x => x.group1 == phenotypes[i] && x.group2 == phenotypes[j]).s_p.SetValue(sAddNum.ToArray(), cnt);
                                            s_pheno.First(x => x.group1 == phenotypes[i] && x.group2 == phenotypes[j]).stype.Add("number");
                                        }
                                    }

                                    cnt++;
                                }
                                else if (listOfCovars.First(x => x.covarName == cf).typeOf == clinicalDataVals.type.categorical)
                                {
                                    List<string> sAddCat = new List<string>();
                                    foreach (List<string> sac in listOfCovars.First(x => x.covarName == cf).c_dictOfVals.Select(x => x.Value.ToList()).ToList())
                                    {
                                        sAddCat.AddRange(sac);
                                    }
                                    s.SetValue(sAddCat.ToArray(), cnt);
                                    stype.Add("factor");

                                    for (int i = 0; i < phenotypes.Count; i++)
                                    {
                                        for (int j = i + 1; j < phenotypes.Count; j++)
                                        {
                                            sAddCat = listOfCovars.First(x => x.covarName == cf).c_dictOfVals
                                                .Where(x => x.Key == phenotypes[i]).Select(x => x.Value.ToList()).ToList().First();
                                            sAddCat.AddRange(listOfCovars.First(x => x.covarName == cf).c_dictOfVals
                                                .Where(x => x.Key == phenotypes[j]).Select(x => x.Value.ToList()).ToList().First());
                                            s_pheno.First(x => x.group1 == phenotypes[i] && x.group2 == phenotypes[j]).s_p.SetValue(sAddCat.ToArray(), cnt);
                                            s_pheno.First(x => x.group1 == phenotypes[i] && x.group2 == phenotypes[j]).stype.Add("factor");
                                        }
                                    }

                                    cnt++;
                                }
                            }
                            else
                            {
                                outputToLog.WriteWarningLine("Cofounder " + cf + " was not found among the covariates");
                            }
                        }

                        //loop over the phenotypes in order to get all potential pairs of phenotypes
                        //calculate the ratio for imputed and non-imputed values for pair-wise phenotypes, and store them in the corresponding lists of tuples
                        //calculate the p-value for imputed and non-imputed values for pair-wise phenotypes, and store them in the corresponding lists of tuples
                        //it does not matter that we use an ANOVA instead of a t-test for the permutation statistic here since they should coincide because an ANOVA for two classes is equal to a t-test
                        for (int i = 0; i < phenotypes.Count; i++)
                        {
                            for (int j = i + 1; j < phenotypes.Count; j++)
                            {
                                returnFCandCI rfcaci = foldChangeCI.calculateFoldChangeAndCI(metabolite_values.ElementAt(j), metabolite_values.ElementAt(i));
                                ratio.Add(new msMetabolite.stats.pairwiseFoldChangeValues()
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
                                    pairValue = permutationTest.wilcoxonMannWhitneyPermutationTest(phenotypeColumnNames,
                                        s_pheno.First(x => x.group1 == phenotypes.ElementAt(i) && x.group2 == phenotypes.ElementAt(j)).s_p,
                                        s_pheno.First(x => x.group1 == phenotypes.ElementAt(i) && x.group2 == phenotypes.ElementAt(j)).stype)
                                });
                            }
                        }

                        //calculate the two-group or multi-group p-values
                        permutationTest.returnIEnurable(phenotypes, metabolite_values);
                        if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two)
                        {
                            if (paramOrNonParam == true)
                            {
                                multiGroupTestPvalue = permutationTest.ttestPermutationTest(phenotypeColumnNames);
                            }
                            else
                            {
                                multiGroupTestPvalue = permutationTest.wilcoxonMannWhitneyPermutationTest(phenotypeColumnNames, s, stype);
                            }
                        }
                        else
                        {
                            if (paramOrNonParam == true)
                            {
                                multiGroupTestPvalue = permutationTest.aovpPermutationTest(phenotypeColumnNames);
                            }
                            else
                            {
                                multiGroupTestPvalue = permutationTest.kruskalWallisPermutationTest(phenotypeColumnNames, s, stype);
                            }
                        }

                        //linear regression model between metabolite levels and cofounders
                        foreach (clinicalDataVals nv in listOfCovars)
                        {
                            if (!publicVariables.cofCovars.Contains(nv.covarName))
                            {
                                if (nv.typeOf == clinicalDataVals.type.numeric)
                                {
                                    if (nv.n_dictOfVals.SelectMany(x => x.Value).Distinct().Count() == 1)
                                    {
                                        regressionVals.Add(new msMetabolite.stats.regressValues()
                                        {
                                            clinical_data_name = nv.covarName,
                                            regrAdjRsquare = 0,
                                            regrPvalue = 1
                                        });
                                    }
                                    else
                                    {
                                        permutationTest.returnIEnurableNumeric(nv.n_dictOfVals.Select(x => x.Value.ToArray()).ToList(), metabolite_values);
                                        regressionVals.Add(permutationTest.linearRegressionTest(new string[] { nv.covarName, custid }, "number", s, stype));
                                    }
                                }
                                else if (nv.typeOf == clinicalDataVals.type.categorical)
                                {
                                    if (nv.c_dictOfVals.SelectMany(x => x.Value).Distinct().Count() == 1)
                                    {
                                        regressionVals.Add(new msMetabolite.stats.regressValues()
                                        {
                                            clinical_data_name = nv.covarName,
                                            regrAdjRsquare = 0,
                                            regrPvalue = 1
                                        });
                                    }
                                    else
                                    {
                                        permutationTest.returnIEnurableCategoric(nv.c_dictOfVals.Select(x => x.Value.ToArray()).ToList(), metabolite_values);
                                        regressionVals.Add(permutationTest.linearRegressionTest(new string[] { nv.covarName, custid }, "factor", s, stype));
                                    }
                                }
                            }
                        }

                        //calculate spearman correlations
                        if (numClinicalDataNames.Count > 0)
                        {
                            correlationToCovariatesVals = correlations.correlateMetabsToCovariates(tissue, charge, custid, numClinicalDataNames);
                        }

                        foreach (sampleForTissueAndCharge sftac in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge))
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

        private class clinicalDataVals
        {
            public Dictionary<string, List<double>> n_dictOfVals;
            public Dictionary<string, List<string>> c_dictOfVals;
            public string covarName;
            public enum type
            {
                numeric,
                categorical
            }
            public type typeOf { get; set; }
            public string sampleID { get; set; }

            // in order to add a numeric value send _cv argument as null
            // for the opposite send the _nv argument as null
            public clinicalDataVals(string _cn, string _pheno, double _nv, string _cv, string _sid)
            {
                covarName = _cn;
                sampleID = _sid;
                if (string.IsNullOrEmpty(_cv) || string.IsNullOrWhiteSpace(_cv))
                {
                    n_dictOfVals = new Dictionary<string, List<double>>() { { _pheno, new List<double>() { _nv } } };
                    typeOf = type.numeric;
                }
                else
                {
                    c_dictOfVals = new Dictionary<string, List<string>>() { { _pheno, new List<string>() { _cv } } };
                    typeOf = type.categorical;
                }
            }
        }

        private class tmpPerSampleRatios
        {
            public string sampleID { get; set; }
            public string cid_nom { get; set; }
            public string charge_nom { get; set; }
            public string cid_denom { get; set; }
            public string charge_denom { get; set; }
            public string phenotype { get; set; }
            public double ratio { get; set; }
        }

        private class tmpPerPhenoConfounders
        {
            public string group1 { get; set; }
            public string group2 { get; set; }
            public IEnumerable[] s_p { get; set; }
            public List<string> stype { get; set; }
        }
    }
}
