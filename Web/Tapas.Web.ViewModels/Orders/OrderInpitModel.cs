namespace Tapas.Web.ViewModels.Orders
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class OrderInpitModel
    {
        public OrderInpitModel()
        {
            this.Cutlery = this.CreateCutleryList();
        }

        public string AddInfoOrder { get; set; }

        public string Address { get; set; }

        public string Username { get; set; }

        public string Phone { get; set; }

        public bool TakeAway { get; set; }

        public string Cart { get; set; }

        public int CutleryCount { get; set; }

        public List<SelectListItem> Cutlery { get; set; }

        private List<SelectListItem> CreateCutleryList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = "0", Selected = true,  Text = "Не желая прибори" },
                new SelectListItem() { Value = "1", Selected = false,  Text = "1 комплект прибори" },
                new SelectListItem() { Value = "2", Selected = false,  Text = "2 комплекта прибори" },
                new SelectListItem() { Value = "3", Selected = false,  Text = "3 комплекта прибори" },
                new SelectListItem() { Value = "4", Selected = false,  Text = "4 комплекта прибори" },
                new SelectListItem() { Value = "5", Selected = false,  Text = "5 комплекта прибори" },
            };
        }
    }
}
