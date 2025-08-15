using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryLogger
{
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item) => _log.Add(item);

        public List<T> GetAll() => new(_log);

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_log, options);
                using var writer = new StreamWriter(_filePath);
                writer.Write(json);
                Console.WriteLine($"Saved {_log.Count} item(s) to {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveToFile Error] {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"No existing file at {_filePath}. Starting with empty log.");
                    return;
                }

                using var reader = new StreamReader(_filePath);
                string json = reader.ReadToEnd();
                var list = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                _log.Clear();
                _log.AddRange(list);
                Console.WriteLine($"Loaded {_log.Count} item(s) from {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadFromFile Error] {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Printer Paper (A4)", 50, DateTime.Now));
            _logger.Add(new InventoryItem(2, "HDMI Cable", 15, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Ethernet Cable (Cat6)", 30, DateTime.Now));
            _logger.Add(new InventoryItem(4, "USB Flash Drive 32GB", 22, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Wireless Mouse", 12, DateTime.Now));
            _logger.Add(new InventoryItem(6, "Laptop Stand", 8, DateTime.Now));
            _logger.Add(new InventoryItem(7, "3 Pin Adaptor", 14, DateTime.Now));
        }

        public void SaveData() => _logger.SaveToFile();
        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            Console.WriteLine("All Items:");
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"  #{item.Id} {item.Name} - Qty: {item.Quantity}, Added: {item.DateAdded:g}");
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Inventory Capture");
            string path = "inventory.json";

            // First session: seed & save
            var app = new InventoryApp(path);
            app.SeedSampleData();
            app.SaveData();

            // Simulate a new session: new app instance, then load & print
            Console.WriteLine("\nNew Session");
            var app2 = new InventoryApp(path);
            app2.LoadData();
            app2.PrintAllItems();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
