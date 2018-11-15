using RDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class correlations
    {
        public static List<msMetabolite.stats.corrVars> correlateMetabsToCovariates(string tissue, string charge, string metaboliteID, List<string> clinDataNames)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            NumericMatrix ms = rEngineInstance.engine.CreateNumericMatrix(correlationMetabsCovarsMatrix(tissue, charge, metaboliteID));
            rEngineInstance.engine.SetSymbol("ms", ms);
            rEngineInstance.engine.Evaluate("df <- as.data.frame(ms)");

            rEngineInstance.engine.Evaluate("corr_res <- rcorr.adjust(ms, type = \"spearman\")");

            CharacterVector padjust = rEngineInstance.engine.Evaluate("corr_res$P[1,2:length(corr_res$P[1,])]").AsCharacter();
            CharacterVector punadjust = rEngineInstance.engine.Evaluate("corr_res$P.unadj[1,2:length(corr_res$P.unadj[1,])]").AsCharacter();
            NumericVector c = rEngineInstance.engine.Evaluate("corr_res$R$r[1,2:length(corr_res$R$r[1,])]").AsNumeric();

            List<double> pNumAdjust = new List<double>();
            double ptmp;
            foreach (var pa in padjust)
            {
                if (pa == "<.0001")
                {
                    pNumAdjust.Add(0.0001);
                }
                else if (Double.TryParse(pa, out ptmp)) // if done, then is a number
                {
                    pNumAdjust.Add(ptmp);
                }
                else
                {
                    pNumAdjust.Add(1);
                }
            }

            List<double> pNumUnadjust = new List<double>();
            foreach (var pa in punadjust)
            {
                if (pa == "<.0001")
                {
                    pNumUnadjust.Add(0.0001);
                }
                else if (Double.TryParse(pa, out ptmp)) // if done, then is a number
                {
                    pNumUnadjust.Add(ptmp);
                }
                else
                {
                    pNumUnadjust.Add(1);
                }
            }

            List<msMetabolite.stats.corrVars> listOfResults = new List<msMetabolite.stats.corrVars>();
            clinDataNames.ForEach(i => listOfResults.Add(new msMetabolite.stats.corrVars(){
                clinical_data_name = i,
                corr_value = c[clinDataNames.IndexOf(i)],
                pValueAdjust = pNumAdjust[clinDataNames.IndexOf(i)],
                pValueUnadjust = pNumUnadjust[clinDataNames.IndexOf(i)]
            }));

            rEngineInstance.engine.Evaluate("rm(ms, df, corr_res)");

            //Re-enable Console printings
            Console.SetOut(stdOut);

            return listOfResults;
        }

        public static Dictionary<string, List<msMetabolite.stats.corrMetabs>> correlateMetabsToMetabs(string tissue)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            NumericMatrix ms = rEngineInstance.engine.CreateNumericMatrix(correlationMetabsMetabsMatrix(tissue));
            rEngineInstance.engine.SetSymbol("ms", ms);
            rEngineInstance.engine.Evaluate("df <- as.data.frame(ms)");

            rEngineInstance.engine.Evaluate("corr_res <- rcorr.adjust(ms, type = \"spearman\")");
            rEngineInstance.engine.Evaluate("print(corr_res$P)");
            CharacterMatrix padjust = rEngineInstance.engine.Evaluate("corr_res$P").AsCharacterMatrix();
            CharacterMatrix punadjust = rEngineInstance.engine.Evaluate("corr_res$P.unadj").AsCharacterMatrix();
            NumericMatrix c = rEngineInstance.engine.Evaluate("corr_res$R$r").AsNumericMatrix();

            double[,] pNumAdjust = new double[padjust.RowCount, padjust.ColumnCount];
            double[,] pNumUnadjust = new double[padjust.RowCount, padjust.ColumnCount];
            double ptmp;
            for (int i = 0; i < padjust.RowCount; i++)
            {
                for (int j = 0; j < padjust.ColumnCount; j++)
                {
                    //Adjusted
                    if (padjust[i, j] == "<.0001")
                    {
                        pNumAdjust[i, j] = 0.0001;
                    }
                    else if (Double.TryParse(padjust[i, j], out ptmp)) // if done, then is a number
                    {
                        pNumAdjust[i, j] = ptmp;
                    }
                    else
                    {
                        pNumAdjust[i, j] = 1;
                    }

                    //Undjusted
                    if (punadjust[i, j] == "<.0001")
                    {
                        pNumUnadjust[i, j] = 0.0001;
                    }
                    else if (Double.TryParse(punadjust[i, j], out ptmp)) // if done, then is a number
                    {
                        pNumUnadjust[i, j] = ptmp;
                    }
                    else
                    {
                        pNumUnadjust[i, j] = 1;
                    }
                }
            }

            List<string> metabNameList = new List<string>();
            foreach (string chrg in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Charge).Distinct())
            {
                metabNameList.AddRange(metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == chrg)
                    .ListOfMetabolites.Select(x => x.mtbltDetails.In_customId + "_" + chrg).ToList());
            }

            Dictionary<string, List<msMetabolite.stats.corrMetabs>> dictOfResults = new Dictionary<string, List<msMetabolite.stats.corrMetabs>>();
            for (int i = 0; i < metabNameList.Count; i++)
            {
                dictOfResults.Add(metabNameList[i], new List<msMetabolite.stats.corrMetabs>());
                for (int j = 0; j < metabNameList.Count; j++)
                {
                    dictOfResults[metabNameList[i]].Add(new msMetabolite.stats.corrMetabs()
                    {
                        metab_id = metabNameList[j],
                        corr_value = c[i, j],
                        pValueAdjust = pNumAdjust[i, j],
                        pValueUnadjust = pNumUnadjust[i, j]
                    });
                }
            }
            rEngineInstance.engine.Evaluate("rm(ms, df, corr_res)");
            //Re-enable Console printings
            Console.SetOut(stdOut);

            return dictOfResults;
        }

        /// <summary>
        /// returns a matrix of doubles in order to be used as a data-frame for the R code for the spearman correlation test.
        /// the first column contains the metabolite for which we are checking the correlation and the rest of the columns contain the clinical data values
        /// </summary>
        /// <param name="tissue">Tissue</param>
        /// <param name="charge">Charge</param>
        /// <param name="metaboliteID">The custom internal ID of the examined metabolite</param>
        /// <returns></returns>
        private static double[,] correlationMetabsCovarsMatrix(string tissue, string charge, string metaboliteID)
        {
            int numOfSamples = metaboliteLevels.List_SampleForTissueAndCharge.Count(x => x.Tissue == tissue && x.Charge == charge);
            int numOfClinicalData = metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge).ListOfNumClinicalData.Count + 1;
            double[,] ctm = new double[numOfSamples, numOfClinicalData];
            int cntRow = 0, cntCol = 0;
            foreach (sampleForTissueAndCharge sftac in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge))
            {
                ctm[cntRow, cntCol++] = sftac.ListOfMetabolites.First(x => x.mtbltDetails.In_customId == metaboliteID).mtbltVals.Imputed;
                foreach (sampleForTissueAndCharge.sampleClinicalData t_cd in sftac.ListOfNumClinicalData)
                {
                    ctm[cntRow, cntCol++] = t_cd.n_value;
                }
                cntRow++;
                cntCol = 0;
            }
            return ctm;
        }

        private static double[,] correlationMetabsMetabsMatrix(string tissue)
        {
            int numOfSamples = metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Id).Distinct().Count();
            int numOfMetabolites = 0;
            foreach (string chrg in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Charge).Distinct())
            {
                numOfMetabolites += metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == chrg)
                    .SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct().Count();
            }
            double[,] ctm = new double[numOfSamples, numOfMetabolites];
            int cntRow = 0, cntColStart = 0, cntCol = cntColStart;
            foreach (string chrg in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue).Select(x => x.Charge).Distinct())
            {
                foreach (sampleForTissueAndCharge sftac in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == chrg))
                {
                    foreach (sampleForTissueAndCharge.parentMetabolite pm in sftac.ListOfMetabolites)
                    {
                        ctm[cntRow, cntCol++] = pm.mtbltVals.Imputed;
                    }
                    cntRow++;
                    cntCol = cntColStart;
                }
                cntRow = 0;
                cntColStart += metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == chrg).ListOfMetabolites.Count;
                cntCol = cntColStart;
            }
            return ctm;
        }
    }
}
