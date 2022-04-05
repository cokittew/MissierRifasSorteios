using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MissierSystem.Models.TonModality
{
    [Table("raffle_business_participant")]

    public class RaffleBusinessParticipant
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("id_raffle")]
        [Column("id_raffle")]
        [Display(Name = "Id da Rifa")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int RaffleId { get; set; }

        [Column("id_identity")]
        [Display(Name = "Código Único")]
        [MaxLength(10, ErrorMessage = "Identity ultrapassou o limite de 10 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Identity { get; set; }

        [Column("full_name")]
        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string FullName { get; set; }

        [Column("email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Email { get; set; }

        [Column("phone")]
        [Display(Name = "Número de Telefone")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Remote(action: "VerifyPhoneNumber", controller: "TonStyle", AdditionalFields = nameof(PhoneNumber2))]
        [MaxLength(15, ErrorMessage = "Número Inválido!")]
        [MinLength(15, ErrorMessage = "Número Inválido!")]
        public string PhoneNumber { get; set; }

        [Column("numbers")]
        [Display(Name = "Seus Números")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Numbers { get; set; }

        [Column("participant_status")]
        [Display(Name = "Estatus Atual")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int ParticipantStatus { get; set; }

        [Column("collaborator_code")]
        [Display(Name = "Código de Convite")]
        [MaxLength(8, ErrorMessage = "Convite ultrapassou o limite de 8 caracteres")]
        [Remote("VerifyInviteCode", "TonStyle")]
        public string CollaboratorCode { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

        [NotMapped]
        [Display(Name = "Confirma Número de Telefone")]
        [Remote(action: "VerifyPhoneNumber", controller: "TonStyle", AdditionalFields = nameof(PhoneNumber))]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(15, ErrorMessage = "Número Inválido!")]
        [MinLength(15, ErrorMessage = "Número Inválido!")]
        public string PhoneNumber2 { get; set; }


        //To Information

        [NotMapped]
        public string ReferenceId { get; set; }
        [NotMapped]
        public string RaffleName { get; set; }
        [NotMapped]
        public string RafflePremiation { get; set; }

        [NotMapped]
        public int NumberQuantity { get; set; }
        [NotMapped]
        public decimal Value { get; set; }
        [NotMapped]
        public string TotalValue { get; set; }
        [NotMapped]
        public string StatusRaffle { get; set; }

        [NotMapped]
        public int StatusPay { get; set; }
        

    }
}
