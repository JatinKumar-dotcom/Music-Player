using asp_applicatie.Models;
using asp_applicatie.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asp_applicatie.Controllers
{
    public class PlayerController : Controller
    {
        private readonly AspApplicatieDbContext _context;

        public PlayerController(AspApplicatieDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? id, int PLId, int AlbumId)
        {
            if (PLId != 0)
            {
                // Retrieve playlist information
                var viewModel = await _context.Playlists
           .Where(p => p.PlaylistId == PLId)
           .Select(p => new PlayListSongViewModel
           {
               PlaylistId = p.PlaylistId,
               PlaylistName = p.Name,
               songs = p.PlaylistSongs!.Select(ps => new SongViewModel
               {
                   SongId = ps.Song!.SongId,
                   SongTitle = ps.Song.Title,
                   AudioFilePath = ps.Song.AudioFilePath,
                   Ratings = ps.Song.Ratings,
                   Duration = ps.Song.Duration
               }).ToList()
           })
           .FirstOrDefaultAsync();

                if (viewModel == null)
                {
                    return NotFound();
                }
                var albums = await _context.Albums.ToListAsync();
                var songs = await _context.Songs.ToListAsync();
                var playlists = await _context.Playlists.ToListAsync();

                ViewBag.albums = albums;
                ViewBag.songs = songs;
                ViewBag.playlists = playlists;
                ViewBag.SongId = id;
                return View(viewModel);
            }
            else if(AlbumId != 0)
            {
                var viewModel = await _context.Albums
           .Where(p => p.AlbumId == AlbumId)
           .Select(p => new PlayListSongViewModel
           {
               PlaylistId = 0,
               PlaylistName = "Album",
               songs = p.Songs!.Select(ps => new SongViewModel
               {
                   SongId = ps.SongId,
                   SongTitle = ps.Title,
                   AudioFilePath = ps.AudioFilePath,
                   Ratings = ps.Ratings,
                   Duration = ps.Duration
               }).ToList()
           })
           .FirstOrDefaultAsync();
                var albums = await _context.Albums.ToListAsync();
                var songs = await _context.Songs.ToListAsync();
                var playlists = await _context.Playlists.ToListAsync();

                ViewBag.albums = albums;
                ViewBag.songs = songs;
                ViewBag.playlists = playlists;
                ViewBag.SongId = 1;
                return View(viewModel);
            }
            else
            {
                PlayListSongViewModel playlistSong = new PlayListSongViewModel();
                if(id == 0 || id == null)
                {
                    id = GeneralInformation.LastPlayedSongId;
                }
                var song = await _context.Songs
                                .FirstOrDefaultAsync(m => m.SongId == id);
                if(song != null)
                {
                    playlistSong = new PlayListSongViewModel
                    {
                        PlaylistId = 0,
                        PlaylistName = "Random",
                        songs = new List<SongViewModel>
                    {
                        new SongViewModel
                        {
                            SongId = song.SongId,
                            SongTitle = song.Title,
                            AudioFilePath = song.AudioFilePath,
                            Ratings = song.Ratings,
                            Duration = song.Duration
                        }
                    }
                    };
                }
                var albums = await _context.Albums.ToListAsync();
                var songs = await _context.Songs.ToListAsync();
                var playlists = await _context.Playlists.ToListAsync();

                ViewBag.albums = albums;
                ViewBag.songs = songs;
                ViewBag.playlists = playlists;
                ViewBag.SongId = id;
                return View(playlistSong);
            }
        }
        //[HttpPost]
        //public IActionResult AddToQueue(int id, int albumId, int playlistId)
        //{
        //    MusicPlayer.Queue.Add(new QueueItem { SongId = id, AlbumId = albumId, PlaylistId = playlistId });
        //    // Redirect back to the original view or any other appropriate action
        //    return RedirectToAction("Index");
        //}
    }
}
