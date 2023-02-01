using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class ScreenshotMaker : MonoBehaviour
    {
        public VRGrabTrigger trigger;
        public string filename;
        public int screenshotMultiplier = 1;
        public AudioClip soundEffect;

        private float lastTake = -1f;
        private float currentTime = 0f;
        private float COOLDOWN = 1f;

        // Update is called once per frame
        void Update()
        {
            currentTime = Time.time;
            if (Time.time > lastTake + COOLDOWN && trigger.Triggered())
            {
                StartCoroutine(captureScreenshot());
                
            }
        }
        
        private IEnumerator captureScreenshot()
        {
            yield return new WaitForEndOfFrame();

            string path = Application.persistentDataPath + filename + "_" + System.DateTime.Now.ToString("MM-dd-yy_hh-mm-ss") + ".png";

            Camera cam = GetComponent<Camera>();
            if(cam == null)
            {
#if UNITY_2017_1_OR_NEWER
                ScreenCapture.CaptureScreenshot(path, screenshotMultiplier);
#else
                Application.CaptureScreenshot(path, screenshotMultiplier);
#endif
            } else
            {
                //read the texture from camera and save it in my Texture2D object
                Texture2D snapShot = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
                RenderTexture snapShotRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
                RenderTexture.active = snapShotRT;
                cam.targetTexture = snapShotRT;
                cam.Render();

                snapShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                snapShot.Apply();
                //release
                RenderTexture.active = null;
                cam.targetTexture = null;
                //Convert to png
#if UNITY_2017_1_OR_NEWER
                byte[] imageBytes = ImageConversion.EncodeToPNG(snapShot);
#else
                byte[] imageBytes = snapShot.EncodeToPNG(); //ImageConversion.EncodeToPNG(texture);
#endif
                //Save image to file
                System.IO.File.WriteAllBytes(path, imageBytes);
            }
            
            Invoke("flashEffectDelayed", 0.2f); // must be delayed to avoid the screenshot to include the effect

        }
        private void flashEffectDelayed()
        {
            lastTake = currentTime;
            if (soundEffect != null)
            {
                AudioSource.PlayClipAtPoint(soundEffect, transform.position);
            }
        }
        private Shader shader;
        private Material m_Material;


        void Start()
        {
            shader = Shader.Find("VREasy/ColourImage");
            if(shader == null)
            {
                Debug.LogError("VREasy/ColourImage shader not found! Needed for screenshot effect");
                enabled = false;
                return;
            }
#if UNITY_2017_1_OR_OLDER
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                return;
            }
#endif

            // Disable the image effect if the shader can't
            // run on the users graphics card
            if (!shader || !shader.isSupported)
                enabled = false;
        }


        private Material material
        {
            get
            {
                if (m_Material == null)
                {
                    m_Material = new Material(shader);
                    m_Material.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_Material;
            }
        }


        void OnDisable()
        {
            if (m_Material)
            {
                DestroyImmediate(m_Material);
            }
        }

        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            material.SetFloat("_Value", Mathf.Clamp01(COOLDOWN - (currentTime - lastTake)));
            Graphics.Blit(source, destination, material);
        }
    }
}