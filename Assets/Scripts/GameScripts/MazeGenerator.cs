using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum WallState
{
    LEFT = 1,   //0001
    RIGHT = 2,  //0010
    UP = 4,     //0100
    DOWN = 8,    //1000

    VISITED = 128 //1000 0000
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

public  static class MazeGenerator
{
    private static WallState GetOppositeWall(WallState wall)
    {
        switch(wall)
        {
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }

    private static WallState[,] ApplyRecursiveBackTrack(WallState[,] maze, int width, int height)
    {
        System.Random rng = new System.Random();
        Stack<Position> positionStack = new Stack<Position>();
        Position position = new Position
        {
            X = rng.Next(0, width),
            Y = rng.Next(0, height),
        };
        maze[position.X, position.Y] |= WallState.VISITED;
        positionStack.Push(position);

        while(positionStack.Count > 0)
        {
            Position currentPosition = positionStack.Pop();
            List<Neighbour> neightbourList = GetVisitedNeighbour(currentPosition, maze, width, height);

            if(neightbourList.Count > 0)
            {
                positionStack.Push(currentPosition);

                int randomIndex = rng.Next(0, neightbourList.Count);
                Neighbour randomNeighbour = neightbourList[randomIndex];

                Position nPosition = randomNeighbour.Position;
                maze[currentPosition.X, currentPosition.Y] &= ~randomNeighbour.SharedWall;
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);

                maze[nPosition.X, nPosition.Y] |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }

    private static List<Neighbour> GetVisitedNeighbour(Position position, WallState[,] maze, int width, int height)
    {
        List<Neighbour> list = new List<Neighbour>();

        //Check left cell
        if(position.X > 0)
        {
            if(!maze[position.X - 1, position.Y].HasFlag(WallState.VISITED))
            {
                Neighbour neighbour = new Neighbour();
                neighbour.Position = new Position
                {
                    X = position.X - 1,
                    Y = position.Y
                };
                neighbour.SharedWall = WallState.LEFT;

                list.Add(neighbour);
            }
        }

        //Check right cell
        if (position.X < width - 1)
        {
            if (!maze[position.X + 1, position.Y].HasFlag(WallState.VISITED))
            {
                Neighbour neighbour = new Neighbour();
                neighbour.Position = new Position
                {
                    X = position.X + 1,
                    Y = position.Y
                };
                neighbour.SharedWall = WallState.RIGHT;

                list.Add(neighbour);
            }
        }

        //Check down cell
        if (position.Y > 0)
        {
            if (!maze[position.X, position.Y - 1].HasFlag(WallState.VISITED))
            {
                Neighbour neighbour = new Neighbour();
                neighbour.Position = new Position
                {
                    X = position.X,
                    Y = position.Y - 1
                };
                neighbour.SharedWall = WallState.DOWN;

                list.Add(neighbour);
            }
        }

        //Check up cell
        if (position.Y < height - 1)
        {
            if (!maze[position.X, position.Y + 1].HasFlag(WallState.VISITED))
            {
                Neighbour neighbour = new Neighbour();
                neighbour.Position = new Position
                {
                    X = position.X,
                    Y = position.Y + 1
                };
                neighbour.SharedWall = WallState.UP;

                list.Add(neighbour);
            }
        }

        return list;
    }

    public static WallState[,] Generator(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.UP | WallState.DOWN | WallState.LEFT | WallState.RIGHT;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = initial;
            }
        }

        return ApplyRecursiveBackTrack(maze, width, height);
    }
}
