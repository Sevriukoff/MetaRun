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
    CharacterCollectedGoldCoin = 140,
    CharacterCollectedLunarCoin = 141,
    CharacterInventoryItemAdded = 150,
    CharacterInventoryItemRemoved = 151,
    CharacterInteractedWithPurchase = 160,
    CharacterLeveledUp = 170,
    CharacterUsedEquipment = 180,
    CharacterDied = 190,
    RunStarted = 200,
    RunStageStarted = 210,
    RunLeveledUp = 220, // Отдельное событие не нужно
    RunPaused = 230, // Отдельное событие не нужно
    RunDifficultyChanged = 240,
    RunEnded = 250,
    RunMobSpawned = 260
    //MobDamageMob
}