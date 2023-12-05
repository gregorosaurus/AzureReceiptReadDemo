using System;
namespace AzFuncProcessReceipts.Model
{
	public class TaxDetail
	{
		public int Id { get; set; }
		public string? LineItem { get; set; }
		public decimal? Ammount { get; set; }
	}
}

