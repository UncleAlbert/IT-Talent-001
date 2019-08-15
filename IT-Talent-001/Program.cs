using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace IT_Talent_001
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Application Started");

            // Assuming that the directory to monitor will be passed as a starting argument
            // Also assuming that a single file could also be specified.
            if (args.Length > 0)
            {
                //Loop through each argument an if not a file or directory display a message on the console
                foreach (string path in args)
                {
                    // Check to see if the path is a file first.  If it does exist it is a file
                    if (File.Exists(path))
                    {
                        Console.WriteLine("The file {0} exists.", path);
                        //Process the file
                        ProcessFile(path);

                    }
                    // Check to see if the path is a directory.   If it does exist it is a directory
                    else if (Directory.Exists(path))
                    {

                    }
                    // If the path is neither a file or a directory it cannot be processed so display an appropriate message in the console.
                    else
                    {
                        Console.WriteLine("{0} is not a valid file or directory,", path);
                    }

                }
            }
            else
            {
                Console.WriteLine("No Directory or File Specified");
            }

            Console.WriteLine("Application Stopped");

            //The Console.Read() statement will pause the console until a key is pressed
            Console.Read();
        }

        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            foreach (string fileName in fileEntries)
            {
                //ProcessFile(fileName);
            }                

            // If the directory contains sub-directories then we can recurse into those directory subdirectories.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory);
            }
        }

        public static void ProcessFile(string path)
        {
            Console.WriteLine("Commenced processing of file {0}.", path);

            //We need to check that the file is a csv file
            Console.WriteLine("Checking File Type");
            string fileExtension;
            fileExtension = Path.GetExtension(path);

            if (fileExtension.ToUpper() == "CSV")
            {
                Console.WriteLine("File Type is : " + fileExtension.ToUpper() + " and IS valid.");

                //Use CSV Helper to load the data 
                //We need an order list object to store the data from each order object
                List<Order> OrderList = new List<Order>();

                using (TextReader reader = File.OpenText(path))
                {
                    CsvReader fileToProcess = new CsvReader(reader);
                    fileToProcess.Configuration.Delimiter = ",";
                    fileToProcess.Configuration.MissingFieldFound = null;
                    while (fileToProcess.Read())
                    {
                        Order orderRecord = fileToProcess.GetRecord<Order>();
                        OrderList.Add(orderRecord);
                    }
                }
            }
            else
            {
                Console.WriteLine("File Type is : " + fileExtension.ToUpper() + " and IS NOT valid.");
            }






            Console.WriteLine("Completed processing of file {0}.", path);
        }
    }
}
