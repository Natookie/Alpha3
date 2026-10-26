using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField] private FloatingText prefab;
    [SerializeField] private Transform textParent;
    [SerializeField] private int poolSize = 25;
    [SerializeField] private float scatterRadius = 0.3f;

    private readonly Queue<FloatingText> pool = new Queue<FloatingText>();

    void Awake(){
        if(Instance != null && Instance != this){ Destroy(gameObject); return; }
        Instance = this;

        for(int i = 0; i < poolSize; i++){
            FloatingText ft = Instantiate(prefab, textParent);
            ft.gameObject.SetActive(false);
            pool.Enqueue(ft);
        }
    }

    public void Spawn(Vector3 worldPos, string content, bool golden, float delay = 0f){
        if(delay > 0f){
            StartCoroutine(SpawnAfterDelay(worldPos, content, golden, delay));
            return;
        }
        SpawnNow(worldPos, content, golden);
    }

    private IEnumerator SpawnAfterDelay(Vector3 worldPos, string content, bool golden, float delay){
        yield return new WaitForSeconds(delay);
        SpawnNow(worldPos, content, golden);
    }

    void SpawnNow(Vector3 worldPos, string content, bool golden){
        FloatingText ft;
        if(pool.Count > 0) ft = pool.Dequeue();
        else ft = RecycleOldestActive();

        if(ft == null) return;

        Vector2 scatter = Random.insideUnitCircle * scatterRadius;
        Vector3 pos = worldPos + new Vector3(scatter.x, scatter.y, 0f);

        ft.gameObject.SetActive(true);
        ft.Play(pos, content, golden, ReturnToPool);
    }

    FloatingText RecycleOldestActive(){
        FloatingText oldest = null;
        float maxAge = 0f;

        if(textParent == null) return null;
        for(int i = 0; i < textParent.childCount; i++){
            FloatingText ft = textParent.GetChild(i).GetComponent<FloatingText>();
            if(ft == null || !ft.gameObject.activeSelf) continue;
            if(ft.Elapsed > maxAge){
                maxAge = ft.Elapsed;
                oldest = ft;
            }
        }

        return oldest;
    }

    void ReturnToPool(FloatingText ft){
        ft.gameObject.SetActive(false);
        pool.Enqueue(ft);
    }
}