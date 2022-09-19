using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GladNet
{
	// Based on: https://github.com/dotnet/runtime/blob/77a714d26dc434012c8234447b9e1183ce7be4a6/src/libraries/System.Memory/src/System/Buffers/ArrayMemoryPool.cs
	internal class GladNetPipeMemoryPool : MemoryPool<byte>
	{
		public sealed override int MaxBufferSize => int.MaxValue;

		private ArrayPool<byte> SharedPipePool { get; } = ArrayPool<byte>.Create();

		public sealed override IMemoryOwner<byte> Rent(int minimumBufferSize = -1)
		{
			if(minimumBufferSize == -1)
				minimumBufferSize = 1 + (4095 / Unsafe.SizeOf<byte>());
			else if(((uint)minimumBufferSize) > int.MaxValue)
				throw new InvalidOperationException($"Provided buffer size: {minimumBufferSize} is too high.");

			return new GladNetPipeMemoryPoolBuffer(minimumBufferSize, SharedPipePool);
		}

		protected sealed override void Dispose(bool disposing) { }  // ArrayMemoryPool is a shared pool so Dispose() would be a nop even if there were native resources to dispose.

		// Based on: https://github.com/dotnet/runtime/blob/9768606ea1f0aa1be6098143ded330dadac8cf91/src/libraries/System.Memory/src/System/Buffers/ArrayMemoryPool.ArrayMemoryPoolBuffer.cs
		private sealed class GladNetPipeMemoryPoolBuffer : IMemoryOwner<byte>, IDisposable
		{
			private ArrayPool<byte> Pool { get; }

			private byte[] _array;

			public GladNetPipeMemoryPoolBuffer(int size, ArrayPool<byte> pool)
			{
				Pool = pool ?? throw new ArgumentNullException(nameof(pool));
				_array = Pool.Rent(size);
			}

			public Memory<byte> Memory
			{
				get
				{
					byte[] array = _array;

					if (array is null)
						throw new ObjectDisposedException("array");

					return new Memory<byte>(array);
				}
			}

			public void Dispose()
			{
				byte[] array = _array;
				if(array != null)
				{
					_array = null;
					Pool.Return(array);
				}
			}
		}
	}
}
