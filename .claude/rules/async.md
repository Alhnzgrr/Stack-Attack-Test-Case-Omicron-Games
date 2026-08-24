# Async

- Use **UniTask** for all async work. Do not use coroutines
  (`StartCoroutine`/`IEnumerator`). Do not use `System.Threading.Tasks.Task`.
- Every async method takes and honors a **`CancellationToken`** tied to the
  object's lifecycle (e.g. `this.GetCancellationTokenOnDestroy()`).
- **No `async void`**, except Unity event handler methods that the engine calls.

## Example

        public async UniTask LoadAsync(CancellationToken ct)
        {
            await UniTask.Delay(500, cancellationToken: ct);
        }
