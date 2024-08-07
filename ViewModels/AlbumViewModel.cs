namespace asp_applicatie.ViewModels
{
    public class AlbumViewModel
    {
        public int AlbumId { get; set; }

        public string? Title { get; set; }

        public int ArtistId { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int? Quantity { get; set; }
    }
}
