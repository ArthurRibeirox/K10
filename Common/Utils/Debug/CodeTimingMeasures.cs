using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

public static class CodeTimingMeasures
{
	static bool enabled = false;

	public static void Enable() { enabled = true; }
	public static void Disable() { enabled = false; _logStopwatch.Stop(); }
	public static void Clear() { Frame.Clear(); Specific.Clear(); }

	private static Stopwatch _logStopwatch = new Stopwatch();
	
	public static string GetLog( string msFormat = "F3", string percentageFormat = "F1" ) { return Frame.GetLog( msFormat, percentageFormat ) + "\n" + Specific.GetLog( msFormat ); }

	public static class Frame
	{
		private static readonly Dictionary<string,Stopwatch> _watches = new Dictionary<string, Stopwatch>();
		private static readonly Dictionary<string,double> _accTimmings = new Dictionary<string,double>();
		private static readonly Dictionary<string,int> _calls = new Dictionary<string,int>();

		public static void LogStart( string tag )
		{
			if( !enabled ) return;
			LogEnd( tag );

			var sw = new Stopwatch();
			sw.Start();
			_watches[tag] = sw;
		}

		public static double LogEnd( string tag )
		{
			if( !_watches.TryGetValue( tag, out var osw ) ) return 0;
			_watches.Remove( tag );
			
			if( osw.IsRunning ) osw.Stop();

			var elapsed = osw.Elapsed.TotalMilliseconds;
			var ms = elapsed;
			if( _accTimmings.TryGetValue( tag, out var acc ) ) ms += acc;
			_accTimmings[tag] = ms;

			var calls = 1;
			if( _calls.TryGetValue( tag, out var oldCalls ) ) calls += oldCalls;
			_calls[tag] = calls;

			return elapsed;
		}

		public static void Clear()
		{
			_watches.Clear();
			_accTimmings.Clear();
			_calls.Clear();
		}

		private static readonly StringBuilder SB = new StringBuilder();
		private static System.Comparison<KeyValuePair<string,double>> DESCENDING_COMPARISON = ( KeyValuePair<string, double> a, KeyValuePair<string, double> b ) => b.Value.CompareTo( a.Value );
		// private static System.Comparison<KeyValuePair<string,double>> ASCENDING_COMPARISON = ( KeyValuePair<string, double> a, KeyValuePair<string, double> b ) => b.Value.CompareTo( a.Value );
		public static string GetLog( string msFormat = "F3", string percentageFormat = "F1" )
		{
			var hasLastLogStopwatch = _logStopwatch.IsRunning;
			var totalMs = _logStopwatch.Elapsed.TotalMilliseconds;
			SB.Clear();
			
			if( hasLastLogStopwatch )
			{
				_logStopwatch.Stop();
				SB.Append( "Total time: " );
				SB.Append( totalMs.ToString( msFormat ) );
				SB.Append( "ms\n" );
			}

			var timings = _accTimmings.ToList();
			timings.Sort( DESCENDING_COMPARISON );
			foreach( var kvp in timings )
			{
				SB.Append( "[" );
				var tag = kvp.Key;
				if( _calls.TryGetValue( tag, out var calls ) ) SB.Append( calls );
				SB.Append( "]" );
				SB.Append( tag );
				SB.Append( ":" );

				var ms = kvp.Value;

				SB.Append( ms.ToString( msFormat ) );
				SB.Append( "ms" );

				if( hasLastLogStopwatch )
				{
					SB.Append( " " );
					SB.Append( ( ms * 100 / totalMs ).ToString( percentageFormat ) );
					SB.Append( "%" );
				}

				SB.Append( "\n" );
			}

			var log = SB.ToString();
			SB.Clear();

			_logStopwatch.Reset();
			_logStopwatch.Start();

			return log;
		}
	}

	public static class Specific
	{
		private class LogData
		{
			public readonly string tag;
			public readonly System.DateTime startTime;
			public readonly double duration;

			public LogData( string tag, double elapsedTime )
			{
				this.tag = tag;
				this.duration = elapsedTime;
				startTime = System.DateTime.Now.AddMilliseconds( -elapsedTime );
			}
		}

		private static readonly Dictionary<string,Stopwatch> _watches = new Dictionary<string, Stopwatch>();
		private static readonly List<LogData> _measures = new List<LogData>();

		public static void LogStart( string tag )
		{
			LogEnd( tag );
			ReallyLogStart( tag );
		}

		public static void TryLogStart( string tag )
		{
			if( _watches.TryGetValue( tag, out var osw ) ) return;
			ReallyLogStart( tag );
		}

		private static void ReallyLogStart( string tag )
		{
			var sw = new Stopwatch();
			sw.Start();
			_watches[tag] = sw;
		}

		public static double LogEnd( string tag )
		{
			if( !_watches.TryGetValue( tag, out var osw ) ) return 0;
			_watches.Remove( tag );
			
			if( osw.IsRunning ) osw.Stop();

			var elapsed = osw.Elapsed.TotalMilliseconds;
			var ms = elapsed;
			_measures.Add( new LogData( tag, ms ) );

			return elapsed;
		}

		public static void Clear()
		{
			_watches.Clear();
			_measures.Clear();
		}

		private static readonly StringBuilder SB = new StringBuilder();
		public static string GetLog( string msFormat = "F3" )
		{
			var totalMs = _logStopwatch.Elapsed.TotalMilliseconds;
			SB.Clear();

			if( _watches.Count > 0 )
			{
				SB.Append( "Open measures:" );
				SB.Append( "\n" );
				foreach( var kvp in _watches )
				{
					var tag = kvp.Key;
					SB.Append( tag );
					SB.Append( ":" );
					
					var ms = kvp.Value.Elapsed.TotalMilliseconds;

					SB.Append( ms.ToString( msFormat ) );
					SB.Append( "ms" );

					SB.Append( "\n" );
				}
				SB.Append( "\n" );
			}

			foreach( var measure in _measures )
			{
				SB.Append( measure.tag );
				SB.Append( ":" );
				SB.Append( measure.duration.ToString( msFormat ) );
				SB.Append( "ms" );
				SB.Append( " @ " );
				SB.Append( measure.startTime.TimeOfDay.ToString() );
				SB.Append( "\n" );
			}

			var log = SB.ToString();
			SB.Clear();
			return log;
		}
	}
}
