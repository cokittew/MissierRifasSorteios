using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.GeneralModels.Models.UserModelItens
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        [Display(Name = "Email")]
        [MaxLength(250,ErrorMessage = "Você execedeu o número maximo de 250 caracteres.")]
        [MinLength(6, ErrorMessage = "Insira no mínimo 6 caracteres.")]
        [Remote(action: "VerifyEmailLogin", controller: "UserBasicInfo")]
        public string Email { get; set; }

        [Required (ErrorMessage = "Este campo é obrigatório.")]
        [Display(Name ="Senha")]
        [MaxLength(25, ErrorMessage = "Você execedeu o número maximo de caracteres.")]
        [MinLength(10, ErrorMessage = "Insira no mínimo 10 caracteres.")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Este campo é obrigatório.")]
        //[Display(Name = "Senha")]
        //[MaxLength(25, ErrorMessage = "Você execedeu o número maximo de caracteres.")]
        //[MinLength(10, ErrorMessage = "Insira no mínimo 10 caracteres.")]
        public string LoginType { get; set; }
    }
}
