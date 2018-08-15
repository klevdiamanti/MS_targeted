# MS_targeted input files

**For detailed explanation on the output files please check [MS_targeted_output.md](MS_targeted_output.md)**

**Input file-formats and directories explained in this section**
- [input ms data file](MS_targeted_input.md#input-ms-data-file)
- [metadata file](MS_targeted_input.md#metadata-file)
- [configuration file](MS_targeted_input.md#configuration-file)
- [MS_targeted custom database file](MS_targeted_input.md#ms_targeted-custom-database-file)

## input ms data file
This is an optionally created directory named metab_to_covar_correlations, and is controlled from the setting *PrintCorrelationsMetabolitesToCovariates* in the configuration file. The files contain information about the correlation analysis between all the metabolites provided in the input data and the selected numeric covariates from the metadata. The covariates are selected by providing their names to the *CovarCorrelate* option in the configuration file. In this case the pipeline runs a Spearman correlation between the metabolites and the selected covariates.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- correlation statistics for the current metabolite
  - Depending on the number of unique numeric covariates chosen for the pipeline to run, there will be triplets of correlation r value, p-value and adjusted p-value for the current metabolite. In case of one covariate there will be only one triplet named covariate_c, covariate_p and covariate_pAdjust. In every case the 'covariate' is the name of the covariate as provided in the configuration file. The 'c' implies the Spearman correlation r value, the 'p' the Spearman correlation p-value, and the 'pAdjust' implies the Benjamini_hochberg adjusted p-value. In case of more than one selected numeric covariates, there will be consecutive triplets of the aforementioned triplets: covariate1_c, covariate1_p and covariate1_pAdjust; covariate2_c, covariate2_p and covariate2_pAdjust.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  
## metadata file
This is an optionally created directory named metab_to_covar_correlations, and is controlled from the setting *PrintCorrelationsMetabolitesToCovariates* in the configuration file. The files contain information about the correlation analysis between all the metabolites provided in the input data and the selected numeric covariates from the metadata. The covariates are selected by providing their names to the *CovarCorrelate* option in the configuration file. In this case the pipeline runs a Spearman correlation between the metabolites and the selected covariates.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- correlation statistics for the current metabolite
  - Depending on the number of unique numeric covariates chosen for the pipeline to run, there will be triplets of correlation r value, p-value and adjusted p-value for the current metabolite. In case of one covariate there will be only one triplet named covariate_c, covariate_p and covariate_pAdjust. In every case the 'covariate' is the name of the covariate as provided in the configuration file. The 'c' implies the Spearman correlation r value, the 'p' the Spearman correlation p-value, and the 'pAdjust' implies the Benjamini_hochberg adjusted p-value. In case of more than one selected numeric covariates, there will be consecutive triplets of the aforementioned triplets: covariate1_c, covariate1_p and covariate1_pAdjust; covariate2_c, covariate2_p and covariate2_pAdjust.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  
## configuration file
This is an optionally created directory named metab_to_covar_correlations, and is controlled from the setting *PrintCorrelationsMetabolitesToCovariates* in the configuration file. The files contain information about the correlation analysis between all the metabolites provided in the input data and the selected numeric covariates from the metadata. The covariates are selected by providing their names to the *CovarCorrelate* option in the configuration file. In this case the pipeline runs a Spearman correlation between the metabolites and the selected covariates.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- correlation statistics for the current metabolite
  - Depending on the number of unique numeric covariates chosen for the pipeline to run, there will be triplets of correlation r value, p-value and adjusted p-value for the current metabolite. In case of one covariate there will be only one triplet named covariate_c, covariate_p and covariate_pAdjust. In every case the 'covariate' is the name of the covariate as provided in the configuration file. The 'c' implies the Spearman correlation r value, the 'p' the Spearman correlation p-value, and the 'pAdjust' implies the Benjamini_hochberg adjusted p-value. In case of more than one selected numeric covariates, there will be consecutive triplets of the aforementioned triplets: covariate1_c, covariate1_p and covariate1_pAdjust; covariate2_c, covariate2_p and covariate2_pAdjust.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  
## MS_targeted custom database file
This is an optionally created directory named metab_to_covar_correlations, and is controlled from the setting *PrintCorrelationsMetabolitesToCovariates* in the configuration file. The files contain information about the correlation analysis between all the metabolites provided in the input data and the selected numeric covariates from the metadata. The covariates are selected by providing their names to the *CovarCorrelate* option in the configuration file. In this case the pipeline runs a Spearman correlation between the metabolites and the selected covariates.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- correlation statistics for the current metabolite
  - Depending on the number of unique numeric covariates chosen for the pipeline to run, there will be triplets of correlation r value, p-value and adjusted p-value for the current metabolite. In case of one covariate there will be only one triplet named covariate_c, covariate_p and covariate_pAdjust. In every case the 'covariate' is the name of the covariate as provided in the configuration file. The 'c' implies the Spearman correlation r value, the 'p' the Spearman correlation p-value, and the 'pAdjust' implies the Benjamini_hochberg adjusted p-value. In case of more than one selected numeric covariates, there will be consecutive triplets of the aforementioned triplets: covariate1_c, covariate1_p and covariate1_pAdjust; covariate2_c, covariate2_p and covariate2_pAdjust.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
