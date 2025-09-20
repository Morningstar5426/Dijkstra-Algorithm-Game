using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;



namespace Agent_mission
{
    public class ObstaclePlacementException : Exception
    {
        public ObstaclePlacementException() { }
        public ObstaclePlacementException(string message) : base(message) { }
        public ObstaclePlacementException(string message, Exception inner) : base(message, inner) { }
    }

    internal class Program
    {
        



        static void Main(string[] args)
        {



            List<List<Node>> map = new List<List<Node>>();

            for (int y = -50; y <= 50; y++) // Creates from -50 to 50 on the y axis
            {
                List<Node> row = new List<Node>();
                for (int x = -50; x <= 50; x++) // Creates from -50 to 50 on the x axis
                {
                    Node node = new Node(x, y, map); 
                                                     
                    row.Add(node);
                }
                map.Add(row);
            }







            string options = "Select one of the following options\n" +
                            "g) Add 'Guard' obstacle\n" +
                            "f) Add 'Fence' obstacle\n" +
                            "s) Add 'Sensor' obstacle\n" +
                            "c) Add 'Camera' obstacle\n" +
                            "t) Add 'Trench' obstacle\n" +
                            "d) Show safe directions\n" +
                            "m) Display obstacle map\n" +
                            "p) Find safe path\n" +
                            "x) Exit\n";

            // Creates Lists of the obtacles
            List<Fence> fences = new List<Fence>();
            List<Sensor> sensors = new List<Sensor>();
            List<Obstacle> obstacles = new List<Obstacle>();
            List<Camera> cameras = new List<Camera>();

            string choice1;
            bool showOptions = true;



            while (true)
            {
                if (showOptions)
                {
                    Console.Write(options);
                }

                Console.WriteLine("Enter code:");
                choice1 = Console.ReadLine();


                if (choice1 == "g")
                {
                    Guard guard = new Guard(0, 0);
                    while (true)
                    {
                        Console.WriteLine("Enter the guard's location (X,Y):");
                        string coords = Console.ReadLine();

                        try // Parses coordinates if they are in correct format (-,-)
                        {
                            if (!IsValidCoordinate(coords))
                            {
                                // Throws exception if they aren't
                                throw new ArgumentException("Invalid coordinates. Make sure they are within the map's valid range.");
                            }

                            string[] parts = coords.Split(',');
                            int guardX = int.Parse(parts[0]);
                            int guardY = int.Parse(parts[1]);

                            // Checks if the coordinates are within the maps' range
                            if (guardX >= -50 && guardX <= 50 && guardY >= -50 && guardY <= 50)
                            {
                                guard.X = guardX;
                                guard.Y = guardY;

                                obstacles.Add(guard);
                                map[guardY + 50][guardX + 50].AddGuard(guard); // Adjusts for negative coordinates
                                showOptions = true;

                                break;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid coordinates. Make sure they are within the map's valid range.");
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            showOptions = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input.");
                            showOptions = false;
                        }
                    }
                }

                else if (choice1 == "f")
                {
                    Fence fence = null;
                    while (true)
                    {
                        Console.WriteLine("Enter the location where the fence starts (X,Y):");
                        string startCoords = Console.ReadLine();
                        Console.WriteLine("Enter the location where the fence ends (X,Y):");
                        string endCoords = Console.ReadLine();

                        try
                        {
                            if (IsValidCoordinate(startCoords) && IsValidCoordinate(endCoords))
                            {
                                string[] startParts = startCoords.Split(',');
                                string[] endParts = endCoords.Split(',');
                                int startX = int.Parse(startParts[0]);
                                int startY = int.Parse(startParts[1]);
                                int endX = int.Parse(endParts[0]);
                                int endY = int.Parse(endParts[1]);

                                // Allows for decreasing fences
                                if (startX > endX)
                                {
                                    int temp = startX;
                                    startX = endX;
                                    endX = temp;
                                }

                                if (startY > endY)
                                {
                                    int temp = startY;
                                    startY = endY;
                                    endY = temp;
                                }

                                int minX = Math.Max(startX, -50);
                                int maxX = Math.Min(endX, 50);
                                int minY = Math.Max(startY, -50);
                                int maxY = Math.Min(endY, 50);

                                if (minX <= maxX && minY <= maxY)
                                {
                                    fence = new Fence(startX, startY, endX, endY); //Initializes a new instance of fence
                                    obstacles.Add(fence);

                                    for (int y = minY; y <= maxY; y++)  // Adds the fence to the map if it is within the range
                                    {
                                        for (int x = minX; x <= maxX; x++)
                                        {
                                            map[y + 50][x + 50].AddFence(fence, map);
                                        }
                                    }

                                    showOptions = true;
                                    break;
                                }
                                else
                                {
                                    throw new ObstaclePlacementException("Invalid input. Coordinates are out of range.");
                                }
                            }
                            else
                            {
                                throw new ObstaclePlacementException("Invalid input.");
                            }
                        }
                        catch (ObstaclePlacementException ex)
                        {
                            Console.WriteLine("Fence placement error: " + ex.Message);
                            showOptions = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input.");
                            showOptions = false;
                        }
                    }
                }

                else if (choice1 == "p")
                {
                    Console.WriteLine("Enter your current location (X,Y):");
                    string currentLocation = Console.ReadLine();

                    Console.WriteLine("Enter the location of your objective (X,Y):");
                    string objectiveLocation = Console.ReadLine();

                    string[] currentParts = currentLocation.Split(','); // Splits the coordinates
                    string[] objectiveParts = objectiveLocation.Split(',');

                    int currentX, currentY, objectiveX, objectiveY;
                    if (currentParts.Length == 2 && int.TryParse(currentParts[0], out currentX) && int.TryParse(currentParts[1], out currentY) &&
                        objectiveParts.Length == 2 && int.TryParse(objectiveParts[0], out objectiveX) && int.TryParse(objectiveParts[1], out objectiveY))
                    {
                        // Checks if the coordinates are within the map
                        if (currentX >= -50 && currentX <= 50 && currentY >= -50 && currentY <= 50 &&
                            objectiveX >= -50 && objectiveX <= 50 && objectiveY >= -50 && objectiveY <= 50)
                        {
                            // Makes sure nodes are within map coordinates
                            if (currentX + 50 >= 0 && currentX + 50 < map[0].Count && currentY + 50 >= 0 && currentY + 50 < map.Count &&
                                objectiveX + 50 >= 0 && objectiveX + 50 < map[0].Count && objectiveY + 50 >= 0 && objectiveY + 50 < map.Count)
                            {
                                Node startNode = map[currentY + 50][currentX + 50];
                                Node endNode = map[objectiveY + 50][objectiveX + 50];
                                Dijkstra dijkstra = new Dijkstra();
                                List<Node> shortestPath = dijkstra.FindShortestPath(map, startNode, endNode, sensors); // Initialises the shortestpath method to find the shortest path

                                try
                                {
                                    if (shortestPath == null) // Checks if the path is blocked
                                    {
                                        throw new ObstaclePlacementException("Path to the objective is obstructed. Mission aborted.");
                                    }
                                    else
                                    {
                                        Node prevNode = null;
                                        Console.WriteLine("The following path will take you to the objective:");
                                        List<string> DirectionsList = new List<string>(); // Creates a list of directions

                                        foreach (var node in shortestPath)
                                        {
                                            if (prevNode != null)
                                            {
                                                string direction = GetDirection(prevNode, node);
                                                DirectionsList.Add(direction);
                                            }

                                            prevNode = node;
                                        }

                                        static string GetDirection(Node from, Node to) // Determines directions
                                        {
                                            if (to.Y < from.Y)
                                            {
                                                return "N";
                                            }
                                            else if (to.X > from.X)
                                            {
                                                return "E";
                                            }
                                            else if (to.Y > from.Y)
                                            {
                                                return "S";
                                            }
                                            else if (to.X < from.X)
                                            {
                                                return "W";
                                            }

                                            else
                                            {
                                                return "Agent, you are already at the objective.";
                                            }
                                        }

                                        string Directions = string.Join("", DirectionsList); // Creates a string of directions and joins them together
                                        Console.WriteLine(Directions);
                                    }
                                }
                                catch (ObstaclePlacementException ex)
                                {
                                    Console.WriteLine("Safe path finding error: " + ex.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Coordinates are out of map range.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter coordinates in the format (X,Y).");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter coordinates in the format (X,Y).");
                    }
                }

                else if (choice1 == "m")
                {
                    Console.WriteLine("Enter the location of the top-left cell of the map (X,Y):");
                    string startcell = Console.ReadLine();

                    Console.WriteLine("Enter the location of the bottom-right cell of the map (X,Y):");
                    string endcell = Console.ReadLine();

                    // Checks if coordinates are valid
                    if (IsValidCoordinate(startcell) && IsValidCoordinate(endcell))
                    {
                        string[] startParts = startcell.Split(',');
                        string[] endParts = endcell.Split(',');
                        int startX = int.Parse(startParts[0]);
                        int startY = int.Parse(startParts[1]);
                        int endX = int.Parse(endParts[0]);
                        int endY = int.Parse(endParts[1]);

                        // Visualises the map within the application
                        CreateBoard(startX, startY, endX, endY, map, obstacles);
                    }

                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }

                }

                else if (choice1 == "d")

                {
                    Console.WriteLine("Enter your current location (X,Y):");
                    string agentLocation = Console.ReadLine();

                    if (IsValidCoordinate(agentLocation))
                    {
                        string[] agent = agentLocation.Split(',');
                        int agentX = int.Parse(agent[0]);
                        int agentY = int.Parse(agent[1]);

                        List<string> safeDirections = new List<string>();

                        // Checks directions for obstacles and whether it is safe to move
                        if (CheckMove(map, agentX, agentY - 1, sensors, cameras))
                            safeDirections.Add("N");

                        if (CheckMove(map, agentX + 1, agentY, sensors, cameras))
                            safeDirections.Add("E");

                        if (CheckMove(map, agentX, agentY + 1, sensors, cameras))
                            safeDirections.Add("S");

                        if (CheckMove(map, agentX - 1, agentY, sensors, cameras))
                            safeDirections.Add("W");

                        if (safeDirections.Count == 0)
                        {
                            Console.WriteLine("You cannot safely move in any direction. Abort mission.");
                        }
                        else
                        {
                            string directions1 = string.Join("", safeDirections);
                            Console.WriteLine("You can safely take any of the following directions: " + directions1); // Prints the directions that are safe to move in
                        }
                    }


                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                }


                else if (choice1 == "s")
                {
                    Sensor sensor = null;

                    while (true)
                    {
                        Console.WriteLine("Enter the sensor's location (X,Y):");
                        string sensorLocation = Console.ReadLine();
                        try
                        {
                            if (IsValidCoordinate(sensorLocation))
                            {
                                string[] sensorcoords = sensorLocation.Split(',');

                                if (sensorcoords.Length == 2 && int.TryParse(sensorcoords[0], out int x) && int.TryParse(sensorcoords[1], out int y))
                                {
                                    // Checks if the coordinates are within the map
                                    if (x >= -50 && x <= 50 && y >= -50 && y <= 50)
                                    {
                                        Console.WriteLine("Enter the sensor's range (in klicks):");
                                        string sensorRange = Console.ReadLine();

                                        if (double.TryParse(sensorRange, out double SensorRange))
                                        {
                                            sensor = new Sensor(x, y, SensorRange);
                                            // Adds sensor to the list of sensors obstacles, and adds it to map
                                            sensors.Add(sensor);
                                            obstacles.Add(sensor);
                                            map[y + 50][x + 50].obstacles.Add(sensor);

                                            showOptions = true;

                                            // Exits the loop after adding a sensor to the map
                                            break;
                                        }
                                        else
                                        {
                                            throw new ObstaclePlacementException("Invalid input for sensor range.");
                                        }
                                    }
                                    else
                                    {
                                        throw new ObstaclePlacementException("Sensor coordinates are outside the bounds of the map.");
                                    }
                                }
                                else
                                {
                                    throw new ObstaclePlacementException("Invalid input for sensor coordinates.");
                                }
                            }
                            else
                            {
                                throw new ObstaclePlacementException("Invalid input format for sensor location.");
                            }
                        }
                        catch (ObstaclePlacementException ex)
                        {
                            Console.WriteLine("Sensor placement error: " + ex.Message);
                            showOptions = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input.");
                            showOptions = false;
                        }
                    }

                }

                else if (choice1 == "c")
                {
                    Camera camera = null;
                    while (true)
                    {
                        Console.WriteLine("Enter the camera's location (X,Y):");
                        string cameraLocation = Console.ReadLine();

                        try
                        {
                            if (IsValidCoordinate(cameraLocation))
                            {
                                string[] cameraCoords = cameraLocation.Split(',');

                                if (cameraCoords.Length == 2 && int.TryParse(cameraCoords[0], out int x) && int.TryParse(cameraCoords[1], out int y))
                                {
                                    if (x >= -50 && x <= 50 && y >= -50 && y <= 50)
                                    {
                                        Console.WriteLine("Enter the direction the camera is facing (n, s, e, or w):");
                                        string cameraDirection = Console.ReadLine();

                                        if (cameraDirection.Length == 1 && "nsew".Contains(cameraDirection.ToLower()))
                                        {
                                            // Create a new instance of the Camera class
                                            camera = new Camera(x, y, Char.ToLower(cameraDirection[0]));

                                            // Add the camera to the list of obstacles
                                            obstacles.Add(camera);

                                            // Create camera nodes on the map
                                            foreach (var mapRow in map)
                                            {
                                                foreach (var node in mapRow)
                                                {
                                                    if (camera.IsInFieldOfView(node.X, node.Y, map))
                                                    {
                                                        node.AddCamera(camera);
                                                    }
                                                }
                                            }

                                            showOptions = true;

                                            // Break the loop after adding a camera
                                            break;
                                        }
                                        else
                                        {
                                            throw new ObstaclePlacementException("Invalid direction for the camera.");
                                        }
                                    }
                                    else
                                    {
                                        throw new ObstaclePlacementException("Camera coordinates are outside the bounds of the map.");
                                    }
                                }
                                else
                                {
                                    throw new ObstaclePlacementException("Invalid input for camera coordinates. Use format X,Y.");
                                }
                            }
                            else
                            {
                                throw new ObstaclePlacementException("Invalid input format for camera location.");
                            }
                        }
                        catch (ObstaclePlacementException ex)
                        {
                            Console.WriteLine("Camera placement error: " + ex.Message);
                            showOptions = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input.");
                            showOptions = false;
                        }
                    }
                }

                else if (choice1 == "t")
                {
                    Trench trench = null;
                    while (true)
                    {
                        Console.WriteLine("Enter the top-left corner (X,Y) of the trench:");
                        string topLeftCoords = Console.ReadLine();
                        Console.WriteLine("Enter the bottom-right corner (X,Y) of the trench:");
                        string bottomRightCoords = Console.ReadLine();

                        try
                        {
                            if (topLeftCoords != null && bottomRightCoords != null && IsValidCoordinate(topLeftCoords) && IsValidCoordinate(bottomRightCoords)) // Checks to make sure the input is in coordinate format
                            {
                                string[] topLeftParts = topLeftCoords.Split(',');
                                string[] bottomRightParts = bottomRightCoords.Split(',');
                                int StartX = int.Parse(topLeftParts[0]);
                                int StartY = int.Parse(topLeftParts[1]);
                                int EndX = int.Parse(bottomRightParts[0]);
                                int EndY = int.Parse(bottomRightParts[1]);

                                if (StartX >= -50 && StartX <= 50 && StartY >= -50 && StartY <= 50 && EndX >= -50 && EndX <= 50 && EndY >= -50 && EndY <= 50)
                                {
                                    trench = new Trench(StartX, StartY, EndX, EndY);
                                    obstacles.Add(trench);

                                    for (int y = Math.Min(StartY, EndY); y <= Math.Max(StartY, EndY); y++) // Iterates through map and adds trench from start and end coordinates
                                    {
                                        for (int x = Math.Min(StartX, EndX); x <= Math.Max(StartX, EndX); x++)
                                        {
                                            map[y + 50][x + 50].AddTrench(trench, map);
                                        }
                                    }

                                    showOptions = true;
                                    break;
                                }
                                else
                                {
                                    throw new ObstaclePlacementException("Trench is out of board dimensions.");
                                }
                            }
                            else
                            {
                                throw new ObstaclePlacementException("Invalid input.");
                            }
                        }
                        catch (ObstaclePlacementException ex)
                        {
                            Console.WriteLine("Trench placement error: " + ex.Message);
                            showOptions = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input.");
                            showOptions = false;
                        }
                    }
                }


                else if (choice1 == "x")
                {
                    break;
                }


                else
                {
                    Console.WriteLine("Invalid option.");
                    showOptions = false;
                }
            }





            static void CreateBoard(int startX, int startY, int endX, int endY, List<List<Node>> map, List<Obstacle> obstacles)
            {
                // Calculate dimensions
                int width = Math.Abs(endX - startX) + 1;
                int height = Math.Abs(endY - startY) + 1;

                // Creates a character grid
                char[,] grid = new char[height, width];

                // Creates the grid with '.'
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        grid[y, x] = '.';
                    }
                }

                // Places  g, f, s, and c for the obstacles added
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int realX = x + startX;
                        int realY = y + startY;

                        foreach (var obstacle in obstacles) // Checks for obstacle types
                        {
                            if (obstacle is Guard guard && realX == guard.X && realY == guard.Y)
                            {
                                grid[y, x] = 'g';
                            }
                            else if (obstacle is Fence fence &&
                                realX >= fence.StartX && realX <= fence.EndX &&
                                realY >= Math.Min(fence.StartY, fence.EndY) && realY <= Math.Max(fence.StartY, fence.EndY))
                            {
                                grid[y, x] = 'f';
                            }
                            else if (obstacle is Sensor sensor) // Uses the pythagorean theorem to detect if a node is within the sensor range
                            {
                                int sensorX = sensor.X - startX;
                                int sensorY = sensor.Y - startY;
                                double distance = Math.Sqrt(Math.Pow(x - sensorX, 2) + Math.Pow(y - sensorY, 2));

                                if (distance <= sensor.Range)
                                {
                                    grid[y, x] = 's';
                                }
                            }
                            else if (obstacle is Camera camera)
                            {
                                int cameraX = camera.X - startX;
                                int cameraY = camera.Y - startY;

                                // Checks what cells are in view of camera and marks them as camera obstacles
                                if (camera.IsInFieldOfView(realX, realY, map))
                                {
                                    grid[y, x] = 'c';
                                }
                            }

                            else if (obstacle is Trench trench) // Checks if the obstacle is a trench and updates map accordingly
                            {
                                if (realX >= trench.StartX && realX <= trench.EndX &&
                                    realY >= trench.StartY && realY <= trench.EndY)
                                {
                                    grid[y, x] = 't';
                                }
                            }
                        }
                    }
                }

                // Displays the grid
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Console.Write(grid[y, x]);
                    }
                    Console.WriteLine();
                }
            }

            static bool IsValidCoordinate(string input)
            {
                string[] parts = input.Split(',');  //Checks if it is in coordinate format

                if (parts.Length == 2)
                {
                    if (double.TryParse(parts[0], out double x) && double.TryParse(parts[1], out double y))
                    {
                        return true;
                    }
                }

                return false; //States the input is invalid if not in -,- format
            }



            static bool CheckMove(List<List<Node>> map, int agentX, int agentY, List<Sensor> sensors, List<Camera> cameras)
            {

                int maxY = map.Count - 1;
                int maxX = map[0].Count - 1;

                if (agentX >= -50 && agentX <= 50 && agentY >= -50 && agentY <= 50) // Checks that agent is within the map
                {
                    Node agentNode = map[agentY + 50][agentX + 50]; // Adjusted for map size
                    if (agentNode.guards.Count > 0 || agentNode.IsObstacle)
                        return false;

                    foreach (var sensor in sensors) // Checks if there is an agent within the range of the sensor
                    {
                        double distance = Math.Sqrt(Math.Pow(agentX - sensor.X, 2) + Math.Pow(agentY - sensor.Y, 2));

                        if (distance <= sensor.Range)
                        {
                            return false;
                        }
                    }

                    foreach (var camera in cameras) // Checks if there is an agent within the range of the camera
                    {
                        if (camera.IsInFieldOfView(agentX, agentY, map))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    // The Agent is outside of map boundaries
                    return false;
                }
            }
        }
                 
                
        
    }
}









