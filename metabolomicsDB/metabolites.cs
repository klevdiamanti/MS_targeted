using System.Collections.Generic;
using System.IO;

namespace metabolomicsDB
{
	public static class metabolites
	{
		public static List<metabolite> List_metabolites = new List<metabolite>();

		public static void Read_metaboliteDatabaseFromFile(string databaseFile)
		{
			//read the all hmdb compounds file
			using (TextReader input = new StreamReader(@"" + databaseFile))
			{
				string line = input.ReadLine();
                metabolite mtb;
                while ((line = input.ReadLine()) != null)
				{
                    mtb = new metabolite();
                    mtb.metabolite_from_db(line);
                    List_metabolites.Add(mtb);
				}
			}
		}
    }
}

