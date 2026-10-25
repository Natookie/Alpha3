using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private int poolSize = 10;

    private readonly Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

    void Awake(){
        if(Instance != null && Instance != this){ Destroy(gameObject); return; }
        Instance = this;

        for(int i = 0; i < poolSize; i++){
            ParticleSystem ps = Instantiate(particlePrefab, transform);
            ps.gameObject.SetActive(false);
            pool.Enqueue(ps);
        }
    }

    public void Spawn(Vector3 position){
        ParticleSystem ps;
        if(pool.Count > 0) ps = pool.Dequeue();
        else ps = Instantiate(particlePrefab, transform);

        ps.transform.position = position;
        ps.gameObject.SetActive(true);
        ps.Clear(true);
        ps.Play(true);

        StartCoroutine(ReturnToPoolWhenDone(ps));
    }

    private System.Collections.IEnumerator ReturnToPoolWhenDone(ParticleSystem ps){
        while(ps != null && ps.IsAlive(true)) yield return null;
        if(ps == null) yield break;
        ps.gameObject.SetActive(false);
        pool.Enqueue(ps);
    }
}