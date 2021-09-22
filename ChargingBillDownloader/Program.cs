
using System.IO;
using System;
using System.Diagnostics;
using System.Text;
namespace ChargingBillDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            bool MoreThan1 = false;
            String CurrentDir = Directory.GetCurrentDirectory();
            String Path = CurrentDir + @"\Config.ini";
            var LoadingPath = new string("");
            if (File.Exists(CurrentDir + @"\Config.ini"))
            {
                var Config = new StreamReader(File.OpenRead(CurrentDir + @"\Config.ini"));
                var PathLine = Config.ReadLine();
                var ConfigLine = PathLine.Split('=');
                var LastPathLine = ConfigLine.GetUpperBound(0);
                Config.Close();
                if (LastPathLine == 1 & ConfigLine[0] == "Path")
                {
                    LoadingPath = ConfigLine[LastPathLine];
                }
                else
                {
                    FileStream WriteConfig = File.Create(Path);
                    byte[] info = new UTF8Encoding(true).GetBytes(@"Path=C:\Ladedetails\");
                    WriteConfig.Write(info, 0, info.Length);
                    WriteConfig.Close();
                    LoadingPath = @"C:\Ladedetails\";
                    Console.WriteLine("Konfigurationsdatei Fehlerhaft.");
                    Console.WriteLine("Konfigurationsdatei wurde neu erstellt.");
                    Console.WriteLine("weiter mit \"Enter\"");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            else
            {
                FileStream WriteConfig = File.Create(Path);
                byte[] info = new UTF8Encoding(true).GetBytes(@"Path=C:\Ladedetails\");
                WriteConfig.Write(info, 0, info.Length);
                LoadingPath = @"C:\Ladedetails\";
                WriteConfig.Close();
            }
            if (Directory.Exists(LoadingPath))
            {
                DirectoryInfo ParentDirectory = new(LoadingPath);
                int FileCount = 0;
                foreach (System.IO.FileInfo Filename in ParentDirectory.GetFiles())
                {
                    FileCount++;
                }
                var Files = new String[FileCount];
                if (FileCount > 1)
                {
                    MoreThan1 = true;
                }
                FileCount = 0;
                foreach (System.IO.FileInfo FileName in ParentDirectory.GetFiles())
                {
                    Files[FileCount] = FileName.ToString();
                    if (MoreThan1)
                    {
                        Console.WriteLine(FileCount + " für " + FileName);
                    }

                    FileCount++;
                }
                if (FileCount > 0)
                {
                    try
                    {
                        int iSelection;
                        if (FileCount > 1)
                        {
                            String Selection = Console.ReadLine();
                            iSelection = Int32.Parse(Selection);
                        }
                        else
                        {
                            iSelection = 0;
                        }
                        if (iSelection <= FileCount)
                        {
                            var reader = new StreamReader(File.OpenRead(Files[iSelection]));
                            int index = 0;
                            var Headline = reader.ReadLine();
                            var header = Headline.Split(',');
                            var LastHeadline = header.GetUpperBound(0);
                            if (header[LastHeadline] == "Invoice")
                            {
                                while (!reader.EndOfStream)
                                {
                                    reader.ReadLine();
                                    index += 1;
                                }
                                var InvoiceSource = new String[index];
                                var InvoiceDestination = new String[index];
                                reader = new StreamReader(File.OpenRead(Files[iSelection]));
                                reader.ReadLine();
                                int Pointer = 0;
                                int NewPointer = 0;

                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    var values = line.Split(',');
                                    var Last = values.GetUpperBound(0);
                                    InvoiceSource[Pointer] = values[Last];
                                    Pointer++;
                                }
                                for (int i = 0; i < Pointer; i++)
                                {
                                    Boolean xFound = false;
                                    for (int i2 = 0; i2 < Pointer; i2++)
                                    {
                                        if (InvoiceDestination[i2] == InvoiceSource[i] ^ InvoiceSource[i] == "https://tesla.com/teslaaccount/charging/invoice/")
                                        {
                                            xFound = true;
                                        }
                                    }
                                    if (!xFound)
                                    {
                                        InvoiceDestination[NewPointer] = InvoiceSource[i];
                                        NewPointer++;
                                    }
                                }
                                int i3;
                                for (i3 = 0; i3 < NewPointer; i3++)
                                {
                                    Process.Start("cmd.exe", "/c start " + InvoiceDestination[i3]);
                                }
                                Console.WriteLine(i3 + " Rechnungen wurden im Browser geöffnet.");
                                Console.WriteLine("weiter mit \"Enter\"");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Dokument ist nicht Gültig!");
                                Console.WriteLine("weiter mit \"Enter\"");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Auswahl nicht vorhanden!");
                            Console.WriteLine("weiter mit \"Enter\"");
                            Console.ReadLine();
                        }
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine("Falsche Eingabe!");
                        Console.WriteLine("weiter mit \"Enter\"");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Keine Dateien gefunden!");
                    Console.WriteLine("weiter mit \"Enter\"");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Ordner existiert nicht, soll er angelegt werden? (J/N)");
                bool choice = false;
                while (!choice)
                {
                    Console.Clear();
                    Console.WriteLine("Ordner existiert nicht, soll er angelegt werden? (J/N)");
                    string key = Console.ReadLine();
                    if (key == "J" ^ key == "j")
                    {
                        Directory.CreateDirectory(LoadingPath);
                        Console.Clear();
                        Console.WriteLine("Ordner " + LoadingPath + " wurde angelegt!");
                        Console.WriteLine("weiter mit \"Enter\"");
                        choice = true;
                    }
                    if (key == "N" ^ key == "n")
                    {
                        choice = true;
                        Console.WriteLine("Anwendung abgebrochen.");
                        Console.WriteLine("weiter mit \"Enter\"");
                    }
                }
                Console.ReadKey();
            }

        }
    }
}