using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;
using SmartExpenseTracker.Infrastructure.Data.Entities;
using System.Text.RegularExpressions;

namespace SmartExpenseTracker.Infrastructure.Services
{
    public class ReceiptAnalysisService
    {
        private readonly DocumentAnalysisClient _client;

        public ReceiptAnalysisService(IConfiguration config)
        {
            _client = new DocumentAnalysisClient(
                new Uri(config["AzureAI:Endpoint"]),
                new AzureKeyCredential(config["AzureAI:Key"]));
        }
        decimal? ExtractDiscountPercentage(string content)
        {
            var match = Regex.Match(
                content,
                @"(\d+(\.\d+)?)\s*%\s*Discount",
                RegexOptions.IgnoreCase
            );

            return match.Success
                ? (decimal?)decimal.Parse(match.Groups[1].Value)
                : null;
        }
        decimal? ExtractUndiscountedTotal(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var match = Regex.Match(
                content,
                @"UNDISCOUNTED\s*TOTAL[\s:]*\$?\s*(\d+(\.\d{1,2})?)",
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );

            if (!match.Success)
                return null;

            return decimal.TryParse(match.Groups[1].Value, out var amount)
                ? amount
                : null;
        }
        public async Task<(Receipt receipt, List<ReceiptItem> items)>
            AnalyzeAsync(Stream stream)
        {
            var result = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-receipt",
                stream);

            var doc = result.Value.Documents.First();

            
            var rawText = result.Value.Content;

            // Undiscounted Total (Field → fallback to raw text)
            decimal? undiscountedAmount =
                doc.Fields.GetValueOrDefault("UnDiscounted Total")?.Value.AsDouble() is double d
                    ? (decimal)d
                    : ExtractUndiscountedTotal(rawText);

            // Subtotal
            decimal? subTotal =
                doc.Fields.GetValueOrDefault("Subtotal")?.Value.AsDouble() is double sub
                    ? (decimal)sub
                    : null;

            // Total (Subtotal preferred, fallback to Total)
            decimal? totalAmount =
                subTotal ??
                (doc.Fields.GetValueOrDefault("Total")?.Value.AsDouble() is double total
                    ? (decimal)total
                    : null);

            // Discount = Undiscounted − Subtotal
            decimal? discount = null;

            if (undiscountedAmount != null && subTotal != null)
            {
                discount = undiscountedAmount - subTotal;

                // Safety check against OCR noise
                if (discount < 0)
                    discount = null;
            }

            var receipt = new Receipt
            {
                MerchantName = doc.Fields.GetValueOrDefault("MerchantName")?.Content,

                TransactionDate =
                    doc.Fields.GetValueOrDefault("TransactionDate")?.Value.AsDate() is DateTimeOffset dto
                        ? dto.UtcDateTime
                        : (DateTime?)null,

                UndiscountedTotalAmount = undiscountedAmount,
                TotalAmount = totalAmount,
                DiscountedAmount = discount,

                Tax =
                    doc.Fields.GetValueOrDefault("TotalTax")?.Value.AsDouble() is double x
                        ? (decimal)x
                        : null
            };


            var items = new List<ReceiptItem>();

            if (doc.Fields.TryGetValue("Items", out var itemsField))
            {
                foreach (var item in itemsField.Value.AsList())
                {
                    var f = item.Value.AsDictionary();

                    var quantity = f.GetValueOrDefault("Quantity")?.Value.AsDouble() is double q
                        ? (decimal)q
                        : (decimal?)null;

                    var unitPrice = f.GetValueOrDefault("Price")?.Value.AsDouble() is double p
                        ? (decimal)p
                        : (decimal?)null;

                    var totalPrice = f.GetValueOrDefault("TotalPrice")?.Value.AsDouble() is double tp
                        ? (decimal)tp
                        : (decimal?)null;

                    // Rule 1: Default Quantity = 1
                    quantity ??= 1;

                    // Rule 2: If UnitPrice is null, calculate it from TotalPrice
                    if (unitPrice == null && totalPrice != null)
                    {
                        unitPrice = totalPrice / quantity;
                    }

                    // Optional safety: If TotalPrice is null, calculate it
                    if (totalPrice == null && unitPrice != null)
                    {
                        totalPrice = unitPrice * quantity;
                    }

                    items.Add(new ReceiptItem
                    {
                        ItemName = f.GetValueOrDefault("Description")?.Content,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = totalPrice
                    });
                }
            }

            return (receipt, items);
        }
    }
}
