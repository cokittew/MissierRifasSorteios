using MissierSystem.Models.GeneralModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.WorkClasses
{
    public class UserNumbers
    {
        public UserNumbers() { }

        public UserNumbers(UserBasic User, UserBasicInfo UserInfo, List<int> Numbers)
        {
            this.User = User;
            this.UserInfo = UserInfo;
            this.Numbers = Numbers;
        }

        public UserBasic User { get; set; }
        public UserBasicInfo UserInfo { get; set; }
        public List<NumberStatus> NumberStatus { get; set; }
        public List<int> Numbers { get; set; }
    }
}
