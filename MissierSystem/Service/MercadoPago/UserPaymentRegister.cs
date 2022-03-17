using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.MercadoPago
{
    [Table("platform_payment_transactions")]
    public class UserPaymentRegister
    {
        [Key]
        public int Id { get; set; }

        [Column("id_basic_user")]
        [Display(Name = "Id do Usuário")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdBasicUser { get; set; }

        [ForeignKey("id_raffle")]
        [Column("id_raffle")]
        [Display(Name = "Id da Rifa")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdRaffle { get; set; }

        [Column("reference")]
        [Display(Name = "Código Único")]
        [MaxLength(25, ErrorMessage = "reference ultrapassou o limite de 25 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Reference { get; set; }

        [Column("referenceId")]
        [Display(Name = "Código Mercado Pago")]
        [MaxLength(250, ErrorMessage = "reference ultrapassou o limite de 25 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string ReferenceId { get; set; }

        [Column("final_status")]
        [Display(Name = "Status")]
        [MaxLength(30, ErrorMessage = "Identity ultrapassou o limite de 30 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string FinalStatus { get; set; }

        [Column("number_quantity")]
        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int NumberQuantity { get; set; }

        [Column("typeTransaction")]
        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int TransactionType { get; set; }

        [Column("totalValue")]
        [Display(Name = "Value")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public decimal TotalValue { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
