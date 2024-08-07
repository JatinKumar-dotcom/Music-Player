using asp_applicatie.Models;

namespace asp_applicatie.ViewModels
{
    public class DashboardViewModel
    {
        public ICollection<Song>? Song { get; set; }
        public ICollection<Album>? albums { get; set; }
        public ICollection<Playlist>? Playlists { get; set; }
    }
}
