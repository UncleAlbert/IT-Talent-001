using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
                        Console.WriteLine("The file [{0}] exists.", path);
                        //Process the file
                        ProcessFile(path);

                    }
                    // Check to see if the path is a directory.   If it does exist it is a directory
                    else if (Directory.Exists(path))
                    {
                        Console.WriteLine("The directory [{0}] exists.", path);
                        ProcessDirectory(path);
                    }
                    // If the path is neither a file or a directory it cannot be processed so display an appropriate message in the console.
                    else
                    {
                        Console.WriteLine("[{0}] is not a valid file or directory,", path);
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
                ProcessFile(fileName);
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
            Console.WriteLine("Commenced processing of file [{0}].", path);

            //We need to check that the file is a csv file
            Console.WriteLine("Checking File Type");
            string fileExtension;
            fileExtension = Path.GetExtension(path);

            if (fileExtension.ToUpper().Substring(1) == "CSV")
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


                // Start Constructing XML Object
                XDocument xDocument = new XDocument();
                XDeclaration xDeclaration = new XDeclaration("1.0", "utf-16", "true");
                XProcessingInstruction xProcessing = new XProcessingInstruction("IT-Talent-Tech-Test", "result");
                XComment xComment = new XComment("This is output from a console application in response to a technical test for IT Talent");
                XElement xOrders = new XElement("ORDERS");
                XElement xOrder;
                XElement xConsignments = new XElement("CONSIGNMENTS");
                XElement xConsignment;
                XElement xParcels = new XElement("PARCELS");
                XElement xParcel = new XElement("PARCEL");
                XElement xParcelItems = new XElement("ITEMS");
                XElement xParcelItem = new XElement("ITEM");


                // Iterate through the collection to get the Total Value and Total Weight
                decimal orderTotalWeight = 0;
                decimal orderTotalValue = 0;
                string orderNumber = "";
                string previousOrderNumber = "";
                string orderConsignmentNumber = "";
                string previousOrderConsignmentNumber = "";
                string orderParcelCode = "";
                string previousOrderParcelCode = "";
                string orderItemDescription = "";

                foreach (Order orderRecord in OrderList)
                {
                    orderNumber = orderRecord.OrderNo;


                    orderTotalWeight = CalculateOrderWeight(orderNumber, OrderList);
                    orderTotalValue = CalculateOrderValue(orderNumber, OrderList);
                    //xConsignments = GetConsignments(orderNumber, OrderList);


                    if (previousOrderNumber != orderNumber)
                    {
                        // Construct ORDER XML Node
                        xOrder = new XElement("ORDER", orderNumber);
                        xOrder.Add(new XElement("TOTALWEIGHT", orderTotalWeight));
                        xOrder.Add(new XElement("TOTALVALUE", orderTotalValue));
                        xOrder.Add(GetConsignments(orderNumber, OrderList));
                        xOrders.Add(xOrder);
                    }

                    previousOrderNumber = orderRecord.OrderNo;
                    previousOrderConsignmentNumber = orderRecord.ConsignmentNo;
                    previousOrderParcelCode = orderRecord.ParcelCode;

                }


                // Build final XML Document
                xDocument.Declaration = xDeclaration;
                xDocument.Add(xProcessing);
                xDocument.Add(xComment);
                xDocument.Add(xOrders);
                xDocument.Save("C:\\Users\\Stephen P Smith\\source\\repos\\Tech Tests\\IT-Talent\\IT-Talent-001\\XMLOutput\\IT-Talent-001.xml");

            }
            else
            {
                Console.WriteLine("File Type is : " + fileExtension.ToUpper() + " and IS NOT valid.");
            }


            Console.WriteLine("Completed processing of file [{0}].", path);
        }

        public static decimal CalculateOrderWeight(string orderNumber, List<Order> OrderList)
        {
            decimal orderTotalWeight = 0;

            // Iterate through the list to extracte the order weight
            foreach (Order orderRecord in OrderList)
            {
                if (orderNumber == orderRecord.OrderNo)
                {
                    orderTotalWeight = orderTotalWeight + Convert.ToDecimal(orderRecord.ItemWeight);
                }
            }

            return orderTotalWeight;
        }

        public static decimal CalculateOrderValue(string orderNumber, List<Order> OrderList)
        {

            decimal orderTotalValue = 0;
            decimal exchangeRate = 0.89M;

            // Iterate throught the order list to extract the order value
            foreach (Order orderRecord in OrderList)
            {

                if (orderNumber == orderRecord.OrderNo)
                {
                    // If the currency is not GBP we shall assume that it is in Euros with an exchange rate of €1.00 = £0.89
                    // In a real world production application I would be using the XE Currency Data API to retriever a live exchange rate
                    if (orderRecord.ItemCurrency != "")
                    {
                        orderTotalValue = orderTotalValue + (Convert.ToDecimal(orderRecord.ItemValue) * exchangeRate);
                    }
                    else
                    {
                        orderTotalValue = orderTotalValue + Convert.ToDecimal(orderRecord.ItemValue);
                    }

                }

            }

            return orderTotalValue;
        }

        public static XElement GetConsignments(string orderNumber, List<Order> OrderList)
        {

            string orderConsignmentNumber = "";
            string previousOrderConsignmentNumber = "";
            XElement xConsignments = new XElement("CONSIGNMENTS");
            XElement xConsignment;

            // Iterate through the order list and order to extract the consignments
            foreach (Order orderRecord in OrderList)
            {
                if (orderNumber == orderRecord.OrderNo)
                {

                    xConsignments = new XElement("CONSIGNMENTS");
                    
                    // Construct CONSIGNMENTS XML Node
                    orderConsignmentNumber = orderRecord.ConsignmentNo;
                    if (orderConsignmentNumber != previousOrderConsignmentNumber)
                    {
                        
                        xConsignment = new XElement("CONSIGNMENT", orderConsignmentNumber);
                        xConsignment.Add(GetParcels);
                        xConsignments.Add(xConsignment);
                    }
                }

            }

            return xConsignments;

        }

        public static XElement GetParcels(string orderNumber, string consignmentNumber, List<Order> Orderlist)
        {

            XElement xParcels = new XElement("PARCELS");
            XElement xParcel;

            // Iterate through the order list to extract the parcels for a consignment number
            foreach (Order orderRecord in Orderlist)
            {

            }

            return xParcels;

        }
    }
}
