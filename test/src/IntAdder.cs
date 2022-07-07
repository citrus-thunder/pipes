public class IntAdder : PipeSegment<int, int>
{
	public IntAdder() : base()
	{

	}

	public IntAdder(int addend)
	{
		Addend = addend;
	}

	public int Addend { get; set; } = 0;

	protected override void Process(ref int input, ref int output, Action next)
	{
		output += Addend;
		next();
	}
}