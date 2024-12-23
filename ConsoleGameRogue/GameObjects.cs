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
            Console.ResetColor();
            Console.Write("\n");
        }
        
    }

    public class Terrorist
    {
        public int[] position;
        public int health;
        public int damage;

        public Terrorist(int[] position, int health, int damage)
        {
            this.damage = damage;
            this.health = health;
            this.position = position;
   
        }
    }
}