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
                List<string> covariate_names = input.ReadLine().Split('\t').ToList(), covariate_types = input.ReadLine().Split('\t').ToList();
                //check if headers of metadata files are equal
                if (covariate_names.Count != covariate_types.Count)
                {
                    Console.WriteLine("Meatdata file contains unequal number of covariates and types!");
                    Environment.Exit(0);
                }

                for (int i = 0; i < covariate_names.Count; i++)
                {
                    if (covariate_types[i].ToLower() == "ignore")
                    {
                        Console.WriteLine("ignored covariate: " + covariate_names[i].ToLower());
                        if (ignored_covariates.ContainsKey(covariate_names[i]))
                        {
                            Console.WriteLine("Covariate " + covariate_names[i] + " already exists! Remove duplicates!");
                            Environment.Exit(0);
                        }
                        else
                        {
                            ignored_covariates.Add(covariate_names[i].ToLower(), i);
                        }
                    }
                    else if (covariate_types[i].ToLower() == "id")
                    {
                        if (id_index == -1)
                        {
                            id_index = i;
                        }
                        else
                        {
                            Console.WriteLine("ID index had been already set once! Cannot reset it!");
                        }
                    }
                    else if (covariate_types[i].ToLower() == "decision")
                    {
                        if (decision_index == -1)
                        {
                            decision_index = i;
                        }
                        else
                        {
                            Console.WriteLine("Decision index had been already set once! Cannot reset it!");
                        }
                    }
                    else if (covariate_types[i].ToLower() == "numeric")
                    {
                        if (numerical_covariates.ContainsKey(covariate_names[i].ToLower()))
                        {
                            Console.WriteLine("Covariate " + covariate_names[i] + " already exists! Remove duplicates!");
                            Environment.Exit(0);
                        }
                        else
                        {
                            numerical_covariates.Add(covariate_names[i].ToLower(), i);
                        }
                    }
                    else if (covariate_types[i].ToLower() == "categorical")
                    {
                        if (categorical_covariates.ContainsKey(covariate_names[i].ToLower()))
                        {
                            Console.WriteLine("Covariate " + covariate_names[i] + " already exists! Remove duplicates!");
                            Environment.Exit(0);
                        }
                        else
                        {
                            categorical_covariates.Add(covariate_names[i].ToLower(), i);
                        }
                    }
                    else if (covariate_types[i].ToLower() == "sampleweight")
                    {
                        if (sampleWeight_covariates.ContainsKey(covariate_names[i].ToLower()))
                        {
                            Console.WriteLine("Covariate " + covariate_names[i] + " already exists! Remove duplicates!");
                            Environment.Exit(0);
                        }
                        else
                        {
                            sampleWeight_covariates.Add(covariate_names[i].ToLower(), i);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unknown type " + covariate_types[i] + " covariate!");
                        Environment.Exit(0);
                    }
                }
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
                            Console.WriteLine(kvp_s_iv.Key + " for phenotype " + cdfs.Phenotype + " could not be imputed! Too few records!");
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
                            Console.WriteLine(kvp_sw_iv.tissue + " " + kvp_sw_iv.charge + " for phenotype " + cdfs.Phenotype + " could not be imputed! Too few records!");
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
