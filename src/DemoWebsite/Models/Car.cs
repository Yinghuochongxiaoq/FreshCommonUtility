using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWebsite.Models
{
    /// <summary>
    /// A class which represents the Car table.
    /// </summary>
    [Table("Car")]
    public partial class Car
    {
        public virtual int Id { get; set; }
        public virtual string Make { get; set; }
        public virtual string ModelName { get; set; }
    }
}