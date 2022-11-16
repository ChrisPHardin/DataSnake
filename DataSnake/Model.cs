using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class ScoreContext : DbContext
{
    public DbSet<Scores> Scores { get; set; }

    public string DbPath { get; }

    public ScoreContext()
    {
        var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        DbPath = System.IO.Path.Join(path, "/scores.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Scores
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int HighScore { get; set; }

}
