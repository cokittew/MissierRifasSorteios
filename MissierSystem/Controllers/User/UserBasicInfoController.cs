using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using MissierSystem.Models.Platform.Services.Raffle;
using MissierSystem.Models.Validation;
using MissierSystem.Service.EmailServices;
using MissierSystem.Service.TokenServices;

namespace MissierSystem.Controllers.User
{
    public class UserBasicInfoController : Controller
    {
        private readonly UserClienteDataContext _context;

        public UserBasicInfoController(UserClienteDataContext context)
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

        #region UserMethods
        public IActionResult Login(string localuse="", string answer = "", string raffleId = "")
        {
            var divisionData = raffleId.Split(':');

            var raffleIdDivided = String.IsNullOrEmpty(divisionData[0]) ? "" : divisionData[0];
            var raffleOwnerId = String.IsNullOrEmpty(raffleId) ? "": divisionData[1];

            ViewBag.alert = answer;
            ViewBag.raffleId = raffleIdDivided;
            ViewBag.owner = raffleOwnerId;

            LoginModel l = new LoginModel()
            {
                LoginType = localuse
            };

            if (localuse.Equals("platform") || localuse.Equals("Platform"))
                ViewBag.redirectTo = "Platform";

            if (localuse.Equals("UseServices") || localuse.Equals("useServices"))
                ViewBag.redirectTo = "UseServices";

            if (localuse.Equals("DirectRaffleAccess"))
                ViewBag.redirectTo = "DirectRaffleAccess";


            if (String.IsNullOrEmpty(localuse))
                return RedirectToAction("Index", "Home");

            var userId = HttpContext.Session.GetInt32("UserLogId");

            #region Login Direto Session

            if (userId != null && userId != 0)
            {
                var userEspecifications = _context.UserBasic.Where(u => u.Id == userId).FirstOrDefault();
                if (userEspecifications != null)
                {
                    var email = userEspecifications.EmailVerify ? "1" : "0";
                    var phone = userEspecifications.PhoneVerify ? "1" : "0";
                    var signature = userEspecifications.SignatureActive ? "1" : "0";

                    string encodeBuild = RandomToken.RandomString(2) + email + RandomToken.RandomString(3) + phone +
                        RandomToken.RandomString(5) + signature + RandomToken.RandomString(2) + userEspecifications.Id;

                    UserDataCrossRoute data = new UserDataCrossRoute
                    {
                        userIdentifier = encodeBuild,
                        actionConfirmation = "001",
                        validationCheck = "PassPass"
                    };

                    if (localuse.Equals("UseServices") || localuse.Equals("useServices"))
                        return RedirectToAction("MainPageParticipant", "Home", data);
                    else if (localuse.Equals("platform") || localuse.Equals("Platform"))
                        return RedirectToAction("MainPagePlatform", "SystemUserPlatform", data);
                    else if (localuse.Equals("DirectRaffleAccess"))
                    {
                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == Convert.ToInt32(raffleIdDivided) && !e.Removed)
                        .Select(e => new PlatformServiceRaffle() { Id = e.Id, RaffleType = e.RaffleType }).FirstOrDefault();
                        var raffleSelectedParameters = new Dictionary<string, string>() { { "raffleId", raffleIdDivided } };

                        if (raffle.RaffleType == 3)
                            return RedirectToAction("AmorinStyle", "Raffle", raffleSelectedParameters);

                        else
                            return RedirectToAction("SelectedRaffleParticipate", "Raffle", raffleSelectedParameters);
                    }
                }
            }
            #endregion

            return View(l);
        }

