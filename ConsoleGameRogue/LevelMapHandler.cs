using System;
using System.Collections.Generic;
using System.IO;

namespace CLI_ROGUERAMBOGAME
{
    public class Map
    {
        public char[,] MapData { get; set; }
        public int Width;
        public int Height;
        private string lastMessage = "";
        
        public Map(char[,] mapData)
        {
            MapData = mapData;
            Width = mapData.GetLength(1);
            Height = mapData.GetLength(0);
        }
        public char GetDataForPosition(int y, int x) => MapData[y, x];
 
        public void ChangeMapData(int y, int x, char changeTo)
        {
            MapData[y, x] = changeTo;
        }
        public void ResetInfo()
        {
            lastMessage = "";
        }
        public void DrawMap(int cursorX, int cursorY,bool drawCursor=true)
        {
            Console.Clear();
            for (int y = 0; y < Height; y++) // Iterate over rows (height)
            {
                for (int x = 0; x < Width; x++) // Iterate over columns (width)
                {
                    if (x == cursorX && y == cursorY &&drawCursor)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray; // Highlight cursor position
                        Console.Write(MapData[y, x]);
                        Console.ResetColor();
                    }
                    else
                    {
                        if (MapData[y, x] == 'P')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else if (MapData[y, x] == 'T')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        } else if(MapData[y,x]=='h' || MapData[y,x]=='H') Console.ForegroundColor = ConsoleColor.Cyan;
                        else if(MapData[y,x]=='h' || MapData[y,x]=='H') Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        
                        Console.Write(MapData[y, x]);
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
            GameDriver.player.DisplayInventoryAndInfo(); //print player info
            Console.WriteLine(lastMessage);
            ResetInfo();
        }
        public void DisplayCellInfo(int x, int y)
        {
            string info = getInfo(MapData[y, x]);
            lastMessage = $"INFO: Cursor Position: ({x}, {y}), is: {info}";

        }

       
        public void CustomMessage(String message)
        {
            lastMessage = message;
        }
        string getInfo(char currentField)
        {
            switch (currentField)
            {
                case 'W':
                    return "Wall";
                case 'P':
                    return "Player";
                case 'T':
                    return "Terrorist";
                default:
                    return "Empty cell";
            }
        }
    }
    public static class LevelMapHandler
    {
        private static Map map { get; set; }

        public static void ResetLevels() =>map=null;
        public static void AddLevel(Map level) => map = level;
        public static Map GetLevel() => map;
        public static Map ReadLevelFromTxt(string filePath,bool readCustom)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int height = lines.Length;
                int width = lines[0].Length;
                
                for (int i = 1; i < height; i++)
                {
                    if (readCustom && lines[i].Equals(GameDriver.dataString))
                    {
                        height = i;
                        continue;
                    }
                    if (lines[i].Length != width)
                    {
                        throw new Exception("ERROR All lines in the level file must have the same length!");
                    }
                }

                char[,] mapData = new char[height,width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (lines[y][x] == '.')
                        {
                            mapData[y, x] =  ' ';
                        }
                        else
                        {
                            mapData[y, x] = lines[y][x];
                            if (mapData[y, x] == 'P')
                            {
                                GameDriver.player = new Player(new[]{y,x});
                            }
                            if (mapData[y, x] == 'T'&& !readCustom)
                            {
                                GameDriver.terrorists.Add(new Terrorist(new[]{y,x},300,GameDriver.terroristDamage));
                            }
                        }
                    }
                }

                if (readCustom)
                {
                    ParseCustomData(lines,height+1,lines.Length);
                }
                return new Map(mapData);
            }
            catch (IOException e)
            {
                Console.WriteLine("Error reading file: " + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("dis"+e.Message);
                return null;
            }
        }
        private static void ParseCustomData(string[] lines,int indexFrom, int indexTo)
        {
            for (int i = indexFrom; i < indexTo; i++)
            {
                var line = lines[i];
                // Check the prefix to determine the type of data
                if (line.StartsWith("CT|")) // Current turns
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int turns))
                    {
                        GameDriver.currentturns = turns;
                    }
                }
                else if (line.StartsWith("P|")) // Player data
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 3)
                    {
                        switch (parts[1])
                        {
                            case "H": // Player health/ammunition
                                if (int.TryParse(parts[2], out int health))
                                {
                                    GameDriver.player.currentHealth = health; // Assuming player.ammonition is used here
                                }
                                break;

                            case "A": // Player ammunition
                                if (int.TryParse(parts[2], out int ammunition))
                                {
                                    GameDriver.player.ammonition = ammunition;
                                }
                                break;

                            case "I": // Player inventory
                                for (int j = 0; j < parts[2].Length && j <  GameDriver.player.inventorySize; j++)
                                {
                                    GameDriver.player.inventory[j] = parts[2][j];
                                }
                                break;
                        }
                    }
                }
                else if (line.StartsWith("T|")) // Terrorist data
                {
                    var parts = line.Split('|');
                    if (parts[1] == "H")
                    {
                        //terrorists.Add(new Terrorist { health = health });
                        GameDriver.terrorists.Add(new Terrorist(new[]{int.Parse(parts[2]),int.Parse(parts[3])},int.Parse(parts[4]),GameDriver.terroristDamage));
                    }
                }
            }
        } 
    }
    
}