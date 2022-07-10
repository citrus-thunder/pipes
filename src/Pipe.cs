using System;
using System.Collections.Generic;

namespace Pipes
{
	/// <summary>
	/// Describes a procedural data manipulation process
	/// </summary>
	/// <typeparam name="I"></typeparam>
	/// <typeparam name="O"></typeparam>
	public class Pipe<I, O> : PipeSegment<I, O>
	{
		private List<PipeSegment<I, O>> _segments = new List<PipeSegment<I, O>>();
		private I _input;
		private O _output;

		internal Pipe() { }

		/// <summary>
		/// Process data by muating existing input and output objects
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		public void In(ref I input, ref O output)
		{
			_input = input;
			_output = output;

			RunSegment(0);

			input = _input;
			output = _output;

			_input = default(I);
			_output = default(O);
		}

		/// <summary>
		/// Process input and produce a new output object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public T Out<T>(ref I input) where T : class, O, new()
		{
			var t = new T();
			var o = (O)t;
			In(ref input, ref o);
			return t;
		}

		private void RunSegment(int index)
		{
			if (index >= _segments.Count)
			{
				return;
			}

			_segments[index]._Process(ref _input, ref _output, () =>
			{
				RunSegment(++index);
			});
		}

		/// <summary>
		/// Add a PipeSegment to the Pipe
		/// </summary>
		/// <param name="segment">PipeSegment to add to the Pipe</param>
		/// <remarks>
		/// PipeSegments process input and output objects in ther
		/// order in which they are added to the Pipe via <c>Pipe.Then()</c>
		/// </remarks>
		public Pipe<I, O> Then(PipeSegment<I, O> segment)
		{
			_segments.Add(segment);
			return this;
		}

		/// <summary>
		/// Add a PipeSegment to the Pipe which will process input and output data using the given behavior
		/// </summary>
		/// <param name="action"></param>
		/// <remarks>
		/// PipeSegments process input and output objects in ther
		/// order in which they are added to the Pipe via <c>Pipe.Then()</c>
		/// </remarks>
		public Pipe<I, O> Then(InlineSegment<I, O>.PipeAction<I, O, Action> action)
		{
			var segment = new InlineSegment<I, O>(action);
			Then(segment);
			return this;
		}

		/// <summary>
		/// Add a PipeSegment to the Pipe
		/// </summary>
		/// <param name="segment">Reference to new PipeSegment added to the Pipe</param>
		/// <typeparam name="T"></typeparam>
		/// <remarks>
		/// PipeSegments process input and output objects in ther
		/// order in which they are added to the Pipe via <c>Pipe.Then()</c>
		/// </remarks>
		public Pipe<I, O> Then<T>(out T segment) where T : PipeSegment<I, O>, new()
		{
			segment = new T();
			Then(segment);
			return this;
		}

		/// <summary>
		/// Add a PipeSegment to the Pipe
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <remarks>
		/// PipeSegments process input and output objects in ther
		/// order in which they are added to the Pipe via <c>Pipe.Then()</c>
		/// </remarks>
		public Pipe<I, O> Then<T>() where T : PipeSegment<I, O>, new()
		{
			return Then<T>(out T segment);
		}

		/// <inheritdoc />
		protected override void Process(ref I input, ref O output, Action next)
		{
			In(ref input, ref output);
			next();
		}
	}

	/// <inheritdoc />
	public class Pipe : Pipe<object, object>
	{
		/// <summary>
		/// Describe the input and output types used by this Pipe
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <typeparam name="O"></typeparam>
		/// <returns></returns>
		public static Pipe<I, O> Takes<I, O>()
		{
			return new Pipe<I, O>();
		}
	}
}