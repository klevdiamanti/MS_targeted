using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
	public static class clinicalDataForSamples
	{
        private static List<clinicalDataForSample> ListTmp_clinicalDataForSamples = new List<clinicalDataForSample>();
        public static List<clinicalDataForSample> List_clinicalDataForSamples = new List<clinicalDataForSample>();

        /// <summary>
        /// Reads the file that contains the clinical data for the samples.
        /// </summary>
		public static void Read_ClinicalDataForSamples()
		{
			using (TextReader input = new StreamReader(@"" + publicVariables.clinicalDataFile))
			{
				string line = input.ReadLine();
                clinicalDataForSample cdfs;
				while ((line = input.ReadLine()) != null)
				{
                    cdfs = new clinicalDataForSample();
                    cdfs.setClinicalDataForSampleFromFile(line);
                    ListTmp_clinicalDataForSamples.Add(cdfs);
				}
			}
            impute_clinicalDataForSamples();
        }

		/// <summary>
		/// Imputes the missing clinical data values for samples.
		/// </summary>
        // imputation follows the concept below:
		// we use the average value of all clinical data for a given phenotype
		// in case there is only one instance of the clinical data for the phenotype and cannot be imputed the we just assign 1.
		private static void impute_clinicalDataForSamples()
        {
            clinicalDataForSample cdf;
            Dictionary<string, imputedValues> nv;
            foreach (clinicalDataForSample cdfs in ListTmp_clinicalDataForSamples)
			{
				nv = new Dictionary<string, imputedValues>();
                foreach (KeyValuePair<string, imputedValues> kvp_s_iv in cdfs.NormalizationVars)
				{
					if (kvp_s_iv.Value.Non_imputed == -1)
					{
						if (ListTmp_clinicalDataForSamples.Where(x => x.Phenotype == cdfs.Phenotype).SelectMany(x => x.NormalizationVars)
                            .Where(x => x.Key == kvp_s_iv.Key).Select(x => x.Value.Non_imputed).Count(x => x != -1) == 0)
						{
							Console.WriteLine(kvp_s_iv.Key + " for phenotype " + cdfs.Phenotype + " could not be imputed! Too few records!");
							nv.Add(kvp_s_iv.Key, new imputedValues()
							{
								Imputed = 1,
								Non_imputed = -1
							});
						}
						else
						{
							nv.Add(kvp_s_iv.Key, new imputedValues()
							{
								Imputed = ListTmp_clinicalDataForSamples.Where(x => x.Phenotype == cdfs.Phenotype).SelectMany(x => x.NormalizationVars)
                                                                        .Where(x => x.Key == kvp_s_iv.Key).Select(x => x.Value.Non_imputed).Where(x => x != -1).Average(),
								Non_imputed = -1
							});
						}
					}
					else
					{
                        nv.Add(kvp_s_iv.Key, new imputedValues()
						{
							Imputed = kvp_s_iv.Value.Imputed,
							Non_imputed = kvp_s_iv.Value.Non_imputed
						});
					}
				}

                cdf = new clinicalDataForSample();
                cdf.setClinicalDataForSampleFromVariables(cdfs.Id, cdfs.Phenotype, cdfs.TissueChargeWeightProblematic, cdfs.OtherPhenotypes, nv);
				List_clinicalDataForSamples.Add(cdf);
			}
        }
	}
}
