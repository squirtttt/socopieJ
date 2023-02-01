using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace VREasy
{
    public class LoadSceneAction : VRAction
    {
        public UnityEngine.Object SceneObject
        {
            get
            {
                return sceneObject;
            }
            set
            {
                sceneObject = value;
                if (sceneObject != null) sceneToLoad = sceneObject.name;
            }
        }
        public bool useCustomParameters = false;
        public UnityEngine.Object sceneObject;
        public string sceneToLoad;
        public bool rememberLastLocation = false;
        public bool hasFuturePosition = false;
        public Vector3 futurePosition;

        public Transform Player
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }
        public Transform player;

        // Custom loading parameters (time to load, fade out)
        public float timeToLoad;
        public bool doFadeOut;

        private LoadSceneManager _loadSceneManager;

        public override void Trigger()
        {
            if(string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log("LoadSceneAction: Scene object not set.");
                return;
            }
            
            if (useCustomParameters)
                LoadSceneManager.instance.LoadScene(sceneToLoad, rememberLastLocation, Player, hasFuturePosition, futurePosition, timeToLoad, doFadeOut);
            else
                LoadSceneManager.instance.LoadScene(sceneToLoad, rememberLastLocation, Player, hasFuturePosition, futurePosition);
        }

        void Awake()
        {
            // Ensure LoadSceneManager script is present in the scene
            //_loadSceneManager = LoadSceneManager.instance;
        }
        
    }
}