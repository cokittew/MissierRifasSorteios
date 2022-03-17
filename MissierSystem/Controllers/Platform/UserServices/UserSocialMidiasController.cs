using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;

namespace MissierSystem.Controllers.Platform.UserServices
{
    public class UserSocialMidiasController : Controller
    {
        private readonly UserClienteDataContext _context;

        public UserSocialMidiasController(UserClienteDataContext context)
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

        // GET: UserSocialMidias
        private async Task<IActionResult> Index()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            return View(await _context.UserSocialMidia.ToListAsync());
        }

        // GET: UserSocialMidias/Details/5
        private async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSocialMidia = await _context.UserSocialMidia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userSocialMidia == null)
            {
                return NotFound();
            }

            return View(userSocialMidia);
        }

        // GET: UserSocialMidias/Create
        public IActionResult Create()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = Convert.ToInt32(idUser);

            try
            {
                var userCount = _context.UserSocialMidia.FirstOrDefault(e => e.IdBasicUser == Convert.ToInt32(idUser));
                if (userCount != null)
                    return View(userCount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View();
        }

        // POST: UserSocialMidias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdBasicUser,Instagram,Twitter,Youtube,Facebook,Reddit,WhatsApp,TikTok,AnotherInformations")] UserSocialMidia userSocialMidia, IFormCollection form)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = Convert.ToInt32(idUser);

            if (userSocialMidia.Id != 0)
            {
                userSocialMidia.BeginningDate = DateTime.Now;
                var userCount = new UserSocialMidia();
                userCount = _context.Update(userSocialMidia).Entity;
                await _context.SaveChangesAsync();
                ViewBag.alert = "success";
                return View(userCount);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    userSocialMidia.BeginningDate = DateTime.Now;
                    var userCount = new UserSocialMidia();
                    userCount = _context.Add(userSocialMidia).Entity;
                    await _context.SaveChangesAsync();
                    ViewBag.alert = "success";
                    return View(userCount);
                }
            }
            ViewBag.alert = "error";
            return View(userSocialMidia);
        }

        // GET: UserSocialMidias/Edit/5
        private async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSocialMidia = await _context.UserSocialMidia.FindAsync(id);
            if (userSocialMidia == null)
            {
                return NotFound();
            }
            return View(userSocialMidia);
        }

        // POST: UserSocialMidias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> Edit(int id, [Bind("Id,IdBasicUser,Instagram,Twitter,Youtube,Facebook,Reddit,Kwai,TikTok,AnotherInformations,BeginningDate,Removed")] UserSocialMidia userSocialMidia)
        {
            if (id != userSocialMidia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userSocialMidia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserSocialMidiaExists(userSocialMidia.Id))
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
            return View(userSocialMidia);
        }

        // GET: UserSocialMidias/Delete/5
        private async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSocialMidia = await _context.UserSocialMidia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userSocialMidia == null)
            {
                return NotFound();
            }

            return View(userSocialMidia);
        }

        // POST: UserSocialMidias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userSocialMidia = await _context.UserSocialMidia.FindAsync(id);
            _context.UserSocialMidia.Remove(userSocialMidia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserSocialMidiaExists(int id)
        {
            return _context.UserSocialMidia.Any(e => e.Id == id);
        }
    }
}
