using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Meteorite{
public class StatsCell : MonoBehaviour{
    public Image bgimg,img;
    public TMP_Text accuracy;
    public void Display(GameManager.RoundInfo info){
        bgimg.color=info.success?Color.green:Color.red;
        //accuracy
        accuracy.text=$"Accuracy: {(info.accuracy*100).ToString("0.##")}%";
        img.fillAmount=info.accuracy;
    }
}
}