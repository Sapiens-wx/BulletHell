using UnityEngine;

namespace Meteorite{
public class PlayerCtrl : Singleton<PlayerCtrl>
{
    [SerializeField] float threshold;
    [SerializeField] ProgressBar leftBar, rightBar;

    int leftCnt, rightCnt, totalCnt;
    /// <summary>
    /// is an output already generated for this spawn?
    /// </summary>
    bool outputGenerated;
    bool receiveInput;
    void Start()
    {
        Spawner.inst.onSpawn+=OnSpawn;
        Spawner.inst.onEnemyDefeated+=OnEnemyDefeated;
        leftBar.SetProgress(0);
        rightBar.SetProgress(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(receiveInput){
            //handle input
            if(Input.GetKey(KeyCode.LeftArrow))
                ++leftCnt;
            else if(Input.GetKey(KeyCode.RightArrow))
                ++rightCnt;
            //finalize input
            if(!outputGenerated){
                bool isOutputLeft=false;
                float leftAmount=(float)leftCnt/totalCnt;
                float rightAmount=(float)rightCnt/totalCnt;
                leftBar.SetProgress(leftAmount);
                rightBar.SetProgress(rightAmount);
                if(leftAmount>=threshold){
                    //left
                    isOutputLeft=true;
                    outputGenerated=true;
                } else if(rightAmount>=threshold){
                    //right
                    isOutputLeft=false;
                    outputGenerated=true;
                }
                //destroy the enemy if output is correct
                if(outputGenerated&&isOutputLeft==Spawner.inst.enemyOnTheLeft){
                    Spawner.inst.spawnedEnemy.DestroyEnemy(false);
                }
            }
        }
    }
    public float GetAccuracy(bool isLeft){
        int total=leftCnt+rightCnt;
        if(total==0) return 0;
        if(isLeft)
            return (float)leftCnt/total;
        else
            return (float)rightCnt/total;
    }
    void OnSpawn(){
        receiveInput=true;
        leftBar.SetProgress(0);
        rightBar.SetProgress(0);
        outputGenerated=false;
        leftCnt=0;
        rightCnt=0;
        totalCnt=(int)(GameManager.inst.testInterval/Time.fixedDeltaTime);
    }
    void OnEnemyDefeated(bool success){
        receiveInput=false;
    }
}
}