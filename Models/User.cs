using System;
using System.Collections.Generic;

namespace asp_applicatie.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public int? LastPlayedSongId { get; set; }

    public int? Status { get; set; }

    public Song? LastPlayedSong { get; set; }

}
