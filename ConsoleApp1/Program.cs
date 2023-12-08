using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    const long thresholdSizeSmall = 1024 * 1024; // Atur ukuran batas untuk file kecil

    static async Task Main()
    {
        string url = "https://filebin.net/z32p9c0y9voqxl61/orch.mp3"; // Ganti dengan URL yang sesuai
        string outputDir = "folder_hasil"; // Ganti dengan direktori keluaran yang diinginkan
        bool keepPartition = true; // Sesuaikan kebutuhan Anda
        bool removePartition = true; // Sesuaikan kebutuhan Anda

        try
        {
            await DownloadAndSplit(url, outputDir, keepPartition, removePartition);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task DownloadAndSplit(string url, string outputDir, bool keepPartition, bool removePartition)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to initiate download: {response.ReasonPhrase}");
            }

            string baseFileName = GetBaseFileName(url);
            string outputFilePath = Path.Combine(outputDir, "gabungan", baseFileName);

            if (response.Content.Headers.ContentLength < thresholdSizeSmall)
            {
                Console.WriteLine("Small file. No need to split.");
                await DownloadToFile(url, outputFilePath);
                return;
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            int numPartitions = CalculateNumPartitions(response.Content.Headers.ContentLength.Value);

            Console.WriteLine($"Downloading {numPartitions} partitions...");

            long downloadSize = response.Content.Headers.ContentLength.Value / numPartitions;
            Task[] downloadTasks = new Task[numPartitions];
            string partitionDir = Path.Combine(outputDir, "file_partisi");
            string finalDir = Path.Combine(outputDir, "gabungan");

            Directory.CreateDirectory(partitionDir);
            Directory.CreateDirectory(finalDir);

            for (int i = 0; i < numPartitions; i++)
            {
                int partitionNum = i + 1;
                string partitionFileName = $"part{partitionNum}_{baseFileName}";
                string partitionPath = Path.Combine(partitionDir, partitionFileName);
                long startRange = i * downloadSize;
                long endRange = startRange + downloadSize - 1;

                if (i == numPartitions - 1)
                {
                    endRange = response.Content.Headers.ContentLength.Value - 1;
                }

                downloadTasks[i] = DownloadRange(url, partitionPath, startRange, endRange, partitionNum, numPartitions);
            }

            await Task.WhenAll(downloadTasks);

            Console.WriteLine("All partitions downloaded. Merging...");

            await MergePartitions(partitionDir, finalDir, outputFilePath, keepPartition, removePartition);

            Console.WriteLine("Download and merge complete.");
        }
    }

    static async Task DownloadToFile(string url, string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fileStream = File.Create(filePath))
                    {
                        await contentStream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
            }
        }
    }

    static string GetBaseFileName(string url)
    {
        Uri uri = new Uri(url);
        return Path.GetFileName(uri.LocalPath);
    }

    static int CalculateNumPartitions(long contentLength)
    {
        return (int)Math.Ceiling((double)contentLength / thresholdSizeSmall);
    }

    static async Task DownloadRange(string url, string filePath, long startRange, long endRange, int partitionNum, int numPartitions)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(startRange, endRange);

            using (HttpResponseMessage response = await client.SendAsync(request))
            {
                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fileStream = File.Create(filePath))
                    {
                        await contentStream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
            }
        }
    }

    static async Task MergePartitions(string partitionDir, string finalDir, string outputFilePath, bool keepPartition, bool removePartition)
    {
        string[] partitionFiles = Directory.GetFiles(partitionDir);

        using (FileStream outputFileStream = File.Create(outputFilePath))
        {
            foreach (string partitionFile in partitionFiles)
            {
                using (FileStream partitionFileStream = File.OpenRead(partitionFile))
                {
                    await partitionFileStream.CopyToAsync(outputFileStream);
                }

                if (!keepPartition && removePartition)
                {
                    File.Delete(partitionFile);
                }
            }
        }
    }
}
