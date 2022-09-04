using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using static IWeatherService;

internal interface IWeatherService
{
    public Task<string> GetWeather(double lat, double lon, long time);
    public Task<string> GetWeather(string cityname);
    public Task<IEnumerable<CityModel>> GetCities( );
    public Task<IEnumerable<CityModel>> GetCities( string country );
    internal class Coords
    {
        public decimal? lon { get; set; }
        public decimal? lat { get; set; }

    }
    internal class CityModel
    {
        public string name { get; set; }
        public string state { get; set; }
        public string country { get; set; }

        public Coords coord { get; set; }
    }
}

internal class WeatherService: IWeatherService
{
    private string API_KEY = "e50b1efc362606b154d86dcb2e86a9ba";
    private string BASE_URL = "https://api.openweathermap.org";

    public async Task<string> GetWeather(double lat, double lon, long time)
    {
        Dictionary<string, object> pars = new Dictionary<string, object>();
        pars["lat"] = lat;
        pars["lon"] = lon;
        pars["time"] = time;
        pars["lang"] = "ru";
        pars["units"] = "metric";
        return await this.Request($"{BASE_URL}/{"data/2.5/onecall"}",pars);
    }

    public async Task<string> GetWeather(string cityname)
    {
        Dictionary<string, object> pars = new Dictionary<string, object>();
        pars["q"] = $"{cityname}";
        pars["lang"] = "ru";
        pars["units"] = "metric";
        return await this.Request($"{BASE_URL}/{"data/2.5/weather"}",pars);
    }


    /// <summary>
    /// Выполнение HTTP-запроса по URL методом GET.
    /// </summary>        
    private async Task<string> Execute(string query)
    {
        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        Console.WriteLine(query);
        HttpResponseMessage response = await client.GetAsync(query);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Выполение запроса с параметрами.
    /// </summary>
    protected async Task<T> Get<T>(string uri, Dictionary<string, object> pars) where T : class   
 
    {
        pars["appid"] = API_KEY;
        string query = this.ToQueryString(uri, pars);
        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        Console.WriteLine(query);
        HttpResponseMessage response = await client.GetAsync(query);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
        string json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }

   

    /// <summary>
    /// Выполение запроса с параметрами.
    /// </summary>
    protected async Task<string> Request(string uri, Dictionary<string, object> pars)
    {
        pars["appid"] = API_KEY;
        return await this.Execute(this.ToQueryString(uri,pars));
    }


    /// <summary>
    /// Преобразование параметров запроса.
    /// </summary>  
    private string ToQueryString(string uri, Dictionary<string, object> pars)
    {
        string queryString = "";
        foreach (var pair in pars)
        {
            queryString += $"{pair.Key}={pair.Value}&";
        }
        return $"{uri}?{queryString}";
    }

    public async Task<IEnumerable<string>> GetCountries()
    {
        string citiesJson = System.IO.File.ReadAllText(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"city.list.json"));
        var result = System.Text.Json.JsonSerializer.Deserialize<List<CityModel>>(citiesJson);
        await Task.CompletedTask;
        return result.Select( city => city.country ).Distinct();
    }
    public async Task<IEnumerable<CityModel>> GetCities( )
    {
        string citiesJson = System.IO.File.ReadAllText(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"city.list.json"));
        var result = System.Text.Json.JsonSerializer.Deserialize<List<CityModel>>(citiesJson);
        await Task.CompletedTask;
        return result;
    }

    public async Task<IEnumerable<CityModel>> GetCities(string country)
    {
        string citiesJson = System.IO.File.ReadAllText(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"city.list.json"));
        var result = System.Text.Json.JsonSerializer.Deserialize<List<CityModel>>(citiesJson);
        await Task.CompletedTask;
        return result.Where( city => city.country == country);
    }
}
