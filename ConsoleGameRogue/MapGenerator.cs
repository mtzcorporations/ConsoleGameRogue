using System;
using System.Collections.Generic;
namespace CLI_ROGUERAMBOGAME
{
    public class MapGenerator
{
    private char[,] map;
    private Random random = new Random();

    public char[,] GenerateMap(int width, int height, int numTerrorists, int numBigHealths, int numSmallHealths, int numBigAmmo, int numSmallAmmo)
    {
        map = new char[height, width];

        // Step 1: Fill the map with walls
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[y, x] = 'W';
            }
        }

        // Step 2: Generate maze using Recursive Backtracking
        GenerateMaze(1, 1);

        // Step 3: Create open areas
        CreateOpenAreas();

        // Step 4: Place player
        PlacePlayer();

        // Step 5: Place items and enemies
        PlaceObjects('T', numTerrorists); // Terrorists
        PlaceObjects('H', numBigHealths); // Big Health
        PlaceObjects('h', numSmallHealths); // Small Health
        PlaceObjects('A', numBigAmmo); // Big Ammo
        PlaceObjects('a', numSmallAmmo); // Small Ammo

        return map;
    }

    private void GenerateMaze(int startY, int startX)
    {
        // Directions: [dy, dx]
        int[] deltaY = { -2, 2, 0, 0 };
        int[] deltaX = { 0, 0, -2, 2 };

        // Shuffle directions for randomness
        for (int i = 0; i < deltaY.Length; i++)
        {
            int swapIndex = random.Next(i, deltaY.Length);
            (deltaY[i], deltaY[swapIndex]) = (deltaY[swapIndex], deltaY[i]);
            (deltaX[i], deltaX[swapIndex]) = (deltaX[swapIndex], deltaX[i]);
        }

        map[startY, startX] = '.'; // Mark starting point as a path

        for (int i = 0; i < deltaY.Length; i++)
        {
            int newY = startY + deltaY[i];
            int newX = startX + deltaX[i];

            // Check if the new cell is valid
            if (newY > 0 && newY < map.GetLength(0) - 1 && newX > 0 && newX < map.GetLength(1) - 1 && map[newY, newX] == 'W')
            {
                // Carve a path between cells
                map[startY + deltaY[i] / 2, startX + deltaX[i] / 2] = '.';
                GenerateMaze(newY, newX); // Recurse
            }
        }
    }

    private void CreateOpenAreas()
    {
        int openAreaCount = random.Next(3, 6); // Number of open areas to create
        for (int i = 0; i < openAreaCount; i++)
        {
            int areaWidth = random.Next(3, 6);
            int areaHeight = random.Next(3, 6);
            int startY = random.Next(1, map.GetLength(0) - areaHeight - 1);
            int startX = random.Next(1, map.GetLength(1) - areaWidth - 1);

            for (int y = 0; y < areaHeight; y++)
            {
                for (int x = 0; x < areaWidth; x++)
                {
                    map[startY + y, startX + x] = '.';
                }
            }
        }
    }

    private void PlacePlayer()
    {
        while (true)
        {
            int y = random.Next(1, map.GetLength(0) - 1);
            int x = random.Next(1, map.GetLength(1) - 1);

            if (map[y, x] == '.') // Place player in an open cell
            {
                map[y, x] = 'P';
                break;
            }
        }
    }

    private void PlaceObjects(char objectType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            while (true)
            {
                int y = random.Next(1, map.GetLength(0) - 1);
                int x = random.Next(1, map.GetLength(1) - 1);

                if (map[y, x] == '.') // Place objects in open cells
                {
                    map[y, x] = objectType;
                    break;
                }
            }
        }
    }

    public void PrintMap()
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                Console.Write(map[y, x]);
            }
            Console.WriteLine();
        }
    }
}
    
}