using System.Collections.Concurrent;

namespace StayWize.Application.Common.Services;

public class ReservationConcurrencyService
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _propertyLocks = new();
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _hostLocalLocks = new();

    public async Task<IDisposable> AcquirePropertyLockAsync(Guid propertyId)
    {
        var semaphore = _propertyLocks.GetOrAdd(propertyId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        return new SemaphoreReleaser(semaphore);
    }

    public async Task<IDisposable> AcquireHostLocalLockAsync(Guid hostLocalId)
    {
        var semaphore = _hostLocalLocks.GetOrAdd(hostLocalId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        return new SemaphoreReleaser(semaphore);
    }

    private class SemaphoreReleaser : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        public SemaphoreReleaser(SemaphoreSlim semaphore) => _semaphore = semaphore;
        public void Dispose() => _semaphore.Release();
    }
}