using System.Security.Cryptography;
using System.Text;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Web.Service;
using NopCommerce.Models;

namespace Nop.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayController : ControllerBase
{
    private readonly ILogger<PayController> _logger;

    public PayController(ILogger<PayController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder(NopCommerce.Models.CDEK.Order order)
    {
        if (order == null)
        {
            return BadRequest();
        }
        
        var client = new ClientCDEK("2C5aVo5xBEQk83PNTaKcvpwHM9YKrkjc", "VXGbhYoE3zVDqlysEBGU7M71Blg0Tc5Z");
// Console.WriteLine(await client.CalculatePrice("Ижевск", 12, 0.07));

        var line = await client.SendOrder(order);
        
        return Ok(line);
    }

    [AllowAnonymous]
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment(PaymentDto payment)
    {
        if (payment == null)
        {
            return BadRequest();
        }
        
        using (HttpClient client = new())
        {
            string orderId = Guid.NewGuid().ToString();

            int fullAnmount = 0;

            foreach (var item in payment.Items)
            {
                fullAnmount += item.Amount;
            }
            
            // 1728391757593DEMO
            // p8h3wOZ6LlauQckv
            
            Payment rootObject = new()
            {
                TerminalKey = "1728391757616",
                Amount = fullAnmount,
                OrderId = orderId,
                Description = payment.Discription,
                Token = "4d80aabfc960d3fe58803302d5af8eee306993e3e6c3ee87a3ac59f60a3240f2",
                DATA = new NopCommerce.Models.Data
                {
                    Email = payment.Email
                },
                Receipt = new Receipt
                {
                    Email = payment.Email,
                    Taxation = "usn_income",
                    Items = payment.Items
                }
            };
            
            var data = new[]
            {
                new { Key = "TerminalKey", Value = "1728391757616" },
                new { Key = "Amount", Value = payment.Anmount.ToString() },
                new { Key = "OrderId", Value = orderId },
                new { Key = "Description", Value = payment.Discription },
                new { Key = "Password", Value = "VBDItoak4QrzD2oI" }
            };
            
            var sortedData = data.OrderBy(x => x.Key).ToArray();
            
            var concatenatedString = string.Join("", sortedData.Select(x => x.Value));
            
            byte[] hashBytes;
            using (var sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
            }

            string token = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            rootObject.Token = token;
            
            var jsonContent = JsonConvert.SerializeObject(rootObject);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var con = await client.PostAsync("https://securepay.tinkoff.ru/v2/Init", content);

            if (con.IsSuccessStatusCode)
            {
                string line = await con.Content.ReadAsStringAsync();
                
                return Ok(line);
            }
            else
            {
                return BadRequest(await con.RequestMessage.Content.ReadAsStringAsync());
            }
        }
    }
}