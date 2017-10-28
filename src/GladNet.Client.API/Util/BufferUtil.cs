using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GladNet
{
	internal static class BufferUtil
	{
		//TODO: Doc
		//For perf we trust that the caller is providing sane args
		/// <summary>
		/// Copies from source to destination in a high performance unsafe inlined manner. Arguments aren't validated
		/// so it is CRITICAL that you validate beforehand.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="sourceOffset"></param>
		/// <param name="destination"></param>
		/// <param name="destinationOffset"></param>
		/// <param name="count"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe void QuickUnsafeCopy(byte[] source, int sourceOffset, byte[] destination, int destinationOffset, int count)
		{
			//See: https://github.com/neuecc/MessagePack-CSharp/issues/117
			//See: https://github.com/dotnet/coreclr/issues/2430
			fixed (byte* overflowPtr = &source[sourceOffset])
			fixed (byte* bufferPtr = &destination[destinationOffset])
			{
				//This is faster than BlockCopy almost all the time
				Buffer.MemoryCopy(overflowPtr, bufferPtr, count, count);
			}
		}
	}
}
