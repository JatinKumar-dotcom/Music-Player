using System;
using System.Collections.Generic;

namespace asp_applicatie.Models;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public string? Name { get; set; }

    public bool IsPrivate { get; set; }

    public int? UserId { get; set; }

    public int? Status { get; set; }

    public User? User { get; set; }
    public ICollection<PlaylistSong>? PlaylistSongs { get; set; }
}
