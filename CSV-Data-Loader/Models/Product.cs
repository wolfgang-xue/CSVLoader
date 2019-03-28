using CSV_Data_Loader.Interfaces;

namespace CSV_Data_Loader.Models
{
    public class Product : IEntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
