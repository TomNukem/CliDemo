
public interface IMyService
{
    Task PerformLongTaskAsync(CancellationToken cancellationToken);
}
