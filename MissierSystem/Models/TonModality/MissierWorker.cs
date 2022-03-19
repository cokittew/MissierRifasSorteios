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
    }
}
