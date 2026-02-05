using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using SmartExpenseTracker.Core.DTOs;

namespace SmartExpenseTracker.UI.Services;

public class ExpenseApiService
{
    private readonly HttpClient _http;

    public ExpenseApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ExpenseDto>> GetExpensesAsync()
    {
        return await _http.GetFromJsonAsync<List<ExpenseDto>>("api/expense")
               ?? new List<ExpenseDto>();
    }

    public async Task UploadReceiptAsync(IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();

        var stream = file.OpenReadStream(10 * 1024 * 1024);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

        content.Add(fileContent, "file", file.Name);

        var response = await _http.PostAsync("api/expenses/upload-receipt", content);
        response.EnsureSuccessStatusCode();
    }
    public async Task<List<MonthlyExpenseDto>> GetMonthlyExpenses(
      DateTime start, DateTime end)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<MonthlyExpenseDto>>(
                $"api/expenses/monthly?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}"
            ) ?? new();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return new();
        }
    } 
    public async Task<List<ExpenseCategorySummaryDto>> GetExpenseByCategory( DateTime start, DateTime end)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<ExpenseCategorySummaryDto>>(
            $"api/expenses/by-category?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}"
        ) ?? new();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return new();
        }
    }
    public async Task<List<ExpenseByStoreDto>> GetExpenseByStore(DateTime start, DateTime end)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<ExpenseByStoreDto>>($"api/expenses/by-store?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}") ?? new();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return new();
        }
    }
    public async Task UploadReceiptWithProgressAsync(
    IBrowserFile file,
    Action<int> reportProgress)
    {
        using var content = new MultipartFormDataContent();

        var stream = file.OpenReadStream(10 * 1024 * 1024);

        var progressStream = new ProgressStream(stream, reportProgress);

        var fileContent = new StreamContent(progressStream);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

        content.Add(fileContent, "files", file.Name);

        var response = await _http.PostAsync("api/expenses/upload-receipts", content);
        response.EnsureSuccessStatusCode();
    }

}