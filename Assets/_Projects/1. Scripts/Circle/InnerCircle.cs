using Nova;
using UnityEngine;

public class InnerCircle : MonoBehaviour
{
    public static InnerCircle Instance { get; private set; }

    [Header("REACTION")]
    [SerializeField] private float squashStrength = 0.15f;
    [SerializeField] private float squashDecay = 0.15f;
    [SerializeField] private float punchStrength = 0.1f;
    [SerializeField] private float punchDecay = 0.12f;

    [Header("GOLDEN FLASH")]
    [SerializeField] private float goldenFlashDuration = 0.2f;
    [SerializeField] private Color goldenFlashColor = new Color(1f, 0.85f, 0.2f, 1f);

    private Transform tr;
    private UIBlock2D self;

    private float squash;
    private float punch;
    private Color originalBorderColor;
    private float goldenFlashTimer;

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;

        tr = GetComponent<Transform>();
        self = GetComponent<UIBlock2D>();
        originalBorderColor = self.Border.Color;
    }

    void Update(){
        if(squash > 0f) squash = Mathf.MoveTowards(squash, 0f, Time.deltaTime / squashDecay);
        if(punch > 0f)  punch  = Mathf.MoveTowards(punch,  0f, Time.deltaTime / punchDecay);

        float sx = 1f + squash * squashStrength;
        float sy = 1f - squash * squashStrength;
        float s  = 1f + punch * punchStrength;
        tr.localScale = new Vector3(sx * s, sy * s, 1f);

        if(goldenFlashTimer > 0f){
            float n = 1f - (goldenFlashTimer / goldenFlashDuration);
            float pulse = Mathf.Sin(n * Mathf.PI);
            self.Border.Color = Color.Lerp(originalBorderColor, goldenFlashColor, pulse);
            goldenFlashTimer -= Time.deltaTime;
            if(goldenFlashTimer <= 0f) self.Border.Color = originalBorderColor;
        }
    }

    public void ReactToHit(bool golden){
        squash = 1f;
        punch  = 1f;

        if(golden) goldenFlashTimer = goldenFlashDuration;
    }
}