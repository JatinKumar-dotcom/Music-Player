using asp_applicatie.Models;
using asp_applicatie.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace asp_applicatie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AspApplicatieDbContext _context;

        public HomeController(ILogger<HomeController> logger, AspApplicatieDbContext context)
        {
            _logger = logger;
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel dashboardView = new DashboardViewModel();
            var albums = await _context.Albums.ToListAsync();
            var songs = await _context.Songs.ToListAsync();
            var playlists = await _context.Playlists.ToListAsync();
            dashboardView.albums = albums!;
            dashboardView.Song = songs!;
            dashboardView.Playlists = playlists!;
            //var viewModel = new DashboardViewModel
            //{
            //    albums = albums,
            //    Song = songs,
            //    Playlists = playlists
            //};
            return View(dashboardView);
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