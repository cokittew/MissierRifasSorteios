using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MissierSystem.DataContext;
using MissierSystem.Models;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using MissierSystem.Service.Automatization;
using MissierSystem.Service.MercadoPago;
using MissierSystem.Service.PlatformServices.Raffle;
using MissierSystem.Service.Telegram;
using MissierSystem.Service.TokenServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MissierSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            TelegramAction bot = new TelegramAction();
            //var b = bot.SendDefaultMessage("").Result;

            var idUser = HttpContext.Session.GetInt32("UserLogId"); ;

            if (idUser != null && idUser != 0)
                return RedirectToAction("MainPageParticipant", "Home");

            return View();
        }
        public IActionResult MainPageParticipant()
        {
            var idUser = HttpContext.Session.GetInt32("UserLogId"); ;

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            return View();
        }

        public IActionResult GetOutFromLogin()
        {
            HttpContext.Session.SetInt32("UserLogId", 0);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
