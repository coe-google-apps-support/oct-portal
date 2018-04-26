using CoE.Ideas.Core.Services;
using CoE.Ideas.Server.Controllers;
using System;

namespace CoE.Ideas.Integration.Apex
{
    internal class ApexListener
    {
        public ApexListener(IInitiativeRepository initiativeRepository, Serilog.ILogger logger)
        {
            //using (var con = new Oracle.ManagedDataAccess.Client.OracleConnection("SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=orarac2-scan.gov.edmonton.ab.ca)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ACES12R1.GOV.EDMONTON.AB.CA)));uid = ITO_RO; pwd = C5dzfAWeegB1; "))
            using (var con = new Oracle.ManagedDataAccess.Client.OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=orarac2-scan.gov.edmonton.ab.ca)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ACES12R1.GOV.EDMONTON.AB.CA)));User Id = ITO_RO; Password = C5dzfAWeegB1; "))
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT BTIR_ID, BTIR_CREATED_BY, BTIR_PROJECT_NAME, BTIR_PROJECT_DESC FROM OCI.BTIR_REQUEST";
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int apexId = reader.GetInt32(0);
                    //foreach (var item in collection)
                    //{


                    //    await IdeasController.p              (newIdeaStatus.Value));
                    //    IdeasController.PostInitiative

                    //}
                    
                    Console.WriteLine(apexId);
                    //Console.Read();
                }


            }

        }

        public async Task<IdeasController> Task
    }
}