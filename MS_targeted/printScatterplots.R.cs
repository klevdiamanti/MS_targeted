using RDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class printScatterplots
    {
        public static void printMyScatterplot(string outputFile)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            rEngineInstance.engine.Evaluate(
                @"scatterplotTissueCharge <- function(df, plotTitle, plotYlabel, coords, rcorrLabel) {

                    corrLine <- coef(lm(df$metab_vals ~ df$clindt_vals));

                    p <- ggplot(df) +
                                theme_bw(base_size = 18) +
                                ggtitle(paste(plotTitle)) +
                                ylab(plotYlabel) +
                                geom_point(aes(x = df$clindt_vals, y = df$metab_vals, color = factor(df$pheno))) +
                                geom_abline(intercept = corrLine[1], slope = corrLine[2]) +
                                geom_text(data = NULL, x = coords[1], y = coords[2], label = rcorrLabel) +
                                theme(legend.position = ""none"",
                                        axis.title.x = element_blank());
                    return (p);
                };");

            //keep track of the last charge value so that we know when to print the tissue names
            string lastCharge = metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x).Last();
            //keep track of the first tissue value so that we know when to print the charge names
            string firstTissue = metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x).First();

            //assisting list of tuples
            List<rDataFrame> phenoMetabValPairs;
            Tuple<double, double> corrPval;
            corrValCoords _corrValCoord;

            foreach (sampleForTissueAndCharge.sampleClinicalData sClinData in metaboliteLevels.List_SampleForTissueAndCharge.First().ListOfNumClinicalData)
            {
                if (sClinData.typeOf == sampleForTissueAndCharge.sampleClinicalData.type.categorical)
                {
                    continue;
                }
                
                //open the pdf stream
                rEngineInstance.engine.Evaluate(@"pdf(file=""" + outputFile.Replace("\\", "/") + sClinData.name + @".pdf"", width=14, height=9)");

                //loop over custom metabolite IDs
                foreach (string mtblid in metaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct().OrderBy(x => x))
                {
                    //extract all the metabolites with the mtblid custom metabolite ID
                    msMetabolite mtbl = metaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).First(x => x.mtbltDetails.In_customId == mtblid).mtbltDetails;
                    //initialize the list that will store the boxplots
                    rEngineInstance.engine.Evaluate("accumulateScatterplots <- list()");

                    //loop over charges
                    foreach (string charge in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                    {
                        //loop over tissues
                        foreach (string tissue in metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
                        {
                            //set the plot title
                            rEngineInstance.engine.SetSymbol("scatterPlotTitleAndYlabel", returnTitleAndYlabelCharacterVector(charge, lastCharge, tissue, firstTissue));

                            //if the metabolite has been detected for the given combination of tissue and charge then do the plot
                            if (metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && !publicVariables.excludedPhenotypes.Contains(x.Phenotype))
                                .SelectMany(x => x.ListOfMetabolites).Any(x => x.mtbltDetails.In_customId == mtbl.In_customId))
                            {
                                //initialize the assisting variable
                                phenoMetabValPairs = new List<rDataFrame>();

                                //loop over all the metabolites for tissue and charge and non-ignore phenotypes
                                //in order to fill in the assisting variables
                                //these variables serve for significance symbols in the plot and for where to plce it in the plot
                                foreach (sampleForTissueAndCharge sftac in metaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && !publicVariables.excludedPhenotypes.Contains(x.Phenotype)))
                                {
                                    phenoMetabValPairs.Add(new rDataFrame(){
                                            phenotype = sftac.Phenotype,
                                            metabolite = sftac.ListOfMetabolites.First(x => x.mtbltDetails.In_customId == mtbl.In_customId).mtbltVals.Imputed,
                                            clinical_data = sftac.ListOfNumClinicalData.First(x => x.name == sClinData.name).n_value
                                    });
                                }

                                //retrieve correlation value and p-value between metabolite and clinical data
                                corrPval = metaboliteLevels.List_SampleForTissueAndCharge.First(x => x.Tissue == tissue && x.Charge == charge && !publicVariables.excludedPhenotypes.Contains(x.Phenotype))
                                    .ListOfMetabolites.First(x => x.mtbltDetails.In_customId == mtbl.In_customId).mtbltDetails.ListOfStats.CorrelationValues.Where(x => x.clinical_data_name == sClinData.name)
                                    .Select(x => new Tuple<double, double>(Math.Round(x.corr_value, 2), x.pValueUnadjust)).ToList().First();

                                //significance symbols and coordinates
                                _corrValCoord = returnSignificanceDataFrame(phenoMetabValPairs, corrPval);
                                rEngineInstance.engine.SetSymbol("xCorrCoord", _corrValCoord.xCoord);
                                rEngineInstance.engine.SetSymbol("yCorrCoord", _corrValCoord.yCoord);
                                rEngineInstance.engine.Evaluate("coords <- c(xCorrCoord, yCorrCoord)");
                                rEngineInstance.engine.SetSymbol("rcorrLabel", _corrValCoord.label);

                                //sets the dataframe variable df in R
                                CharacterVector pheno = rEngineInstance.engine.CreateCharacterVector(phenoMetabValPairs.Select(x => x.phenotype).ToArray());
                                rEngineInstance.engine.SetSymbol("pheno", pheno);
                                NumericVector clindt_vals = rEngineInstance.engine.CreateNumericVector(phenoMetabValPairs.Select(x => x.clinical_data).ToArray());
                                rEngineInstance.engine.SetSymbol("clindt_vals", clindt_vals);
                                NumericVector metab_vals = rEngineInstance.engine.CreateNumericVector(phenoMetabValPairs.Select(x => x.metabolite).ToArray());
                                rEngineInstance.engine.SetSymbol("metab_vals", metab_vals);
                                rEngineInstance.engine.Evaluate("df <- cbind.data.frame(metab_vals, clindt_vals, pheno)");

                                //do not plot anything to the console
                                //adds the boxplot in the list of plots
                                //stop not printing stuff in teh console
                                rEngineInstance.engine.Evaluate(@"pdf(NULL); 
                                                            accumulateScatterplots[[length(accumulateScatterplots) + 1]] <- 
                                                                scatterplotTissueCharge(df, scatterPlotTitleAndYlabel[1], scatterPlotTitleAndYlabel[2], coords, rcorrLabel);
                                                            dev.off();");
                            }
                            else //if the metabolite has not been detected for this combination of tissue and charge provide and empty plot
                            {
                                //empty plot: is defined in the initialization of rEngineInstance
                                rEngineInstance.engine.Evaluate("accumulateScatterplots[[length(accumulateScatterplots) + 1]] <- nullPlot(scatterPlotTitleAndYlabel[1], scatterPlotTitleAndYlabel[2])");
                            }
                        }
                    }
                    //do the plot
                    rEngineInstance.engine.Evaluate(@"" + printBoxPlotGrid(metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().Count(), metaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().Count()) +
                            @"p <- grid.text(""x-" + sClinData.name + @" | y-" + mtbl.In_customId + @"_" + mtbl.In_Name + @""", x=unit(10,""mm""), y=unit(225,""mm""), just=c(""left"", ""top""), gp = gpar(fontface=""bold"", fontsize=24, col=""blue""));
                    print(p);");
                }
                rEngineInstance.engine.Evaluate(@"dev.off()");
            }

            //Re-enable Console printings
            Console.SetOut(stdOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charge"></param>
        /// <param name="lastCharge"></param>
        /// <param name="tissue"></param>
        /// <param name="firstTissue"></param>
        /// <returns>returns title and headers for the plots</returns>
        private static CharacterVector returnTitleAndYlabelCharacterVector(string charge, string lastCharge, string tissue, string firstTissue)
        {
            string[] myCharVect = new string[2];

            if (charge == lastCharge)
            {
                myCharVect[0] = tissue;
            }
            else
            {
                myCharVect[0] = "";
            }

            if (tissue == firstTissue)
            {
                myCharVect[1] = charge;
            }
            else
            {
                myCharVect[1] = "";
            }

            return rEngineInstance.engine.CreateCharacterVector(myCharVect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phenoMetabValPairs"></param>
        /// <param name="pValPairs"></param>
        /// <returns>a matrix of significance level symbols and coordinates of where to place them in the boxplot</returns>
        private static corrValCoords returnSignificanceDataFrame(List<rDataFrame> phenoMetabValPairs, Tuple<double, double> pValPairs)
        {
            return new corrValCoords()
            {
                xCoord = rEngineInstance.engine.CreateNumeric(phenoMetabValPairs.OrderByDescending(x => x.clinical_data).ElementAt(5).clinical_data),
                yCoord = rEngineInstance.engine.CreateNumeric(phenoMetabValPairs.OrderByDescending(x => x.metabolite).ElementAt(1).metabolite),
                label = rEngineInstance.engine.CreateCharacter(significanceSymbol(pValPairs.Item1, pValPairs.Item2))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pvalue"></param>
        /// <returns>the significance symbol for a given p-value</returns>
        private static string significanceSymbol(double corrValue, double pvalue)
        {
            if (pvalue > 0.05 || double.IsNaN(pvalue))
            {
                return corrValue.ToString() + "\n(ns)";
            }
            else if (pvalue <= 0.05 && pvalue > 0.01)
            {
                return corrValue.ToString() + "\n(*)";
            }
            else if (pvalue <= 0.01 && pvalue > 0.001)
            {
                return corrValue.ToString() + "\n(**)";
            }
            else if (pvalue <= 0.001 && pvalue > 0.0001)
            {
                return corrValue.ToString() + "\n(***)";
            }
            else
            {
                return corrValue.ToString() + "\n(****)";
            }
        }

        //This methods prints the whole arrangegrob line for prinitng
        private static string printBoxPlotGrid(int numCharge, int numTissue)
        {
            double myDevider = Math.Round(Convert.ToDouble(Convert.ToDouble(9) / Convert.ToDouble(numCharge)), 2);

            string s = @"grid.arrange(";

            //s += arrangeGrobLine(1, numTissue);
            for (int i = 1; i <= numCharge; i++)
            {
                s += arrangeGrobLine(((numTissue * (i - 1)) + 1), (i * numTissue));
                if (i < numCharge)
                {
                    s += @", ";
                }
            }

            if (numCharge == 2)
            {
                s += @", ncol=1, nrow=" + numCharge + @", heights=c(";

                for (int i = 0; i < (numCharge - 1); i++)
                {
                    s += @"" + myDevider + @"/10, ";
                }

                s += @"" + myDevider + @"/10));";
            }
            else
            {
                s += @");";
            }

            return s;
        }

        //This methods prints all the arrangeGrob lines needed based on the number of charges and tissues
        private static string arrangeGrobLine(int startFrom, int stopAt)
        {
            int numTissue = (stopAt - startFrom) + 1;
            double myDevider = Math.Round(Convert.ToDouble(Convert.ToDouble(10) / Convert.ToDouble(numTissue)), 2);
            string s = @"arrangeGrob(";
            for (int i = startFrom; i <= stopAt; i++)
            {
                s += @"accumulateScatterplots[[" + i + @"]], ";
            }
            s += @"ncol=" + numTissue + @", nrow=1, widths=c(";
            for (int i = 0; i < (numTissue - 1); i++)
            {
                s += @"" + myDevider + @"/10, ";
            }
            s += @"" + myDevider + @"/10))";
            return s;
        }

        public class corrValCoords
        {
            public NumericVector xCoord { get; set; }
            public NumericVector yCoord { get; set; }
            public CharacterVector label { get; set; }
        }

        public class rDataFrame
        {
            public string phenotype { get; set; }
            public double metabolite { get; set; }
            public double clinical_data { get; set; }
        }
    }
}
