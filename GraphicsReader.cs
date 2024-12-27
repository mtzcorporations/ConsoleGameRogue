using System;
using System.Collections.Generic;
using System.IO;

namespace CLI_ROGUERAMBOGAME
{
    public static class GraphicsReader
    {
        private static Dictionary<string, string> graphics = new Dictionary<string, string>();

        /// <summary>
        /// Reads all graphics files in the specified folder and stores them in a dictionary.
        /// The key is the file name without the extension, and the value is the content of the file.
        /// </summary>
        /// <param name="graphicsFolderPath">Path to the folder containing graphics files.</param>
        public static void ReadGraphics(string graphicsFolderPath)
        {
            if (!Directory.Exists(graphicsFolderPath))
                throw new DirectoryNotFoundException($"Graphics folder not found: {graphicsFolderPath}");

            var files = Directory.GetFiles(graphicsFolderPath, "*.*"); // Reads all files in the folder
            foreach (var file in files)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                var content = File.ReadAllText(file); // Read the entire content of the file

                if (!graphics.ContainsKey(fileNameWithoutExtension))
                {
                    graphics[fileNameWithoutExtension] = content;
                }
            }
        }

        /// <summary>
        /// Prints the desired graphic to the console by its key.
        /// </summary>
        /// <param name="key">The name of the graphic to print.</param>
        public static void PrintGraphics(string key)
        {
            if (graphics.TryGetValue(key, out var graphic))
            {
                Console.Write(graphic);
            }
            else
            {
                Console.WriteLine($"Graphic with key '{key}' not found.");
            }
        }
    }
}