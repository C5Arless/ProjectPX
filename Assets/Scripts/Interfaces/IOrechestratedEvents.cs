using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IOrchestratedEvent {
    [SerializeField] int EventIndex { get; }
    [SerializeField] bool IsRepeatable { get; }
    [SerializeField] EventPriority Priority { get; }

    Task FireEvent(CancellationToken token);
}