using System;

namespace DistributedDocs.Utils
{
	public abstract class Disposable : IDisposable
	{
		private bool _isDisposed;
		
		~Disposable()
		{
			DisposeInternal();
		}

		protected abstract void ReleaseResources();

		private void DisposeInternal()
		{
			if (!_isDisposed)
			{
				ReleaseResources();
			}

			_isDisposed = true;
		}

		public void Dispose()
		{
			DisposeInternal();
			GC.SuppressFinalize(this);
		}
	}
}
