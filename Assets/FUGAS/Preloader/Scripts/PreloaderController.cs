using System.Collections;
using UnityEngine;

namespace Assets.FUGAS.Preloader.Scripts
{
    public class PreloaderController : MonoBehaviour
    {
        private static PreloaderController instance;

        public GameObject LandscapePrefab;
        public GameObject PortraitPrefab;

        [HideInInspector] public bool PreloaderUsed;

        public bool UseInDevelop;

        public void Awake()
        {
#if UNITY_EDITOR
            if (!UseInDevelop)
            {
                Destroy(gameObject);
                return;
            }
#endif
            if (instance is null && !PreloaderUsed)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        public void Start()
        {
            if (Screen.width > Screen.height)
                LandscapePrefab = Instantiate(LandscapePrefab, transform);
            else if (Screen.width < Screen.height)
                PortraitPrefab = Instantiate(PortraitPrefab, transform);
            var animator = GetComponentInChildren<Animator>();
            StartCoroutine(PreloaderFadeOut(animator));
        }

        private IEnumerator PreloaderFadeOut(Animator animator)
        {
            yield return new WaitForSeconds(2f);
            animator.SetBool("is_ON", false);
            yield return new WaitForSeconds(3);
            // allow to destroy after intro 
            Destroy(gameObject);
            PreloaderUsed = true;
        }
    }
}