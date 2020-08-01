using CatalogMicroservice.Model;
using System;
using System.Collections.Generic;

namespace CatalogMicroservice.Repository
{
    public interface ICatalogRepository
    {
        List<CatalogItem> GetCatalogItems();
        CatalogItem GetCatalogItem(Guid catalogItemId);
        void InsertCatalogItem(CatalogItem catalogItem);
        void UpdateCatalogItem(CatalogItem catalogItem);
        void DeleteCatalogItem(Guid catalogItemId);
    }
}
