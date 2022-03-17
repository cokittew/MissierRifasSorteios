using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.MercadoPago
{
    public class PaymentDataFromWebHook
    {
        public string id { get; set; }
        public string date_created { get; set; }
        public string date_approved { get; set; }
        public string payment_method_id { get; set; }
        public string status { get; set; }
        public string status_detail { get; set; }
        public string transaction_amount { get; set; }
    }
}
