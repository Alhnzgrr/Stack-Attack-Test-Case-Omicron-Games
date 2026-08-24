# Components (same GameObject)

- Components on the **same GameObject** are obtained with `GetComponent`,
  `GetComponentInChildren`, or `GetComponentInParent` — **not** `[SerializeField]`.
- Cache them **once** in `Awake` (or `Start`). **Never** call `GetComponent*`,
  `Camera.main`, or `Find*` inside `Update`/`FixedUpdate`/`LateUpdate`.
- Guarantee presence with `[RequireComponent(typeof(X))]` so the cached lookup can
  never be null (this is what makes the no-null-check rule safe here).

## Example

        [RequireComponent(typeof(Rigidbody))]
        public class Mover : MonoBehaviour
        {
            private Rigidbody _rigidbody;

            private void Awake()
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }
