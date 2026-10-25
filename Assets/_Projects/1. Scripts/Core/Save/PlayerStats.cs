using System;
using System.Drawing;
using NaughtyAttributes;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public event Action<float> OnPointOnHitChanged;
    public event Action<float> OnCircleShrinkDurationChanged;
    public event Action<float> OnGoldenCircleChanceChanged;
    public event Action<int> OnOuterCircleCountChanged;
    public event Action<float> OnGlobalMultiplierChanged;

    private float pointOnHit;
    private float circleShrinkDuration;
    private float goldenCircleChance;
    private int outerCircleCount;
    private float globalMultiplier;

    [Header("STATS")]
    [SerializeField] private float defaultPointOnHit = 3;
    [SerializeField] private float defaultCircleShrinkDuration = 2;
    [Range(0f, 1f)] 
    [SerializeField] private float defaultGoldenCircleChance = 0.01f;
    [SerializeField] private int defaultOuterCircleCount = 1;
    [SerializeField] private float defaultGlobalMultiplier = 1;

    public float PointOnHit{
        get => pointOnHit;
        set{
            if(Mathf.Approximately(pointOnHit, value)) return;
            pointOnHit = value;
            OnPointOnHitChanged?.Invoke(pointOnHit);
        }
    }

    public float CircleShrinkDuration{
        get => circleShrinkDuration;
        set{
            if(Mathf.Approximately(circleShrinkDuration, value)) return;
            circleShrinkDuration = value;
            OnCircleShrinkDurationChanged?.Invoke(circleShrinkDuration);
        }
    }

    public float GoldenCircleChance{
        get => goldenCircleChance;
        set{
            float clamped = Mathf.Clamp01(value);
            if(Mathf.Approximately(goldenCircleChance, clamped)) return;
            goldenCircleChance = clamped;
            OnGoldenCircleChanceChanged?.Invoke(goldenCircleChance);
        }
    }

    public int OuterCircleCount{
        get => outerCircleCount;
        set{
            if(outerCircleCount == value) return;
            outerCircleCount = value;
            OnOuterCircleCountChanged?.Invoke(outerCircleCount);
        }
    }

    public float GlobalMultiplier{
        get => globalMultiplier;
        set{
            if(Mathf.Approximately(globalMultiplier, value)) return;
            globalMultiplier = value;
            OnGlobalMultiplierChanged?.Invoke(globalMultiplier);
        }
    }

    void Awake(){
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Button("Reset Default")]
    public void ResetToDefaults(){
        pointOnHit = defaultPointOnHit;
        circleShrinkDuration = defaultCircleShrinkDuration;
        goldenCircleChance = defaultGoldenCircleChance;
        outerCircleCount = defaultOuterCircleCount;
        globalMultiplier = defaultGlobalMultiplier;
    }
}