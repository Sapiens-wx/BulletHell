using System;
using UnityEngine;

namespace Bullet{
public class BulletBase : MonoBehaviour
{
    public BulletManager.BulletType type;
    [HideInInspector][NonSerialized] public Vector2 velocity;

    void FixedUpdate(){
        transform.position+=new Vector3(velocity.x*Time.fixedDeltaTime, velocity.y*Time.fixedDeltaTime,0);
        //if outside bounds, destroy the bullet
        Vector2 pos=transform.position;
        if(pos.x<GameManager.inst.bounds.min.x) DestroyBullet();
        else if(pos.x>GameManager.inst.bounds.max.x) DestroyBullet();
        else if(pos.y<GameManager.inst.bounds.min.y) DestroyBullet();
        else if(pos.y>GameManager.inst.bounds.max.y) DestroyBullet();
    }
    public void DestroyBullet(){
        BulletManager.inst.ReleaseBullet(this);
    }
}
}