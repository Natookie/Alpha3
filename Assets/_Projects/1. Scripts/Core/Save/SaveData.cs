using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float pointOnHit;
    public float circleShrinkDuration;
    public float goldenCircleChance;
    public int outerCircleCount;
    public float globalMultiplier;

    public float masterVolume = 1f;
    public float sfxVolume = 1f;
    public float bgmVolume = 1f;

    public float currency;

    public List<UpgradeLevelEntry> upgradeLevels = new List<UpgradeLevelEntry>();
}

[Serializable]
public class UpgradeLevelEntry
{
    public string upgradeId;
    public int level;
}