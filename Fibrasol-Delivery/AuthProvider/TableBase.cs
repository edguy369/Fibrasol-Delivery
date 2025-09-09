
namespace Fibrasol_Delivery.AuthProvider;

public abstract class TableBase : IDisposable
{
    private bool _disposed = false;

    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }
    protected abstract void OnDispose();


    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            // Free any other managed objects here.
            OnDispose();
        }
        _disposed = true;
    }
}
