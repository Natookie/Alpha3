using System;
using System.IO;
using UnityEngine;
using NaughtyAttributes;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private AudioManager audioManager;

    void Awake(){
        if(Instance != null && Instance != this){ 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;

        if(!HasSave()) playerStats.ResetToDefaults();
        else Load();
    }

    public bool HasSave() => File.Exists(SavePath);

    [Button("Save")]
    public void Save(){
        SaveData data = new SaveData {
            pointOnHit = playerStats.PointOnHit,
            circleShrinkDuration = playerStats.CircleShrinkDuration,
            goldenCircleChance = playerStats.GoldenCircleChance,
            outerCircleCount = playerStats.OuterCircleCount,
            globalMultiplier = playerStats.GlobalMultiplier,
            masterVolume = audioManager != null ? audioManager.GetMasterVolume() : 1f,
            sfxVolume = audioManager != null ? audioManager.GetSfxVolume() : 1f,
            bgmVolume = audioManager != null ? audioManager.GetBgmVolume() : 1f,
            currency = inventory.GetCurrency(),
            upgradeLevels = upgradeManager.ExportLevels()
        };

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log("[Save Manager] Save");
    }

    [Button("Load")]
    public void Load(){
        if(!HasSave()) return;

        string json = File.ReadAllText(SavePath);
        SaveData data;
        try{
            data = JsonUtility.FromJson<SaveData>(json);
        }catch(Exception e){
            Debug.LogError($"Failed to parse save: {e.Message}");
            return;
        }
        if(data == null) return;

        ApplyData(data);
        Debug.Log("[Save Manager] Load");
    }

    [Button("Reset Save")]
    public void ResetSave(){
        if(File.Exists(SavePath)) File.Delete(SavePath);

        playerStats.ResetToDefaults();
        inventory.SetCurrency(0f);
        upgradeManager.ResetLevels();

        if(audioManager != null){
            audioManager.SetMasterVolume(1f);
            audioManager.SetSfxVolume(1f);
            audioManager.SetBgmVolume(1f);
        }
        Debug.Log("[Save Manager] Reset Save");
    }

    void ApplyData(SaveData data){
        playerStats.PointOnHit = data.pointOnHit;
        playerStats.CircleShrinkDuration = data.circleShrinkDuration;
        playerStats.GoldenCircleChance = data.goldenCircleChance;
        playerStats.OuterCircleCount = data.outerCircleCount;
        playerStats.GlobalMultiplier = data.globalMultiplier;

        if(audioManager != null){
            audioManager.SetMasterVolume(data.masterVolume);
            audioManager.SetSfxVolume(data.sfxVolume);
            audioManager.SetBgmVolume(data.bgmVolume);
        }

        inventory.SetCurrency(data.currency);
        upgradeManager.ImportLevels(data.upgradeLevels);
    }

    void OnApplicationQuit() => Save();
}