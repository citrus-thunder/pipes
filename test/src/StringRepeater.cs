public class StringRepeater : PipeSegment<string, string>
{
	public StringRepeater()
	{

	}

	public StringRepeater(int repeatCount)
	{
		RepeatCount = repeatCount;
	}

	public int RepeatCount {get; set;} = 1;

	protected override void Process(ref string input, ref string output, Action next)
	{
		for (var i = 0; i < RepeatCount; i++)
		{
			output += output;
		}
		next();
	}
}