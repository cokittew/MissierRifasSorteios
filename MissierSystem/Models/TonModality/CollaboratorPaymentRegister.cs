using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MissierSystem.Models.TonModality
{
    [Table("collaborator_payment_register")]

    public class CollaboratorPaymentRegister
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("collaborator_id")]
        [Column("collaborator_id")]
        [Display(Name = "Id do Colaborador")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int CollaboratorId { get; set; }

        [ForeignKey("missier_worker_id")]
        [Column("missier_worker_id")]
        [Display(Name = "Id do Funcionario")]
        public int MissierWorkerId { get; set; }

        [Column("period_value")]
        [Display(Name = "Valor Arrecadado")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public decimal PeriodValue { get; set; }

        [Column("period_time")]
        [Display(Name = "Período")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public DateTime PeriodTime { get; set; }

        [Column("receipt_file")]
        [Display(Name = "Comprovante")]
        public string ReceiptFile { get; set; }

        [Column("observation")]
        [Display(Name = "Observações")]
        [MaxLength(300, ErrorMessage = "Ultrapassou o limite de 300 caracteres")]
        public string Observation { get; set; }

        [Column("payment_date")]
        [Display(Name = "Data de Pagamento")]
        public DateTime PaymentDate { get; set; }

        [Column("is_payed")]
        [Display(Name = "Foi Pago?")]
        public bool IsPayed { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
