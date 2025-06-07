using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public GameObject bodyPrefab;
    public GameObject tailPrefab;

    private Vector2 direction = Vector2.right;
    private float timer;

    private List<Transform> bodyParts = new List<Transform>(); // 不含头和尾
    private Transform tail;
    private Vector3 lastHeadPosition;

    void Start()
    {
        // 创建尾巴，初始放在蛇头左边
        tail = Instantiate(tailPrefab).transform;
        tail.position = transform.position - new Vector3(1, 0, 0); // 初始方向向右
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= moveSpeed)
        {
            Move();
            timer = 0f;
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
            direction = Vector2.right;
    }

    void Move()
    {
        lastHeadPosition = transform.position;

        // 蛇头移动
        Vector3 previousPos = transform.position;
        transform.position += new Vector3(direction.x, direction.y, 0);

        // 身体依次移动并设置旋转
        for (int i = 0; i < bodyParts.Count; i++)
        {
            Vector3 temp = bodyParts[i].position;
            bodyParts[i].position = previousPos;
            previousPos = temp;

            // 获取方向：前一节身体 or 蛇头
            Vector3 forwardPartPos = (i == 0) ? transform.position : bodyParts[i - 1].position;
            Vector2 diff = (forwardPartPos - bodyParts[i].position);
            diff = new Vector2(Mathf.Round(diff.x), Mathf.Round(diff.y));

            float angle = 0;
            if (diff == Vector2.up)
                angle = 0;
            else if (diff == Vector2.down)
                angle = 180;
            else if (diff == Vector2.left)
                angle = 90;
            else if (diff == Vector2.right)
                angle = -90;

            bodyParts[i].rotation = Quaternion.Euler(0, 0, angle);
        }

        // 尾巴移动
        tail.position = previousPos;

        // 尾巴方向朝向最后一节 or 蛇头
        Vector3 last = (bodyParts.Count > 0 ? bodyParts[^1].position : transform.position);
        Vector2 tailDir = (last - tail.position);
        tailDir = new Vector2(Mathf.Round(tailDir.x), Mathf.Round(tailDir.y));

        if (tailDir == Vector2.up)
            tail.rotation = Quaternion.Euler(0, 0, 0);
        else if (tailDir == Vector2.down)
            tail.rotation = Quaternion.Euler(0, 0, 180);
        else if (tailDir == Vector2.left)
            tail.rotation = Quaternion.Euler(0, 0, 90);
        else if (tailDir == Vector2.right)
            tail.rotation = Quaternion.Euler(0, 0, -90);
    }


    public void Grow()
    {
        // 新身体生成在尾巴原本的位置
        Vector3 insertPos = tail.position;
        GameObject newBody = Instantiate(bodyPrefab, insertPos, Quaternion.identity);
        newBody.tag = "Body";

        // 在 body 列表中加入新的身体，排在尾巴前一位
        bodyParts.Add(newBody.transform);
    }
}

