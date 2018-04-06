using System;
using System.Collections.Generic;
using System.Linq;

namespace MS_targeted
{
    public class clinicalDataForSample
    {
        public string Id { get; set; }
        public string Phenotype { get; set; }
        public List<sampleWeight> TissueChargeWeightProblematic;
        public Dictionary<string, string> OtherPhenotypes;
        public Dictionary<string, imputedValues> NormalizationVars;

        public void setClinicalDataForSampleFromFile(string line)
        {
			if (publicVariables.donorSource == publicVariables.donorSourceValues.exodiab)
			{
				clinicalDataForSamplesFromFileExodiab(line);
			}
			else if (publicVariables.donorSource == publicVariables.donorSourceValues.exodiab)
			{
				clinicalDataForSamplesFromFilePatients(line);
			}
			else
			{
				outputToLog.WriteErrorLine("Could not read from metadata file. The donor-source was set incorrectly!");
			}
        }

		public void setClinicalDataForSampleFromVariables(string _id, string _phenotype, List<sampleWeight> _tcwp,
														 Dictionary<string, string> _oph, Dictionary<string, imputedValues> _nv)
		{
			Id = _id;
			Phenotype = _phenotype;
			TissueChargeWeightProblematic = _tcwp;
			OtherPhenotypes = _oph;
			NormalizationVars = _nv;
		}

        private void clinicalDataForSamplesFromFileExodiab(string line)
        {
            Id = line.Split(publicVariables.breakCharInFile).ElementAt(3);
            if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.three)
            {
                Phenotype = line.Split(publicVariables.breakCharInFile).ElementAt(16);
            }
            else if (publicVariables.numberOfClasses == publicVariables.numberOfClassesValues.two)
            {
                Phenotype = line.Split(publicVariables.breakCharInFile).ElementAt(17);
            }
			OtherPhenotypes = new Dictionary<string, string>()
				{
					{ "Gender", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(5))) ? "U" : line.Split(publicVariables.breakCharInFile).ElementAt(5) },
					{ "BmiDiscrete", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(9))) ? "U" : line.Split(publicVariables.breakCharInFile).ElementAt(9) },
					{ "HbA1cDiscrete", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(11))) ? "U" : line.Split(publicVariables.breakCharInFile).ElementAt(11) },
					{ "BloodGroup", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(12))) ? "U" : line.Split(publicVariables.breakCharInFile).ElementAt(12) },
					{ "PhenotypeThreeClasses", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(16))) ? "U" : line.Split(publicVariables.breakCharInFile).ElementAt(16) },
					{ "PhenotypeTwoClasses", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(17))) ? "U" : line.Split(publicVariables.breakCharInFile).ElementAt(17) }
				};

			NormalizationVars = new Dictionary<string, imputedValues>()
				{
					{ "Age", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(6))) ?
						new imputedValues(){ Imputed = -1, Non_imputed = -1 } :
						new imputedValues(){ Imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(6)), Non_imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(6)) } },
					{ "YearOfBirth", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(7))) ?
						new imputedValues(){ Imputed = -1, Non_imputed = -1 } :
						new imputedValues(){ Imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(7)), Non_imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(7)) } },
					{ "Bmi", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(8))) ?
						new imputedValues(){ Imputed = -1, Non_imputed = -1 } :
						new imputedValues(){ Imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(8)), Non_imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(8)) } },
					{ "HbA1c", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(10))) ?
						new imputedValues(){ Imputed = -1, Non_imputed = -1 } :
						new imputedValues(){ Imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(10)), Non_imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(10)) } },
					{ "StimulatoryIndex", (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(15))) ?
						new imputedValues(){ Imputed = -1, Non_imputed = -1 } :
						new imputedValues(){ Imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(15)), Non_imputed = Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(15)) } },
				};

            TissueChargeWeightProblematic = new List<sampleWeight>();
			//for (int i = publicVariables.exodiabVariables.sampleMetadataStartingPoint; i < (publicVariables.exodiabVariables.sampleMetadataStartingPoint + (publicVariables.exodiabVariables.tissueChargeList.Count * 2));
			//	i += publicVariables.exodiabVariables.tissueChargeStep)
            for (int i = publicVariables.exodiabVariables.sampleMetadataStartingPoint; i < (publicVariables.exodiabVariables.columnsOfMetadataLimit - 1);
                i += publicVariables.exodiabVariables.tissueChargeStep)
            {
                TissueChargeWeightProblematic.Add(new sampleWeight()
                {
                    tissue = publicVariables.exodiabVariables.tissueChargeList.ElementAt((i - publicVariables.exodiabVariables.sampleMetadataStartingPoint) / publicVariables.exodiabVariables.tissueChargeStep).Item1,
                    charge = publicVariables.exodiabVariables.tissueChargeList.ElementAt((i - publicVariables.exodiabVariables.sampleMetadataStartingPoint) / publicVariables.exodiabVariables.tissueChargeStep).Item2,
                    weight = (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(i))) ? -1 : Convert.ToDouble(line.Split(publicVariables.breakCharInFile).ElementAt(i)),
                    isProblematic = (string.IsNullOrEmpty(line.Split(publicVariables.breakCharInFile).ElementAt(i + 1))) ? false : Convert.ToBoolean(line.Split(publicVariables.breakCharInFile).ElementAt(i + 1))
                });
			}
        }

        private void clinicalDataForSamplesFromFilePatients(string line)
        {
            
        }

        public class sampleWeight
        {
            public string tissue { get; set; }
            public string charge { get; set; }
            public double weight { get; set; }
            public bool isProblematic { get; set; }
        }
    }
}