        public ActionResult LoginPlatform()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            return RedirectToAction("MainPagePlatform", "SystemUserPlatform");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind("Email, Password, LoginType")] LoginModel userLogin, IFormCollection form)
        {
            try
            {
                var idUser = _context.UserBasicInfo.Where(user => user.Email.Equals(userLogin.Email) && user.UserPassword.Equals(userLogin.Password))
                    .Select(e => new UserBasicInfo() { Id = e.Id, IdBasicUser = e.IdBasicUser, NickName = e.NickName}).FirstOrDefault();

                if (idUser != null)
                {
                    //var teste = _context.UserBasic.ToList();
                    var userEspecifications = _context.UserBasic.Where(u => u.Id == idUser.IdBasicUser)
                        .Select(e=> new UserBasic() { Id = e.Id, SignatureActive = e.SignatureActive}).FirstOrDefault();
                    if (userEspecifications != null)
                    {
                        HttpContext.Session.SetInt32("UserLogId", userEspecifications.Id);

                        #region Adicionar Variavel de ambiente

                        //string enviromentUrlIdUser = userEspecifications.IdIdentity + userEspecifications.Id.ToString();
                        //string EnviromentUserIdVar = Environment.GetEnvironmentVariable(enviromentUrlIdUser);

                        //if (EnviromentUserIdVar == null)
                        //{
                        //    Environment.SetEnvironmentVariable(enviromentUrlIdUser, userEspecifications.Id.ToString());

                        //    EnviromentUserIdVar = Environment.GetEnvironmentVariable(enviromentUrlIdUser);

                        //    if (String.IsNullOrEmpty(EnviromentUserIdVar))
                        //    {
                        //        var outUser = new Dictionary<string, string>()
                        //                {
                        //                    {"localuse",userLogin.LoginType },
                        //                    {"answer","Error"}
                        //                };
                        //        return RedirectToAction("Login", outUser);
                        //    }
                        //}

                        #endregion

                        if (userEspecifications.SignatureActive)
                        {
                            if (userLogin.LoginType.Equals("UseServices"))
                                return RedirectToAction("MainPageParticipant", "Home");
                            else if (userLogin.LoginType.Equals("Platform"))
                                return RedirectToAction("MainPagePlatform", "SystemUserPlatform");
                            else if (userLogin.LoginType.Equals("DirectRaffleAccess"))
                            {
                                var r = form["raffleId"];
                                var o = form["owner"];

                                if (!String.IsNullOrEmpty(o) && o != "0")
                                {
                                    if (userEspecifications.Id == Convert.ToInt32(o))
                                        return RedirectToAction("MainPagePlatform", "SystemUserPlatform"/*, data*/);
                                    else
                                    {
                                        if (String.IsNullOrEmpty(r) || r == "0")
                                            return RedirectToAction("MainPageParticipant", "Home"/*, data*/);

                                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == Convert.ToInt32(r) && !e.Removed)
                                                    .Select(e => new PlatformServiceRaffle() { Id = e.Id, RaffleType = e.RaffleType }).FirstOrDefault();
                                        var raffleSelectedParameters = new Dictionary<string, string>() { { "raffleId", r } };

                                        if (raffle.RaffleType == 3)
                                            return RedirectToAction("AmorinStyle", "Raffle", raffleSelectedParameters);

                                        else
                                            return RedirectToAction("SelectedRaffleParticipate", "Raffle", raffleSelectedParameters);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var token = _context.UserTelegramValidation.Where(e => e.UserId == idUser.IdBasicUser && !e.Removed).FirstOrDefault();
                            if(token != null)
                                return RedirectToAction("MainPageParticipantAutentification", new Dictionary<string, string>() { { "userName", idUser.NickName} });
                            else
                            {
                                IsAutenticated();
                                return RedirectToAction("GetOutFromLogin", "Home");
                            }
                        } 
                    }
                }
            }
            catch (Exception nullError)
            {
                Console.WriteLine(nullError);
            }

            var ra = form["raffleId"];
            var ow = form["owner"];

            var dic = new Dictionary<string, string>()
            {
                {"localuse",userLogin.LoginType },
                {"raffleId", ra + ":" + ow  },
                {"answer","NotFound" }
            };
            return RedirectToAction("Login", dic);
        }

        public ActionResult RescuePassword()
        {
            return View();
        }

        public IActionResult NewUsersignUp(string localuse="", string raffleId="", string alert = "")
        {
            ViewBag.localUse = localuse;
            ViewBag.raffleId = raffleId;
            ViewBag.alert = alert;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewUsersignUp([Bind("Id,FullName,NickName,Email,Phone,UserPassword,ConfirmPassword")] UserBasicInfo userBasicInfo, IFormCollection form)
        {
            var localUse = form["RedirectTo"];
            var raffleId = form["raffleId"];

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.BeginTransaction();
                    var user = _context.Add(new UserBasic 
                    { UserNumberBag = 10, IdIdentity = RandomToken.RandomString(10) , BeginningDate = DateTime.Now, SignatureActive = true });
                    await _context.SaveChangesAsync();

                    if (user != null)
                        userBasicInfo.IdBasicUser = user.Entity.Id;
                    else
                        return Json($"Houve um erro ao registrar seu usuário."); // Alterar isso

                    userBasicInfo.BeginningDate = DateTime.Now;

                    if (!String.IsNullOrEmpty(userBasicInfo.Phone))
                    {
                        var pattern = new Regex("[^0-9]");
                        var number = pattern.Replace(userBasicInfo.Phone, string.Empty);
                        userBasicInfo.Phone = number;
                    }

                    _context.Add(userBasicInfo);
                    await _context.SaveChangesAsync();

                    UserTelegramValidation telegramValidation = new UserTelegramValidation()
                    {
                        UserId = userBasicInfo.IdBasicUser,
                        CodeAccess = RandomToken.RandomString(8),
                        Removed = true
                    };

                    var result = _context.UserTelegramValidation.Add(telegramValidation);
                    await _context.SaveChangesAsync();

                    var raffleSelectedToNewAccountParameters = new Dictionary<string, string>()
                        {
                            {"localuse", localUse},
                            {"raffleId", raffleId },
                            {"alert", "fail" }
                        };

                    if (result.Entity.Id != 0)
                    {
                        //Adicionar Pré Url nas configs
                        //if (!SenEmailTo.Send(userBasicInfo.Email, "Missier Bot Services - Email de Confirmação", "https://missier.azurewebsites.net/UserBasic/VerifyEmailAutencityConfirmation?code=" + result.Entity.CodeAccess))
                        //{
                        //    _context.Database.RollbackTransaction();
                        //    if (localUse == "DirectRaffleAccess")
                        //        return RedirectToAction("NewUsersignUp", raffleSelectedToNewAccountParameters);

                        //    return View(userBasicInfo);
                        //}
                    }
                    else
                    {
                        _context.Database.RollbackTransaction();

                        if (localUse == "DirectRaffleAccess")
                            return RedirectToAction("NewUsersignUp", raffleSelectedToNewAccountParameters);

                        return View(userBasicInfo);
                    }

                    _context.Database.CommitTransaction();

                    HttpContext.Session.SetInt32("UserLogId", user.Entity.Id);

                    if (localUse == "DirectRaffleAccess")
                    {
                        var raffleSelectedToLoginParameters = new Dictionary<string, string>()
                        {
                            {"localuse", "DirectRaffleAccess"},
                            {"answer", "ToSeeRaffle" },
                            {"raffleId", raffleId + ":" }
                        };

                        return RedirectToAction("Login", "UserBasicInfo", raffleSelectedToLoginParameters);
                    }

                    return RedirectToAction("MainPageParticipantAutentification", new Dictionary<string, string>() { { "userName", userBasicInfo.NickName } });

                    //return RedirectToAction("MainPageParticipant", "Home");
                }
                catch (SqlException e)
                {
                    _context.Database.RollbackTransaction();
                    Console.WriteLine(e);
                }
            }

            return View(userBasicInfo);
        }
        #endregion
        public IActionResult MainPageParticipantAutentification(string userName)
        {
            var idUser = HttpContext.Session.GetInt32("UserLogId");

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            var token = _context.UserTelegramValidation.FirstOrDefaultAsync(e => e.UserId == idUser).Result;
            ViewBag.userName = userName;
            return View(token);
        }

        #region Remote Validations
        public async Task<IActionResult> VerifyNewEmail(string email)
        {
            if (email.Contains('@') && email.Contains('.'))
            {
                var lastChar = email.ElementAt(email.Length - 1);

                if (lastChar.Equals('.') || lastChar.Equals('@'))
                    return Json($"Endereco de email inválido!");

                try
                {
                    var userFind = await _context.UserBasicInfo.FirstOrDefaultAsync(u => u.Email.Equals(email));

                    if (userFind != null)
                        return Json($"Esse email já está sendo usado!");
                    else
                        return Json(true);
                }
                catch (Exception e)
                {
                    _context.Database.RollbackTransaction();
                    Console.WriteLine(e);
                }
            }
            return Json($"Endereco de email inválido!");
        }

        public IActionResult VerifyEmailLogin(string email)
        {
            if (email.Contains('@') && email.Contains('.'))
            {
                var lastChar = email.ElementAt(email.Length - 1);

                if (lastChar.Equals('.') || lastChar.Equals('@'))
                    return Json($"Endereco de email inválido!");

                return Json(true);
            }
            return Json($"Endereco de email inválido!");
        }

        public async Task<IActionResult> VerifyNewPhone(string phone)
        {
            var pattern = new Regex("[^0-9]");
            var number = pattern.Replace(phone, string.Empty);

            if (number.Length == 11)
            {
                try
                {
                    var userFind = await _context.UserBasicInfo.FirstOrDefaultAsync(u => u.Phone.Equals(number));

                    if (userFind != null)
                        return Json($"Esse Nº de telefone já está sendo usado!");
                }
                catch (Exception e)
                {
                    _context.Database.RollbackTransaction();
                    Console.WriteLine(e);
                    return Json($"Nº de telefone inválido!");
                }

                return Json(true);
            }
            return Json($"Nº de telefone inválido!");
        }

        public IActionResult VerifyPasswordFormat(string password)
        {
            string caractersEspecialSequence = "/*-+!@#$%¨&*(){}^`><:|´[]~;.,\\<>'";
            string uperCase = "abcdefghijklmnopqrstuvwxyzéáíóúãõôîûâê".ToUpper();
            string lowCase = "abcdefghijklmnopqrstuvwxyzéáíóúãõôîûâê";

            bool especialCaracter = false;
            bool uperCaseCaracter = false;
            bool lowCaracter = false;

            for (int i = 0; i < caractersEspecialSequence.Length; i++)
            {
                if (password.Contains(caractersEspecialSequence[i]))
                {
                    especialCaracter = true;
                    break;
                }
            }

            for (int i = 0; i < uperCase.Length; i++)
            {
                if (password.Contains(uperCase[i]))
                {
                    uperCaseCaracter = true;
                    break;
                }
            }

            for (int i = 0; i < lowCase.Length; i++)
            {
                if (password.Contains(lowCase[i]))
                {
                    lowCaracter = true;
                    break;
                }
            }

            if (!especialCaracter || !uperCaseCaracter || !lowCaracter)
                return Json($"A senha precisa conter caracteres especiais e letras maiúsculas.");

            return Json(true);
        }

        public IActionResult VerifyPasswordsSimilarity(string userPassword, string confirmPassword)
        {
            if (userPassword == null)
                return Json(true);
            return VerifyPasswordFormat(userPassword);
        }

        public IActionResult VerifyNewNickName(string nickName)
        {
            var user = _context.UserBasicInfo.FirstOrDefault(e => e.NickName.Equals(nickName));

            if (user != null)
                return Json($"infelizmente '" + nickName + "' já está em uso.");

            return Json(true);
        }

        #endregion

        #region EntityAction
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserBasicInfo.ToListAsync());
        }

        // GET: UserBasicInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBasicInfo = await _context.UserBasicInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBasicInfo == null)
            {
                return NotFound();
            }

            return View(userBasicInfo);
        }

        // GET: UserBasicInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBasicInfo = await _context.UserBasicInfo.FindAsync(id);
            if (userBasicInfo == null)
            {
                return NotFound();
            }
            return View(userBasicInfo);
        }

        // POST: UserBasicInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdBaiscUser,FullName,Email,Phone,UserPassword,BeginningDate")] UserBasicInfo userBasicInfo)
        {
            if (id != userBasicInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBasicInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBasicInfoExists(userBasicInfo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userBasicInfo);
        }

        // GET: UserBasicInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBasicInfo = await _context.UserBasicInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBasicInfo == null)
            {
                return NotFound();
            }

            return View(userBasicInfo);
        }

        // POST: UserBasicInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userBasicInfo = await _context.UserBasicInfo.FindAsync(id);
            _context.UserBasicInfo.Remove(userBasicInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool UserBasicInfoExists(int id)
        {
            return _context.UserBasicInfo.Any(e => e.Id == id);
        }
    }
}
