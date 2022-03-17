using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.GeneralModels.Models
{
    [Table("user_basic_info")]
    public class UserBasicInfo
    {
        [Key]
        public int Id { get; set; }

        [Column("id_basic_user")]
        [ForeignKey("id_basic_user")]
        [Display(Name = "Seu ID")]
        public int IdBasicUser { get; set; }

        [Column("full_name")]
        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Por favor, digite seu nome completo!")]
        public string FullName { get; set; }

        [Column("nick_name")]
        [Display(Name = "Nome de Usuário")]
        [Required(ErrorMessage = "Por favor, digite um nome de usuário para sua conta.")]
        [MaxLength(100, ErrorMessage = "Você excedeu o limite de 100 caracteres.")]
        [MinLength(10, ErrorMessage = "Insira no mínimo 10 caracteres.")]
        [Remote(action: "VerifyNewNickName", controller: "UserBasicInfo")]
        public string NickName { get; set; }

        [Column("email")]
        [Required(ErrorMessage = "Por favor, digite um email válido!")]
        [MinLength(6, ErrorMessage = "Insira no mínimo 6 caracteres.")]
        [Remote(action: "VerifyNewEmail", controller: "UserBasicInfo")]
        public string Email { get; set; }

        [Column("phone")]
        [Display(Name = "Nº de Telefone")]
        [Remote(action: "VerifyNewPhone", controller: "UserBasicInfo")]
        public string Phone { get; set; }

        [Column("user_password")]
        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Por favor, digite sua senha.")]
        [Remote(action: "VerifyPasswordsSimilarity", controller: "UserBasicInfo", AdditionalFields = nameof(ConfirmPassword))]
        [MinLength(10, ErrorMessage = "A senha deve conter no mínimo 10 caracteres")]
        [MaxLength(25)]
        public string UserPassword { get; set; }

        [Display(Name = "Confirmar Senha")]
        [Remote(action: "VerifyPasswordsSimilarity", controller: "UserBasicInfo", AdditionalFields = nameof(UserPassword))]
        [Required(ErrorMessage = "Por favor, confirme sua senha.")]
        [MaxLength(25, ErrorMessage ="Limite máximo de 25 caracteres.")]
        [Compare("UserPassword", ErrorMessage = "As senhas não são iguais!")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Primeiro Acesso Registrado")]
        public DateTime BeginningDate { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

    }
}
