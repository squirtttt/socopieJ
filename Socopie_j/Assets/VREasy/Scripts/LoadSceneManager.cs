using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if BATTLE_HUD_SDK
using Battlehub.RTCommon;
#endif

namespace VREasy
{
    // USER SHOULD ADD A FADE IMAGE TO THE SCENE -IMAGE COMPONENT BLOCKING THE VR CAMERA VIEW
    // IF ONE IS NOT PROVIDED, A DEFAULT ONE IS CREATED
    public class LoadSceneManager : MonoBehaviour
    {
        public static LoadSceneManager instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject ob = new GameObject();
                    ob.name = "[VREasy]LoadSceneManager";
                    //Debug.Log("LoadSceneManager was not detected on your scene, one has been added. To avoid seeing this message, add a LoadSceneManager object to your scene");
                    _instance = ob.AddComponent<LoadSceneManager>();
                    DontDestroyOnLoad(ob);
                }
#if BATTLE_HUD_SDK
                _instance.Init();
                _instance.FadeIn(0);
#endif
                return _instance;
            }
        }
        private static LoadSceneManager _instance = null;

        private Dictionary<int, Vector3> lastScenePositions = new Dictionary<int, Vector3>();
        private string playerName;
        private bool hasFuturePosition = false;
        private Vector3 futurePosition;

        void Awake()
        {
            Init();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Init()
        {
            m_FadeOutColor = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 0f);
            bool check = true;
            try
            {
                check = m_FadeImage == null; // check for null
                if (m_FadeImage != null) m_FadeImage.color = m_FadeImage.color; // check for MissingReference
            }
#pragma warning disable 0168
            catch (MissingReferenceException e)
#pragma warning restore 0168
            {
                check = false;
            }
            if (check)
            {
                try
                {
                    GameObject c = new GameObject();
                    c.name = "[vreasy]Canvas";
                    c.transform.parent = VREasy_utils.GetMainCameraTransform();
                    c.transform.localScale = Vector3.one;
                    c.transform.localPosition = Vector3.zero;
                    Canvas canvas = c.AddComponent<Canvas>();
                    canvas.scaleFactor = 1;
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    Camera cam = VREasy_utils.GetMainCamera();
                    if(cam != null) {
                        canvas.worldCamera = cam;
                        canvas.planeDistance = cam.nearClipPlane * 1.01f;
                    }
                    
                    GameObject i = new GameObject();
                    i.name = "[vreasy]FadeImage";
                    i.transform.parent = canvas.transform;
                    i.transform.localScale = Vector3.one;
                    i.transform.localPosition = Vector3.zero;
                    i.transform.localRotation = Quaternion.identity;
                    m_FadeImage = i.AddComponent<Image>();
                    Sprite fadeSprite = Resources.Load("black_dot", typeof(Sprite)) as Sprite;
                    m_FadeImage.sprite = fadeSprite;
                    m_FadeImage.rectTransform.anchorMin = Vector2.zero;
                    m_FadeImage.rectTransform.anchorMax = Vector2.one;
                    m_FadeImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    m_FadeImage.rectTransform.localScale = new Vector3(2f, 1f, 1f);
                    m_FadeImage.color = m_FadeColor;

                    //Debug.Log("Fade image not found: Added default fade image");
                }
                catch (System.Exception e)
                {
                    Debug.Log("No default sprite for FadeOut was found! " + e.ToString());
                }
            } else
            {
                // check if the m_FadeImage is pointing at the right Image component (the one within the object's hierarchy)
                bool needsRemapping = true;
                Transform parent = m_FadeImage.transform.parent;
                while(parent != null) {
                    if (parent == transform)
                    {
                        needsRemapping = false;
                        break;
                    }
                    parent = parent.parent;
                }
                if(needsRemapping)
                {
                    m_FadeImage = GameObject.Find("[vreasy]FadeImage").GetComponent<Image>();

                }
            }
            if(m_FadeImage != null) m_FadeImage.enabled = true;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void LoadScene(string sceneToLoad, bool remember, Transform player, bool hasFuturePos, Vector3 futurePos)
        {
            setFuturePosition(hasFuturePos, futurePos, player);
            rememberLastLocation(remember, player);
            StartCoroutine(loadAsync(sceneToLoad,m_FadeDuration,m_FadeOut));
        }

        public void LoadScene(string sceneToLoad, bool remember, Transform player, bool hasFuturePos, Vector3 futurePos, float timeToLoad, bool doFadeOut)
        {
            setFuturePosition(hasFuturePos, futurePos, player);
            rememberLastLocation(remember, player);
            StartCoroutine(loadAsync(sceneToLoad,timeToLoad,doFadeOut));
        }

        private void setFuturePosition(bool hasFuturePos, Vector3 futurePos, Transform player)
        {
            hasFuturePosition = hasFuturePos;
            futurePosition = futurePos;
            if (player != null) playerName = player.name;
        }

        private void rememberLastLocation(bool remember, Transform player)
        {
            if (!remember) return;
#if UNITY_2019_2_OR_NEWER
            System.Collections.Generic.List<UnityEngine.XR.XRNodeState> nodeStates = new System.Collections.Generic.List<UnityEngine.XR.XRNodeState>();
            UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);
            UnityEngine.XR.XRNodeState nodeState = new UnityEngine.XR.XRNodeState();

            foreach (var state in nodeStates)
            {
                if (state.nodeType == UnityEngine.XR.XRNode.Head)
                {
                    nodeState = state;
                }
            }

            Vector3 nodePosition = new Vector3();
            nodeState.TryGetPosition(out nodePosition);

            lastScenePositions[SceneManager.GetActiveScene().buildIndex] = player.position - nodePosition;

#elif UNITY_2017_2_OR_NEWER
            lastScenePositions[SceneManager.GetActiveScene().buildIndex] = player.position - UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.Head);
