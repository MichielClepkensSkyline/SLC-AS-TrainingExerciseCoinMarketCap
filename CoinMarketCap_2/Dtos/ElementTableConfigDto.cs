namespace CoinMarketCap_2.Dtos
{
	using System.Collections.Generic;

	public class ElementTableConfigDto
	{
		public int AgentId { get; set;  }

		public int ElementId { get; set; }

		public int TableId { get; set; }

		public int LastTableColumnId { get; set; }

		public List<int> TableDateColumnIds { get; set; }
	}
}