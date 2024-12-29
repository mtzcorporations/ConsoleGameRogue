using System;

namespace CLI_ROGUERAMBOGAME
{
    public static class MenuScreen
    {
        public static void Menu()
        {
            string[] options = { "Play game","Load saved Game", "Controls and Rules","Generate Level", "Exit" };
            int selectedIndex = 0;
            bool stayInMenu = true;
            while (stayInMenu)
            {
                Console.Clear();
                Console.WriteLine("__________Welcome to Rogue__________");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("> " + options[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("  " + options[i]);
                    }
                }
                GraphicsReader.PrintGraphics("AK_47");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex + 1) % options.Length;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    stayInMenu = false;
                    switch (selectedIndex)
                    {
                        case 0:
                            PlayGame();
                            break;
                        case 1:
                            LoadLevel();
                            break;
                        case 2:
                            ControlsAndRules();
                            break;
                        case 3:
                            GenerateMap();
                            break;
                        case 4:
                            Environment.Exit(0);
                            break;
                    }
                }
            }
            
        }

        public static void GameOver(String customPath,bool customLevel,bool isWin)
        {
            string[] options = { "Restart", "Main Menu", "Exit" };
            int selectedIndex = 0;
            bool stayInMenu = true;
            while (stayInMenu)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                if(isWin)   Console.WriteLine("Congratulations, you won the game!");
                else Console.WriteLine("GAME OVER! YOU WERE SHOT!");
                Console.ResetColor();
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("> " + options[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("  " + options[i]);
                    }
                }

                GraphicsReader.PrintGraphics("BOMB");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex + 1) % options.Length;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    stayInMenu = false;
                    switch (selectedIndex)
                    {
                        case 0:
                            GameDriver.PlayGame(customPath,customLevel);
                            break;
                        case 1:
                            Menu();
                            break;
                        case 2:
                            Environment.Exit(0);
                            break;

                    }
                }
            }
        }

        static void LoadLevel()
        {
            //String path = GameDriver.pathToSavedData+"\\tale.txt";
            string path = ListDir(GameDriver.pathToSavedData);
            GameDriver.PlayGame(path,true);
       
        }

        public static string ListDir(string directoryPath)
        {
            Console.Clear();
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Directory not found.");
                return "";
            }

            string[] files = Directory.GetFiles(directoryPath);
            if (files.Length == 0)
            {
                Console.WriteLine("No files found in the directory.");
                return "";
            }

            int selectedIndex = 0;
            bool stayInMenu = true;

            while (stayInMenu)
            {
                Console.Clear();
                Console.WriteLine($"__________Files in {directoryPath}__________");
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"CHOOSE LEVEL");
                Console.ResetColor();
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileName(files[i]);
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("> " + fileName);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("  " + fileName);
                    }
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                GraphicsReader.PrintGraphics("KNIFE");
                Console.ResetColor();
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex - 1 + files.Length) % files.Length;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex + 1) % files.Length;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    stayInMenu = false;
                    Console.Clear();
                    Console.WriteLine($"You selected: {Path.GetFileName(files[selectedIndex])}");
                    return files[selectedIndex];
                }
            }

            return "";
        }
        static void GenerateMap()
        {
            Console.Clear();
            // Implementation for saved  data
            Console.WriteLine("Loading saved Data...");
            MapGenerator generator = new MapGenerator();
            Random rand = new Random();
            int width = rand.Next(70, 140);
            int height = rand.Next(13, 25);
            int terroristNum = rand.Next(3, 8);
            int bigHealth = rand.Next(2, 5);
            int smallHealth = rand.Next(3, 8);
            int bigAmmo = rand.Next(2, 5);
            int smallAmmo = rand.Next(4, 8);
            char[,] mapC = generator.GenerateMap(width, height, terroristNum, bigHealth, smallHealth, bigAmmo, smallAmmo);
            Map map = new Map(mapC);
            GameMenu(generator,map);
        }
        static void GameMenu(MapGenerator generator,Map map)
        {
            while (true)
            {
                Console.Clear();
                map.MapAreaPrint(0,0,false);
                Console.WriteLine("=== Menu Options ===");
                Console.WriteLine("[R] Generate a new map");
                Console.WriteLine("[Enter] Save the current map");
                Console.WriteLine("[F1] Main menu");
                Console.WriteLine("====================");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.R:
                        GenerateMap();
                        break;
                    case ConsoleKey.Enter:
                        string uniqueId = Guid.NewGuid().ToString();
                        string filePath =GameDriver.pathToLevels+ $"\\customLevel_{uniqueId}.txt";
                        SaveMap(map.MapData, filePath);
                        break;
                    case ConsoleKey.F1:
                        Menu();
                        break;
                    default:
                        Console.WriteLine("Invalid key. Please select a valid option.");
                        break;
                }
            }
        }
        
        static void SaveMap(char[,] map, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    int rows = map.GetLength(0);
                    int cols = map.GetLength(1);

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            writer.Write(map[i, j]);
                        }
                        writer.WriteLine();
                    }
                }

                Console.WriteLine($"Map successfully saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving map: {ex.Message}");
            }
        }

        static void PlayGame()
        {
            string path = ListDir(GameDriver.pathToLevels);
            // Implementation for loading a level
            GameDriver.PlayGame(path,false);
        }
        static void ControlsAndRules()
        {
            // Implementation for generating a custom level
            Console.Clear();
            Console.WriteLine("CONTROLS AND INSTRUCTIONS FOR GAME");
            PrintBadGuy();
            Console.ForegroundColor = ConsoleColor.Green;
            
            Console.Write($"You have {GameDriver.AVAIBLETURNS} turns per cycle. ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"Movement costs  {GameDriver.ACTIVITYCOST} for 1 step in each direction. \n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"Relod costs {GameDriver.ACTIVITYCOST} turn, shooting costs 0. ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Picking up item costs 0\n");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            PrintBadGuy();
            Console.WriteLine("To win the game defeat all Terrorists. You can only shoot in 8 directions,\nup, down, left, right and " +
                              "diagonal so be careful how you set cursor!\n" +
                              "Items on map will help you win the game! Be aware Terrorists can shoot you.");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            PrintBadGuy();
            Console.WriteLine("Controls:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Space".PadRight(15) + "--> Shooting");
            Console.WriteLine("R".PadRight(15) + "--> Reload");
            Console.WriteLine("D".PadRight(15) + "--> Drop item");
            Console.WriteLine("U".PadRight(15) + "--> Use item");
            Console.WriteLine("P".PadRight(15) + "--> Pickup item");
            Console.WriteLine("M".PadRight(15) + "--> Restart game");
            Console.WriteLine("Arrow Keys".PadRight(15) + "--> Movement");
            Console.WriteLine("W, A, S, D".PadRight(15) + "--> Cursor movement");
            Console.WriteLine("I".PadRight(15) + "--> Info");
            Console.WriteLine("F3".PadRight(15) + "--> Save Game");
            Console.WriteLine("F1".PadRight(15) + "--> Return to main menu");
           
            Console.ResetColor();
            PrintBadGuy();
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.F1)
                {
                    break;
                }
            }

            Menu();
        }

        private static void PrintBadGuy()
        {
            GraphicsReader.PrintGraphics("BADGUY");
            Console.Write("                 ");
            GraphicsReader.PrintGraphics("BADGUY");
            Console.Write("                 ");
            GraphicsReader.PrintGraphics("BADGUY");
            Console.Write("\n");
        }
    }
}