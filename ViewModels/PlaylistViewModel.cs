using asp_applicatie.Models;

namespace asp_applicatie.ViewModels
{
    public class PlaylistViewModel
    {
        public int PlaylistId { get; set; }

        public string? Name { get; set; }

        public bool? IsPrivate { get; set; }

        public int? UserId { get; set; }

        public User? User { get; set; }
        public ICollection<PlaylistSong>? PlaylistSongs { get; set; }
    }
}
