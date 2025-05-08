using System;
using TMPro;
using UnityEngine;

namespace Meteorite{
public class GameManager : Singleton<GameManager>{
    public Records records;
    public GameObject gameStartPanel;
    public StatsPanel statsPanel;
    public float testInterval, restInterval;
    public int numOfRounds;

    [HideInInspector][NonSerialized] public int numRoundsPlayed, roundsSuccess;
    [HideInInspector][NonSerialized] public string playerName;
    RoundInfo[] stats;
    void Start(){
        Spawner.inst.onEnemyDefeated+=FinishRound;
        stats=new RoundInfo[numOfRounds];
        statsPanel.gameObject.SetActive(false);
    }
    public void FinishRound(bool success){
        stats[numRoundsPlayed]=new RoundInfo(success, PlayerCtrl.inst.GetAccuracy(Spawner.inst.enemyOnTheLeft));
        ++numRoundsPlayed;
        if(success) roundsSuccess++;
    }
    public void StartGame(TMP_InputField nameField){
        gameStartPanel.SetActive(false);
        Spawner.inst.Spawn();
        playerName=nameField.text;
    }
    public void EndGame(){
        statsPanel.gameObject.SetActive(true);
        records.AddRecord(new Records.Record(playerName, Score, Accuracy));
        statsPanel.DisplayStats(stats);
    }
    public float Score{ get=>(float)roundsSuccess/numRoundsPlayed;}
    public float Accuracy{
        get{
            float accuracy=0;
            for(int i=stats.Length-1;i>-1;--i){
                accuracy+=stats[i].accuracy;
            }
            accuracy/=stats.Length;
            return accuracy;
        }
    }
    public class RoundInfo{
        public bool success;
        public float accuracy;
        public RoundInfo(bool success, float accuracy){
            this.success=success;
            this.accuracy=accuracy;
        }
    }
}
}