using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MazeRender : MonoBehaviour
{
    [SerializeField] private Vector3 mazeOffset;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private GameObject wallPrefab;

    public void DrawMaze()
    {
        WallState[,] maze = MazeGenerator.Generator(width, height);

        for (int i = transform.childCount - 1; i > 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        Draw(maze);
        GameController.Instance.SpawnBall();
    }
    
    private void Draw(WallState[,] maze)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WallState cell = maze[x, y];
                Vector3 position = new Vector3(-width / 2 + x, 0.5f, -height / 2 + y) * cellSize + mazeOffset;

                if(cell.HasFlag(WallState.UP))
                {
                    GameObject wall = Instantiate(wallPrefab, transform);
                    wall.transform.localPosition = position + Vector3.forward * (cellSize / 2);
                    wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, wall.transform.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    GameObject wall = Instantiate(wallPrefab, transform);
                    wall.transform.localPosition = position - Vector3.right * (cellSize / 2);
                    wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, wall.transform.localScale.z);
                    wall.transform.eulerAngles = Vector3.up * 90f;
                }

                if(x == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        GameObject wall = Instantiate(wallPrefab, transform);
                        wall.transform.localPosition = position + Vector3.right * (cellSize / 2);
                        wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, wall.transform.localScale.z);
                        wall.transform.eulerAngles = Vector3.up * 90f;
                    }
                }

                if(y == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        GameObject wall = Instantiate(wallPrefab, transform);
                        wall.transform.localPosition = position - Vector3.forward * (cellSize / 2);
                        wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, wall.transform.localScale.z);
                    }
                }
            }
        }
    }

    public Vector3 ReturnRandomPosition()
    {
        System.Random rng = new System.Random();

        int X = rng.Next(0, width);
        int Y = rng.Next(0, height);

        Vector3 position = new Vector3(-width / 2 + X, 0, -height / 2 + Y) * cellSize * 0.05f + mazeOffset;
        return position;
    }

    public Vector3 ReturnPostionByIndex(int X, int Y)
    {
        Vector3 position = new Vector3(-width / 2 + X, 0, -height / 2 + Y) * cellSize + mazeOffset;
        return position;
    }

    public void DestroyMaze()
    {
        WallState[,] maze = MazeGenerator.Generator(width, height);

        for (int i = transform.childCount - 1; i > 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
