using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.HelperServices
{
    public class SeveralFunctions
    {
        public static string GetUserFirstName(string userFullName)
        {
            if (!String.IsNullOrEmpty(userFullName) || userFullName.Contains(" "))
            {
                var firstName = "";
                for (int i = 0; i < userFullName.Length; i++)
                {
                    if (!userFullName.ElementAt(i).Equals(' '))
                        firstName += userFullName.ElementAt(i);
                    else
                        break;
                }
                return firstName;
            }

            return null;

        }
    }
}
