using System.Collections;
using UnityEngine;

namespace Meteorite{
public class CameraShake : Singleton<CameraShake>
{
    [SerializeField] float duration, amplitude;

    Camera cam;
    Coroutine shakeCoro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam=Camera.main;
    }

    public void Shake(){
        if(shakeCoro!=null){
            StopCoroutine(shakeCoro);
            shakeCoro=null;
        }
        shakeCoro=StartCoroutine(ShakeAnim());
    }
    IEnumerator ShakeAnim(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float t=Time.time+duration;
        float normalizedTime=0, normalizedDt=Time.fixedDeltaTime/duration;
        Vector3 originalPos=transform.position;
        while(Time.time<t){
            transform.position=originalPos+(Vector3)(UnityEngine.Random.insideUnitCircle*amplitude*Easing.OutQuint(normalizedTime));
            normalizedTime+=normalizedDt;
            yield return wait;
        }
        transform.position=originalPos;
    }
}
}