using asp_applicatie.Models;
using asp_applicatie.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asp_applicatie.Controllers
{
    public class PlaylistQueueController : Controller
    {
        private readonly AspApplicatieDbContext _context;

        public PlaylistQueueController(AspApplicatieDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FormatQueue(int id)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM PlaylistQueue;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='PlaylistQueue';");
                transaction.Commit();

                return Json(new { Message = "OK" });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(new { Message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddQueue(PlaylistQueue model)
        {
            var existingsong = await _context.PlaylistQueues
             .FirstOrDefaultAsync(q => q.SongId == model.SongId && q.AlbumId == null && q.PlaylistId == null);
            var existingalbum = await _context.PlaylistQueues
             .FirstOrDefaultAsync(q => q.SongId == null && q.AlbumId == model.AlbumId && q.PlaylistId == null);
            var existingplaylist = await _context.PlaylistQueues
             .FirstOrDefaultAsync(q => q.SongId == null && q.AlbumId == null && q.PlaylistId == model.PlaylistId);

            if (existingsong != null || existingalbum != null || existingplaylist != null)
            {
                return Json(new { Message = "Item Already Available in queue" });
            }
            else if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { QueueId = model.QueueId });
            }
            return Json(new { Message = "Something went wrong" });
        }

        [HttpGet]
        public async Task<IActionResult> GetQueueItems()
        {
            try
            {
                var queueItems = await _context.PlaylistQueues
                    .ToListAsync();

                var result = await Task.WhenAll(queueItems.Select(async queue =>
                {
                    if (queue.PlaylistId != null)
                    {
                        // Handle playlist case
                        var playlist = await _context.Playlists
                            .Where(p => p.PlaylistId == queue.PlaylistId)
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

                        return playlist;
                    }
                    else if (queue.AlbumId != null)
                    {
                        // Handle album case
                        var album = await _context.Albums
                            .Where(a => a.AlbumId == queue.AlbumId)
                            .Select(a => new PlayListSongViewModel
                            {
                                PlaylistId = 0,
                                PlaylistName = a.Title,
                                songs = a.Songs!.Select(song => new SongViewModel
                                {
                                    SongId = song.SongId,
                                    SongTitle = song.Title,
                                    AudioFilePath = song.AudioFilePath,
                                    Ratings = song.Ratings,
                                    Duration = song.Duration
                                }).ToList()
                            })
                            .FirstOrDefaultAsync();

                        return album;
                    }
                    else if (queue.SongId != null)
                    {
                        // Handle standalone song case
                        var song = await _context.Songs
                            .FirstOrDefaultAsync(s => s.SongId == queue.SongId);

                        return new PlayListSongViewModel
                        {
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
                    else
                    {
                        return null;
                    }
                }));

                // Filter out null results and return the valid ones
                var validQueueItems = result.Where(item => item != null).ToList();
                return Json(new { queue = validQueueItems });
            }
            catch (Exception ex)
            {
                return Json(new { Message = $"Error: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLastQueueItem(int queueId)
        {
            int? id = 0 ; int? PLId = 0; int? AlbumId = 0;

            if (queueId != 0)
            {
                var queue = await _context.PlaylistQueues
                .FirstOrDefaultAsync(m => m.QueueId == queueId);
                
                if(queue != null && queue.PlaylistId != null)
                {
                    PLId = queue.PlaylistId;
                }
                else if (queue != null && queue.AlbumId != null)
                {
                    AlbumId = queue.AlbumId ;
                }
                else if (queue != null && queue.SongId != null)
                {
                    id = queue.SongId;
                }
                else
                {
                    return Json(new { Message = "Something went wrong" });
                }
            }
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
                return Json(new { queue = viewModel });
            }
            else if (AlbumId != 0)
            {
                var viewModel = await _context.Albums
           .Where(p => p.AlbumId == AlbumId)
           .Select(p => new PlayListSongViewModel
           {
               PlaylistId = 0,
               PlaylistName = p.Title,
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
                return Json(new { queue = viewModel });

            }
            else if (id != 0)
            {
                PlayListSongViewModel playlistSong = new PlayListSongViewModel();
                var song = await _context.Songs
                                .FirstOrDefaultAsync(m => m.SongId == id);
                if (song != null)
                {
                    playlistSong = new PlayListSongViewModel
                    {
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
                return Json(new { queue = playlistSong });
            }
            else
            {
                return Json(new { Message = "Something went wrong" });

            }
        }

        //     .Include(q => q.songs)
        //         .ThenInclude(s => s.)  // Assuming your Song entity has a property named Song
        //     .Include(q => q.ablums)
        //     .Include(q => q.playlists)
        //     .FirstOrDefaultAsync(p => p.QueueId == queueId);
        //}

        // Assuming you want to return the collection as JSON
    }
}
