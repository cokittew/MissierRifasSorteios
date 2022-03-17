using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.GeneralModels.Models
{
    [Table("user_basic")]
    public class UserBasic
    {
        [Key]
        public int Id { get; set; }

        [Column("id_identity")]
        [Display(Name = "Identidade no Sistema")]
        public string IdIdentity { get; set; }

        [Column("email_verify")]
        [Display(Name ="Email Verificado")]
        public bool EmailVerify { get; set; }

        [Column("phone_verify")]
        [Display(Name = "Nº de Telefone Verificado")]
        public bool PhoneVerify { get; set; }

        [Column("signature_active")]
        [Display(Name = "Possui Assinatura Ativa")]
        public bool SignatureActive { get; set; }

        [Column("user_number_bag")]
        [Display(Name = "Cofre de Números")]
        public int UserNumberBag { get; set; }

        [Column("id_telegram")]
        [Display(Name = "Id do Telegram")]
        public string IdTelegram { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Primeiro Acesso Registrado")]
        public DateTime BeginningDate { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

    }
}
