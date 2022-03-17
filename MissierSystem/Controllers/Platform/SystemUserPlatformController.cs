using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using MissierSystem.Service.HelperServices;
using MissierSystem.Service.TokenServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Controllers.Platform
{
    public class SystemUserPlatformController : Controller
    {
        private readonly UserClienteDataContext _context;

        public SystemUserPlatformController(UserClienteDataContext context)
        {
            _context = context;
        }

        private int? IsAutenticated()
        {
            var id = HttpContext.Session.GetInt32("UserLogId");
            if (id == 0 || id == null)
                return 0;

            var user = _context.UserBasic.Where(e => e.Id == id).Select(e => new UserBasic() { Id = e.Id, SignatureActive = e.SignatureActive }).FirstOrDefault();

            if (user == null || (user != null && !user.SignatureActive))
            {
                HttpContext.Session.SetInt32("UserLogId", 0);
                return 0;
            }

            return user.Id;
        }

        public IActionResult MainPagePlatform()
        {

            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            if(idUser != null && idUser != 0)
            {
                try
                {
                    var userInfo = _context.UserBasicInfo.Where(e => e.IdBasicUser == Convert.ToInt32(idUser))
                        .Select(u=> new UserBasicInfo() {Id= u.Id, FullName = u.FullName }).FirstOrDefault();

                    if(userInfo != null)
                    {
                        UserPlatform userPlatform = new UserPlatform
                        {
                            UserBasic = new UserBasic() {Id = Convert.ToInt32(idUser),Removed = false  },
                            UserBasicInfo = userInfo,
                            UserFirstName = SeveralFunctions.GetUserFirstName(userInfo.FullName),
                        };

                        return View(userPlatform);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            //Houve um erro ao buscar seu usuário.
            return View();
        }

        public IActionResult RaffleTips()
        {
            return View();
        }
    }
}
