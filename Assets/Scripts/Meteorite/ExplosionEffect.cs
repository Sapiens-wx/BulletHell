using UnityEngine;

namespace Meteorite{
public class ExplosionEffect : Singleton<ExplosionEffect>
{
    [SerializeField] Animator animator;
    public void Play(Vector3 pos){
        transform.position=pos;
        animator.SetTrigger("explode");
        CameraShake.inst.Shake();
    }
}
}