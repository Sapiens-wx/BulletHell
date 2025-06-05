using UnityEngine;

namespace HideNSeek{
    public class PlayerCtrl : Singleton<PlayerCtrl>
    {
        public float walkDuration; //amount of time it takes to walk from 0 to GameManager.inst.boundX

        bool isAtRest;
        int inputX;
        float xspd;
        // Start is called before the first frame update
        void Start()
        {
            inputX=0;
            xspd=Time.fixedDeltaTime*GameManager.inst.boundX/walkDuration;
            GameManager.inst.onRoundEnd+=(success)=>{transform.position=Vector3.zero; inputX=0; isAtRest=true;};
            GameManager.inst.onRoundBegin+=(time,dir)=>{isAtRest=false;};
        }

        void FixedUpdate()
        {
            if(!isAtRest){
                int key=(int)Input.GetAxisRaw("Horizontal");
                if(key!=0) inputX=key;
                //if the player direction is not the same as the kid's direction
                if(GameManager.inst.onTheLeft!=(inputX<=0))
                    inputX=0;
                if(inputX>0&&transform.position.x<=GameManager.inst.boundX)
                    transform.position+=new Vector3(xspd,0,0);
                else if(inputX<0&&transform.position.x>=-GameManager.inst.boundX)
                    transform.position+=new Vector3(-xspd,0,0);
            }
        }
    }
}