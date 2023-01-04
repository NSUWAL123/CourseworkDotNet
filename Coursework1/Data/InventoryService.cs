using Coursework1.Pages;

using System.Text.Json;

namespace Coursework1.Data;

public static class InventoryService
{
    //Saves all items to the file.
    private static void SaveAll(List<InventoryItem> inventory)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string inventoryFilePath = Utils.GetInventoryRecordFilePath(); //path of file where inventory items record are stored

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(inventory);
        File.WriteAllText(inventoryFilePath, json);
    }

    //Returns list of items currently saved in the inventory file.
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

    //Creates a new item.
    public static List<InventoryItem> Create(string ItemName, int Quantity)
    {
        List<InventoryItem> inventory = GetAll();

        if (ItemName == null) //if no item name is entered
        {
            throw new Exception("Please enter item name.");
        }
        else if (Quantity < 1) //restrict from creating item if quantity entered is not more than 0
        {
            throw new Exception("Quantity should be 1 or more.");
        }

        bool itemExists = inventory.Any(x => x.ItemName.ToLower() == ItemName.ToLower());

        if (itemExists) //updates the value of the item if the item present in the inventory is again tried to be created
        {
            InventoryItem inventoryUpdate = inventory.FirstOrDefault(x => x.ItemName.ToLower() == ItemName.ToLower());
            inventoryUpdate.Quantity += Quantity; //updating quantity of item in inventory
        }
        else //executes if item name is unique than that of the already present items
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

    //Deletes an item from the inventory based on item id.
    public static List<InventoryItem> Delete(Guid id)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem inventoryToBeDeleted = inventory.FirstOrDefault(x => x.Id == id);

        if (inventoryToBeDeleted == null)
        {
            throw new Exception("Item not found.");
        }

        inventory.Remove(inventoryToBeDeleted);
        SaveAll(inventory);
        return inventory;
    }

    //Updates the quantity of the item.
    public static List<InventoryItem> Update(Guid id, string ItemName, int Quantity)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem inventoryToBeUpdated = inventory.FirstOrDefault(x => x.Id == id);

        if (ItemName == null)
        {
            throw new Exception("Please enter item name.");
        }
        else if (Quantity < 1)
        {
            throw new Exception("Quantity should be 1 or more.");
        }

        if (inventoryToBeUpdated == null)
        {
            throw new Exception("Item not found.");
        }

        inventoryToBeUpdated.ItemName = ItemName;
        inventoryToBeUpdated.Quantity = Quantity;
        SaveAll(inventory);
        return inventory;
    }

    //Updates the last taken-out date of the item.
    public static List<InventoryItem> UpdateByName(string ItemName, DateTime LastTakenOutOn)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem inventoryToBeUpdated = inventory.FirstOrDefault(x => x.ItemName == ItemName);

        if (inventoryToBeUpdated == null)
        {
            throw new Exception("Item not found.");
        }

        inventoryToBeUpdated.LastTakenOutOn = LastTakenOutOn;
        SaveAll(inventory);
        return inventory;
    }

    //Returns all the requested items by the staff for retrieval.
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

    //Creates a new request for item retrieval.
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

    //Saves all the requested creation and deletion of the requested item.
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

    //Updates the quantity in inventory and approved status of the item.
    public static List<ItemRequest> EditRequestedItem(Guid id, string UserName, string ItemName, int Quantity, bool approvalStatus, DateTime ApprovedAt, string Admin)
    {
        List<ItemRequest> inventory = GetAllRequestedItem();
        ItemRequest inventoryUpdate = inventory.FirstOrDefault(x => x.Id == id);

        List<InventoryItem> allItems = GetAll();
        InventoryItem item = allItems.FirstOrDefault(x => x.ItemName == ItemName);

        item.Quantity -= Quantity;
        if (inventoryUpdate == null)
        {
            throw new Exception("Item not found.");
        }

        inventoryUpdate.ItemName = ItemName;
        inventoryUpdate.ApprovalStatus = approvalStatus;
        inventoryUpdate.ApprovedAt = ApprovedAt;
        inventoryUpdate.Admin = Admin;

        SaveAll(allItems);
        SaveAllRequestedItem(inventory);
        return inventory;
    }

    //Deletes requested items from the file in the case of decline. 
    public static List<ItemRequest> RemoveRequestedItem(Guid id, string UserName, string ItemName, int Quantity, bool approvalStatus)
    {
        List<ItemRequest> inventory = GetAllRequestedItem();
        ItemRequest inventoryUpdate = inventory.FirstOrDefault(x => x.Id == id);

        if (inventoryUpdate == null)
        {
            throw new Exception("Item not found.");
        }

        inventory.Remove(inventoryUpdate);
        SaveAllRequestedItem(inventory);
        return inventory;
    }

    //Validates if the requested quantity is more than 0 and if the requested quantity is less than the current inventory stock for that item.
    public static bool IsValid(int Quantity, int _takenQuantity, string itemName)
    {
        if (Quantity < _takenQuantity)
        {
            throw new Exception("Only " + Quantity + " " + itemName + "'s are left in the inventory.");
        }
        else if (_takenQuantity < 1)
        {
            throw new Exception("You cannot take less than 1 items.");
        }

        return true;
    }

    //Checks stock before approval to make sure sufficient item is there to approve the request.
    public static bool StockCheckForApproval(int Quantity, string ItemName)
    {
        List<InventoryItem> inventory = GetAll();
        InventoryItem item = inventory.FirstOrDefault(x => x.ItemName == ItemName);

        if (item.Quantity < Quantity)
        {
            throw new Exception("Cannot approve the item because of less stock.");
        }
        return true;
    }
}
