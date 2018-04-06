using RDotNet;
using System;
using System.IO;

namespace MS_targeted
{
    public static class rEngineInstance
    {
        public static REngine engine;

        public static void initializeREngine(string R_HOME, string rDll)
        {
            //block (intercept) REngine from printing to the Console
            //we are just redirecting the output of it to some StringWriter
            var stdOut = Console.Out;
            Console.SetOut(new StringWriter());

            if (string.IsNullOrEmpty(R_HOME) || string.IsNullOrWhiteSpace(R_HOME))
            {
                REngine.SetEnvironmentVariables();
            }
            else
            {
                REngine.SetEnvironmentVariables(rHome: R_HOME);
            }

            if (string.IsNullOrEmpty(rDll) || string.IsNullOrWhiteSpace(rDll))
            {
                engine = REngine.GetInstance();
            }
            else
            {
                engine = REngine.GetInstance(dll: rDll);
            }

            StartupParameter myparameter = new StartupParameter()
            {
                Interactive = false,
                Quiet = true,
                Verbose = true,
                Slave = true
            };

            // REngine requires explicit initialization.
            // You can set some parameters.
            engine.Initialize(parameter: myparameter);
            prepareREndgine();

            //Re-enable Console printings
            Console.SetOut(stdOut);
        }

        private static void prepareREndgine()
        {
            //load libraries
            engine.Evaluate(@"library(lmPerm); library(ggplot2); library(grid); library(gridExtra); library(coin); library(Hmisc); library(RcmdrMisc);");

            //set seed
            engine.Evaluate("set.seed(2017)");

            //print nullplot function
            engine.Evaluate(
                @"nullPlot <- function(plotTitle, plotYlabel) {
                    p <- ggplot() + 
                        theme_bw(base_size=18) + 
                        ggtitle(plotTitle) +
                        ylab(plotYlabel) +
                        annotate(""text"", x=1, y=1, label=""NA"", size=10) +
                        theme(legend.position=""none"",
                            axis.title.x = element_blank(),
                            axis.text = element_blank(),
                            axis.ticks = element_blank());
                    return (p);
                };");
        }

        public static void disposeREngine()
        {
            // you should always dispose of the REngine properly.
            // After disposing of the engine, you cannot reinitialize nor reuse it
            engine.Dispose();
        }
    }
}
