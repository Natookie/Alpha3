using Nova;
using UnityEngine;

public class OuterCircle : MonoBehaviour
{
    [SerializeField] private float SpawnScale = 1.5f;
    [SerializeField] private float punchStrength = 0.3f;
    [SerializeField] private float punchDecay = 0.12f;
    [SerializeField] private Color goldenBorderColor = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private Color normalBorderColor = Color.white;
    [SerializeField] private float goldenFlashDuration = 0.15f;
    [SerializeField] private Color goldenFlashColor = Color.white;

    [Header("TEXT SPLIT")]
    [SerializeField] private int maxChunks = 8;
    [SerializeField] private float minSpawnDelay = 0.03f;
    [SerializeField] private float maxSpawnDelay = 0.09f;

    private Transform tr;
    private PlayerStats ps;
    private UIBlock2D self;

    private const float TARGET_SCALE = 1f;
    private const float GROW_MULTIPLIER = 2f;

    private bool shrinking = false;
    private bool isGolden;
    private float timer;
    private float shrinkDuration;

    private float punch;
    private float goldenFlashTimer;

    void Start(){
        self = GetComponent<UIBlock2D>();
        tr = GetComponent<Transform>();
        tr.localScale = new Vector3(TARGET_SCALE, TARGET_SCALE, TARGET_SCALE);
        ps = PlayerStats.Instance;
        shrinkDuration = ps.CircleShrinkDuration;

        RollGolden();
        ApplyGoldenVisual();
    }

    public void SetShrinkDuration(float newDuration) => shrinkDuration = newDuration;

    void Update(){
        float duration = shrinking ? shrinkDuration : shrinkDuration / GROW_MULTIPLIER;
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        float baseScale = shrinking
            ? Mathf.Lerp(SpawnScale, TARGET_SCALE, t)
            : Mathf.Lerp(TARGET_SCALE, SpawnScale, t);

        if(punch > 0f) punch = Mathf.MoveTowards(punch, 0f, Time.deltaTime / punchDecay);
        float finalScale = baseScale * (1f + punch * punchStrength);

        tr.localScale = new Vector3(finalScale, finalScale, finalScale);

        if(goldenFlashTimer > 0f){
            float n = 1f - (goldenFlashTimer / goldenFlashDuration);
            float pulse = Mathf.Sin(n * Mathf.PI);
            Color baseCol = isGolden ? goldenBorderColor : normalBorderColor;
            self.Border.Color = Color.Lerp(baseCol, goldenFlashColor, pulse);
            goldenFlashTimer -= Time.deltaTime;
            if(goldenFlashTimer <= 0f) self.Border.Color = baseCol;
        }

        if(t >= 1f){
            timer = 0f;
            if(!shrinking) shrinking = true;
            else{
                shrinking = false;
                OnHit();
                RollGolden();
                ApplyGoldenVisual();
            }
        }
    }

    void OnHit(){
        punch = 1f;

        if(isGolden) goldenFlashTimer = goldenFlashDuration;
        if(InnerCircle.Instance != null) InnerCircle.Instance.ReactToHit(isGolden);

        float currencyValue = ps.PointOnHit * ps.GlobalMultiplier;
        if(isGolden) currencyValue *= Random.Range(5f, 10f);
        InventoryManager.Instance.AddCurrency(currencyValue);

        SpawnSplitText(currencyValue, isGolden);
        ParticleManager.Instance.Spawn(tr.position);
        AudioManager.Instance.PlaySfx(isGolden ? "Golden Circle Hit" : "Circle Hit");
    }

    void SpawnSplitText(float totalValue, bool golden){
        int total = Mathf.Max(1, Mathf.FloorToInt(totalValue));
        int count = Mathf.Min(Random.Range(1, 5), total);
        float delay = 0f;

        int remaining = total;
        for(int i = 0; i < count; i++){
            int chunksLeft = count - i;
            int chunk;
            if(chunksLeft == 1){
                chunk = remaining;
            }
            else {
                int avg = remaining / chunksLeft;
                int min = Mathf.Max(1, Mathf.FloorToInt(avg * 0.3f));
                int max = Mathf.Max(min, Mathf.CeilToInt(avg * 1.7f));
                chunk = Random.Range(min, max + 1);
                chunk = Mathf.Clamp(chunk, 1, remaining - (chunksLeft - 1));
            }

            remaining -= chunk;

            FloatingTextManager.Instance.Spawn(tr.position, $"+{chunk}", golden, delay);
            delay += Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }

    void RollGolden() => isGolden = Random.value < ps.GoldenCircleChance;
    void ApplyGoldenVisual(){
        if(goldenFlashTimer <= 0f) self.Border.Color = isGolden ? goldenBorderColor : normalBorderColor;
    }
}