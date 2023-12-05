using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzFuncProcessReceipts.Services
{
	public interface IReceiptProcessor
	{
		Task<List<Model.Receipt>> ProcessReceiptDataAsync(Stream receiptData);
	}
}

