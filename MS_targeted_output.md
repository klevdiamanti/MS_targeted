
# MS_targeted output files

**For detailed explanation on the input files and examples please check [MS_targeted_input.md](MS_targeted_input.md)**

**Files used in this section:**
- [input ms data file](MS_targeted/sample_data/input_ms/gcms_adams_plasma.tsv)
- [metadata file](MS_targeted/sample_data/metadata/gcms_fiehn_metadata.tsv)
- [configuration file](MS_targeted/sample_data/conf/MS_targeted.conf)
- [MS_targeted custom database file](MS_targeted/sample_data/db/20171204_metabolites_db.tsv.zip)

## log file
MS_targeted prints a log file that contains the configuration of the pipeline and some of the key steps. The log file is located under the 'output_dir' and its name is set in the configuration file.

## metabolite details
This file is optionally printed as 'metabolites.tsv' under 'output_dir', and is controlled from the setting *PrintMetaboliteDetails* in the configuration file. It is a tab- or comma-separated file that contains a collection of various database identifiers and pathways for the collection of unique metabolites provided from the input.

## boxplots
This is an optionally created directory named 'boxplots', and is controlled from the setting *PrintBoxplots* in the configuration file. This is a pdf file that illustrates in boxplots the levels of each metabolite for a categorical covariates from the metadata file. Every metabolite is printed in a separate page. The covariate is selected in the configuration file in the setting *CovarCorrelate*. Only the selected categorical covariates are used for boxplots. For every combination of tissue, charge and covariate a separate pdf file is created.

## scatterplots
This is an optionally created directory named 'scatterplot', and is controlled from the setting *PrintScatterplots* in the configuration file. This is a pdf file that illustrates in scatterplots the correlation of each metabolite for a numerical covariates from the metadata file. Every metabolite is printed in a separate page. The covariate is selected in the configuration file in the setting *CovarCorrelate*. Only the selected numerical covariates are used for scatterplots. For every combination of tissue, charge and covariate a separate pdf file is created.

## pathways
This is an optionally created directory named 'pathway_significance', and is controlled from the setting *PrintPathwaysForMetabolites* in the configuration file. The files contain information about pathways enriched for metabolites from the input data.

For every tissue a tab- or comma-separated file is created. This file contains in every row a pathway (current pathway) and a metabolite (current metabolite):
- basic information about the current pathway
  - SuperPathway: name of the super-pathway the encapsulates the current pathway 
  - SubPathwayID: KEGG or SMPDB id of current pathway
  - SubPathway: KEGG or SMPDB name of current pathway
  - PathwayCount: number of metabolites from the analysis that are members of the current pathway
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name	of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- statistics for the current metabolite
  - Depending on the number of unique decisions chosen for the pipeline to run, there will be pairs of fold-change and p-value for the current metabolite. In case of two distinct decisions there will be only one pair named AtoBfc and AtoBpv. In every case the 'A' is the initial letter of one of the distinct decisions and the 'B' the initial letter of the other one. The 'to' implies the transition from one decision to the other one.  The 'fc' or 'pv' imply fold-change or p-value, respectively. In case of three decisions, three pair-wise comparisons will be performed, hence there will be: AtoBfc and AtoBpv; AtoCfc and AtoCpv; BtoCfc and BtoCpv.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  - Genes: Genes associated with the current SubPathwayID pathway that the current metabolite is involved in

## metabolite statistics
This is an optionally created directory named 'metabolite_significance', and is controlled from the setting *PrintMetaboliteStatistics* in the configuration file. The files contain information about the differential analysis for metabolites from the input data on the selected decision/phenotype in the configuration file. In case of two decision classes the pipeline runs a non-parametric Wilcoxon-Mann Whitney test for the metabolites. In case of more than two decision classes it runs the equivalent non-parametric test fur multiple decisions, Kruskal-Wallis test.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name	of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Kingdom: the broadest HMDB class/taxa of metabolites where the current metabolite belongs to
  - SuperClass: the broader HMDB class/taxa of metabolites where the current metabolite belongs to
  - Class: the narrower HMDB class/taxa of metabolites where the current metabolite belongs to
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- statistics for the current metabolite
  - Depending on the number of unique decisions chosen for the pipeline to run, there will be triplets of fold-change, confidence interval and p-value for the current metabolite. In case of two distinct decisions there will be only one triplet named AtoBfc, AtoBci and AtoBpv. In every case the 'A' is the initial letter of one of the distinct decisions and the 'B' the initial letter of the other one. The 'to' implies the transition from one decision to the other one.  The 'fc', 'ci' or 'pv' imply fold-change, confidence interval or p-value, respectively. In case of three decisions, three pair-wise comparisons will be performed, hence there will be: AtoBfc, AtoBci and AtoBpv; AtoCfc, AtoCci and AtoCpv; BtoCfc, BtoCci and BtoCpv.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  - Genes: Genes associated with the current SubPathwayID pathway that the current metabolite is involved in
  
