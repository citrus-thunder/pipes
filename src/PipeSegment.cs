using System;

namespace Pipes
{
	/// <summary>
	/// Data processing segment used in a Pipe
	/// </summary>
	/// <typeparam name="I"></typeparam>
	/// <typeparam name="O"></typeparam>
	public class PipeSegment<I, O>
	{
		internal void _Process(ref I input, ref O output, Action next) => Process(ref input, ref output, next);

		/// <summary>
		/// Describe the data processing steps this PipeSegment performs
		/// </summary>
		/// <param name="input">Input data reference</param>
		/// <param name="output">Output data reference</param>
		/// <param name="next">Next segment signal method</param>
		/// <remarks>
		/// Once data processing is complete, remember to call next() 
		/// to signal the Pipe to move on to the next segment
		/// (or terminate if this is the final segment)
		/// </remarks>
		protected virtual void Process(ref I input, ref O output, Action next)
		{
			next();
		}
	}

	/// <inheritdoc />
	public class PipeSegment : PipeSegment<object, object>
	{
		/// <summary>
		/// Describe the input and output types used by this PipeSegment
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <typeparam name="O"></typeparam>
		/// <returns></returns>
		public static InlineSegment<I, O> Takes<I, O>()
		{
			return new InlineSegment<I, O>();
		}
	}
}