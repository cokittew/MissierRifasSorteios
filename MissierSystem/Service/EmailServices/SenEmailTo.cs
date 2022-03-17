using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MissierSystem.Service.EmailServices
{
    public static class SenEmailTo
    {
        public static bool Send(string to, string subject, string body)
        {
            MailMessage mm = new MailMessage("dunaddaoficial@gmail.com", to, subject, body);
            mm.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("dunaddaoficial@gmail.com", "G428289393!g");

            smtp.Credentials = nc;

            try
            {
                smtp.Send(mm);
                return true;

            }catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
           
        }
    }
}
