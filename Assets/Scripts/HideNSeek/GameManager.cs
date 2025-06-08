using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideNSeek{
    public class GameManager : Singleton<GameManager>
    {
        public Animator leftKid, rightKid;
        public float boundX;
        [Header("Timing")]
        public float restInterval;
        public float appearDuration;

        public System.Action<bool> onRoundEnd; //<success>
        public System.Action<double,int> onRoundBegin; //<time,direction>. direction=0: left, 1: right
        [NonSerialized][HideInInspector] public bool onTheLeft;
        Animator activeKid;
        Coroutine delayReappearCoro;
        void OnDrawGizmosSelected(){
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(boundX*2,10,0));
        }
        // Start is called before the first frame update
        void Start()
        {
            Appear();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //player enters left
            if(PlayerCtrl.inst.transform.position.x<=-boundX){
                if(delayReappearCoro!=null)
                    OnRoundEnd(onTheLeft);
            }
            //player enters right
            else if(PlayerCtrl.inst.transform.position.x>=boundX){
                if(delayReappearCoro!=null)
                    OnRoundEnd(!onTheLeft);
            }
        }
        public void Appear(){
            onTheLeft=UnityEngine.Random.Range(0,2)==0;
            activeKid=onTheLeft?leftKid:rightKid;
            activeKid.SetTrigger("appear");
            delayReappearCoro=StartCoroutine(DelayReappear());
            onRoundBegin?.Invoke(Time.time, onTheLeft?0:1);
            //send message to predict.py
            //Communicator.inst.SendMessage(new byte[]{(byte)(onTheLeft?0:1)});
            ScoreRecorder.inst.BeginRound();
        }
        public void OnRoundEnd(bool success){
            onRoundEnd?.Invoke(success);
            ScoreRecorder.inst.EndRound(success);
            if(delayReappearCoro!=null){
                StopCoroutine(delayReappearCoro);
                delayReappearCoro=null;
            }
            activeKid.SetTrigger("disappear");
            StartCoroutine(Reappear());
        }
        /// <summary>
        /// if the player fails to catch the kid, the automatically goto the next round
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayReappear(){
            yield return new WaitForSeconds(appearDuration);
            OnRoundEnd(false);
        }
        IEnumerator Reappear(){
            yield return new WaitForSeconds(GameManager.inst.restInterval);
            Appear();
        }
    }
}