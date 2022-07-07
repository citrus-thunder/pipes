public class StringCapitalizer : PipeSegment<string, string>
{
	public enum CapitalizationMode
	{
		FirstLetter,
		All
	}

	public StringCapitalizer()
	{

	}

	public StringCapitalizer(CapitalizationMode mode)
	{
		Mode = mode;
	}

	public CapitalizationMode Mode { get; set; } = CapitalizationMode.FirstLetter;

	protected override void Process(ref string input, ref string output, Action next)
	{
		output = Mode switch
		{
			CapitalizationMode.FirstLetter => output[0].ToString().ToUpper() + output.Substring(1),
			CapitalizationMode.All => output.ToUpper(),
			_ => output.ToUpper()
		};
		next();
	}
}