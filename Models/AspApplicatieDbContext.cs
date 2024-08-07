using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace asp_applicatie.Models;

public partial class AspApplicatieDbContext : DbContext
{
    public AspApplicatieDbContext()
    {
    }

    public AspApplicatieDbContext(DbContextOptions<AspApplicatieDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistSong> PlaylistSongs { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<PlaylistQueue> PlaylistQueues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     => optionsBuilder.UseSqlite("Data Source=mylocaldb.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.ToTable("Album");

            entity.Property(e => e.ReleaseDate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.ToTable("Playlist");

            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlaylistSong>(entity =>
        {
            entity.ToTable("PlaylistSong");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.ToTable("Song");


            entity.Property(e => e.AudioFilePath).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Ratings)
                .HasMaxLength(100)
                .IsUnicode(false);
        });
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.ToTable("Artist");
            entity.Property(e => e.ArtistName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                            .HasMaxLength(500)
                            .IsUnicode(false);
            entity.Property(e => e.Genre)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.FullName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(200)
                .IsUnicode(false);
        });
        modelBuilder.Entity<PlaylistQueue>(entity =>
        {
            entity.ToTable("PlaylistQueue");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
