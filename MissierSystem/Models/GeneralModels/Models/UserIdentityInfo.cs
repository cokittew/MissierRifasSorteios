using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.GeneralModels.Models
{
    [Table("user_identity_info")]
    public class UserIdentityInfo
    {
        [Key]
        public int Id { get; set; }

        [Column("id_basic_user")]
        [ForeignKey("id_basic_user")]
        [Display(Name = "Seu ID")]
        public int IdBaiscUser { get; set; }

        [Column("cpfCnpj")]
        [Display(Name = "CPF/CNPJ")]
        [Remote(action: "", controller: "")]
        [Required(ErrorMessage = "Por favor, digite um CPF/CNPJ válido!")]
        public string CpfCnpj { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Primeiro Acesso Registrado")]
        public DateTime BeginningDate { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
