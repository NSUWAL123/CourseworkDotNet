using System;
using System.ComponentModel.DataAnnotations;

namespace Coursework1.Data
{
    public class InventoryItem
    {
        public Guid Id { get; set; } = Guid.NewGuid(); //Uniquely identifies each item in inventory.
        [Required(ErrorMessage = "Please provide the Item name.")] 
        public string ItemName { get; set; } //Name of the item in the inventory.
        public int Quantity { get; set; } //Quantity of the item currently present.
        public DateTime CreatedAt { get; set; } = DateTime.Now; //Date and Time of the item creation.
        public DateTime LastTakenOutOn { get; set; } //Date and Time of the item last taken out from the inventory.
    }


    public class ItemRequest 
    {
        public Guid Id { get; set; } = Guid.NewGuid(); //Uniquely identifies each request by the staff to take items out.
        public string UserName { get; set; } //Username of the user.
        public string ItemName { get; set; } //Name of the item in the inventory.
        public int Quantity { get; set; } //Quantity of the item currently present.
        public bool ApprovalStatus { get; set; } //Defines whether the requested item is approved or not.     
        public DateTime ApprovedAt { get; set; } = DateTime.Now; //Date and Time when the requested item was approved.
        public string Admin { get; set; } //Name of the admin who approved the requested item.
    }
}
