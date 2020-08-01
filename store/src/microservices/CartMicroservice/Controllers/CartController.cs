using CartMicroservice.Model;
using CartMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CartMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        // GET: api/<CartController>
        [HttpGet]
        public ActionResult<IEnumerable<CartItem>> Get([FromQuery(Name = "u")] Guid userId)
        {
            var cartItems = _cartRepository.GetCartItems(userId);
            return Ok(cartItems);
        }

        // POST api/<CartController>
        [HttpPost]
        public ActionResult Post([FromQuery(Name = "u")] Guid userId, [FromBody] CartItem cartItem)
        {
            _cartRepository.InsertCartItem(userId, cartItem);
            return Ok();
        }

        // PUT api/<CartController>
        [HttpPut]
        public ActionResult Put([FromQuery(Name = "u")] Guid userId, [FromBody] CartItem cartItem)
        {
            _cartRepository.UpdateCartItem(userId, cartItem);
            return Ok();
        }

        // DELETE api/<CartController>
        [HttpDelete]
        public ActionResult Delete([FromQuery(Name = "u")] Guid userId, [FromQuery(Name = "ci")] Guid catalogItemId)
        {
            _cartRepository.DeleteCartItem(userId, catalogItemId);
            return Ok();
        }
    }
}
