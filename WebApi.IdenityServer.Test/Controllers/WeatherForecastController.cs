using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.IdenityServer.Test.Controllers
{
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    [ApiController]
    [Route("/api/weather")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastController(IHttpClientFactory clientFactory, ILogger<WeatherForecastController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var claims = User.Claims.ToList();
            
            var apiClient = _clientFactory.CreateClient();
            apiClient.SetBearerToken(await HttpContext.GetTokenAsync("access_token"));

            var responseMessage = await apiClient.GetAsync("https://localhost:5010/secret");
            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            return Ok(new { value = responseContent });

        }
    }
}