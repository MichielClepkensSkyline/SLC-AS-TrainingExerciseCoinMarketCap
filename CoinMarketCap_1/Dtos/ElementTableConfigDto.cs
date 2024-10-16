namespace CoinMarketCap_1.Dtos
{
	using System.Collections.Generic;

	public class ElementTableConfigDto
	{
		public int AgentId { get; }

		public int ElementId { get; }

		public int TableId { get; }

		public int LastTableColumnId { get; }

		public List<int> TableDateColumnIds { get; }

		public ElementTableConfigDto(int agentId, int elementId, int tableId, int lastTableColumnId, List<int> tableDateColumnIds)
		{
			AgentId = agentId;
			ElementId = elementId;
			TableId = tableId;
			LastTableColumnId = lastTableColumnId;
			TableDateColumnIds = tableDateColumnIds ?? new List<int>();
		}
	}
}