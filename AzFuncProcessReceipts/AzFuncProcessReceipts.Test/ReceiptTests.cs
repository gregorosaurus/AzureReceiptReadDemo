using Microsoft.Extensions.Logging;

namespace AzFuncProcessReceipts.Test;

[TestClass]
public class ReceiptTests
{
    [TestMethod]
    public void TestProcessingReceipt()
    {
        ILogger<Services.AzReceiptProceesor> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<Services.AzReceiptProceesor>();
        Services.AzReceiptProceesor.Options options = new Services.AzReceiptProceesor.Options()
        {
            FormRecoginzerEndpoint = "https://canadacentral.api.cognitive.microsoft.com/",
            FormRecognizerKey = ""
        };
        Services.AzReceiptProceesor azReceiptProceesor = new Services.AzReceiptProceesor(logger,options);

        FileStream inData = new FileStream("Data/receipt_sample2.jpeg", FileMode.Open, FileAccess.Read);
        var receipts = azReceiptProceesor.ProcessReceiptDataAsync(inData).Result;
        Assert.IsTrue(receipts.Count > 0);

    }
}
