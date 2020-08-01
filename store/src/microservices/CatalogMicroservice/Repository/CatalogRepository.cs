﻿using CatalogMicroservice.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatalogMicroservice.Repository
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly IMongoCollection<CatalogItem> _col;

        public CatalogRepository(IMongoDatabase db)
        {
            _col = db.GetCollection<CatalogItem>(CatalogItem.DocumentName);
        }

        public List<CatalogItem> GetCatalogItems() =>
            _col.Find(FilterDefinition<CatalogItem>.Empty).ToList();

        public CatalogItem GetCatalogItem(Guid catalogItemId) =>
            _col.Find(c => c.Id == catalogItemId).FirstOrDefault();

        public void InsertCatalogItem(CatalogItem catalogItem) =>
            _col.InsertOne(catalogItem);

        public void UpdateCatalogItem(CatalogItem catalogItem) =>
            _col.UpdateOne(c => c.Id == catalogItem.Id, Builders<CatalogItem>.Update
                .Set(c => c.Name, catalogItem.Name)
                .Set(c => c.Description, catalogItem.Description)
                .Set(c => c.Price, catalogItem.Price));

        public void DeleteCatalogItem(Guid catalogItemId) =>
            _col.DeleteOne(c => c.Id == catalogItemId);
    }
}
