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
        public async Task<List<Personal>>GetAllPersonalsAsync() //if retrieving multiple rows need Task<List<Personal>>
        {
            var personalService = new List<Personal>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                //retrieve data
                var cmd = new MySqlCommand("SELECT * FROM tblPersonal", conn);
                using (var reader = await cmd.ExecuteReaderAsync())  //if Select use ExecuteReaderAsync only
                {
                    while (await reader.ReadAsync())
                    {
                        personalService.Add(new Personal
                        {
                            Id = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Gender = reader.GetString("Gender"),
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }
            return personalService;
        }   
        public async Task<bool>AddPersonalAsync(Personal newPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("INSERT INTO tblpersonal (Name, GENDER, ContactNo) VALUES (@Name, @Gender, @ContactNo)", conn);
                    cmd.Parameters.AddWithValue("@Name", newPerson.Name);
                    cmd.Parameters.AddWithValue("@Gender", newPerson.Gender);
                    cmd.Parameters.AddWithValue("@ContactNo", newPerson.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding personal record: {ex.Message}");
                return false;
            }
        }
    }
}
