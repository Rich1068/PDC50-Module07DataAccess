using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//added
using System.Data;
using Module07DataAccess.Model;
using MySql.Data.MySqlClient;

namespace Module07DataAccess.Services
{
    public class PersonalService
    {
        private readonly string _connectionString;

        public PersonalService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }
        public async Task<List<Personal>>GetAllPersonalsAsync()
        {
            var personalService = new List<Personal>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                //retrieve data
                var cmd = new MySqlCommand("SELECT * FROM tblEmployee", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalService.Add(new Personal
                        {
                            EmployeeId = reader.GetInt32("EmployeeId"),
                            Name = reader.GetString("Name"),
                            Address = reader.GetString("Address"),
                            email = reader.GetString("email"),
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }
            return personalService;
        }   
    }
}
