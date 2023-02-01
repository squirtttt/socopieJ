using UnityEngine;
using System.Collections;
using System;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    public class StereoscopicViewer : MonoBehaviour
    {

        public Texture2D LeftEyeImage {
            get
            {
                return leftEyeImage;
            }
            set
            {
                leftEyeImage = value;
                try
                {
                    Transform leftImage = transform.Find("Image_left");
                    if(autodetectImageSize && leftEyeImage != null) leftImage.localScale = (Vector3.up + Vector3.right * leftEyeImage.width / leftEyeImage.height + Vector3.forward) * imageScale;
                    Material m = leftImage.GetComponent<Renderer>().sharedMaterial;
                    if (m == null)
                    {
                        m = Resources.Load<Material>("left_eye");
                        leftImage.GetComponent<Renderer>().sharedMaterial = m;
                    }
                    m.mainTexture = leftEyeImage;
                } catch(System.Exception e)
                {
                    Debug.LogError("[VREasy] StereoscopicViewer: could not find left image within the hierarchy. Please use the Stereoscopic prefab provided. " + e.ToString());
                }
            }
        }
        public Texture2D leftEyeImage;

        public Texture2D RightEyeImage
        {
            get
            {
                return rightEyeImage;
            }
            set
            {
                rightEyeImage = value;
                try
                {
                    Transform rightImage = transform.Find("Image_right");
                    if (autodetectImageSize && rightEyeImage != null) rightImage.localScale = (Vector3.up + Vector3.right * rightEyeImage.width / rightEyeImage.height + Vector3.forward) * imageScale;
                    Material m = rightImage.GetComponent<Renderer>().sharedMaterial;
                    if (m == null)
                    {
                        m = Resources.Load<Material>("right_eye");
                        rightImage.GetComponent<Renderer>().sharedMaterial = m;
                    }
                    m.mainTexture = rightEyeImage;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[VREasy] StereoscopicViewer: could not find right image within the hierarchy. Please use the Stereoscopic prefab provided. " + e.ToString());
                }
            }
        }
        public Texture2D rightEyeImage;

#if UNITY_5_6_OR_NEWER
        public VideoClip LeftEyeClip
        {
            get
            {
                return leftEyeClip;
            }
            set
            {
                leftEyeClip = value;
                SetVideoPlayers();
            }
        }
        public VideoClip leftEyeClip;

        public VideoClip RightEyeClip
        {
            get
            {
                return rightEyeClip;
            }
            set
            {
                rightEyeClip = value;
                SetVideoPlayers();
            }
        }
        public VideoClip rightEyeClip;
#endif

        public float ImageScale
        {
            get
            {
                return imageScale;
            }
            set
            {
                imageScale = value;
                if (autodetectImageSize)
                {
                    if(rightEyeImage != null) RightEyeImage = rightEyeImage;
                    if(leftEyeImage != null) LeftEyeImage = leftEyeImage;
                }
            }
        }
        public float imageScale = 1f;

        public STEREOSCOPIC_MODE mode = STEREOSCOPIC_MODE.IMAGE;
        public int leftEyeLayer;
        public int rightEyeLayer;
        public bool autodetectImageSize = true;


        // Use this for initialization
        void Awake()
        {
            Transform leftEye = transform.Find("Main Camera_left");
            Transform rightEye = transform.Find("Main Camera_right");
            
            if (leftEye == null || rightEye == null)
            {
                Debug.LogError("[VREasy] StereoscopicViewer: could not find left or right camera/image within the hierarchy. Please use the Stereoscopic prefab provided");
                return;
            }
            leftEye.gameObject.layer = leftEyeLayer;
            rightEye.gameObject.layer = rightEyeLayer;
            
            try
            {
                Camera left = leftEye.GetComponent<Camera>();
                left.stereoTargetEye = StereoTargetEyeMask.Left;
                left.cullingMask = ~((1 << rightEyeLayer) & int.MaxValue);
                Camera right = rightEye.GetComponent<Camera>();
                right.stereoTargetEye = StereoTargetEyeMask.Right;
                right.cullingMask = ~((1 << leftEyeLayer) & int.MaxValue);
            } catch(System.Exception e)
            {
                Debug.LogError("[VREasy] StereoscopicViewer: could not find Cameras in left or right eyes. Please use the Stereoscopic prefab provided. " + e.ToString());
                return;
            }

            RightEyeImage = rightEyeImage;
            LeftEyeImage = leftEyeImage;

        }

        public void ClearVideoPlayers()
        {
#if UNITY_5_6_OR_NEWER
#if UNITY_EDITOR
            Transform rightClip = transform.Find("Image_right");
            DestroyImmediate(rightClip.GetComponent<VideoPlayer>());
            DestroyImmediate(rightClip.GetComponent<AudioSource>());
            Transform leftClip = transform.Find("Image_left");
            DestroyImmediate(leftClip.GetComponent<VideoPlayer>());
            DestroyImmediate(leftClip.GetComponent<AudioSource>());
#endif
#endif
        }

        public void SetVideoPlayers()
        {
#if UNITY_5_6_OR_NEWER

            try
            {
                Transform rightClip = transform.Find("Image_right");
                VideoPlayer pl = rightClip.GetComponent<VideoPlayer>();
                if (pl == null)
                    pl = rightClip.gameObject.AddComponent<VideoPlayer>();
                pl.clip = rightEyeClip;
                AudioSource audio = rightClip.GetComponent<AudioSource>();
                if (audio == null)
                    audio = rightClip.gameObject.AddComponent<AudioSource>();
                pl.SetTargetAudioSource(0, audio);
               
                Transform leftClip = transform.Find("Image_left");
                pl = leftClip.GetComponent<VideoPlayer>();
                if (pl == null)
                    pl = leftClip.gameObject.AddComponent<VideoPlayer>();
                pl.clip = leftEyeClip;
                /*audio = leftClip.GetComponent<AudioSource>();
                if (audio == null)
                    audio = leftClip.gameObject.AddComponent<AudioSource>();
                pl.SetTargetAudioSource(0, audio);*/
            }
            catch (System.Exception e)
            {
                Debug.LogError("[VREasy] StereoscopicViewer: could not find right/left video player within the hierarchy. Please use the Stereoscopic prefab provided. " + e.ToString());
            }
#endif
        }

    }
}