# MS_targeted
MS_targeted is an open-source command-line pipeline for statistical analysis of mass spectrometry metabolomics data. The pipeline is implemented in C# and R, and runs in all platforms. In Windows you can run it on cmd.exe, while in OSX and Linux on the terminal with the cross-platform open-source .NET framework [mono](https://www.mono-project.com/).
MS_targeted can handle multiple tissues and charges simultaneously.

**For any questions or issues please use the Issues in github or contact Klev Diamanti.**

## Notes
- You will find the pre-compiled executables under [MS_targeted/exec/](MS_targeted/exec/MS_targeted.exe).
- It is required that you have installed the latest version of mono and R 3.4.X. **Note:** MS_targeted will not run with R 3.5.X due to incompatibilities of the RDotNet package with the latest version of R.
- The R packages [lmPerm](https://cran.r-project.org/web/packages/lmPerm/index.html), [coin](https://cran.r-project.org/web/packages/coin/index.html), [gridExtra](https://cran.r-project.org/web/packages/gridExtra/index.html), [ggplot2](https://cran.r-project.org/web/packages/ggplot2/index.html), [Hmisc](https://cran.r-project.org/web/packages/Hmisc/index.html), [RcmdrMisc](https://cran.r-project.org/web/packages/RcmdrMisc/index.html), [RVAideMemoire](https://cran.r-project.org/web/packages/RVAideMemoire/index.html) and [lm.beta](https://cran.r-project.org/web/packages/lm.beta/index.html) are required to be pre-installed.
- You will need to decompress the [database file](MS_targeted/sample_data/db/20171204_metabolites_db.tsv.zip) prior to running the pipeline.
- Detailed explanation of the input files and options is provided in [MS_targeted_input.md](MS_targeted_input.md).
- Detailed explanation of the output files is provided in [MS_targeted_output.md](MS_targeted_output.md).

## Run MS_tageted
```
[mono] MS_targeted.exe input_ms_dir clinical_data_file combined_db_file conf_file output_dir
```
Prior to running MS_targeted you will need to structure the metadata (covariates) and mass spectrometry input data according to the [sample files](MS_targeted/sample_data/) from metabolomics workbench study [ST000383](http://www.metabolomicsworkbench.org/data/DRCCMetadata.php?Mode=Study&StudyID=ST000383) [(Fiehn et al., 2010)](https://www.ncbi.nlm.nih.gov/pubmed/21170321).
#### input_ms_dir
Directory where the input mass spectrometry data are stored in plain text comma- or tab-separated files. All the input files should have the same <u>prefix</u> prior to the first underscore (_) and the <u>extension</u>. The various database id's for each metabolite should be set in the input files. We recommend you have one file for each tissue. Please check [MS_targeted/sample_data/input_ms/](MS_targeted/sample_data/input_ms/) for an example.
Note that one custom id for every metabolite is required, that is defined in the row m_id. These id's should contain at least one letter and numbers.
#### clinical_data_file
File where the metadata or covariates for the samples are stored. This is a plain text comma- or tab-delimited file. The first row should contain unique names for each covariate. The next row should contain the type of the covariate <u>(exclusively categorical or numeric)</u>. Please check [MS_targeted/sample_data/metadata/](MS_targeted/sample_data/metadata/) for an example.
#### combined_db_file
This is a meta-data file that contains various database id's for thousands of metabolites. The file should be the output from the repository [metabolomicsDB](https://github.com/klevdiamanti/metabolomicsDB/). For a start you might use the tab-separated file under [MS_targeted/sample_data/db/](MS_targeted/sample_data/db/). Please decompress the file before using it.
#### conf_file
This file configures various parameters for MS_targeted. The sample file under [MS_targeted/sample_data/conf/](MS_targeted/sample_data/conf/) contains comments and details.
#### output_dir
An existing or not existing output directory where all the output files will be written. Please note that if the directory exists it will be overwritten.
## Citation
Klev Diamanti, Marco Cavalli, Gang Pan, Maria João Pereira, Chanchal Kumar, Stanko Skrtic, Manfred Grabherr, Ulf Risérus, Jan W Eriksson, Jan Komorowski and Claes Wadelius (2018). "Intra- and inter-individual metabolic profiling highlights carnitine and lysophosphatidylcholine pathways as key molecular defects in type 2 diabetes". Accepted - Scientific Reports.
