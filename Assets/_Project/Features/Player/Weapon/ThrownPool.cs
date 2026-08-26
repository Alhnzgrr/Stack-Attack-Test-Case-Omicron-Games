using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace StackAttack.Player
{
    // Everything the player throws is spawned and dropped several times a second, so
    // it is pooled rather than instantiated. The pool also keeps the live ones, which
    // is what lets the field sweep the air clear in one call between levels.
    public class ThrownPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Transform _root;
        private readonly ObjectPool<T> _pool;
        private readonly List<T> _active = new List<T>();
        private readonly Dictionary<T, TrailRenderer[]> _trails = new Dictionary<T, TrailRenderer[]>();

        public ThrownPool(T prefab, Transform root, int capacity)
        {
            _prefab = prefab;
            _root = root;

            _pool = new ObjectPool<T>(Create, OnTake, OnReturn, Destroy, true, capacity);
        }

        public T Take(Vector3 position)
        {
            T item = _pool.Get();

            item.transform.position = position;
            item.transform.rotation = Quaternion.identity;

            // A trail keeps its points in world space, so one that came back from a
            // hit high up the screen would draw a line from there down to the muzzle
            // the moment it is thrown again. Cleared after the move, never before.
            TrailRenderer[] trails = _trails[item];

            for (int i = 0; i < trails.Length; i++)
                trails[i].Clear();

            return item;
        }

        // The active list is the guard as well as the bookkeeping: a rocket that runs
        // out of life on the same frame it touches a stack asks to go back twice, and
        // the second ask finds it already gone.
        public bool Release(T item)
        {
            if (!_active.Remove(item))
                return false;

            _pool.Release(item);
            return true;
        }

        public void Clear()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                _pool.Release(_active[i]);

            _active.Clear();
        }

        private T Create()
        {
            T item = Object.Instantiate(_prefab, _root);

            _trails.Add(item, item.GetComponentsInChildren<TrailRenderer>(true));

            return item;
        }

        private void OnTake(T item)
        {
            _active.Add(item);
            item.gameObject.SetActive(true);
        }

        private void OnReturn(T item)
        {
            item.gameObject.SetActive(false);
        }

        private void Destroy(T item)
        {
            _trails.Remove(item);
            Object.Destroy(item.gameObject);
        }
    }
}
