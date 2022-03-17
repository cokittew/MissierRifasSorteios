using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services
{
    [Table("platform_all_services_basic")]
    public class PlataformAllServicesBasic
    {
        [Key]
        public int Id { get; set; }

        [Column("name_service")]
        public string ServiceName { get; }

        [Column("name_service")]
        public string ServiceDescription { get; }

        [Column("is_free")]
        public bool IsFree { get; }

        [Column("is_limited")]
        public bool IsLimited { get; }

        [Column("signature_type")]
        public int SignatureType { get; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }
    }
}
