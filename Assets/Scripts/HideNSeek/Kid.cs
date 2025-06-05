using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid : MonoBehaviour
{
    public Transform eyes, spr;

    float eyeOriginalPosY;
    // Start is called before the first frame update
    void Start()
    {
        eyeOriginalPosY=eyes.transform.localPosition.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos=eyes.transform.localPosition;
        pos.y=eyeOriginalPosY*spr.transform.localScale.y;
        eyes.transform.localPosition=pos;
    }
}
