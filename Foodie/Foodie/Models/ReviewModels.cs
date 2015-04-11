using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Foodie.Models
{

    [Table("Reviews", Schema = "Public")]
    public class Review
    {
        [Key]
        [Column("ReviewId")]
        public int ReviewId { get; set; }

        [DataType(DataType.Text)]
        [Column("ReviewText")]
        [Display(Name = "Review Text")]
        public string ReviewText { get; set; }
        
        [Column("AverageRating")]
        public float AverageRating { get; set; }

        [DataType(DataType.DateTime)]
        [Column("DatePosted")]
        public DateTime DatePosted { get; set; }

        [ForeignKey("Users")]
        [Column("PostedBy")]
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        [ForeignKey("Restauraunts")]
        [Column("RestaurantId")]
        public int RestaurantId { get; set; }

        [ForeignKey("Restauraunts")]
        [Column("RestaurantId")]
        [Display(Name = "Restaurant")]
        public string RestaurantName { get; set; }



    }
    
    [Table("Reviews", Schema = "Public")]
    public class CreateReviewModel
    {
        [Required]
        [Display(Name = "Restaurant")]
        public string RestaurantName { get; set; }

        [ForeignKey("Restauraunts")]
        [Column("RestaurantId")]
        public int RestaurantId { get; set; }

        [Required]
        [Display(Name = "Review Text")]
        [DataType(DataType.MultilineText)]
        public string ReviewText { get; set; }
    }
}