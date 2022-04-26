using Microsoft.AspNetCore.Mvc;
using MissierSystem.Models.Platform.Services.Raffle.RaffleAttributeValidation;
using MissierSystem.Models.Platform.Services.Raffle.WorkClasses.RaffleActionsModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle
{
    [Table("platform_service_raffle")]
    public class PlatformServiceRaffle
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("id_basic_user")]
        [Column("id_basic_user")]
        [Display(Name = "Id do Usuário")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdBasicUser { get; set; }

        [ForeignKey("id_platform_service")]
        [Column("id_platform_service")]
        [Display(Name = "Id do Serviço")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdPlatformService { get; set; }

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

        [Column("raffle_max_number")]
        [Display(Name = "Máximo de Números Para o Sorteio")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        //[Range(10, 500, ErrorMessage = "Selecione um número entre 10 e 500.")]
        public int RaffleMaxNumber { get; set; }

        [Column("raffle_payment_id_allowed")]
        [Display(Name = "Tipos de Pagamento Permitidos")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string RafflePaymentIdAllowed { get; set; }

        [Column("raffle_user_max_numbers")]
        [Display(Name = "Máximo de Números Por Participante")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        //[Range(1, 10, ErrorMessage = "Selecione um número entre 1 e 10.")]
        public int RaffleUserMaxNumbers { get; set; }

        [Column("raffle_number_value")]
        [Display(Name = "Valor")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Range(0, 10000, ErrorMessage = "Digite um valor entre R$1,00 e R$10.000,00")]
        [DataType(DataType.Currency)]
        public decimal RaffleNumbersValue { get; set; }

        [Column("raffle_status")]
        [Display(Name = "Estatus Atual")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int RaffleStatus { get; set; } 

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

        [Column("raffle_numbers_result")]
        [Display(Name = "Resultado")]
        public string RaffleNumberResult { get; set; }

        [Column("raffle_winners_number")]
        [Display(Name = "Máx. de Números Vencedores")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Remote("VerifyMaxWinners", "PlataformServiceRaffle", AdditionalFields = "RaffleMaxNumberLimited")]
        public int RaffleWinnersNumber { get; set; }

        [Column("raffle_type")]
        [Display(Name = "Tipo de Rifa")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int RaffleType { get; set; }

        [Column("raffle_close_option")]
        [Display(Name = "Manual")]
        public bool RaffleCloseOption { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

        //Limited Signature

        [NotMapped]
        [Display(Name = "Máximo de Nº Para o Sorteio")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Range(10, 1000, ErrorMessage = "Selecione um número entre 10 e 1000.")]
        [Remote("VerifyRaffleMaxNumber", "PlataformServiceRaffle")]
        public int RaffleMaxNumberLimited { get; set; }

        [NotMapped]
        [Display(Name = "Máximo de Nº Por Participante")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Remote("VerifyNumberQuantityParticipant", "PlataformServiceRaffle", AdditionalFields = "RaffleMaxNumberLimited")]
        public int RaffleUserMaxNumbersLimited { get; set; }

        [NotMapped]
        [Display(Name = "Iniciar daqui a (em dias)")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Range(0, 7, ErrorMessage = "Selecione um número entre 0 e 7.")]

        public int BegginRaffleDay { get; set; }

        [NotMapped]
        [Display(Name = "Encerrar daqui a (em dias)")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [Range(7, 91, ErrorMessage = "Selecione um número entre 7 e 90.")]
        public int EndRaffleDay { get; set; }



    }
}
