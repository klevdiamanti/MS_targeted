using RDotNet;
using System;
using System.IO;

namespace MS_targeted
{
    public static class rEngineInstance
    {
        public static REngine engine;

        public static void initializeREngine(string R_HOME, string R_DLL, string R_packages)
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

            StartupParameter myparameter = new StartupParameter()
            {
                Interactive = false,
                Quiet = true,
                Verbose = false                
            };

            if (string.IsNullOrEmpty(R_DLL) || string.IsNullOrWhiteSpace(R_DLL))
            {
                engine = REngine.GetInstance(null, true, myparameter, null);
            }
            else
            {
                engine = REngine.GetInstance(R_DLL, true, myparameter, null);
            }

            // REngine requires explicit initialization.
            // You can set some parameters.
            prepareREndgine(R_packages);

            //Re-enable Console printings
            Console.SetOut(stdOut);
        }

        private static void prepareREndgine(string R_packages)
        {
            if (!string.IsNullOrEmpty(R_packages) && !string.IsNullOrWhiteSpace(R_packages))
            {
                engine.Evaluate(".libPaths(c('" + R_packages + "', .libPaths()))");
            }
            
            //load libraries
            engine.Evaluate(@"library(lmPerm); library(ggplot2); library(grid); library(gridExtra); library(coin); library(Hmisc); library(RcmdrMisc); library();");

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

            //calculate fold change
            engine.Evaluate(
                @"fold_change <- function(x, y, confidence.level=90, var.equal=F) {
                    fc.interval(length(x), mean(x), var(x), length(y), mean(y), var(y), confidence.level, var.equal)
                };");

            //calculate fc.interval
            engine.Evaluate(
                @"fc.interval <- function(x.n, x.mu, x.var, y.n, y.mu, y.var, confidence.level=90, var.equal=F) {
                    mu <- x.mu - y.mu
                    if (var.equal) {
                        nu <- x.n + y.n - 2
                        se <- sqrt(((x.n-1)*x.var + (y.n-1)*y.var) / nu) * sqrt(1/x.n + 1/y.n)
                    } else {
                        nu <- (x.var/x.n + y.var/y.n)^2 / (x.var^2/x.n^2/(x.n-1) + y.var^2/y.n^2/(y.n-1))
                        se <- sqrt(x.var/x.n + y.var/y.n)
                    }
                    t <- -qt((1-confidence.level/100)/2, df=nu)
                    if (mu >= 0) {
                        mu.lower <- max(0, mu - t*se)
                        mu.upper <- mu + t*se
                    } else {
                        mu.lower <- min(0, mu + t*se)
                        mu.upper <- mu - t*se
                    }
                    data.frame(fc=mu, df=nu, lower=mu.lower, upper=mu.upper)
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
