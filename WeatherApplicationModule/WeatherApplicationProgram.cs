using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static Api.Utils;
using System;

internal class WeatherApplicationProgram
{
    internal static void Run(ref string[] args)
        => MainMenu( ref args );

    internal static void TestWeatherService(ref string[] args)
    {                 
        var service = new WeatherService();
        foreach (var country in service.GetCountries().Result)
            Console.WriteLine(country);
        foreach (var city in service.GetCities().Result)
            Console.WriteLine($"{city.country} {city.name} {(city.coord==null?"":""+city.coord.lat)}{(city.coord == null ? "" : "" + city.coord.lon)}");
        Console.WriteLine(service.GetWeather(59.9294f, 30.3563f, DateTimeOffset.Now.ToUnixTimeSeconds()).Result);
        ConfirmContinue();
        MainMenu( ref args );
    }

    /// <summary>
    /// Главное меню
    /// </summary>
    public static void MainMenu(ref string[] args)
    {
        Clear();
        switch (ProgramDialog.SingleSelect("OpenWeatherApi", new string[]{
            "Прогноз погоды","Выход"},ref args))
        {
            case "Прогноз погоды":
                WeatherForecastMenu(ref args);
                break;
            case "Тестирование":
                TestWeatherService(ref args);
                break;
            case "Выход":
                Process.GetCurrentProcess().Kill();
                break;
            default: break;
        }
    }

    /// <summary>
    /// Меню выбора метода построения прогноза погоды
    /// </summary>
    public static void WeatherForecastMenu(ref string[] args)
    {
        Clear();
        switch (ProgramDialog.SingleSelect("Прогноз погоды", new string[]{
            "По городам", "По координатам", "Назад"}, ref args))
        {
            case "По городам":
                ForecastForCityMenu(ref args);
                break;
            case "По координатам":
                ForecastForLocationMenu(ref args);
                break;
            case "Назад":
                MainMenu(ref args);
                break;
            default: break;
        }
    }

    private static void ForecastForCountryMenu( string country, ref string[] args)
    {
        Clear();
        var service = new WeatherService();
        var options = new List<string>() { "Назад" };        
        options.AddRange(service.GetCities(country).Result.Select(city => city.name) );
        string city = null;
        switch (city = ProgramDialog.SingleSelect("Выберите страну", options.ToArray(), ref args))
        {
            case "Назад":
                ForecastForCityMenu(ref args);
                break;
            default:
                ForecastForCity(city, ref args);
                break;
        }
    }

    private static void ForecastForCity(string city, ref string[] args)
    {
        var service = new WeatherService();
        service.Info(service.GetWeather(city).Result.ToJsonOnScreen());        
        ConfirmContinue();
        Clear();
        MainMenu(ref args);
    }

    private static void ForecastForCityMenu(ref string[] args)
    {
        Clear();
        var service = new WeatherService();
        var options = new List<string>() { "Назад" };
        options.AddRange(service.GetCountries().Result);
        string country = null;
        switch (country = ProgramDialog.SingleSelect("Выберите страну", options.ToArray(), ref args))
        {           
            case "Назад":
                WeatherForecastMenu(ref args);
                break;
            default:
                ForecastForCountryMenu(country, ref args);
                break;
        }
    }

    private static void ForecastForLocationMenu(ref string[] args)
    {
        throw new NotImplementedException();
    }
}
