using System.Collections;
using UnityEngine;

namespace Bullet{
public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] float spd;
    [SerializeField] float rotateAmount;
    Rigidbody2D rgb;
    SpriteRenderer spr;

    Vector2 input,rawInput;
    void Start(){
        rgb=GetComponent<Rigidbody2D>();
        spr=GetComponent<SpriteRenderer>();
        StartCoroutine(Test());
    }
    void FixedUpdate(){
        //rotation
        transform.eulerAngles=new Vector3(0,0,-input.x*rotateAmount);
        //velocity
        rgb.velocity=rawInput*spd;
        Vector2 pos=transform.position;
        if(pos.x<GameManager.inst.bounds.min.x) transform.position=new Vector3(GameManager.inst.bounds.min.x,pos.y,0);
        else if(pos.x>GameManager.inst.bounds.max.x) transform.position=new Vector3(GameManager.inst.bounds.max.x,pos.y,0);
        pos.x=transform.position.x;
        if(pos.y<GameManager.inst.bounds.min.y) transform.position=new Vector3(pos.x,GameManager.inst.bounds.min.y,0);
        else if(pos.y>GameManager.inst.bounds.max.y) transform.position=new Vector3(pos.x,GameManager.inst.bounds.max.y,0);
    }
    IEnumerator Test(){
        WaitForSeconds wait=new WaitForSeconds(.2f);
        while(true){
            BulletBase b=BulletManager.inst.GetBullet(BulletManager.BulletType.PlayerBullet);
            b.velocity=new Vector2(0,7);
            b.transform.position=transform.position;
            yield return wait;
        }
    }
    // Update is called once per frame
    void Update()
    {
        input.x=Input.GetAxis("Horizontal");
        input.y=Input.GetAxis("Vertical");
        rawInput.x=Input.GetAxisRaw("Horizontal");
        rawInput.y=Input.GetAxisRaw("Vertical");
    }
    void OnTriggerEnter2D(Collider2D collider){
        //collide with enemy bullet
        if(GameManager.IsLayer(GameManager.inst.layer_enemyBullet, collider.gameObject.layer)){
            collider.GetComponent<BulletBase>().DestroyBullet();
            //hit anim
            Effects.HitEffect(spr);
        }
    }
}
}