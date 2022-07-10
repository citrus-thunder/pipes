using System;
using System.Linq;

using Pipes;

public class StringReverser : PipeSegment<string, string>
{
	protected override void Process(ref string input, ref string output, Action next)
	{
		var rev = output.ToArray();
		Array.Reverse(rev);
		output = new string(rev);
		next();
	}
}