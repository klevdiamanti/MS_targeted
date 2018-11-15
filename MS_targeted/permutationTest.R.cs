using System.Linq;
using RDotNet;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

namespace MS_targeted
{
    public static class permutationTest
    {
        private static IEnumerable[] dataFrameValues;
        public static double aovpPermutationTest(string[] columnNames)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            //create the data frame
            DataFrame df = rEngineInstance.engine.CreateDataFrame(dataFrameValues, columnNames: columnNames);
            rEngineInstance.engine.SetSymbol("df", df);

            //Exact permutation test intends to run a complete permutation test which is extremely time consuming
            //When there are more that 10 observations it will switch from Exact to Prob which approximates the permutation distribution by randomly exchanging pairs of Y elements
            //in case we want to use Exact the we should change the limit of Exact from 10 to something greater by using maxExact = X (where X>10)
            //nCycle performs  a  complete  random  permutation,  instead  of  pairwise  exchanges, every nCycle cycles
            //run permutation test and take the pvalue
            double aovp_pValue = rEngineInstance.engine.Evaluate(string.Format("summary(aovp(df[,'{0}'] ~ factor(df[,'{1}']), perm = \"{2}\", seqs = {3}, " +
                "center = {4}, projections = {5}, qr = {6}, maxIter = {7}, , nCycle = {8}))[[1]][[\"Pr(Prob)\"]][1]",
                columnNames[1],
                columnNames[0],
                "Prob",
                "TRUE",
                "TRUE",
                "TRUE",
                "TRUE",
                publicVariables.numberOfPermutations,
                (publicVariables.numberOfPermutations / 1000).ToString())).AsNumeric().First();

            rEngineInstance.engine.Evaluate("rm(df)");
            //Re-enable Console printings
            Console.SetOut(stdOut);

            return Math.Round(aovp_pValue, 5);
        }

        public static double ttestPermutationTest(string[] columnNames)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            //create the data frame
            DataFrame df = rEngineInstance.engine.CreateDataFrame(dataFrameValues, columnNames: columnNames);
            rEngineInstance.engine.SetSymbol("df", df);

            //This permutation T test is quite slow. It run 1000 permutations in approximately 1 second.
            //Run permutation test and take the pvalue
            double ttest_pValue = rEngineInstance.engine.Evaluate(string.Format("perm.t.test(as.numeric(df[,'{0}']) ~ as.factor(df[,'{1}']), paired = {2}, " +
                "alternative = \"{3}\", nperm = {4}, progress = {5})$p.value",
                columnNames[1],
                columnNames[0],
                "FALSE",
                "two.sided",
                publicVariables.numberOfPermutations,
                "FALSE")).AsNumeric().First();

            rEngineInstance.engine.Evaluate("rm(df)");
            //Re-enable Console printings
            Console.SetOut(stdOut);

