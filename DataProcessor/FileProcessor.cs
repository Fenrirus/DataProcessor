namespace DataProcessor
{
    internal class FileProcessor
    {
        public FileProcessor(string filePath)
        {
            this.filePath = filePath;
        }

        public string filePath { get; }

        public void Process()
        {
            System.Console.WriteLine($"Begin process of {filePath}");
        }
    }
}