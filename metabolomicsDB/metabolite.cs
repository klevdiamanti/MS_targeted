using System;
using System.Collections.Generic;
using System.Linq;

namespace metabolomicsDB
{
    public class metabolite
    {
        public string Hmdb_accession { get; set; }
        public List<string> Hmdb_secondary_accessions { get; set; }
        public string Name { get; set; }
        public List<string> Synonym_names { get; set; }
        public string Description { get; set; }
        public string Description_chebi { get; set; }
        public string Quality { get; set; }
        public string Comment { get; set; }
        public string Charge { get; set; }
        public string Formula { get; set; }
        public string Formula_chebi { get; set; }
        public double Average_molecular_weight { get; set; }
        public double Monisotopic_molecular_weight { get; set; }
        public double Mass { get; set; }
        public double Monoisotopic_mass { get; set; }
        public string Iupac_name { get; set; }
        public string Traditional_iupac { get; set; }
        public string Cas_registry_number { get; set; }
        public List<string> Cts_cas { get; set; }
        public string Smiles { get; set; }
        public string Inchi { get; set; }
        public string Inchi_chebi { get; set; }
        public string Inchikey { get; set; }
        public taxonomy My_taxonomy { get; set; }
        public ontology My_onotology { get; set; }
        public string State { get; set; }
        public List<string> Biofluid_locations { get; set; }
        public List<string> Tissue_locations { get; set; }
        public List<pathway> List_of_pathways { get; set; }
        public string Drugbank_id { get; set; }
        public string Drugbank_metabolite_id { get; set; }
        public string Chemspider_id { get; set; }
        public string Kegg_id { get; set; }
        public List<string> Cts_kegg { get; set; }
        public string Metlin_id { get; set; }
        public string Pubchem_compound_id { get; set; }
        public List<string> Cts_Pubchem { get; set; }
        public string Lipidmaps_id { get; set; }
        public List<string> Cts_Lipidmaps { get; set; }
        public string Chebi_id { get; set; }
        public List<string> Cts_chebi { get; set; }
        public string Synthesis_reference { get; set; }
        public List<protein> List_of_proteins { get; set; }
        public List<disease> List_of_diseases { get; set; }
        public bool IsProblematic { get; set; }

