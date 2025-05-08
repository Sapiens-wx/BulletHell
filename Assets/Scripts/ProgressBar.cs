using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image bar;
    public void SetProgress(float t){
        bar.fillAmount=t;
    }
}
