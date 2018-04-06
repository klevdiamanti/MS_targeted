using RDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MS_targeted
{
    public static class printBoxplots
    {
        public static void printMyBoxplot(string outputFile, List<string> phenotypes)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            //print boxplot function
            rEngineInstance.engine.Evaluate(
                @"boxplotTissueCharge <- function(phenoMetabVals, plotTitle, plotYlabel, signifSymb, signifSymbYlevel, signifSymbXlevel) {
                    p <- ggplot(data=phenoMetabVals) + 
                                theme_bw(base_size=18) + 
                                ggtitle(plotTitle) +
                                ylab(plotYlabel) +
                                geom_boxplot(aes(factor(phenoMetabVals[,1]), phenoMetabVals[,2])) +" +
                                geom_textLinesInGgplot2Function(phenotypes.Count) +
                                @"theme(legend.position=""none"",
                                        axis.title.x = element_blank());
                    return (p);
                };");

            //keep track of the last charge value so that we know when to print the tissue names
            string lastCharge = msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x).Last();
            //keep track of the first tissue value so that we know when to print the charge names
            string firstTissue = msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x).First();

            //open the pdf stream
            rEngineInstance.engine.Evaluate(@"pdf(file=""" + outputFile.Replace("\\", "/") + @""", width=14, height=9)");

            //assisting list of tuples
            List<Tuple<string, double>> phenoMetabValPairs, pValPairs;

            //loop over custom metabolite IDs
            foreach (string mtblid in msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).Select(x => x.mtbltDetails.In_customId).Distinct().OrderBy(x => x))
            {
                //extract all the metabolites with the mtblid custom metabolite ID
                msMetabolite mtbl = msMetaboliteLevels.List_SampleForTissueAndCharge.SelectMany(x => x.ListOfMetabolites).First(x => x.mtbltDetails.In_customId == mtblid).mtbltDetails;
                //initialize the list that will store the boxplots
                rEngineInstance.engine.Evaluate("accumulateBoxplots <- list()");

                //loop over charges
                foreach (string charge in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().OrderBy(x => x))
                {
                    //loop over tissues
                    foreach (string tissue in msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().OrderBy(x => x))
                    {
                        //set the plot title
                        rEngineInstance.engine.SetSymbol("boxPlotTitleAndYlabel", returnTitleAndYlabelCharacterVector(charge, lastCharge, tissue, firstTissue));

                        //if the metabolite has been detected for the given combination of tissue and charge then do the plot
                        if (msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && !publicVariables.excludedPhenotypes.Contains(x.Phenotype))
                            .SelectMany(x => x.ListOfMetabolites).Any(x => x.mtbltDetails.In_customId == mtbl.In_customId))
                        {
                            //initialize the assisting variable
                            phenoMetabValPairs = new List<Tuple<string, double>>();
                            pValPairs = new List<Tuple<string, double>>();

                            //loop over all the metabolites for tissue and charge and non-ignore phenotypes
                            //in order to fill in the assisting variables
                            //these variables serve for significance symbols in the plot and for where to plce it in the plot
                            foreach (sampleForTissueAndCharge sftac in msMetaboliteLevels.List_SampleForTissueAndCharge.Where(x => x.Tissue == tissue && x.Charge == charge && !publicVariables.excludedPhenotypes.Contains(x.Phenotype)))
                            {
                                foreach (sampleForTissueAndCharge.parentMetabolite tdm in sftac.ListOfMetabolites.Where(x => x.mtbltDetails.In_customId == mtbl.In_customId))
                                {
                                    phenoMetabValPairs.Add(new Tuple<string, double>(sftac.Phenotype, tdm.mtbltVals.Imputed));
                                    if (pValPairs.Count == 0)
                                    {
                                        for (int i = 0; i < phenotypes.Count; i++)
                                        {
                                            for (int j = (i + 1); j < phenotypes.Count; j++)
                                            {
                                                pValPairs.Add(new Tuple<string, double>(phenotypes[i].First() + "v" + phenotypes[j].First(),
                                                    tdm.mtbltDetails.ListOfStats.PairwiseTestPvalue.First(x => x.group1 == phenotypes[i] && x.group2 == phenotypes[j]).pairValue));
                                            }
                                        }

                                        if (publicVariables.numberOfClasses != publicVariables.numberOfClassesValues.two)
                                        {
                                            pValPairs.Add(new Tuple<string, double>("", tdm.mtbltDetails.ListOfStats.MultiGroupPvalue));
                                        }
                                    }
                                }
                            }

                            //significance symbols matrix
                            rEngineInstance.engine.SetSymbol("signifSymbYlevel", returnSignificanceDataFrame(phenoMetabValPairs, pValPairs));

                            //sets the dataframe variable df in R
                            rEngineInstance.engine.SetSymbol("df", returnIEnurable(phenoMetabValPairs));

                            //do not plot anything to the console
                            //adds the boxplot in the list of plots
                            //stop not printing stuff in teh console
                            rEngineInstance.engine.Evaluate(@"pdf(NULL); 
                                                            accumulateBoxplots[[length(accumulateBoxplots) + 1]] <- 
                                                                boxplotTissueCharge(df, boxPlotTitleAndYlabel[1], boxPlotTitleAndYlabel[2], signifSymbYlevel[,1], signifSymbYlevel[,2], signifSymbYlevel[,3]);
                                                            dev.off();"); 
                        }
                        else //if the metabolite has not been detected for this combination of tissue and charge provide and empty plot
                        {
                            //empty plot: is defined in the initialization of rEngineInstance
                            rEngineInstance.engine.Evaluate("accumulateBoxplots[[length(accumulateBoxplots) + 1]] <- nullPlot(boxPlotTitleAndYlabel[1], boxPlotTitleAndYlabel[2])");
                        }
                    }
                }
                //do the plot
                rEngineInstance.engine.Evaluate(@"" + printBoxPlotGrid(msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Charge).Distinct().Count(), msMetaboliteLevels.List_SampleForTissueAndCharge.Select(x => x.Tissue).Distinct().Count()) +
                        @"p <- grid.text(""" + mtbl.In_customId + @"_" + mtbl.In_Name + @""", x=unit(130,""mm""), y=unit(225,""mm""), just=c(""left"", ""top""), gp = gpar(fontface=""bold"", fontsize=24, col=""blue""));
                    print(p);");
            }
            rEngineInstance.engine.Evaluate(@"dev.off()");

            //Re-enable Console printings
            Console.SetOut(stdOut);
        }

        /// <summary>
        /// text to be placed in the boxplot for significance symbols
        /// </summary>
        /// <param name="numOfPhenotypes"></param>
        /// <returns></returns>
        private static string geom_textLinesInGgplot2Function(int numOfPhenotypes)
        {
            int numOfLines = Convert.ToInt32(Math.Round(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(numOfPhenotypes) * Convert.ToDouble(Convert.ToDouble(numOfPhenotypes) - Convert.ToDouble(1))) / Convert.ToDouble(2)))) + 1;
            string s = @"";
            for (int i = 1; i <= numOfLines; i++)
            {
                s += @"geom_text(data=NULL, x=signifSymbXlevel[" + i + "], y=signifSymbYlevel[" + i + "], label=signifSymb[" + i + "], color='red') + ";
            }
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pvalue"></param>
        /// <returns>the significance symbol for a given p-value</returns>
        private static string significanceSymbol(double pvalue)
        {
            if (pvalue > 0.05 || double.IsNaN(pvalue))
            {
                return "ns";
            }
            else if (pvalue <= 0.05 && pvalue > 0.01)
            {
                return "*";
            }
            else if (pvalue <= 0.01 && pvalue > 0.001)
            {
                return "**";
            }
            else if (pvalue <= 0.001 && pvalue > 0.0001)
            {
                return "***";
            }
            else
            {
                return "****";
            }
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
        private static DataFrame returnSignificanceDataFrame(List<Tuple<string, double>> phenoMetabValPairs, List<Tuple<string, double>> pValPairs)
        {
            List<string> my_signSymb;
            if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two)
            {
                my_signSymb = pValPairs.Select(x => x.Item1 + "\n" + significanceSymbol(x.Item2)).ToList();
            }
            else
            {
                my_signSymb = pValPairs.GetRange(0, pValPairs.Count - 1).Select(x => x.Item1 + "\n" + significanceSymbol(x.Item2)).ToList();
                my_signSymb.Add(significanceSymbol(pValPairs.Last().Item2));
            }

            List<double> my_signYlevel = new List<double>() { phenoMetabValPairs.Select(x => x.Item2).OrderBy(x => x).ElementAt(phenoMetabValPairs.Count - 2) };
            List<double> my_signXlevel = new List<double>() { 1.5 };

            if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two)
            {
                return rEngineInstance.engine.CreateDataFrame(new IEnumerable[] { my_signSymb.ToArray(), my_signYlevel.ToArray(), my_signXlevel.ToArray() });
            }

            double mymin = -1; int mymin_pheno = -1, mymin_cnt = 1;
            my_signYlevel.Add(phenoMetabValPairs.Select(x => x.Item2).OrderBy(x => x).Last());
            my_signYlevel.Add(my_signYlevel.First());
            foreach (string pheno in phenoMetabValPairs.Select(x => x.Item1).Distinct().OrderBy(x => x))
            {
                if (mymin < phenoMetabValPairs.Where(x => x.Item1 == pheno).Select(x => x.Item2).Min())
                {
                    mymin = phenoMetabValPairs.Where(x => x.Item1 == pheno).Select(x => x.Item2).Min();
                    mymin_pheno = mymin_cnt;
                }
                mymin_cnt++;
            }
            my_signYlevel.Add(phenoMetabValPairs.Select(x => x.Item2).Min());
            my_signXlevel.AddRange(new List<double>() { 2, 2.5, mymin_pheno });

            return rEngineInstance.engine.CreateDataFrame(new IEnumerable[] { my_signSymb.ToArray(), my_signYlevel.ToArray(), my_signXlevel.ToArray() });
        }

        private static DataFrame returnIEnurable(List<Tuple<string, double>> phenoMetabValPairs)
        {
            //initialize the two lists that will be added in the IEnumerable
            List<string> my_phenotypes = new List<string>();
            List<double> my_values = new List<double>();

            //loop over the phenotypes
            for (int i = 0; i < phenoMetabValPairs.Count; i++)
            {
                //create a lists of phenotypes equal to the length of the corresponding imputed_pvalues
                my_phenotypes.Add(phenoMetabValPairs.ElementAt(i).Item1);
                //just add the numerical values
                my_values.Add(phenoMetabValPairs.ElementAt(i).Item2);
            }

            return rEngineInstance.engine.CreateDataFrame(new IEnumerable[] { my_phenotypes.ToArray(), my_values.ToArray() });
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
                s += @"accumulateBoxplots[[" + i + @"]], ";
            }
            s += @"ncol=" + numTissue + @", nrow=1, widths=c(";
            for (int i = 0; i < (numTissue - 1); i++)
            {
                s += @"" + myDevider + @"/10, ";
            }
            s += @"" + myDevider + @"/10))";
            return s;
        }
    }
}
