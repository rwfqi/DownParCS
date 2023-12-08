# DownParCS
Partition Downloader dengan menggunakan bahasa C#
Program ini memungkinkan Anda mengunduh file dari URL yang diberikan, membaginya menjadi beberapa partisi, dan menggabungkannya kembali menjadi satu file.

Untuk menggunakan program Partition Downloader yang telah disesuaikan dalam bahasa C#, ikuti langkah-langkah di bawah ini:

1. Pastikan Anda memiliki lingkungan pengembangan C# yang terinstal di sistem Anda. Jika belum, Anda dapat mengunduhnya dari Visual Studio Official Website.

2. Buka terminal atau command prompt.
   Pindah ke direktori tempat Anda ingin menyimpan program dengan menggunakan perintah cd.
   ```cmd
   cd path/to/your/directory
   ```
3. Gunakan perintah berikut untuk mengunduh program dari repositori GitHub:
   ```cmd
   git clone https://github.com/rwfqi/DownParCS.git
   ```

4. Atur URL dan Konfigurasi.
   Buka proyek menggunakan lingkungan pengembangan C# (misalnya, Visual Studio atau Visual Studio Code).
   Temukan dan buka file sumber kode (.cs) dari proyek yang diunduh.
   Ganti nilai variabel url dengan URL file yang ingin Anda unduh.
   Sesuaikan nilai variabel numParts dengan jumlah bagian yang diinginkan untuk pembagian file.
    Contoh:
    ```C#
    string url = "https://filebin.net/z32p9c0y9voqxl61/lec16.txt";
    int numParts = 3;
    string destFile = "";
    ```
   
6. Eksekusi Program:
   Jalankan program melalui lingkungan pengembangan C# Anda dengan menggunakan
   ```cmd
   path/to/your/directory dotnet run
   ```
7. Tambahan:
   Jika anda ingin menyimpan partisi file setelah file telah digabungkan, anda dapat menghapus:
   ```C#
   File.Delete(inputFile);
   ```
   di bagian
   ```C#
   static void CombineFiles(string[] inputFiles, string outputFile, string originalExtension)
    {
        Console.WriteLine("Combining files...");
        using (FileStream outputStream = new FileStream($"{outputFile}{originalExtension}", FileMode.Create))
        {
            foreach (string inputFile in inputFiles)
            {
                byte[] fileData = File.ReadAllBytes(inputFile);
                outputStream.Write(fileData, 0, fileData.Length);
                File.Delete(inputFile); // <- Hapus bagian ini
            }
            
        }
        Console.WriteLine($"Files combined: {outputFile}{originalExtension}");
    }
   ```
Dibuat Oleh: Rifqi Ramadhani Hidayat
