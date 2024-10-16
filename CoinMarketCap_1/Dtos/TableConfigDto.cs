namespace CoinMarketCap_1.Dtos
{
	using System.Collections.Generic;

	public class TableConfigDto
	{
		public TableConfigDto(int agentId, int elementId, int tableId, int lastTableColumnId, List<int> tableDateColumnIds)
		{
			AgentId = agentId;
			ElementId = elementId;
			TableId = tableId;
			LastTableColumnId = lastTableColumnId;
			TableDateColumnIds = tableDateColumnIds ?? new List<int>();
		}

		public int AgentId { get; }

		public int ElementId { get; }

		public int TableId { get; }

		public int LastTableColumnId { get; }

		public List<int> TableDateColumnIds { get; }
	}
}