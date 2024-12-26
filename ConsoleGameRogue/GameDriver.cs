using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CLI_ROGUERAMBOGAME
{
    public static class GameDriver
    {
        public static String pathToLevels;
        public static String pathToSavedData;
        private const String levelsDir = "\\Levels";
        private const String savedGames = "\\SavedGame";
        
        public  const int AVAIBLETURNS = 5;
        public const int ACTIVITYCOST = 1;
    
        public static char emptyMapCellChar = ' ';
        private static int[] deltaY = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private static int[] deltaX = { -1, 0, 1, -1, 1, -1, 0, 1 };
        
        public static  int currentturns = 0;
        public static Player player;
        public static List<Terrorist> terrorists = new List<Terrorist>();
        public const int terroristDamage = 50;
        public static String dataString = "===GAME_DATA===";
        //save function - char mapa; -currentTurns, player, terrorists
        public enum Controls
        {
            CursorUp,       // Corresponds to W key
            CursorDown,     // Corresponds to S key
            CursorLeft,     // Corresponds to A key
            CursorRight,    // Corresponds to D key
            Info,           // Corresponds to I key
            Reset,          // Corresponds to M key
            MainMenu,       // Corresponds to f1 key
            Reload,          // Corresponds to R key
            Shoot,          // Corresponds to Space key
            DropItem,        // Corresponds to X key
            PickUpItem,      // Corresponds to P key
            UseItem,      // Corresponds to U key
            MoveUp,          // Corresponds to arrowUp key
            MoveDown,       // Corresponds to arrowDown key
            SaveGame,       // F3
            MoveRight,      // Corresponds to arrowRight key
            MoveLeft        // Corresponds to arrowLeft key
        }
        private static Dictionary<ConsoleKey, Controls> keyToControlMap = new Dictionary<ConsoleKey, Controls>
        {
            { ConsoleKey.W, Controls.CursorUp },
            { ConsoleKey.S, Controls.CursorDown },
            { ConsoleKey.A, Controls.CursorLeft },
            { ConsoleKey.D, Controls.CursorRight },
            { ConsoleKey.UpArrow, Controls.MoveUp },
            { ConsoleKey.DownArrow, Controls.MoveDown},
            { ConsoleKey.LeftArrow, Controls.MoveLeft},
            { ConsoleKey.RightArrow, Controls.MoveRight},
            { ConsoleKey.Spacebar, Controls.Shoot},
            { ConsoleKey.I, Controls.Info },
            { ConsoleKey.U, Controls.UseItem  },
            { ConsoleKey.M, Controls.Reset },
            { ConsoleKey.R, Controls.Reload },
            { ConsoleKey.P, Controls.PickUpItem },
            { ConsoleKey.X, Controls.DropItem },
            { ConsoleKey.F1, Controls.MainMenu },
            { ConsoleKey.F3, Controls.SaveGame }
        };

      
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the path to the file as an argument.");
                return;
            }
            string rootPath = args[0];
            ReadGraphics(rootPath);
            pathToLevels=rootPath+ levelsDir;
            pathToSavedData = rootPath + savedGames;
                        
            MenuScreen.Menu();
        }

        static void ReadGraphics(String pathToRoot)
        {
            string graphicsFolderPath = pathToRoot+ "\\Graphics";
            try
            {
                GraphicsReader.ReadGraphics(graphicsFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public static void PlayGame(String customPath,bool customLevel) {    
            int cursorX = 1, cursorY = 1;
            LoadLevel(customPath,customLevel);
            Map levelData = LevelMapHandler.GetLevel();
            int mapHeight = levelData.Height;
            int mapWidth = levelData.Width;
            
            ConsoleKey key=ConsoleKey.F1;
          
            bool stayInloop = true;
            while(stayInloop)
            {
                Controls control;
                Console.Clear();
                levelData.DrawMap(cursorX,cursorY);
                if (currentturns <= 0)
                {
                    TerroristTurn(levelData,cursorY,cursorX);
                    currentturns = AVAIBLETURNS;
                    continue;
                }
                key = Console.ReadKey(true).Key;
               
                // Check dictionary
                if (keyToControlMap.ContainsKey(key))
                {
                    control = keyToControlMap[key];  
                }
                else
                {
                    levelData.CustomMessage("Pressed key does nothing!");
                    continue;
                }
                
                switch (control)
                {
                    case Controls.MoveUp:
                        MoveControler(mapHeight,mapWidth,key,levelData);
                        break;
                    case Controls.MoveDown:
                        MoveControler(mapHeight,mapWidth,key,levelData);
                        break;
                    case Controls.MoveLeft:
                        MoveControler(mapHeight,mapWidth,key,levelData);
                        break;
                    case Controls.MoveRight:
                        MoveControler(mapHeight,mapWidth,key,levelData);
                        break;
                    case Controls.CursorUp:
                        if (cursorY > 0) cursorY--;
                        break;
                    case Controls.CursorDown:
                        if (cursorY < mapHeight - 1) cursorY++;
                        break;
                    case Controls.CursorLeft:
                        if (cursorX > 0) cursorX--;
                        break;
                    case Controls.CursorRight:
                        if (cursorX < mapWidth - 1) cursorX++;
                        break;
                    case Controls.Info:
                        levelData.DisplayCellInfo(cursorX, cursorY);
                        break;
                    case Controls.Shoot:
                        Shooting(new []{cursorY,cursorX},player.position,5,levelData);
                        break;
                    case Controls.Reload:
                        if (player.ammonition < 4)
                        {
                            currentturns -= 1;
                            player.ammonition = 4;
                        }
                        break;
                    case Controls.PickUpItem:
                        PickUpItem(levelData);
                        break;
                    case Controls.DropItem:
                        player.DropFromInventory(deltaY,deltaX,levelData);
                        break;
                    case Controls.UseItem:
                        player.UseItem(levelData);
                        break;
                    case Controls.SaveGame:
                        string[] parts = customPath.Split('\\');
                        string name = parts[^1].Split('.')[0];
                        SaveGame(levelData,name);
                        break;
                    case Controls.Reset:
                        stayInloop = false;
                        break;
                    case Controls.MainMenu:
                        stayInloop = false;
                        break;
                }

            }
            if (keyToControlMap[key] == Controls.Reset)
            {
                Reset(customPath,customLevel);
                PlayGame(customPath,customLevel);
            }
            else
            {
                Reset(customPath,customLevel);
                MenuScreen.Menu();
            }
            Thread.Sleep(100);
        }

        private static  void TerroristTurn(Map level,int cursorY,int cursorX)
        {
            for (int t = 0; t < terrorists.Count; t++)
            {
                var path = terrorists[t].FindPath(level.MapData, player.position );

                int count = path.Count;
                int minCount = Math.Min(count, terrorists[t].Mooves());
                if (count > terrorists[t].Vision()) //patrol mode!
                {
                   path=PatrolPath(level, cursorY, cursorX, t);
                   if(path==null || path.Count<2) continue;
                   count = path.Count;
                   minCount = Math.Min(count, terrorists[t].Mooves());
                }
                if(minCount<5)
                {
                    //shooting
                    TerroristShoot(level,path,cursorY,cursorX,t);
                    continue;        
                }
                for (int i = 0; i < minCount; i++) //in vision--chase
                {
                   
                    MooveOnPath(level,path,cursorY,cursorX,t,true);
                    terrorists[t].patrolPoint = terrorists[t].position; //set new patrol point
                }
                if (player.currentHealth <= 0)
                {
                    return ;
                }
            }

        }
        private static void TerroristShoot(Map level,Stack<int[]> path,int cursorY,int cursorX,int t)
        {
            List<int[]> pathTrace = new List<int[]>(); 
            pathTrace.AddRange(path);
            var range = path.Count;
            for(int i=0;i<range;i++) {
                var newPosition = path.Pop();
                if (path.Count ==0)
                {
                    level.DrawMap(newPosition[1], newPosition[0]);
                    Thread.Sleep(150);
                    level.DrawMap(0, 0,false);
                    Thread.Sleep(100);
                    level.DrawMap(newPosition[1], newPosition[0]);
                    Thread.Sleep(100);
            
                }
                if (level.GetDataForPosition(newPosition[0], newPosition[1]) != emptyMapCellChar)
                {
                
                    continue;
                }
                level.ChangeMapData(newPosition[0],newPosition[1],'*');
                level.DrawMap(cursorX, cursorY);
                Thread.Sleep(100);
            }

            range = pathTrace.Count();
            for (int j=0;j<range-1;j++)
            {
                var trace = pathTrace[j];
                if(trace[0]==terrorists[t].position[0]&&trace[1]==terrorists[t].position[1]) continue;
                level.ChangeMapData(trace[0],trace[1],emptyMapCellChar);
            }
            level.DrawMap(cursorX, cursorY);
            Thread.Sleep(100);
        }
        private static void MooveOnPath(Map level,Stack<int[]> path,int cursorY,int cursorX,int t,bool chase=false)
        {
            if(path.Count()<5&&chase)
            {
                //shooting
                TerroristShoot(level,path,cursorY,cursorX,t);
                return;        
            }
            var newPosition = path.Pop();
            if (level.GetDataForPosition(newPosition[0], newPosition[1]) != ' ') return ;
            level.ChangeMapData(terrorists[t].position[0], terrorists[t].position[1], emptyMapCellChar);
            terrorists[t].UpdatePosition(newPosition);
            level.ChangeMapData(newPosition[0], newPosition[1], 'T');
            level.DrawMap(cursorX, cursorY);
            Thread.Sleep(100);

           
        }
        private static Stack<int[]> PatrolPath(Map level,int cursorY,int cursorX, int t)
        {
            int[] patroPosition = GeneratePatrolPoint(terrorists[t].patrolPoint,terrorists[t].PatrolRange(),level.MapData);
            if (patroPosition == null) return null;
            var patrolPath = terrorists[t].FindPath(level.MapData, patroPosition );
            int steps = Math.Min(terrorists[t].Mooves(), patrolPath.Count);
            for (int i=0;i<steps;i++)
            {
                var path = terrorists[t].FindPath(level.MapData, player.position );
                if (path.Count() <= terrorists[t].Vision())
                {
                    return path;
                }
                MooveOnPath(level,patrolPath,cursorY,cursorX,t);
                
            }

            return null;
        }
        public static bool IsPositionFree(int y, int x, char[,] mapGrid)
        {
            if (y < 0 || x < 0 || y >= mapGrid.GetLength(0) || x >= mapGrid.GetLength(1))
                return false;
            return mapGrid[y, x] == ' ';
        }
        
        public static int[] GeneratePatrolPoint(int[] patrolPosition, int patrolRange, char[,] mapGrid)
        {
            Random rand = new Random();
            int patrolPointY = patrolPosition[0];
            int patrolPointX = patrolPosition[1];

            // Try generating a patrol point until we find a free position within range
            for (int i = 0; i < 150; i++) // Limit retries to avoid infinite loops
            {
                int dy = rand.Next(-patrolRange, patrolRange + 1);
                int dx = rand.Next(-patrolRange, patrolRange + 1);
                
                if (Math.Abs(dy) + Math.Abs(dx) <= patrolRange)
                {
                    int newY = patrolPointY + dy;
                    int newX = patrolPointX + dx;
                    if (IsPositionFree(newY, newX, mapGrid))
                    {
                        return new int[] { newY, newX };
                    }
                }
            }

            return null; // Return null if no valid patrol point was found within 100 retries
        }
        private static int ChooseLevel()
        {
            //Console.Clear();
            int numberOfLevels = 1;
            var message = $"Choose level from 1 to {numberOfLevels}:";
            while (true)
            {
                Console.Clear();
                Console.WriteLine(message);
                GraphicsReader.PrintGraphics("KNIFE");
                Console.Write("\n");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (char.IsDigit(keyInfo.KeyChar))
                {
                    int level = int.Parse(keyInfo.KeyChar.ToString());
                    if (level >= 1 && level <= numberOfLevels)
                    {
                        return level;
                    }

                    message=$"Invalid input. Please enter a number between 1 and {numberOfLevels}.";

                }
                else
                {
                    message=$"Invalid input. Please enter a number between 1 and {numberOfLevels}.";
                }
            }
        }

        private static void SaveGame(Map levelData,string levelIndex)
        {
            if (!Directory.Exists(pathToSavedData))
            {
                Directory.CreateDirectory(pathToSavedData);
            }
            
            string fileName = $"{levelIndex}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            string filePath = Path.Combine(pathToSavedData, fileName);

            try
            {
                StringBuilder fileContent = new StringBuilder();
                fileContent.AppendLine(SerializeMapData(levelData.MapData));
                fileContent.AppendLine(dataString);
                fileContent.AppendLine($"CT|{currentturns}");
                fileContent.AppendLine($"P|H|{player.currentHealth}");
                fileContent.AppendLine($"P|A|{player.ammonition}");
                fileContent.AppendLine($"P|I|{new string(player.inventory)}");
                //fileContent.AppendLine("Terrorists:");
                foreach (var terrorist in terrorists)
                {
                    fileContent.AppendLine($"T|H|{terrorist.position[0]}|{terrorist.position[1]}|{terrorist.health}");
                }
                // Write the file
                File.WriteAllText(filePath, fileContent.ToString());
                levelData.CustomMessage($"Game saved {filePath}");
            }
            catch (Exception ex)
            {
                levelData.CustomMessage($"Error saving game: {ex.Message}");
            }
        }
        private static string SerializeMapData(char[,] mapData)
        {
            StringBuilder sb = new StringBuilder();

            int rows = mapData.GetLength(0);
            int cols = mapData.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(mapData[i, j]);
                }
                if(i==rows-1) continue;
                sb.AppendLine(); // Add a new line after each row
            }

            return sb.ToString();
        }
        private static void PickUpItem(Map levelData)
        {
            if (player.IsInventoryFull())
            {
                levelData.CustomMessage("Inventory is full");
                return;
            }
            int playerY = player.position[0];
            int playerX = player.position[1];
            
            int mapHeight = levelData.MapData.GetLength(0);
            int mapWidth = levelData.MapData.GetLength(1);
            
            
            for (int i = 0; i < 8; i++)
            {
                int newY = playerY + deltaY[i];
                int newX = playerX + deltaX[i];
                if (player.IsInventoryFull()) break;
          
                if (newY >= 0 && newY < mapHeight && newX >= 0 && newX < mapWidth)
                {
                    var dataInMap = levelData.MapData[newY, newX];
                    if(dataInMap=='T' || dataInMap=='W' ||dataInMap== emptyMapCellChar) continue;
                    player.AddToInventory(dataInMap);
                    levelData.ChangeMapData(newY,newX,emptyMapCellChar);
                }
            }
        }
        private static void LoadLevel(String customLevelPath,bool customLevel)
        {
            LevelMapHandler.ResetLevels();
         
            var map = LevelMapHandler.ReadLevelFromTxt(customLevelPath,customLevel);
            if (map != null)
            {
                LevelMapHandler.AddLevel(map);
                Console.WriteLine($"Loaded level from {customLevelPath}");
            }
        }

     
        private static void MoveControler(int height, int width, ConsoleKey key,Map currentMap)
        {
            int currentX = player.position[1];
            int currentY = player.position[0];
            bool didMoove = false;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (currentY > 0 &&  currentMap.GetDataForPosition(player.position[0]-1,player.position[1])==emptyMapCellChar)
                    {
                        player.position[0]--; // Move up
                        didMoove = true;
                    }

                    break;

                case ConsoleKey.DownArrow:
                    if (currentY < height - 1 &&  currentMap.GetDataForPosition(player.position[0]+1,player.position[1])==emptyMapCellChar) {
                        player.position[0]++; // Move down
                        didMoove = true;
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (currentX > 0 &&  currentMap.GetDataForPosition(player.position[0],player.position[1]-1)==emptyMapCellChar) {
                        player.position[1]--; // Move left
                        didMoove = true;
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (currentX < width - 1&&  currentMap.GetDataForPosition(player.position[0],player.position[1]+1)==emptyMapCellChar) {
                            player.position[1]++; // Move right
                            didMoove = true;
                    }
                    break;
            }
            if (didMoove)
            {
                currentMap.ChangeMapData(player.position[0],player.position[1],'P');
                currentMap.ChangeMapData(currentY,currentX,emptyMapCellChar);
                currentturns -= 1;
            } 
        }

        private static void Shooting(int[] cursorPos, int[] shooterPos, int bulletRange, Map currentMap)
        {
            if (player.ammonition == 0) return;
            if (cursorPos[0] == shooterPos[0] && cursorPos[1] == shooterPos[1]) return;
            // Calculate the differences between shooter and target positions
            int dx = cursorPos[1] - shooterPos[1]; 
            int dy = cursorPos[0] - shooterPos[0]; 
            player.ammonition -= 1;
            double angleToTarget = Math.Atan2(dy, dx) * 180 / Math.PI+90;

            // Determine the closest direction
            int closestDirection = 0; // 0: up, 1: up-right, 2: right, 3: down-right, 4: down, 5: down-left, 6: left, 7: up-left
            double minAngleDiff = double.MaxValue;
            for (int i = 0; i < 8; i++) {
                double segmentStartAngle = i * 45;
                double angleDiff = Math.Abs(angleToTarget - segmentStartAngle);
                if (angleDiff < minAngleDiff) {
                    minAngleDiff = angleDiff;
                    closestDirection = i;
                }
            }

            int stepY=0;
            int stepX=0;
            switch (closestDirection) {
                case 0: // Up
                    stepX = 0;
                    stepY = -1;
                    break;
                case 1: // Up-Right
                    stepX = 1;
                    stepY = -1;
                    break;
                case 2: // Right
                    stepX = 1;
                    stepY = 0;
                    break;
                case 3: // Down-Right
                    stepX = 1;
                    stepY = 1;
                    break;
                case 4: // Down
                    stepX = 0;
                    stepY = 1;
                    break;
                case 5: // Down-Left
                    stepX = -1;
                    stepY = 1;
                    break;
                case 6: // Left
                    stepX = -1;
                    stepY = 0;
                    break;
                case 7: // Up-Left
                    stepX = -1;
                    stepY = -1;
                    break;
            }
// Simulate bullet travel along the chosen direction
            int bulletX = shooterPos[1];
            int bulletY = shooterPos[0];
            List<int> bulletXPath = new List<int>();
            List<int> bulletYPath = new List<int>();
            for (int step = 0; step < bulletRange; step++)
            {
                bulletX += stepX;
                bulletY += stepY;
                var mapCell =currentMap.GetDataForPosition(bulletY, bulletX);
                
                if (mapCell != emptyMapCellChar)
                {
                    Thread.Sleep(200); 
                    break;
                }
                bulletXPath.Add(bulletX);
                bulletYPath.Add(bulletY);
                currentMap.ChangeMapData(bulletY,bulletX,'.');
                
                currentMap.DrawMap(cursorPos[1],cursorPos[0]);
          
                Thread.Sleep(100); // Delay to simulate bullet movement

            }

            for (int i=0;i<bulletXPath.Count;i++)
            {
                bulletX = bulletXPath[i];
                bulletY = bulletYPath[i];
                currentMap.ChangeMapData(bulletY,bulletX,emptyMapCellChar);
            }
            Thread.Sleep(300);
        }

        private static void Reset(String customLevelPath,bool customLevel)
        {
            currentturns = AVAIBLETURNS;
            terrorists.Clear();
            player = null;
            //LevelMapHandler.ResetLevels();
            LoadLevel(customLevelPath,customLevel);
         
        }
      
    }
    
}