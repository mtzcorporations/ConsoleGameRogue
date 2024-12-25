using System;
using System.Collections.Generic;

namespace CLI_ROGUERAMBOGAME
{
    public  class Player
    {
        public int [] position;
        public int maxHealth = 700;
        public int currentHealth;
        public int ammonition = 4;
        public char[] inventory;
        public int inventorySize=5;
        public Player(int [] position)
        {
            this.position = position;
            currentHealth = 500;
            inventory = new char[inventorySize];
            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = '-'; // '-' indicates an empty slot
            }
        }
        
        public void AddToInventory(char item)
        {
            int emptySlot = FindEmptySlot();
            if (emptySlot != -1) 
            {
                inventory[emptySlot] = item;
            }
        }
         public void UseItem(Map level)
        {
            while (true)
            {
                void ClearLastLine()
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1); // Move cursor to the last line
                    Console.Write(new string(' ', Console.WindowWidth)); // Overwrite it with spaces
                    Console.SetCursorPosition(0, Console.CursorTop - 1); // Move cursor back to the cleared line
                }
                Console.WriteLine("Press a key (1-5) to use an item from the inventory. Press 'P' to cancel.");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Read key input without displaying it
                if (keyInfo.Key == ConsoleKey.P)
                {
                    return; // Exit the method
                }
                // Check if the key is in the range 1 to 5
                if (keyInfo.KeyChar >= '1' && keyInfo.KeyChar <= '5')
                {
                    int index = keyInfo.KeyChar - '1'; // Convert '1'-'5' to 0-4
                    if (inventory[index] != '-')
                    {
                        if (inventory[index]=='A')
                        {
                            ammonition += 5;
                            inventory[index] = '-';
                            return;
                        }
                        if (inventory[index]=='a')
                        {
                            ammonition += 3;
                            inventory[index] = '-';
                            return;
                        }
                        if (inventory[index]=='H')
                        {
                            UseHealthItem('H', 200, index,level);
                            return;
                        }
                        if (inventory[index]=='h')
                        {
                            UseHealthItem('h', 100, index,level);
                            return;
                        }
                        return; // Exit the method after using item
                    }
                }

