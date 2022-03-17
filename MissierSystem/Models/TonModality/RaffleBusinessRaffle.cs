using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MissierSystem.Models.TonModality
{
    [Table("raffle_business_raffle")]

    public class RaffleBusinessRaffle
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("id_basic_user")]
        [Column("id_basic_user")]
        [Display(Name = "Id do Usuário")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdBasicUser { get; set; }

        [Column("id_identity")]
        [Display(Name = "Código Único")]
        [MaxLength(10, ErrorMessage = "Identity ultrapassou o limite de 10 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Identity { get; set; }

        [Column("raffle_name")]
        [Display(Name = "Nome do Sorteio")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(200, ErrorMessage = "Máximo de 200 caracteres.")]
        [MinLength(10, ErrorMessage = "Mínimo de 10 caracteres.")]
        public string RaffleName { get; set; }

        [Column("raffle_general_description")]
        [Display(Name = "Orientações")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(600, ErrorMessage = "Máximo de 200 caracteres.")]
        [MinLength(10, ErrorMessage = "Mínimo de 10 caracteres.")]
        public string RaffleGeneralDescription { get; set; }
        
        [Column("raffle_premiation_description")]
        [Display(Name = "Descrição Sobre a Premiação")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(500, ErrorMessage = "Máximo de 200 caracteres.")]
        [MinLength(10, ErrorMessage = "Mínimo de 10 caracteres.")]
        public string RafflePremiationDescription { get; set; }

        [Column("raffle_receipt_file")]
        [Display(Name = "Foto")]
        public string RaffleReceiptFile { get; set; }

        [Column("raffle_number_value")]
        [Display(Name = "Valor")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Range(0, 10000, ErrorMessage = "Digite um valor entre R$1,00 e R$10.000,00")]
        [DataType(DataType.Currency)]
        public decimal RaffleNumbersValue { get; set; }

        [Column("raffle_start_date")]
        [Display(Name = "Data de Início")]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Remote("VerifyDateConcordancy", "PlataformServiceRaffle", nameof(RaffleEndDate))]
        public DateTime RaffleStartDate { get; set; }

        [Column("raffle_end_date")]
        [Display(Name = "Data de Finalização")]
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Remote("VerifyDateConcordancy", "PlataformServiceRaffle", AdditionalFields = nameof(RaffleStartDate))]
        public DateTime RaffleEndDate { get; set; }

        [Column("raffle_status")]
        [Display(Name = "Estatus Atual")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int RaffleStatus { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
