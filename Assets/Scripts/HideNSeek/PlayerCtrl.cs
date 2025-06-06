using UnityEngine;

namespace HideNSeek{
    public class PlayerCtrl : Singleton<PlayerCtrl>
    {
        public float walkDuration; //amount of time it takes to walk from 0 to GameManager.inst.boundX
        [Header("Animation")]
        public float maxShrinkYScale;
        public Transform eyes,spr;

        bool isAtRest;
        int inputX;
        float xspd;
        float yScale,prevX,eyeOriginalPosY;
        const float maxXSpd=5;
        // Start is called before the first frame update
        void Start()
        {
            eyeOriginalPosY=eyes.localPosition.y;
            inputX=0;
            yScale=1;
            xspd=Time.fixedDeltaTime*HideAndSeekManager.inst.boundX/walkDuration;
            HideAndSeekManager.inst.onRoundEnd+=(success)=>{transform.position=Vector3.zero; inputX=0; isAtRest=true;};
            HideAndSeekManager.inst.onRoundBegin+=(time,dir)=>{isAtRest=false;};
        }

        void FixedUpdate()
        {
            if(!isAtRest){
                int key=(int)Input.GetAxisRaw("Horizontal");
                if(key!=0) inputX=key;
                //if the player direction is not the same as the kid's direction
                if(HideAndSeekManager.inst.onTheLeft!=(inputX<=0))
                    inputX=0;
                if(inputX>0&&transform.position.x<=HideAndSeekManager.inst.boundX)
                    transform.position+=new Vector3(xspd,0,0);
                else if(inputX<0&&transform.position.x>=-HideAndSeekManager.inst.boundX)
                    transform.position+=new Vector3(-xspd,0,0);
            }
            //animate player
            float x=transform.position.x;
            float v=Mathf.Abs(x-prevX)/Time.fixedDeltaTime;
            if(isAtRest) v=0;
            yScale=Mathf.Lerp(yScale,1-v/maxXSpd*maxShrinkYScale,.1f);
            float yOffset=(yScale-1)*.5f;
            Vector3 pos=spr.transform.localPosition, scale=spr.transform.localScale;
            pos.y=yOffset;
            scale.y=yScale;
            scale.x=2-yScale;
            spr.transform.localPosition=pos;
            spr.transform.localScale=scale;
            prevX=x;
            //eyes movement
            pos=eyes.localPosition;
            pos.x=Mathf.Lerp(pos.x, inputX*(1-yScale), .1f);
            pos.y=eyeOriginalPosY+yOffset;
            eyes.localPosition=pos;
        }
    }
}