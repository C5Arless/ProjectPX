using System.Threading;
using System.Threading.Tasks;

public interface IOrchestratedEvent {    
    public int EventIndex { get; }
    public bool IsRepeatable { get; }
    public EventPriority Priority { get; }

    Task FireEvent();
}