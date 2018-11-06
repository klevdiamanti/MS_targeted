using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class clinicalData
    {
        public static List<clinicalDataFS> List_clinicalData = new List<clinicalDataFS>();

        /// <summary>
        /// Reads the file that contains the clinical data for the samples.
        /// </summary>
		public static void Read_ClinicalDataForSamples()
        {
            List<clinicalDataFS> listTmp_clinicalDataFS = new List<clinicalDataFS>();

            //set numeric and categorical metadata dictionary indecies
            //set id index
            //set ignored covariates
            //set decision
            int id_index = -1, decision_index = -1;
            Dictionary<string, int> numerical_covariates = new Dictionary<string, int>(), categorical_covariates = new Dictionary<string, int>(),
                ignored_covariates = new Dictionary<string, int>(), sampleWeight_covariates = new Dictionary<string, int>();
            using (TextReader input = new StreamReader(@"" + publicVariables.clinicalDataFile))
            {
                #region set covariate indecies
                List<string> covariate_names = input.ReadLine().Split(publicVariables.breakCharInFile).Select(x => x.ToLower().Replace("-", "_").Replace(" ", "_").Replace("=", "_")).ToList(),
                    covariate_types = input.ReadLine().Split(publicVariables.breakCharInFile).Select(x => x.ToLower()).ToList();

                #region check headers and types of metadata
                if (covariate_names.Count != covariate_types.Count)
                {
                    outputToLog.WriteErrorLine("Meatdata file contains unequal number of covariates and types");
                }

                if (covariate_types.Count(x => x != "numeric" && x != "categorical") > 0)
                {
                    outputToLog.WriteErrorLine("Meatdata file contains unexpected types of entries. They should be strictly numeric or categorical");
                }

                if (covariate_names.Count != covariate_names.Distinct().Count())
                {
                    outputToLog.WriteErrorLine("Meatdata file contains duplicates in the header. There should be only unique entries");
                }
                #endregion

                foreach (string covar_name in covariate_names)
                {
                    if (publicVariables.sampleId == covar_name)
                    {
                        if (id_index == -1)
                        {
                            id_index = covariate_names.IndexOf(covar_name);
                        }
                        else
                        {
                            outputToLog.WriteWarningLine("CovarId index had been already set once. The second entry will be ignored. The metadata/covariates file contains duplicates");
                        }
                    }
                    else if (publicVariables.phenotype == covar_name)
                    {
                        if (decision_index == -1)
                        {
                            decision_index = covariate_names.IndexOf(covar_name);
                        }
                        else
                        {
                            outputToLog.WriteWarningLine("CovarDecision index had been already set once. The second entry will be ignored. The metadata/covariates file contains duplicates");
                        }
                    }
                    else if (publicVariables.normCovars.Any(x => x == covar_name))
                    {
                        if (covariate_types.ElementAt(covariate_names.IndexOf(covar_name)) == "numeric")
                        {
                            numerical_covariates.Add(covar_name, covariate_names.IndexOf(covar_name));
                        }
                        else if (covariate_types.ElementAt(covariate_names.IndexOf(covar_name)) == "categorical")
                        {
                            categorical_covariates.Add(covar_name, covariate_names.IndexOf(covar_name));
                        }
                        else
                        {
                            outputToLog.WriteErrorLine("Meatdata file contains unexpected types of entries. They should be strictly numeric or categorical");
                        }
                    }
                    else if (publicVariables.sampleWeight.Any(x => x.Item1 == covar_name))
                    {
                        if (covariate_types.ElementAt(covariate_names.IndexOf(covar_name)) != "numeric")
                        {
                            outputToLog.WriteErrorLine("Sample weight columns in the meatdata file should be strictly numeric");
                        }
                        sampleWeight_covariates.Add(publicVariables.sampleWeight.Where(x => x.Item1 == covar_name).Select(x => x.Item2 + "_" + x.Item3).First(),
                            covariate_names.IndexOf(covar_name));
                    }
                    else
                    {
                        ignored_covariates.Add(covar_name, covariate_names.IndexOf(covar_name));
                        outputToLog.WriteWarningLine(covar_name + " was not mentioned in the metadata/covariates file. Assuming ignore");
                    }
                }

                #region comment out
                //for (int i = 0; i < covariate_names.Count; i++)
                //{
                //    if (covariate_types[i].ToLower() == "ignore")
                //    {
                //        outputToLog.WriteWarningLine("ignored covariate: " + covariate_names[i].ToLower());
                //        if (ignored_covariates.ContainsKey(covariate_names[i]))
                //        {
                //            outputToLog.WriteErrorLine("Covariate " + covariate_names[i] + " already exists");
                //        }
                //        else
                //        {
                //            ignored_covariates.Add(covariate_names[i].ToLower(), i);
                //        }
                //    }
                //    else if (covariate_types[i].ToLower() == "id")
                //    {
                //        if (id_index == -1)
                //        {
                //            id_index = i;
                //        }
                //        else
                //        {
                //            outputToLog.WriteWarningLine("ID index had been already set once! Cannot reset it!");
                //        }
                //    }
                //    else if (covariate_types[i].ToLower() == "decision")
                //    {
                //        if (decision_index == -1)
                //        {
                //            decision_index = i;
                //        }
                //        else
                //        {
                //            outputToLog.WriteWarningLine("Decision index had been already set once! Cannot reset it!");
                //        }
                //    }
                //    else if (covariate_types[i].ToLower() == "numeric")
                //    {
                //        if (numerical_covariates.ContainsKey(covariate_names[i].ToLower()))
                //        {
                //            outputToLog.WriteErrorLine("Covariate " + covariate_names[i] + " already exists");
                //        }
                //        else
                //        {
                //            numerical_covariates.Add(covariate_names[i].ToLower(), i);
                //        }
                //    }
                //    else if (covariate_types[i].ToLower() == "categorical")
                //    {
                //        if (categorical_covariates.ContainsKey(covariate_names[i].ToLower()))
                //        {
                //            outputToLog.WriteErrorLine("Covariate " + covariate_names[i] + " already exists");
                //        }
                //        else
                //        {
                //            categorical_covariates.Add(covariate_names[i].ToLower(), i);
                //        }
                //    }
                //    else if (covariate_types[i].ToLower() == "sampleweight")
                //    {
                //        if (sampleWeight_covariates.ContainsKey(covariate_names[i].ToLower()))
                //        {
                //            outputToLog.WriteErrorLine("Covariate " + covariate_names[i] + " already exists");
                //        }
                //        else
                //        {
                //            sampleWeight_covariates.Add(covariate_names[i].ToLower(), i);
                //        }
                //    }
                //    else
                //    {
                //        outputToLog.WriteErrorLine("Unknown type " + covariate_types[i] + " covariate");
                //    }
                //}
                #endregion
                #endregion

                string line;
                clinicalDataFS cdfs;
                while ((line = input.ReadLine()) != null)
                {
                    cdfs = new clinicalDataFS();
                    cdfs.setClinicalDataFromFile(id_index, decision_index, sampleWeight_covariates, categorical_covariates, numerical_covariates, ignored_covariates, line);
                    listTmp_clinicalDataFS.Add(cdfs);
                }
            }
            impute_clinicalDataForSamples(listTmp_clinicalDataFS);
        }

        /// <summary>
        /// Imputes the missing clinical data values for samples.
        /// </summary>
        // imputation follows the concept below:
        // we use the average value of all clinical data for a given phenotype
        // in case there is only one instance of the clinical data for the phenotype and cannot be imputed the we just assign 1.
        private static void impute_clinicalDataForSamples(List<clinicalDataFS> listTmp_clinicalDataFS)
        {
            clinicalDataFS cdf;
            Dictionary<string, imputedValues> numcov;
            List<clinicalDataFS.sampleWeight> swcov;
            foreach (clinicalDataFS cdfs in listTmp_clinicalDataFS)
            {
                //Impute numerical covariates
                numcov = new Dictionary<string, imputedValues>();
                foreach (KeyValuePair<string, imputedValues> kvp_s_iv in cdfs.Numerical_covariates)
                {
                    if (kvp_s_iv.Value.Non_imputed == -1)
                    {
                        if (listTmp_clinicalDataFS.Where(x => x.Phenotype == cdfs.Phenotype).SelectMany(x => x.Numerical_covariates)
                            .Where(x => x.Key == kvp_s_iv.Key).Select(x => x.Value.Non_imputed).Count(x => x != -1) == 0)
                        {
                            outputToLog.WriteWarningLine(kvp_s_iv.Key + " for phenotype " + cdfs.Phenotype + " could not be imputed! Too few records!");
                            numcov.Add(kvp_s_iv.Key, new imputedValues()
                            {
                                Imputed = 1,
                                Non_imputed = -1
                            });
                        }
                        else
                        {
                            numcov.Add(kvp_s_iv.Key, new imputedValues()
                            {
                                Imputed = listTmp_clinicalDataFS.Where(x => x.Phenotype == cdfs.Phenotype).SelectMany(x => x.Numerical_covariates)
                                                                        .Where(x => x.Key == kvp_s_iv.Key).Select(x => x.Value.Non_imputed).Where(x => x != -1).Average(),
                                Non_imputed = -1
                            });
                        }
                    }
                    else
                    {
                        numcov.Add(kvp_s_iv.Key, new imputedValues()
                        {
                            Imputed = kvp_s_iv.Value.Imputed,
                            Non_imputed = kvp_s_iv.Value.Non_imputed
                        });
                    }
                }

                //Impute sample weights
                swcov = new List<clinicalDataFS.sampleWeight>();
                foreach (clinicalDataFS.sampleWeight kvp_sw_iv in cdfs.SampleWeight_covariates)
                {
                    if (kvp_sw_iv.weight.Non_imputed == -1)
                    {
                        if (listTmp_clinicalDataFS.Where(x => x.Phenotype == cdfs.Phenotype).SelectMany(x => x.SampleWeight_covariates)
                                                                            .Where(x => x.tissue == kvp_sw_iv.tissue && x.charge == kvp_sw_iv.charge)
                                                                            .Select(x => x.weight.Non_imputed).Count(x => x != -1) == 0)
                        {
                            outputToLog.WriteWarningLine(kvp_sw_iv.tissue + " " + kvp_sw_iv.charge + " for phenotype " + cdfs.Phenotype + " could not be imputed! Too few records!");
                            swcov.Add(new clinicalDataFS.sampleWeight()
                            {
                                tissue = kvp_sw_iv.tissue,
                                charge = kvp_sw_iv.charge,
                                weight = new imputedValues()
                                {
                                    Imputed = -1,
                                    Non_imputed = -1
                                }
                            });
                        }
                        else
                        {
                            swcov.Add(new clinicalDataFS.sampleWeight()
                            {
                                tissue = kvp_sw_iv.tissue,
                                charge = kvp_sw_iv.charge,
                                weight = new imputedValues()
                                {
                                    Imputed = listTmp_clinicalDataFS.Where(x => x.Phenotype == cdfs.Phenotype).SelectMany(x => x.SampleWeight_covariates)
                                                                            .Where(x => x.tissue == kvp_sw_iv.tissue && x.charge == kvp_sw_iv.charge)
                                                                            .Select(x => x.weight.Non_imputed).Where(x => x != -1).Average(),
                                    Non_imputed = -1
                                }
                            });
                        }
                    }
                    else
                    {

                        swcov.Add(new clinicalDataFS.sampleWeight()
                        {
                            tissue = kvp_sw_iv.tissue,
                            charge = kvp_sw_iv.charge,
                            weight = new imputedValues()
                            {
                                Imputed = kvp_sw_iv.weight.Imputed,
                                Non_imputed = kvp_sw_iv.weight.Non_imputed
                            }
                        });
                    }
                }

                cdf = new clinicalDataFS();
                cdf.setClinicalDataFromVariable(cdfs.Id, cdfs.Phenotype, swcov, cdfs.Categorical_covariates, numcov, cdfs.Ignored_covariates);
                List_clinicalData.Add(cdf);
            }
        }
    }
}
