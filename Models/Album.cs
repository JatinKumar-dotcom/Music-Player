using System;
using System.Collections.Generic;

namespace asp_applicatie.Models;

public partial class Album
{
    public int AlbumId { get; set; }

    public string? Title { get; set; }

    public int? ArtistId { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int? Quantity { get; set; }

    public int? Status { get; set; }
    public Artist? artist { get; set; }
    public ICollection<Song>? Songs { get; set; }

}
