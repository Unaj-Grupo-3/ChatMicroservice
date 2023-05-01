﻿using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using Application.Reponsive;
using Azure.Core;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class ChatController : Controller
    {
       
        private readonly IChatServices _chatServices;
        private readonly ITokenServices _tokenServices;
        private readonly IMessageServices _messageServices;

        public ChatController(IChatServices chatServices, ITokenServices tokenServices, IMessageServices messageServices)
        {
            _chatServices = chatServices;
            _tokenServices = tokenServices;
            _messageServices = messageServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(ChatRequest request)
        {
            try
            {
                var response = await _chatServices.CreateChat(request);

                return new JsonResult(response) { StatusCode = 201};

            }catch (Exception ex)
            {
                return new JsonResult(new { Message = ex.Message }) { StatusCode = 500 };
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetChatById([FromQuery] MessageInitalizeRequest request, int id)
        {
            try
            {
                // Ejemplo de uso del token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                var response = await _chatServices.GetChatById(id);
                if (response == null)
                {
                    return new JsonResult(new { Message = $"No existe un chat con el id {id}" }) { StatusCode = 404 };
                }
                if ( !_tokenServices.ValidateUserId(identity,response.User1Id) & !_tokenServices.ValidateUserId(identity, response.User2Id))
                {
                    return new JsonResult(new { Message = "No esta autorizado a ver este chat" }) { StatusCode = 403 };
                }
                var userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                if (response.User1Id != userId)
                {
                    response.User2Id = response.User1Id;
                    response.User1Id = userId;
                }


                var responses = new MessageInitalizeResponse
                {
                    PageSize = request.PageSize,
                    PageIndex = request.PageIndex,
                    Messages = await _messageServices.GetMessages(request.PageSize, request.PageIndex, request.ChatId),
                    TotalItems = await _messageServices.GetMessagesLong(request.ChatId)
                };
                return Ok(responses);

            }catch (Exception ex)
            {
                return new JsonResult(new {Message = ex.Message}) { StatusCode = 500};
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyChats()
        { 
            try
            {
                // Ejemplo de uso del token
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                var response = await _chatServices.GetChatsByUserId(userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Message = ex.Message }) { StatusCode = 500 };
            }
        }
    }
}