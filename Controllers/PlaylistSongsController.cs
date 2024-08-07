using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using asp_applicatie.Models;
using asp_applicatie.ViewModels;

namespace asp_applicatie.Controllers
{
    public class PlaylistSongsController : Controller
    {
        private readonly AspApplicatieDbContext _context;

        public PlaylistSongsController(AspApplicatieDbContext context)
        {
            _context = context;
        }

        // GET: PlaylistSongs
        public async Task<IActionResult> Index(int id)
        {
            // Retrieve playlist information
            var viewModel = await _context.Playlists
       .Where(p => p.PlaylistId == id)
       .Select(p => new PlayListSongViewModel
       {
           PlaylistId = p.PlaylistId,
           PlaylistName = p.Name,
           songs = p.PlaylistSongs!.Select(ps => new SongViewModel
           {
               SongId = ps.Song!.SongId,
               SongTitle = ps.Song.Title,
               Duration = ps.Song.Duration
           }).ToList()
       })
       .FirstOrDefaultAsync();

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: PlaylistSongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PlaylistSongs == null)
            {
                return NotFound();
            }

            var playlistSong = await _context.PlaylistSongs
                .FirstOrDefaultAsync(m => m.PlaylistSongId == id);
            if (playlistSong == null)
            {
                return NotFound();
            }

            return View(playlistSong);
        }

        // GET: PlaylistSongs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlaylistSongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaylistSongId,PlaylistId,SongId,Status")] PlaylistSong playlistSong)
        {playlistSong.Status = 1;
            if (ModelState.IsValid)
            {
                _context.Add(playlistSong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlistSong);
        }

        // GET: PlaylistSongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PlaylistSongs == null)
            {
                return NotFound();
            }

            var playlistSong = await _context.PlaylistSongs.FindAsync(id);
            if (playlistSong == null)
            {
                return NotFound();
            }
            return View(playlistSong);
        }

        // POST: PlaylistSongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlaylistSongId,PlaylistId,SongId,Status")] PlaylistSong playlistSong)
        {
            playlistSong.Status = 1;
            if (id != playlistSong.PlaylistSongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlistSong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistSongExists(playlistSong.PlaylistSongId))
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
            return View(playlistSong);
        }

        // GET: PlaylistSongs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PlaylistSongs == null)
            {
                return NotFound();
            }

            var playlistSong = await _context.PlaylistSongs
                .FirstOrDefaultAsync(m => m.PlaylistSongId == id);
            if (playlistSong == null)
            {
                return NotFound();
            }

            return View(playlistSong);
        }

        // POST: PlaylistSongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PlaylistSongs == null)
            {
                return Problem("Entity set 'AspApplicatieDbContext.PlaylistSongs'  is null.");
            }
            var playlistSong = await _context.PlaylistSongs.FindAsync(id);
            if (playlistSong != null)
            {
                _context.PlaylistSongs.Remove(playlistSong);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistSongExists(int id)
        {
          return (_context.PlaylistSongs?.Any(e => e.PlaylistSongId == id)).GetValueOrDefault();
        }
    }
}
