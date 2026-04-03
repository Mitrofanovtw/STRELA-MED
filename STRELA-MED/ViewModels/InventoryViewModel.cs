using STRELA_MED.Services;
using STRELA_MED.Models;

namespace STRELA_MED.ViewModels
{
    public class InventoryViewModel
    {
        private Employee _currentUser;

        public InventoryViewModel(Employee user)
        {
            _currentUser = user;

        }
    }
}