﻿namespace Sevriukoff.MetaRun.Domain.Enum;

public enum EventType
{
    //---------------Character-----------------//
    /// <summary>
    /// Персонаж, управляемый игроком нанёс урон игровому объекту.
    /// Необходима исчерпыввающая информацию о объекте, которому был нанесён урон.
    /// </summary>
    CharacterDealtDamage = 100,
    
    /// <summary>
    /// Персонаж, управляемый игроком получил урон от игрового объекта.
    /// Необходима исчерпывающая информация о объекте, который нанёс урон.
    /// </summary>
    CharacterTookDamage = 101,
    
    /// <summary>
    /// Миньон персонажа, который управляется игроком, нанёс урон игровому объекту.
    /// Необходима исчерпывающая информация о объекте, которому был нанесён урон.
    /// </summary>
    CharacterMinionDealtDamage = 102,
    
    /// <summary>
    /// Миньон персонажа, который управляется игроком, получил урон от игрового объекта.
    /// Необходима исчерпывающая информация о объекте, который нанёс урон.
    /// </summary>
    CharacterMinionTookDamage = 103,
    
    /// <summary>
    /// Персонаж, управляемый игроком убил игровую сущность.
    /// Необходима исчерпывающая информация о погибшей игрвой сущности.
    /// </summary>
    CharacterKilledMob = 110,
    
    /// <summary>
    /// Персонаж, управляемый игроком, получил здоровье.
    /// Необходима информация о способе и количестве полученного здоровья.
    /// </summary>
    CharacterHealed = 120,
    
    /// <summary>
    /// Персонаж, управляемый игроком, переместился в пространстве.
    /// Необходима информация о новом положении персонажа.
    /// </summary>
    CharacterMoved = 130,
    
    /// <summary>
    /// 
    /// </summary>
    CharacterGoldCoinChanged = 140,
    CharacterLunarCoinChanged = 141,
    
    /// <summary>
    /// 
    /// </summary>
    CharacterInventoryItemAdded = 150,
    CharacterInventoryItemRemoved = 151,
    
    /// <summary>
    /// Персонаж, управляемый игроком, взаимодействовал с игровыми объекта за различную валюту (монеты, здоровье).
    /// Необходима исчерпывающая информация о объекте взаимодействия.
    /// </summary>
    CharacterInteractedWithPurchase = 160,
    
    /// <summary>
    /// Персонаж, управляемый игроком, получил новый уровень.
    /// Необходима информация о новом уровне персонажа.
    /// </summary>
    CharacterLeveledUp = 170,
    
    /// <summary>
    /// Персонаж, управляемый игроком, использовал снаряжение.
    /// Необходима информация о использованом снаряжении.
    /// </summary>
    CharacterUsedEquipment = 180,
    
    /// <summary>
    /// Персонаж, управляемый игроком, был убит.
    /// Необходима информация о причине смерти персонажа.
    /// </summary>
    CharacterDied = 190,
    
    //------------------Run--------------------//
    /// <summary>
    /// Забег начался.
    /// Необходима исчерпывающая информация о забеге.
    /// </summary>
    RunStarted = 200,
    
    /// <summary>
    /// Началась новая стадия в забеге.
    /// Необходима исчерпывающая информация о новой стадии.
    /// </summary>
    RunStageStarted = 210,
    
    /// <summary>
    /// У забега увеличился уровень сложности на 1.
    /// </summary>
    RunLeveledUp = 220, // Отдельное событие не нужно
    
    /// <summary>
    /// Хост остановил забег.
    /// </summary>
    RunPaused = 230, // Отдельное событие не нужно
    
    /// <summary>
    /// Секундомер забега остановился.
    /// </summary>
    RunTimePaused = 240,
    
    /// <summary>
    /// Забег закончился.
    /// Необходима исчерпывающая информация о причине окончания забега.
    /// </summary>
    RunEnded = 250,
    
    //------------------Mob--------------------//
    MobSpawned = 300,
    MobDamageMob = 310
}