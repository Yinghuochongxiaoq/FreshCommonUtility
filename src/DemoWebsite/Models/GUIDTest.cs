using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWebsite.Models
{

    /// <summary>
    /// A class which represents the GUIDTest table.
    /// </summary>
    [Table("GUIDTest")]
    public partial class GUIDTest
    {
        [Key]
        public virtual int id { get; set; }
        public virtual string guid { get; set; }
        public virtual string name { get; set; }
    }

}