## metabolite regression models
This is an optionally created directory named 'metabolite_regression', and is controlled from the setting *PrintRegressionStatistics* in the configuration file. The files contain information about the differential analysis for metabolites from the input data on the selected decision/phenotypes in the configuration file. It additionally contains information statistics for regression models of various metabolites with the chosen covariates. The regression models are calculated from permutation statistics using the R package lmPerm.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name	of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Kingdom: the broadest HMDB class/taxa of metabolites where the current metabolite belongs to
  - SuperClass: the broader HMDB class/taxa of metabolites where the current metabolite belongs to
  - Class: the narrower HMDB class/taxa of metabolites where the current metabolite belongs to
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- statistics of the differential analysis for the current metabolite
  - Depending on the number of unique decisions chosen for the pipeline to run, there will be triplets of fold-change, confidence interval and p-value for the current metabolite. In case of two distinct decisions there will be only one triplet named AtoBfc, AtoBci and AtoBpv. In every case the 'A' is the initial letter of one of the distinct decisions and the 'B' the initial letter of the other one. The 'to' implies the transition from one decision to the other one.  The 'fc', 'ci' or 'pv' imply fold-change, confidence interval or p-value, respectively. In case of three decisions, three pair-wise comparisons will be performed, hence there will be: AtoBfc, AtoBci and AtoBpv; AtoCfc, AtoCci and AtoCpv; BtoCfc, BtoCci and BtoCpv.
- statistics of the regression analysis for the current metabolite
  - For each chosen covariate from the configuration file there is a pair of values printed for the regression model to the current metabolite. The names of columns are 'covariate_pv' and 'covariate_r2adjust' that represent the permutation statistic regression p-value and the R<sup>2</sup> regression statistic. In case of one such covariate there will be only one pair of 'covariate_pv' and 'covariate_r2adjust'. In case of multiple ones there will be a corresponding number of pairs.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  - Genes: Genes associated with the current SubPathwayID pathway that the current metabolite is involved in
  
## significance of ratios of metabolites
This is an optionally created directory named metabolite_ratios, and is controlled from the setting *PrintRatiosOfMetabolites* in the configuration file. For every tissue a tab- or comma-separated file is created. The p-value calculation was performed after considering the ratio every metabolite pair per tissue and charge. Once all the ratios are calculated we run a non-parametric permutation test to calculate a p-value of the ratios. The goal is to calculate the deficiency of the enzyme converting one metabolite to another one in the biochemical pathways (Weckmann et al., 2018 Scientific Reports).
- In case of 2 decisions/phenotypes:
  - Every file contains in the first 2x2 entries the names of the compared decisions. The rest of the top row contains the biochemical names of the metabolites and the custom ids' of the metabolites provided by the user in the configuration file are provided in the second row from the top. The same information in located in the leftmost and second leftmost columns. Within the matrix there are the p-values of the statistical analysis mentioned above. The metabolites in the top row are the ones used as nominator when the ratio is calculated.
- In case of >2 decisions/phenotypes:
  - The table described above is printed multiple times for each pair of decisions below the initial one.

## correlation of metabolites to covariates
This is an optionally created directory named metab_to_covar_correlations, and is controlled from the setting *PrintCorrelationsMetabolitesToCovariates* in the configuration file. The files contain information about the correlation analysis between all the metabolites provided in the input data and the selected numeric covariates from the metadata. The covariates are selected by providing their names to the *CovarCorrelate* option in the configuration file. In this case the pipeline runs a Spearman correlation between the metabolites and the selected covariates.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemical (common) name	of the current metabolite
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
  
## correlation of metabolites to metabolites
This is an optionally created directory named metab_to_metab_correlations, and is controlled from the setting *PrintCorrelationsMetabolitesToMetabolites* in the configuration file. The files contain information about the correlation analysis among all the metabolites provided in the input data. In this case the pipeline runs a Spearman correlation among all pairs of metabolites.

For every tissue two tab- or comma-separated file are created. One named \*corrValue\* contains the Spearman correlation r values for every pair of metabolites. The other one named \*pValueAdjust\* contains the Spearman correlation Benjamini-Hochberg adjusted p-values for every pair of metabolites. This file contains in every row and in every row the same sequence of a metabolites. The top row and leftmost column contain the biochemical (common) names of each metabolite. The second row from the top and the second leftmost row contain the custom id given by the user in the input files of metabolite intensities. The rest of the file is a NxN matrix (where N is the number of unique input metabolites) filled with the respective r values or BH adjusted p-values.

