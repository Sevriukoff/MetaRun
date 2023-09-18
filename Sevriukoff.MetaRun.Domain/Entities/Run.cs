using System;
using System.Collections.Generic;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Entities;

public class Run
{
    public Guid Id { get; }
    public ulong Seed { get; }
    public string ServerName { get; }
    public RunDifficulty Difficulty { get; }
    public GameMode GameMode { get; }
    public string[] Artifacts { get; }
    public string[] Dlcs { get;  }
    public Dictionary<ulong, string> PlayerCharacters { get; set; }
    
    public RunStatus Status { get; set; }
    public int TimeInMinutes { get; set; }
    public int CountCompletedStages { get; set; }
    public int MonstersLevel { get; set; }

    public Run(Guid id, ulong seed, string serverName, RunDifficulty difficulty,
        GameMode gameMode, string[] dlcs, string[] artifacts)
    {
        Id = id;
        Seed = seed;
        ServerName = serverName;
        Difficulty = difficulty;
        GameMode = gameMode;
        Dlcs = dlcs;
        Artifacts = artifacts;

        Status = RunStatus.Started;
    }

    public float CalculateDifficulty()
    {
        var playerFactor = 1 + 0.3 * (PlayerCharacters.Count - 1);
        var timeFactor = 0.0506 * (int) Difficulty * Math.Pow(PlayerCharacters.Count, 0.2);
        var stageFactor = Math.Pow(1.15, CountCompletedStages);

        return (float) ((playerFactor + TimeInMinutes * timeFactor) * stageFactor);
    }
}
