using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.MercadoPago
{
    public class PaymentResult
    {
     
        public string date_approved { get; set; }

        public string date_created { get; set; }

        public string id { get; set; }

        public string status { get; set; }

        public string external_reference { get; set; }

        public string payment_type_id { get; set; }

        public string transaction_amount { get; set; }

        public string reference_id { get; set; }
    }
}
