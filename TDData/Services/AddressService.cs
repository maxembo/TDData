using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TDData.DTO;
using TDData.Settings;

namespace TDData.Services;

public class AddressService
{
    private const string RequestUri = "api/v1/clean/address";
    private const string JsonMediaType = "application/json";

    private readonly HttpClient _httpClient;
    private readonly DaDataSettings _settings;

    public AddressService(HttpClient httpClient, IOptions<DaDataSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_settings.ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("X-Secret", _settings.SecretApi);
    }

    public async Task<AddressResponseDto> Get(string address)
    {
        var jsonConvert = JsonConvert.SerializeObject(new[] { address });
        
        var content = new StringContent(jsonConvert, Encoding.UTF8, JsonMediaType);

        var response = await _httpClient.PostAsync(RequestUri, content);
        response.EnsureSuccessStatusCode();
        
        var responseData = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<AddressResponseDto>>(responseData);
        
        return result.FirstOrDefault();
    }
}