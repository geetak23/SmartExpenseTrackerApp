using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartExpenseTracker.Core.Interfaces;
using SmartExpenseTracker.Core.Models;
using System;
using System.Threading.Tasks;
using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Configuration; 

namespace SmartExpenseTracker.Infrastructure.Services
{
    public class DocumentAnalyzerService : IDocumentAnalyzer
    {
        private readonly DocumentIntelligenceClient _client;

        public DocumentAnalyzerService(IConfiguration config)
        {
            var endpoint = config["AzureDocumentIntelligence:Endpoint"];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("AzureDocumentIntelligence:Endpoint configuration value is missing or empty.", nameof(config));

            var key = config["AzureDocumentIntelligence:Key"];
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("AzureDocumentIntelligence:Key configuration value is missing or empty.", nameof(config));

            _client = new DocumentIntelligenceClient(
                new Uri(endpoint),
                new AzureKeyCredential(key)
            );
        }

        public async Task<Expense> AnalyzeReceiptAsync(Stream stream, string fileName)
        {
            // var result = await _client.AnalyzeDocumentAsync(
            //   WaitUntil.Completed,"prebuilt-receipt",stream);
            // Convert the stream to BinaryData as required by the correct overload
            var binaryData = BinaryData.FromStream(stream);

            var result = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed, "prebuilt-receipt", binaryData);

            var doc = result.Value.Documents.First();

            // Safely extract fields using the correct DocumentField properties
            string merchantName = doc.Fields.TryGetValue("MerchantName", out var merchantField)
                ? merchantField.ValueString
                : string.Empty;

            DateTime? transactionDate = doc.Fields.TryGetValue("TransactionDate", out var dateField)
                ? dateField.ValueDate?.DateTime
                : null;

            decimal amount = doc.Fields.TryGetValue("Total", out var totalField) && totalField.ValueDouble.HasValue
                ? (decimal)totalField.ValueDouble.Value
                : 0;

            return new Expense
            {
                MerchantName = merchantName,
                TransactionDate = transactionDate,
                Amount = amount,
                Category = Core.Enums.ExpenseCategory.Grocery,
                DocumentName = fileName
            };
        }
    }
}
