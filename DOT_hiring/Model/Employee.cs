using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOT_hiring.Model
{
    public class Employee
    {
        public int id { get; set; }
        public int departmentid { get; set; }

        public Department department { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string email { get; set; }
    }
}