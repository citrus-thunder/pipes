public class PipeTests
{
	[Fact]
	public void TestMutateStringsInline()
	{
		var copier = PipeSegment
			.Takes<string, string>()
			.Does((ref string input, ref string output, Action next) =>
			{
				output = input;
				next();
			});

		var doubler = PipeSegment
			.Takes<string, string>()
			.Does((ref string input, ref string output, Action next) =>
			{
				output += output;
				next();
			});

		var capitalizer = PipeSegment
			.Takes<string, string>()
			.Does((ref string input, ref string output, Action next) =>
			{
				output = output[0].ToString().ToUpper() + output.Substring(1);
				next();
			});

		var messagePipe1 = Pipe.Takes<string, string>()
			.Then(copier)
			.Then(doubler)
			.Then(capitalizer);

		var messagePipe2 = Pipe.Takes<string, string>()
			.Then(copier)
			.Then(capitalizer)
			.Then(doubler);

		var @in = "test";
		var res1 = "res1";
		var res2 = "res2";

		messagePipe1.In(ref @in, ref res1);
		messagePipe2.In(ref @in, ref res2);

		Assert.Equal("Testtest", res1);
		Assert.Equal("TestTest", res2);
	}

	[Fact]
	public void TestMutateStringsDerived()
	{
		var input = "teststring";

		var pipe = Pipe
			.Takes<string, string>()
			.Then((ref string input, ref string output, Action next) =>
			{
				output = input;
				next();
			})
			.Then<StringCapitalizer>()
			.Then((ref string input, ref string output, Action next) =>
			{
				Assert.Equal("Teststring", output);
				next();
			})
			.Then<StringReverser>()
			.Then((ref string input, ref string output, Action next) =>
			{
				Assert.Equal("gnirtstseT", output);
				next();
			})
			.Then<StringRepeater>()
			.Then((ref string input, ref string output, Action next) =>
			{
				Assert.Equal("gnirtstseTgnirtstseT", output);
				next();
			});

		var output = "";
		pipe.In(ref input, ref output);

		Assert.Equal("gnirtstseTgnirtstseT", output);
	}

	[Fact]
	public void TestMutateIntsInline()
	{
		var addTen = PipeSegment
			.Takes<int, int>()
			.Does((ref int input, ref int output, Action next) =>
			{
				output += 10;
				next();
			});

		var @double = PipeSegment
			.Takes<int, int>()
			.Does((ref int input, ref int output, Action next) =>
			{
				output *= 2;
				next();
			});

		var pipe1 = Pipe
			.Takes<int, int>()
			.Then(addTen)
			.Then(@double);

		var pipe2 = Pipe
			.Takes<int, int>()
			.Then(@double)
			.Then(addTen);

		var @in = 5;
		var res1 = @in;
		var res2 = @in;

		pipe1.In(ref @in, ref res1);
		pipe2.In(ref @in, ref res2);

		Assert.Equal(30, res1);
		Assert.Equal(20, res2);
	}

	[Fact]
	public void TestMutateIntsDerived()
	{
		var addTen = new IntAdder(10);
		var @double = new IntMultiplier(2);

		var pipe1 = Pipe
			.Takes<int, int>()
			.Then(addTen)
			.Then<IntAdder>(out IntAdder addFive)
			.Then(@double)
			.Then<IntMultiplier>(out IntMultiplier triple);

		addFive.Addend = 5;
		triple.Multiplier = 3;

		var pipe2 = Pipe
			.Takes<int, int>()
			.Then(addFive)
			.Then(triple)
			.Then(addTen)
			.Then(@double);

		int input = 5;
		int out1 = 5;
		int out2 = 5;

		pipe1.In(ref input, ref out1);
		pipe2.In(ref input, ref out2);

		Assert.Equal(120, out1);
		Assert.Equal(80, out2);
	}

	[Fact]
	public void TestMutateReferenceTypes()
	{
		var copyInput = PipeSegment
			.Takes<IntContainer, IntContainer>()
			.Does((ref IntContainer input, ref IntContainer output, Action next) =>
			{
				output.Contents = input.Contents;
				next();
			});

		var add10 = PipeSegment
			.Takes<IntContainer, IntContainer>()
			.Does((ref IntContainer input, ref IntContainer output, Action next) =>
			{
				output.Contents += 10;
				next();
			});

		var doubleOutput = PipeSegment
			.Takes<IntContainer, IntContainer>()
			.Does((ref IntContainer input, ref IntContainer output, Action next) =>
			{
				output.Contents *= 2;
				next();
			});

		var doubleInput = PipeSegment
			.Takes<IntContainer, IntContainer>()
			.Does((ref IntContainer input, ref IntContainer output, Action next) =>
			{
				input.Contents *= 2;
				next();
			});

		var pipe = Pipe
			.Takes<IntContainer, IntContainer>()
			.Then(copyInput)
			.Then(add10)
			.Then(doubleOutput)
			.Then(doubleInput);

		var input = new IntContainer
		{
			Contents = 5
		};

		var res = pipe.Out<IntContainer>(ref input);

		Assert.Equal(10, input.Contents);
		Assert.Equal(30, res.Contents);
	}

	[Fact]
	public void TestMetaPipe()
	{
		var pipe1 = Pipe
			.Takes<string, string>()
			.Then<StringRepeater>()
			.Then<StringReverser>();

		var pipe2 = Pipe
			.Takes<string, string>()
			.Then((ref string input, ref string output, Action next) =>
			{
				output = input;
				next();
			})
			.Then<StringCapitalizer>()
			.Then(pipe1);

		var input = "test";
		var output = "";

		pipe2.In(ref input, ref output);

		Assert.Equal("tseTtseT", output);
	}

	private class IntContainer
	{
		public int Contents { get; set; }
	}
}