                ClearLastLine();
                Console.WriteLine("Invalid key. Please press a number between 1 and 5, or 'P' to cancel.");
            }
        }
        private void UseHealthItem(char itemType, int healthIncrease, int index, Map level)
        {
            if (currentHealth == maxHealth)
            {
                level.CustomMessage("Health is already full");
                return;
            }

            currentHealth += healthIncrease;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            inventory[index] = '-';
        }
        public void DropFromInventory(int [] neighboursY, int [] neighboursX, Map currentMap)
        {
            Random random = new Random();
            while (true)
            {
                void ClearLastLine()
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1); // Move cursor to the last line
                    Console.Write(new string(' ', Console.WindowWidth)); // Overwrite it with spaces
                    Console.SetCursorPosition(0, Console.CursorTop - 1); // Move cursor back to the cleared line
                }
                Console.WriteLine("Press a key (1-5) to drop an item from the inventory. Press 'P' to cancel.");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Read key input without displaying it
                if (keyInfo.Key == ConsoleKey.P)
                {
                    return; // Exit the method
                }
                // Check if the key is in the range 1 to 5
                if (keyInfo.KeyChar >= '1' && keyInfo.KeyChar <= '5')
                {
                    int index = keyInfo.KeyChar - '1'; // Convert '1'-'5' to 0-4
                    if (inventory[index] != '-') // Check if there's an item to drop
                    {
                        List<int[]> emptyIndices = new List<int[]>();
                        for (int i = 0; i < neighboursY.Length; i++)
                        {
                            int newY = neighboursY[i]+position[0];
                            int newX = position[1] +neighboursX[i];
                    
                            // Ensure the position is valid and the cell is empty
                            if (currentMap.GetDataForPosition(newY, newX) == GameDriver.emptyMapCellChar) 
                            {
                                emptyIndices.Add(new []{newY,newX});
                            }
                        }

                        if (emptyIndices.Count > 0)
                        {
                            int randomIndex = random.Next(emptyIndices.Count);
                            var data = emptyIndices[randomIndex];
                            int x = emptyIndices[randomIndex][1];
                            // Drop the item at the random position
                            currentMap.ChangeMapData(data[0], x,inventory[index]);
                            inventory[index] = '-'; 
                            Console.WriteLine($"Dropped item '{inventory[index]}' from slot {index + 1}.");
                        }

                      
                      
                    }
                    else
                    {
                        Console.WriteLine($"Slot {index + 1} is already empty.");
                    }
                    return; // Exit the method after dropping
                }

                ClearLastLine();
                Console.WriteLine("Invalid key. Please press a number between 1 and 5, or 'P' to cancel.");
            }
        }
        public bool IsInventoryFull()
        {
            return FindEmptySlot() == -1;
        }
        
        private int FindEmptySlot()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == '-') // Empty slot
                {
                    return i;
                }
            }
            return -1; // No empty slot found
        }
        
        public void DisplayInventoryAndInfo()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Health: {currentHealth} |");
            Console.ForegroundColor = ConsoleColor.Cyan; 
            Console.Write($" Ammo {ammonition} |");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Inventory: |");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            for (int i = 0; i < inventory.Length; i++)
            {
                Console.Write($"Slot {i+1}: {inventory[i]} | ");
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"\n Avaible turns: {GameDriver.currentturns}");
            Console.ResetColor();
            Console.Write("\n");
        }
        
    }

    public class Terrorist
    {
        public int[] position;
        public int health;
        private int damage;
        private  int vision=10;
        private  int patrolRange = 7;
        private const int mooves = 5;
        public int[] patrolPoint;
        public int Damage()=>damage;
        public int Mooves()=>mooves;
        public int Vision()=>vision;
        public int PatrolRange()=>patrolRange;
        public Terrorist(int[] position, int health, int damage)
        {
            this.damage = damage;
            this.health = health;
            this.position = position;
            patrolPoint = this.position;

        }

        public void UpdatePosition(int[] position)
        {
            this.position = position;
        }
         public Stack<int[]> FindPath(char[,] map, int[] targetYX)
{
    int height = map.GetLength(0);
    int width = map.GetLength(1);

    // Direction vectors for moving up, down, left, right, and diagonals
    int[] directionsX = { 0, 1, 0, -1, 1, 1, -1, -1 };
    int[] directionsY = { -1, 0, 1, 0, -1, 1, -1, 1 };

    // Priority queue using a List (sorted by f-cost)
    List<(int[] position, int gCost, int fCost)> openList = new List<(int[], int, int)>();
    HashSet<string> closedList = new HashSet<string>(); // To avoid revisiting nodes
    Dictionary<string, int[]> parents = new Dictionary<string, int[]>(); // Track parent nodes

    // Add starting node
    int[] start = position;
    int startG = 0;
    int startH = HeuristicManhattan(start, targetYX);
    openList.Add((start, startG, startG + startH));
    parents[Key(start)] = null; // Start node has no parent

    // Pathfinding loop
    while (openList.Count > 0)
    {
        // Sort open list by f-cost, then g-cost (to break ties)
        openList.Sort((a, b) => a.fCost != b.fCost ? a.fCost.CompareTo(b.fCost) : a.gCost.CompareTo(b.gCost));

        // Get node with the lowest f-cost
        var currentNode = openList[0];
        openList.RemoveAt(0);

        int[] currentPosition = currentNode.position;
        int currentG = currentNode.gCost;

        // Check if we've reached the target
        if (currentPosition[0] == targetYX[0] && currentPosition[1] == targetYX[1])
        {
            Stack<int[]> path = new Stack<int[]>();

            // Reconstruct the path
            while (currentPosition != null)
            {
                path.Push(currentPosition);
                currentPosition = parents[Key(currentPosition)];
            }

            return path; // Return the reconstructed path
        }

        // Add current position to closed list
        closedList.Add(Key(currentPosition));

        // Explore neighbors
        for (int i = 0; i < 8; i++) // 8 directions
        {
            int newX = currentPosition[1] + directionsX[i];
            int newY = currentPosition[0] + directionsY[i];

            // Check bounds
            if (newY < 0 || newX < 0 || newY >= height || newX >= width)
                continue;

            // Check if the tile is a wall
            if (map[newY, newX] == 'W'  )
                continue;

            // Calculate g-cost for the new position
            int jumpCost = 1;
            if (map[newY, newX] == 'A' || map[newY, newX] == 'a' || map[newY, newX] == 'H' || map[newY, newX] == 'h' ||  map[newY, newX] == 'T')
                jumpCost = 4; // Higher cost for jumping over assets

            int newG = currentG + jumpCost;

            // Skip already visited positions
            if (closedList.Contains(Key(new int[] { newY, newX })))
                continue;

            // Calculate h-cost and f-cost
            int[] newPosition = { newY, newX };
            int newH = HeuristicManhattan(newPosition, targetYX);
            int newF = newG + newH;

            // Add to open list if not already present, or update if a better path is found
            var existingNode = openList.FirstOrDefault(n => n.position[0] == newY && n.position[1] == newX);
            if (existingNode.position == null || newG < existingNode.gCost)
            {
                if (existingNode.position != null)
                    openList.Remove(existingNode);

                openList.Add((newPosition, newG, newF));
                parents[Key(newPosition)] = currentPosition; // Track the parent
            }
        }
    }

    // No path found
    return new Stack<int[]>();
}

private string Key(int[] position)
{
    return $"{position[0]},{position[1]}";
}

public int HeuristicManhattan(int[] current, int[] target)
{
    return Math.Abs(current[0] - target[0]) + Math.Abs(current[1] - target[1]);
}

    }
}