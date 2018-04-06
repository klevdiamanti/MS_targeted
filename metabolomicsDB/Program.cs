
namespace metabolomicsDB
{
    class Program
    {
        static void Main(string[] args)
        {
            metabolites.Read_metaboliteDatabaseFromFile(args[0]);
        }
    }
}
