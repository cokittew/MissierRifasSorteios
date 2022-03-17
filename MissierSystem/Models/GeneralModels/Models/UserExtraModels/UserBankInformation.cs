using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MissierSystem.Models.GeneralModels.Models.UserExtraModels
{
    [Table("user_bank_information")]
    public class UserBankInformation
    {
        [Key]
        public int Id { get; set; }

        [Column("id_basic_user")]
        [ForeignKey("id_basic_user")]
        [Display(Name = "Seu ID")]
        public int IdBasicUser { get; set; }

        [Column("what_services_use")]
        [Display(Name = "Usar em ")]
        [MaxLength(200, ErrorMessage = "Você excedeu o limite de 100 caracteres.")]
        //[Remote(action: "VerifyInstagram", controller: "UserBasicInfo")]
        public string WhatServiceUse { get; set; }

        [Column("bank_code")]
        [Display(Name = "Cádigo do banco")]
        [MaxLength(5, ErrorMessage = "Você excedeu o limite de 5 caracteres.")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        //[Remote(action: "VerifyInstagram", controller: "UserBasicInfo")]
        public string BankCode { get; set; }

        [Column("bank_account")]
        [Display(Name = "Nº da conta(com dígito)")]
        [MaxLength(15, ErrorMessage = "Você excedeu o limite de 15 caracteres.")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        //[Remote(action: "VerifyInstagram", controller: "UserBasicInfo")]
        public string BankAccount { get; set; }

        [Column("agence_account")]
        [Display(Name = "Agência")]
        [MaxLength(5, ErrorMessage = "Você excedeu o limite de 5 caracteres.")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        //[Remote(action: "VerifyInstagram", controller: "UserBasicInfo")]
        public string AgenceAccount { get; set; }

        [Column("cpf_owner")]
        [Display(Name = "CPF do titular")]
        [MaxLength(11, ErrorMessage = "Você excedeu o limite de 11 caracteres.")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        //[Remote(action: "VerifyInstagram", controller: "UserBasicInfo")]
        public string AccountOwnerCpf { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Data de Registro")]
        public DateTime BeginningDate { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
