using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseInventory
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"[E] #{Id} {Name} ({Brand}) - Qty: {Quantity}, Warranty: {WarrantyMonths}m";
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() => $"[G] #{Id} {Name} - Qty: {Quantity}, Expires: {ExpiryDate:d}";
    }

    public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
    public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
    public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => _items.Values.ToList();

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");

            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Router", 15, "Starlink", 14));
            _electronics.AddItem(new ElectronicItem(4, "Smartwatch", 25, "Iphone", 6));
            _electronics.AddItem(new ElectronicItem(5, "Televisions", 12, "Sony", 10));

            _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 40, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(102, "Milk 1L", 20, DateTime.Today.AddDays(10)));
            _groceries.AddItem(new GroceryItem(103, "Eggs Tray", 30, DateTime.Today.AddDays(11)));
            _groceries.AddItem(new GroceryItem(104, "Salt 1Kg", 5, DateTime.Today.AddDays(6)));
            _groceries.AddItem(new GroceryItem(105, "Spaghetti", 10, DateTime.Today.AddDays(8)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine("  " + item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                int newQty = item.Quantity + quantity;
                if (newQty < 0) throw new InvalidQuantityException("Resulting quantity cannot be negative.");
                repo.UpdateQuantity(id, newQty);
                Console.WriteLine($"Stock updated for #{id}. New Qty: {newQty}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IncreaseStock Error] {ex.Message}");
            }
        }

        public void SetQuantity<T>(InventoryRepository<T> repo, int id, int newQuantity) where T : IInventoryItem
        {
            try
            {
                repo.UpdateQuantity(id, newQuantity);
                Console.WriteLine($"Quantity set for #{id}. New Qty: {newQuantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SetQuantity Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item #{id} removed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RemoveItem Error] {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Warehouse Inventory App");
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("\nGrocery Items:");
            manager.PrintAllItems(manager.GroceriesRepo);
            Console.WriteLine("\nElectronic Items:");
            manager.PrintAllItems(manager.ElectronicsRepo);

            Console.WriteLine("\nCustom Exceptions: ");

            try
            {
                manager.GroceriesRepo.AddItem(new GroceryItem(101, "Rice 5kg (Duplicate)", 5, DateTime.Today.AddMonths(12)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Duplicate Add Error] {ex.Message}");
            }

            manager.RemoveItemById(manager.ElectronicsRepo, 999);
            manager.SetQuantity(manager.GroceriesRepo, 102, -10);
            manager.IncreaseStock(manager.ElectronicsRepo, 1, 5);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
