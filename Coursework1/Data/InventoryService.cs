using Coursework1.Pages;
using System.Text.Json;

namespace Coursework1.Data;

public static class InventoryService
{
    private static void SaveAll( List<InventoryItem> inventory) //userId
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string inventoryFilePath = Utils.GetinventoryFilePath(); //userId

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(inventory);
        File.WriteAllText(inventoryFilePath, json);
    }

    public static List<InventoryItem> GetAll() //userId
    {
        string inventoryFilePath = Utils.GetinventoryFilePath();//userId
        if (!File.Exists(inventoryFilePath))
        {
            return new List<InventoryItem>();
        }

        var json = File.ReadAllText(inventoryFilePath);

        return JsonSerializer.Deserialize<List<InventoryItem>>(json);
    }

    public static List<InventoryItem> Create(string ItemName, int Quantity) //userId
    {
        List<InventoryItem> inventory = GetAll();//userId

        bool itemExists = inventory.Any(x => x.ItemName == ItemName);

        if (itemExists)
        {
            InventoryItem inventoryUpdate = inventory.FirstOrDefault(x => x.ItemName == ItemName);
            inventoryUpdate.Quantity += Quantity;
        }
        else { 
        inventory.Add(new InventoryItem
        {
            ItemName = ItemName,
            Quantity = Quantity
        });
        
        }
        SaveAll(inventory); //userId
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

    public static void DeleteByUserId()//userId
    {
        string inventoryFilePath = Utils.GetinventoryFilePath();//userId
        if (File.Exists(inventoryFilePath))
        {
            File.Delete(inventoryFilePath);
        }
    }

    public static List<InventoryItem> Update( Guid id, string ItemName, int Quantity)//userId
    {
        List<InventoryItem> inventory = GetAll();//userId
        InventoryItem inventoryUpdate = inventory.FirstOrDefault(x => x.Id == id);

        if (inventoryUpdate == null)
        {
            throw new Exception("inventorys not found.");
        }

        inventoryUpdate.ItemName = ItemName;
        inventoryUpdate.Quantity = Quantity;
        SaveAll(inventory);//userId
        return inventory;
    }


    public static List<ItemRequest> GetAllRequestedItem() //userId
    {
        string requestedItemFilePath = Utils.GetItemRequestFilePath();//userId
        if (!File.Exists(requestedItemFilePath))
        {
            return new List<ItemRequest>();
        }

        var json = File.ReadAllText(requestedItemFilePath);

        return JsonSerializer.Deserialize<List<ItemRequest>>(json);
    }

    public static List<ItemRequest> CreateRequestItem(string UserName, string ItemName, int Quantity, bool approvalStatus) //userId
    {
        List<ItemRequest> requestedItemList = GetAllRequestedItem();//userId

        //bool itemExists = inventory.Any(x => x.ItemName == ItemName);


        requestedItemList.Add(new ItemRequest
        {
            UserName = UserName,
            ItemName = ItemName,
            Quantity = Quantity,
            ApprovalStatus = approvalStatus,
        });

        SaveAllRequestedItem(requestedItemList); //userId
        return requestedItemList;
    }

    private static void SaveAllRequestedItem(List<ItemRequest> requestedItemList) //userId
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string itemRequestFilePath = Utils.GetItemRequestFilePath(); //userId

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(requestedItemList);
        File.WriteAllText(itemRequestFilePath, json);
    }


    public static List<ItemRequest> EditRequestedItem(Guid id, string UserName, string ItemName, int Quantity, bool approvalStatus)//userId
    {
        List<ItemRequest> inventory = GetAllRequestedItem();//userId
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

        SaveAll(allItems);
        SaveAllRequestedItem(inventory);//userId
        return inventory;
    }

    public static List<ItemRequest> RemoveRequestedItem(Guid id, string UserName, string ItemName, int Quantity, bool approvalStatus)//userId
    {
        List<ItemRequest> inventory = GetAllRequestedItem();//userId
        ItemRequest inventoryUpdate = inventory.FirstOrDefault(x => x.Id == id);

        if (inventoryUpdate == null)
        {
            throw new Exception("inventorys not found.");
        }

        inventoryUpdate.ApprovalStatus = approvalStatus;

        SaveAllRequestedItem(inventory);//userId
        return inventory;
    }
}
