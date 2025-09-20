using System;

public  class Obstacle // Creates obstacle parent class with variables to inherit
{
    public int X { get; set; }
    public int Y { get; set; }

    
}

public class Camera : Obstacle // Child class of obstacle
{
    public char Direction { get; set; }
    public int Range { get; set; }

    public Camera(int x, int y, char direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }

    // Checks what cells are within camera fov
    public bool IsInFieldOfView(int targetX, int targetY, List<List<Node>> map)
    {
        int deltaX = targetX - X;
        int deltaY = targetY - Y;

        // Determines the direction of the camera's view
        if (Direction == 'n' && deltaY <= 0 && Math.Abs(deltaX) <= Math.Abs(deltaY))
        {
            for (int y = Y; y >= targetY; y--)
            {
                int x = X; 
                if (x >= 0 && x < map[0].Count && y >= 0 && y < map.Count)
                {
                    map[y][x].IsObstacle = true;
                }
            }
            return true;
        }

        else if (Direction == 'e' && deltaX >= 0 && Math.Abs(deltaY) <= Math.Abs(deltaX))
        {
            for (int x = X; x <= targetX; x++)
            {
                int y = Y; 
                if (x >= 0 && x < map[0].Count && y >= 0 && y < map.Count)
                {
                    map[y][x].IsObstacle = true;
                }
            }
            return true;
        }

        else if (Direction == 's' && deltaY >= 0 && Math.Abs(deltaX) <= Math.Abs(deltaY))
        {
            for (int y = Y; y <= targetY; y++)
            {
                int x = X; 
                if (x >= 0 && x < map[0].Count && y >= 0 && y < map.Count)
                {
                    map[y][x].IsObstacle = true;
                }
            }
            return true;
        }

        else if (Direction == 'w' && deltaX <= 0 && Math.Abs(deltaY) <= Math.Abs(deltaX))
        {
            for (int x = X; x >= targetX; x--)
            {
                int y = Y; 
                if (x >= 0 && x < map[0].Count && y >= 0 && y < map.Count)
                {
                    map[y][x].IsObstacle = true;
                }
            }
            return true;
        }

        return false;
    }
}

public class Fence : Obstacle
{
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }

    public Fence(int startX, int startY, int endX, int endY) 
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
    }
}

public class Guard : Obstacle
{
    public Guard(int x, int y) 
    {
    }
}

public class Sensor : Obstacle
{
    public double Range { get; set; }

    public Sensor(int x, int y, double range)
    {
        X = x; 
        Y = y; 
        Range = range;
    }
}

public class Trench : Obstacle
{
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }

    public Trench(int startX, int startY, int endX, int endY)
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
    }
}