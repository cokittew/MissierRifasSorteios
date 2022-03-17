using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MissierSystem.DataContext;
using MissierSystem.Models.TonModality;
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

        // GET: Collaborators
        public async Task<IActionResult> Index()
        {
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

        // POST: Collaborators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
