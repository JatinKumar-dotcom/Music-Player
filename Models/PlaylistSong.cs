using System;
using System.Collections.Generic;

namespace asp_applicatie.Models;

public partial class PlaylistSong
{
    public int PlaylistSongId { get; set; }

    public int? PlaylistId { get; set; }

    public int? SongId { get; set; }

    public int? Status { get; set; }

    public Playlist? Playlist { get; set; }
    public Song? Song { get; set; }
}
