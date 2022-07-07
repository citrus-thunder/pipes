using System;

namespace Pipes
{
	/// <summary>
	/// PipeSegment with inline-defined behavior
	/// </summary>
	/// <typeparam name="I"></typeparam>
	/// <typeparam name="O"></typeparam>
	public class InlineSegment<I, O> : PipeSegment<I, O>
	{
		/// <summary>
		/// Delegate method used by inline segments to define behavior
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <param name="action"></param>
		/// <typeparam name="In"></typeparam>
		/// <typeparam name="Out"></typeparam>
		/// <typeparam name="Action"></typeparam>
		/// <returns></returns>
		public delegate void PipeAction<In, Out, Action>(ref In input, ref Out output, Action action);

		private PipeAction<I, O, Action> _io;

		internal InlineSegment(PipeAction<I, O, Action> io)
		{
			_io = io;
		}

		internal InlineSegment()
		{
			_io = (ref I input, ref O output, Action done) => {done();};
		}

		/// <summary>
		/// Describe this InlineSegment's data processing behavior via PipeAction delegate
		/// </summary>
		/// <param name="action">PipeAction delegate describing this InlineSegment's behavior</param>
		/// <returns></returns>
		public InlineSegment<I, O> Does(PipeAction<I, O, Action> action)
		{
			_io = action;
			return this;
		}

		/// <inheritdoc />
		protected sealed override void Process(ref I input, ref O output, Action done)
		{
			_io(ref input, ref output, done);
		}
	}
}