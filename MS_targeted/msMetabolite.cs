using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using metabolomicsDB;

namespace MS_targeted
{
    public class msMetabolite : metabolite
    {
        public string In_Name { get; set; }
        public string In_Type { get; set; }
        public string In_Formula { get; set; }
        public double In_Mass { get; set; }
        public double In_Rt { get; set; }
        public string In_customId { get; set; }
        public string In_Cas_id { get; set; }
        public List<string> In_add_Cas_id { get; set; }
        public string In_Hmdb_id { get; set; }
        public List<string> In_add_Hmdb_id { get; set; }
        public string In_Kegg_id { get; set; }
        public List<string> In_add_Kegg_id { get; set; }
        public string In_Chebi_id { get; set; }
        public List<string> In_add_Chebi_id { get; set; }
        public string In_Pubchem_id { get; set; }
        public List<string> In_add_Pubchem_id { get; set; }
        public string In_Chemspider_id { get; set; }
        public List<string> In_add_Chemspider_id { get; set; }
        public string In_Lipidmaps_id { get; set; }
        public List<string> In_add_Lipidmaps_id { get; set; }
        public string In_Metlin_id { get; set; }
        public List<string> In_add_Metlin_id { get; set; }
        public bool In_isProblematic { get; set; }
        public bool In_msMsConfirmed { get; set; }
        public bool In_inBlank { get; set; }
        public bool In_msProblematic { get; set; }
        public string In_AZmSuperClass { get; set; }
        public string In_AZmClass { get; set; }
        public string In_AZmNameFixed { get; set; }
        public int In_Index { get; set; }

        public stats ListOfStats { get; set; }

