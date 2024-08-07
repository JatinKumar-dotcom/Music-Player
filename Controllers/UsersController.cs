using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using asp_applicatie.Models;

namespace asp_applicatie.Controllers
{
    public class UsersController : Controller
    {
        private readonly AspApplicatieDbContext _context;

        public UsersController(AspApplicatieDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Logout()
        {
            GeneralInformation.UId = 0;
            GeneralInformation.UserName = "";
            GeneralInformation.LastPlayedSongId = 0;
            return RedirectToAction("Index", "Users");
        }
        [HttpPost]
        public string SaveLastSongPlayed(int lastPlayedSongId)
        {
            var user = _context.Users.FindAsync(GeneralInformation.UId).Result;

            if (user != null)
            {
                // Update the LastPlayedSongId
                GeneralInformation.LastPlayedSongId = lastPlayedSongId;
                user.LastPlayedSongId = lastPlayedSongId;

                // Save the changes to the database
                _context.SaveChangesAsync();
            }
            return null;
        }
        // GET: Users/Details/5
        public async Task<IActionResult> Login(string userName, string Password)
        {
            if (userName == null || Password == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users!
                .FirstOrDefaultAsync(m => m.Username == userName && m.Password == Password);
            if (user == null)
            {
                return NotFound();
            }
            GeneralInformation.UId = user.UserId;
            GeneralInformation.UserName = user.Username;
            GeneralInformation.LastPlayedSongId = user.LastPlayedSongId;

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> SignUp(string FullName, string userName, string Password)
        {
            if (userName == null || Password == null)
            {
                return NotFound();
            }
            User user = new User()
            {
                FullName = FullName,
                Username = userName,
                Password = Password,
                Status = 1
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Users");
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,FullName,LastPlayedSongId,Status")] User user)
        {user.Status = 1;
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,FullName,LastPlayedSongId,Status")] User user)
        {
            user.Status = 1;
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'AspApplicatieDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        
    }
}
