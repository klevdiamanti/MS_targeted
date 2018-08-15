# MS_targeted
MS_targeted is an open-source command-line pipeline for statistical analysis of mass spectrometry metabolomics data. The pipeline is written in C# and runs in all platforms. For Windows you can run it through cmd.exe, in OSX and in Linux through the terminal.
MS_targeted can handle multiple tissues and charges simultaneously.

**For any questions or issues please use the Issues in github or contact Klev Diamanti.**

## Notes
- It is required that you have installed the latest version of mono and R 3.4.X.
- You will find the ready-to-run executable under [MS_targeted/exec/](MS_targeted/exec/MS_targeted.exe).
- The R packages lmPerm, coin, gridExtra, ggplot2, Hmisc, RcmdrMisc and RVAideMemoire are required to be pre-installed.
- MS_targeted will not run with R 3.5.X due to incompatibilities of the RDotNet package with the latest version of R.
- You will need to decompress the [database file](MS_targeted/sample_data/db/20171204_metabolites_db.tsv.zip) prior to running the pipeline.
- Detailed explanation of the input files and options is provided in [MS_targeted_input.md](MS_targeted_input.md).
- Detailed explanation of the output files is provided in [MS_targeted_output.md](MS_targeted_output.md).

## Run MS_tageted
```
[mono] MS_targeted.exe input_ms_dir clinical_data_file combined_db_file conf_file output_dir
```
Prior to running MS_targeted you will need to structure the metadata (covariates) and mass spectrometry input data according to the [sample files](MS_targeted/sample_data/) provided. The sample data have been taken from the metabolomics workbench study ST000383 (Fiehn et al., 2010).
#### input_ms_dir
Directory where the input mass spectrometry data are stored in plain text comma-separated or tab-separated files. All the input files should have the same prefic prior to the first underscore (_) and the extension. The various database id for each metabolite you can set them in the input files. We suggest that you have one file for each tissue. Please check [MS_targeted/sample_data/input_ms/](MS_targeted/sample_data/input_ms/) for an example.
Note that it is required that you have one custom id for every metabolite, that is defined in the row AZm_id. These id's should contain at least one letter.
#### clinical_data_file
File where the metadata or covariates for the samples are stored. This is a plain text file preferably tab-delimited. In the first row the unique names for each covariate should be mentioned. In the next row the type of the covariate should be mentioned (exclusively categorical or numeric). Please check [MS_targeted/sample_data/metadata/](MS_targeted/sample_data/metadata/) for an example.
#### combined_db_file
This is a meta-data file that contains various database id's for thousands of metabolites. The file should be the output from the repository metabolomicsDB.exe. For a start you might use the tab-separated file under [MS_targeted/sample_data/db/](MS_targeted/sample_data/db/). Please decompress the file before using it.
#### conf_file
This file configures various parameters for MS_targeted. The sample file under [MS_targeted/sample_data/conf/](MS_targeted/sample_data/conf/) contains enough comments and details.
#### output_dir
An existing or not existing output directory  where all the output files will be written. Please note that if the file exists it will be overwritten.
## Citation
Klev Diamanti, Marco Cavalli, Gang Pan, Maria João Pereira, Chanchal Kumar, Stanko Skrtic, Manfred Grabherr, Ulf Risérus, Jan W Eriksson, Jan Komorowski and Claes Wadelius (2018). "Metabolic landscape of five tissues suggests new molecular defects in type-2 diabetes". Submitted.
