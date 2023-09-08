namespace Sevriukoff.MetaRun.Domain.Enum;

public enum EventType
{
    CharacterDealtDamage = 100,
    CharacterTookDamage = 101,
    CharacterMinionDealtDamage = 102,
    CharacterMinionTookDamage = 103,
    CharacterKilledMob = 110,
    CharacterHealed = 120,
    CharacterMoved = 130,
    CharacterCollectedGoldCoin = 141,
    CharacterCollectedLunarCoin = 142,
    CharacterInventoryChanged = 150,
    CharacterLeveledUp = 160,
    CharacterUsedEquipment = 170,
    CharacterDied = 180,
    RunStarted = 200,
    RunStageStarted = 210,
    RunLeveledUp = 220, // Отдельное событие не нужно
    RunPaused = 230, // Отдельное событие не нужно
    RunDifficultyChanged = 240,
    RunEnded = 250,
    RunMobSpawned = 260
    //MobDamageMob
}