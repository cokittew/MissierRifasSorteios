using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.WorkClasses
{
    public class NumberStatus
    {
        public NumberStatus() { }
        public NumberStatus(int number, int status)
        {
            this.Number = number;
            this.Status = status;
        }

        public int Number { get; set; }
        public int Status { get; set; }
    }
}
