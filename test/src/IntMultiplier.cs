public class IntMultiplier : PipeSegment<int, int>
{
	public IntMultiplier() : base()
	{

	}

	public IntMultiplier(int multiplier)
	{
		Multiplier = multiplier;
	}

	public int Multiplier { get; set; } = 1;

	protected override void Process(ref int input, ref int output, Action next)
	{
		output *= Multiplier;
		next();
	}
}