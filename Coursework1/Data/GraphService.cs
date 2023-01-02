using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coursework1.Data
{
    public static class GraphService
    {

        public static List<GraphItem> GetAll() //userId
        {
            string itemsTakenOutDataFilePath = Utils.ItemsTakenOutDataFilePath();//userId
            if (!File.Exists(itemsTakenOutDataFilePath))
            {
                return new List<GraphItem>();
            }

            var json = File.ReadAllText(itemsTakenOutDataFilePath);

            return JsonSerializer.Deserialize<List<GraphItem>>(json);
        }


        public static List<GraphItem> Create(string ItemName, int Quantity) //userId
        {
            List<GraphItem> takenOutItems = GetAll();//userId

            bool itemExists = takenOutItems.Any(x => x.ItemName == ItemName);

            if (itemExists)
            {
                GraphItem takenOutItemsUpdate = takenOutItems.FirstOrDefault(x => x.ItemName == ItemName);
                takenOutItemsUpdate.Quantity += Quantity;
            }
            else
            {
                takenOutItems.Add(new GraphItem
                {
                    ItemName = ItemName,
                    Quantity = Quantity
                });

            }
            SaveAll(takenOutItems); //userId
            return takenOutItems;

        }

        private static void SaveAll(List<GraphItem> inventory) //userId
        {
            string appDataDirectoryPath = Utils.GetAppDirectoryPath();
            string itemsTakenOutDataFilePath = Utils.ItemsTakenOutDataFilePath(); //userId

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            var json = JsonSerializer.Serialize(inventory);
            File.WriteAllText(itemsTakenOutDataFilePath, json);
        }
    }
}
