using System;
using System.Collections.Generic;
using System.Linq;

namespace MS_targeted
{
    public class sampleForTissueAndCharge
    {
        public string Id { get; set; }
        public string Phenotype { get; set; }
        public string Tissue { get; set; }
        public string Charge { get; set; }
        public List<numClinicalData> ListOfNumClinicalData; //the clinical data for this patient
        public List<parentMetabolite> ListOfMetabolites; //metabolite object, level (apriori normalized or non-normalized)

        public sampleForTissueAndCharge(List<string> _metabolitebVals, List<msMetabolite> _lom)
        {
            Id = _metabolitebVals.ElementAt(publicVariables.indexToStartFrom - 3);
            Tissue = _metabolitebVals.ElementAt(publicVariables.indexToStartFrom - 2);
            Charge = _metabolitebVals.ElementAt(publicVariables.indexToStartFrom - 1);
            Phenotype = (clinicalData.List_clinicalData.Any(x => x.Id == Id)) ? clinicalData.List_clinicalData.First(x => x.Id == Id).Phenotype : "U";

            ListOfNumClinicalData = new List<numClinicalData>();
            ListOfNumClinicalData.Add(new numClinicalData()
            {
                name = "sampleweight",
                value = ((publicVariables.prefixValues.mixed == publicVariables.prefix || publicVariables.prefixValues.gcms == publicVariables.prefix) 
                    ? clinicalData.List_clinicalData.First(x => x.Id == Id).SampleWeight_covariates.Where(x => x.tissue.ToLower() == Tissue.ToLower()).Average(x => x.weight)
                        : clinicalData.List_clinicalData.First(x => x.Id == Id).SampleWeight_covariates
                            .First(x => x.tissue.ToLower() == Tissue.ToLower() && x.charge.ToLower() == Charge.ToLower()).weight)
            });
            foreach (KeyValuePair<string, imputedValues> kvp_nv in clinicalData.List_clinicalData.First(x => x.Id == Id).Numerical_covariates)
            {
                ListOfNumClinicalData.Add(new numClinicalData()
                {
                    name = kvp_nv.Key,
                    value = kvp_nv.Value.Imputed
                });
            }

            ListOfMetabolites = new List<parentMetabolite>();
            foreach (msMetabolite _m in _lom)
            {
                if (string.IsNullOrEmpty(_metabolitebVals.ElementAt(_m.In_Index)) || string.IsNullOrWhiteSpace(_metabolitebVals.ElementAt(_m.In_Index)))
                {
                    ListOfMetabolites.Add(new parentMetabolite()
                    {
                        mtbltDetails = _m,
                        mtbltVals = new imputedValues()
                        {
                            Imputed = -1,
                            Non_imputed = -1
                        }
                    });
                }
                else
                {
                    ListOfMetabolites.Add(new parentMetabolite()
                    {
                        mtbltDetails = _m,
                        mtbltVals = new imputedValues()
                        {
                            Imputed = Convert.ToDouble(_metabolitebVals.ElementAt(_m.In_Index)),
                            Non_imputed = Convert.ToDouble(_metabolitebVals.ElementAt(_m.In_Index))
                        }
                    });
                }
            }
        }

        public class parentMetabolite
        {
            public msMetabolite mtbltDetails { get; set; }
            public imputedValues mtbltVals { get; set; }
        }

        public class numClinicalData
        {
            public string name { get; set; }
            public double value { get; set; }
        }
    }
}
