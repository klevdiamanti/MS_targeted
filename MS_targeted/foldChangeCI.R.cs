using System;
using System.Linq;
using RDotNet;

namespace MS_targeted
{
    public static class foldChangeCI
    {
        public static returnFCandCI calculateFoldChangeAndCI(double[] _numerator, double[] _denominator)
        {
            NumericVector numerator = rEngineInstance.engine.CreateNumericVector(_numerator);
            rEngineInstance.engine.SetSymbol("numerator", numerator);
            NumericVector denominator = rEngineInstance.engine.CreateNumericVector(_denominator);
            rEngineInstance.engine.SetSymbol("denominator", denominator);

            rEngineInstance.engine.Evaluate(@"fcres <- fold_change(numerator, denominator)");

            return new returnFCandCI()
            {
                fc = Math.Round(rEngineInstance.engine.Evaluate(@"fcres$fc").AsNumeric().First(), 5),
                lower = Math.Round(rEngineInstance.engine.Evaluate(@"fcres$lower").AsNumeric().First(), 5),
                upper = Math.Round(rEngineInstance.engine.Evaluate(@"fcres$upper").AsNumeric().First(), 5)
            };
        }
    }

    public class returnFCandCI
    {
        public double fc { get; set; }
        public double lower { get; set; }
        public double upper { get; set; }
    }
}