        public void getFromMetaboliteDB()
        {
            //here the idea is as follows
            //first search with primary hmdb, cas, kegg, chebi, pubchem, chemspider, lipidmaps and metlin in the all metabolites list on primary hmdb, cas, kegg, chebi, pubchem, chemspider, lipidmaps and metlin ids respectively
            //if nothing found search with primary hmdb, cas, kegg and chebi in the all metabolites list on additional (secodnary) hmdb, cas, kegg and chebi ids respectively
            //if nothing found search with additional (secondary) hmdb, cas, kegg, chebi, pubchem, chemspider, lipidmaps and metlin in the all metabolites list on primary and additional (secodnary) hmdb, cas, kegg, chebi, pubchem, chemspider, lipidmaps and metlin ids respectively

            if (!string.IsNullOrEmpty(In_Hmdb_id) && !string.IsNullOrWhiteSpace(In_Hmdb_id) && metabolites.List_metabolites.Any(x => x.Hmdb_accession == In_Hmdb_id))
            {
                search_hmdb_in_hmdb_id();
            }
            else if (!string.IsNullOrEmpty(In_Cas_id) && !string.IsNullOrWhiteSpace(In_Cas_id) && metabolites.List_metabolites.Any(x => x.Cas_registry_number == In_Cas_id))
            {
                search_cas_in_cas_id();
            }
            else if (!string.IsNullOrEmpty(In_Kegg_id) && !string.IsNullOrWhiteSpace(In_Kegg_id) && metabolites.List_metabolites.Any(x => x.Kegg_id == In_Kegg_id))
            {
                search_kegg_in_kegg_id();
            }
            else if (!string.IsNullOrEmpty(In_Chebi_id) && !string.IsNullOrWhiteSpace(In_Chebi_id) && metabolites.List_metabolites.Any(x => x.Chebi_id == In_Chebi_id))
            {
                search_chebi_in_chebi_id();
            }
            else if (!string.IsNullOrEmpty(In_Pubchem_id) && !string.IsNullOrWhiteSpace(In_Pubchem_id) && metabolites.List_metabolites.Any(x => x.Pubchem_compound_id == In_Pubchem_id))
            {
                search_pubchem_in_pubchem_id();
            }
            else if (!string.IsNullOrEmpty(In_Chemspider_id) && !string.IsNullOrWhiteSpace(In_Chemspider_id) && metabolites.List_metabolites.Any(x => x.Chemspider_id == In_Chemspider_id))
            {
                search_chemspider_in_chemspider_id();
            }
            else if (!string.IsNullOrEmpty(In_Lipidmaps_id) && !string.IsNullOrWhiteSpace(In_Lipidmaps_id) && metabolites.List_metabolites.Any(x => x.Lipidmaps_id == In_Lipidmaps_id))
            {
                search_lipidmaps_in_lipidmaps_id();
            }
            else if (!string.IsNullOrEmpty(In_Metlin_id) && !string.IsNullOrWhiteSpace(In_Metlin_id) && metabolites.List_metabolites.Any(x => x.Metlin_id == In_Metlin_id))
            {
                search_metlin_in_metlin_id();
            }
            else if (!string.IsNullOrEmpty(In_Hmdb_id) && !string.IsNullOrWhiteSpace(In_Hmdb_id) && metabolites.List_metabolites.SelectMany(x => x.Hmdb_secondary_accessions).Any(x => x == In_Hmdb_id))
            {
                search_hmdb_in_additional_hmdb_id();
            }
            else if (!string.IsNullOrEmpty(In_Cas_id) && !string.IsNullOrWhiteSpace(In_Cas_id) && metabolites.List_metabolites.SelectMany(x => x.Cts_cas).Any(x => x == In_Cas_id))
            {
                search_cas_in_additional_cas_id();
            }
            else if (!string.IsNullOrEmpty(In_Kegg_id) && !string.IsNullOrWhiteSpace(In_Kegg_id) && metabolites.List_metabolites.SelectMany(x => x.Cts_kegg).Any(x => x == In_Kegg_id))
            {
                search_kegg_in_additional_kegg_id();
            }
            else if (!string.IsNullOrEmpty(In_Chebi_id) && !string.IsNullOrWhiteSpace(In_Chebi_id) && metabolites.List_metabolites.SelectMany(x => x.Cts_chebi).Any(x => x == In_Chebi_id))
            {
                search_chebi_in_additional_chebi_id();
            }
            else if (In_add_Hmdb_id.Count > 0 &&
                (metabolites.List_metabolites.Select(x => x.Hmdb_accession).Intersect(In_add_Hmdb_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any() ||
                metabolites.List_metabolites.SelectMany(x => x.Hmdb_secondary_accessions).Intersect(In_add_Hmdb_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any()))
            {
                search_additional_hmdb_in_hmdb_and_additional_hmdb_id();
            }
            else if (In_add_Cas_id.Count > 0 &&
                (metabolites.List_metabolites.Select(x => x.Cas_registry_number).Intersect(In_add_Cas_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any() ||
                metabolites.List_metabolites.SelectMany(x => x.Cts_cas).Intersect(In_add_Cas_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any()))
            {
                search_additional_cas_in_cas_and_additional_cas_id();
            }
            else if (In_add_Kegg_id.Count > 0 &&
                (metabolites.List_metabolites.Select(x => x.Kegg_id).Intersect(In_add_Kegg_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any() ||
                metabolites.List_metabolites.SelectMany(x => x.Cts_kegg).Intersect(In_add_Kegg_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any()))
            {
                search_additional_kegg_in_kegg_and_additional_kegg_id();
            }
            else if (In_add_Chebi_id.Count > 0 &&
                (metabolites.List_metabolites.Select(x => x.Chebi_id).Intersect(In_add_Chebi_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any() ||
                metabolites.List_metabolites.SelectMany(x => x.Cts_chebi).Intersect(In_add_Chebi_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any()))
            {
                search_additional_chebi_in_chebi_and_additional_chebi_id();
            }
            else if (In_add_Pubchem_id.Count > 0 &&
                metabolites.List_metabolites.Select(x => x.Pubchem_compound_id).Intersect(In_add_Pubchem_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                search_additional_pubchem_in_pubchem();
            }
            else if (In_add_Chemspider_id.Count > 0 &&
                metabolites.List_metabolites.Select(x => x.Chemspider_id).Intersect(In_add_Chemspider_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                search_additional_chemspider_in_chemspider();
            }
            else if (In_add_Lipidmaps_id.Count > 0 &&
                metabolites.List_metabolites.Select(x => x.Lipidmaps_id).Intersect(In_add_Lipidmaps_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                search_additional_lipidmaps_in_lipidmaps();
            }
            else if (In_add_Metlin_id.Count > 0 &&
                metabolites.List_metabolites.Select(x => x.Metlin_id).Intersect(In_add_Metlin_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                search_additional_metlin_in_metlin();
            }
            else
            {
                add_not_found_metabolite();
                return;
            }
        }

        private void search_hmdb_in_hmdb_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Hmdb_accession == In_Hmdb_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary HMDB_ID in DB for primary HMDB_ID=" + In_Hmdb_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Hmdb_accession == In_Hmdb_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_cas_in_cas_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Cas_registry_number == In_Cas_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary CAS_ID in DB for primary CAS_ID=" + In_Cas_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Cas_registry_number == In_Cas_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_kegg_in_kegg_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Kegg_id == In_Kegg_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary KEGG_ID in DB for primary KEGG_ID=" + In_Kegg_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Kegg_id == In_Kegg_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_chebi_in_chebi_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Chebi_id == In_Chebi_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary ChEBI_ID in DB for primary ChEBI_ID=" + In_Chebi_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Chebi_id == In_Chebi_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_pubchem_in_pubchem_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Pubchem_compound_id == In_Pubchem_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary PubChem_ID in DB for primary PubChem_ID=" + In_Pubchem_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Pubchem_compound_id == In_Pubchem_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_chemspider_in_chemspider_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Chemspider_id == In_Chemspider_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary ChemSpider_ID in DB for primary ChemSpider_ID=" + In_Chemspider_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Chemspider_id == In_Chemspider_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_lipidmaps_in_lipidmaps_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Lipidmaps_id == In_Lipidmaps_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary LipidMaps_ID in DB for primary LipidMaps_ID=" + In_Lipidmaps_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Lipidmaps_id == In_Lipidmaps_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_metlin_in_metlin_id()
        {
            if (metabolites.List_metabolites.Count(x => x.Metlin_id == In_Metlin_id) > 1)
            {
                outputToLog.WriteLine("more than one compounds for primary Metlin_ID in DB for primary Metlin_ID=" + In_Metlin_id);
            }
            //in case of multiple hits take the one with most pathways
            ToHMDB_metabolite(metabolites.List_metabolites.Where(x => x.Metlin_id == In_Metlin_id).OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_hmdb_in_additional_hmdb_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            Parallel.ForEach(metabolites.List_metabolites, m =>
            {
                if (m.Hmdb_secondary_accessions.Any(x => x == In_Hmdb_id))
                {
                    tmpListOfMetab.Add(m);
                }
            });
            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for alternative HMDB_ID=" + In_Hmdb_id);
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_cas_in_additional_cas_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            Parallel.ForEach(metabolites.List_metabolites, m =>
            {
                if (m.Cts_cas.Any(x => x == In_Cas_id))
                {
                    tmpListOfMetab.Add(m);
                }
            });
            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for CAS_ID=" + In_Cas_id);
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_kegg_in_additional_kegg_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            Parallel.ForEach(metabolites.List_metabolites, m =>
            {
                if (m.Cts_kegg.Any(x => x == In_Kegg_id))
                {
                    tmpListOfMetab.Add(m);
                }
            });
            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for alternative KEGG_ID=" + In_Kegg_id);
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_chebi_in_additional_chebi_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            Parallel.ForEach(metabolites.List_metabolites, m =>
            {
                if (m.Cts_chebi.Any(x => x == In_Chebi_id))
                {
                    tmpListOfMetab.Add(m);
                }
            });
            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for alternative ChEBI_ID=" + In_Chebi_id);
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_hmdb_in_hmdb_and_additional_hmdb_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            List<string> foundids = new List<string>();
            if (metabolites.List_metabolites.Select(x => x.Hmdb_accession).Intersect(In_add_Hmdb_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Hmdb_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addHmdb =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Hmdb_accession == addHmdb))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Hmdb_accession == addHmdb).OrderByDescending(x => x.List_of_pathways.Count).First());
                        foundids.Add(addHmdb);
                    }
                });
            }
            else
            {
                Parallel.ForEach(In_add_Hmdb_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addHmdb =>
                {
                    Parallel.ForEach(metabolites.List_metabolites, m =>
                    {
                        if (m.Hmdb_secondary_accessions.Any(x => x == addHmdb))
                        {
                            tmpListOfMetab.Add(m);
                            foundids.Add(addHmdb);
                        }
                    });
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional HMDB_ID=" + string.Join("|", foundids));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_cas_in_cas_and_additional_cas_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            List<string> foundids = new List<string>();
            if (metabolites.List_metabolites.Select(x => x.Cas_registry_number).Intersect(In_add_Cas_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Cas_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addCas =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Cas_registry_number == addCas))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Cas_registry_number == addCas).OrderByDescending(x => x.List_of_pathways.Count).First());
                        foundids.Add(addCas);
                    }
                });
            }
            else
            {
                Parallel.ForEach(In_add_Cas_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addCas =>
                {
                    Parallel.ForEach(metabolites.List_metabolites, m =>
                    {
                        if (m.Cts_cas.Any(x => x == addCas))
                        {
                            tmpListOfMetab.Add(m);
                            foundids.Add(addCas);
                        }
                    });
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional CAS_ID=" + string.Join("|", foundids));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_kegg_in_kegg_and_additional_kegg_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            List<string> foundids = new List<string>();
            if (metabolites.List_metabolites.Select(x => x.Kegg_id).Intersect(In_add_Kegg_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Kegg_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addKegg =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Kegg_id == addKegg))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Kegg_id == addKegg).OrderByDescending(x => x.List_of_pathways.Count).First());
                        foundids.Add(addKegg);
                    }
                });
            }
            else
            {
                Parallel.ForEach(In_add_Kegg_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addKegg =>
                {
                    Parallel.ForEach(metabolites.List_metabolites, m =>
                    {
                        if (m.Cts_kegg.Any(x => x == addKegg))
                        {
                            tmpListOfMetab.Add(m);
                            foundids.Add(addKegg);
                        }
                    });
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional KEGG_ID=" + string.Join("|", foundids));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_chebi_in_chebi_and_additional_chebi_id()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            List<string> foundids = new List<string>();
            if (metabolites.List_metabolites.Select(x => x.Chebi_id).Intersect(In_add_Chebi_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Chebi_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addChebi =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Chebi_id == addChebi))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Chebi_id == addChebi).OrderByDescending(x => x.List_of_pathways.Count).First());
                        foundids.Add(addChebi);
                    }
                });
            }
            else
            {
                Parallel.ForEach(In_add_Chebi_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addChebi =>
                {
                    Parallel.ForEach(metabolites.List_metabolites, m =>
                    {
                        if (m.Cts_chebi.Any(x => x == addChebi))
                        {
                            tmpListOfMetab.Add(m);
                            foundids.Add(addChebi);
                        }
                    });
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional ChEBI_ID=" + string.Join("|", foundids));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_pubchem_in_pubchem()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            if (metabolites.List_metabolites.Select(x => x.Pubchem_compound_id).Intersect(In_add_Pubchem_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Pubchem_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addPubchem =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Pubchem_compound_id == addPubchem))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Pubchem_compound_id == addPubchem).OrderByDescending(x => x.List_of_pathways.Count).First());
                    }
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional PubChem_ID=" + string.Join("|", In_add_Pubchem_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_chemspider_in_chemspider()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            if (metabolites.List_metabolites.Select(x => x.Chemspider_id).Intersect(In_add_Chemspider_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Chemspider_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addChemspider =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Chemspider_id == addChemspider))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Chemspider_id == addChemspider).OrderByDescending(x => x.List_of_pathways.Count).First());
                    }
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional ChemSpider_ID=" + string.Join("|", In_add_Chemspider_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_lipidmaps_in_lipidmaps()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            if (metabolites.List_metabolites.Select(x => x.Lipidmaps_id).Intersect(In_add_Lipidmaps_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Lipidmaps_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addLipidmaps =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Lipidmaps_id == addLipidmaps))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Lipidmaps_id == addLipidmaps).OrderByDescending(x => x.List_of_pathways.Count).First());
                    }
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional LipidMaps_ID=" + string.Join("|", In_add_Lipidmaps_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void search_additional_metlin_in_metlin()
        {
            List<metabolite> tmpListOfMetab = new List<metabolite>();
            if (metabolites.List_metabolites.Select(x => x.Metlin_id).Intersect(In_add_Metlin_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))).Any())
            {
                Parallel.ForEach(In_add_Metlin_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)), addMetlin =>
                {
                    if (metabolites.List_metabolites.Any(x => x.Metlin_id == addMetlin))
                    {
                        tmpListOfMetab.Add(metabolites.List_metabolites.Where(x => x.Metlin_id == addMetlin).OrderByDescending(x => x.List_of_pathways.Count).First());
                    }
                });
            }

            if (tmpListOfMetab.Count > 1)
            {
                outputToLog.WriteLine("more than one compounds in DB file for additional Metlin_ID=" + string.Join("|", In_add_Metlin_id.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))));
            }
            ToHMDB_metabolite(tmpListOfMetab.OrderByDescending(x => x.List_of_pathways.Count).First());
        }

        private void add_not_found_metabolite()
        {
            if (!string.IsNullOrEmpty(In_Hmdb_id) && !string.IsNullOrWhiteSpace(In_Hmdb_id))
            {
                outputToLog.WriteLine("no compounds detected with HMDB_ID=" + In_Hmdb_id + " (" + In_Name + ")");
            }
            else if (!string.IsNullOrEmpty(In_Cas_id) && !string.IsNullOrWhiteSpace(In_Cas_id))
            {
                outputToLog.WriteLine("no compounds detected with CAS_ID=" + In_Cas_id + " (" + In_Name + ")");
            }
            else if (!string.IsNullOrEmpty(In_Kegg_id) && !string.IsNullOrWhiteSpace(In_Kegg_id))
            {
                outputToLog.WriteLine("no compounds detected with KEGG_ID=" + In_Kegg_id + " (" + In_Name + ")");
            }
            else if (!string.IsNullOrEmpty(In_Chebi_id) && !string.IsNullOrWhiteSpace(In_Chebi_id))
            {
                outputToLog.WriteLine("no compounds detected with ChEBI_ID=" + In_Chebi_id + " (" + In_Name + ")");
            }
            else if (!string.IsNullOrEmpty(In_Pubchem_id) && !string.IsNullOrWhiteSpace(In_Pubchem_id))
            {
                outputToLog.WriteLine("no compounds detected with PubChem_ID=" + In_Pubchem_id + " (" + In_Name + ")");
            }
            else if (!string.IsNullOrEmpty(In_Chemspider_id) && !string.IsNullOrWhiteSpace(In_Chemspider_id))
            {
                outputToLog.WriteLine("no compounds detected with ChemSpider_ID=" + In_Chemspider_id + " (" + In_Name + ")");
            }
            else if (!string.IsNullOrEmpty(In_Lipidmaps_id) && !string.IsNullOrWhiteSpace(In_Lipidmaps_id))
            {
                outputToLog.WriteLine("no compounds detected with LipidMaps_ID=" + In_Lipidmaps_id + " (" + In_Name + ")");
            }
            else if (!string.IsNullOrEmpty(In_Metlin_id) && !string.IsNullOrWhiteSpace(In_Metlin_id))
            {
                outputToLog.WriteLine("no compounds detected with Metlin_ID=" + In_Metlin_id + " (" + In_Name + ")");
            }
            else
            {
                outputToLog.WriteLine("no compounds detected with Name=" + In_Name);
            }
            metabolite hmdb_metab = new metabolite();
            hmdb_metab.metabolite_from_db(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}" +
            "{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}{0}{27}{0}{28}{0}{29}{0}{30}{0}{31}{0}{32}{0}{33}{0}{34}{0}{35}{0}{36}{0}{37}{0}{38}{0}{39}{0}{40}{0}{41}{0}{42}{0}{43}{0}{44}{0}{45}{0}{46}{0}{47}{0}{48}{0}{49}{0}{50}{0}{51}" +
            "{0}{52}{0}{53}{0}{54}{0}{55}{0}{56}{0}{57}{0}{58}{0}{59}{0}{60}{0}{61}{0}{62}{0}{63}{0}{64}{0}{65}{0}{66}{0}{67}{0}{68}{0}{69}{0}{70}{0}{71}{0}{72}{0}{73}{0}{74}{0}{75}{0}{76}{0}{77}{0}{78}",
                '\t', "", "", In_Name, "TRUE", "", "", "", "", "", "", In_Formula, "", "-1", "-1", In_Mass, "-1", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""));
            ToHMDB_metabolite(hmdb_metab);
        }

        public void ToHMDB_metabolite(metabolite db_metabolite)
        {
            Hmdb_accession = db_metabolite.Hmdb_accession;
            Hmdb_secondary_accessions = db_metabolite.Hmdb_secondary_accessions;
            Name = db_metabolite.Name;
            IsProblematic = db_metabolite.IsProblematic;
            Synonym_names = db_metabolite.Synonym_names;
            Description = db_metabolite.Description;
            Description_chebi = db_metabolite.Description_chebi;
            Quality = db_metabolite.Quality;
            Comment = db_metabolite.Comment;
            Charge = db_metabolite.Charge;
            Formula = db_metabolite.Formula;
            Formula_chebi = db_metabolite.Formula_chebi;
            Average_molecular_weight = db_metabolite.Average_molecular_weight;
            Monisotopic_molecular_weight = db_metabolite.Monisotopic_molecular_weight;
            Mass = db_metabolite.Mass;
            Monoisotopic_mass = db_metabolite.Monoisotopic_mass;
            Iupac_name = db_metabolite.Iupac_name;
            Traditional_iupac = db_metabolite.Traditional_iupac;
            Cas_registry_number = db_metabolite.Cas_registry_number;
            Cts_cas = db_metabolite.Cts_cas;
            Smiles = db_metabolite.Smiles;
            Inchi = db_metabolite.Inchi;
            Inchi_chebi = db_metabolite.Inchi_chebi;
            Inchikey = db_metabolite.Inchikey;
            My_taxonomy = new taxonomy()
            {
                Direct_parent = db_metabolite.My_taxonomy.Direct_parent,
                Kingdom = db_metabolite.My_taxonomy.Kingdom,
                Super_class = db_metabolite.My_taxonomy.Super_class,
                Tclass = db_metabolite.My_taxonomy.Tclass,
                Substituents = db_metabolite.My_taxonomy.Substituents,
                Other_descriptors = db_metabolite.My_taxonomy.Other_descriptors
            };
            My_onotology = new ontology()
            {
                Status = db_metabolite.My_onotology.Status,
                Origins = db_metabolite.My_onotology.Origins,
                Biofunctions = db_metabolite.My_onotology.Biofunctions,
                Applications = db_metabolite.My_onotology.Applications,
                Cellular_locations = db_metabolite.My_onotology.Cellular_locations
            };
            State = db_metabolite.State;
            Biofluid_locations = db_metabolite.Biofluid_locations;
            Tissue_locations = db_metabolite.Tissue_locations;
            List_of_pathways = db_metabolite.List_of_pathways;
            Drugbank_id = db_metabolite.Drugbank_id;
            Drugbank_metabolite_id = db_metabolite.Drugbank_metabolite_id;
            Chemspider_id = db_metabolite.Chemspider_id;
            Kegg_id = db_metabolite.Kegg_id;
            Cts_kegg = db_metabolite.Cts_kegg;
            Metlin_id = db_metabolite.Metlin_id;
            Pubchem_compound_id = db_metabolite.Pubchem_compound_id;
            Chebi_id = db_metabolite.Chebi_id;
            Cts_chebi = db_metabolite.Cts_chebi;
            Synthesis_reference = db_metabolite.Synthesis_reference;
            List_of_proteins = db_metabolite.List_of_proteins;
        }

        public class stats
        {
            public string Tissue { get; set; }
            public string Charge { get; set; }

            public List<pairwiseTestValues> Ratio { get; set; }
            public double MultiGroupPvalue { get; set; }
            public List<pairwiseTestValues> PairwiseTestPvalue { get; set; }
            public List<corrVars> CorrelationValues { get; set; }
            public List<corrMetabs> CorrelationMetabolites { get; set; }

            public class pairwiseTestValues
            {
                public string group1 { get; set; }
                public string group2 { get; set; }
                public double pairValue { get; set; }
            }

            public class corrVars
            {
                public string clinical_data_name { get; set; }
                public double corr_value { get; set; }
                public double pValueAdjust { get; set; }
                public double pValueUnadjust { get; set; }
            }

            public class corrMetabs
            {
                public string metab_id { get; set; }
                public double corr_value { get; set; }
                public double pValueAdjust { get; set; }
                public double pValueUnadjust { get; set; }
            }
        }
    }
}
