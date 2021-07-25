using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Runtime.Caching;

namespace DataProcessor
{
    internal class Program
    {
        //private static ConcurrentDictionary<string, string> FilesToProcess = new ConcurrentDictionary<string, string>();

        private static MemoryCache FilesToProcess = MemoryCache.Default;

        private static void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);

            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFile,
                SlidingExpiration = TimeSpan.FromSeconds(2)
            };

            FilesToProcess.Add(item, policy);
        }

        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File Created {e.Name} type {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            //FilesToProcess.TryAdd(e.FullPath, e.FullPath);
            AddToCache(e.FullPath);
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File Changed {e.Name} type {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            //FilesToProcess.TryAdd(e.FullPath, e.FullPath);

            AddToCache(e.FullPath);
        }

        private static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File Deleted {e.Name} type {e.ChangeType}");
        }

        private static void FileRenemed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File Redenemed {e.OldName} to {e.Name} type {e.ChangeType}");
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Parsing Command line options");

            //var command = args[0];

            //if (command == "--file")
            //{
            //    var filePath = args[1];
            //    Console.WriteLine($"Single File {filePath} Selected");
            //    ProcessSingleFile(filePath);
            //}
            //else if (command == "--dir")
            //{
            //    var directioPath = args[1];
            //    var fileType = args[2];
            //    Console.WriteLine($"Directory {directioPath} selected for {fileType} files");
            //    ProcessDirectory(directioPath, fileType);
            //}
            //else
            //{
            //    Console.WriteLine("Invalid command");
            //}

            //Console.WriteLine("Press endter to quit");
            //Console.ReadLine();

            var directoryToWatch = args[0];

            if (!Directory.Exists(directoryToWatch))
            {
                Console.WriteLine($"Error {directoryToWatch} doestn exist");
            }
            else
            {
                Console.WriteLine($"Watching directory {directoryToWatch}");

                ProcessExisitngFiles(directoryToWatch);
                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
                // using (var time = new Timer(ProcessFiles, null, 0, 1000))
                {
                    inputFileWatcher.IncludeSubdirectories = false;
                    inputFileWatcher.InternalBufferSize = 32768;
                    inputFileWatcher.Filter = "*.*";
                    inputFileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                    inputFileWatcher.Created += FileCreated;
                    inputFileWatcher.Changed += FileChanged;
                    inputFileWatcher.Deleted += FileDeleted;
                    inputFileWatcher.Renamed += FileRenemed;
                    inputFileWatcher.Error += WatcherError;

                    inputFileWatcher.EnableRaisingEvents = true;

                    Console.WriteLine("Press enter to quit");
                    Console.ReadKey();
                }
            }
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

        private static void ProcessExisitngFiles(string directoryToWatch)
        {
            Console.WriteLine($" Checking {directoryToWatch} For File");
            foreach (var filePath in Directory.EnumerateFiles(directoryToWatch))
            {
                Console.WriteLine($"           - found {filePath}");
                AddToCache(filePath);
            }
        }

        private static void ProcessFile(CacheEntryRemovedArguments arguments)
        {
            Console.WriteLine($"Cache item removed {arguments.CacheItem.Key} because {arguments.RemovedReason}");

            if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(arguments.CacheItem.Key);
                fileProcessor.Process();
            }
            else
            {
                Console.WriteLine($"Warning {arguments.CacheItem.Key} was removed unexpectedly");
            }
        }

        //private static void ProcessFiles(object state)
        //{
        //    foreach (var fileName in FilesToProcess.Keys)
        //    {
        //        if (FilesToProcess.TryRemove(fileName, out _))
        //        {
        //            var fileProcessor = new FileProcessor(fileName);
        //            fileProcessor.Process();
        //        }
        //    }
        //}

        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }

        private static void WatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"Error {e.GetException()}");
        }
    }
}