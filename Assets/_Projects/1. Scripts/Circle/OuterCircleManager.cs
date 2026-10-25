using System.Collections;
using UnityEngine;

public class OuterCircleManager : MonoBehaviour
{
    [SerializeField] private GameObject outerCirclePrefab;
    [SerializeField] private Transform outerCircleParent;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private float minSpawnDelay = 0f;
    [SerializeField] private float maxSpawnDelay = 0.5f;

    private Transform tr;

    void Start(){
        tr = GetComponent<Transform>();
        SpawnCircles();

        PlayerStats.Instance.OnOuterCircleCountChanged += HandleOuterCountChanged;
        PlayerStats.Instance.OnCircleShrinkDurationChanged += HandleCircleSpeedChanged;
    }

    void OnDisable(){
        if(PlayerStats.Instance == null) return;
        PlayerStats.Instance.OnOuterCircleCountChanged -= HandleOuterCountChanged;
        PlayerStats.Instance.OnCircleShrinkDurationChanged -= HandleCircleSpeedChanged;
    }

    void SpawnCircles(){
        int count = PlayerStats.Instance.OuterCircleCount;
        if(count <= 0 || outerCirclePrefab == null) return;

        for(int i = 0; i < count; i++){
            float angle = i * Mathf.PI * 2f / count;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;
            Vector3 spawnPos = tr.position + offset;
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            StartCoroutine(SpawnCircleAfterDelay(spawnPos, delay));
        }
    }

    IEnumerator SpawnCircleAfterDelay(Vector3 spawnPos, float delay){
        yield return new WaitForSeconds(delay);
        GameObject circle = Instantiate(outerCirclePrefab, spawnPos, Quaternion.identity, outerCircleParent);
        circle.GetComponent<OuterCircle>().SetShrinkDuration(PlayerStats.Instance.CircleShrinkDuration);
    }

    void HandleOuterCountChanged(int newCount) => RefreshCircles();
    void HandleCircleSpeedChanged(float newSpeed) => UpdateChildSpeeds(newSpeed);

    void UpdateChildSpeeds(float newSpeed){
        if(outerCircleParent == null) return;
        foreach(Transform child in outerCircleParent){
            OuterCircle oc = child.GetComponent<OuterCircle>();
            if(oc != null) oc.SetShrinkDuration(newSpeed);
        }
    }

    public void RefreshCircles(){
        StopAllCoroutines();

        if(outerCircleParent != null){
            for(int i = outerCircleParent.childCount - 1; i >= 0; i--){
                Destroy(outerCircleParent.GetChild(i).gameObject);
            }
        }

        SpawnCircles();
    }
}