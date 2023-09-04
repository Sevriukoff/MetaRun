using System;
using System.Collections.Generic;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain;

public class Run
{
    public Guid Id { get; }
    public ulong Seed { get; }
    public string ServerName { get; }
    public RunDifficulty Difficulty { get; }
    public GameMode GameMode { get; }
    public Dictionary<ulong, string> PlayerCharacters { get; set; }
    public string[] Artifacts { get; set; }
    public string[] Dlcs { get;  }

    public RunStatus Status { get; set; }
    
    public TimeSpan TotalTime { get; set; }
    public int TotalCompletedStages { get; set; }
    public int TotalMonstersLevel { get; set; }

    public Run(Guid id, ulong seed, string serverName, RunDifficulty difficulty,
        GameMode gameMode, string[] dlcs)
    {
        Id = id;
        Seed = seed;
        ServerName = serverName;
        Difficulty = difficulty;
        GameMode = gameMode;
        Dlcs = dlcs;

        Status = RunStatus.Started;
    }
}
