using UnityEngine;

namespace Bullet{
public class EnemyBase : MonoBehaviour{
    protected SpriteRenderer spr;
    protected virtual void Start(){
        spr=GetComponent<SpriteRenderer>();
    }
    void OnTriggerEnter2D(Collider2D collider){
        //collide with player bullet
        if(GameManager.IsLayer(GameManager.inst.layer_playerBullet, collider.gameObject.layer)){
            collider.GetComponent<BulletBase>().DestroyBullet();
            //hit effect
            Effects.HitEffect(spr);
        }
    }
}
}