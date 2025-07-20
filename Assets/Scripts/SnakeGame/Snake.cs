using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public GameObject bodyPrefab;
    public GameObject tailPrefab;

    public static Snake Instance;

    private Vector2 minViewport;
    private Vector2 maxViewport;

    private Vector2 direction = Vector2.right;
    private float timer;

    private List<Transform> bodyParts = new List<Transform>(); // 不含头和尾
    private Transform tail;
    private Vector3 lastHeadPosition;
    [SerializeField] private float tailRotationOffset = 0f;

    //f_i
    public int foodCollected = 0;

    //v_i
    private float totalDistance = 0f;
    private float totalTime = 0f;
    private Vector3 lastPosition;

    //s_i
    public int totalSwitches = 0;
    public List<int> switchesPerFood = new List<int>();
    private Vector2 lastDirection = Vector2.right; // 初始方向
    private int currentSwitchCount = 0;

    void Start()
    {
        // 创建尾巴，初始放在蛇头左边
        tail = Instantiate(tailPrefab).transform;
        tail.position = transform.position - new Vector3(1, 0, 0); // 初始方向向右
        tail.rotation = Quaternion.Euler(0, 0, 90);

        //v_i
        lastPosition = transform.position;

        //camera 边界
        minViewport = Camera.main.ViewportToWorldPoint(Vector2.zero);
        maxViewport = Camera.main.ViewportToWorldPoint(Vector2.one);

        Instance = this;

    }

    void Update()
    {
        HandleInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float score = GetSnakeScore();
            Debug.Log($"Score: {score}, Food: {foodCollected}, AvgSpeed: {GetAverageSpeed()}, AvgSwitch: {totalSwitches / (float)switchesPerFood.Count}");
        }

    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= moveSpeed)
        {
            Move();
            CheckBoundaryAndReflect();
            timer = 0f;
        }

        //v_i
        float distance = Vector3.Distance(transform.position, lastPosition);
        totalDistance += distance;
        totalTime += Time.fixedDeltaTime;
        lastPosition = transform.position;

    }

    //v_i
    public float GetAverageSpeed()
    {
        return totalTime > 0 ? totalDistance / totalTime : 0f;
    }


    void HandleInput()
    {
        Vector2 newDirection = direction;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // 向左转
        {
            newDirection = new Vector2(-direction.y, direction.x);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // 向右转
        {
            newDirection = new Vector2(direction.y, -direction.x);
        }

        // s_i, num of switches
        if (newDirection != direction)
        {
            direction = newDirection;
            currentSwitchCount++;
            totalSwitches++;
        }

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
            tail.rotation = Quaternion.Euler(0, 0, 180);
        else if (tailDir == Vector2.down)
            tail.rotation = Quaternion.Euler(0, 0, 0);
        else if (tailDir == Vector2.left)
            tail.rotation = Quaternion.Euler(0, 0, -90);
        else if (tailDir == Vector2.right)
            tail.rotation = Quaternion.Euler(0, 0, 90);
    }

    void CheckBoundaryAndReflect()
    {
        Vector2 headPos = transform.position;
        bool reflected = false;

        if (headPos.x < minViewport.x || headPos.x > maxViewport.x)
        {
            direction = new Vector2(-direction.x, direction.y);
            reflected = true;
        }

        if (headPos.y < minViewport.y || headPos.y > maxViewport.y)
        {
            direction = new Vector2(direction.x, -direction.y);
            reflected = true;
        }

        if (reflected)
        {
            transform.position = new Vector3(
                Mathf.Clamp(headPos.x, minViewport.x, maxViewport.x),
                Mathf.Clamp(headPos.y, minViewport.y, maxViewport.y),
                0f
            );
        }
    }

    public void Grow()
    {
        // 新身体生成在尾巴原本的位置
        Vector3 insertPos = tail.position;
        GameObject newBody = Instantiate(bodyPrefab, insertPos, Quaternion.identity);
        newBody.tag = "Body";

        // 在 body 列表中加入新的身体，排在尾巴前一位
        bodyParts.Add(newBody.transform);

        //这次吃到食物时的switch num
        switchesPerFood.Add(currentSwitchCount);
        currentSwitchCount = 0;

    }

    public float GetSnakeScore(int foodMax = 20, float maxSpeed = 5f, int maxSwitch = 5)
    {
        float f_norm = Mathf.Clamp01((float)foodCollected / foodMax);
        float v_norm = Mathf.Clamp01(GetAverageSpeed() / maxSpeed);
        float s_avg = 0f;

        if (switchesPerFood.Count > 0)
            s_avg = (float)totalSwitches / switchesPerFood.Count;

        float s_norm = 1f - Mathf.Clamp01(s_avg / maxSwitch);

        return f_norm * v_norm * s_norm;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }
}

