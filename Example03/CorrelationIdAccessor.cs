namespace Example03;

public interface ICorrelationIdAccessor
{
    string CorrelationId { get; set; }
}

public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private static readonly AsyncLocal<string> LocalCorrelationId = new AsyncLocal<string>();

    public string CorrelationId
    {
        get => LocalCorrelationId.Value;
        set => LocalCorrelationId.Value = value;
    }
}