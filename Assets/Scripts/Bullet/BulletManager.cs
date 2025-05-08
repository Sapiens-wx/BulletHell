using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Bullet{
public class BulletManager : Singleton<BulletManager>{
    public BulletBase[] prefabs;

    Dictionary<BulletType,Pool> pools;
    protected override void Awake()
    {
        base.Awake();
        pools=new Dictionary<BulletType, Pool>(prefabs.Length);
        for(int i=0;i<prefabs.Length;++i){
            BulletType type=prefabs[i].type;
            pools[type]=new Pool(prefabs[i]);
        }
    }
    public BulletBase GetBullet(BulletType type){
        return pools[type].Get();
    }
    public void ReleaseBullet(BulletBase bullet){
        pools[bullet.type].Release(bullet);
    }

    public enum BulletType{
        PlayerBullet,
        EnemyBullet
    }
    [System.Serializable]
    private class Pool{
        ObjectPool<BulletBase> pool;
        BulletBase prefab;
        public Pool(BulletBase prefab){
            pool=new ObjectPool<BulletBase>(Proj_CreateFunc, Proj_OnGet, Proj_OnRelease, Proj_OnPoolObjDestroy);
            this.prefab=prefab;
        }
        public BulletBase Get(){
            return pool.Get();
        }
        public void Release(BulletBase inst){
            pool.Release(inst);
        }
        BulletBase Proj_CreateFunc(){
            BulletBase go = Instantiate(prefab).GetComponent<BulletBase>();
            go.gameObject.SetActive(false);
            return go;
        }
        static void Proj_OnGet(BulletBase go){
            go.gameObject.SetActive(true);
        }
        static void Proj_OnRelease(BulletBase go){
            go.gameObject.SetActive(false);
        }
        static void Proj_OnPoolObjDestroy(BulletBase go){
            if(go!=null) Destroy(go.gameObject);
        }
    }
}
}