using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AzFuncProcessReceipts.Model;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Microsoft.Extensions.Logging;

namespace AzFuncProcessReceipts.Services
{
    public class AzReceiptProceesor : IReceiptProcessor
    {
        public class Options
        {
            public string FormRecognizerKey { get; set; }
            public string FormRecoginzerEndpoint { get; set; }
        }

        private ILogger<AzReceiptProceesor> _log;

        private Options _options;

        public AzReceiptProceesor(ILogger<AzReceiptProceesor> log,
            Options options)
        {
            _log = log;
            _options = options;
        }

        public async Task<List<Receipt>> ProcessReceiptDataAsync(Stream receiptData)
        {
            List<Receipt> receipts = new List<Receipt>();

            // Create a FormRecognizerClient
            var credential = new AzureKeyCredential(_options.FormRecognizerKey);
            var client = new FormRecognizerClient(new Uri(_options.FormRecoginzerEndpoint), credential);

            RecognizeReceiptsOperation operation = await client.StartRecognizeReceiptsAsync(receiptData);
            await operation.WaitForCompletionAsync();

            RecognizedFormCollection operationResult = operation.Value;

            // Process the extracted data

            foreach (RecognizedForm receipt in operationResult)
            {
                Model.Receipt dbReceipt = new Model.Receipt();

                ProcessFieldDictionary<Model.Receipt>(receipt.Fields, dbReceipt);

                //special cases
                if (receipt.Fields.ContainsKey("TransactionDate"))
                {
                    dbReceipt.TransactionDateTime = receipt.Fields["TransactionDate"].Value.AsDate();
                    if (receipt.Fields.ContainsKey("TransactionTime"))
                    {
                        var time = receipt.Fields["TransactionTime"].Value.AsTime();
                        dbReceipt.TransactionDateTime.Value.Add(time);
                    }
                }

                if (receipt.Fields.ContainsKey("Items"))
                {
                    FormField fieldValue = receipt.Fields["Items"];
                    foreach(FormField itemFieldValue in fieldValue.Value.AsList())
                    {
                        Model.ReceiptItem item = new ReceiptItem();
                        dbReceipt.Items.Add(item);
                        var itemDictionary = itemFieldValue.Value.AsDictionary();
                        ProcessFieldDictionary(itemDictionary, item);
                    }
                }

                receipts.Add(dbReceipt);
            }

            return receipts;
        }

        private void ProcessFieldDictionary<T>(IReadOnlyDictionary<string, FormField> fields, T obj)
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                try
                {
                    string matchingField = fields.Keys.Where(x => x.ToLower() == property.Name.ToLower()).FirstOrDefault();
                    if (!string.IsNullOrEmpty(matchingField))
                    {
                        FormField fieldValue = fields[matchingField];

                        if (property.PropertyType == typeof(string))
                        {
                            if (fieldValue.Value.ValueType == FieldValueType.PhoneNumber)
                            {
                                property.SetValue(obj, fieldValue.Value.AsPhoneNumber());
                            }
                            else if (fieldValue.Value.ValueType == FieldValueType.String)
                            {
                                property.SetValue(obj, fieldValue.Value.AsString());
                            }
                            else
                            {
                                //unsuppported
                            }

                        }
                        else if (property.PropertyType == typeof(decimal?))
                        {
                            property.SetValue(obj, (decimal)fieldValue.Value.AsFloat());
                        }
                        else if (property.PropertyType == typeof(int?))
                        {
                            property.SetValue(obj, (int)fieldValue.Value.AsInt64());
                        }

                    }
                }
                catch (Exception e)
                {
                    _log.LogWarning($"Unable to set property {property.Name}: {e.Message}");
                }
            }
        }
    }
}
