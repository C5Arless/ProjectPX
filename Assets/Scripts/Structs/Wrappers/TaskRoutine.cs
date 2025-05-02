using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskRoutine {
    public static Task RunCoroutineAsync(MonoBehaviour runner, IEnumerator coroutine) {
        var tcs = new TaskCompletionSource<object>();

        runner.StartCoroutine(WrapCoroutine(coroutine, tcs));

        return tcs.Task;
    }

    private static IEnumerator WrapCoroutine(IEnumerator coroutine, TaskCompletionSource<object> tcs) {
        yield return coroutine;
        tcs.SetResult(null);
    }
}
