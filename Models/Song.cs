using System;
using System.Collections.Generic;

namespace asp_applicatie.Models;

public partial class Song
{
    public int SongId { get; set; }

    public string? Title { get; set; }

    public string? AudioFilePath { get; set; }

    public int? AlbumId { get; set; }

    public int? Duration { get; set; }
    public int? Ratings { get; set; }

    public int? Status { get; set; }

    public Album? album { get; set; }

}
