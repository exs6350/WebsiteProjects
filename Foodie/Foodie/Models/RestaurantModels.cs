using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Foodie.Models
{
    [Table("Restaurants", Schema = "Public")]
    public class Restaurant
    {
        [Key]
        [Column("RestaurantId")]
        public Guid RestaurantId { get; set; }

        [DataType(DataType.Text)]
        [Column("Name")]
        [Display(Name = "Restaurant Name")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Column("FoodType")]
        [Display(Name = "Food Type")]
        public string FoodType { get; set; }

        [DataType(DataType.Text)]
        [Column("City")]
        public string City { get; set; }

        [DataType(DataType.Text)]
        [Column("State")]
        public string State { get; set; }

        [DataType(DataType.Text)]
        [Column("Country")]
        public string Country { get; set; }

        [DataType(DataType.Text)]
        [Column("Location")]
        public string Location { get; set; }

        [DataType(DataType.Text)]
        [Column("Address")]
        public string Address { get; set; }
    }
}