        public void metabolite_from_db(string db_line)
        {
            List<string> breakAtSep = db_line.Split('\t').ToList();

            Hmdb_accession = breakAtSep.First(); //0
            Hmdb_secondary_accessions = breakAtSep.ElementAt(1).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //1
            Name = breakAtSep.ElementAt(2); //2
            IsProblematic = Convert.ToBoolean(breakAtSep.ElementAt(3)); //3
            Synonym_names = breakAtSep.ElementAt(4).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //4
            Description = breakAtSep.ElementAt(5); //5
            Description_chebi = breakAtSep.ElementAt(6); //6
            Quality = breakAtSep.ElementAt(7); //7
            Comment = breakAtSep.ElementAt(8); //8
            Charge = breakAtSep.ElementAt(9); //9
            Formula = breakAtSep.ElementAt(10); //10
            Formula_chebi = breakAtSep.ElementAt(11); //11
            Average_molecular_weight = Convert.ToDouble(breakAtSep.ElementAt(12)); //12
            Monisotopic_molecular_weight = Convert.ToDouble(breakAtSep.ElementAt(13)); //13
            Mass = Convert.ToDouble(breakAtSep.ElementAt(14)); //14
            Monoisotopic_mass = Convert.ToDouble(breakAtSep.ElementAt(15)); //15
            Iupac_name = breakAtSep.ElementAt(16); //16
            Traditional_iupac = breakAtSep.ElementAt(17); //17
            Cas_registry_number = breakAtSep.ElementAt(18); //18
            Cts_cas = breakAtSep.ElementAt(19).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //19
            Smiles = breakAtSep.ElementAt(20); //20
            Inchi = breakAtSep.ElementAt(21); //21
            Inchi_chebi = breakAtSep.ElementAt(22); //22
            Inchikey = breakAtSep.ElementAt(23); //23
            My_taxonomy = new taxonomy()
            {
                Description = breakAtSep.ElementAt(24), //24
                Direct_parent = breakAtSep.ElementAt(25), //25
                Kingdom = breakAtSep.ElementAt(26), //26
                Super_class = breakAtSep.ElementAt(27), //27
                Tclass = breakAtSep.ElementAt(28), //28
                Molecular_framework = breakAtSep.ElementAt(29), //29
                Alternative_parents = breakAtSep.ElementAt(30).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(), //30
                Substituents = breakAtSep.ElementAt(31).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(), //31
                Other_descriptors = breakAtSep.ElementAt(32).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList() //32
            };
            My_onotology = new ontology()
            {
                Status = breakAtSep.ElementAt(33), //33
                Origins = breakAtSep.ElementAt(34).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(), //34
                Biofunctions = breakAtSep.ElementAt(35).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(), //35
                Applications = breakAtSep.ElementAt(36).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(), //36
                Cellular_locations = breakAtSep.ElementAt(37).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList() //37
            };
            State = breakAtSep.ElementAt(38); //38
            Biofluid_locations = breakAtSep.ElementAt(39).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //39
            Tissue_locations = breakAtSep.ElementAt(40).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //40
            List_of_pathways = returnListOfPathways(breakAtSep.GetRange(41, 16)); //41-56
            Drugbank_id = breakAtSep.ElementAt(57); //57
            Drugbank_metabolite_id = breakAtSep.ElementAt(58); //58
            Chemspider_id = breakAtSep.ElementAt(59); //59
            Kegg_id = breakAtSep.ElementAt(60); //60
            Cts_kegg = breakAtSep.ElementAt(61).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //61
            Metlin_id = breakAtSep.ElementAt(62); //62
            Pubchem_compound_id = breakAtSep.ElementAt(63); //63
            Cts_Pubchem = breakAtSep.ElementAt(64).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //64
            Lipidmaps_id = breakAtSep.ElementAt(65); //65
            Cts_Lipidmaps = breakAtSep.ElementAt(66).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //66
            Chebi_id = breakAtSep.ElementAt(67); //67
            Cts_chebi = breakAtSep.ElementAt(68).Split('|').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(); //68
            Synthesis_reference = breakAtSep.ElementAt(69); //69
            List_of_proteins = returnListOfProteins(breakAtSep.GetRange(70, 5)); //70-74
            List_of_diseases = returnListOfDiseases(breakAtSep.GetRange(75, 3)); //75-77
        }

