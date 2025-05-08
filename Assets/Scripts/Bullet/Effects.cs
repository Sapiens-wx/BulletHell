using UnityEngine;
using System.Collections;

public class Effects:Singleton<Effects>{
    [SerializeField] float hitDuration;
    public static void HitEffect(SpriteRenderer spr){
        inst.StartCoroutine(inst.HitEffectAnim(spr));
    }
    IEnumerator HitEffectAnim(SpriteRenderer spr){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float t=Time.time+hitDuration;
        float nt=0,dt=Time.fixedDeltaTime/hitDuration;

        MaterialPropertyBlock matPB=new MaterialPropertyBlock();

        while(Time.time<t){
            spr.GetPropertyBlock(matPB);
            matPB.SetFloat("whiteness", Easing.SinYoyo(nt));
            spr.SetPropertyBlock(matPB);
            nt+=dt;
            yield return wait;
        }
        spr.GetPropertyBlock(matPB);
        matPB.SetFloat("whiteness", 0);
        spr.SetPropertyBlock(matPB);
    }
}