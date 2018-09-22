using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;
using JetBrains.Annotations;
using Reinterpret.Net;

namespace GladNet
{
	public sealed class FileLogger : AbstractSimpleLogger
	{
		/// <summary>
		/// The file to log to.
		/// </summary>
		public string FileName { get; }

		private byte[] NewLineBytes { get; } = System.Environment.NewLine.Reinterpret();

		/// <inheritdoc />
		public FileLogger([NotNull] string fileName, LogLevel logLevel)
			: base("Core", logLevel, true, false, false, String.Empty)
		{
			FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
		}

		public FileLogger()
			: base("Core", LogLevel.All, true, false, false, String.Empty)
		{
			FileName = $"Log/Log_{Guid.NewGuid()}.txt";

			if(!Directory.Exists("Log"))
				Directory.CreateDirectory("Log");
		}

		/// <inheritdoc />
		protected override void WriteInternal(LogLevel level, object message, Exception exception)
		{
			using(FileStream fs = new FileStream(FileName, FileMode.Append | FileMode.OpenOrCreate, FileAccess.Write))
			{
				if(message is string m)
				{
					byte[] bytes = m.Reinterpret(Encoding.Unicode);
					fs.Write(bytes, 0, bytes.Length);
					fs.Write(NewLineBytes, 0, NewLineBytes.Length);
				}
			}
		}
	}
}
