using System;
using System.Collections.Generic;
using System.Linq;

namespace MS_targeted
{
    public class clinicalDataFS
    {
        public string Id { get; set; }
        public string Phenotype { get; set; }
        public List<sampleWeight> SampleWeight_covariates;
        public Dictionary<string, string> Categorical_covariates;
        public Dictionary<string, imputedValues> Numerical_covariates;
        public Dictionary<string, string> Ignored_covariates;

        public class sampleWeight
        {
            public string tissue { get; set; }
            public string charge { get; set; }
            public imputedValues weight { get; set; }
        }

        public void setClinicalDataFromFile(int _id_index, int _phenotype_index, Dictionary<string, int> _sampleweight_covariates, Dictionary<string, int> _categorical_covariates,
            Dictionary<string, int> _numerical_covariates, Dictionary<string, int> _ignored_covariates, string _line)
        {
            Id = _line.Split(publicVariables.breakCharInFile).ElementAt(_id_index);
            Phenotype = _line.Split(publicVariables.breakCharInFile).ElementAt(_phenotype_index);
            SampleWeight_covariates = new List<sampleWeight>();
            foreach (KeyValuePair<string, int> kvp_swci in _sampleweight_covariates)
            {
                SampleWeight_covariates.Add(new sampleWeight()
                {
                    tissue = kvp_swci.Key.Split('_').First(),
                    charge = kvp_swci.Key.Split('_').Last(),
                    weight = new imputedValues()
                    {
                        Non_imputed = (string.IsNullOrEmpty(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_swci.Value))) ? -1 : Convert.ToDouble(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_swci.Value)),
                        Imputed = (string.IsNullOrEmpty(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_swci.Value))) ? -1 : Convert.ToDouble(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_swci.Value))
                    }
                });
            }
            Categorical_covariates = new Dictionary<string, string>();
            foreach (KeyValuePair<string, int> kvp_cci in _categorical_covariates)
            {
                Categorical_covariates.Add(kvp_cci.Key, (string.IsNullOrEmpty(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_cci.Value))) ? "U" : _line.Split(publicVariables.breakCharInFile).ElementAt(kvp_cci.Value));
            }
            Numerical_covariates = new Dictionary<string, imputedValues>();
            foreach (KeyValuePair<string, int> kvp_nci in _numerical_covariates)
            {
                Numerical_covariates.Add(kvp_nci.Key, (string.IsNullOrEmpty(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_nci.Value))) ?
                        new imputedValues() { Imputed = -1, Non_imputed = -1 } :
                        new imputedValues() { Imputed = Convert.ToDouble(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_nci.Value)), Non_imputed = Convert.ToDouble(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_nci.Value)) });
            }
            Ignored_covariates = new Dictionary<string, string>();
            foreach (KeyValuePair<string, int> kvp_ici in _ignored_covariates)
            {
                Ignored_covariates.Add(kvp_ici.Key, (string.IsNullOrEmpty(_line.Split(publicVariables.breakCharInFile).ElementAt(kvp_ici.Value))) ? "U" : _line.Split(publicVariables.breakCharInFile).ElementAt(kvp_ici.Value));
            }
        }

        public void setClinicalDataFromVariable(string _id, string _phenotype, List<sampleWeight> _losw, Dictionary<string, string> _ccv,
            Dictionary<string,imputedValues> _ncv, Dictionary<string, string> _icv)
        {
            Id = _id;
            Phenotype = _phenotype;
            SampleWeight_covariates = _losw;
            Categorical_covariates = _ccv;
            Numerical_covariates = _ncv;
            Ignored_covariates = _icv;
        }
    }
}
