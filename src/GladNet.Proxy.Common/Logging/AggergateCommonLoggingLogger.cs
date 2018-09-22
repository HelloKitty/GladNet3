using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;
using JetBrains.Annotations;

namespace GladNet
{
	/// <summary>
	/// Aggerates a collection of <see cref="ILog"/> and
	/// uses them in aggergate to log messages.
	/// </summary>
	public sealed class AggergateCommonLoggingLogger : AbstractSimpleLogger
	{
		private ILog[] Loggers { get; }

		public AggergateCommonLoggingLogger([NotNull] params ILog[] loggers)
			: base("Core", LogLevel.All, true, false, false, String.Empty)
		{
			if(loggers == null) throw new ArgumentNullException(nameof(loggers));
			Loggers = loggers;
		}

		public AggergateCommonLoggingLogger([NotNull] IEnumerable<ILog> loggers)
			: base("Core", LogLevel.All, true, false, false, String.Empty)
		{
			if(loggers == null) throw new ArgumentNullException(nameof(loggers));
			Loggers = loggers.ToArray();
		}

		/// <inheritdoc />
		protected override void WriteInternal(LogLevel level, object message, Exception exception)
		{
			foreach(ILog log in Loggers)
				switch(level)
				{
					case LogLevel.Trace:
						if(log.IsTraceEnabled)
							log.Trace(message, exception);
						break;
					case LogLevel.Debug:
						if(log.IsDebugEnabled)
							log.Debug(message, exception);
						break;
					case LogLevel.Info:
						if(log.IsInfoEnabled)
							log.Info(message, exception);
						break;
					case LogLevel.Warn:
						if(log.IsWarnEnabled)
							log.Warn(message, exception);
						break;
					case LogLevel.Error:
						if(log.IsErrorEnabled)
							log.Error(message, exception);
						break;
					case LogLevel.Fatal:
						if(log.IsFatalEnabled)
							log.Fatal(message, exception);
						break;
					case LogLevel.Off:
						break;
				}
		}
	}
}