using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
100119
 add Action<float> progressDel
 remove public  Action LoadingCallBack
 130319
 add method  ReLoadCurrentScene()

*/

namespace Mkey
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private SimpleSlider simpleSlider;
        [SerializeField]
        private GuiFader_v2 LoadGroup;
        [SerializeField]
        private bool showLoader;

        private float loadProgress;

        public static SceneLoader Instance;

        #region regular
        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); }
            else
            {
                Instance = this;
            }
        }
        #endregion regular

        public void LoadScene(int scene)
        {
            StartCoroutine(AsyncLoadBeaty(scene,null, null));

        }

        public void LoadScene(int scene, Action completeCallBack)
        {
            StartCoroutine(AsyncLoadBeaty(scene, null, completeCallBack));
        }

        public void LoadScene(int scene, Action<float> progresUpdate, Action completeCallBack)
        {
            StartCoroutine(AsyncLoadBeaty(scene, progresUpdate, completeCallBack));
        }

        public void LoadScene(string sceneName)
        {
            int scene = SceneManager.GetSceneByName(sceneName).buildIndex;
            StartCoroutine(AsyncLoadBeaty(scene,null, null));
        }

        public void ReLoadCurrentScene()
        {
            int scene = SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(AsyncLoadBeaty(scene, null, null));
        }

        private IEnumerator AsyncLoadBeaty(int scene, Action <float> progresUpdate, Action completeCallBack)
        {
            float apprLoadTime = 1f;
            float steps = 100f;
            float loadTime = 0.0f;
            loadProgress = 0;
            if (simpleSlider && showLoader) simpleSlider.value = loadProgress;

            bool fin = false;
            if (LoadGroup && showLoader)
            {
                LoadGroup.gameObject.SetActive(true);
                LoadGroup.FadeIn(0, () => { fin = true; });
            }
            while (LoadGroup && showLoader && !fin)
            {
                yield return null;
            }

            AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
            ao.allowSceneActivation = false;
            float lastTime = Time.time;
            while (!ao.isDone )//&& loadProgress < 0.99f)
            {
                loadTime += (Time.time - lastTime);
                lastTime = Time.time;
                loadProgress = Mathf.Clamp01(loadProgress + 0.01f);
                if (simpleSlider && showLoader) simpleSlider.value = loadProgress;

                if (loadTime >= 0.5f * apprLoadTime && (ao.progress < 0.5f))
                {
                    apprLoadTime *= 1.1f;
                }
                else if (loadTime >= 0.5f * apprLoadTime && (ao.progress > 0.5f))
                {
                    apprLoadTime /= 1.1f;
                }

                if (ao.progress >= 0.90f && !ao.allowSceneActivation && loadProgress >= 0.99f)
                {
                    ao.allowSceneActivation = true;
                }
                progresUpdate?.Invoke(loadProgress);
                // Debug.Log("waite scene: " + loadTime + "ao.progress : " + ao.progress);
                yield return new WaitForSeconds(apprLoadTime / steps); ;
            }
            if (LoadGroup && showLoader) LoadGroup.FadeOut(0, null);
            completeCallBack?.Invoke();
        }
       
    }
}