using System;
using System.ComponentModel.DataAnnotations;

namespace Coursework1.Data
{
    public class InventoryItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please provide the Item name.")]
        public string ItemName { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }


    public class ItemRequest 
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public bool ApprovalStatus { get; set; }
    }
}
