using Nova;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("MOTION")]
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private float gravity = -8f;
    [SerializeField] private float initialUpwardSpeed = 4f;
    [SerializeField] private float horizontalSpread = 3f;
    [SerializeField] private float swayAmplitude = 0.3f;
    [SerializeField] private float swayFrequency = 6f;

    [Header("ROTATION")]
    [SerializeField] private float minStartRotation = -20f;
    [SerializeField] private float maxStartRotation = 20f;
    [SerializeField] private float rotationDrift = 30f;

    [Header("SIZE")]
    [SerializeField] private float punchScale = 1.3f;
    [SerializeField] private float punchDecay = 0.08f;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("GOLDEN")]
    [SerializeField] private Color textColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color goldenColor = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private float goldenSizeMultiplier = 1.5f;

    private TextBlock text;
    private Transform tr;
    private ClipMask clipMask;

    private Vector3 velocity;
    private float elapsed;
    private float baseScale;
    private float punch;
    private float swayPhase;
    private float startRotation;
    private float rotationSpeed;
    private Color baseColor;
    private System.Action<FloatingText> onDone;
    private bool finished;

    public float Elapsed => elapsed;
    public bool Finished => finished;

    void Awake(){
        text = GetComponent<TextBlock>();
        tr = GetComponent<Transform>();
        clipMask = GetComponent<ClipMask>();
        if(clipMask == null) clipMask = GetComponentInParent<ClipMask>();
    }

    public void Play(Vector3 worldPos, string content, bool golden, System.Action<FloatingText> onComplete){
        onDone = onComplete;
        finished = false;
        elapsed = 0f;
        punch = 1f;
        swayPhase = Random.Range(0f, Mathf.PI * 2f);

        tr.position = worldPos;
        text.Position.X.Value = text.Position.Y.Value = 0f;
        text.Text = content;

        startRotation = Random.Range(minStartRotation, maxStartRotation);
        rotationSpeed = Random.Range(-rotationDrift, rotationDrift);
        tr.localRotation = Quaternion.Euler(0f, 0f, startRotation);


        float randomSize = Random.Range(0.9f, 1.15f) * (golden ? goldenSizeMultiplier : 1f);
        baseScale = randomSize;
        tr.localScale = Vector3.one * baseScale;

        baseColor = golden ? goldenColor : textColor;
        text.Color = baseColor;

        if(clipMask != null){
            Color tint = clipMask.Tint;
            tint.a = 1f;
            clipMask.Tint = tint;
        }

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float horizontalSpeed = Random.Range(horizontalSpread * 0.5f, horizontalSpread);
        velocity = new Vector3(Mathf.Cos(angle) * horizontalSpeed, initialUpwardSpeed, 0f);
    }

    void Update(){
        if(finished) return;

        elapsed += Time.deltaTime;

        velocity.y += gravity * Time.deltaTime;
        tr.position += velocity * Time.deltaTime;

        float sway = Mathf.Sin(elapsed * swayFrequency + swayPhase) * swayAmplitude;
        Vector3 pos = tr.position;
        pos.x += sway * Time.deltaTime;
        tr.position = pos;

        float currentRotation = startRotation + rotationSpeed * elapsed;
        tr.localRotation = Quaternion.Euler(0f, 0f, currentRotation);

        if(punch > 0f) punch = Mathf.MoveTowards(punch, 0f, Time.deltaTime / punchDecay);
        float scale = baseScale * (1f + punch * (punchScale - 1f));
        tr.localScale = Vector3.one * scale;

        float fadeStart = lifetime - fadeDuration;
        if(elapsed >= fadeStart && clipMask != null){
            float alpha = Mathf.Clamp01(1f - (elapsed - fadeStart) / fadeDuration);
            Color tint = clipMask.Tint;
            tint.a = alpha;
            clipMask.Tint = tint;
        }

        if(elapsed >= lifetime){
            finished = true;
            onDone?.Invoke(this);
        }
    }
}