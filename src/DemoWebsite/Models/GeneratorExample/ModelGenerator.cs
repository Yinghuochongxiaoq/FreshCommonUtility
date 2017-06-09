// This file was automatically generated by the Dapper.SimpleCRUD T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: `testdb`
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `Data Source = (LocalDB)\v11.0;Initial Catalog=SimplecrudDemoWebsite;Integrated Security=True;`
//     Include Views:          `True`

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DemoWebsite.GeneratorExample.Models
{
    /// <summary>
    /// A class which represents the Car table.
    /// </summary>
	[Table("Car")]
	public partial class Car
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Make { get; set; }
		public virtual string ModelName { get; set; }
	}

    /// <summary>
    /// A class which represents the Users table.
    /// </summary>
	[Table("Users")]
	public partial class User
	{
		[Key]
		public virtual int UserId { get; set; }
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
		public virtual int intAge { get; set; }
	}

    /// <summary>
    /// A class which represents the GUIDTest table.
    /// </summary>
	[Table("GUIDTest")]
	public partial class GUIDTest
	{
		[Key]
		public virtual Guid guid { get; set; }
		public virtual string name { get; set; }
	}
}