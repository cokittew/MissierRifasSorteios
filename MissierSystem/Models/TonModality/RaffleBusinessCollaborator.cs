using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MissierSystem.Models.TonModality
{
    [Table("raffle_business_collaborator")]

    public class RaffleBusinessCollaborator
    {    
        [Key]
        public int Id { get; set; }

        [Column("id_identity")]
        [Display(Name = "Código Único")]
        [MaxLength(10, ErrorMessage = "Identity ultrapassou o limite de 10 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Identity { get; set; }

        [Column("pass_word")]
        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(20, ErrorMessage = "Ultrapassou o limite de 20 caracteres")]
        public string Password { get; set; }

        [Column("personal_code")]
        [Display(Name = "Seu Código")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(8, ErrorMessage = "Ultrapassou o limite de 8 caracteres")]
        [MinLength(8, ErrorMessage = "Ultrapassou o limite de 8 caracteres")]
        [Remote("VerifyInviteCodeExistence", "Collaborators")]
        public string PersonalCode { get; set; }


        [Column("phone")]
        [Display(Name = "Número de Telefone")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(15, ErrorMessage = "Ultrapassou o limite de 15 caracteres")]
        public string Phone { get; set; }

        [Column("pix_type")]
        [Display(Name = "Tipo de Chave")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string PixType { get; set; }

        [Column("pix_key")]
        [Display(Name = "Chave PIX")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(100, ErrorMessage = "Ultrapassou o limite de 100 caracteres")]

        public string PixKey { get; set; }

        [Column("full_name")]
        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(200, ErrorMessage = "Ultrapassou o limite de 200 caracteres")]

        public string FullName { get; set; }

        [Column("your_cash")]
        [Display(Name = "Seu Dinheiro")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public decimal YourCash { get; set; }

        [Column("your_cash_percentage")]
        [Display(Name = "Sua porcentagem")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public decimal YourCashPercentage { get; set; }

        [Column("email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(250, ErrorMessage = "Ultrapassou o limite de 250 caracteres")]

        public string Email { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

        [NotMapped]
        public string FirstName { get; set; }
        [NotMapped]
        public string KeyTypeString { get; set; }

        [NotMapped]
        public List<CollaboratorPaymentRegister> PeriodRegisters { get; set; }

    }
}
