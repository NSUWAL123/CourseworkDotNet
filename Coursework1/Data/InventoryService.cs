using Coursework1.Pages;

using System.Text.Json;

namespace Coursework1.Data;

public static class InventoryService
{
    private static void SaveAll(List<InventoryItem> inventory)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string inventoryFilePath = Utils.GetInventoryRecordFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(inventory);
        File.WriteAllText(inventoryFilePath, json);
    }

    public static List<InventoryItem> GetAll()
    {
        string inventoryFilePath = Utils.GetInventoryRecordFilePath();
        if (!File.Exists(inventoryFilePath))
        {
            return new List<InventoryItem>();
        }

        var json = File.ReadAllText(inventoryFilePath);
        return JsonSerializer.Deserialize<List<InventoryItem>>(json);
    }

    public static List<InventoryItem> Create(string ItemName, int Quantity)
    {
        List<InventoryItem> inventory = GetAll();

        if (ItemName == null)
        {
            throw new Exception("Please enter item name.");
        }
        else if (Quantity < 1)
        {
            throw new Exception("Quantity should be 1 or more.");
        }

        bool itemExists = inventory.Any(x => x.ItemName.ToLower() == ItemName.ToLower());

        if (itemExists)
        {
            InventoryItem inventoryUpdate = inventory.FirstOrDefault(x => x.ItemName.ToLower() == ItemName.ToLower());
            inventoryUpdate.Quantity += Quantity;
        }
        else
        {
            inventory.Add(new InventoryItem
            {
                ItemName = ItemName,
                Quantity = Quantity
            });

        }
        SaveAll(inventory);
        return inventory;

    }

    public static List<InventoryItem> Delete(Guid id)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem inventorys = inventory.FirstOrDefault(x => x.Id == id);

        if (inventorys == null)
        {
            throw new Exception("inventorys not found.");
        }

        inventory.Remove(inventorys);
        SaveAll(inventory);
        return inventory;
    }

    public static void DeleteByUserId()
    {
        string inventoryFilePath = Utils.GetInventoryRecordFilePath();
        if (File.Exists(inventoryFilePath))
        {
            File.Delete(inventoryFilePath);
        }
    }

    public static List<InventoryItem> Update(Guid id, string ItemName, int Quantity)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem inventoryUpdate = inventory.FirstOrDefault(x => x.Id == id);

        if (ItemName == null)
        {
            throw new Exception("Please enter item name.");
        }
        else if (Quantity < 1)
        {
            throw new Exception("Quantity should be 1 or more.");
        }

        if (inventoryUpdate == null)
        {
            throw new Exception("inventorys not found.");
        }

        inventoryUpdate.ItemName = ItemName;
        inventoryUpdate.Quantity = Quantity;
        SaveAll(inventory);
        return inventory;
    }

    public static List<InventoryItem> UpdateByName(string ItemName, DateTime LastTakenOutOn)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem inventoryUpdate = inventory.FirstOrDefault(x => x.ItemName == ItemName);

        if (inventoryUpdate == null)
        {
            throw new Exception("inventorys not found.");
        }

        inventoryUpdate.LastTakenOutOn = LastTakenOutOn;
        SaveAll(inventory);
        return inventory;
    }

    public static List<ItemRequest> GetAllRequestedItem()
    {
        string requestedItemFilePath = Utils.GetItemRequestFilePath();
        if (!File.Exists(requestedItemFilePath))
        {
            return new List<ItemRequest>();
        }

        var json = File.ReadAllText(requestedItemFilePath);
        return JsonSerializer.Deserialize<List<ItemRequest>>(json);
    }

    public static List<ItemRequest> CreateRequestItem(string UserName, string ItemName, int Quantity, bool approvalStatus)
    {
        List<ItemRequest> requestedItemList = GetAllRequestedItem();

        requestedItemList.Add(new ItemRequest
        {
            UserName = UserName,
            ItemName = ItemName,
            Quantity = Quantity,
            ApprovalStatus = approvalStatus,
        });

        SaveAllRequestedItem(requestedItemList);
        return requestedItemList;
    }

    private static void SaveAllRequestedItem(List<ItemRequest> requestedItemList)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string itemRequestFilePath = Utils.GetItemRequestFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(requestedItemList);
        File.WriteAllText(itemRequestFilePath, json);
    }

    public static List<ItemRequest> EditRequestedItem(Guid id, string UserName, string ItemName, int Quantity, bool approvalStatus, DateTime ApprovedAt, string Admin)
    {
        List<ItemRequest> inventory = GetAllRequestedItem();
        ItemRequest inventoryUpdate = inventory.FirstOrDefault(x => x.Id == id);

        List<InventoryItem> allItems = GetAll();
        InventoryItem item = allItems.FirstOrDefault(x => x.ItemName == ItemName);

        item.Quantity -= Quantity;
        if (inventoryUpdate == null)
        {
            throw new Exception("inventorys not found.");
        }

        inventoryUpdate.ItemName = ItemName;
        inventoryUpdate.ApprovalStatus = approvalStatus;
        inventoryUpdate.ApprovedAt = ApprovedAt;
        inventoryUpdate.Admin = Admin;

        SaveAll(allItems);
        SaveAllRequestedItem(inventory);
        return inventory;
    }

    public static List<ItemRequest> RemoveRequestedItem(Guid id, string UserName, string ItemName, int Quantity, bool approvalStatus)
    {
        List<ItemRequest> inventory = GetAllRequestedItem();
        ItemRequest inventoryUpdate = inventory.FirstOrDefault(x => x.Id == id);

        if (inventoryUpdate == null)
        {
            throw new Exception("inventorys not found.");
        }

        inventory.Remove(inventoryUpdate);
        SaveAllRequestedItem(inventory);
        return inventory;
    }

    public static bool IsValid(int Quantity, int _takenQuantity, string itemName)
    {
        if (Quantity < _takenQuantity)
        {
            throw new Exception("Only " + Quantity + " " + itemName + "'s are left in the inventory.");
        }
        else if (_takenQuantity < 1)
        {
            throw new Exception("You take less than 1 items.");
        }

        return true;
    }

    public static bool StockCheckForApproval(int Quantity, string ItemName)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem item = inventory.FirstOrDefault(x => x.ItemName == ItemName);

        if (item.Quantity < Quantity)
        {
            throw new Exception("You take less than 1 items.");
        }
        return true;
    }
}
