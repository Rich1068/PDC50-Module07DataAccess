using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module07DataAccess.Model
{
    public class Personal
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string email { get; set; }
        public string ContactNo { get; set; }

        public string FullInfo { get; set; }
    }
}
