using UnityEngine;
using TMPro;
using ResearchUtilities;
using System.Collections;

namespace HideNSeek{
public class ScoreRecorder : Singleton<ScoreRecorder>{
    [Header("Score")]
    public TMP_Text text_score;
    public TMP_Text tips;
    public float tipsDuration, tipsSpd;

    bool inRound;
    int round;
    float score;
    float formulaScore, formulaScore_sum, maxDist, v_bar, v_bar_samples;
    Coroutine tipsCoro;
    void Start(){
        text_score.text="Score/Rounds: 0/0";
        tips.gameObject.SetActive(false);
    }
    void OnDisable(){
        //EventCollector.Instance.CollectAllEventLogsToFile();
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
        if(success){
            score+=1;
            if(tipsCoro!=null) StopCoroutine(tipsCoro);
            tipsCoro=StartCoroutine(TipAnim(new Vector3(GameManager.inst.onTheLeft?GameManager.inst.leftKid.transform.position.x:GameManager.inst.rightKid.transform.position.x,tips.transform.position.y,0)));
        }
        v_bar/=v_bar_samples*PlayerCtrl.inst.MoveSpd;
        formulaScore_sum+=maxDist*v_bar;
        formulaScore=formulaScore_sum/round;
        text_score.text="Score/Rounds: "+score.ToString("0")+"/"+round.ToString();
        text_score.text=$"Score/Rounds: {score.ToString("0")}/{round}\nControllability Score: {formulaScore}";
        //EventCollector.Instance.RecordEvent($"Round Results", $"Round {round}: {text_score.text}");
    }
    IEnumerator TipAnim(Vector3 startPos){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float spd=tipsSpd*Time.fixedDeltaTime;
        float toTime=Time.time+tipsDuration;
        Color originalColor=tips.color, curColor=originalColor;
        Vector3 displacement=new Vector3(0,spd,0);
        float normalizedDt=Time.fixedDeltaTime/tipsDuration;
        tips.transform.position=startPos;
        tips.gameObject.SetActive(true);
        while(Time.time<toTime){
            tips.transform.position+=displacement;
            curColor.a-=normalizedDt;
            tips.color=curColor;
            yield return wait;
        }
        tips.gameObject.SetActive(false);
        tips.transform.position=startPos;
        tips.color=originalColor;
        tipsCoro=null;
    }
}
}