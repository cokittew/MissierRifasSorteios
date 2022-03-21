using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.TonModality;
using MissierSystem.Service.HelperServices;
using MissierSystem.Service.TokenServices;

namespace MissierSystem.Controllers
{
    public class CollaboratorsController : Controller
    {
        private readonly UserClienteDataContext _context;

        public CollaboratorsController(UserClienteDataContext context)
        {
            _context = context;
        }

        private int? IsAutenticated(bool admin = true)
        {
            var id = HttpContext.Session.GetInt32("UserLogId");
            if (id == 0 || id == null || id != 1)
                return 0;

            var user = _context.UserBasicInfo.Where(e => e.IdBasicUser == id)
                .Select(e => new UserBasicInfo() { Id = e.Id, Email = e.Email, FullName = e.FullName }).FirstOrDefault();

            if (user != null)
            {
                bool worker;
                if (admin)
                    worker = _context.MissierWorker.Any(e => (!e.Removed && e.HasPermission && e.HasPermissionCollaborator) && (e.FullName == user.FullName && e.Email == user.Email));
                else
                    worker = _context.MissierWorker.Any(e => (!e.Removed && e.HasPermission) && (e.FullName == user.FullName && e.Email == user.Email));

                if (worker)
                    return id;
                else
                {
                    HttpContext.Session.SetInt32("UserLogId", 0);
                    return 0;
                }
            }
            else
            {
                HttpContext.Session.SetInt32("UserLogId", 0);
                return 0;
            }

        }


        public async Task<IActionResult> AccessPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AccessPageValidation(IFormCollection form)
        {
            var email = form["Email"].ToString();
            if (String.IsNullOrEmpty(email))
                return RedirectToAction("AccessPage");

            var isValid = _context.RaffleBusinessCollaborator.Any(e => !e.Removed && e.Email == email);

            if (isValid)
            {
                HttpContext.Session.SetString("EmailCollaborator", email);
                return RedirectToAction("CollaboratorMainPage");
            }
            else
                return RedirectToAction("AccessPage");

        }

        public async Task<IActionResult> CollaboratorMainPage()
        {
            var email = HttpContext.Session.GetString("EmailCollaborator");

            if(String.IsNullOrEmpty(email))
                return RedirectToAction("AccessPage");

            var collaborator = _context.RaffleBusinessCollaborator.Where(e => !e.Removed && e.Email == email)
                .Select(e=> new RaffleBusinessCollaborator()
                {
                    Id = e.Id,
                    PersonalCode = e.PersonalCode,
                    PixKey = e.PixKey,
                    PixType = e.PixType,
                    FirstName = SeveralFunctions.GetUserFirstName(e.FullName),
                    YourCash = e.YourCash,
                    YourCashPercentage = e.YourCashPercentage,
                    Phone = Convert.ToInt64(e.Phone).ToString(@"(00) 00000-0000"),
                    Email = e.Email,
                    FullName = e.FullName

                })
                .FirstOrDefault();

            if(collaborator != null)
                collaborator.PeriodRegisters =  await _context.CollaboratorPaymentRegister
                    .Where(e => !e.Removed && e.CollaboratorId == collaborator.Id)
                    .Select(e=> new CollaboratorPaymentRegister() 
                    {
                        Id = e.Id,
                        IsPayed = e.IsPayed,
                        Observation = e.Observation,
                        PeriodValue = e.PeriodValue,
                        PeriodTime = e.PeriodTime,
                        PaymentDate = e.PaymentDate,
                        ReceiptFile = String.IsNullOrEmpty(e.ReceiptFile) ? "" : e.ReceiptFile
                    })
                    .ToListAsync();

            ViewBag.Currency = CultureInfo.CreateSpecificCulture("pt-BR");

            return View(collaborator);
        }













        // GET: Collaborators
        public async Task<IActionResult> Index()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            return View(await _context.RaffleBusinessCollaborator.ToListAsync());
        }

        // GET: Collaborators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var raffleBusinessCollaborator = await _context.RaffleBusinessCollaborator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (raffleBusinessCollaborator == null)
            {
                return NotFound();
            }

            return View(raffleBusinessCollaborator);
        }

        // GET: Collaborators/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonalCode,Phone,PixType,PixKey,FullName,Email")] RaffleBusinessCollaborator raffleBusinessCollaborator)
        {
            raffleBusinessCollaborator.Password = "missier";
            raffleBusinessCollaborator.Identity = RandomToken.RandomString(10);
            raffleBusinessCollaborator.YourCash = 0;
            raffleBusinessCollaborator.YourCashPercentage = 10;
            raffleBusinessCollaborator.PersonalCode = raffleBusinessCollaborator.PersonalCode.ToLower();

            var pattern = new Regex("[^0-9]");
            var phone = pattern.Replace(raffleBusinessCollaborator.Phone, string.Empty);
            raffleBusinessCollaborator.Phone = phone;

            _context.Add(raffleBusinessCollaborator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Collaborators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var raffleBusinessCollaborator = await _context.RaffleBusinessCollaborator.FindAsync(id);
            if (raffleBusinessCollaborator == null)
            {
                return NotFound();
            }
            return View(raffleBusinessCollaborator);
        }

        // POST: Collaborators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Identity,Password,PersonalCode,Phone,PixType,PixKey,FullName,YourCash,Email,Removed")] RaffleBusinessCollaborator raffleBusinessCollaborator)
        {
            if (id != raffleBusinessCollaborator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(raffleBusinessCollaborator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RaffleBusinessCollaboratorExists(raffleBusinessCollaborator.Id))
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
            return View(raffleBusinessCollaborator);
        }

        // GET: Collaborators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var raffleBusinessCollaborator = await _context.RaffleBusinessCollaborator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (raffleBusinessCollaborator == null)
            {
                return NotFound();
            }

            return View(raffleBusinessCollaborator);
        }

        // POST: Collaborators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var raffleBusinessCollaborator = await _context.RaffleBusinessCollaborator.FindAsync(id);
            _context.RaffleBusinessCollaborator.Remove(raffleBusinessCollaborator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RaffleBusinessCollaboratorExists(int id)
        {
            return _context.RaffleBusinessCollaborator.Any(e => e.Id == id);
        }

        public IActionResult VerifyInviteCodeExistence(string PersonalCode)
        {
            if (!String.IsNullOrEmpty(PersonalCode))
                if (PersonalCode.Length == 8)
                {
                    var answer = _context.RaffleBusinessCollaborator.Any(e => e.PersonalCode == PersonalCode.ToLower() && !e.Removed);
                    if (answer)
                        return Json("Esse Código de Convite já existe, tente outro.");
                    else
                        return Json(true);
                }
                else
                    return Json("Um convite válido possui 8 caracteres.");

            return Json(false);
        }
    }
}
