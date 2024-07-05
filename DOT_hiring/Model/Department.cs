using System.ComponentModel.DataAnnotations;

namespace DOT_hiring.Model
{
    public class Department
    {
        public int id { get; set; }
        public string name { get; set; }
        public string head { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
