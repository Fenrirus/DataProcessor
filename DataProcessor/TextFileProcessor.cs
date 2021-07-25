using System.IO;

namespace DataProcessor
{
    public class TextFileProcessor
    {
        public TextFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public string InputFilePath { get; set; }

        public string OutputFilePath { get; set; }

        public void Process()
        {
            //var originalText = File.ReadAllText(InputFilePath);
            //var processedText = originalText.ToUpperInvariant();
            //File.WriteAllText(OutputFilePath, processedText);

            //File.AppendAllText(InputFilePath, "cos");
            //var lines = File.ReadAllLines(InputFilePath);
            //lines[1] = lines[1].ToUpperInvariant();
            //File.WriteAllLines(OutputFilePath, lines);

            //using (var inputFileStream = new FileStream(InputFilePath, FileMode.Open))
            //using (var inputStreamReader = new StreamReader(inputFileStream))
            //using (var OutputFileStream = new FileStream(OutputFilePath, FileMode.Create))
            //using (var outputStreamWriter = new StreamWriter(OutputFileStream))

            using (var inputStreamReader = new StreamReader(InputFilePath))
            using (var outputStreamWriter = new StreamWriter(OutputFilePath))
            {
                while (!inputStreamReader.EndOfStream)
                {
                    var line = inputStreamReader.ReadLine();
                    var processedLine = line.ToUpperInvariant();
                    outputStreamWriter.WriteLine(processedLine);
                }
            }
        }
    }
}