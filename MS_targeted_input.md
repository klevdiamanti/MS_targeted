# MS_targeted input files

**For detailed explanation on the output files please check [MS_targeted_output.md](MS_targeted_output.md)**

**Input file-formats and directories explained in this section**
- [input ms data file](MS_targeted_input.md#input-ms-data-file)
- [metadata file](MS_targeted_input.md#metadata-file)
- [configuration file](MS_targeted_input.md#configuration-file)
- [MS_targeted custom database file](MS_targeted_input.md#ms_targeted-custom-database-file)

## input ms data files
This input is a directory that contains various data files with the same prefix prior to the first underscore (_) of the file name and the same suffix (e.g. file1: lcms_serum.tsv; file2: lcms_urine.tsv; file3: lcms_saliva.tsv). The input ms data files contain the metabolites' details and intensities detected from mass-spectrometry and computational annotations. The first 29 rows of the file contain metadata for the detected metabolites and the next N rows contain the levels of the metaolites for the respective samples (N is the numer of samples). We will split this section into two sub-sections:

### metabolite metadata: rows 1 to 29
The first row contains: Type, ID, Tissue, Charge. The first 29 rows for these columns are to navigate the user.
- Type for rows 2-29 is always "Meta".
- ID for rows 2-29 contains the following with this exact order: Type, Formula, Mass, RT, m_ID, CAS_ID, ALT_CAS_ID, HMDB_ID, ALT_HMDB_ID, KEGG_ID, ALT_KEGG_ID, ChEBI_ID, ALT_ChEBI_ID, PUBCHEM_ID, ALT_PUBCHEM_ID, CHEMSPIDER_ID, ALT_CHEMSPIDER_IS, LIPIDMAPS_ID, ALT_LIPIDMAPS_ID, METLIN_ID, ALT_METLIN_ID, PROBLEMATIC, MSMS_CONFIRMED, IN_BLANK, MS_PROBLEMATIC, m_SUPERCLASS, m_CLASS, m_NAME_FIXED.
  - Formula: biochemical formula of the metabolite in the corresponding column (provided from computational annotation).
  - Mass: mass of the metabolite in the corresponding column (provided from computational annotation).
  - RT: retention time of the metabolite in the corresponding column (provided from computational annotation).
  - m_ID: custom ID of each metabolite in the corresponding column. This ID is provided by the user and is internally used to mark unique metabolites. The name should be alpharithemtic and contain at least one character [a-z, A-Z].
  - CAS_ID: Chemical Abstracts Service (CAS) registry number of the metabolite in the corresponding column.
  - ALT_CAS_ID: additional or alternative Chemical Abstracts Service (CAS) registry number of the metabolite in the corresponding column.
  - HMDB_ID:
  - ALT_HMDB_ID:
  - KEGG_ID:
  - ALT_KEGG_ID:
  - ChEBI_ID:
  - ALT_ChEBI_ID:
  - PUBCHEM_ID:
  - ALT_PUBCHEM_ID:
  - CHEMSPIDER_ID:
  - ALT_CHEMSPIDER_IS:
  - LIPIDMAPS_ID:
  - ALT_LIPIDMAPS_ID:
  - METLIN_ID:
  - ALT_METLIN_ID:
  - PROBLEMATIC:
  - MSMS_CONFIRMED:
  - IN_BLANK:
  - MS_PROBLEMATIC:
  - m_SUPERCLASS:
  - m_CLASS:
  - m_NAME_FIXED:
- Tissue for rows 2-29 contains always the name of the corresponding tissue.
- Charge for rows 2-29 contains always the name of the charge detection in the corresponding tissue. (e.g. positive or negative for LC-MS, or none for GCMS or MIXED).


The columns 5 to M of the first row (M = number of detected metabolites) contain the names of the detected metabolites.

### metabolite levels: rows 30 to N (N = number of samples)
  
## metadata file

  
## configuration file

  
## MS_targeted custom database file

