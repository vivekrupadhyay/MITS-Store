using IdentityMicroservice.Controllers;
using IdentityMicroservice.Model;
using IdentityMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Middleware;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IdentityMicroservice.UnitTests
{
    public class IdentityControllerTest
    {
        private readonly IdentityController _controller;
        private readonly List<User> _users = new List<User>
        {
            new User{ Id = new Guid("b4f431d7-2653-4ec9-a34c-d4035e74c663"), Email= "user@store.com", Password = "tabSdQLDL9r29Mek9PvWnw9kM61CT3klw0pOZBQhAskqW11/4zpEpA==", Salt = "BCHatrzmOgciBaIW/DjLgw/lCCbBNNWxGvL1C25mZHaxgTBAfolOVA==", IsAdmin= false},
            new User{ Id = new Guid("0228f78e-1e6e-4929-9f2d-3986403ca84f"), Email= "admin@store.com", Password = "tabSdQLDL9r29Mek9PvWnw9kM61CT3klw0pOZBQhAskqW11/4zpEpA==", Salt = "BCHatrzmOgciBaIW/DjLgw/lCCbBNNWxGvL1C25mZHaxgTBAfolOVA==", IsAdmin= true}
        };

        public IdentityControllerTest()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.InsertUser(It.IsAny<User>())).Callback<User>(u => _users.Add(u));
            mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<string>(email => _users.FirstOrDefault(u => u.Email == email));
            var mockJwtBuilder = new Mock<IJwtBuilder>();
            mockJwtBuilder.Setup(repo => repo.GetToken(It.IsAny<Guid>())).Returns<Guid>(userId =>
            {
                if (userId == new Guid("b4f431d7-2653-4ec9-a34c-d4035e74c663"))
                {
                    return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJkZjFlYjFmYi01NjBlLTRjMGUtYmJiMC01OTFhNWQ4NWMzN2EiLCJleHAiOjE1OTMxODQxMDV9.k2t0qhvq6XMJAPA32xFH2hC6BY_6PC9jYay9RKEGcws";
                }
                else if (userId == new Guid("0228f78e-1e6e-4929-9f2d-3986403ca84f"))
                {
                    return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJlM2YxZjk0OS0yYzYwLTQwMmMtYmUwMi1lMzVkMTQwMDQwYTEiLCJleHAiOjE1OTMxODQxMzN9.3dhkOAY0duDSDYX5m9GHaA4qezHSrtnZvt436cPD3LE";
                }
                return string.Empty;
            });
            mockJwtBuilder.Setup(repo => repo.ValidateToken(It.IsAny<string>())).Returns<string>(token =>
            {
                if (token == "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJkZjFlYjFmYi01NjBlLTRjMGUtYmJiMC01OTFhNWQ4NWMzN2EiLCJleHAiOjE1OTMxODQxMDV9.k2t0qhvq6XMJAPA32xFH2hC6BY_6PC9jYay9RKEGcws")
                {
                    return new Guid("b4f431d7-2653-4ec9-a34c-d4035e74c663");
                }
                else if (token == "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJlM2YxZjk0OS0yYzYwLTQwMmMtYmUwMi1lMzVkMTQwMDQwYTEiLCJleHAiOjE1OTMxODQxMzN9.3dhkOAY0duDSDYX5m9GHaA4qezHSrtnZvt436cPD3LE")
                {
                    return new Guid("0228f78e-1e6e-4929-9f2d-3986403ca84f");
                }
                return Guid.Empty;
            });
            var mockEncryptor = new Mock<IEncryptor>();
            mockEncryptor.Setup(repo => repo.GetSalt(It.IsAny<string>())).Returns<string>(password =>
            {
                if (password == "pass")
                {
                    return "BCHatrzmOgciBaIW/DjLgw/lCCbBNNWxGvL1C25mZHaxgTBAfolOVA==";
                }
                return string.Empty;
            });
            mockEncryptor.Setup(repo => repo.GetHash(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((password, salt) =>
            {
                if (password == "pass" && salt == "BCHatrzmOgciBaIW/DjLgw/lCCbBNNWxGvL1C25mZHaxgTBAfolOVA==")
                {
                    return "tabSdQLDL9r29Mek9PvWnw9kM61CT3klw0pOZBQhAskqW11/4zpEpA==";
                }
                return string.Empty;
            });
            _controller = new IdentityController(mockRepo.Object, mockJwtBuilder.Object, mockEncryptor.Object);
        }

        [Fact]
        public void LoginTest()
        {
            var user = new User { Email = "user@store.com", Password = "pass" };
            var okObjectResult = _controller.Login(user);
            var okResult = Assert.IsType<OkObjectResult>(okObjectResult.Result);
            var token = Assert.IsType<string>(okResult.Value);
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJkZjFlYjFmYi01NjBlLTRjMGUtYmJiMC01OTFhNWQ4NWMzN2EiLCJleHAiOjE1OTMxODQxMDV9.k2t0qhvq6XMJAPA32xFH2hC6BY_6PC9jYay9RKEGcws", token);
            var user2 = new User { Email = "user2@store.com", Password = "pass" };
            okObjectResult = _controller.Login(user2);
            Assert.IsType<NotFoundObjectResult>(okObjectResult.Result);
            user.Password = "pass2";
            okObjectResult = _controller.Login(user);
            Assert.IsType<BadRequestObjectResult>(okObjectResult.Result);
            user.Password = "pass";
            okObjectResult = _controller.Login(user, "backend");
            Assert.IsType<BadRequestObjectResult>(okObjectResult.Result);
            var admin = new User { Email = "admin@store.com", Password = "pass" };
            okObjectResult = _controller.Login(admin, "backend");
            okResult = Assert.IsType<OkObjectResult>(okObjectResult.Result);
            token = Assert.IsType<string>(okResult.Value);
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJlM2YxZjk0OS0yYzYwLTQwMmMtYmUwMi1lMzVkMTQwMDQwYTEiLCJleHAiOjE1OTMxODQxMzN9.3dhkOAY0duDSDYX5m9GHaA4qezHSrtnZvt436cPD3LE", token);
        }

        [Fact]
        public void RegisterTest()
        {
            var user = new User { Email = "user2@store.com", Password = "pass" };
            var okResult = _controller.Register(user);
            Assert.IsType<OkResult>(okResult);
            okResult = _controller.Register(new User { Email = "user@store.com", Password = "pass" });
            Assert.IsType<BadRequestObjectResult>(okResult);
        }

        [Fact]
        public void ValidateTest()
        {
            var email = "user@store.com";
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJkZjFlYjFmYi01NjBlLTRjMGUtYmJiMC01OTFhNWQ4NWMzN2EiLCJleHAiOjE1OTMxODQxMDV9.k2t0qhvq6XMJAPA32xFH2hC6BY_6PC9jYay9RKEGcws";
            var okObjectResult = _controller.Validate(email, token);
            var okResult = Assert.IsType<OkObjectResult>(okObjectResult.Result);
            var userId = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(new Guid("b4f431d7-2653-4ec9-a34c-d4035e74c663"), userId);
            okObjectResult = _controller.Validate("user2@store.com", token);
            Assert.IsType<NotFoundObjectResult>(okObjectResult.Result);
            token = "fyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJkZjFlYjFmYi01NjBlLTRjMGUtYmJiMC01OTFhNWQ4NWMzN2EiLCJleHAiOjE1OTMxODQxMDV9.k2t0qhvq6XMJAPA32xFH2hC6BY_6PC9jYay9RKEGcws";
            okObjectResult = _controller.Validate(email, token);
            Assert.IsType<BadRequestObjectResult>(okObjectResult.Result);
        }
    }
}
