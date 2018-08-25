# MS_targeted input files

**For detailed explanation on the output files please check [MS_targeted_output.md](MS_targeted_output.md)**

**Input file-formats and directories explained in this section**
- [input ms data file](MS_targeted_input.md#input-ms-data-file) [example](MS_targeted/sample_data/input_ms/gcms_adams_plasma.tsv
)
- [metadata file](MS_targeted_input.md#metadata-file) [example](MS_targeted/sample_data/metadata/gcms_fiehn_metadata.tsv
)
- [configuration file](MS_targeted_input.md#configuration-file) [example](MS_targeted/sample_data/conf/MS_targeted.conf
)
- [MS_targeted custom database file](MS_targeted_input.md#ms_targeted-custom-database-file) [example](MS_targeted/sample_data/db/20171204_metabolites_db.tsv.zip
)

## input ms data files
This input is a directory that contains various data files with the same prefix prior to the first underscore (_) of the file name and the same suffix (e.g. file1: lcms_serum.tsv; file2: lcms_urine.tsv; file3: lcms_saliva.tsv). The input ms data files contain the metabolites' details and intensities detected from mass-spectrometry and computational annotations. The first 29 rows of the file contain metadata for the detected metabolites and the next N rows contain the levels of the metaolites for the respective samples (N is the numer of samples). We will split this section into two sub-sections:

