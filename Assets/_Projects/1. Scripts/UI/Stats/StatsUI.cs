using UnityEngine;
using Nova;

public class StatsUI : MonoBehaviour
{
    [Header("POINT ON HIT")]
    [SerializeField] private TextBlock pointOnHitText;

    [Header("CIRCLE SHRINK DURATION")]
    [SerializeField] private TextBlock circleShrinkDurationText;

    [Header("GOLDEN CIRCLE CHANCE")]
    [SerializeField] private TextBlock goldenCircleChanceText;

    [Header("OUTER CIRCLE COUNT")]
    [SerializeField] private TextBlock outerCircleCountText;

    [Header("GLOBAL MULTIPLIER")]
    [SerializeField] private TextBlock globalMultiplierText;

    private PlayerStats ps;

    void Start(){
        ps = PlayerStats.Instance;
        if(ps == null) return;

        ps.OnPointOnHitChanged += HandlePointOnHitChanged;
        ps.OnCircleShrinkDurationChanged += HandleCircleShrinkDurationChanged;
        ps.OnGoldenCircleChanceChanged += HandleGoldenCircleChanceChanged;
        ps.OnOuterCircleCountChanged += HandleOuterCircleCountChanged;
        ps.OnGlobalMultiplierChanged += HandleGlobalMultiplierChanged;

        RefreshAll();
    }

    void OnDisable(){
        if(ps == null) return;

        ps.OnPointOnHitChanged -= HandlePointOnHitChanged;
        ps.OnCircleShrinkDurationChanged -= HandleCircleShrinkDurationChanged;
        ps.OnGoldenCircleChanceChanged -= HandleGoldenCircleChanceChanged;
        ps.OnOuterCircleCountChanged -= HandleOuterCircleCountChanged;
        ps.OnGlobalMultiplierChanged -= HandleGlobalMultiplierChanged;
    }

    void RefreshAll(){
        HandlePointOnHitChanged(ps.PointOnHit);
        HandleCircleShrinkDurationChanged(ps.CircleShrinkDuration);
        HandleGoldenCircleChanceChanged(ps.GoldenCircleChance);
        HandleOuterCircleCountChanged(ps.OuterCircleCount);
        HandleGlobalMultiplierChanged(ps.GlobalMultiplier);
    }

    void HandlePointOnHitChanged(float value){
        if(pointOnHitText != null) pointOnHitText.Text = ": " + value.ToString("0.##");
    }

    void HandleCircleShrinkDurationChanged(float value){
        if(circleShrinkDurationText != null) circleShrinkDurationText.Text = ": " + value.ToString("0.##") + "s";
    }

    void HandleGoldenCircleChanceChanged(float value){
        if(goldenCircleChanceText != null) goldenCircleChanceText.Text = ": " + (value * 100f).ToString("0.#") + "%";
    }

    void HandleOuterCircleCountChanged(int value){
        if(outerCircleCountText != null) outerCircleCountText.Text = ": " + value.ToString();
    }

    void HandleGlobalMultiplierChanged(float value){
        if(globalMultiplierText != null) globalMultiplierText.Text = ": " + "x" + value.ToString("0.##");
    }
}