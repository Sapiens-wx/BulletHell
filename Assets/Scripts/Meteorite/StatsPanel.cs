using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Meteorite{
public class StatsPanel : MonoBehaviour{
    [SerializeField] TMP_Text successfulRounds, averageAccuracy;
    [SerializeField] GridLayoutGroup statsGrid, recordsGrid;
    [SerializeField] GameObject statsCellPrefab, recordCellPrefab;
    //records
    public void DisplayStats(GameManager.RoundInfo[] stats){
        //number of successful rounds
        successfulRounds.text=$"Score: {GameManager.inst.roundsSuccess}/{GameManager.inst.numRoundsPlayed} ({(GameManager.inst.Score*100).ToString("0.##")}%)";
        //average accuracy
        averageAccuracy.text=$"Accuracy: {(GameManager.inst.Accuracy*100).ToString("0.##")}%";
        //individual stats
        for(int i=0;i<stats.Length;++i){
            GameObject cell=Instantiate(statsCellPrefab,statsGrid.transform);
            cell.GetComponent<StatsCell>().Display(stats[i]);
        }
        //rank (records)
        List<Records.Record> records=GameManager.inst.records.records;
        for(int i=0;i<records.Count;++i){
            GameObject cell=Instantiate(recordCellPrefab,recordsGrid.transform);
            cell.GetComponent<RecordCell>().Display(records[i]);
        }
    }
}
}