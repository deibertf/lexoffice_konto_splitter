using CsvHelper;
using CsvHelper.Configuration;

namespace LexofficeKontenSplitter;

class Program
{
    static void Main(string[] args)
    {
        parseArguments(args, out string inputFile, out string outputDirectory);
               
        var config = new CsvConfiguration(new System.Globalization.CultureInfo("de-DE"))
        {
            Delimiter = ";",
            Quote = '"'
        };

        using (var reader = new StreamReader(inputFile))
        using (var csvReader = new CsvReader(reader, config))
        {
            var records = csvReader.GetRecords<dynamic>();

            var fileDictionary = new Dictionary<string, List<dynamic>>();

            foreach (var record in records)
            {
                var recordDict = record as IDictionary<string, object>;
                if (recordDict != null && recordDict.Count > 0)
                {
                    var firstColumnValue = recordDict.Values.ToList()[0].ToString();

                    if (!fileDictionary.ContainsKey(firstColumnValue))
                    {
                        fileDictionary[firstColumnValue] = new List<dynamic>();
                    }

                    fileDictionary[firstColumnValue].Add(record);
                }
            }

            foreach (var key in fileDictionary.Keys)
            {
                string outputFile = Path.Combine(outputDirectory, $"konto_{key}.csv");
                using (var writer = new StreamWriter(outputFile))
                using (var csvWriter = new CsvWriter(writer, config))
                {
                    csvWriter.WriteRecords(fileDictionary[key]);
                }
            }
        }
    }

    private static void parseArguments(
        string[] args,
        out string inputFile,
        out string outputDirectory)
    {
        inputFile = "";
        outputDirectory = "";

        // Parse command line arguments
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-i":
                    if (i + 1 < args.Length)
                    {
                        inputFile = args[i + 1];
                        i++; // skip next item
                    }
                    break;
                case "-o":
                    if (i + 1 < args.Length)
                    {
                        outputDir = args[i + 1];
                        i++; // skip next item
                    }
                    break;
            }
        }

        // Validate parameters
        if (string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
        {
            Console.WriteLine("Geben Sie bitte eine gültige Eingabedatei an mit -i <Dateipfad>");
            return;
        }

        if (string.IsNullOrEmpty(outputDirectory) || !Directory.Exists(outputDirectory))
        {
            Console.WriteLine("Geben Sie bitte ein gültiges Ausgabeverzeichnis an mit -o <Verzeichnispfad>");
            return;
        }
    }
}

