using System;
namespace AzFuncProcessReceipts.Model
{
	public class ReceiptItem
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? TotalPrice { get; set; }
		public int? Number { get; set; }
		/// <summary>
		/// individual price
		/// </summary>
		public decimal? Price { get; set; }
		public string? ProductCode { get; set; }
		public string? QuantityUnity { get; set; }
	}
}

