using System;
using UnityEngine;

namespace HideNSeek{
    public class PlayerCtrl : Singleton<PlayerCtrl>
    {
        public float walkDuration; //amount of time it takes to walk from 0 to GameManager.inst.boundX
        public bool correctMovement;
        [Header("Animation")]
        public float maxShrinkYScale;
        public Transform eyes,spr;

        bool isAtRest;
        int inputX;
        float delta_x, moveSpd;
        float yScale,prevX,eyeOriginalPosY;
        const float maxXSpd=5;
        [NonSerialized][HideInInspector]public float vx;
        public float MoveSpd=>moveSpd;
        Vector3 startPos;
        // Start is called before the first frame update
        void Start()
        {
            eyeOriginalPosY=eyes.localPosition.y;
            inputX=0;
            yScale=1;
            moveSpd=GameManager.inst.boundX/walkDuration;
            delta_x=Time.fixedDeltaTime*moveSpd;
            startPos=transform.position;
            GameManager.inst.onRoundEnd+=(success)=>{transform.position=startPos; inputX=0; isAtRest=true;};
            GameManager.inst.onRoundBegin+=(time,dir)=>{isAtRest=false;};
        }

        void FixedUpdate()
        {
            if(!isAtRest){
                int key=(int)Input.GetAxisRaw("Horizontal");
                inputX = key;
                //if the player direction is not the same as the kid's direction
                if(correctMovement&&GameManager.inst.onTheLeft!=(inputX<=0))
                    inputX=0;
                if(inputX>0&&transform.position.x<=GameManager.inst.boundX)
                    transform.position+=new Vector3(delta_x,0,0);
                else if(inputX<0&&transform.position.x>=-GameManager.inst.boundX)
                    transform.position+=new Vector3(-delta_x,0,0);
            }
            //animate player
            float x=transform.position.x;
            vx=Mathf.Abs(x-prevX)/Time.fixedDeltaTime;
            if(isAtRest) vx=0;
            yScale=Mathf.Lerp(yScale,1-vx/maxXSpd*maxShrinkYScale,.1f);
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