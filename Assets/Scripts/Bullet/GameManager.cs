using UnityEngine;

namespace Bullet{
public class GameManager : Singleton<GameManager>{
    public Bounds bounds;
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
    public LayerMask layer_player, layer_enemyBullet, layer_enemy, layer_playerBullet; //groundLayer: platform|ground, platformLayer: platform
    public static bool IsLayer(LayerMask mask, int layer){
        return (mask.value&(1<<layer))!=0;
    }
}
}