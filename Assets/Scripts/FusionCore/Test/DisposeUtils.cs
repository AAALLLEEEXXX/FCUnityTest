using System;

namespace FusionCore.Test
{
	public static class DisposeUtils
	{
		public static void DisposeAndSetNull<T>(ref T disposable)
			where T : class, IDisposable
		{
			disposable?.Dispose();
			disposable = null;
		}
	}
}