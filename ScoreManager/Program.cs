using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ScoreManager
{
    class Program
    {
        public static string ballancePath = "";

        static void Main(string[] args)
        {
            //get ballance folder
            ConsoleAssistance.WriteLine("Searching Ballance path in register...", ConsoleColor.Yellow);
            if (Environment.Is64BitOperatingSystem)
            {
                ballancePath = RegisterHelper.GetRegValue("SOFTWARE\\Wow6432Node\\Ballance\\Settings", "TargetDir");
            }
            else
            {
                ballancePath = RegisterHelper.GetRegValue("SOFTWARE\\Ballance\\Settings", "TargetDir");
            }

            if (string.IsNullOrEmpty(ballancePath))
            {
                ConsoleAssistance.WriteLine("Can't find Ballance in register", ConsoleColor.Red);
                Console.WriteLine(@"Please input Ballance root path: (without \) (which contains database.tdb)");
                ballancePath = Console.ReadLine();
                if (!System.IO.File.Exists(ballancePath + "\\database.tdb"))
                {
                    ConsoleAssistance.WriteLine("Game not found. Exiting program.", ConsoleColor.Red);
                    Environment.Exit(0);
                }
            }
            ConsoleAssistance.WriteLine("Game detected in " + ballancePath, ConsoleColor.Green);

            //pre-generate
            var t = new Token();
            if (!t.Generate())
            {
                Console.WriteLine("Token generation failed. Exiting program. (Consider run as Administrator?)");
                Console.ReadLine();
                Environment.Exit(0);
            }

            //check file
            ConsoleAssistance.WriteLine("Deploying injection files...", ConsoleColor.Yellow);
            System.IO.File.Copy(Environment.CurrentDirectory + @"\MenuLevel.nmo", ballancePath + @"\3D Entities\MenuLevel.nmo", true);
            System.IO.File.Copy(Environment.CurrentDirectory + @"\ScoreManager.nmo", ballancePath + @"\3D Entities\ScoreManager.nmo", true);

            ConsoleAssistance.WriteLine("Initializing log watcher...", ConsoleColor.Yellow);
            var watcher = new FileSystemWatcher
            {
                Path = ballancePath + "\\Bin",
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.bsm"
            };

            // watching log files
            watcher.Changed += (sender, e) =>
            {
                ExportTxt exportTxt = null;

                if (System.IO.File.Exists(ballancePath + "\\Bin\\ScoreOutput.bsm"))
                {
                    var file = new FileStream(ballancePath + "\\Bin\\ScoreOutput.bsm", FileMode.Open);
                    var txtReader = new TxtReader(file);
                    file.Close();
                    System.IO.File.Delete(ballancePath + "\\Bin\\ScoreOutput.bsm");
                    exportTxt = txtReader.GetExport();
                }
                if (exportTxt != null)
                {
                    if (exportTxt.HSScore == -1)
                    {
                        ConsoleAssistance.WriteLine($"Log file change detected. Level {exportTxt.LvID.ToString()} has started.", ConsoleColor.Yellow);
                        
                    }
                    else
                    {
                        ConsoleAssistance.WriteLine("Log file change detected. Level has ended.", ConsoleColor.Yellow);
                        Console.WriteLine("Level ID: " + exportTxt.LvID);
                        ConsoleAssistance.WriteLine("Speedrun data:", ConsoleColor.Magenta);            
                        Console.WriteLine("Score: " + exportTxt.HSScore);
                        Console.WriteLine("Score Counter-based time: {0}:{1:00.0}", exportTxt.SRTime / 120, (exportTxt.SRTime % 120) / 2.0);
                        Console.WriteLine("Reference time: " + exportTxt.ReferenceTime + "ms");

                        ConsoleAssistance.WriteLine("Statistics data:", ConsoleColor.Magenta);
                        Console.WriteLine($"Life up: {exportTxt.LifeUp}");
                        Console.WriteLine($"Life lost: {exportTxt.LifeLost}");
                        Console.WriteLine($"Extra point collected: {exportTxt.ExtraPoints}");
                        Console.WriteLine($"Sub extra points collected: {exportTxt.SubExtraPoints}");
                        Console.WriteLine($"Trafo passed: {exportTxt.Trafo}");
                        Console.WriteLine($"Checkpoint passed: {exportTxt.Checkpoint}");

                        t.Generate();
                    }
                    Console.WriteLine("-----------------------------");
                }
            };
            watcher.EnableRaisingEvents = true;

            ConsoleAssistance.WriteLine("Launching Ballance...", ConsoleColor.Yellow);
            void CreateShortCut()
            {
                var shell = new WshShell();
                var shortcut = (IWshShortcut)shell.CreateShortcut("rungame.lnk");
                shortcut.TargetPath = ballancePath + "\\Bin\\Player.exe";
                shortcut.WorkingDirectory = ballancePath + "\\Bin\\";

                shortcut.Save();
            }

            try
            {
                CreateShortCut();
                Process.Start("rungame.lnk");
                System.IO.File.Delete(ballancePath + "\\Bin\\rungame.lnk");
            }
            catch (Exception)
            {
                ConsoleAssistance.WriteLine("Automatic launch failed. Please launch ballance manually.", ConsoleColor.Red);
                Console.ReadLine();
            }

            Console.WriteLine("Press Z to exit.", ConsoleColor.Yellow);
            Console.WriteLine("-----------------------------");

            // infinite loop
            while (true)
            {
                var result = Console.ReadKey(true);
                if (result.Key == ConsoleKey.Z) break;
                Thread.Sleep(500);
            }

            watcher.EnableRaisingEvents = false;
            t.Destroy();
            watcher.Dispose();
        }
    }
}
