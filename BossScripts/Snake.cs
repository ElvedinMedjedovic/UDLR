using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int gridPosition;
    public Vector2Int gridDirection = Vector2Int.right;
    public float moveTimerMax = 0.2f;
    private float moveTimer;

    private List<Transform> snakeSegments;
    private bool ateFood;

    void Start()
    {
        snakeSegments = new List<Transform>();
        snakeSegments.Add(this.transform);

        for (int i = 1; i < 4; i++)
        {
            Transform segment = Instantiate(segmentPrefab);
            segment.position = new Vector3(-i, 0, 0);
            snakeSegments.Add(segment);
        }
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && gridDirection != Vector2Int.down)
            gridDirection = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S) && gridDirection != Vector2Int.up)
            gridDirection = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A) && gridDirection != Vector2Int.right)
            gridDirection = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D) && gridDirection != Vector2Int.left)
            gridDirection = Vector2Int.right;
    }

    private void HandleMovement()
    {
        moveTimer += Time.deltaTime;

        if (moveTimer >= moveTimerMax)
        {
            moveTimer = 0;

            Vector3 previousPosition = snakeSegments[0].position;
            Vector3 newPosition = previousPosition + new Vector3(gridDirection.x, gridDirection.y, 0);

            for (int i = snakeSegments.Count - 1; i > 0; i--)
            {
                snakeSegments[i].position = snakeSegments[i - 1].position;
            }

            snakeSegments[0].position = newPosition;

            if (ateFood)
            {
                Transform segment = Instantiate(segmentPrefab);
                segment.position = snakeSegments[snakeSegments.Count - 1].position;
                snakeSegments.Add(segment);
                ateFood = false;
            }

            CheckCollision(newPosition);
        }
    }

    private void CheckCollision(Vector3 newPosition)
    {
        if (newPosition.x < -10 || newPosition.x > 10 || newPosition.y < -10 || newPosition.y > 10)
        {
            GameOver();
        }

        for (int i = 1; i < snakeSegments.Count; i++)
        {
            if (snakeSegments[i].position == newPosition)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        Debug.Log("Game Over");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Food")
        {
            ateFood = true;
            Destroy(collision.gameObject);
        }
    }
}
