using System;
using System.Collections.Generic;

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public double Distance { get; set; }
    public bool Visited { get; set; }
    public Node Parent { get; set; }

    private List<List<Node>> Map;


    public Node(int x, int y, List<List<Node>> map) 
    {
        X = x;
        Y = y;
        IsObstacle = false;
        Distance = double.MaxValue;
        Visited = false;
        Parent = null;
        Map = map;
    }

    // Creates the lists for obstacles 
    public List<Guard> guards { get; } = new List<Guard>();
    public List<Fence> fences { get; } = new List<Fence>();
    public List<Sensor> sensors { get; } = new List<Sensor>();
    public List<Camera> cameras { get; } = new List<Camera>();
    public List<Obstacle> obstacles { get; } = new List<Obstacle>();
    public List<Trench> trenches { get; } = new List<Trench>();
    public bool IsObstacle { get; set; } 

    public void AddGuard(Guard guard)
    {
        guards.Add(guard);
        IsObstacle = true;
    }

    public void AddSensor(Sensor sensor)
    {
        sensors.Add(sensor);
        IsObstacle = true;

    }

    public void AddFence(Fence fence, List<List<Node>> map)
    {
        fences.Add(fence);
        IsObstacle = true;

        // Adds fence from initial coordinates to the final ones inputted
        for (int y = Math.Min(fence.StartY, fence.EndY); y <= Math.Max(fence.StartY, fence.EndY); y++)
        {
            for (int x = Math.Min(fence.StartX, fence.EndX); x <= Math.Max(fence.StartX, fence.EndX); x++)
            {
                if (x >= 0 && x < map[0].Count && y >= 0 && y < map.Count)
                {
                    map[y][x].IsObstacle = true;
                }
            }
        }

    }
    public void AddCameraObstacle()
    {
        IsObstacle = true;
    }


    public void AddCamera(Camera camera) // Sets camera as an obstacle and adds it to the list of cameras
    {
        cameras.Add(camera);
        IsObstacle = true;
    }

    public void AddTrench(Trench trench, List<List<Node>> map) 
    {
        trenches.Add(trench);
        IsObstacle = true;
    }




    public void AddObstacle(Obstacle obstacle) // Adds the different obstacles to the list of obstacles
    {
        if (obstacle is Guard guard)
        {
            guards.Add(guard);
        }
        else if (obstacle is Fence fence)
        {
            fences.Add(fence);
        }
        else if (obstacle is Sensor sensor)
        {
            sensors.Add(sensor);
        }
        else if (obstacle is Trench trench) 
        {
            trenches.Add(trench);
        }
        else if (obstacle is Camera camera)
        {
            cameras.Add(camera); 
            IsObstacle = true; 

            foreach (var mapRow in Map)
            {
                foreach (var node in mapRow)
                {
                    if (camera.IsInFieldOfView(node.X, node.Y, Map))
                    {
                        cameras.Add(camera);
                        IsObstacle = true;
                    }
                }
            }
        }
        obstacles.Add(obstacle);
    }
    // Returns neighbouring nodes that are not obstacles
    public IEnumerable<Node> GetNeighbours(List<List<Node>> graph)  
    {   
        List<Node> neighbours = new List<Node>();
        int maxX = graph[0].Count - 1;
        int maxY = graph.Count - 1;

        if (X > 0)
        {
            Node leftNode = graph[Y][X - 1];
            if (!leftNode.IsObstacle)
                neighbours.Add(leftNode);
        }

        if (X < maxX)
        {
            Node rightNode = graph[Y][X + 1];
            if (!rightNode.IsObstacle)
                neighbours.Add(rightNode);
        }

        if (Y > 0)
        {
            Node topNode = graph[Y - 1][X];
            if (!topNode.IsObstacle)
                neighbours.Add(topNode);
        }

        if (Y < maxY)
        {
            Node bottomNode = graph[Y + 1][X];
            if (!bottomNode.IsObstacle)
                neighbours.Add(bottomNode);
        }

        return neighbours;
    }



}