            return Math.Round(ttest_pValue, 5);
        }

        public static double kruskalWallisPermutationTest(string[] columnNames)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            //create the data frame
            DataFrame df = rEngineInstance.engine.CreateDataFrame(dataFrameValues, columnNames: columnNames);
            rEngineInstance.engine.SetSymbol("df", df);

            //Approximative (Monte Carlo) multivariate Kruskal-Wallis test
            //For two samples (phenotypes), the Kruskal-Wallis test is equivalent to the W-M-W (Wilcoxon-Mann-Whitney) test
            rEngineInstance.engine.Evaluate(string.Format(@"kwres <- independence_test(df[,'{0}'] ~ factor(df[,'{1}']), teststat = ""quadratic"", 
                distribution = approximate(B = {2}), ytrafo = function(data) trafo(data, numeric_trafo = rank_trafo))",
                      columnNames[1],
                      columnNames[0],
                      publicVariables.numberOfPermutations.ToString()));


            rEngineInstance.engine.Evaluate("rm(df)");

            //Re-enable Console printings
            Console.SetOut(stdOut);

            return Math.Round(rEngineInstance.engine.Evaluate(@"kwres@distribution@pvalue(kwres@statistic@teststatistic)[1]").AsNumeric().First(), 5);
        }

        public static double wilcoxonMannWhitneyPermutationTest(string[] columnNames)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            //create the data frame
            DataFrame df = rEngineInstance.engine.CreateDataFrame(dataFrameValues, columnNames: columnNames);
            rEngineInstance.engine.SetSymbol("df", df);

            //independence_test(diffusion$pd ~ diffusion$age, teststat = "quadratic", distribution = approximate(B = 10000))

            //Approximative (Monte Carlo) multivariate Kruskal-Wallis test
            //For two samples (phenotypes), the Kruskal-Wallis test is equivalent to the W-M-W (Wilcoxon-Mann-Whitney) test
            rEngineInstance.engine.Evaluate(string.Format(@"wmwres <- independence_test(df[,'{0}'] ~ factor(df[,'{1}']), teststat = ""quadratic"", 
                distribution = approximate(B = {2}))",
                      columnNames[1],
                      columnNames[0],
                      publicVariables.numberOfPermutations.ToString()));


            rEngineInstance.engine.Evaluate("rm(df)");
            
            //Re-enable Console printings
            Console.SetOut(stdOut);

            return Math.Round(rEngineInstance.engine.Evaluate(@"wmwres@distribution@pvalue(wmwres@statistic@teststatistic)[1]").AsNumeric().First(), 5);
        }

        public static msMetabolite.stats.regressValues linearRegressionTest(string[] columnNames, string _typeof, IEnumerable[] ie_cofounders, List<string> cof_typeof)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            //create the cofounder data frame
            regrCovars rCovar = covarsDf(ie_cofounders, cof_typeof);
            if (cof_typeof.Count > 0)
            {
                DataFrame cofounders = rEngineInstance.engine.CreateDataFrame(rCovar.IEcofounders, columnNames: rCovar.cofounders.ToArray());
                rEngineInstance.engine.SetSymbol("cofounders", cofounders);
            }
            
            //create the data frame
            DataFrame df = rEngineInstance.engine.CreateDataFrame(dataFrameValues, columnNames: columnNames);
            rEngineInstance.engine.SetSymbol("df", df);

            //Exact permutation test intends to run a complete permutation test which is extremely time consuming
            //When there are more that 10 observations it will switch from Exact to Prob which approximates the permutation distribution by randomly exchanging pairs of Y elements
            //in case we want to use Exact the we should change the limit of Exact from 10 to something greater by using maxExact = X (where X>10)
            //nCycle performs  a  complete  random  permutation,  instead  of  pairwise  exchanges, every nCycle cycles
            //run permutation test and take the pvalue
            if (_typeof == "factor")
            {
                rEngineInstance.engine.Evaluate(string.Format(@"myres <- summary(lmPerm::lmp(as.numeric(df[,'{0}']) ~ as.factor(df[,'{1}']){2}, perm = ""{3}"", seqs = {4}, " +
                    @"center = {4}, projections = {4}, qr = {4}, maxIter = {5}, nCycle = {6}))",
                        columnNames[1],
                        columnNames[0],
                        rCovar.cof_string,
                        "Prob",
                        "TRUE",
                        publicVariables.numberOfPermutations,
                        (publicVariables.numberOfPermutations / 1000).ToString()));
            }
            else if (_typeof == "number")
            {
                rEngineInstance.engine.Evaluate(string.Format(@"myres <- summary(lmPerm::lmp(as.numeric(df[,'{0}']) ~ as.numeric(df[,'{1}']){2}, perm = ""{3}"", seqs = {4}, " +
                    @"center = {4}, projections = {4}, qr = {4}, maxIter = {5}, nCycle = {6}))",
                        columnNames[1],
                        columnNames[0],
                        rCovar.cof_string,
                        "Prob",
                        "TRUE",
                        publicVariables.numberOfPermutations,
                        (publicVariables.numberOfPermutations / 1000).ToString()));
            }
            else
            {
                outputToLog.WriteErrorLine("Regression failed");
            }

            //rEngineInstance.engine.Evaluate("print(myres)");
            rEngineInstance.engine.Evaluate("rm(df, cofounders)");

            //Re-enable Console printings
            Console.SetOut(stdOut);

            return new msMetabolite.stats.regressValues()
            {
                clinical_data_name = columnNames.First(),
                regrPvalue = Math.Round(rEngineInstance.engine.Evaluate("myres$coefficients[2, 3]").AsNumeric().First(), 5),
                regrAdjRsquare = Math.Round(rEngineInstance.engine.Evaluate("myres$adj.r.squared").AsNumeric().First(), 5)
            };
        }

        private static regrCovars covarsDf(IEnumerable[] ie_cofounders, List<string> _typeof)
        {
            int cnt = 0;
            List<int> indToRemove = new List<int>();
            foreach (var iec in ie_cofounders)
            {
                if (iec.Cast<object>().ToList().Distinct().Count() == 1)
                {
                    indToRemove.Add(cnt);
                }
                cnt++;
            }

            IEnumerable[] r_cofounders = new IEnumerable[_typeof.Count - indToRemove.Count];
            int currCnt = 0, addCnt = 0;
            List<string> r_typeOf = new List<string>(), r_cofCovars = new List<string>();
            foreach (var iec in ie_cofounders)
            {
                if (!indToRemove.Contains(currCnt))
                {
                    r_cofounders.SetValue(iec, addCnt);
                    r_typeOf.Add(_typeof.ElementAt(currCnt));
                    r_cofCovars.Add(publicVariables.cofCovars.ElementAt(currCnt));
                    addCnt++;
                }
                currCnt++;
            }

            string s = @"";
            for (int i = 0; i < r_typeOf.Count; i++)
            {
                if (r_typeOf[i] == "factor")
                {
                    s += string.Format(@" + as.factor(cofounders[,'{0}'])", r_cofCovars[i]);
                }
                else if (r_typeOf[i] == "number")
                {
                    s += string.Format(@" + as.numeric(cofounders[,'{0}'])", r_cofCovars[i]);
                }
                else
                {
                    outputToLog.WriteErrorLine("Regression failed");
                }
            }

            return (new regrCovars()
            {
                cofounders = r_cofCovars,
                cof_string = s,
                IEcofounders = r_cofounders,
                _typeOf = r_typeOf
            });
        }

        /// <summary>
        /// returns an IEnumerable of arrays in order to be used as a data-frame for the R code for the permutation test.
        /// the two input lists have the same length
        /// the IEnumbrable contains exactly two arrays. the first one contains the phenotypes and the second one the numerical values.
        /// the IEnumerable should be of the following format:
        /// Phenotype1  Value1
        /// Phenotype1  Value2
        /// Phenotype1  Value3
        /// Phenotype2  Value4
        /// Phenotype2  Value5
        /// </summary>
        /// <param name="phenotypes">a list of the phenotypes for which the ANOVA test will run</param>
        /// <param name="imputed_values">a list of arrays(double) that contain the values for the metabolite</param>
        /// <returns>an IEnumerable of arrays</returns>
        public static void returnIEnurable(List<string> phenotypes, List<double[]> numerical_values)
        {
            //initialize the two lists that will be added in the IEnumerable
            List<string> my_phenotypes = new List<string>();
            List<double> my_values = new List<double>();

            //loop over the phenotypes
            for (int i = 0; i < phenotypes.Count; i++)
            {
                //create a lists of phenotypes equal to the length of the corresponding imputed_pvalues
                my_phenotypes.AddRange(Enumerable.Repeat(phenotypes.ElementAt(i), numerical_values.ElementAt(i).Length));
                //just add the numerical values
                my_values.AddRange(numerical_values.ElementAt(i));
            }

            dataFrameValues = new IEnumerable[] { my_phenotypes.ToArray(), my_values.ToArray() };
        }

        /// <summary>
        /// returns an IEnumerable of arrays in order to be used as a data-frame for the R code for the permutation test.
        /// the two input lists have the same length
        /// the IEnumbrable contains exactly two arrays. the first one contains the phenotypes and the second one the numerical values.
        /// the IEnumerable should be of the following format:
        /// Phenotype1  Value1
        /// Phenotype1  Value2
        /// Phenotype1  Value3
        /// Phenotype2  Value4
        /// Phenotype2  Value5
        /// </summary>
        /// <param name="covariate_values">a list of arrays(double) that contain the values for the covariate</param>
        /// <param name="metabolite_values">a list of arrays(double) that contain the values for the metabolite</param>
        /// <returns>an IEnumerable of arrays</returns>
        public static void returnIEnurableNumeric(List<double[]> covariate_values, List<double[]> metabolite_values)
        {
            //initialize the two lists that will be added in the IEnumerable
            List<double> my_covariate_values = new List<double>();
            List<double> my_metabolite_values = new List<double>();

            //loop over the phenotypes
            for (int i = 0; i < metabolite_values.Count; i++)
            {
                //create a lists of phenotypes equal to the length of the corresponding imputed_pvalues
                my_covariate_values.AddRange(covariate_values.ElementAt(i));
                //just add the numerical values
                my_metabolite_values.AddRange(metabolite_values.ElementAt(i));
            }

            dataFrameValues = new IEnumerable[] { my_covariate_values.ToArray(), my_metabolite_values.ToArray() };
        }

        /// <summary>
        /// returns an IEnumerable of arrays in order to be used as a data-frame for the R code for the permutation test.
        /// the two input lists have the same length
        /// the IEnumbrable contains exactly two arrays. the first one contains the phenotypes and the second one the numerical values.
        /// the IEnumerable should be of the following format:
        /// Phenotype1  Value1
        /// Phenotype1  Value2
        /// Phenotype1  Value3
        /// Phenotype2  Value4
        /// Phenotype2  Value5
        /// </summary>
        /// <param name="covariate_values">a list of arrays(double) that contain the values for the covariate</param>
        /// <param name="metabolite_values">a list of arrays(double) that contain the values for the metabolite</param>
        /// <returns>an IEnumerable of arrays</returns>
        public static void returnIEnurableCategoric(List<string[]> covariate_values, List<double[]> metabolite_values)
        {
            //initialize the two lists that will be added in the IEnumerable
            List<string> my_covariate_values = new List<string>();
            List<double> my_metabolite_values = new List<double>();

            //loop over the phenotypes
            for (int i = 0; i < metabolite_values.Count; i++)
            {
                //create a lists of phenotypes equal to the length of the corresponding imputed_pvalues
                my_covariate_values.AddRange(covariate_values.ElementAt(i));
                //just add the numerical values
                my_metabolite_values.AddRange(metabolite_values.ElementAt(i));
            }

            dataFrameValues = new IEnumerable[] { my_covariate_values.ToArray(), my_metabolite_values.ToArray() };
        }

        private class regrCovars
        {
            public IEnumerable[] IEcofounders { get; set; }
            public string cof_string { get; set; }
            public List<string> _typeOf { get; set; }
            public List<string> cofounders { get; set; }
        }
    }
}
