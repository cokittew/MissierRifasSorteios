using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Validation
{
    [Table("user_telegram_validation")]

    public class UserTelegramValidation
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

        [Column(name: "removed")]
        public bool Removed { get; set; }
    }
}
