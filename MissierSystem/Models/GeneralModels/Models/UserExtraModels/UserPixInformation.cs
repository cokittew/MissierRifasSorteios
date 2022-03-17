using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MissierSystem.Models.GeneralModels.Models.UserExtraModels
{
    [Table("user_pix_information")]
    public class UserPixInformation
    {
        [Key]
        public int Id { get; set; }

        [Column("id_basic_user")]
        [ForeignKey("id_basic_user")]
        [Display(Name = "Seu ID")]
        public int IdBasicUser { get; set; }

        [Column("pix_key_type")]
        [ForeignKey("pix_key_type")]
        [Display(Name = "Tipo de Chave")]
        public int PixKeyType { get; set; }

        [Column("pix_key")]
        [Display(Name = "Chave PIX")]
        [MaxLength(42, ErrorMessage = "Você excedeu o limite de 100 caracteres.")]
        [MinLength(11, ErrorMessage = "Limite mínimo de 11 caracteres.")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string PixKey { get; set; }

        [Column("what_services_use")]
        [Display(Name = "Serviços usando")]
        [MaxLength(200, ErrorMessage = "Você excedeu o limite de 100 caracteres.")]
        public string WhatServiceUse { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Data de Registro")]
        public DateTime BeginningDate { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

    }
}
