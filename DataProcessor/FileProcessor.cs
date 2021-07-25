namespace DataProcessor
{
    using System;
    using System.IO;

    internal class FileProcessor
    {
        private static readonly string Backup = "bacup";
        private static readonly string Complete = "complete";
        private static readonly string Inprogress = "inprogress";

        public FileProcessor(string filePath)
        {
            this.FilePath = filePath;
        }

        public string FilePath { get; }

        public void Process()
        {
            System.Console.WriteLine($"Begin process of {FilePath}");

            if (!File.Exists(FilePath))
            {
                System.Console.WriteLine("File doesnt exist");
            }

            string rootDirectoryPath = new DirectoryInfo(FilePath).Parent.Parent.FullName;

            System.Console.WriteLine($"Root directory {rootDirectoryPath}");

            string inputFilerDirectoryPath = Path.GetDirectoryName(FilePath);
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, Backup);

            //nie trzeba sprawdzać czy istnieje
            Directory.CreateDirectory(backupDirectoryPath);

            string inputFileName = Path.GetFileName(FilePath);
            var backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            Console.WriteLine($"Copy {FilePath} to {backupFilePath}");
            File.Copy(FilePath, backupFilePath, true);

            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, Inprogress));
            var inprogressFilePath = Path.Combine(rootDirectoryPath, Inprogress, inputFileName);

            if (File.Exists(inprogressFilePath))
            {
                Console.WriteLine($"File with the name {inprogressFilePath} is being processed");
                return;
            }

            System.Console.WriteLine($"Moving {FilePath} to {inprogressFilePath}");
            File.Move(FilePath, inprogressFilePath);

            string extension = Path.GetExtension(FilePath);
            switch (extension)
            {
                case ".txt":
                    ProcessTextFile(inprogressFilePath);
                    break;

                default:
                    Console.WriteLine($"{extension} is an unsupported file type.");
                    break;
            }

            string completedDirectoryPath = Path.Combine(rootDirectoryPath, Complete);
            Directory.CreateDirectory(completedDirectoryPath);

            Console.WriteLine($"Moving {inprogressFilePath} to {completedDirectoryPath}");
            //File.Move(inProgressFilePath, Path.Combine(completedDirectoryPath, inputFileName));

            var completedFileName =
                $"{Path.GetFileNameWithoutExtension(FilePath)}-{Guid.NewGuid()}{extension}";

            //completedFileName = Path.ChangeExtension(completedFileName, ".complete");

            var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

            File.Move(inprogressFilePath, completedFilePath);

            // jest wielowątkowe, więc jak nie ma folderu to się psuje
            //var inProgressDirectoryPath = Path.GetDirectoryName(inprogressFilePath);
            //Directory.Delete(inProgressDirectoryPath, true);
        }

        private void ProcessTextFile(string inProgressFilePath)
        {
            Console.WriteLine($"Processing text file {inProgressFilePath}");
            // Read in and process
        }
    }
}