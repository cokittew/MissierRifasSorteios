using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.WorkClasses
{
    public class BuyNumberPackeage
    {
        public int Quantity { get; set; }

        public decimal Value { get; set; }
    }
}
