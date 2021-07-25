using System;
using System.IO;

namespace DataProcessor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Parsing Command line options");

            var command = args[0];

            if (command == "--file")
            {
                var filePath = args[1];
                Console.WriteLine($"Single File {filePath} Selected");
                ProcessSingleFile(filePath);
            }
            else if (command == "--dir")
            {
                var directioPath = args[1];
                var fileType = args[2];
                Console.WriteLine($"Directory {directioPath} selected for {fileType} files");
                ProcessDirectory(directioPath, fileType);
            }
            else
            {
                Console.WriteLine("Invalid command");
            }

            Console.WriteLine("Press endter to quit");
            Console.ReadLine();
        }

        private static void ProcessDirectory(string directioPath, string fileType)
        {
            //var allFiles = Directory.GetFiles(directioPath);
            switch (fileType)
            {
                case "TEXT":
                    var textFile = Directory.GetFiles(directioPath, "*txt");
                    foreach (var textFilePath in textFile)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.Process();
                    }
                    break;

                default:
                    Console.WriteLine($"{fileType} not suported");
                    return;
            }
        }

        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }
    }
}