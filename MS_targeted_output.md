
# MS_targeted output files

## log file
MS_targeted prints a log file that contains the configuration of the pipeline and some of the key steps. The log file is located under the output_dir and its name is set in the connf_file.

## metabolite details
This file is optionally printed, and is controlled from the setting *PrintMetaboliteDetails* in the configuration file.

It is a tab- or comma-separated file that contains a collection of various database identifiers and pathways for the collection of unique metabolites provided from the use as input.

## boxplots
This is an optionally created directory named boxplots, and is controlled from the setting *PrintBoxplots* in the configuration file.

This is a pdf file that illustrates in boxplots the levels of each metabolite for a categorical covariates from the metadata file. Every metabolite is printed in a separate page. The covairiate is selected in the configuration file in the setting *CovarCorrelate*. Only the selected categorical covariates are used for boxplots.

For every combination of tissue, charge and covariate one .pdf file is created.

## scatterplots
This is an optionally created directory named scatterplots, and is controlled from the setting *PrintScatterplots* in the configuration file.

This is a pdf file that illustrates in scatterplots the correlation of each metabolite for a numerical covariates from the metadata file. Every metabolite is printed in a separate page. The covairiate is selected in the configuration file in the setting *CovarCorrelate*. Only the selected numerical covariates are used for scatterplots.

For every combination of tissue, charge and covariate one .pdf file is created.

## pathways
This is an optionally created directory named pathway_significance, and is controlled from the setting *PrintPathwaysForMetabolites* in the configuration file.

For every tissue a tab- or comm-aseparated file is created. This file contains:
- basic information about the pathway
  - SuperPathway: name of the super-pathway the encapsulates the current pathway 
  - SubPathwayID: KEGG or SMPDB id of current pathway
  - SubPathway: KEGG or SMPDB name of current pathway
  - PathwayCount: number of metabolites that are part of the current pathway
- basic information the metabolite in the pathway
  - BiochemicalName: biochemichal (common) name	of the current metabolite
  - Formula: biochemichal formula of the current metabolite
  - Platform: the platform in which the current metabolite was detected (GCMS, LCMS or MIXED)
  - Charge: the charge mode of the platform in which the current metabolite was detected (positive or negative for LCMS, or none for GCMS or MIXED)
- statistics for the current metabolite
  - Depending on the number of unique decisions chosen for the pipeline to run on there will be pairs of fold-changes and p-values for the current metabolite. In case of two distinct dicisions there will be only one pair named AtoBfc and AtoBpv. A is the initial letter of one of the distinct decisions and B the initial letter of the other one. The to implies the transition from one decision to the other one, and the final fc or pv imply fold-change or p-value, respectively. In case of three decisions, three pair-wise comparisons will be performed, hence there will be: AtoBfc and AtoBpv, AtoCfc and AtoCpv, and BtoCfc and BtoCpv.
- public database and local identifiers for the current metabolite
  - CAS: CAS id of the current metabolite
  - HMDB: Human Metabolome Database id of the current metabolite
  - KEGG: Kyoto Encyclopedia of Genes and Genomes id of the current metabolite
  - ChEBI: Chemical Entities of Biological Interest id of the current metabolite
  - PubChem: PubChem id of the current metabolite
  - CustID: Custom id given by the user in the input files of metabolite intensities
  - Genes: Genes involved in the pathway with the current SubPathwayID that the current is involved in