## machine learning datasets
This is an optionally created directory named machine_learning_datasets, and is controlled from the setting *PrintMachineLearningDatasets* in the configuration file. The files contain pre-compiled data that can be plugged-in to any machine learning algorithm in your preferred tool (e.g. R, Weka). The files is tab- or comma-separated and consist of a header line that contains the ID of the sample, the biochemical (common) names of the metabolites and the decision.

## output for MoDentify
This is an optionally created directory named MoDentify (Do et al., 2018 Bioinformatics), and is controlled from the setting *PrintOutputForMoDentify* in the configuration file. The files contain pre-compiled set of data files that can be plugged-in to the recently developed R package MoDentify that examines whether metabolites from various tissues are correlated to the decision/phenotype individually or as members of modules. MoDentify also examines intra- or inter-tissue correlations of metabolites using Gaussian graphical models. The pre-compiled tab- or comma-separated set of data files consists of:
1. MoDentify_AZm_annotations.tsv
  
   This is a tab- or comma-separated file that contains various metadata for the metabolites from the provided input files. The file is designed in such a way so that it represents metabolites from various tissues. In multiple columns we use the tissue name follow by two consecutive colons '::'. This is done in order to introduce uniqueness for the combination of tissues and metabolites. Keep in mind potential metabolite introduced by not removing duplicate metabolites from the same tissue and charge mode. The following columns are present:
   - mID: custom id given by the user in the input files of metabolite intensities 
   - tmID: tissue name followed by '::' and the custom id given by the user in the input files of metabolite intensities 
   - HmdbID: HMDB id of the current metabolite
   - HmdbSecondaryID: list of HMDB ids' separated by the '|' column
   - mName: biochemical (common) name of the metabolite as provided by the user in the input files of metabolite intensities
   - HmdbName: HMDB biochemical (common) name of the metabolite
   - mHmdbName: manually curated biochemical (common) name of the metabolite
   - tmHmdbName: tissue name followed by '::' and the manually curated biochemical (common) name of the metabolite
   - mSuperClass: custom super class/taxa defined by the user in the input files of metabolite intensities
   - tmSuperClass: tissue name followed by '::' and the custom super class/taxa defined by the user in the input files of metabolite intensities
   - mClass: custom class/taxa defined by the user in the input files of metabolite intensities
   - tmClass: tissue name followed by '::' and the custom class/taxa defined by the user in the input files of metabolite intensities
   - HmdbDirectParent: direct parent of the metabolite as defined by HMDB
   - tHmdbDirectParent: tissue name followed by '::' and the direct parent of the metabolite as defined by HMDB
   - HmdbKingdom: metabolite kingdom as defined by HMDB
   - tHmdbKingdom: tissue name followed by '::' and the metabolite kingdom as defined by HMDB
   - HmdbSuperClass: super class/taxa of the metabolite as defined by HMDB
   - tHmdbSuperClass: tissue name followed by '::' and the super class/taxa of the metabolite as defined by HMDB
   - mHmdbSuperClass: super class/taxa of the metabolite as manually curated according to HMDB
   - tmHmdbSuperClass: tissue name followed by '::' and the super class/taxa of the metabolite as manually curated according to HMDB
   - HmdbClass: class/taxa of the metabolite as defined by HMDB
   - tHmdbClass: tissue name followed by '::' and the class/taxa of the metabolite as defined by HMDB
   - mHmdbClass: class/taxa of the metabolite as manually curated according to HMDB
   - tmHmdbClass: tissue name followed by '::' and the class/taxa of the metabolite as manually curated according to HMDB
   - mPlatform: platform where the metabolite was detected (LCMS, GCMS or MIXED)
   - mCharge: charge mode in which the metabolite was detected (positive or negative for LCMS; none for GCMS and MIXED)
2. MoDentify_AZm_data.tsv

   A matrix that contains the individual metabolite values for each patient in each tissue is the rows. The top row contains a combination of the tissue name followed by '::' and the manually curated biochemical (common) name of the metabolite. The second row from the top contains a combination of the tissue name followed by '::' and the custom id given by the user in the input files of metabolite intensities. The leftmost column contains the decision/phenotype chosen by the user in the configuration file. The next column contains the sample ids. The rest of the matrix simply contains the metabolite values given by the user in the input files of metabolite intensities.
   
   Note that there might not be unique combinations of tissue and biochemical name, and tissue and metabolite custom id. In such cases we recommend you modify the names or ids in order to include them in the future analysis, or remove the corresponding columns to exclude them from the analysis.
3. MoDentify_AZm_phenotypes.tsv

   A tab- or comma-separated files that contains the metadata/covariate values for each sample including the decision/phenotype.
