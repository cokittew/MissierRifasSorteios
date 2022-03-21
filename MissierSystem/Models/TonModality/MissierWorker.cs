using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MissierSystem.Models.TonModality
{
    [Table("misser_worker")]

    public class MissierWorker
    {
        [Key]
        public int Id { get; set; }

        [Column("full_name")]
        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(200, ErrorMessage = "Máximo de 200 caracteres.")]
        [MinLength(10, ErrorMessage = "Mínimo de 10 caracteres.")]
        public string FullName { get; set; }

        [Column("id_identity")]
        [Display(Name = "Código Único")]
        [MaxLength(10, ErrorMessage = "Identity ultrapassou o limite de 10 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Identity { get; set; }

        [Column("email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(250, ErrorMessage = "Máximo de 250 caracteres.")]
        [MinLength(10, ErrorMessage = "Mínimo de 10 caracteres.")]
        public string Email { get; set; }

        [Column("pass_word")]
        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        [MaxLength(20, ErrorMessage = "Máximo de 20 caracteres.")]
        [MinLength(15, ErrorMessage = "Mínimo de 15 caracteres.")]
        public string Password { get; set; }

        [Column("hasPermission")]
        [Display(Name = "Permissão")]
        public bool HasPermission { get; set; }

        [Column("hasPermission")]
        [Display(Name = "Permissão Colaboradores")]
        public bool HasPermissionCollaborator { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
