using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/*
TO-DO:
- GET LOGS DIRECTORY VIA USER INPUT
- Creat log file. -- Done
- Get paths via user instead of set path. -- Done but warnings of possible null (de)reference even when precautions are in place.
- Download, Upload and Sync option. -- Done (kind of roughly but can fix later if wanted.)
- Overwrite with old files option. (Useful to restoring to backup I guess?) -- Done.
- Include self folder.
- Add errors in log files.
*/

namespace BackupProgram
{
    class MainProgram
    {
        static public List<string> logList = new List<string>();
        static public string       pathToLogs = "";
        static public bool enableLogs = false;

        static public string      pathToE = ""; // path to E drive
        static public string      pathToNetworkDrive = ""; // path to network drive
        static public string[]    Edirs = {};
        static public string[]    networkDirs = {};
        static public string[]    Efiles = {};
        static public string[]    networkFiles = {};

        static public bool overWriteWithOldFiles = false;
        
        static void Main(string[] args)
        {
            AsciiArt();
            SetUp();

            Edirs = Directory.GetDirectories(pathToE, "*", SearchOption.AllDirectories); // Including all sub directories
            networkDirs = Directory.GetDirectories(pathToNetworkDrive, "*", SearchOption.AllDirectories); // Including all sub directories
            Efiles = Directory.GetFiles(pathToE, "*", SearchOption.AllDirectories);
            networkFiles = Directory.GetFiles(pathToNetworkDrive, "*", SearchOption.AllDirectories);
            
            AskOptions();
        }

