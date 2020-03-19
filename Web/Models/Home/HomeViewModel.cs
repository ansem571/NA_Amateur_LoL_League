using Domain.Views;
using Web.Models.ManageViewModels;

namespace Web.Models.Home
{
    public class HomeViewModel
    {
        public IndexViewModel IndexViewModel { get; set; }
        public SeasonInfoViewPartial SeasonInfo { get; set; }
    }
}
