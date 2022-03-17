using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.GeneralModels.Models
{
    [Table("user_guest")]
    public class UserGuest
    {
        [Key]
        public int Id { get; set; }

        [Column("id_telegram")]
        [Display(Name = "Id do Telegram")]
        public int IdTelegram { get; set; }

        [Column("cpfCnpj")]
        [Display(Name = "CPF/CNPJ")]
        public string CpfCnpj { get; set; }

        [Column("email")]
        [Remote(action: "", controller: "")]
        public string Email { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Primeiro Acesso Registrado")]
        public DateTime BeginningDate { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