        static void AsciiArt()
        {
            Console.WriteLine(@"
▒██   ██▒ ▄▄▄       ██▓▒███████▒▓█████  █     █░
▒▒ █ █ ▒░▒████▄    ▓██▒▒ ▒ ▒ ▄▀░▓█   ▀ ▓█░ █ ░█░
░░  █   ░▒██  ▀█▄  ▒██▒░ ▒ ▄▀▒░ ▒███   ▒█░ █ ░█ 
 ░ █ █ ▒ ░██▄▄▄▄██ ░██░  ▄▀▒   ░▒▓█  ▄ ░█░ █ ░█ 
▒██▒ ▒██▒ ▓█   ▓██▒░██░▒███████▒░▒████▒░░██▒██▓ 
▒▒ ░ ░▓ ░ ▒▒   ▓▒█░░▓  ░▒▒ ▓░▒░▒░░ ▒░ ░░ ▓░▒ ▒  
░░   ░▒ ░  ▒   ▒▒ ░ ▒ ░░░▒ ▒ ░ ▒ ░ ░  ░  ▒ ░ ░  
 ░    ░    ░   ▒    ▒ ░░ ░ ░ ░ ░   ░     ░   ░  
 ░    ░        ░  ░ ░    ░ ░       ░  ░    ░    
                       ░                         
[Program Name: SyncProgram]
[Current Version 1.0.0]
[Author: Jacob Taylor]
[Program Description: Easily backup and restore with more options than the regular copy and pasting method, and at a more efficient rate.]
");
        }

        static void SetUp()
        {
            Console.WriteLine("Would you like to enable logs?");

            while (true)
            {
                var enableLogsInput = Console.ReadLine();
                switch (enableLogsInput)
                {
                    case "yes":
                        enableLogs = true;
                        Console.WriteLine("Where would you like to save logs to?");
                        break;
                    case "no":
                        enableLogs = false;
                        break;
                    default:
                        Console.WriteLine("Invalid answer, please type yes or no.");
                        continue;
                }
                break;
            }
            if (enableLogs)
            {
                while (true)
                {
                    pathToLogs = Console.ReadLine() ?? string.Empty;
                    if (!Directory.Exists(pathToLogs)) {Console.WriteLine("Directory does not exist or is invalid."); continue;}
                    else {break;}
                }
            }

            Console.WriteLine("Enter directory of folder 1.");
            
            while (true)
            {
                pathToE = Console.ReadLine() ?? string.Empty;
                if (!Directory.Exists(pathToE)) {Console.WriteLine("Directory does not exist"); continue;}
                else {break;}
            }

            Console.WriteLine("Enter directory of folder 2.");

            while (true)
            {
                pathToNetworkDrive = Console.ReadLine() ?? string.Empty;
                if (!Directory.Exists(pathToNetworkDrive) && pathToNetworkDrive != pathToE) {Console.WriteLine("Directory does not exist or is the same as folder 1."); continue;}
                else {break;}
            }
        }

        static void AskOptions()
        {
            Console.WriteLine("Would you like to overwrite newer files with older files?");
            while (true)
            {
                string overWriteAnswer = Console.ReadLine() ?? string.Empty;
                switch (overWriteAnswer)
                {
                    case "yes":
                        overWriteWithOldFiles = true;
                        break;
                    case "no":
                        overWriteWithOldFiles = false;
                        break;
                    default:
                        Console.WriteLine("Invalid answer, please type yes or no.");
                        continue;
                }
                break;
            }

            Console.WriteLine("Would you like to upload your files, download or sync? ");
            while (true)
            {
                string options = Console.ReadLine() ?? string.Empty;
                switch (options)
                {
                    case "upload":
                        Console.WriteLine("uploading...");
                        Upload();
                        break;
                    case "download":
                        Console.WriteLine("downloading...");
                        Download();
                        break;
                    case "sync":
                        Console.WriteLine("syncing...");
                        Sync();
                        break;
                    default:
                        Console.WriteLine("Please type: 'upload', 'download', or 'sync'.");
                        continue;
                }
            }
        }

        static void Upload()
        {
            foreach(string dir in Edirs)
            {
                var info = new DirectoryInfo(dir);
                string shortDir = dir.Substring(pathToE.Length + 1);
                var tempDirectoryInfo = info.LastAccessTime;

                if (Directory.Exists(pathToNetworkDrive + @"\" + shortDir))
                {
                    Console.WriteLine("Directory " + pathToNetworkDrive + @"\" + shortDir + " already exists");
                    logList.Add("[" + DateTime.Now + "] " + "Directory " + pathToNetworkDrive + @"\" + shortDir + " already exists");
                }
                else
                {
                    Directory.CreateDirectory(pathToNetworkDrive + @"\" + shortDir);
                    Console.WriteLine("Created " + pathToNetworkDrive + @"\" + shortDir);
                    logList.Add("[" + DateTime.Now + "] " + "Created " + pathToNetworkDrive + @"\" + shortDir);
                }
            }

            foreach(var file in Efiles)
            {
                var fileInfo = new FileInfo(file);
                string filePath = file.Substring(pathToE.Length + 1);
                var fileLastAccessTime = fileInfo.LastAccessTime;

                if (File.Exists(pathToNetworkDrive + @"\" + filePath))
                {
                    var existingFile = new FileInfo(pathToNetworkDrive + @"\" + filePath);
                    if (fileLastAccessTime < existingFile.LastAccessTime && overWriteWithOldFiles == false)
                    {
                        Console.WriteLine("File " + fileInfo.Name + " is outdated or doesn't need updating");
                        logList.Add("[" + DateTime.Now + "] " + "File " + fileInfo.Name + " is outdated or doesn't need updating");
                    }
                    else if (fileLastAccessTime > existingFile.LastAccessTime || fileLastAccessTime < existingFile.LastAccessTime && overWriteWithOldFiles == true)
                    {
                        File.Delete(pathToNetworkDrive + @"\" + filePath);
                        File.Copy(file, pathToNetworkDrive + @"\" + filePath);
                        Console.WriteLine("Updated " + fileInfo.Name);
                        logList.Add("[" + DateTime.Now + "] " + "Updated" + fileInfo.Name);
                    }
                }
                else
                {
                    try
                    {
                        File.Copy(file, pathToNetworkDrive + @"\" + filePath);
                        Console.WriteLine("Copied " + fileInfo.Name);
                        logList.Add("[" + DateTime.Now + "] " + "Copied " + filePath + @"\" + fileInfo.Name);
                    }
                    catch (Exception e)
                    {
                        if (e.Source != null)
                        {
                            Console.WriteLine("Error source: {0]", e.Source);
                            logList.Add("Error source: " + e.Source);
                        }
                        throw;
                    }
                }
            }
            Logs();
            Console.WriteLine("Done uploading...");
            Console.ReadLine();
            System.Environment.Exit(1);
        }

        static void Download()
        {
            foreach(string dir in networkDirs)
            {
                var info = new DirectoryInfo(dir);
                string shortDir = dir.Substring(pathToNetworkDrive.Length + 1);
                var tempDirectoryInfo = info.LastAccessTime;

                if (Directory.Exists(pathToE + @"\" + shortDir))
                {
                    Console.WriteLine("Directory " + pathToE + @"\" + shortDir + " already exists");
                    logList.Add("[" + DateTime.Now + "] " + "Directory " + pathToE + @"\" + shortDir + " already exists");
                }
                else
                {
                    Directory.CreateDirectory(pathToE + @"\" + shortDir);
                    Console.WriteLine("Created " + pathToE + @"\" + shortDir);
                    logList.Add("[" + DateTime.Now + "] " + "Created " + pathToE + @"\" + shortDir);
                }
            }

            foreach(var file in networkFiles)
            {
                var fileInfo = new FileInfo(file);
                string filePath = file.Substring(pathToNetworkDrive.Length + 1);
                var fileLastAccessTime = fileInfo.LastAccessTime;

                if (File.Exists(pathToE + @"\" + filePath))
                {
                    var existingFile = new FileInfo(pathToE + @"\" + filePath);
                    if (fileLastAccessTime < existingFile.LastAccessTime && overWriteWithOldFiles == false)
                    {
                        Console.WriteLine("File " + fileInfo.Name + " is outdated or doesn't need updating");
                        logList.Add("File " + fileInfo.Name + " is outdated or doesn't need updating");
                    }
                    else if (fileLastAccessTime > existingFile.LastAccessTime || fileLastAccessTime < existingFile.LastAccessTime && overWriteWithOldFiles == true)
                    {
                        File.Delete(pathToE + @"\" + filePath);
                        File.Copy(file, pathToE + @"\" + filePath);
                        Console.WriteLine("Updated " + fileInfo.Name);
                        logList.Add("[" + DateTime.Now + "] " + "Updated" + fileInfo.Name);
                    }
                }
                else
                {
                    try
                    {
                        File.Copy(file, pathToE + @"\" + filePath);
                        Console.WriteLine("Copied " + fileInfo.Name);
                        logList.Add("[" + DateTime.Now + "] " + "Copied " + filePath + @"\" + fileInfo.Name);
                    }
                    catch (Exception e)
                    {
                        if (e.Source != null)
                        {
                            Console.WriteLine("Error source: {0]", e.Source);
                            logList.Add("Error source: " + e.Source);
                        }
                        throw;
                    }
                }
                
            }
            Logs();
            Console.WriteLine("Done Downloading...");
            Console.ReadLine();
            System.Environment.Exit(1);
        }

        static void Sync()
        {
            Upload();
            Download();
        }

        static void Logs()
        {
            string currentTime = DateTime.Now.ToString();
            currentTime = Regex.Replace(currentTime, @"[^\w\.@-]", " ");
            string logDir = pathToLogs + @"\" + "Logs";

            if (Directory.Exists(logDir) && enableLogs == true)
            {
                File.WriteAllLines(logDir + @"\" + currentTime + ".txt", logList);
            }
            else if (enableLogs == true)
            {
                Directory.CreateDirectory(pathToLogs + @"\" + "Logs");
                File.WriteAllLines(logDir + @"\" + currentTime + ".txt", logList);
            }
        }
    }
}