        private static List<pathway> returnListOfPathways(List<string> lst)
        {
            List<pathway> retrn = new List<pathway>();
            for (int i = 0; i < lst.First().Split('|').Length; i++)
            {
                retrn.Add(new pathway()
                {
                    Kegg_map_id = lst.First().Split('|').ElementAt(i), //29
                    List_of_names = lst.ElementAt(1).Split('|').ElementAt(i).Split(';').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList(), //30
                    Super_class = lst.ElementAt(2).Split('|').ElementAt(i), //31
                    Pathway_map = (!string.IsNullOrEmpty(lst.ElementAt(3).Split('|').ElementAt(i)) && !string.IsNullOrWhiteSpace(lst.ElementAt(3).Split('|').ElementAt(i))) ?
                    new Tuple<string, string>(lst.ElementAt(3).Split('|').ElementAt(i).Split('-').First(), lst.ElementAt(3).Split('|').ElementAt(i).Split('-').ElementAt(1)) :
                    new Tuple<string, string>("", ""), //32
                    List_of_modules = (!string.IsNullOrEmpty(lst.ElementAt(4).Split('|').ElementAt(i)) && !string.IsNullOrWhiteSpace(lst.ElementAt(4).Split('|').ElementAt(i))) ?
                    lst.ElementAt(4).Split('|').ElementAt(i).Split(';').Select(x => new Tuple<string, string>(x.Split('-').First(), string.Join("-", x.Split('-').Skip(1)))).ToList() :
                    new List<Tuple<string, string>>(), //33
                    List_of_diseases = (!string.IsNullOrEmpty(lst.ElementAt(5).Split('|').ElementAt(i)) && !string.IsNullOrWhiteSpace(lst.ElementAt(5).Split('|').ElementAt(i))) ?
                    lst.ElementAt(5).Split('|').ElementAt(i).Split(';').Select(x => new Tuple<string, string>(x.Split('-').First(), string.Join("-", x.Split('-').Skip(1)))).ToList() :
                    new List<Tuple<string, string>>(), //34
                    Organism = lst.ElementAt(6).Split('|').ElementAt(i), //35
                    Gene = lst.ElementAt(7).Split('|').ElementAt(i), //36
                    Enzyme = lst.ElementAt(8).Split('|').ElementAt(i), //37
                    Reaction = lst.ElementAt(9).Split('|').ElementAt(i), //38
                    Compound = lst.ElementAt(10).Split('|').ElementAt(i), //39
                    Ko_pathway = lst.ElementAt(11).Split('|').ElementAt(i), //40
                    Rel_pathway = lst.ElementAt(12).Split('|').ElementAt(i), //41
                    Smpdb_map_id = lst.ElementAt(13).Split('|').ElementAt(i), //42
                    Smpdb_map_name = lst.ElementAt(14).Split('|').ElementAt(i), //43
                    Smpadb_map_description = lst.ElementAt(15).Split('|').ElementAt(i) //44
                });
            }
            return retrn;
        }

        private static List<protein> returnListOfProteins(List<string> lst)
        {
            List<protein> retrn = new List<protein>();
            for (int i = 0; i < lst.First().Split('|').Length; i++)
            {
                retrn.Add(new protein()
                {
                    Protein_accession = lst.First().Split('|').ElementAt(i), //55 or 57
                    Name = lst.ElementAt(1).Split('|').ElementAt(i), //56 or 58
                    Uniprot_id = lst.ElementAt(2).Split('|').ElementAt(i), //57 or 59
                    Gene_name = lst.ElementAt(3).Split('|').ElementAt(i), //58 or 60
                    Protein_type = lst.ElementAt(4).Split('|').ElementAt(i) //59 or 61
                });
            }
            return retrn;
        }

        private static List<disease> returnListOfDiseases(List<string> lst)
        {
            List<disease> retrn = new List<disease>();
            for (int i = 0; i < lst.First().Split('|').Length; i++)
            {
                retrn.Add(new disease()
                {
                    Name = lst.First().Split('|').ElementAt(i), //56
                    Omim_id = lst.ElementAt(1).Split('|').ElementAt(i), //57
                    List_of_pubmed_ids = lst.ElementAt(2).Split('|').ElementAt(i).Split(';').ToList() //
                });
            }
            return retrn;
        }

        public class taxonomy
        {
            public string Description { get; set; }
            public string Direct_parent { get; set; }
            public string Kingdom { get; set; }
            public string Super_class { get; set; }
            public string Tclass { get; set; }
            public string Molecular_framework { get; set; }
            public List<string> Alternative_parents { get; set; }
            public List<string> Substituents { get; set; }
            public List<string> Other_descriptors { get; set; }
        }

        public class ontology
        {
            public string Status { get; set; }
            public List<string> Origins { get; set; }
            public List<string> Biofunctions { get; set; }
            public List<string> Applications { get; set; }
            public List<string> Cellular_locations { get; set; }
        }

        public class protein
        {
            public string Protein_accession { get; set; }
            public string Name { get; set; }
            public string Uniprot_id { get; set; }
            public string Gene_name { get; set; }
            public string Protein_type { get; set; }
        }

        public class disease
        {
            public string Name { get; set; }
            public string Omim_id { get; set; }
            public List<string> List_of_pubmed_ids { get; set; }
        }
    }
}
