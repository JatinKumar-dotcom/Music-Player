using asp_applicatie.Models;

namespace asp_applicatie.ViewModels
{
    public class SongViewModel
    {
        public int SongId { get; set; }
        public string? SongTitle { get; set; }
        public string? AudioFilePath { get; set; }
        public int? Duration { get; set; }
        public int? Ratings { get; set; }

        public Album? album { get; set; }
    }
}
