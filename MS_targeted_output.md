
# MS_targeted output files

## log file
MS_targeted prints a log file that contains the configuration of the pipeline and some of the key steps. The log file is located under the output_dir and its name is set in the configuration file.

## metabolite details
This file is optionally printed, and is controlled from the setting *PrintMetaboliteDetails* in the configuration file.

It is a tab- or comma-separated file that contains a collection of various database identifiers and pathways for the collection of unique metabolites provided from the input.

## boxplots
This is an optionally created directory named boxplots, and is controlled from the setting *PrintBoxplots* in the configuration file.

This is a pdf file that illustrates in boxplots the levels of each metabolite for a categorical covariates from the metadata file. Every metabolite is printed in a separate page. The covairiate is selected in the configuration file in the setting *CovarCorrelate*. Only the selected categorical covariates are used for boxplots.

For every combination of tissue, charge and covariate a separate pdf file is created.

## scatterplots
This is an optionally created directory named scatterplots, and is controlled from the setting *PrintScatterplots* in the configuration file.

This is a pdf file that illustrates in scatterplots the correlation of each metabolite for a numerical covariates from the metadata file. Every metabolite is printed in a separate page. The covairiate is selected in the configuration file in the setting *CovarCorrelate*. Only the selected numerical covariates are used for scatterplots.

For every combination of tissue, charge and covariate a separate pdf file is created.

## pathways
This is an optionally created directory named pathway_significance, and is controlled from the setting *PrintPathwaysForMetabolites* in the configuration file. The files contain information about pathways enriched for metabolites from the input data.

For every tissue a tab- or comma-separated file is created. This file contains in every row a pathway (current pathway) and a metabolite (current metabolite):
- basic information about the current pathway
  - SuperPathway: name of the super-pathway the encapsulates the current pathway 
  - SubPathwayID: KEGG or SMPDB id of current pathway
  - SubPathway: KEGG or SMPDB name of current pathway
  - PathwayCount: number of metabolites from the analysis that are members of the current pathway
- basic information about the current metabolite
  - BiochemicalName: biochemichal (common) name	of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- statistics for the current metabolite
  - Depending on the number of unique decisions chosen for the pipeline to run, there will be pairs of fold-change and p-value for the current metabolite. In case of two distinct dicisions there will be only one pair named AtoBfc and AtoBpv. In every case the 'A' is the initial letter of one of the distinct decisions and the 'B' the initial letter of the other one. The 'to' implies the transition from one decision to the other one.  The 'fc' or 'pv' imply fold-change or p-value, respectively. In case of three decisions, three pair-wise comparisons will be performed, hence there will be: AtoBfc and AtoBpv; AtoCfc and AtoCpv; BtoCfc and BtoCpv.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  - Genes: Genes assoiated with the current SubPathwayID pathway that the current metabolite is involved in

## metabolite statistics
This is an optionally created directory named metabolite_significance, and is controlled from the setting *PrintMetaboliteStatistics* in the configuration file. The files contain information about the differential analysis for metabolites from the input data on the selected decision/phenotype.

For every tissue a tab- or comma-separated file is created. This file contains in every row a metabolite (current metabolite):
- basic information about the current metabolite
  - BiochemicalName: biochemichal (common) name	of the current metabolite
  - Formula: biochemical formula of the current metabolite
  - Kingdom: the broadest HMDB class/taxa of metabolites where the current metabolite belongs to
  - SuperClass: the broader HMDB class/taxa of metabolites where the current metabolite belongs to
  - Class: the narrower HMDB class/taxa of metabolites where the current metabolite belongs to
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- statistics for the current metabolite
  - Depending on the number of unique decisions chosen for the pipeline to run, there will be triplets of fold-change, confidence interval and p-value for the current metabolite. In case of two distinct dicisions there will be only one triplet named AtoBfc, AtoBci and AtoBpv. In every case the 'A' is the initial letter of one of the distinct decisions and the 'B' the initial letter of the other one. The 'to' implies the transition from one decision to the other one.  The 'fc', 'ci' or 'pv' imply fold-change, confidence interval or p-value, respectively. In case of three decisions, three pair-wise comparisons will be performed, hence there will be: AtoBfc, AtoBci and AtoBpv; AtoCfc, AtoCci and AtoCpv; BtoCfc, BtoCci and BtoCpv.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: HMDB id of the current metabolite
  - KEGG: KEGG id of the current metabolite
  - ChEBI: ChEBI id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  - Genes: Genes assoiated with the current SubPathwayID pathway that the current metabolite is involved in
