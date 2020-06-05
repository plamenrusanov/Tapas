﻿namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Extras;

    public interface IExtrasService
    {
        ICollection<ExtraCartItemModel> All();

        Create Create();

        Task Create(Create model);

        EditModel Edit(int id);

        Task Edit(EditModel model);

        Task Delete(int id);

        Task Activate(int id);
    }
}
