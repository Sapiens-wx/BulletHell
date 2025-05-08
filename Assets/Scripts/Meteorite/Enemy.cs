using System.Collections;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace Meteorite{
public class Enemy : MonoBehaviour{
    Rigidbody2D rgb;
    Coroutine coroutine;
    void Start(){
        rgb=GetComponent<Rigidbody2D>();
    }
    public void Init(){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        gameObject.SetActive(true);
        rgb.velocity=new Vector2(0,-Spawner.inst.screenHeight/GameManager.inst.testInterval);
        coroutine=StartCoroutine(DelayDisable());
    }
    public void DestroyEnemy(bool autoDestroy){
        if(coroutine!=null){
            StopCoroutine(coroutine);
            coroutine=null;
        }
        gameObject.SetActive(false);
        Spawner.inst.OnEnemyDefeated(!autoDestroy);
        //explosion effects
        if(!autoDestroy)
            ExplosionEffect.inst.Play(transform.position);
    }
    IEnumerator DelayDisable(){
        yield return new WaitForSeconds(GameManager.inst.testInterval);
        coroutine=null;
        DestroyEnemy(true);
    }
}
}