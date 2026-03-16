using System.Collections.Generic;
using STRELA_MED.Models;

namespace STRELA_MED.Services
{
    public class WarehouseService
    {
        private List<Medicine> _inventory = new List<Medicine>();

        public void AddStock(string name, int amount)
        {
            var item = _inventory.Find(m => m.Name == name);
            if (item != null) item.Quantity += amount;
            else _inventory.Add(new Medicine { Name = name, Quantity = amount });
        }

        public bool DecreaseStock(string name, int amount)
        {
            var item = _inventory.Find(m => m.Name == name);
            if (item != null && item.Quantity >= amount)
            {
                item.Quantity -= amount;
                return true;
            }
            return false;
        }
    }
}