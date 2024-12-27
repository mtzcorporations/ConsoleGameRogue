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
        GenerateMaze(height, width);

        // Step 3: Create open areas
        //CreateOpenAreas();

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

    private void GenerateMaze(int sizeY, int sizeX)
    {
        Random rand = new Random();
        int numBlocks = rand.Next(7,12);
        if (sizeX > 100) numBlocks += 2;
        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                if (i == 0 || i == sizeY - 1 || j==0 || j==sizeX-1)
                {
                    map[i, j] = 'W';
                }
                else
                {
                    map[i, j] = ' ';
                }
            }
        }
        int minimalBlockDistance = 3; // At least 3 empty spaces between walls
        int usualDistance = sizeX / numBlocks;
        int lastBlockEndX = -minimalBlockDistance; 

        for (int i = 0; i < numBlocks; i++)
        {
            Random rnd = new Random();
            int blockHeight = rnd.Next(sizeY / 2 + 3, sizeY - 3);
            int blockWidth = rnd.Next(2, 5); // Randomize wall thickness
            // Ensure the next wall starts at least minimalBlockDistance away from the previous wall
            int startX = Math.Max(lastBlockEndX + minimalBlockDistance, i * usualDistance + rnd.Next(0, usualDistance));
            // ensure the wall fits within the map boundaries
            startX = Math.Min(startX, sizeX - blockWidth);

            int startY;
            int wallPositionChoice = rnd.Next(3); // 0 = top edge, 1 = bottom edge, 2 = middle

            if (wallPositionChoice == 0)
            {
                // Top edge
                startY = 0;
            }
            else if (wallPositionChoice == 1)
            {
                // Bottom edge
                startY = sizeY - blockHeight;
            }
            else
            {
                // Middle
                startY = rnd.Next(0, sizeY - blockHeight);
            }

            // Place the wall in the map
            for (int j = 0; j < blockWidth; j++)
            {
                for (int k = 0; k < blockHeight; k++)
                {
                    if (startY + k < sizeY && startX + j < sizeX)
                    {
                        map[startY + k, startX + j] = 'W';
                    }
                }
            }

            // Update the end position of the current wall
            lastBlockEndX = startX + blockWidth - 1;
        }
    }

 

    private void PlacePlayer()
    {
        while (true)
        {
            int y = random.Next(1, map.GetLength(0) - 1);
            int x = random.Next(1, map.GetLength(1) - 1);

            if (map[y, x] == ' ') // Place player in an open cell
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

                if (map[y, x] == ' ') // Place objects in open cells
                {
                    map[y, x] = objectType;
                    break;
                }
            }
        }
    }
    
}
    
}