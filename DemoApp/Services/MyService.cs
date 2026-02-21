public class MyService : IMyService
{
    public async Task PerformLongTaskAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(50000, cancellationToken);
    }
}