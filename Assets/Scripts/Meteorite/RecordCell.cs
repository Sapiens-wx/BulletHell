using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Meteorite{
public class RecordCell : MonoBehaviour{
    public TMP_Text nameField, dataField, scoreField, accuracyField;
    public void Display(Records.Record record){
        nameField.text=record.name;
        dataField.text=record.time.ToString("HH:mm yyyy/MM/dd");
        scoreField.text=$"Score: {(record.score*100).ToString("0.##")}%";
        accuracyField.text=$"Accuracy: {(record.accuracy*100).ToString("0.##")}%";
    }
}
}