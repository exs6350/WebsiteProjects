using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Foodie.Models
{
    [Table("Comments", Schema = "Public")]
    public class CommentViewModel
    {
        [Key]
        [Column("CommentId")]
        public string CommentId { get; set; }

        public string ReviewId { get; set; }

        public string UserId { get; set; }

        [DataType(DataType.Text)]
        public string CommentText { get; set; }

        public string UserName { get; set; }

        public int commentOrder { get; set; }
    }
}