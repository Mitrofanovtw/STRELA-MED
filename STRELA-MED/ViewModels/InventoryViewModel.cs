using STRELA_MED.Services;

namespace STRELA_MED.ViewModels
{
    public class InventoryViewModel
    {
        private User _currentUser;

        public InventoryViewModel(User user)
        {
            _currentUser = user;

        }
    }
}