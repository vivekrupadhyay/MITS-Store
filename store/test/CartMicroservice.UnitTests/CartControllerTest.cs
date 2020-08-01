using CartMicroservice.Controllers;
using CartMicroservice.Model;
using CartMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CartMicroservice.UnitTests
{
    public class CartControllerTest
    {
        private readonly CartController _controller;
        private readonly List<Cart> _carts = new List<Cart>
        {
            new Cart
            {
                Id = new Guid("6bd19484-2237-4319-a690-510e4f2258d8"),
                UserId = new Guid("f5dd5ea6-ac9c-4dc3-86c3-090853945167"),
                CartItems = new List<CartItem>
                {
                    new CartItem{ CatalogItemId= new Guid("ce2dbb82-6689-487b-9691-0a05ebabce4a"), CatalogItemName = "Samsung Galaxy S10", CatalogItemPrice = 1000, Quantity = 1},
                    new CartItem{ CatalogItemId= new Guid("13b87ba8-f542-441c-bc2c-db32fb01ec3f"), CatalogItemName = "Samsung Galaxy S9", CatalogItemPrice = 700, Quantity = 1}
                }
            }
        };

        public CartControllerTest()
        {
            var mockRepo = new Mock<ICartRepository>();
            mockRepo.Setup(repo => repo.GetCartItems(It.IsAny<Guid>())).Returns<Guid>(userId =>
            {
                var cart = _carts.FirstOrDefault(c => c.UserId == userId);
                if (cart != null)
                {
                    return cart.CartItems;
                }
                return new List<CartItem>();
            });
            mockRepo.Setup(repo => repo.InsertCartItem(It.IsAny<Guid>(), It.IsAny<CartItem>())).Callback<Guid, CartItem>((userId, cartItem) =>
            {
                var cart = _carts.FirstOrDefault(c => c.UserId == userId);
                if (cart != null)
                {
                    var ci = cart.CartItems.FirstOrDefault(i => i.CatalogItemId == cartItem.CatalogItemId);
                    if (ci == null)
                    {
                        cart.CartItems.Add(cartItem);
                    }
                    else
                    {
                        ci.Quantity++;
                    }
                }
            });
            mockRepo.Setup(repo => repo.UpdateCartItem(It.IsAny<Guid>(), It.IsAny<CartItem>())).Callback<Guid, CartItem>((userId, cartItem) =>
            {
                var cart = _carts.FirstOrDefault(c => c.UserId == userId);
                if (cart != null)
                {
                    var ci = cart.CartItems.FirstOrDefault(i => i.CatalogItemId == cartItem.CatalogItemId);
                    if (ci != null)
                    {
                        ci.CatalogItemName = cartItem.CatalogItemName;
                        ci.CatalogItemPrice = cartItem.CatalogItemPrice;
                        ci.Quantity = cartItem.Quantity;
                    }
                }
            });
            mockRepo.Setup(repo => repo.DeleteCartItem(It.IsAny<Guid>(), It.IsAny<Guid>())).Callback<Guid, Guid>((userId, cartItemId) =>
            {
                var cart = _carts.FirstOrDefault(c => c.UserId == userId);
                if (cart != null)
                {
                    cart.CartItems.RemoveAll(i => i.CatalogItemId == cartItemId);
                }
            });
            _controller = new CartController(mockRepo.Object);
        }

        [Fact]
        public void GetCartItemsTest()
        {
            var userId = new Guid("f5dd5ea6-ac9c-4dc3-86c3-090853945167");
            var okObjectResult = _controller.Get(userId);
            var okResult = Assert.IsType<OkObjectResult>(okObjectResult.Result);
            var items = Assert.IsType<List<CartItem>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public void InsertCartItemTest()
        {
            var userId = new Guid("f5dd5ea6-ac9c-4dc3-86c3-090853945167");
            var cartItem = new CartItem { CatalogItemId = new Guid("d378ff93-dc4b-4bf6-8756-58b6901cd47b"), CatalogItemName = "iPhone X", CatalogItemPrice = 1000, Quantity = 1 };
            var okResult = _controller.Post(userId, cartItem);
            Assert.IsType<OkResult>(okResult);
            var cart = _carts.First(c => c.UserId == userId);
            var ci = cart.CartItems.FirstOrDefault(i => i.CatalogItemId == cartItem.CatalogItemId);
            Assert.NotNull(ci);
            Assert.Equal(1, ci.Quantity);
            okResult = _controller.Post(userId, cartItem);
            Assert.IsType<OkResult>(okResult);
            Assert.Equal(2, ci.Quantity);
        }

        [Fact]
        public void UpdateCartItemTest()
        {
            var userId = new Guid("f5dd5ea6-ac9c-4dc3-86c3-090853945167");
            var cartItem = new CartItem { CatalogItemId = new Guid("ce2dbb82-6689-487b-9691-0a05ebabce4a"), CatalogItemName = "Samsung Galaxy S10", CatalogItemPrice = 1000, Quantity = 2 };
            var okResult = _controller.Put(userId, cartItem);
            Assert.IsType<OkResult>(okResult);
            var cart = _carts.First(c => c.UserId == userId);
            var ci = cart.CartItems.First(i => i.CatalogItemId == cartItem.CatalogItemId);
            Assert.Equal(2, ci.Quantity);
        }

        [Fact]
        public void DeleteCartItemTest()
        {
            var userId = new Guid("f5dd5ea6-ac9c-4dc3-86c3-090853945167");
            var catlogItemId = new Guid("ce2dbb82-6689-487b-9691-0a05ebabce4a");
            var cart = _carts.First(c => c.UserId == userId);
            var ci = cart.CartItems.FirstOrDefault(i => i.CatalogItemId == catlogItemId);
            Assert.NotNull(ci);
            var okResult = _controller.Delete(userId, catlogItemId);
            Assert.IsType<OkResult>(okResult);
            ci = cart.CartItems.FirstOrDefault(i => i.CatalogItemId == catlogItemId);
            Assert.Null(ci);
        }
    }
}
