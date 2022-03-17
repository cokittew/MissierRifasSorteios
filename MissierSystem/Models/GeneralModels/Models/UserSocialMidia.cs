using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MissierSystem.Models.GeneralModels.Models
{
    [Table("user_social_media")]
    public class UserSocialMidia
    {
        [Key]
        public int Id { get; set; }

        [Column("id_basic_user")]
        [ForeignKey("id_basic_user")]
        [Display(Name = "Seu ID")]
        public int IdBasicUser { get; set; }

        [Column("instagram")]
        [Display(Name = "Instagram")]
        [MaxLength(250, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        //[Remote(action: "VerifyInstagram", controller: "UserBasicInfo")]
        public string Instagram { get; set; }

        [Column("twitter")]
        [Display(Name = "Twitter")]
        [MaxLength(250, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        //[Remote(action: "VerifyTwitter", controller: "UserBasicInfo")]
        public string Twitter { get; set; }

        [Column("youtube")]
        [Display(Name = "Youtube")]
        [MaxLength(250, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        //[Remote(action: "VerifyYoutube", controller: "UserBasicInfo")]
        public string Youtube { get; set; }

        [Column("facebook")]
        [Display(Name = "Facebook")]
        [MaxLength(250, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        //[Remote(action: "VerifyFacebook", controller: "UserBasicInfo")]
        public string Facebook { get; set; }

        [Column("reddit")]
        [Display(Name = "Reddit")]
        [MaxLength(250, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        //[Remote(action: "VerifyReddit", controller: "UserBasicInfo")]
        public string Reddit { get; set; }

        [Column("kwai")]
        [Display(Name = "Kwai")]
        [MaxLength(250, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        //[Remote(action: "VerifyKwai", controller: "UserBasicInfo")]
        public string Kwai { get; set; }

        [Column("whatsapp")]
        [Display(Name = "WhatsApp")]
        [MaxLength(25, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        [Phone]
        //[Remote(action: "VerifyKwai", controller: "UserBasicInfo")]
        public string WhatsApp { get; set; }

        [Column("tikTok")]
        [Display(Name = "TikTok")]
        [MaxLength(250, ErrorMessage = "Você excedeu o limite de 250 caracteres.")]
        //[Remote(action: "VerifyTikTok", controller: "UserBasicInfo")]
        public string TikTok { get; set; }

        [Column("another_informations")]
        [Display(Name = "Outras Informações")]
        [MaxLength(500, ErrorMessage = "Você excedeu o limite de 500 caracteres.")]
        //[Required(ErrorMessage ="Este campo é obrigatório!")]
        public string AnotherInformations { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Data de Registro")]
        public DateTime BeginningDate { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

    }
}
