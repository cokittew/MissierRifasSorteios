using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle
{
    [Table("platform_service_raffle_files")]
    public class PlatformServiceRaffleFile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("id_basic_user")]
        [Column("id_basic_user")]
        [Display(Name = "Id do Usuário")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdBasicUser { get; set; }

        [ForeignKey("id_raffle")]
        [Column("id_raffle")]
        [Display(Name = "Id do Sorteio/Rifa")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdRaffle { get; set; }

        [Column("index_type")]
        [Display(Name = "Tipo de Indexação")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(200, ErrorMessage = "Máximo de 200 caracteres.")]
        [MinLength(10, ErrorMessage = "Mínimo de 10 caracteres.")]
        public string IndexType { get; set; }

        [Column("number_sequence")]
        [Display(Name = "Nº Relacionados")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(200, ErrorMessage = "Máximo de 200 caracteres.")]
        public string NumberSequence { get; set; }

        [Column("receipt_value")]
        [Display(Name = "Valor do Comprovante")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public decimal ReceiptValue { get; set; }

        [Column("receipt_file")]
        [Display(Name = "Arquivo")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string ReceiptFile { get; set; }

        [Column("pre_aproved")]
        [Display(Name = "Pré Aprovado")]
        public bool PreAproved { get; set; }

        [Column("aproved")]
        [Display(Name = "Aprovado")]
        public bool Aproved { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Primeiro Acesso Registrado")]
        public DateTime BeginningDate { get; set; }

        [NotMapped]
        public List<int> Numbers { get; set; }

    }
}
