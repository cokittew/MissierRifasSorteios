using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.GeneralModels.Models.UserModelItens
{
    [Table("user_email_confirmation")]
    public class EmailConfirmation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("id_basic_user")]
        [Column(name: "id_basic_user")]
        public int UserId { get; set; }

        [Required]
        [Column(name: "code_access")]
        public string CodeAccess { get; set; }
    }
}
