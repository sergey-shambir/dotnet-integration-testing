using System.Collections.Concurrent;

namespace WebService.Specs.Fixture;

public class TestServerHostPool(int size) : IAsyncDisposable
{
    private readonly ConcurrentQueue<TestServerHost> _freeHosts = [];
    private readonly Semaphore _semaphore = new(initialCount: size, maximumCount: size);
    private int _lastInstanceId;

    public TestServerHost Acquire()
    {
        _semaphore.WaitOne();

        if (!_freeHosts.TryDequeue(out TestServerHost? host))
        {
            host = new TestServerHost(Interlocked.Increment(ref _lastInstanceId));
        }

        return host;
    }

    public void Release(TestServerHost host)
    {
        _freeHosts.Enqueue(host);
        _semaphore.Release();
    }

    public async ValueTask DisposeAsync()
    {
        while (_freeHosts.TryDequeue(out TestServerHost? host))
        {
            await host.DisposeAsync();
        }

        _semaphore.Dispose();
    }
}