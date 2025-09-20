using System;
using System.Collections.Generic;

public class Dijkstra
{
    public List<Node> FindShortestPath(List<List<Node>> graph, Node start, Node end, List<Sensor> sensors)
    {
        List<Node> path = new List<Node>();


        // Creates new path for the option 'p'
        foreach (var row in graph)
        {
            foreach (var n in row)
            {
                n.Distance = double.MaxValue;
                n.Visited = false;
                n.Parent = null;
            }
        }

        start.Distance = 0;

        while (true) // Finds the shortest distance between nodes whilst checkingg for obstacles
        {
            Node current = GetClosestUnvisitedNode(graph);
            if (current == null)
                break;

            current.Visited = true;

            foreach (var neighbour in GetNeighbours(graph, current))
            {
                if (neighbour.IsObstacle || IsWithinSensorRange(neighbour, sensors))
                    continue;

                double tentativeDistance = current.Distance + CalculateDistance(current, neighbour);

                if (tentativeDistance < neighbour.Distance)
                {
                    neighbour.Distance = tentativeDistance;
                    neighbour.Parent = current;
                }
            }
        }

        Node node = end;
        while (node != null)
        {
            path.Insert(0, node);
            node = node.Parent;
        }
        return path;
    }

    private bool IsWithinSensorRange(Node node, List<Sensor> sensors) // Boolean value to return if a node is withing the sensor range
    {
        foreach (var sensor in sensors)
        {
            double distance = Math.Sqrt(Math.Pow(node.X - sensor.X, 2) + Math.Pow(node.Y - sensor.Y, 2));
            if (distance <= sensor.Range)
            {
                return true;
            }
        }
        return false;
    }

    private Node GetClosestUnvisitedNode(List<List<Node>> graph) // Finds the closest node that hasn't been visited yet
    {
        Node closest = null;
        double minDistance = double.MaxValue;

        foreach (var row in graph)
        {
            foreach (var node in row)
            {
                if (!node.Visited && node.Distance < minDistance)
                {
                    closest = node;
                    minDistance = node.Distance;
                }
            }
        }

        return closest;
    }

    // Returns all neighbouring nodes (does not check for obstacles)
    private IEnumerable<Node> GetNeighbours(List<List<Node>> graph, Node node) 
    {
        List<Node> neighbours = new List<Node>();
        int maxX = graph[0].Count - 1;
        int maxY = graph.Count - 1;

        int x = node.X + 50;
        int y = node.Y + 50;

        // Checks the neighbour on the left
        if (x > 0)
        {
            Node leftNode = graph[y][x - 1];
            if (!leftNode.IsObstacle)
                neighbours.Add(leftNode);
        }

        // Checks neighbour on the right
        if (x < maxX)
        {
            Node rightNode = graph[y][x + 1];
            if (!rightNode.IsObstacle)
                neighbours.Add(rightNode);
        }

        // Checks the neighbour above
        if (y > 0)
        {
            Node topNode = graph[y - 1][x];
            if (!topNode.IsObstacle)
                neighbours.Add(topNode);
        }

        // Checks the neighbour below
        if (y < maxY)
        {
            Node bottomNode = graph[y + 1][x];
            if (!bottomNode.IsObstacle)
                neighbours.Add(bottomNode);
        }

        return neighbours;
    }

    private double CalculateDistance(Node node1, Node node2) // Calculates the distance between nodes
    {
        double dx = node2.X - node1.X;
        double dy = node2.Y - node1.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    
}