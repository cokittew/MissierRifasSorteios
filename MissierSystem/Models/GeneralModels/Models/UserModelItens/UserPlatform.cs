using MissierSystem.Models.Platform.Services.Raffle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.GeneralModels.Models.UserModelItens
{
    public class UserPlatform
    {
        public UserBasic UserBasic { get; set; }
        public UserBasicInfo UserBasicInfo { get; set; }
        public string UserFirstName { get; set; }
    }
}
