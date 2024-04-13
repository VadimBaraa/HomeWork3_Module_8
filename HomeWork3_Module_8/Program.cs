using System;
using System.IO;
using System.Security.AccessControl;

class Program
{
    static void Main(string[] args)
    {
        try
        {

            Console.WriteLine("Введите путь к директории:");
            string directoryPath = Console.ReadLine();


            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Указанной директории не существует.");
                return;
            }

            DirectorySecurity directorySecurity = new DirectoryInfo(directoryPath).GetAccessControl();
            if (!directorySecurity.AreAccessRulesProtected)
            {
                Console.WriteLine("Права доступа к директории не защищены.");
            }
            else
            {
                Console.WriteLine("Права доступа к директории защищены.");
            }

            WriteBeforeSize(directoryPath);
            
            DateTime currentTime = DateTime.Now;


            TimeSpan timeLimit = TimeSpan.FromMinutes(1);


            string[] files = Directory.GetFiles(directoryPath);
            string[] subdirectories = Directory.GetDirectories(directoryPath);


            Console.Write("Файлы в указанной директории:\t");
            foreach (string file in files)
            {
                Console.Write(file, ", ");
            }
            Console.WriteLine();


            Console.Write("Поддиректории в указанной директории:\t");
            foreach (string subdirectory in subdirectories)
            {
                Console.Write(subdirectory, ", ");
            }
            Console.WriteLine();


            // Проходимся по всем файлам и проверяем, когда они были последний раз доступны
            foreach (string filePath in files)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (currentTime - fileInfo.LastAccessTime > timeLimit)
                {

                    File.Delete(filePath);
                    Console.WriteLine($"Файл {filePath} был удален.");
                }
            }

            // Проходимся по всем поддиректориям и их содержимому
            foreach (string subdirectory in subdirectories)
            {

                DeleteFilesInDirectory(subdirectory, currentTime, timeLimit);

                FileInfo subdirectoryInfo = new FileInfo(subdirectory);
                if (currentTime - subdirectoryInfo.LastAccessTime > timeLimit)
                {

                    File.Delete(subdirectory);
                    Console.WriteLine($"Файл {subdirectory} был удален.");
                }


                if (Directory.GetFiles(subdirectory).Length == 0 && Directory.GetDirectories(subdirectory).Length == 0)
                {
                    Directory.Delete(subdirectory);
                    Console.WriteLine($"Пустая директория {subdirectory} была удалена.");
                }


                Console.WriteLine("Операция завершена успешно.");
            }
            WriteAfterSize(directoryPath);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    // функция для удаления файлов в директории
    static void DeleteFilesInDirectory(string directoryPath, DateTime currentTime, TimeSpan timeLimit)
    {
        string[] files = Directory.GetFiles(directoryPath);

        foreach (string filePath in files)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (currentTime - fileInfo.LastAccessTime > timeLimit)
            {

                File.Delete(filePath);
                Console.WriteLine($"Файл {filePath} был удален.");
            }
        }

        // Рекурсивно вызываем функцию для всех поддиректорий
        string[] subdirectories = Directory.GetDirectories(directoryPath);
        foreach (string subdirectory in subdirectories)
        {
            DeleteFilesInDirectory(subdirectory, currentTime, timeLimit);
        }
    }

    static long GetDirectorySize(string directoryPath)
    {
        long size = 0;


        string[] files = Directory.GetFiles(directoryPath);


        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            size += fileInfo.Length;
        }


        string[] subdirectories = Directory.GetDirectories(directoryPath);


        foreach (string subdirectory in subdirectories)
        {
            size += GetDirectorySize(subdirectory);
        }

        return size;
    }

    static void WriteBeforeSize(string directoryPath)
    {
        long directorySize = GetDirectorySize(directoryPath);

        double SizeInKilobytes = (double)directorySize / 1024;
        int precision = 3;

        string KilobyteNumber = Math.Round((double)directorySize / 1024, precision).ToString();


        string GigabyteNumber = Math.Round((double)directorySize / (1024 * 1024 * 1024), precision).ToString();

        Console.WriteLine($"Размер папки {directoryPath} до очистки составляет {KilobyteNumber} кбайт ({GigabyteNumber} Гбайт)");

    }

    static void WriteAfterSize(string directoryPath)
    {
        long directorySize = GetDirectorySize(directoryPath);

        double SizeInKilobytes = (double)directorySize / 1024;
        int precision = 3;

        string KilobyteNumber = Math.Round((double)directorySize / 1024, precision).ToString();


        string GigabyteNumber = Math.Round((double)directorySize / (1024 * 1024 * 1024), precision).ToString();

        Console.WriteLine($"Размер папки {directoryPath} после очистки составляет {KilobyteNumber} кбайт ({GigabyteNumber} Гбайт)");

    }

}
