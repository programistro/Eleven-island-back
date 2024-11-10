using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NopCommerce.Models.CDEK;
using Payment = NopCommerce.Models.Payment;

namespace Nop.Web.Service;

public class ClientCDEK
{
    private readonly string _account;
    private readonly string _securePassword;
    private string _accessToken = "";
    
    public ClientCDEK(string account, string securePassword)
    {
        _account = account;
        _securePassword = securePassword; 
        RenewAccessToken().Wait();
    }

    public async Task<string> SendOrder(Order order)
    {
        string urlRequest = "https://api.edu.cdek.ru/v2/orders";


        using (HttpClient client = new HttpClient())
        {
            // Order order = new Order
            // {
            //     Number = "ddOererre7450813980068",
            //     Comment = "Новый заказ",
            //     DeliveryRecipientCost = new DeliveryRecipientCost { Value = 50 },
            //     DeliveryRecipientCostAdv = new List<DeliveryRecipientCostAdv>
            //     {
            //         new DeliveryRecipientCostAdv { Sum = 3000, Threshold = 200 }
            //     },
            //     FromLocation = new Location
            //     {
            //         Code = "44",
            //         FiasGuid = Guid.NewGuid().ToString(),
            //         PostalCode = "",
            //         Longitude = "",
            //         Latitude = "",
            //         CountryCode = "",
            //         Region = "72",
            //         SubRegion = "",
            //         City = "Москва",
            //         KladrCode = "1",
            //         Address = "пр. Ленинградский, д.4"
            //     },
            //     ToLocation = new Location
            //     {
            //         Code = "270",
            //         FiasGuid = Guid.NewGuid().ToString(),
            //         PostalCode = "",
            //         Longitude = "",
            //         Latitude = "",
            //         CountryCode = "",
            //         Region = "",
            //         SubRegion = "",
            //         City = "Новосибирск",
            //         KladrCode = "",
            //         Address = "ул. Блюхера, 32"
            //     },
            //     Packages = new List<Package>
            //     {
            //         new Package
            //         {
            //             Number = "bar-001",
            //             Comment = "Упаковка",
            //             Height = 10,
            //             Items = new List<Item>
            //             {
            //                 new Item
            //                 {
            //                     WareKey = "00055",
            //                     Payment = new NopCommerce.Models.CDEK.Payment { Value = 3000 },
            //                     Name = "Товар",
            //                     Cost = 300,
            //                     Amount = 2,
            //                     Weight = 700,
            //                     Url = "www.item.ru"
            //                 }
            //             },
            //             Length = 10,
            //             Weight = 4000,
            //             Width = 10
            //         }
            //     },
            //     Recipient = new Recipient
            //     {
            //         Name = "Иванов Иван",
            //         Phones = new List<Phone>
            //         {
            //             new Phone { Number = "+79134637228" }
            //         }
            //     },
            //     Sender = new Sender
            //     {
            //         Name = "Петров Петр"
            //     },
            //     Services = new List<NopCommerce.Models.CDEK.Service>
            //     {
            //         new NopCommerce.Models.CDEK.Service { Code = "SECURE_PACKAGE_A2" }
            //     },
            //     TariffCode = 139,
            // };

            var jsonContent = JsonConvert.SerializeObject(order);

            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            HttpResponseMessage response = await client.PostAsync(urlRequest, content);

            Console.WriteLine(response.IsSuccessStatusCode);
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

            Console.WriteLine("Error: " + response.StatusCode);

            return await response.Content.ReadAsStringAsync();
        }
    }

    private async Task RenewAccessToken()
    {
        string urlRequest = "https://api.cdek.ru/v2/oauth/token?parameters";
        
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials"},
                    { "client_id", _account },
                    { "client_secret", _securePassword },
                };
                HttpContent content = new FormUrlEncodedContent(formData);
                HttpResponseMessage response = await client.PostAsync(urlRequest, content);
                
                if (response.IsSuccessStatusCode)
                {
                    string pattern = "\"access_token\":\"(.*?)\"";
                    var match = Regex.Match(response.Content.ReadAsStringAsync().Result, pattern);
                    _accessToken = match.Groups[1].Value;
                    return;
                }
                
                Console.WriteLine("Error: " + response.StatusCode);

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    public async Task<string?> GetCodeCity(string fullNameCity)
    {
        string urlRequest = "https://api.cdek.ru/v2/location/cities?";
        urlRequest += $"city={fullNameCity}";
        
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                HttpResponseMessage response = await client.GetAsync(urlRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    string pattern = "\"code\":(\\d+)";
                    var match = Regex.Match(response.Content.ReadAsStringAsync().Result, pattern);
                    return match.Groups[1].Value;
                }
                
                Console.WriteLine("Error: " + response.StatusCode);
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
            
        }

        return null;
    }
    
    public async Task<string?> CalculatePrice(string cityArrival, double mass, double volume)
    {
        string urlRequest = "https://api.cdek.ru/v2/calculator/tariff";
        string tarifCode = "136";   // код тарифа
        string fromCodeCity = "563";    //код города отправления - Чайковский 
        if (cityArrival.Contains(','))
        {
            cityArrival = cityArrival.Split(',')[0];
            Console.WriteLine(cityArrival);
        }
        string? toCodeCity = await GetCodeCity(cityArrival);
        string massStr = (mass * 1000).ToString(CultureInfo.InvariantCulture);
        string volumeStr = volume.ToString(CultureInfo.InvariantCulture);
        
        string jsonRequestData = $"{{\n    \"tariff_code\": \"{tarifCode}\",\n    " +
                     $"\"from_location\": {{\n        \"code\": {fromCodeCity}\n    }},\n    \"to_location\": {{\n        " +
                     $"\"code\": {toCodeCity}\n    }},\n    \"packages\": [\n        {{\n            \"height\": 0,\n           " +
                     $" \"length\": 0,\n            \"weight\": {massStr},\n            \"width\": 0\n        }}\n    ]\n}}";
        
        try
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                HttpResponseMessage response = await client.PostAsync(urlRequest, content);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    string pattern =  "\"delivery_sum\":(\\d+(\\.\\d+)?)";
                    var match = Regex.Match(response.Content.ReadAsStringAsync().Result, pattern);
                    
                    return match.Groups[1].Value;
                }
                
                Console.WriteLine("Error: " + response.StatusCode);

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }

}