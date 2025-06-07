using UnityEngine;
using TMPro;
using ResearchUtilities;

namespace HideNSeek{
public class ScoreRecorder : Singleton<ScoreRecorder>{
    [Header("Score")]
    public TMP_Text text_score;

    bool inRound;
    int round;
    float score;
    float formulaScore, formulaScore_sum, maxDist, v_bar, v_bar_samples;
    void Start(){
        text_score.text="Score/Rounds: 0/0";
    }
    void OnDisable(){
        EventCollector.Instance.CollectAllEventLogsToFile();
    }
    void FixedUpdate(){
        if(inRound){
            maxDist=Mathf.Max(maxDist,PlayerCtrl.inst.transform.position.x/(GameManager.inst.onTheLeft?-GameManager.inst.boundX:GameManager.inst.boundX));
            v_bar+=PlayerCtrl.inst.vx;
            ++v_bar_samples;
        }
    }
    public void BeginRound(){
        inRound=true;
        maxDist=0;
        v_bar=0;
        v_bar_samples=0;
    }
    public void EndRound(bool success){
        inRound=false;
        ++round;
        //update score
        if(success) score+=1;
        v_bar/=v_bar_samples*PlayerCtrl.inst.MoveSpd;
        formulaScore_sum+=maxDist*v_bar;
        formulaScore=formulaScore_sum/round;
        text_score.text="Score/Rounds: "+score.ToString("0")+"/"+round.ToString();
        text_score.text=$"Score/Rounds: {score.ToString("0")}/{round}\nFormula Score: {formulaScore}";
        EventCollector.Instance.RecordEvent($"Round Results", $"Round {round}: {text_score.text}");
    }
}
}