using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using asp_applicatie.Models;
using Microsoft.Extensions.Hosting.Internal;
using asp_applicatie.ViewModels;

namespace asp_applicatie.Controllers
{
    public class SongsController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly AspApplicatieDbContext _context;
        public SongsController(IWebHostEnvironment hostingEnvironment, AspApplicatieDbContext context)
        {
            this.hostingEnvironment = hostingEnvironment;
            _context = context;

        }


        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var viewModel = await _context.Songs
            .Select(p => new SongViewModel
            {
                SongId = p.SongId,
                SongTitle = p.Title,
                AudioFilePath = p.AudioFilePath,
                Duration = p.Duration,
                album = p.album
            })
            .ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        public string SaveRatings(int songId, int rating)
        {
            var song = _context.Songs.FindAsync(songId).Result;

            if (song != null)
            {
                // Update the LastPlayedSongId
                song.Ratings = rating;

                // Save the changes to the database
                _context.SaveChangesAsync();
            }
            return null;
        }
        // GET: Songs/Details/5
        public IActionResult Details(int? id)
        {
            //if (id == null || _context.Songs == null)
            //{
            //    return NotFound();
            //}

            //var song = await _context.Songs
            //    .FirstOrDefaultAsync(m => m.SongId == id);
            //if (song == null)
            //{
            //    return NotFound();
            //}

            //return View(song);
            var playlists = _context.Playlists
        .Select(p => new SelectListItem
        {
            Value = p.PlaylistId.ToString(),
            Text = p.Name
        })
        .ToList();

            ViewData["Playlists"] = playlists;
            ViewBag.Id = id;
            return View();
        }

        // GET: Songs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,Title,AudioFilePath,Artist,AlbumId,Duration,Status")] Song song)
        {song.Status = 1;
            if (ModelState.IsValid)
            {
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(song);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (_context.Songs == null)
            {
                return NotFound();
            }
            var album = _context.Albums
            .Select(p => new SelectListItem
            {
                Value = p.AlbumId.ToString(),
                Text = p.Title
            })
            .ToList();

            ViewData["Albums"] = album;

            var song = await _context.Songs.FindAsync(id);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind("SongId,Title,AudioFilePath,Artist,AlbumId,Duration,Status")] Song song)
        {
            song.Status = 1;
            if (song.SongId == 0)
            {
                _context.Add(song);
            }
            else
            {
                _context.Update(song);
            }
            await _context.SaveChangesAsync(); // Save changes to songs first

            int songCount = await _context.Songs
                .Where(m => m.AlbumId == song.AlbumId)
                .CountAsync();

            var album = await _context.Albums.FindAsync(song.AlbumId);
            if (album != null)
            {
                // Update the LastPlayedSongId
                album.Quantity = songCount;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Songs == null)
            {
                return Problem("Entity set 'AspApplicatieDbContext.Songs'  is null.");
            }
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
          return (_context.Songs?.Any(e => e.SongId == id)).GetValueOrDefault();
        }

        [HttpPost]
        public List<string> UploadImage(IFormFile imageFile)
        {
            try
            {
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "Songs");
                string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadFolder, fileName);
                imageFile.CopyTo(new FileStream(filePath, FileMode.Create));
                var imagepath = $"/Songs/{fileName}";
                return new List<string> { imagepath };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddtoPlaylist([Bind("PlaylistSongId,PlaylistId,SongId,Status")] PlaylistSong playlistSong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(playlistSong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Search(string searchString)
        {
            var suggestions = _context.Songs
            .Where(s => s.Title.Contains(searchString))
            .Select(s => s.Title)
            .Distinct()
            .ToList();

            return Json(suggestions);
        }
    }
}