### metabolite metadata: rows 1 to 29
The first 4 columns of the first row contain: Type, ID, Tissue, Charge. The first 29 rows for these columns are to navigate the user. The columns 5 to M of the first row (M = number of detected metabolites) contain the names of the detected metabolites. The rest columns and rows are explained below:
*Note that options marked with '\*' are obligatory while the rest optional.*
- \*Type\*: type for rows 2-29 is always "Meta".
- \*ID\* for rows 2-29 contains the following with this exact order: Type, Formula, Mass, RT, m_ID, CAS_ID, ALT_CAS_ID, HMDB_ID, ALT_HMDB_ID, KEGG_ID, ALT_KEGG_ID, ChEBI_ID, ALT_ChEBI_ID, PUBCHEM_ID, ALT_PUBCHEM_ID, CHEMSPIDER_ID, ALT_CHEMSPIDER_IS, LIPIDMAPS_ID, ALT_LIPIDMAPS_ID, METLIN_ID, ALT_METLIN_ID, PROBLEMATIC, MSMS_CONFIRMED, IN_BLANK, MS_PROBLEMATIC, m_SUPERCLASS, m_CLASS, m_NAME_FIXED.
  - \*Type\*: should be always set to "Metabolite".
  - \*Formula\*: biochemical formula of the metabolite in the corresponding column (provided from computational annotation).
  - \*Mass\*: isotopic mass of the metabolite in the corresponding column (provided from computational annotation).
  - \*RT\*: retention time of the metabolite in the corresponding column (provided from computational annotation).
  - \*m_ID\*: custom ID of each metabolite in the corresponding column. This ID is provided by the user and is internally used to mark unique metabolites. The name should be alpharithemtic and contain at least one character [a-z, A-Z].
  - CAS_ID: Chemical Abstracts Service (CAS) registry number of the metabolite in the corresponding column.
  - ALT_CAS_ID: additional or alternative Chemical Abstracts Service (CAS) registry numbers of the metabolite in the corresponding column.
  - HMDB_ID: Human Metabolome Database [(HMDB)](http://www.hmdb.ca/) ID of the metabolite in the corresponding column.
  - ALT_HMDB_ID: additional or alternative Human Metabolome Database [(HMDB)](http://www.hmdb.ca/) IDs of the metabolite in the corresponding column.
  - KEGG_ID: Kyoto Encyclopedia of Genes and Genomes [(KEGG)-compound](https://www.kegg.jp/kegg/compound/) ID of the metabolite in the corresponding column.
  - ALT_KEGG_ID: additional or alternative Kyoto Encyclopedia of Genes and Genomes [(KEGG)-compound](https://www.kegg.jp/kegg/compound/) IDs of the metabolite in the corresponding column.
  - ChEBI_ID: Chemical Entities of Biological Interest [(ChEBI)](https://www.ebi.ac.uk/chebi/) ID of the metabolite in the corresponding column.
  - ALT_ChEBI_ID: additional or alternative Chemical Entities of Biological Interest [(ChEBI)](https://www.ebi.ac.uk/chebi/) IDs of the metabolite in the corresponding column.
  - PUBCHEM_ID: [PubChem](https://pubchem.ncbi.nlm.nih.gov/) ID of the metabolite in the corresponding column.
  - ALT_PUBCHEM_ID: additional or alternative [PubChem](https://pubchem.ncbi.nlm.nih.gov/) IDs of the metabolite in the corresponding column.
  - CHEMSPIDER_ID: [ChemSpider](http://www.chemspider.com/) ID of the metabolite in the corresponding column.
  - ALT_CHEMSPIDER_ID: additional or alternative [ChemSpider](http://www.chemspider.com/) IDs of the metabolite in the corresponding column.
  - LIPIDMAPS_ID: [LipidMaps](https://www.lipidmaps.org/) ID of the metabolite in the corresponding column.
  - ALT_LIPIDMAPS_ID: additional or alternative [LipidMaps](https://www.lipidmaps.org/) IDs of the metabolite in the corresponding column.
  - METLIN_ID: [Metlin](https://metlin.scripps.edu/landing_page.php?pgcontent=mainPage) ID of the metabolite in the corresponding column.
  - ALT_METLIN_ID: additional or alternative [Metlin](https://metlin.scripps.edu/landing_page.php?pgcontent=mainPage) IDs of the metabolite in the corresponding column.
  - \*PROBLEMATIC\*: a binary indicator (exclussively TRUE or FALSE) for metabolites that had computational issues.
  - \*MSMS_CONFIRMED\*: a binary indicator (exclussively TRUE or FALSE) for metabolites that were confirmed by MS-MS.
  - \*IN_BLANK\*: a binary indicator (exclussively TRUE or FALSE) for metabolites that were also present in the blank sample.
  - \*MS_PROBLEMATIC\*: a binary indicator (exclussively TRUE or FALSE) for metabolites that had experimental mass-spectrometry issues.
  - m_SUPERCLASS: a manually curated super-class or taxon where the corresponding metabolite belongs to.
  - m_CLASS: a manually curated class or taxon where the corresponding metabolite belongs to.
  - \*m_NAME_FIXED\*: a manually curated name of the corresponding metabolite.
- \*Tissue\*: tissue for rows 2-29 contains always the name of the corresponding tissue.
- \*Charge\*: charge for rows 2-29 contains always the name of the charge detection in the corresponding tissue. (e.g. positive or negative for LC-MS, or none for GCMS or MIXED).

### metabolite levels: rows 30 to N (N = number of samples)
The first 4 columns of the first row contain: Type, ID, Tissue, Charge. There is one row for each sample. The columns 5 to M (M = number of detected metabolites) contain the levels of the detected metabolites on the corresponding sample. *Note that options marked with '\*' are obligatory while the rest optional.*
- \*Type\*: type for these rows should always be "Sample".
- \*ID\*: sample ID.
- \*Tissue\*: tissue contains always the name of the corresponding tissue.
- \*Charge\*: charge contains always the name of the charge detection in the corresponding tissue. (e.g. positive or negative for LC-MS, or none for GCMS or MIXED).

## metadata file
The metadata file should contain all the available clnical data available for every sample. The first row should state the unique label of the metadata, while the second row the type of it, which is strinctly 'Categorical' or 'Numeric'. The metadata that the user selects in the configuration file are used for visual exploration, correlation analysis and regression analysis, while the rest are ignored.
  
## configuration file
A configuration file to set advanced options of the MS-Targeted pipeline. Lines that start with hash '#' are comments to navigate the user. Options that the user might need to set manually are:
- R (R_HOME directory and the .dll, .so or .dylib library file) in case there is some specific R configuration.
- CovarSampleWeight denotes the column of the sample weight covariate. For example if the header of the column that contains the sample weight is 'Serum' this option should be set to Serum#Serum_None in case charge is unknown or Serum#Serum_positive if it is positive. In case of multiple tissues it should be Serum#Serum_None,Urine#Urine_None.
- Please consult the comments on the printings in the configuration file. Some of these options add up a considerable amount of time to the pipeline.

## MS_targeted custom database file
This is an option that we are actively working in optimizing. For the time being we have compiled a tab-separated file from ChEBI and HMDB as a meta-database that contains a large set of known metabolites to which the metabolites detected in the current study can be mapped to. This file has to be decompressed prior to running MS_targeted and provided as input.