#else
            lastScenePositions[SceneManager.GetActiveScene().buildIndex] = player.position - UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head);
#endif
            if (player != null) playerName = player.name;
        }

        private IEnumerator loadAsync(string sceneToLoad,float timeToLoad, bool doFadeOut)
        {
            if (doFadeOut)
            {
                FadeOut(timeToLoad);
                yield return new WaitForSeconds(timeToLoad);
            }
            // load scene
            int scene;
            AsyncOperation async;
            if (Int32.TryParse(sceneToLoad, out scene))
            {
                async = SceneManager.LoadSceneAsync(scene);
            }
            else
            {
                async = SceneManager.LoadSceneAsync(sceneToLoad);
            }
            // progress bar
            while (!async.isDone)
            {
                //setLoadProgress(async.progress);
                yield return async.isDone;
            }
        }
        /////////////
        // VR Fade
        ////////////
        public Image m_FadeImage;                     // Reference to the image that covers the screen.
        private Color m_FadeColor = Color.black;       // The colour the image fades out to.
        public float m_FadeDuration = 2.0f;           // How long it takes to fade in seconds.
        public bool m_FadeInOnSceneLoad = false;      // Whether a fade in should happen as soon as the scene is loaded.
        public bool m_FadeOut = true;                // Whether a fade out should happen when scene closes.


        private bool m_IsFading;                                        // Whether the screen is currently fading.
        private float m_FadeStartTime;                                  // The time when fading started.
        private Color m_FadeOutColor;                                   // This is a transparent version of the fade colour, it will ensure fading looks normal.

        
        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Init();
            // If applicable set the immediate colour to be faded out and then fade in.
            if (m_FadeInOnSceneLoad)
            {
                if(m_FadeImage != null) m_FadeImage.color = m_FadeColor;
                FadeIn(m_FadeDuration);
            }
            else
            {
                if (m_FadeImage != null) m_FadeImage.color = new Color(0, 0, 0, 0.0f);
            }

            if(hasFuturePosition)
            {
                // how to get a reference to the player?
                GameObject pl = GameObject.Find(playerName);
                if (pl != null)
                {
                    pl.transform.position = futurePosition; 
                }
                hasFuturePosition = false;
            } else
            {
                // check if position needs to be restored
                if (lastScenePositions.ContainsKey(scene.buildIndex))
                {
                    // how to get a reference to the player?
                    GameObject pl = GameObject.Find(playerName);
                    if (pl != null)
                    {
                        pl.transform.position = lastScenePositions[scene.buildIndex]; // new Vector3(lastScenePositions[scene.buildIndex].x,pl.transform.position.y, lastScenePositions[scene.buildIndex].z);
                    }
                    lastScenePositions.Remove(scene.buildIndex);
                }
            }
            
        }

        public void FadeOut(float duration)
        {
            // If not already fading start a coroutine to fade from the fade out colour to the fade colour.
            if (m_IsFading)
                return;
            StartCoroutine(BeginFade(m_FadeOutColor, m_FadeColor, duration));
        }


        public void FadeIn(float duration)
        {
            // If not already fading start a coroutine to fade from the fade colour to the fade out colour.
            if (m_IsFading)
                return;
            StartCoroutine(BeginFade(m_FadeColor, m_FadeOutColor, duration));
        }

        private IEnumerator BeginFade(Color startCol, Color endCol, float duration)
        {
            m_IsFading = true;

            float timer = 0f;
            while (timer <= duration)
            {
                if (m_FadeImage != null) m_FadeImage.color = Color.Lerp(startCol, endCol, timer / duration);
                
                timer += Time.deltaTime;
                yield return null;
            }
            m_IsFading = false;
        }
    }
}
