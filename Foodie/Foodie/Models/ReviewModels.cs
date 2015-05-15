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
        public Guid ReviewId { get; set; }

        [DataType(DataType.Text)]
        [Column("ReviewText")]
        [Display(Name = "Review Text")]
        public string ReviewText { get; set; }
        
        [Column("AverageReviewRating")]
        public double AverageReviewRating { get; set; }

        [DataType(DataType.DateTime)]
        [Column("DatePosted")]
        public DateTime DatePosted { get; set; }

        [ForeignKey("Users")]
        [Column("UserId")]
        public Guid UserId { get; set; }

        [Display(Name = "Submitted By")]
        public string UserName { get; set; }

        [ForeignKey("Restauraunts")]
        [Column("RestaurantId")]
        public Guid RestaurantId { get; set; }

        [ForeignKey("Restauraunts")]
        [Column("RestaurantId")]
        [Display(Name = "Restaurant")]
        public string RestaurantName { get; set; }

        public int Rating { get; set; }

        //whether or not the user in the current session has already rated 
        //the helpfulness of this review(for use with listing reviews)
        public bool currentUserRated;

        public string currentUser { get; set; }

        public IEnumerable<CommentViewModel> comments { get; set; }

    }
    
    [Table("Reviews", Schema = "Public")]
    public class CreateReviewModel
    {
        [Required]
        [Display(Name = "Restaurant")]
        public string RestaurantName { get; set; }

        [ForeignKey("Restauraunts")]
        [Column("RestaurantId")]
        public string RestaurantId { get; set; }

        [Display(Name = "Review Text")]
        [DataType(DataType.MultilineText)]
        public string ReviewText { get; set; }

        public int Rating { get; set; }
    }
    [Table("HelpfulRatings", Schema = "Public")]
    public class HelpfullnessViewModel
    {
        public string HelpfulId { get; set; }
        public string ReviewId { get; set; }
        public string AuthorId { get; set; }

        public string RatingUserId { get; set; }

        public int Rating { get; set; }
    }
}