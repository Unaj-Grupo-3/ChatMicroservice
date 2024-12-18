using Application.Interface;
using Application.Models;
using Application.Reponsive;
using ChatMicroService.Controllers;
using Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Moq;
using Azure;
using System.Drawing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Domain.Entities;
using System;
using Newtonsoft.Json.Linq;

namespace Testchat1
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatServices> _mockChatServices;
        private readonly Mock<ITokenServices> _mockTokenServices;
        private readonly Mock<IMessageServices> _mockMessageServices;
        private readonly Mock<IUserApiServices> _mockUserApiServices;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            _mockChatServices = new Mock<IChatServices>();
            _mockTokenServices = new Mock<ITokenServices>();
            _mockMessageServices = new Mock<IMessageServices>();
            _mockUserApiServices = new Mock<IUserApiServices>();
            _mockConfiguration = new Mock<IConfiguration>();

            _controller = new ChatController(
                _mockChatServices.Object,
                _mockTokenServices.Object,
                _mockMessageServices.Object,
                _mockUserApiServices.Object,
                _mockConfiguration.Object
            );
        }

        private void SetupHttpContext(string apiKey = null, List<Claim> claims = null)
        {
            var mockHttpContext = new Mock<HttpContext>();
            var headers = new HeaderDictionary();

            if (!string.IsNullOrEmpty(apiKey))
            {
                headers["X-API-KEY"] = apiKey;
            }

            mockHttpContext.Setup(ctx => ctx.Request.Headers).Returns(headers);

            if (claims != null)
            {
                var identity = new ClaimsIdentity(claims);
                mockHttpContext.Setup(ctx => ctx.User).Returns(new ClaimsPrincipal(identity));
            }

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task CreateChat_ReturnsBadRequest_WhenRequestIsNull()
        {
            //Propósito: Verificar que el controlador responde con un código 400 Bad Request
            //cuando la solicitud enviada es nula.
            // Arrange
            _mockConfiguration.Setup(config => config.GetSection("ApiKey").Value).Returns("valid-api-key");
            SetupHttpContext("valid-api-key");

            // Act
            var result = await _controller.CreateChat(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CreateChat_ReturnsNotFound_WhenChatServiceReturnsNull()
        {
            //Propósito: Verificar que el controlador responde con un código 404 Not Found
            //si el servicio de chat devuelve un valor nulo.
            // Arrange
            var request = new ChatRequest();
            _mockConfiguration.Setup(config => config.GetSection("ApiKey").Value).Returns("valid-api-key");
            _mockChatServices.Setup(service => service.CreateChat(request)).ReturnsAsync((ChatResponse)null);

            SetupHttpContext("valid-api-key");

            // Act
            var result = await _controller.CreateChat(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(404, jsonResult.StatusCode);
        }

        [Fact]
        public async Task CreateChat_ReturnsUnauthorized_WhenApiKeyHeaderIsMissing()
        {
            //Propósito: Verificar que el controlador responde con un código 401 Unauthorized
            //si la cabecera X-API - KEY está ausente.
            // Arrange
            var request = new ChatRequest();
            _mockConfiguration.Setup(config => config.GetSection("ApiKey").Value).Returns("valid-api-key");

            SetupHttpContext();

            // Act
            var result = await _controller.CreateChat(request);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CreateChat_ReturnsUnauthorized_WhenApiKeyIsInvalid()
        {
            //Propósito: Verificar que el controlador responde con un código 401 Unauthorized
            //si la cabecera X-API - KEY contiene un valor no válido.
            // Arrange
            var request = new ChatRequest();
            _mockConfiguration.Setup(config => config.GetSection("ApiKey").Value).Returns("valid-api-key");

            SetupHttpContext("invalid-api-key");

            // Act
            var result = await _controller.CreateChat(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(401, jsonResult.StatusCode);
        }

        [Fact]
        public async Task CreateChat_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            //Propósito: Verificar que el controlador responde con un código 500 Internal Server Error
            //si ocurre una excepción inesperada durante la creación del chat.
            // Arrange
            var request = new ChatRequest();
            _mockConfiguration.Setup(config => config.GetSection("ApiKey").Value).Returns("valid-api-key");
            _mockChatServices.Setup(service => service.CreateChat(request)).ThrowsAsync(new Exception("Unexpected error"));

            SetupHttpContext("valid-api-key");

            // Act
            var result = await _controller.CreateChat(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(500, jsonResult.StatusCode); 
        }

        [Fact]
        public async Task CreateChat_ReturnsCreatedResult_WhenRequestIsValid()
        {
            //Propósito: Verificar que el controlador responde con un código 201 Created
            //cuando la solicitud es válida y se crea el chat correctamente.
            // Arrange
            var request = new ChatRequest();
            var chatResponse = new ChatResponse();

            _mockConfiguration.Setup(config => config.GetSection("ApiKey").Value).Returns("valid-api-key");
            _mockChatServices.Setup(service => service.CreateChat(request)).ReturnsAsync(chatResponse);

            SetupHttpContext("valid-api-key");

            // Act
            var result = await _controller.CreateChat(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(201, jsonResult.StatusCode);
            Assert.Equal(chatResponse, jsonResult.Value);
        }




      
        [Fact]
        public async Task GetChatById_ReturnsBadRequest_WhenPaginationValuesAreInvalid()
        {
            // Propósito: Verificar que el controlador responde con un código 400 Bad Request
            // cuando los valores de paginación son inválidos.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 1, PageIndex = -1, PageSize = 0 };
            var chatResponse = new ChatResponse
            {
                ChatId = 1,
                User1Id = 1,
                User2Id = 3
            };

            _mockChatServices.Setup(service => service.GetChatById(request.ChatId)).ReturnsAsync(chatResponse);
            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "1") });

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

        }

        [Fact]
        public async Task GetChatById_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            //Propósito: Verificar que el controlador responde con un código 400 Bad Request
            //si la solicitud contiene un ChatId no válido.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 0, PageIndex = 1, PageSize = 10 }; // ID no válido
            var chatResponse = new ChatResponse
            {
                ChatId = 0,
                User1Id = 0,
                User2Id = 3
            };

            _mockChatServices.Setup(service => service.GetChatById(request.ChatId)).ReturnsAsync(chatResponse);
            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "0") });

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetChatById_ReturnsUnauthorized_WhenUserIdIsMissingInToken()
        {
            //Propósito: Verificar que el controlador responde con un código 401 Unauthorized
            //si el token del usuario no incluye un UserId.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 1, PageIndex = 0, PageSize = 10 };

            // Simular un token sin el UserId
            SetupHttpContext(claims: new List<Claim>()); // Sin UserId

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task GetChatById_ReturnsForbidden_WhenUserIsNotInvolvedInChat()
        {
            //Propósito: Verificar que el controlador responde con un código 403 Forbidden
            //si el usuario autenticado no está involucrado en el chat solicitado.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 1, PageIndex = 0, PageSize = 10 };
            var chatResponse = new ChatResponse
            {
                ChatId = 1,
                User1Id = 2,
                User2Id = 3 // IDs que no coinciden con el usuario autenticado
            };

            _mockChatServices.Setup(service => service.GetChatById(request.ChatId)).ReturnsAsync(chatResponse);
            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "1") });

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(403, jsonResult.StatusCode);
        }

        [Fact]
        public async Task GetChatById_ReturnsNotFound_WhenChatDoesNotExist()
        {
            //Propósito: Verificar que el controlador responde con un código 404 Not Found
            //si el chat solicitado no existe.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 1 };
            _mockChatServices.Setup(service => service.GetChatById(request.ChatId)).ReturnsAsync((ChatResponse)null);
            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "1") });

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(404, jsonResult.StatusCode);
        }

        [Fact]
        public async Task GetChatById_ReturnsInternalServerError_WhenUserServiceFails()
        {
            //Propósito: Verificar que el controlador responde con un código 500 Internal Server Error
            //si el servicio de usuario falla al obtener información de los usuarios del chat.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 1, PageIndex = 1, PageSize = 10 };
            var chatResponse = new ChatResponse
            {
                ChatId = 1,
                User1Id = 1,
                User2Id = 2
            };

            _mockChatServices.Setup(service => service.GetChatById(request.ChatId)).ReturnsAsync(chatResponse);
            _mockUserApiServices.Setup(service => service.GetUserById(It.IsAny<List<int>>())).ReturnsAsync((List<UserResponse>)null);
            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "1") });

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(500, jsonResult.StatusCode);
        }

        [Fact]
        public async Task GetChatById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            //Propósito: Verificar que el controlador responde con un código 500 Internal Server Error
            //si ocurre una excepción inesperada.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 1, PageIndex = 0, PageSize = 10 };
            _mockChatServices.Setup(service => service.GetChatById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "1") });

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(500, jsonResult.StatusCode);
        }
       
        [Fact]
        public async Task GetChatById_ReturnsOk_WhenChatExistsAndUserHasAccess()
        {
            // Propósito: Verificar que el controlador responde con un código 200 OK
            // si el chat existe y el usuario autenticado tiene acceso.
            // Arrange
            var request = new MessageInitalizeRequest { ChatId = 1, PageIndex = 0, PageSize = 10 };
            var chatResponse = new ChatResponse
            {
                ChatId = 1,
                User1Id = 1, // Usuario autenticado tiene acceso
                User2Id = 2
            };
   
            // Mockear el servicio de chat
            _mockChatServices.Setup(service => service.GetChatById(request.ChatId)).ReturnsAsync(chatResponse);

            _mockUserApiServices.Setup(service => service.GetUserById(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<UserResponse>
                {
                    new UserResponse { UserId = 1, UserName = "User1" },
                    new UserResponse { UserId = 2, UserName = "User2" }
                });
            
            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "1") });

            // Act
            var result = await _controller.GetChatById(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageInitalizeResponse>(okResult.Value); 
            // Verificar que el chat es correcto en caso de éxito
            Assert.Equal(200, okResult.StatusCode);   
            Assert.Equal(1, response.ChatId);
            Assert.Equal("User1", response.UserMe.UserName);
            Assert.Equal("User2", response.UserFriend.UserName);
        }




        [Fact]
        public async Task GetMyChats_ReturnsBadRequest_WhenUserIdIsInvalid()
        {
            // Propósito: Verificar que el controlador responde con un código 401 Unauthorized
            // cuando el UserId en el token no es válido(no numérico).
            // Arrange
            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", "invalid-id") }); // UserId no numérico

            // Act
            var result = await _controller.GetMyChats();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task GetMyChats_ReturnsUnauthorized_WhenUserIdIsMissingInToken()
        {
            // Propósito: Verificar que el controlador responde con un código 401 Unauthorized
            // si el token no contiene un UserId.
            // Arrange
            SetupHttpContext(claims: new List<Claim>()); // Sin UserId

            // Act
            var result = await _controller.GetMyChats();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task GetMyChats_ReturnsEmptyList_WhenUserHasNoChats()
        {
            //Propósito: Verificar que el controlador devuelve una lista vacía cuando el usuario
            //no tiene chats asociados.
            // Arrange
            var userId = 1;
            _mockChatServices.Setup(service => service.GetChatsByUserId(userId)).ReturnsAsync((UserChat)null);

            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", userId.ToString()) });

            // Act
            var result = await _controller.GetMyChats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(okResult.Value); // Verifica que no haya chats
        }

        [Fact]
        public async Task GetMyChats_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Propósito: Verificar que el controlador responde con un código 500 Internal Server Error
            // si ocurre una excepción inesperada al obtener los chats.
            // Arrange
            var userId = 1;
            _mockChatServices.Setup(service => service.GetChatsByUserId(userId)).ThrowsAsync(new Exception("Unexpected error"));

            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", userId.ToString()) });

            // Act
            var result = await _controller.GetMyChats();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(500, jsonResult.StatusCode);
        }

        [Fact]
        public async Task GetMyChats_ReturnsOk_WhenChatsExist()
        {
            // Propósito: Verificar que el controlador responde con un código 200 OK
            // y devuelve la lista de chats cuando el usuario tiene chats disponibles.
            // Arrange
            var userId = 1;
            var userChat = new UserChat
            {
                UserMe = new UserResponse { UserId = userId, UserName = "John" },
                ListChat = new List<ChatSimpleResponse> { new ChatSimpleResponse { ChatId = 1 } }
            };

            _mockChatServices.Setup(service => service.GetChatsByUserId(userId)).ReturnsAsync(userChat);

            SetupHttpContext(claims: new List<Claim> { new Claim("UserId", userId.ToString()) });

            // Act
            var result = await _controller.GetMyChats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserChat>(okResult.Value);

            // Verifica propiedades principales
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(userId, response.UserMe.UserId);
            Assert.NotEmpty(response.ListChat);
        }

    }
}
