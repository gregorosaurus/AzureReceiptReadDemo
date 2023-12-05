using System;
namespace AzFuncProcessReceipts.Model
{
	public class Receipt
	{
		public int Id { get; set; }
		public string? MerchantName { get; set; }
		public string? MerchantPhoneNumber { get; set; }
		public string? MerchantAddress { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
		public DateTime? TransactionDateTime { get; set; }
		public decimal? TotalTax { get; set; }
		public decimal? Tip { get; set; }

		public virtual List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
		public virtual List<TaxDetail> TaxDetails { get; set; } = new List<TaxDetail>();
	}
}

