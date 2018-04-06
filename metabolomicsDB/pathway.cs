using System;
using System.Collections.Generic;

namespace metabolomicsDB
{
    public class pathway
    {
        private string kegg_map_id;

		private string smpdb_map_id;
		private string smpdb_map_name;
		private string smpadb_map_description;

		private List<string> list_of_names;
		private string super_class;
		private Tuple<string, string> pathway_map;
		private string organism;
		private string gene;
		private string enzyme;
		private string reaction;
		private string compound;
		private List<Tuple<string, string>> list_of_modules;
		private List<Tuple<string, string>> list_of_diseases;
		private string ko_pathway;
		private string rel_pathway;

        public string Kegg_map_id { get { return kegg_map_id; } set { kegg_map_id = value; } }

		public string Smpdb_map_id { get { return smpdb_map_id; } set { smpdb_map_id = value; } }
		public string Smpdb_map_name { get { return smpdb_map_name; } set { smpdb_map_name = value; } }
		public string Smpadb_map_description { get { return smpadb_map_description; } set { smpadb_map_description = value; } }

		public List<string> List_of_names { get { return list_of_names; } set {list_of_names = value; } }
		public string Super_class { get { return super_class; } set { super_class = value; } }
        public Tuple<string, string> Pathway_map { get { return pathway_map; } set { pathway_map = value;} }
		public string Organism { get { return organism; } set { organism = value; } }
        public string Gene { get { return gene; } set { gene = value; } }
        public string Enzyme { get { return enzyme; } set { enzyme = value; } }
        public string Reaction { get { return reaction; } set { reaction = value; } }
        public string Compound { get { return compound; } set { compound = value; } }
        public List<Tuple<string, string>> List_of_modules { get { return list_of_modules; } set { list_of_modules = value; } }
        public List<Tuple<string, string>> List_of_diseases { get { return list_of_diseases; } set { list_of_diseases = value; } }
        public string Ko_pathway { get { return ko_pathway; } set { ko_pathway = value; } }
        public string Rel_pathway { get { return rel_pathway; } set { rel_pathway = value; } }

        public List<string> pathwayID()
        {
            if (!string.IsNullOrEmpty(kegg_map_id) && !string.IsNullOrWhiteSpace(kegg_map_id) && !string.IsNullOrEmpty(smpdb_map_id) && !string.IsNullOrWhiteSpace(smpdb_map_id))
            {
                return new List<string>() { kegg_map_id, smpdb_map_id };
            }
            if (!string.IsNullOrEmpty(kegg_map_id) && !string.IsNullOrWhiteSpace(kegg_map_id))
            {
                return new List<string>() { kegg_map_id };
            }
            else if (!string.IsNullOrEmpty(smpdb_map_id) && !string.IsNullOrWhiteSpace(smpdb_map_id))
            {
                return new List<string>() { smpdb_map_id };
            }
            else
            {
                return new List<string>() { "Unknown" };
            }
        }

        public string pathwayDetails()
        {
            if (!string.IsNullOrEmpty(kegg_map_id) && !string.IsNullOrWhiteSpace(kegg_map_id))
            {
                return pathway_map.Item2;
            }
            else if (!string.IsNullOrEmpty(smpdb_map_id) && !string.IsNullOrWhiteSpace(smpdb_map_id))
            {
                return smpdb_map_name;
            }
            else
            {
                return "Unknown";
            }
        }

        public string pathwayName()
        {
            if (!string.IsNullOrEmpty(kegg_map_id) && !string.IsNullOrWhiteSpace(kegg_map_id))
            {
                return pathway_map.Item1 + ":" + pathway_map.Item2;
            }
            else if (!string.IsNullOrEmpty(smpdb_map_id) && !string.IsNullOrWhiteSpace(smpdb_map_id))
            {
                return smpdb_map_id + ":" + smpdb_map_name;
            }
            else
            {
                return "Unknown:Unknown";
            }
        }
    }
}
