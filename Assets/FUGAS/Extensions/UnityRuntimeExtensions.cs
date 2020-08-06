using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.FUGAS.Extensions
{
    public static class UnityRuntimeExtensions
    {
        /// <summary>
        /// Searches for child 1 level with concrete name including inactive game objects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetChildWithName(this GameObject obj, string name)
        {
            var res = obj.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.gameObject.name == name);
            return res != default ? res.gameObject : default;
        }

        /// <summary>
        /// Returns all children (1 level) of specified <see cref="GameObject"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>children enumerable</returns>
        public static IEnumerable<GameObject> GetChildren(this GameObject obj)
        {
            return from Transform o in obj.transform select o.gameObject;
        }

        /// <summary>
        /// Returns all children (1 level) where name starts with '<see cref="namePartial"/>'
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="namePartial"></param>
        /// <returns></returns>
        public static IEnumerable<GameObject> GetChildrenNameStartsWith(this GameObject obj, string namePartial)
        {
            return from Transform o in obj.transform
                   where o.name.StartsWith(namePartial, StringComparison.InvariantCultureIgnoreCase)
                   select o.gameObject;
        }

        /// <summary>
        /// Returns self and children (1 level)
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static IEnumerable<GameObject> GetSelfAndChildren(GameObject arg)
        {
            yield return arg;
            foreach (Transform o in arg.transform)
                yield return o.gameObject;
        }

        /// <summary>
        /// Returns all levels childs recursively
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<GameObject> GetChildrenRec(this GameObject obj)
        {
            foreach (Transform o in obj.transform)
                foreach (var c in GetChildrenRec(o.gameObject))
                {
                    yield return c.gameObject;
                }

            yield return obj;
        }

        /// <summary>
        /// Applies function to first char in string 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string FirstCharMod(this string source, Func<char, char> action)
        {
            return action(source[0]) + source.Substring(1);
        }

        /// <summary>
        /// Interpolates rotation of <see cref="target"/> by given duration
        /// </summary>
        /// <param name="core"><see cref="MonoBehaviour"/> to run <see cref="Coroutine"/></param>
        /// <param name="target">target game object</param>
        /// <param name="currentPosition"></param>
        /// <param name="primaryPosition"></param>
        /// <param name="onEnd">Optional final action</param>
        /// <param name="duration">Effect duration</param>
        /// <returns></returns>
        public static Coroutine Interpolate(this MonoBehaviour core, GameObject target, Quaternion currentPosition, Quaternion primaryPosition, Action onEnd = default, float duration = 1)
        {
            IEnumerator InterpolateCore()
            {
                var fadeOutTime = duration;
                for (float t = 0.01f; t < fadeOutTime;)
                {
                    t += Time.deltaTime;
                    t = Math.Min(t, fadeOutTime);
                    target.transform.rotation = Quaternion.Lerp(currentPosition, primaryPosition, t);
                    yield return null;
                }

                onEnd?.Invoke();
            }

            return core.StartCoroutine(InterpolateCore());
        }

        /// <summary>
        /// Interpolates linear movement of <see cref="target"/> by given duration
        /// </summary>
        /// <param name="core"><see cref="MonoBehaviour"/> to run <see cref="Coroutine"/></param>
        /// <param name="target">target game object</param>
        /// <param name="currentPosition"></param>
        /// <param name="primaryPosition"></param>
        /// <param name="onEnd">Optional final action</param>
        /// <param name="duration">Effect duration</param>
        /// <returns></returns>
        public static Coroutine Interpolate(this MonoBehaviour core, GameObject target, Vector3 currentPosition, Vector3 primaryPosition, Action onEnd = default, float duration = 1)
        {
            IEnumerator InterpolateCore()
            {
                var fadeOutTime = duration;
                for (float t = 0.01f; t < fadeOutTime;)
                {
                    t += Time.deltaTime;
                    t = Math.Min(t, fadeOutTime);
                    target.transform.position = Vector3.Lerp(currentPosition, primaryPosition, t);
                    yield return null;
                }

                onEnd?.Invoke();
            }

            return core.StartCoroutine(InterpolateCore());
        }
    }
}
