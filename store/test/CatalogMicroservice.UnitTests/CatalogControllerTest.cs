using CatalogMicroservice.Controllers;
using CatalogMicroservice.Model;
using CatalogMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CatalogMicroservice.UnitTests
{
    public class CatalogControllerTest
    {
        private readonly CatalogController _controller;
        private readonly List<CatalogItem> _items = new List<CatalogItem>
        {
            new CatalogItem { Id = new Guid("ce2dbb82-6689-487b-9691-0a05ebabce4a"), Name = "Samsung Galaxy S10", Description = "Samsung Galaxy S10 mobile phone", Price= 1000 },
            new CatalogItem { Id = new Guid("13b87ba8-f542-441c-bc2c-db32fb01ec3f"), Name = "Samsung Galaxy S9", Description = "Samsung Galaxy S9 mobile phone", Price= 700 }
        };

        public CatalogControllerTest()
        {
            var mockRepo = new Mock<ICatalogRepository>();
            mockRepo.Setup(repo => repo.GetCatalogItems()).Returns(_items);
            mockRepo.Setup(repo => repo.GetCatalogItem(It.IsAny<Guid>())).Returns<Guid>(id => _items.FirstOrDefault(i => i.Id == id));
            mockRepo.Setup(repo => repo.InsertCatalogItem(It.IsAny<CatalogItem>())).Callback<CatalogItem>(i => _items.Add(i));
            mockRepo.Setup(repo => repo.UpdateCatalogItem(It.IsAny<CatalogItem>())).Callback<CatalogItem>(i =>
            {
                var item = _items.FirstOrDefault(i => i.Id == i.Id);
                if (item != null)
                {
                    item.Name = i.Name;
                    item.Description = i.Description;
                    item.Price = i.Price;
                }
            });
            mockRepo.Setup(repo => repo.DeleteCatalogItem(It.IsAny<Guid>())).Callback<Guid>(id => _items.RemoveAll(i => i.Id == id));
            _controller = new CatalogController(mockRepo.Object);
        }

        [Fact]
        public void GetCatalogItemsTest()
        {
            var okObjectResult = _controller.Get();
            var okResult = Assert.IsType<OkObjectResult>(okObjectResult.Result);
            var items = Assert.IsType<List<CatalogItem>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public void GetCatalogItemTest()
        {
            var id = new Guid("ce2dbb82-6689-487b-9691-0a05ebabce4a");
            var okObjectResult = _controller.Get(id);
            var okResult = Assert.IsType<OkObjectResult>(okObjectResult.Result);
            var item = Assert.IsType<CatalogItem>(okResult.Value);
            Assert.Equal(id, item.Id);
        }

        [Fact]
        public void InsertCatalogItemTest()
        {
            var createdResponse = _controller.Post(new CatalogItem { Id = new Guid("d378ff93-dc4b-4bf6-8756-58b6901cd47b"), Name = "iPhone X", Description = "iPhone X mobile phone", Price = 1000 });
            var response = Assert.IsType<CreatedAtActionResult>(createdResponse);
            var item = Assert.IsType<CatalogItem>(response.Value);
            Assert.Equal("iPhone X", item.Name);
        }

        [Fact]
        public void UpdateCatalogItemTest()
        {
            var id = new Guid("ce2dbb82-6689-487b-9691-0a05ebabce4a");
            var okObjectResult = _controller.Put(new CatalogItem { Id = id, Name = "Samsung Galaxy S10+", Description = "Samsung Galaxy S10+ mobile phone", Price = 1100 });
            Assert.IsType<OkResult>(okObjectResult);
            var item = _items.First(i => i.Id == id);
            Assert.Equal("Samsung Galaxy S10+", item.Name);
            okObjectResult = _controller.Put(null);
            Assert.IsType<NoContentResult>(okObjectResult);
        }

        [Fact]
        public void DeleteCatalogItemTest()
        {
            var id = new Guid("ce2dbb82-6689-487b-9691-0a05ebabce4a");
            var item = _items.FirstOrDefault(i => i.Id == id);
            Assert.NotNull(item);
            var okObjectResult = _controller.Delete(id);
            Assert.IsType<OkResult>(okObjectResult);
            item = _items.FirstOrDefault(i => i.Id == id);
            Assert.Null(item);
        }
    }
}
