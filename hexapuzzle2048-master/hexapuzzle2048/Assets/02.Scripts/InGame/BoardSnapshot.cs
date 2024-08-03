
public class BoardSnapshot
{
	public int highestNumber { get; private set; }
	public int[] boardNumbers { get; private set; }
	public int[] blockNumbers { get; private set; }
	public int score { get; private set; }
	public int savings { get; private set; }

	public BoardSnapshot(CombinableHexaBoard board, HexaBlock block, int score, int savings)
	{
		blockNumbers = block.numbers;
		boardNumbers = board.numbers;
		highestNumber = board.highestNumber;

		this.score = score;
		this.savings = savings;
	}
}
