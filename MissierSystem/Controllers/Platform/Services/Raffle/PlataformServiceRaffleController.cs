using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MissierSystem.DataContext;
using MissierSystem.Models.Platform.Enums;
using MissierSystem.Models.Platform.Services.Raffle;
using MissierSystem.Models.Platform.Services.Raffle.WorkClasses.RaffleActionsModel;
using MissierSystem.Service.HelperServices;
using MissierSystem.Service.TokenServices;
using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using MissierSystem.Service.PlatformServices.Raffle;
using MissierSystem.Models.Platform.Services.Raffle.WorkClasses;
using MissierSystem.Models.GeneralModels.Models;
using Newtonsoft.Json;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using MissierSystem.Service.MercadoPago;
using Microsoft.Extensions.Configuration;
using MissierSystem.Models.GeneralModels.Models.UserExtraModels;
using System.Globalization;
using MissierSystem.Service.Telegram;
using MissierSystem.Models.TonModality;

namespace MissierSystem.Controllers.Platform.Services.Raffle
{
    public class PlataformServiceRaffleController : Controller
    {
        private readonly UserClienteDataContext _context;
        private IConfiguration _configuration;
        private readonly string defaultUrlToRaffleDirectAccess;

        public PlataformServiceRaffleController(UserClienteDataContext context, IConfiguration Configuration)
        {
            _context = context;
            _configuration = Configuration;
            //defaultUrlToRaffleDirectAccess = _configuration["URLs:DefaultUrlRaffleDirect"];
            defaultUrlToRaffleDirectAccess = _configuration["URLs:DefaultUrlProdRaffleDirect"];
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

        #region Functions
        public IActionResult RaffleMainPage(string paymentAnswer = "")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = Convert.ToInt32(idUser);

            UserBasic userBasic = null;
            UserBasicInfo userInfo = null;

            try
            {
                userInfo = _context.UserBasicInfo.Where(e => e.IdBasicUser == Convert.ToInt32(idUser))
                    .Select(u => new UserBasicInfo()
                    {
                        IdBasicUser = u.IdBasicUser,
                        FullName = u.FullName,
                        Email = u.Email

                    }).FirstOrDefault();

                userBasic = _context.UserBasic.Where(e => e.Id == userInfo.IdBasicUser)
                    .Select(u => new UserBasic()
                    {
                        Id = u.Id,
                        UserNumberBag = u.UserNumberBag,
                        IdIdentity = u.IdIdentity

                    }).FirstOrDefault();

                var worker = _context.MissierWorker
                    .Where(e => !e.Removed && e.FullName == userInfo.FullName && e.HasPermission && e.Email == userInfo.Email)
                    .Select(e=> new MissierWorker() {Id = e.Id, HasPermission = e.HasPermission, HasPermissionCollaborator = e.HasPermissionCollaborator })
                    .FirstOrDefault();
                ViewBag.Worker = worker;

            } catch (Exception) {
                ViewBag.alert = "error";
                return View(new RaffleAllWeNeed());
            }

            RaffleAllWeNeed rafflePlatform = new RaffleAllWeNeed()
            {
                UserData = new Models.GeneralModels.Models.UserModelItens.UserPlatform(),
            };

            if (userInfo != null && userBasic != null)
            {
                rafflePlatform.UserData.UserBasicInfo = userInfo;
                rafflePlatform.UserData.UserBasic = userBasic;
                rafflePlatform.UserData.UserFirstName = SeveralFunctions.GetUserFirstName(userInfo.FullName);
            }
            else
            {
                ViewBag.alert = "error";
                return View(new RaffleAllWeNeed());
            }

            if (!String.IsNullOrEmpty(paymentAnswer))
                ViewBag.alert = paymentAnswer;

            //Remover os outros dados do allweneed
            return View(rafflePlatform);
        }

        public IActionResult ListRaffleView()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.defaultURL = defaultUrlToRaffleDirectAccess; //Mudar para Prod

            var listQuery = _context.PlataformServiceRaffle.Where(e => e.IdBasicUser == Convert.ToInt32(idUser) && e.RaffleStatus == 1)
                .Select(u => new PlatformServiceRaffle()
                {
                    Id = u.Id,
                    Identity = u.Identity,
                    RaffleCloseOption = u.RaffleCloseOption,
                    RaffleName = u.RaffleName,
                    RaffleStatus = u.RaffleStatus,
                    RaffleMaxNumber = u.RaffleMaxNumber,
                    RaffleNumbersValue = u.RaffleNumbersValue,
                    RaffleUserMaxNumbers = u.RaffleUserMaxNumbers,
                    RaffleStartDate = u.RaffleStartDate,
                    RaffleEndDate = u.RaffleEndDate,
                    RaffleNumberResult = u.RaffleNumberResult,
                    RaffleType = u.RaffleType

                }).OrderByDescending(e=> e.RaffleEndDate).Take(15).ToList();

            foreach (var raffle in listQuery)
            {
                if (DateTime.Now.Date.CompareTo(raffle.RaffleStartDate.Date) >= 0 && raffle.RaffleStatus == 2)
                {
                    raffle.RaffleStatus = 1;
                    _context.PlataformServiceRaffle.Update(raffle);
                    _context.Attach(raffle).Property(e => e.RaffleStatus).IsModified = true;
                    _context.SaveChangesAsync();
                }
                else if (raffle.RaffleEndDate != null)
                {
                    if (raffle.RaffleEndDate.Date.CompareTo(DateTime.Parse("2016-10-01").Date) != 0)
                    {
                        if (DateTime.Now.Date.CompareTo(raffle.RaffleEndDate) >= 0 && raffle.RaffleStatus == 1)
                        {
                            raffle.RaffleStatus = 3;
                            _context.PlataformServiceRaffle.Update(raffle);
                            _context.Attach(raffle).Property(e => e.RaffleStatus).IsModified = true;
                            _context.SaveChangesAsync();
                        }
                    }
                }
            }

            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;

            return View(listQuery.Where(e=> e.RaffleStatus == 1));
        }

