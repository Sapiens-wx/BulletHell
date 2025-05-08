using System.Collections;
using UnityEngine;

namespace Bullet{
public class Enemy1 : EnemyBase{
    [SerializeField] int num;
    [SerializeField] float shootInterval, bulletSpd;
    [SerializeField] float spdToWidthRate;
    [SerializeField] float safeAreaWidth;
    [Header("Interval")]
    [SerializeField] float restTime;
    [SerializeField] float restRange, switchDirDuration, switchDirDurationRange;
    [Header("Noise")]
    [SerializeField] Vector2 noiseScale;
    [SerializeField] float noiseThreshold; //given a value -1~1, mask[i] with greator than [noiseThreshold] noise will be masked false

    float[] positions;
    float safeAreaX, safeAreaX_normalized, last_safeAreaX_normalized, safeAreaX_spd;
    protected override void Start(){
        base.Start();
        safeAreaX_normalized=1;
        positions=new float[num];
        float margin=GameManager.inst.bounds.size.x/num;
        float curX=GameManager.inst.bounds.min.x+margin/2;
        for(int i=0;i<num;++i){
            positions[i]=curX;
            curX+=margin;
        }
        StartCoroutine(ShootAnim());
        StartCoroutine(ShootDirSwitcher());
    }
    IEnumerator ShootDirSwitcher(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float restMinTime=restTime-0.5f*restRange, restMaxTime=restMinTime+restRange;
        float switchMinTime=switchDirDuration-0.5f*switchDirDurationRange, switchMaxTime=switchMinTime+switchDirDurationRange;
        while(true){
            float switchDuration=Random.Range(switchMinTime,switchMaxTime);
            float t=0, dt=Time.fixedDeltaTime/switchDuration;
            float xrange=safeAreaX_normalized>0?-2:2;
            for(;switchDuration>0;switchDuration-=Time.fixedDeltaTime,t+=dt){
                safeAreaX_normalized=(Easing.InOutQuad(t)-0.5f)*xrange;
                safeAreaX_spd=(safeAreaX_normalized-last_safeAreaX_normalized)/Time.fixedDeltaTime;
                last_safeAreaX_normalized=safeAreaX_normalized;
                yield return wait;
            }
            safeAreaX_spd=0;
            yield return new WaitForSeconds(Random.Range(restMinTime, restMaxTime));
        }
    }
    IEnumerator ShootAnim(){
        WaitForSeconds wait=new WaitForSeconds(shootInterval);
        float amp=(GameManager.inst.bounds.size.x-safeAreaWidth)/2;
        bool[] mask=new bool[positions.Length];
        while(true){
            safeAreaX=amp*safeAreaX_normalized;
            //update mask
            float actualWidth=safeAreaWidth*(1+spdToWidthRate*Mathf.Abs(safeAreaX_spd));
            float l=safeAreaX-actualWidth/2,r=l+actualWidth;
            for(int i=positions.Length-1;i>-1;--i){
                mask[i] = positions[i]>l && positions[i]<r;
                if(!mask[i]){
                    //generate noise
                    float t=(float)i/positions.Length;
                    float noise=Mathf.PerlinNoise(t*noiseScale.x,Time.time*noiseScale.y);
                    if(noise>noiseThreshold) mask[i]=true;
                }
            }
            //shoot
            Shoot(mask);
            yield return wait;
        }
    }
    /// <summary>
    /// if mask[i]==true, then does not generate bullet at that point
    /// </summary>
    /// <param name="mask"></param>
    void Shoot(bool[] mask){
        for(int i=0;i<positions.Length;++i){
            if(mask[i]) continue;
            BulletBase b=BulletManager.inst.GetBullet(BulletManager.BulletType.EnemyBullet);
            b.transform.position=new Vector3(positions[i],transform.position.y,0);
            b.velocity=new Vector2(0,-bulletSpd);
        }
    }
}
}