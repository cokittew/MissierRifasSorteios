using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.TokenServices
{
    public static class DescriptToken
    {
        public static UserBasic GetUserIdFromDecodeRouteObject(UserDataCrossRoute data)
        {

            if(!String.IsNullOrEmpty(data.userIdentifier) && !String.IsNullOrEmpty(data.actionConfirmation) && !String.IsNullOrEmpty(data.validationCheck))
            {
                var codeLenght = data.userIdentifier.Length;
                string idUser = "";

                try
                {
                    for (int i = 15; i < codeLenght; i++)
                    {
                        idUser += data.userIdentifier.ElementAt(i);
                    }

                    var email = data.userIdentifier.ElementAt(2).Equals('0') ? false : true;
                    var phone = data.userIdentifier.ElementAt(6).Equals('0') ? false : true;
                    var signature = data.userIdentifier.ElementAt(12).Equals('0') ? false : true;

                    var user = new UserBasic()
                    {
                        Id = Convert.ToInt32(idUser),
                        EmailVerify = email,
                        PhoneVerify = phone,
                        SignatureActive = signature
                    };

                    return user;

                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }

            return null;
        }
    }
}