        public IActionResult RaffleList(string code = "", string nameNick = "", string type = "", string answer = "")
        {
            ViewBag.alert = answer;

            var idUser = IsAutenticated();

            List<PlatformServiceRaffle> raffleList = new List<PlatformServiceRaffle>();

            ViewBag.defaultURL = defaultUrlToRaffleDirectAccess;
            var status = 1;

            if (idUser != 0 && idUser != null)
            {
                if (!String.IsNullOrEmpty(type) && type != "0")
                    status = Convert.ToInt32(type);

                if (!String.IsNullOrEmpty(code))
                    raffleList = _context.PlataformServiceRaffle.Where(e => e.Identity.Equals(code) && e.IdBasicUser == idUser && e.RaffleStatus == status).Select(u => new PlatformServiceRaffle()
                    {
                        Id = u.Id,
                        Identity = u.Identity,
                        RaffleCloseOption = u.RaffleCloseOption,
                        RaffleName = u.RaffleName,
                        RaffleStatus = u.RaffleStatus,
                        RaffleMaxNumber = u.RaffleMaxNumber,
                        RaffleNumbersValue = u.RaffleNumbersValue,
                        RaffleUserMaxNumbers = u.RaffleUserMaxNumbers,
                        RaffleStartDate = u.RaffleStartDate,
                        RaffleEndDate = u.RaffleEndDate,
                        RaffleNumberResult = u.RaffleNumberResult,
                        RaffleType = u.RaffleType

                    }).ToList();

                else if (!String.IsNullOrEmpty(nameNick))
                {
                    UserBasicInfo userBasicInfo = new UserBasicInfo();
                    userBasicInfo = _context.UserBasicInfo.Where(e => e.NickName.Equals(nameNick) && e.IdBasicUser == idUser)
                        .Select(u => new UserBasicInfo() { IdBasicUser = u.IdBasicUser }).FirstOrDefault();

                    if (userBasicInfo != null)
                        raffleList = _context.PlataformServiceRaffle.Where(e => e.IdBasicUser == userBasicInfo.IdBasicUser && e.IdBasicUser == idUser && e.RaffleStatus == status).Select(u => new PlatformServiceRaffle()
                        {
                            Id = u.Id,
                            Identity = u.Identity,
                            RaffleCloseOption = u.RaffleCloseOption,
                            RaffleName = u.RaffleName,
                            RaffleStatus = u.RaffleStatus,
                            RaffleMaxNumber = u.RaffleMaxNumber,
                            RaffleNumbersValue = u.RaffleNumbersValue,
                            RaffleUserMaxNumbers = u.RaffleUserMaxNumbers,
                            RaffleStartDate = u.RaffleStartDate,
                            RaffleEndDate = u.RaffleEndDate,
                            RaffleNumberResult = u.RaffleNumberResult,
                            RaffleType = u.RaffleType

                        }).Take(15).ToList();
                    else
                        raffleList = _context.PlataformServiceRaffle.Where(e => e.RaffleName.Contains(nameNick) && e.IdBasicUser == idUser && e.RaffleStatus == status).Select(u => new PlatformServiceRaffle()
                        {
                            Id = u.Id,
                            Identity = u.Identity,
                            RaffleCloseOption = u.RaffleCloseOption,
                            RaffleName = u.RaffleName,
                            RaffleStatus = u.RaffleStatus,
                            RaffleMaxNumber = u.RaffleMaxNumber,
                            RaffleNumbersValue = u.RaffleNumbersValue,
                            RaffleUserMaxNumbers = u.RaffleUserMaxNumbers,
                            RaffleStartDate = u.RaffleStartDate,
                            RaffleEndDate = u.RaffleEndDate,
                            RaffleNumberResult = u.RaffleNumberResult,
                            RaffleType = u.RaffleType

                        }).OrderByDescending(e => e.RaffleEndDate).Take(15).ToList();
                }
                else
                    raffleList = _context.PlataformServiceRaffle.OrderByDescending(e => e.RaffleEndDate).Where(e => e.IdBasicUser == idUser && e.RaffleStatus == status).Select(u => new PlatformServiceRaffle()
                    {
                        Id = u.Id,
                        Identity = u.Identity,
                        RaffleCloseOption = u.RaffleCloseOption,
                        RaffleName = u.RaffleName,
                        RaffleStatus = u.RaffleStatus,
                        RaffleMaxNumber = u.RaffleMaxNumber,
                        RaffleNumbersValue = u.RaffleNumbersValue,
                        RaffleUserMaxNumbers = u.RaffleUserMaxNumbers,
                        RaffleStartDate = u.RaffleStartDate,
                        RaffleEndDate = u.RaffleEndDate,
                        RaffleNumberResult = u.RaffleNumberResult,
                        RaffleType = u.RaffleType

                    }).Take(15).ToList();

                if (status != 3)
                {
                    foreach (var raffle in raffleList)
                    {
                        if (DateTime.Now.Date.CompareTo(raffle.RaffleStartDate.Date) >= 0 && raffle.RaffleStatus == 2)
                        {
                            raffle.RaffleStatus = 1;
                            _context.PlataformServiceRaffle.Update(raffle);
                            _context.Attach(raffle).Property(e => e.RaffleStatus).IsModified = true;
                            _context.SaveChangesAsync();
                        }
                        else if (raffle.RaffleEndDate != null)
                        {
                            if (raffle.RaffleEndDate.Date.CompareTo(DateTime.Parse("2016-10-01").Date) != 0)
                            {
                                if (DateTime.Now.Date.CompareTo(raffle.RaffleEndDate) >= 0 && raffle.RaffleStatus == 1)
                                {
                                    raffle.RaffleStatus = 3;
                                    _context.PlataformServiceRaffle.Update(raffle);
                                    _context.Attach(raffle).Property(e => e.RaffleStatus).IsModified = true;
                                    _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }

            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;

            return View(raffleList.Where(e => e.RaffleStatus == status).ToList());
        }

        [HttpPost]
        public async Task<ContentResult> RaffleUpdateInformations([FromQuery(Name = "raffleIdUpdate")] int raffleId, [FromQuery(Name = "description")] string paymentDescription = "", [FromQuery] int userMaxNumber = 0, [FromQuery(Name = "beginDate")] string begginDate = "", [FromQuery] int endDate = 0)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return Content("{\"success\": false, \"msg\": 'Por favor atualize a página para continuar acessando os serviços.' }", "application/json");
            if (raffleId == 0)
                return Content("{\"success\": false, \"msg\": 'Não conseguimos identificar os dados relacionados a sua rifa/sorteio. Tente novamente mais tarde.' }", "application/json");

            PlatformServiceRaffle raffle = null;

            try
            {
                raffle = await _context.PlataformServiceRaffle.FindAsync(raffleId);
                if (raffle == null)
                    return Content("{\"success\": false, \"msg\": 'Não conseguimos identificar os dados relacionados a sua rifa/sorteio. Tente novamente mais tarde.' }", "application/json");
            }
            catch (Exception)
            {
                return Content("{\"success\": false, \"msg\": 'Houve um erro ao buscar os dados de sua rifa no banco de dados. Tente novamente mais tarde.' }", "application/json");
            }

            if (!String.IsNullOrEmpty(paymentDescription))
                raffle.RaffleGeneralDescription = paymentDescription;

            if (userMaxNumber > 0)
                raffle.RaffleUserMaxNumbers = userMaxNumber;

            if (raffle.RaffleStatus != 1)
            {
                if (String.IsNullOrEmpty(begginDate))
                {
                    var days = Convert.ToInt32(begginDate);
                    if (days > 0)
                        raffle.RaffleStartDate = DateTime.Now.AddDays(days);
                    else
                        raffle.RaffleStartDate = DateTime.Now;
                }
            }

            if (raffle.RaffleStatus == 2)
            {
                if (endDate > 0)
                {
                    if (endDate == 91)
                        raffle.RaffleEndDate = DateTime.Parse("2016-10-01").Date;
                    else
                    {
                        var date = raffle.RaffleStartDate;
                        raffle.RaffleEndDate = date.AddDays(endDate);
                    }
                }
            }

            raffle.RaffleStatus = GetStatusFromDate(raffle.RaffleStartDate);

            try
            {
                _context.Database.BeginTransaction();
                _context.PlataformServiceRaffle.Update(raffle);
                await _context.SaveChangesAsync();
                return Content("{\"success\": true, \"msg\": 'Atualizações salvas com sucesso!' }", "application/json");
            }
            catch (Exception)
            {
                _context.Database.RollbackTransaction();
                return Content("{\"success\": false, \"msg\": 'Erro ao atualizar os dados. Tente novamente mais tarde.' }", "application/json");
            }
        }

        public IActionResult AddNewRaffle()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            var hasPIX = new UserPixInformation();
            var hasBK = new UserBankInformation();

            var userBasicBag = new UserBasic();

            userBasicBag = _context.UserBasic.Where(e => e.Id == Convert.ToInt32(idUser))
                    .Select(u => new UserBasic()
                    {
                        Id = u.Id,
                        UserNumberBag = u.UserNumberBag,

                    }).FirstOrDefault();

            ViewBag.UserNumberBag = userBasicBag.UserNumberBag;

            hasPIX = _context.UserPixInformation.Where(e => e.IdBasicUser == Convert.ToInt32(idUser) && e.Removed == false)
                .Select(u => new UserPixInformation()
                {
                    Id = u.Id,

                }).FirstOrDefault();

            hasBK = _context.UserBankInformation.Where(e => e.IdBasicUser == Convert.ToInt32(idUser) && e.Removed == false)
                .Select(u => new UserBankInformation()
                {
                    Id = u.Id

                }).FirstOrDefault();


            if (hasPIX == null && hasBK == null)
            {
                ViewBag.alert = "PixBankAccountError";
            }

            ViewBag.userId = idUser;
            ViewBag.defaultURL = defaultUrlToRaffleDirectAccess; //Mudar para Prod

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewRaffle(IFormCollection form)
        {
            var UserIdSession = HttpContext.Session.GetInt32("UserLogId");

            if (UserIdSession == null || UserIdSession == 0)
            {
                return RedirectToAction("GetOutFromLogin", "Home");
            }

            PlatformServiceRaffle newRaffle = new PlatformServiceRaffle();

            newRaffle.IdBasicUser = Convert.ToInt32(form["IdBasicUser"]);

            var n = form["RaffleNumbersValue"].ToString().Replace(".", ",");
            var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            var culture = CultureInfo.CreateSpecificCulture("pt-BR");

            bool canConvert = decimal.TryParse(n, style, culture, out decimal number);
            if (canConvert)
                newRaffle.RaffleNumbersValue = number;

            newRaffle.RaffleGeneralDescription = form["RaffleGeneralDescription"];
            newRaffle.RafflePaymentIdAllowed = form["RafflePaymentIdAllowed"];
            newRaffle.RaffleName = form["RaffleName"];
            newRaffle.RafflePremiationDescription = form["RafflePremiationDescription"];
            newRaffle.RaffleMaxNumberLimited = Convert.ToInt32(form["RaffleMaxNumberLimited"]);
            newRaffle.RaffleUserMaxNumbersLimited = Convert.ToInt32(form["RaffleUserMaxNumbersLimited"]);
            newRaffle.BegginRaffleDay = Convert.ToInt32(form["BegginRaffleDay"]);
            newRaffle.EndRaffleDay = Convert.ToInt32(form["EndRaffleDay"]);
            newRaffle.RaffleWinnersNumber = Convert.ToInt32(form["RaffleWinnersNumber"]);
            newRaffle.RaffleType = Convert.ToInt32(form["RaffleType"]);

            if (newRaffle.RaffleType == 2)
                newRaffle.RaffleCloseOption = newRaffle.RaffleType == 2 ? true : false;
            else if(newRaffle.RaffleType == 3)
            {
                newRaffle.RaffleMaxNumberLimited = 100;
                newRaffle.RaffleWinnersNumber = 1;

            }
            //var value = form["RaffleCloseOption"];

            //newRaffle.RaffleCloseOption = value[0].Equals("true") ? true : false;

            if (newRaffle.RaffleCloseOption)
                newRaffle.RaffleUserMaxNumbersLimited = 1;

            if (newRaffle.BegginRaffleDay > 0)
                newRaffle.RaffleStartDate = DateTime.Now.ToLocalTime().AddDays(newRaffle.BegginRaffleDay);
            else
                newRaffle.RaffleStartDate = DateTime.Now.ToLocalTime();

            if (newRaffle.EndRaffleDay == 91)
                newRaffle.RaffleEndDate = DateTime.Parse("2016-10-01").Date;
            else
            {
                var date = newRaffle.RaffleStartDate;
                newRaffle.RaffleEndDate = date.AddDays(newRaffle.EndRaffleDay);
            }

            newRaffle.Identity = RandomToken.RandomString(10);
            newRaffle.IdPlatformService = (int)PlatformGeneralEnums.ServicesName.Raffle;
            newRaffle.RaffleStatus = GetStatusFromDate(newRaffle.RaffleStartDate);

            if (newRaffle.RaffleMaxNumberLimited > 0)
                newRaffle.RaffleMaxNumber = newRaffle.RaffleMaxNumberLimited;
            if (newRaffle.RaffleUserMaxNumbersLimited > 0)
                newRaffle.RaffleUserMaxNumbers = newRaffle.RaffleUserMaxNumbersLimited;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Database.BeginTransaction();

                    //Filtrar busca
                    var userUpdateBag = _context.UserBasic.Where(e=> e.Id == UserIdSession && !e.Removed).Select(e=> new UserBasic() {
                        Id = e.Id,
                        IdTelegram = e.IdTelegram,
                        UserNumberBag = e.UserNumberBag,
                    }).FirstOrDefault();

                    if(newRaffle.RaffleNumbersValue >= (decimal)2)
                        userUpdateBag.UserNumberBag -= newRaffle.RaffleMaxNumberLimited;
                    else
                        userUpdateBag.UserNumberBag -= (int)(newRaffle.RaffleMaxNumberLimited / 2);

                    if (userUpdateBag.UserNumberBag >= 0)
                    {
                        _context.Update(userUpdateBag);
                        _context.Attach(userUpdateBag).Property(e=> e.UserNumberBag).IsModified = true;
                        await _context.SaveChangesAsync();

                        var raffle = _context.Add(newRaffle);
                        await _context.SaveChangesAsync();

                        var raffleInfo = new PlatformServiceRaffleInformations()
                        {
                            IdRaffle = raffle.Entity.Id,
                            RaffleEasyLink = RandomToken.RandomString(30),
                            RaffleResultLink = "None",
                            RaffleParticipant = raffle.Entity.RaffleCloseOption ? "M" : "None"
                        };

                        _context.Add(raffleInfo);
                        await _context.SaveChangesAsync();

                        _context.Database.CommitTransaction();

                        //Readicionar item ao enviar requisição
                        ViewBag.alert = "successSave";
                        ViewBag.defaultUrl = defaultUrlToRaffleDirectAccess;
                        ViewBag.newRaffleCode = newRaffle.Identity;
                        ViewBag.UserNumberBag = userUpdateBag.UserNumberBag;
                        ViewBag.userId = UserIdSession;

                        var CreatedRaffle = new ToUserInformationsTelegram()
                        {
                            Action = "Novo Sorteio Criado",
                            RaffleName = raffle.Entity.RaffleName,
                            NumberQuantity = raffle.Entity.RaffleMaxNumber.ToString(),
                            RaffleLink = defaultUrlToRaffleDirectAccess + raffle.Entity.Identity,
                            RaffleCode = raffle.Entity.Identity,
                            RaffleNumberValue = raffle.Entity.RaffleNumbersValue.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")),
                            OcurrData = DateTime.UtcNow.AddHours(-3).ToString("dd/MM/yyyy"),
                            RewardName = raffle.Entity.RafflePremiationDescription
                        };

                        var t = new TelegramAction();
                        await t.SendDefaultMessageStructure(userUpdateBag.IdTelegram, CreatedRaffle, TelegramMessageAction.NewRaffleCreated);

                        return View();
                    }
                }
                catch (Exception e)
                {
                    _context.Database.RollbackTransaction();
                    Console.WriteLine(e);
                }
            }

            ViewBag.alert = "failSave";
            return View(newRaffle);
        }

        [HttpPost]
        public async Task<IActionResult> RaffleMarkPlaceBuyNumberMercadoPago([FromForm] BuyNumberPackeage pack)
        {
            var userId = HttpContext.Session.GetInt32("UserLogId");

            if (userId == 0 || userId == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            var payerDATA = _context.UserBasicInfo.Where(e => !e.Removed && e.IdBasicUser == userId).Select(e => new UserBasicInfo() { FullName = e.FullName, Email = e.Email }).FirstOrDefault();

            if (pack.Quantity >= 30)
            {
                var price = (decimal)0.5;

                if (pack.Quantity >= 50 && pack.Quantity <= 100)
                    price = (decimal)0.35;

                else if (pack.Quantity > 100 && pack.Quantity <= 400)
                    price = (decimal)0.30;

                else if (pack.Quantity > 400)
                    price = (decimal)0.25;


                decimal value = pack.Quantity * price;

                var token = _configuration["MercadoPago:ProductionToken"];
                //var token = _configuration["MercadoPago:DevelopToken"];

                MercadoPagoConfig.AccessToken = token; //"TEST-8592250525638227-050915-296a3a265b0b1c6ea02a852f947ed027-325628631";//  "YOUR_ACCESS_TOKEN";
                var reference = RandomToken.RandomString(25);

                var paymentMethods = new PreferencePaymentMethodsRequest
                {
                    ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                    {
                        new PreferencePaymentTypeRequest
                        {
                            Id = "ticket", //BOLETO
                        },
                        new PreferencePaymentTypeRequest
                        {
                            Id = "digital_currency", //GIFTCard
                        },
                        new PreferencePaymentTypeRequest
                        {
                            Id = "digital_wallet", //PayPal
                        },
                        //new PreferencePaymentTypeRequest
                        //{
                        //    Id = "bank_transfer", //PIX
                        //},
                    },
                    Installments = 12,
                };

                var payer = new PreferencePayerRequest()
                {
                    Email = payerDATA.Email,
                    Name = payerDATA.FullName,
                };

                try
                {
                    var request = new PreferenceRequest
                    {
                        Items = new List<PreferenceItemRequest>
                        {
                            new PreferenceItemRequest
                            {
                                Title = "Pacote de Números",
                                Quantity = pack.Quantity,
                                CurrencyId = "BRL",
                                UnitPrice = price,
                            },
                         },
                        ExternalReference = reference,
                        PaymentMethods = paymentMethods,

                        BackUrls = new PreferenceBackUrlsRequest
                        {
                            Success = _configuration["URLs:DefaultUrl"] + "PlataformServiceRaffle/RaffleMainPage?paymentAnswer=approved",
                            Failure = _configuration["URLs:DefaultUrl"] + "PlataformServiceRaffle/RaffleMainPage?paymentAnswer=failed",
                            Pending = _configuration["URLs:DefaultUrl"] + "PlataformServiceRaffle/RaffleMainPage?paymentAnswer=pending",
                        },
                        AutoReturn = "approved",
                        NotificationUrl = "https://missiersorteios.azurewebsites.net/paymentspace/ReceiveNotification",
                    };

                    var client = new PreferenceClient();
                    Preference preference = await client.CreateAsync(request);

                    if (preference != null)
                    {
                        UserPaymentRegister prePay = new UserPaymentRegister()
                        {
                            IdBasicUser = Convert.ToInt32(userId),
                            FinalStatus = "Start",
                            NumberQuantity = pack.Quantity,
                            Reference = reference,
                            TransactionType = 1,
                            ReferenceId = preference.Id,
                            TotalValue = pack.Quantity * price
                        };

                        try
                        {
                            _context.Database.BeginTransaction();
                            _context.UserPaymentRegister.Add(prePay);
                            await _context.SaveChangesAsync();

                            ViewBag.referenceId = preference.Id;
                            ViewBag.value = value;
                            ViewBag.quant = pack.Quantity;
                            ViewBag.userId = userId;

                            CultureInfo culture = new CultureInfo("pt-BR");
                            ViewBag.Culture = culture;

                            _context.Database.CommitTransaction();
                            return View();
                        }
                        catch (Exception e) { _context.Database.RollbackTransaction(); }
                    }
                }
                catch (Exception e) { }
            }

            var parameters = new Dictionary<string, string>() { { "paymentAnswer", "fail" } };

            return RedirectToAction("RaffleMainPage", parameters);
        }

        //private async Task<IActionResult> PaymentResultAction([FromQuery] PaymentResult pay)
        //{
        //    var paymentSystemResult = "failed";
        //    if (pay != null)
        //    {
        //        var register = _context.UserPaymentRegister.FirstOrDefault(e => e.Reference == pay.ExternalReference && e.Removed == false);

        //        if (register != null)
        //        {
        //            if (pay.Status == "approved")
        //            {
        //                var user = _context.UserBasic.FirstOrDefault(e => e.Id == register.IdBasicUser);

        //                if (user != null)
        //                {
        //                    user.UserNumberBag += register.NumberQuantity;
        //                    register.FinalStatus = pay.Status;
        //                    register.Removed = true;

        //                    try
        //                    {
        //                        _context.Database.BeginTransaction();

        //                        _context.UserPaymentRegister.Update(register);
        //                        await _context.SaveChangesAsync();

        //                        _context.UserBasic.Update(user);
        //                        await _context.SaveChangesAsync();

        //                        _context.Database.CommitTransaction();
        //                        paymentSystemResult = pay.Status;

        //                    }
        //                    catch (Exception) { _context.Database.RollbackTransaction(); }
        //                }
        //            }
        //            else if (pay.Status == "pending")
        //            {
        //                paymentSystemResult = pay.Status;
        //                register.FinalStatus = pay.Status;

        //                try
        //                {
        //                    _context.UserPaymentRegister.Update(register);
        //                    await _context.SaveChangesAsync();

        //                    paymentSystemResult = pay.Status;
        //                }
        //                catch (Exception) { }
        //            }
        //        }
        //    }

        //    var userId = HttpContext.Session.GetInt32("UserLogId");

        //    if (userId == 0 || userId == null)
        //        return RedirectToAction("GetOutFromLogin", "Home");

        //    var parameters = new Dictionary<string, string>() { { "paymentAnswer", paymentSystemResult } };

        //    return RedirectToAction("RaffleMainPage", parameters);
        //}

        public IActionResult ShowRaffleDetails(string raffleId, string errorMessage = "")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.alert = errorMessage;

            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;

            if (!String.IsNullOrEmpty(raffleId) && !raffleId.Equals("0"))
            {
                try
                {
                    var raffleInfo = _context.PlatformServiceRaffleInformations.Where(e => e.IdRaffle == Convert.ToInt32(raffleId))
                        .Select(u => new PlatformServiceRaffleInformations() {
                            Id = u.Id,
                            IdRaffle = u.IdRaffle,
                            RaffleParticipant = u.RaffleParticipant,

                        }).FirstOrDefault();

                    if (raffleInfo != null)
                    {
                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleInfo.IdRaffle)
                            .Select(u => new PlatformServiceRaffle() {
                                Id = u.Id,
                                IdBasicUser = u.IdBasicUser,
                                RaffleMaxNumber = u.RaffleMaxNumber,
                                RaffleCloseOption = u.RaffleCloseOption,
                                RaffleStatus = u.RaffleStatus,
                                RaffleName = u.RaffleName,
                                RaffleGeneralDescription = u.RaffleGeneralDescription,
                                RafflePremiationDescription = u.RafflePremiationDescription,
                                RaffleNumbersValue = u.RaffleNumbersValue,
                                RaffleUserMaxNumbers = u.RaffleUserMaxNumbers,

                            }).FirstOrDefault();
                        var userBasicInfo = _context.UserBasicInfo.FirstOrDefault(e => e.IdBasicUser == raffle.IdBasicUser);

                        if (!raffleInfo.RaffleParticipant.Equals("None") && !String.IsNullOrEmpty(raffleInfo.RaffleParticipant))
                        {
                            if (raffleInfo.RaffleParticipant.ElementAt(0) == 'M')
                            {
                                //Melhorar desempenho
                                var list = _context.PlatformGuestReservedNumber.Where(e => e.IdRaffle == raffleInfo.IdRaffle && !e.Removed)
                                    .Select(e=> new PlatformGuestReservedNumber() { Id = e.Id, Number = e.Number} ).ToList();

                                var avaliableNumber = raffle.RaffleMaxNumber;
                                var schaduleNumber = 0;
                                var unaviableNumber = list.Count();

                                avaliableNumber -= unaviableNumber;

                                var list2 = new List<NumberStatus>();
                                foreach (var item in list)
                                {
                                    var itemSave = new NumberStatus(item.Number, 3);
                                    list2.Add(itemSave);
                                }

                                if (userBasicInfo != null)
                                {
                                    var raffleNumberSelection = new NumberStatusTotal(new List<PlatformServiceRaffleFile>(),
                                        list2, raffle.RaffleMaxNumber, userBasicInfo, raffle, raffleInfo, new UserSocialMidia(), avaliableNumber, schaduleNumber, unaviableNumber);

                                    return View(raffleNumberSelection);
                                }
                            }
                            else
                            {
                                var raffleNumber = NumbersUserDescrypt.GetRaffleCurrentNumberSituation(raffleInfo.RaffleParticipant);

                                if (raffleNumber.Count > 0 && raffle != null)
                                {
                                    var avaliableNumber = raffle.RaffleMaxNumber;
                                    var schaduleNumber = 0;
                                    var unaviableNumber = 0;

                                    foreach (var number in raffleNumber)
                                    {
                                        if (number.Status == 2)
                                            schaduleNumber += 1;
                                        else
                                            unaviableNumber += 1;
                                    }

                                    avaliableNumber -= schaduleNumber + unaviableNumber;

                                    if (userBasicInfo != null)
                                    {
                                        var raffleNumberSelection = new NumberStatusTotal(new List<PlatformServiceRaffleFile>(),
                                            raffleNumber, raffle.RaffleMaxNumber, userBasicInfo, raffle, raffleInfo, new UserSocialMidia(), avaliableNumber, schaduleNumber, unaviableNumber);

                                        return View(raffleNumberSelection);
                                    }
                                }
                            }
                        }

                        return View(new NumberStatusTotal(new List<PlatformServiceRaffleFile>(), new List<NumberStatus>(), raffle.RaffleMaxNumber, userBasicInfo, raffle, raffleInfo, new UserSocialMidia(), raffle.RaffleMaxNumber));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            return View(new NumberStatusTotal());
        }

        public IActionResult ShowRaffleDetailsAmorin(string raffleId, string errorMessage = "")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.alert = errorMessage;

            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;

            if (!String.IsNullOrEmpty(raffleId) && !raffleId.Equals("0"))
            {
                try
                {
                    var raffleInfo = _context.PlatformServiceRaffleInformations.Where(e => e.IdRaffle == Convert.ToInt32(raffleId))
                        .Select(u => new PlatformServiceRaffleInformations()
                        {
                            Id = u.Id,
                            IdRaffle = u.IdRaffle,
                            RaffleParticipant = u.RaffleParticipant,

                        }).FirstOrDefault();

                    if (raffleInfo != null)
                    {
                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleInfo.IdRaffle)
                            .Select(u => new PlatformServiceRaffle()
                            {
                                Id = u.Id,
                                IdBasicUser = u.IdBasicUser,
                                RaffleMaxNumber = u.RaffleMaxNumber,
                                RaffleCloseOption = u.RaffleCloseOption,
                                RaffleStatus = u.RaffleStatus,
                                RaffleName = u.RaffleName,
                                RaffleGeneralDescription = u.RaffleGeneralDescription,
                                RafflePremiationDescription = u.RafflePremiationDescription,
                                RaffleNumbersValue = u.RaffleNumbersValue,
                                RaffleUserMaxNumbers = u.RaffleUserMaxNumbers,

                            }).FirstOrDefault();
                        var userBasicInfo = _context.UserBasicInfo.FirstOrDefault(e => e.IdBasicUser == raffle.IdBasicUser);

                        if (!raffleInfo.RaffleParticipant.Equals("None") && !String.IsNullOrEmpty(raffleInfo.RaffleParticipant))
                        {
                            if (raffleInfo.RaffleParticipant.ElementAt(0) == 'M')
                            {
                                //Melhorar desempenho
                                var list = _context.PlatformGuestReservedNumber.Where(e => e.IdRaffle == raffleInfo.IdRaffle && !e.Removed)
                                    .Select(e => new PlatformGuestReservedNumber() { Id = e.Id, Number = e.Number }).ToList();

                                var avaliableNumber = raffle.RaffleMaxNumber;
                                var schaduleNumber = 0;
                                var unaviableNumber = list.Count();

                                avaliableNumber -= unaviableNumber;

                                var list2 = new List<NumberStatus>();
                                foreach (var item in list)
                                {
                                    var itemSave = new NumberStatus(item.Number, 3);
                                    list2.Add(itemSave);
                                }

                                if (userBasicInfo != null)
                                {
                                    var raffleNumberSelection = new NumberStatusTotal(new List<PlatformServiceRaffleFile>(),
                                        list2, raffle.RaffleMaxNumber, userBasicInfo, raffle, raffleInfo, new UserSocialMidia(), avaliableNumber, schaduleNumber, unaviableNumber);

                                    return View(raffleNumberSelection);
                                }
                            }
                            else
                            {
                                var raffleNumber = NumbersUserDescrypt.GetRaffleCurrentNumberSituation(raffleInfo.RaffleParticipant);

                                if (raffleNumber.Count > 0 && raffle != null)
                                {
                                    var avaliableNumber = raffle.RaffleMaxNumber;
                                    var schaduleNumber = 0;
                                    var unaviableNumber = 0;

                                    foreach (var number in raffleNumber)
                                    {
                                        if (number.Status == 2)
                                            schaduleNumber += 1;
                                        else
                                            unaviableNumber += 1;
                                    }

                                    avaliableNumber -= schaduleNumber + unaviableNumber;

                                    if (userBasicInfo != null)
                                    {
                                        var raffleNumberSelection = new NumberStatusTotal(new List<PlatformServiceRaffleFile>(),
                                            raffleNumber, raffle.RaffleMaxNumber, userBasicInfo, raffle, raffleInfo, new UserSocialMidia(), avaliableNumber, schaduleNumber, unaviableNumber);

                                        return View(raffleNumberSelection);
                                    }
                                }
                            }
                        }

                        return View(new NumberStatusTotal(new List<PlatformServiceRaffleFile>(), new List<NumberStatus>(), raffle.RaffleMaxNumber, userBasicInfo, raffle, raffleInfo, new UserSocialMidia(), raffle.RaffleMaxNumber));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            return View(new NumberStatusTotal());
        }

        public ContentResult GetParticipantInformationByNumber([FromQuery] int raffleId, [FromQuery] int number)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return Content("{\"success\": false, \"msg\": 'Por favor atualize a página para continuar acessando os serviços.' }", "application/json");

            if (raffleId == 0 || number == 0)
                return Content("{\"success\": false, \"msg\": 'Houve um erro ao buscar os dados relacionados a este número.' }", "application/json");

            try
            {
                //Ver questão da reserva.
                var info = _context.PlatformUserReservedNumber.FirstOrDefault(e => e.IdRaffle == raffleId && e.Number == number);
                var userReceiptList = new List<PlatformServiceRaffleFile>();
                var userReceiptNumbers = "";
                int idUserBasic = 0;
                string numberToFront = "";

                if (info == null)
                {
                    userReceiptList = _context.PlatformServiceRaffleFile.Where(e => e.IdRaffle == raffleId && !e.Removed && e.NumberSequence.Contains(number.ToString()))
                    .Select(n => new PlatformServiceRaffleFile()
                    {
                        NumberSequence = n.NumberSequence,
                        IdBasicUser = n.IdBasicUser,
                        ReceiptFile = n.ReceiptFile,
                        ReceiptValue = n.ReceiptValue

                    }).ToList();
                }
                else
                {
                    idUserBasic = info.IdBasicUser;
                    numberToFront = number.ToString();
                }

                var receiptToFront = new PlatformServiceRaffleFile();

                if (userReceiptList.Count() > 0)
                {
                    foreach (var receipt in userReceiptList)
                    {
                        bool canStop = false; ;
                        var listNumber = receipt.NumberSequence.Split(',');

                        foreach (var numberChoose in listNumber)
                        {
                            if (number.ToString() == numberChoose)
                            {
                                idUserBasic = receipt.IdBasicUser;
                                receiptToFront = receipt;
                                numberToFront = receipt.NumberSequence;
                                canStop = true;
                                break;
                            }
                        }

                        if (canStop)
                            break;
                    }


                }

                if (idUserBasic == 0)
                    return Content("{\"success\": false, \"msg\": '" + "Não encontramos dados relacionados a este número, tente atualizar a página" + "'}", "application/json");

                var participantData = _context.UserBasicInfo.FirstOrDefault(e => e.IdBasicUser == idUserBasic);

                if (participantData == null)
                    return Content("{\"success\": false, \"msg\": '" + "Não encontramos dados relacionados ao participante, se o erro persistir, considere entrar em contato conosco." + "'}", "application/json");

                //var r = new { NickName = participantData.NickName, Number = numberToFront, File = receiptToFront.ReceiptFile, Value = receiptToFront.ReceiptValue };
                var JsonAnswer = "\"NickName\": \"" + participantData.NickName + "\", \"Number\":\"" + number + "\", \"File\":\"" + receiptToFront.ReceiptFile + "\", \"Numbers\":\"" + numberToFront + "\", \"Value\":\"" + receiptToFront.ReceiptValue + "\"";

                return Content("{\"success\":true," + JsonAnswer + "}", "application/json");
            }
            catch (Exception)
            {
                return Content("{\"success\": false, \"msg\": '" + "Houve um erro inesperado, tente novamente mais tarde." + "'}", "application/json");
            }
        }

        public ContentResult GetParticipantInformationByNumberRaffleManual([FromQuery] int raffleId, [FromQuery] int number)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return Content("{\"success\": false, \"msg\": 'Por favor atualize a página para continuar acessando os serviços.' }", "application/json");

            if (raffleId == 0 || number == 0)
                return Content("{\"success\": false, \"msg\": 'Houve um erro ao buscar os dados relacionados a este número.' }", "application/json");

            try
            {
                var info = _context.PlatformGuestReservedNumber.FirstOrDefault(e => e.IdRaffle == raffleId && e.Number == number);

                if (info == null)
                    return Content("{\"success\": false, \"msg\": '" + "Não encontramos dados relacionados a este número, tente atualizar a página" + "'}", "application/json");

                var r = new { NickName = info.FullName, Number = info.Number };
                var JsonAnswer = "\"NickName\": \"" + info.FullName + "\", \"Number\":\"" + info.Number + "\"";

                return Content("{\"success\":true," + JsonAnswer + "}", "application/json");
            }
            catch (Exception)
            {
                return Content("{\"success\": false, \"msg\": '" + "Houve um erro inesperado, tente novamente mais tarde." + "'}", "application/json");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RaffleConfirmNumberSelled(IFormCollection form)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            int number = Convert.ToInt32(form["NumberBought"]);
            //int participantId = Convert.ToInt32(form["participantId"]);

            int raffleId = Convert.ToInt32(form["RaffleId"]);
            int isAproved = Convert.ToInt32(form["aprovedValue"]);
            string Identity = (form["Identity"]);
            var typeR = 1;
            if (number > 0 && raffleId > 0)
            {
                PlatformServiceRaffleInformations raffleInfo = new PlatformServiceRaffleInformations();

                try
                {
                    raffleInfo = _context.PlatformServiceRaffleInformations.FirstOrDefault(e => e.IdRaffle == raffleId && !e.Removed);
                }
                catch (Exception)
                {
                    var parameterFi = new Dictionary<string, string>()
                                {
                                    {"raffleId", raffleId.ToString() },
                                    {"errorMessage","fail" }
                                };
                    return RedirectToAction("ShowRaffleDetails", parameterFi);
                }

                if (raffleInfo != null)
                {
                    if (!String.IsNullOrEmpty(Identity) && raffleInfo.RaffleParticipant == "M")
                    {
                        PlatformGuestReservedNumber guest = new PlatformGuestReservedNumber()
                        {
                            BeginningDate = DateTime.Now,
                            FullName = Identity,
                            Identity = RandomToken.RandomString(10),
                            IdRaffle = raffleId,
                            Number = number,
                            Removed = false,
                        };

                        try
                        {
                            _context.PlatformGuestReservedNumber.Add(guest);
                            await _context.SaveChangesAsync();

                            var parameterS = new Dictionary<string, string>()
                                    {
                                        {"raffleId", raffleId.ToString() },
                                        {"errorMessage","success" }
                                    };

                            return RedirectToAction("ShowRaffleDetails", parameterS);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    else
                    {
                        var raffleDecriptObj = NumbersUserDescrypt.CreateRuffleDecryptObjectByEncode(raffleInfo.RaffleParticipant, raffleInfo.IdRaffle);

                        if (raffleDecriptObj.RaffleDataComponentFromEncode.Count() > 0)
                        {
                            try
                            {
                                _context.Database.BeginTransaction();

                                //Revisar se é necessaario ainda.
                                //_context.PlatformUserReservedNumber.Remove(reservation);
                                //await _context.SaveChangesAsync();

                                //Alterar/Aprovar informações do comprovante

                                var userReceiptList = _context.PlatformServiceRaffleFile.Where(e => e.IdRaffle == raffleId && !e.Removed && e.NumberSequence.Contains(number.ToString()))
                                .Select(n => new PlatformServiceRaffleFile()
                                {
                                    Id = n.Id,
                                    NumberSequence = n.NumberSequence,
                                    IdBasicUser = n.IdBasicUser,
                                    ReceiptFile = n.ReceiptFile,
                                    ReceiptValue = n.ReceiptValue

                                }).ToList();

                                if (userReceiptList.Count() > 0)
                                {
                                    var receiptFromFront = new PlatformServiceRaffleFile();

                                    foreach (var receipt in userReceiptList)
                                    {
                                        bool canStop = false; ;
                                        var listNumber = receipt.NumberSequence.Split(',');

                                        foreach (var numberChoose in listNumber)
                                        {
                                            if (number.ToString() == numberChoose)
                                            {
                                                receiptFromFront = receipt;
                                                canStop = true;
                                                break;
                                            }
                                        }

                                        if (canStop)
                                            break;
                                    }

                                    if (receiptFromFront.Id > 0)
                                    {
                                        //Fazer numero retornar pra ocupado;

                                        receiptFromFront.Aproved = isAproved == 1 ? true : false;

                                        if (!receiptFromFront.Aproved)
                                            receiptFromFront.PreAproved = true;

                                        _context.PlatformServiceRaffleFile.Update(receiptFromFront);
                                        _context.Attach(receiptFromFront).Property(e => e.Aproved).IsModified = true;
                                        await _context.SaveChangesAsync();

                                        var listNumber = receiptFromFront.NumberSequence.Split(',');

                                        foreach (var numberToAprove in listNumber)
                                        {
                                            if (raffleDecriptObj.RaffleDataComponentFromEncode.Count() > 0)
                                            {
                                                foreach (var participantPersonalData in raffleDecriptObj.RaffleDataComponentFromEncode)
                                                {
                                                    foreach (var participantNumberData in participantPersonalData.Numbers)
                                                    {
                                                        if (participantNumberData.Number == Convert.ToInt32(numberToAprove))
                                                        {
                                                            participantNumberData.Status = 3;
                                                            //participantId = participantPersonalData.ParticipantId;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var newDecode = NumbersUserDescrypt.CreateRaffleDecodeStringFromDecryptObject(raffleDecriptObj);

                                        raffleInfo.RaffleParticipant = newDecode;

                                        _context.PlatformServiceRaffleInformations.Update(raffleInfo);
                                        await _context.SaveChangesAsync();

                                        _context.Database.CommitTransaction();

                                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleId).Select(e => new PlatformServiceRaffle()
                                        {
                                            Id = e.Id,
                                            RaffleName = e.RaffleName,
                                            Identity = e.Identity,
                                            RaffleType = e.RaffleType

                                        }).FirstOrDefault();

                                        var participant = _context.UserBasic.Where(e => e.Id == receiptFromFront.IdBasicUser).Select(e => new UserBasic() { IdTelegram = e.IdTelegram }).FirstOrDefault();
                                        typeR = raffle.RaffleType;

                                        if (raffle != null && participant != null)
                                        {
                                            var ApproveReceiptRaffle = new ToUserInformationsTelegram()
                                            {
                                                Action = "Pagamento Confirmado (ACEITO)",
                                                RaffleName = raffle.RaffleName,
                                                NumberReference = receiptFromFront.NumberSequence,
                                                RaffleCode = raffle.Identity,
                                                ValueOperation = receiptFromFront.ReceiptValue.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")),
                                                OcurrData = DateTime.UtcNow.AddHours(-3).ToString("dd/MM/yyyy"),
                                            };

                                            var t = new TelegramAction();
                                            await t.SendDefaultMessageStructure(participant.IdTelegram, ApproveReceiptRaffle, TelegramMessageAction.aNumberHasBeenApproved);
                                        }

                                        var parameterS = new Dictionary<string, string>()
                                        {
                                            {"raffleId", raffleId.ToString() },
                                            {"errorMessage","success" }
                                        };

                                        if (raffle.RaffleType == 3)
                                            return RedirectToAction("ShowRaffleDetailsAmorin", parameterS);
                                        else
                                            return RedirectToAction("ShowRaffleDetails", parameterS);

                                    }

                                }
                                else
                                {
                                    _context.Database.RollbackTransaction();
                                    //Erro
                                }

                                _context.Database.RollbackTransaction();

                            }
                            catch (Exception e) { _context.Database.RollbackTransaction(); }

                        }
                    }
                }
            }

            var parameterF = new Dictionary<string, string>()
                                {
                                    {"raffleId", raffleId.ToString() },
                                    {"errorMessage","fail" }
                                };

            if (typeR == 3)
                return RedirectToAction("ShowRaffleDetailsAmorin", parameterF);
            else
                return RedirectToAction("ShowRaffleDetails", parameterF);
        }

        [HttpPost]
        public async Task<IActionResult> RaffleRefuseNumberSelled(IFormCollection form)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            int number = Convert.ToInt32(form["NumberBoughtRefuse"]);
            //int participantId = Convert.ToInt32(form["participantIdRefuse"]);
            int raffleId = Convert.ToInt32(form["RaffleId"]);
            int isAproved = Convert.ToInt32(form["aprovedValue"]);
            string Identity = (form["Identity"]);
            var typeR = 1;

            if (number > 0 && raffleId > 0)
            {
                PlatformServiceRaffleInformations raffleInfo = new PlatformServiceRaffleInformations();

                try
                {
                    raffleInfo = _context.PlatformServiceRaffleInformations.FirstOrDefault(e => e.IdRaffle == raffleId && !e.Removed);
                }
                catch (Exception)
                {
                    var parameterFi = new Dictionary<string, string>()
                                {
                                    {"raffleId", raffleId.ToString() },
                                    {"errorMessage","fail" }
                                };
                    return RedirectToAction("ShowRaffleDetails", parameterFi);
                }

                if(raffleInfo != null)
                {
                   // var raffleDecriptObj = NumbersUserDescrypt.CreateRuffleDecryptObjectByEncode(raffleInfo.RaffleParticipant, raffleInfo.IdRaffle);

                    var isReservedNumberObject = _context.PlatformUserReservedNumber.FirstOrDefault(e => e.IdRaffle == raffleId && e.Number == number);
                    var userReceiptList = new List<PlatformServiceRaffleFile>();
                    int idUserBasic = 0;
                    string numberFromFront = "";

                    if (isReservedNumberObject == null)
                    {
                        //Foi enviado o comprovante - está em pré-aprovado
                        userReceiptList = _context.PlatformServiceRaffleFile.Where(e => e.IdRaffle == raffleId && !e.Removed && e.NumberSequence.Contains(number.ToString()))
                        .Select(n => new PlatformServiceRaffleFile()
                        {
                            Id = n.Id,
                            NumberSequence = n.NumberSequence,
                            IdBasicUser = n.IdBasicUser,
                            ReceiptFile = n.ReceiptFile,
                            ReceiptValue = n.ReceiptValue

                        }).ToList();
                    }
                    else
                    {
                        //Não foi enviado comprovante - Então não está em pre-aprovado
                        idUserBasic = isReservedNumberObject.IdBasicUser;
                        numberFromFront = number.ToString();
                    }

                    var receiptFromFront = new PlatformServiceRaffleFile();

                    if (userReceiptList.Count() > 0)
                    {
                        foreach (var receipt in userReceiptList)
                        {
                            bool canStop = false; ;
                            var listNumber = receipt.NumberSequence.Split(',');

                            foreach (var numberChoose in listNumber)
                            {
                                if (number.ToString() == numberChoose)
                                {
                                    idUserBasic = receipt.IdBasicUser;
                                    receiptFromFront = receipt;
                                    numberFromFront = receipt.NumberSequence;
                                    canStop = true;
                                    break;
                                }
                            }

                            if (canStop)
                                break;
                        }
                    }

                    //Verificando se existe um objeto contendo dados de um comprovante;
                    if (receiptFromFront.Id > 0)
                    {
                        //Fazer numero retornar pra ocupado;
                        await _context.Database.BeginTransactionAsync();

                        receiptFromFront.Aproved = isAproved == 1 ? true : false;

                        if (!receiptFromFront.Aproved)
                            receiptFromFront.PreAproved = false;

                        receiptFromFront.Removed = true;

                        _context.PlatformServiceRaffleFile.Update(receiptFromFront);
                        _context.Attach(receiptFromFront).Property(x => x.Aproved).IsModified = true;
                        _context.Attach(receiptFromFront).Property(x => x.PreAproved).IsModified = true;
                        _context.Attach(receiptFromFront).Property(x => x.Removed).IsModified = true;
                        await _context.SaveChangesAsync();

                        var listNumber = receiptFromFront.NumberSequence.Split(',');

                        try
                        {
                            foreach (var numberToBackAgain in listNumber)
                            {
                                var reserved = new PlatformUserReservedNumber()
                                {
                                    IdBasicUser = idUserBasic,
                                    IdRaffle = raffleId,
                                    Number = Convert.ToInt32(numberToBackAgain),
                                    BeginningDate = DateTime.Now,
                                    Removed = false
                                };

                                _context.PlatformUserReservedNumber.Add(reserved);
                                await _context.SaveChangesAsync();
                            }

                            _context.Database.CommitTransaction();

                            var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleId).Select(e => new PlatformServiceRaffle()
                            { 
                                Id = e.Id,
                                RaffleName = e.RaffleName,
                                Identity = e.Identity,

                            }).FirstOrDefault();
                            var participant = _context.UserBasic.Where(e => e.Id == idUserBasic).Select(e => new UserBasic() { IdTelegram = e.IdTelegram }).FirstOrDefault();
                            typeR = raffle.RaffleType;

                            if (raffle != null && participant != null)
                            {
                                var RefuseReceiptRaffle = new ToUserInformationsTelegram()
                                {
                                    Action = "Pagamento Inválido (RECUSADO)",
                                    RaffleName = raffle.RaffleName,
                                    NumberReference = receiptFromFront.NumberSequence,
                                    RaffleCode = raffle.Identity,
                                    ValueOperation = receiptFromFront.ReceiptValue.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")),
                                    OcurrData = DateTime.UtcNow.AddHours(-3).ToString("dd/MM/yyyy"),
                                };

                                var t = new TelegramAction();
                                await t.SendDefaultMessageStructure(participant.IdTelegram, RefuseReceiptRaffle, TelegramMessageAction.aNumberHasBeenRefused);
                            }

                            var parameterS = new Dictionary<string, string>()
                                        {
                                            {"raffleId", raffleId.ToString() },
                                            {"errorMessage","success" }
                                        };

                            if (raffle.RaffleType == 3)
                                return RedirectToAction("ShowRaffleDetailsAmorin", parameterS);
                            else
                                return RedirectToAction("ShowRaffleDetails", parameterS);
                        }
                        catch (Exception e) { _context.Database.RollbackTransaction(); }

                    }
                    else
                    {
                        //Não havendo comprovante o numero retorna a está disponivel
                        await _context.Database.BeginTransactionAsync();

                        isReservedNumberObject.Removed = true;
                        _context.PlatformUserReservedNumber.Update(isReservedNumberObject);

                        try
                        {
                            await _context.SaveChangesAsync();

                            var newCode = RaffleDataSerachAndUpdate(raffleInfo.RaffleParticipant, raffleInfo.IdRaffle, number);

                            if (raffleInfo.RaffleParticipant != newCode)
                            {
                                raffleInfo.RaffleParticipant = newCode;
                                _context.PlatformServiceRaffleInformations.Update(raffleInfo);
                                _context.SaveChanges();

                                _context.Database.CommitTransaction();

                                var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleId).Select(e => new PlatformServiceRaffle()
                                {
                                    Id = e.Id,
                                    RaffleName = e.RaffleName,
                                    Identity = e.Identity,
                                    RaffleType = e.RaffleType

                                }).FirstOrDefault();

                                var participant = _context.UserBasic.Where(e => e.Id == idUserBasic).Select(e => new UserBasic() { IdTelegram = e.IdTelegram }).FirstOrDefault();

                                if (raffle != null && participant != null)
                                {
                                    var RefuseReceiptRaffle = new ToUserInformationsTelegram()
                                    {
                                        Action = "Reserva Retirada Pelo Criador (RECUSADO)",
                                        RaffleName = raffle.RaffleName,
                                        NumberReference = receiptFromFront.NumberSequence,
                                        RaffleCode = raffle.Identity,
                                        ValueOperation = receiptFromFront.ReceiptValue.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")),
                                        OcurrData = DateTime.UtcNow.AddHours(-3).ToString("dd/MM/yyyy"),
                                    };

                                    var t = new TelegramAction();
                                    await t.SendDefaultMessageStructure(participant.IdTelegram, RefuseReceiptRaffle, TelegramMessageAction.aNumberHasBeenManualOutReserved);
                                }

                                var parameterS = new Dictionary<string, string>()
                                        {
                                            {"raffleId", raffleId.ToString() },
                                            {"errorMessage","success" }
                                        };

                                if (raffle.RaffleType == 3)
                                    return RedirectToAction("ShowRaffleDetailsAmorin", parameterS);
                                else
                                    return RedirectToAction("ShowRaffleDetails", parameterS);

                            }

                        }
                        catch (Exception e) { _context.Database.RollbackTransaction(); }

                       
                    }

                }

            }

            var parameterF = new Dictionary<string, string>()
                                {
                                    {"raffleId", raffleId.ToString() },
                                    {"errorMessage","fail" }
                                };

            if (typeR == 3)
                return RedirectToAction("ShowRaffleDetailsAmorin", parameterF);
            else
                return RedirectToAction("ShowRaffleDetails", parameterF);
        }

        private string RaffleDataSerachAndUpdate(string raffleParticipantCode, int raffleId, int number)
        {
            var raffleDecriptObj = NumbersUserDescrypt.CreateRuffleDecryptObjectByEncode(raffleParticipantCode, raffleId);

            var canBrak = false;
            int? raiz = null;
            int? raiz2 = null;
            if (raffleDecriptObj.RaffleDataComponentFromEncode.Count() > 0)
            {
                foreach (var participantPersonalData in raffleDecriptObj.RaffleDataComponentFromEncode)
                {
                    foreach (var participantNumberData in participantPersonalData.Numbers)
                    {
                        if (participantNumberData.Number == number)
                        {
                            raiz = raffleDecriptObj.RaffleDataComponentFromEncode.IndexOf(participantPersonalData);
                            raiz2 = raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers.IndexOf(participantNumberData);

                            canBrak = true;
                            break;
                        }
                    }

                    if (canBrak)
                        break;
                }

                if (raiz != null && raiz2 != null)
                {
                    var t = raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers
                        .Remove(raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers[Convert.ToInt32(raiz2)]);

                    if (raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers.Count <= 0)
                        raffleDecriptObj.RaffleDataComponentFromEncode.Remove(raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)]);
                }

                var newDecode = NumbersUserDescrypt.CreateRaffleDecodeStringFromDecryptObject(raffleDecriptObj);
                return newDecode;
            }

            return null;
        }

        public async Task<IActionResult> ToRaffleResult ([FromQuery]int raffleId, [FromQuery] int quantity=1 )
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            if (raffleId == 0)
                return RedirectToAction("RaffleMainPage");

            //Verificar se o sortear já foi clicado em multiplas páginas
            var raffleInfo = _context.PlatformServiceRaffleInformations.Where(e => e.IdRaffle == raffleId && !e.Removed)
                .Select(e=> new PlatformServiceRaffleInformations() {Id = e.Id, RaffleParticipant = e.RaffleParticipant }).FirstOrDefault();
            var updateRaffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleId && !e.Removed)
                                        .Select(e => new PlatformServiceRaffle()
                                        {
                                            Id = e.Id,
                                            Identity = e.Identity,
                                            RaffleName = e.RaffleName,
                                            RafflePremiationDescription = e.RafflePremiationDescription,
                                            RaffleWinnersNumber = e.RaffleWinnersNumber,
                                        })
                                        .FirstOrDefault();
            if (raffleInfo!= null)
            {
                quantity = updateRaffle.RaffleWinnersNumber;

                if (raffleInfo.RaffleParticipant.ElementAt(0) == 'M')
                {
                    //Melhorar desempenho
                    var list = _context.PlatformGuestReservedNumber.Where(e => e.IdRaffle == raffleId && !e.Removed).ToList();

                    var raffleNumber = new List<NumberStatus>();
                    foreach (var item in list)
                    {
                        var itemSave = new NumberStatus(item.Number, 3);
                        raffleNumber.Add(itemSave);
                    }

                    if (raffleNumber.Count() >= quantity)
                    {
                        var result = new List<NumberStatus>();
                        var randomNumbers = new List<int>();

                        Random r = new Random(0);

                        for (int i = 0; i < quantity; i++)
                        {
                            var randomIndex = r.Next(raffleNumber.Count() - 1);

                            bool repeat = false;
                            foreach (var results in randomNumbers)
                            {
                                if (results == randomIndex)
                                    repeat = true;
                            }

                            if (repeat)
                                i -= 1;
                            else
                            {
                                randomNumbers.Add(randomIndex);
                                result.Add(raffleNumber[randomIndex]);
                            }
                        }

                        if (result.Count() == quantity)
                        {
                            var toEncode = "";

                            foreach (var re in result)
                            {
                                if (result.Count() > 1)
                                    toEncode += re.Number.ToString() + ":";
                                else
                                    toEncode += re.Number.ToString();
                            }

                            try
                            {
                                if (updateRaffle != null)
                                {
                                    updateRaffle.RaffleNumberResult = toEncode;

                                    _context.PlataformServiceRaffle.Update(updateRaffle);
                                    _context.PlataformServiceRaffle.Attach(updateRaffle).Property(e => e.RaffleNumberResult).IsModified = true;

                                    await _context.SaveChangesAsync();
                                    return RedirectToAction("RaffleList", new { type = "3" });
                                }
                            }
                            catch (Exception) { }
                        }
                    }

                }
                else
                {
                    if (String.IsNullOrEmpty(raffleInfo.RaffleParticipant) || raffleInfo.RaffleParticipant != "None")
                    {
                        var raffleNumber = NumbersUserDescrypt.GetRaffleCurrentNumberSituation(raffleInfo.RaffleParticipant);
                        var raffleNumberUserData = NumbersUserDescrypt.GetRaffleCurrentRaffleUserData(raffleInfo.RaffleParticipant);

                        if (raffleNumber.Count() >= quantity)
                        {
                            var result = new List<NumberStatus>();
                            var randomNumbers = new List<int>();

                            Random r = new Random(0);

                            for (int i = 0; i < quantity; i++)
                            {
                                var randomIndex = r.Next(raffleNumber.Count() - 1);

                                bool repeat = false;
                                foreach (var results in randomNumbers)
                                {
                                    if (results == randomIndex)
                                        repeat = true;
                                }

                                if (repeat)
                                    i -= 1;
                                else
                                {
                                    randomNumbers.Add(randomIndex);
                                    result.Add(raffleNumber[randomIndex]);
                                }
                            }

                            if (result.Count() == quantity)
                            {
                                var toEncode = "";

                                foreach (var re in result)
                                {
                                    if (result.Count() > 1)
                                        toEncode += re.Number.ToString() + ":";
                                    else
                                        toEncode += re.Number.ToString();
                                }

                                try
                                {

                                    if (updateRaffle != null)
                                    {
                                        updateRaffle.RaffleNumberResult = toEncode;

                                        _context.PlataformServiceRaffle.Update(updateRaffle);
                                        _context.PlataformServiceRaffle.Attach(updateRaffle).Property(e => e.RaffleNumberResult).IsModified = true;
                                        await _context.SaveChangesAsync();

                                        var t = new TelegramAction();

                                        foreach (var re in result)
                                        {
                                            foreach(var userData in raffleNumberUserData)
                                            {
                                                var number = userData.Numbers.Where(e => e == re.Number).FirstOrDefault();

                                                if(number != 0)
                                                {
                                                    var userToSendMessage = _context.UserBasic.Where(e => e.Id == userData.User.Id)
                                                        .Select(e => new UserBasic() { Id = e.Id, IdTelegram = e.IdTelegram }).FirstOrDefault();

                                                    if (!String.IsNullOrEmpty(userToSendMessage.IdTelegram))
                                                    {
                                                        var CongratulationRaffle = new ToUserInformationsTelegram()
                                                        {
                                                            Action = "Parabéns, você foi sorteado!",
                                                            RaffleName = updateRaffle.RaffleName,
                                                            RewardName = updateRaffle.RafflePremiationDescription,
                                                            RaffleCode = updateRaffle.Identity,
                                                            NumberReference = re.Number.ToString(),
                                                            OcurrData = DateTime.UtcNow.AddHours(-3).ToString("dd/MM/yyyy"),
                                                        };

                                                        await t.SendDefaultMessageStructure(userToSendMessage.IdTelegram, CongratulationRaffle, TelegramMessageAction.RaffleWinners);
                                                    }

                                                    break;
                                                }
                                            }                                           
                                        }

                                        return RedirectToAction("RaffleList", new { type = "3" });
                                    }
                                }
                                catch (Exception e) { }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private int GetStatusFromDate(DateTime initial)
        {
            if (initial.Date.CompareTo(DateTime.Now.Date) == 0)
                return (int)RaffleEnums.Status.Iniciado;

            return (int)RaffleEnums.Status.Agendado;
        }

        public async Task<IActionResult> ToStartRaffle([FromQuery] int raffleId)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            if (raffleId == 0)
                return RedirectToAction("RaffleList", new { type = "2" });

            var raffle = await _context.PlataformServiceRaffle.FindAsync(raffleId);
            if (raffle != null)
            {
                raffle.RaffleStartDate = DateTime.Now;
                raffle.RaffleStatus = 1;
                _context.Update(raffle);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception) { }
            }

            return RedirectToAction("RaffleList", new { type = "2" });
        }

        public async Task<IActionResult> ToEndRaffle([FromQuery] int raffleId)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            if (raffleId == 0)
                return RedirectToAction("RaffleList", new { type = "1" });

            var raffle = await _context.PlataformServiceRaffle.FindAsync(raffleId);
            var oldStatus = raffle.RaffleStatus;
            if (raffle != null)
            {
                raffle.RaffleStatus = 3;
                raffle.RaffleEndDate = DateTime.Now;
                // _context.Database.BeginTransaction();
                _context.PlataformServiceRaffle.Update(raffle);
                try
                {
                    await _context.SaveChangesAsync();
                    //if(oldStatus != 2)
                    //{
                    //    var list = _context.PlatformUserReservedNumber.Where(e => e.IdRaffle == raffleId).ToList();

                    //    if (list != null)
                    //        _context.PlatformUserReservedNumber.RemoveRange(list);
                    //}

                    //await _context.SaveChangesAsync();
                    //_context.Database.CommitTransaction();

                }
                catch (Exception) { _context.Database.RollbackTransaction(); }
            }

            return RedirectToAction("RaffleList", new { type = "1" });
        }
        #endregion

        #region FormValidations
        public IActionResult VerifyDateConcordancy([FromQuery(Name = "RaffleStartDate")]DateTime RaffleStartDate, [FromQuery(Name = "RaffleEndDate")] DateTime RaffleEndDate)
        {
            if (RaffleStartDate.Date.CompareTo(RaffleEndDate) < 0)
                return Json($"A Data de finalização não pode ser menor que a de início!");

            if (RaffleStartDate.Date.CompareTo(RaffleEndDate) > 0)
                return Json($"A Data de início não pode ser maior que a de finalização!");

            return Json(true);
        }

        public IActionResult VerifyDateConcordancy(string nome)
        {
            var v = _context.PlataformServiceRaffle.FirstOrDefault(e => e.RaffleName.Equals(nome)).Id;

            if (v != 0)
                return Json($"Este nome de sorteio já está em uso.");

            return Json(true);
        }

        public IActionResult VerifyRaffleMaxNumber([FromQuery(Name = "RaffleMaxNumberLimited")] int RaffleMaxNumberLimited, int RaffleWinnersNumber)
        {
            var userId = HttpContext.Session.GetInt32("UserLogId");

            if (userId == null || userId == 0)
                return Json($"Não conseguimos verificar seu cofre de números, atualize a página.");
            else
            {
                if (RaffleMaxNumberLimited <= 0)
                    return Json($"O valor precisa ser maior que 0");

                try
                {
                    var userBagNumber = _context.UserBasic.Find(userId).UserNumberBag;

                    if (userBagNumber >= RaffleMaxNumberLimited)
                        return Json(true);
                    else
                        return Json($"Você não possui números suficientes em seu cofre. Compre mais logo acima.");

                }
                catch (Exception) { return Json($"Houve um erro ao verificar seu registro, tente mais tarde."); } 
            }
        }

        public IActionResult VerifyRaffleMaxUserNumber([FromQuery(Name = "RaffleUserMaxNumbersLimited")] int RaffleUserMaxNumbersLimited, [FromQuery(Name = "CreateNewRaffle.RaffleMaxNumberLimited")] int RaffleMaxNumberLimited)
        {
            var userId = HttpContext.Session.GetInt32("UserLogId");

            if (userId == null || userId == 0)
            {
                return Json($"Não conseguimos verificar seu cofre de números, tente novamente mais tarde.");
            }
            else
            {
                if (RaffleMaxNumberLimited <= 0)
                    return Json($"O valor precisa ser maior que 0");

                try
                {
                    var userBagNumber = _context.UserBasic.Find(userId).UserNumberBag;

                    if (userBagNumber >= RaffleMaxNumberLimited)
                        return Json(true);
                    else
                        return Json($"Você não possue números suficientes em seu cofre.");
                }
                catch (Exception) { return Json($"Houve um erro ao verificar seu registro, tente mais tarde."); }
            }
        }

        public IActionResult VerifyNumberQuantityParticipant(int RaffleUserMaxNumbersLimited, int RaffleMaxNumberLimited)
        {
            var userId = HttpContext.Session.GetInt32("UserLogId");

            if (userId == null || userId == 0)
                return Json($"Não conseguimos validar este campo, tente novamente mais tarde.");

            else
            {
                if (RaffleUserMaxNumbersLimited <= 0)
                    return Json("O valor precisa ser maior que 0");

                if (RaffleMaxNumberLimited < 10)
                    return Json("Aguardando para validação.");

                if (((decimal)(RaffleMaxNumberLimited / 2)) < (decimal)RaffleUserMaxNumbersLimited)
                    return Json($"Limite de até 50% do total máximo de números do sorteio.");

            }
            return Json(true);
        }

        public IActionResult VerifyMaxWinners(int RaffleWinnersNumber, int RaffleMaxNumberLimited)
        {
            var userId = HttpContext.Session.GetInt32("UserLogId");

            if (userId == null || userId == 0)
                return Json($"Não conseguimos validar este campo, tente novamente mais tarde.");

            else
            {
                if (RaffleWinnersNumber <= 0)
                    return Json("O valor precisa ser maior que 0");

                if (RaffleMaxNumberLimited < 10)
                    return Json("Aguardando para validação.");

                if (RaffleWinnersNumber > RaffleMaxNumberLimited )
                    return Json($"Valor inválido para este sorteio.");

            }

            return Json(true);
        }


        #endregion

    }
}
