using CSV_Data_Loader.Interfaces;

namespace CSV_Data_Loader.Models
{
    public class Person : IEntityBase
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

    }
}
