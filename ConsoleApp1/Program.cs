using System;
using System.IO;
using System.Net;

class Program
{
    static void Main()
    {
        string url = "https://filebin.net/z32p9c0y9voqxl61/lec16.txt";
        int numParts = 3;
        string destFile = "";

        try
        {
            string originalExtension;
            destFile = DownloadFile(url, out originalExtension);

            SplitFile(destFile, numParts);

            string[] inputFiles = new string[numParts];
            for (int i = 0; i < numParts; i++)
            {
                inputFiles[i] = $"{destFile}_part{i + 1}.dat";
            }
            string outputFile = "combined_file";
            CombineFiles(inputFiles, outputFile, originalExtension);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
        }
    }

    static string DownloadFile(string url, out string originalExtension)
    {
        Console.WriteLine("Downloading file...");
        using (WebClient webClient = new WebClient())
        {
            // Mendapatkan ekstensi file dari URL
            originalExtension = Path.GetExtension(url);
            string destFile = Path.GetFileName(new Uri(url).LocalPath);

            webClient.DownloadFile(url, destFile);

            return destFile;
        }
    }

    static void SplitFile(string inputFile, int numParts)
    {
        byte[] inputData = File.ReadAllBytes(inputFile);
        int partSize = (int)Math.Ceiling((double)inputData.Length / numParts);

        for (int i = 0; i < numParts; i++)
        {
            string partFile = $"{inputFile}_part{i + 1}.dat";
            int startIdx = i * partSize;
            int length = Math.Min(partSize, inputData.Length - startIdx);
            byte[] partData = new byte[length];
            Array.Copy(inputData, startIdx, partData, 0, length);
            File.WriteAllBytes(partFile, partData);
            Console.WriteLine($"Part {i + 1} downloaded: {partFile}");
        }

        Console.WriteLine("Downloaded Complete.");
    }

    static void CombineFiles(string[] inputFiles, string outputFile, string originalExtension)
    {
        Console.WriteLine("Combining files...");
        using (FileStream outputStream = new FileStream($"{outputFile}{originalExtension}", FileMode.Create))
        {
            foreach (string inputFile in inputFiles)
            {
                byte[] fileData = File.ReadAllBytes(inputFile);
                outputStream.Write(fileData, 0, fileData.Length);
                File.Delete(inputFile);
            }
            
        }
        Console.WriteLine($"Files combined: {outputFile}{originalExtension}");
    }
}
