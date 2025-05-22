using UnityEngine;
using System.Collections;
using System;

namespace Meteorite{
public class Spawner : Singleton<Spawner>{
    public Enemy enemyPrefab;
    public float spawnX;

    [NonSerialized] public Enemy spawnedEnemy;
    [NonSerialized] public bool enemyOnTheLeft;
    [NonSerialized] public float screenHeight;
    public event System.Action onSpawn;
    public event System.Action<bool> onEnemyDefeated;
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawnX*2,10,0));
    }
    void Start(){
        spawnedEnemy=Instantiate(enemyPrefab.gameObject).GetComponent<Enemy>();
        screenHeight=Camera.main.orthographicSize*2;
    }
    public void Spawn(){
        enemyOnTheLeft=UnityEngine.Random.Range(0,2)==0;
        spawnedEnemy.transform.position=new Vector3(enemyOnTheLeft?-spawnX:spawnX,screenHeight/2,0);
        spawnedEnemy.Init();
        onSpawn?.Invoke();
        //send message to predict.py
        Communicator.inst.SendMessage(new byte[]{(byte)(enemyOnTheLeft?0:1)});
    }
    public void OnEnemyDefeated(bool success){
        onEnemyDefeated?.Invoke(success);
        if(GameManager.inst.numRoundsPlayed==GameManager.inst.numOfRounds)
            GameManager.inst.EndGame();
        else
            StartCoroutine(RespawnEnemy());
        //send message to predict.py
        Communicator.inst.SendMessage(new byte[]{2});
    }
    IEnumerator RespawnEnemy(){
        yield return new WaitForSeconds(GameManager.inst.restInterval);
        Spawn();
    }
}
}