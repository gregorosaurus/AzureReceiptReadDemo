using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.AI.FormRecognizer.Models;
using Azure;
using Azure.AI.FormRecognizer;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using AzFuncProcessReceipts.Services;

namespace AzFuncProcessReceipts.Functions
{
    public class ProcessReceipt
    {
       
        private ILogger<ProcessReceipt> _log;
        private IReceiptProcessor _receiptProcessor;
        private Model.ReceiptsContext _context;
        public ProcessReceipt(ILogger<ProcessReceipt> log, IReceiptProcessor receiptProcessor, Model.ReceiptsContext dbContext)
        {
            _log = log;
            _receiptProcessor = receiptProcessor;
            _context = dbContext;
        }

        [FunctionName("ProcessReceipt")]
        public async Task ProcessReceiptAsync([BlobTrigger("receipts/{name}", Connection = "SFTPStorageConnectionString")]Stream myBlob, string name)
        {
            _log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var receipts = await _receiptProcessor.ProcessReceiptDataAsync(myBlob);
            _log.LogInformation($"Found {receipts.Count} receipts.");

            _context.Receipts.AddRange(receipts);
            _context.SaveChanges();
        }
    }
}

