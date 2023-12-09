using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace inventorymanagement3
{
    public class InventoryItem
    {
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public double ManufacturingCost { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserIdentifier { get; set; }
    }

    public static class InventoryFileHandler
    {
        private static string filePath;  // Use a static variable to store the file path

        // Property to set the file path
        public static string FilePath
        {
            get => filePath;
            set
            {
                // Ensure the directory exists before writing to the file
                string directoryPath = Path.GetDirectoryName(value);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                filePath = value;
            }
        }

        public static void SaveInventoryItems(List<InventoryItem> inventoryItems, string userIdentifier)
        {
            // Ensure the directory exists before writing to the file
            string directoryPath = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Read existing items from the file
            List<InventoryItem> existingItems = new List<InventoryItem>();
            if (File.Exists(FilePath))
            {
                string existingJson = File.ReadAllText(FilePath);
                existingItems = JsonConvert.DeserializeObject<List<InventoryItem>>(existingJson);
            }

            // Combine existing items and new items
            existingItems.AddRange(inventoryItems);

            // Filter items for the current user
            var userItems = existingItems.FindAll(item => item.UserIdentifier == userIdentifier);

            // Serialize the combined list of items to JSON
            string json = JsonConvert.SerializeObject(userItems, Formatting.Indented);

            // Write JSON to the file
            File.WriteAllText(FilePath, json);
        }

        public static List<InventoryItem> LoadInventoryItems(string userIdentifier)
        {
            // Ensure the file exists before attempting to read from it
            if (File.Exists(FilePath))
            {
                // Read the JSON from the file
                string json = File.ReadAllText(FilePath);

                // Deserialize the JSON to a list of InventoryItem objects
                List<InventoryItem> loadedItems = JsonConvert.DeserializeObject<List<InventoryItem>>(json);

                // Filter items for the current user
                return loadedItems.FindAll(item => item.UserIdentifier == userIdentifier);
            }

            // Return an empty list if the file doesn't exist
            return new List<InventoryItem>();
        }
    }
}
