using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

namespace VREasy
{
    public class AddVRelementHelper : EditorWindow
    {

        [MenuItem("VREasy/VR Element")]
        public static void ShowWindow()
        {
            GetWindow(typeof(AddVRelementHelper), false, "VR Element");
        }
        
        // VR elements
        private VRELEMENT_TYPE _elementType = VRELEMENT_TYPE.BUTTON_2D;
        // VR2DBUTTON
        private VR2DButton _vr2DButton;
        // VROBJECTBUTTON
        private VR3DButton _vrObjectButton;
        private GameObject _VRObjectButtonModel
        {
            get
            {
                return _vrObjectButtonModel;
            }
            set
            {
                if (_vrObjectButtonModel != value)
                {
                    if (_vrObjectButton != null)
                    {
                        DestroyImmediate(_vrObjectButton.gameObject);
                    }
                    _colliderMesh = null;
                    _vrObjectButtonModel = value;
                    if (_vrObjectButtonModel != null)
                    {
                        hasCollider = _vrObjectButtonModel.GetComponent<Collider>();
                        MeshFilter filter = _vrObjectButtonModel.GetComponent<MeshFilter>();
                        if (filter != null)
                            _colliderMesh = filter.sharedMesh;
                    }
                }
            }
        }
        private GameObject _vrObjectButtonModel;
        private bool hasCollider = false;
        private VRBUTTON_COLLIDER_TYPE _collider_type = VRBUTTON_COLLIDER_TYPE.BOX;
        private Mesh _colliderMesh;
        private bool _vrObjectButton_isInstantiated = false;

        // VR GUI BUTTON
        private GameObject _guiButtonModel = null;

        // DISPLAY BUTTON //
        private VRDisplayButton _displayButton = null;

        // TOGGLE GROUP //
        private VRToggle _toggleButton = null;

        // TRIGGER AREA //
        private VRTriggerArea _triggerArea = null;

        // VR SLIDER // 
        private VRSliderDisplay _sliderDisplay = null;

        // VR Scrollview BUTTON
        private ScrollRect _scrollviewModel = null;

        // PANORAMA VIEW //
        private VRPanoramaView _panoramaView = null;
        private Texture2D _panoramaImage = null;

        // OTHERS
        private Vector2 scrollPos;
        private string _GOname;
        private float _GOscale = 1.0f;

        bool handleRepaintErrors = false;

        void OnGUI()
        {
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            // select vr element to create
            EditorGUILayout.LabelField("Element type", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select VR Element to create");
            VRELEMENT_TYPE type = (VRELEMENT_TYPE)EditorGUILayout.EnumPopup("", _elementType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            _elementType = type;
            switch (_elementType)
            {
                case VRELEMENT_TYPE.BUTTON_2D:
                    drawButton2DPanel();
                    break;
                case VRELEMENT_TYPE.BUTTON_3D:
                    drawButtonObjectPanel();
                    break;
                case VRELEMENT_TYPE.GUI_BUTTON:
                    drawGUIbuttonViewPanel();
                    break;
                case VRELEMENT_TYPE.DISPLAY_BUTTON:
                    drawDisplayButtonPanel();
                    break;
                case VRELEMENT_TYPE.TOGGLE_BUTTON:
                    drawTogglePanel();
                    break;
                case VRELEMENT_TYPE.TRIGGER_AREA:
                    drawTriggerAreaPanel();
                    break;
                case VRELEMENT_TYPE.SLIDER:
                    drawSliderPanel();
                    break;
                case VRELEMENT_TYPE.SCROLLVIEW:
                    drawScrollViewPanel();
                    break;
                case VRELEMENT_TYPE.PANORAMA_VIEW:
                    if (drawPanoramaViewPanel())
                        return;
                    break;
            }


            VREasy_utils.DrawHelperInfo();

            EditorGUILayout.EndScrollView();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnDestroy()
        {
            if (_vr2DButton != null)
            {
                try
                {
                    DestroyImmediate(_vr2DButton.gameObject); // this may cause editor to crash when shutdown
                }
                catch (System.Exception e)
                {
                    Debug.Log("AddVRelementHelper error: " + e.ToString());
                }
            }
            if (_vrObjectButton != null)
            {
                try
                {
                    DestroyImmediate(_vrObjectButton.gameObject); // this may cause editor to crash when shutdown
                }
                catch (System.Exception e)
                {
                    Debug.Log("AddVRelementHelper error: " + e.ToString());
                }
            }
            if(_displayButton != null)
            {
                try
                {
                    DestroyImmediate(_displayButton.gameObject); // this may cause editor to crash when shutdown
                }
                catch (System.Exception e)
                {
                    Debug.Log("AddVRelementHelper error: " + e.ToString());
                }
            }
            if(_triggerArea != null)
            {
                try
                {
                    DestroyImmediate(_triggerArea.gameObject); // this may cause editor to crash when shutdown
                }
                catch (System.Exception e)
                {
                    Debug.Log("AddVRelementHelper error: " + e.ToString());
                }
            }
            if (_sliderDisplay != null)
            {
                try
                {
                    DestroyImmediate(_sliderDisplay.gameObject); // this may cause editor to crash when shutdown
                }
                catch (System.Exception e)
                {
                    Debug.Log("AddVRelementHelper error: " + e.ToString());
                }
            }
            if (_toggleButton != null)
            {
                try
                {
                    DestroyImmediate(_toggleButton.gameObject); // this may cause editor to crash when shutdown
                }
                catch (System.Exception e)
                {
                    Debug.Log("AddVRelementHelper error: " + e.ToString());
                }
            }
        }

        // 2D BUTTON //
        private void drawButton2DPanel()
        {
            _GOname = EditorGUILayout.TextField("Game Object name", _GOname);
            //_GOscale = EditorGUILayout.FloatField("Scale", _GOscale);
            if (_vr2DButton == null)
            {
                GameObject go = new GameObject("_");
                go.transform.position = Vector3.forward * int.MaxValue;
                go.hideFlags = HideFlags.HideInHierarchy;
                //go.AddComponent<BoxCollider>();
                //go.AddComponent<SpriteRenderer>();
                _vr2DButton = go.AddComponent<VR2DButton>();
            }
            VR2DButtonEditor.Configure2DButton(ref _vr2DButton, new VR2DButton[] { _vr2DButton});
            EditorGUILayout.Separator();
            if (GUILayout.Button("Create Button"))
            {
                create2DButton(ref _vr2DButton);
            }
        }
        private void create2DButton(ref VR2DButton _vrButton, float scaleMultiplier = 1f)
        {
            VREasy_utils.SetGameObjectInScene(_vrButton.gameObject);

            _vrButton.gameObject.name = _GOname;
            //_vrButton.SetScale(_GOscale);
            //_vrButton.ConfigureCollider(_GOscale * scaleMultiplier);
            switch (_vrButton.faceDirection)
            {
                case VRELEMENT_FACE_DIRECTION.FORWARD:
                    break;
                case VRELEMENT_FACE_DIRECTION.UP:
                    _vrButton.transform.Rotate(Vector3.right, 90.0f);
                    break;
            }

            _vrButton.gameObject.hideFlags = HideFlags.None;
            _vrButton = null;
        }

        // OBJECT BUTTON //
        private void drawButtonObjectPanel()
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select Object to convert to 3D Button");
            _VRObjectButtonModel = (GameObject)EditorGUILayout.ObjectField("", _VRObjectButtonModel, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            if (_VRObjectButtonModel == null)
                return;

            if (_vrObjectButton == null)
            {
#if UNITY_2019_2_OR_NEWER
                switch (PrefabUtility.GetPrefabAssetType(_VRObjectButtonModel))
                {
                    case PrefabAssetType.Regular:
                        {
                            // instantiate and check components
                            _vrObjectButton_isInstantiated = true;
                        }
                        break;
                    case PrefabAssetType.Model:
                        {
                            // instantiate and check components
                            _vrObjectButton_isInstantiated = true;
                        }
                        break;
                    case PrefabAssetType.Variant:
                    case PrefabAssetType.NotAPrefab:
                    case PrefabAssetType.MissingAsset:
                        // check components
                        _vrObjectButton_isInstantiated = false;
                        break;
                }
#else
                switch (PrefabUtility.GetPrefabType(_VRObjectButtonModel))
                {
                    case PrefabType.Prefab:
                        {
                            // instantiate and check components
                            _vrObjectButton_isInstantiated = true;
                        }
                        break;
                    case PrefabType.ModelPrefab:
                        {
                            // instantiate and check components
                            _vrObjectButton_isInstantiated = true;
                        }
                        break;
                    case PrefabType.ModelPrefabInstance:
                    case PrefabType.PrefabInstance:
                    case PrefabType.None:
                    case PrefabType.DisconnectedModelPrefabInstance:
                    case PrefabType.DisconnectedPrefabInstance:
                    case PrefabType.MissingPrefabInstance:
                        // check components
                        _vrObjectButton_isInstantiated = false;
                        break;
                }
#endif
                GameObject go = Instantiate(_VRObjectButtonModel);
                go.hideFlags = HideFlags.HideInHierarchy;
                _vrObjectButton = go.AddComponent<VR3DButton>();
                go.transform.localPosition = Vector3.one * int.MaxValue;
                
            } else
            {
                if (!hasCollider)
                {
                    _collider_type = (VRBUTTON_COLLIDER_TYPE)EditorGUILayout.EnumPopup("Collider type", _collider_type);
                    if (_collider_type == VRBUTTON_COLLIDER_TYPE.MESH)
                    {
                        _colliderMesh = (Mesh)EditorGUILayout.ObjectField("Collider mesh", _colliderMesh, typeof(Mesh), true);
                    }
                }
                if (_vrObjectButton_isInstantiated)
                {
                    _GOname = EditorGUILayout.TextField("Game Object name", _GOname);
                    VR3DButtonEditor.ConfigureObjectButton(ref _vrObjectButton, new VR3DButton[] { _vrObjectButton });
                    EditorGUILayout.Separator();
                    if (_collider_type == VRBUTTON_COLLIDER_TYPE.MESH && _colliderMesh == null)
                    {
                        EditorGUILayout.HelpBox("Please specify collider mesh to proceed", MessageType.Error);
                    }
                    else
                    {
                        if (GUILayout.Button("Create"))
                        {
                            createObjectButton();
                        }
                    }
                }
                else
                {
                    VR3DButtonEditor.ConfigureObjectButton(ref _vrObjectButton, new VR3DButton[] { _vrObjectButton });
                    EditorGUILayout.Separator();
                    if (_collider_type == VRBUTTON_COLLIDER_TYPE.MESH && _colliderMesh == null)
                    {
                        EditorGUILayout.HelpBox("Please specify collider mesh to proceed", MessageType.Error);
                    }
                    else
                    {
                        if (GUILayout.Button("Add"))
                        {
                            addObjectButton();
                        }
                    }
                }
            }
            
        }
        private void addObjectButton()
        {
            bool proceed = prepareObjectButton(_VRObjectButtonModel);
            
            if (proceed) {
                VR3DButton dest = _VRObjectButtonModel.AddComponent<VR3DButton>();
                EditorUtility.CopySerialized(_vrObjectButton, dest);
                copyActions(_vrObjectButton.gameObject, _VRObjectButtonModel);
                
                DestroyImmediate(_vrObjectButton.gameObject);
                _vrObjectButton = null;
                _vrObjectButtonModel = null;
            }
        }
        
        private void createObjectButton()
        {
            GameObject go = Instantiate(_VRObjectButtonModel);
            //bool proceed = prepareObjectButton(_vrObjectButton.gameObject, true);
            bool proceed = prepareObjectButton(go);

            if (proceed)
            {
                VR3DButton dest = go.AddComponent<VR3DButton>();
                EditorUtility.CopySerialized(_vrObjectButton, dest);
                copyActions(_vrObjectButton.gameObject, go);

                VREasy_utils.SetGameObjectInScene(go);
                go.gameObject.name = _GOname;

                //_vrObjectButton.gameObject.hideFlags = HideFlags.None;
                DestroyImmediate(_vrObjectButton.gameObject);
                _vrObjectButton = null;
                _VRObjectButtonModel = null;
            }
            else 
            {
                DestroyImmediate(go);
            }
            

        }

        private void copyActions(GameObject origin, GameObject dest)
        {
            ActionList dest_list = dest.GetComponent<ActionList>();
            EditorUtility.CopySerialized(origin.GetComponent<ActionList>(), dest_list);
            Component[] acts = origin.GetComponents<Component>();
            foreach (Component a in acts)
            {
                if (a as VRAction != null)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(a);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(dest);
                }
            }
            VRAction[] actions = dest.GetComponents<VRAction>();
            dest_list.list.Capacity = actions.Length;
            for (int ii = 0; ii < dest_list.list.Count; ii++)
            {
                dest_list.list[ii] = actions[ii];
            }
        }
        private bool prepareObjectButton(GameObject model)
        {
            VROBJECTBUTTON_VALIDATE_ERROR validate = validateObjectButton();
            bool proceed = false;
            switch (validate)
            {
                case VROBJECTBUTTON_VALIDATE_ERROR.ALREADY_VROBJECTBUTTON:
                    proceed = false;
                    break;
                case VROBJECTBUTTON_VALIDATE_ERROR.NONE:
                    proceed = true;
                    break;
                case VROBJECTBUTTON_VALIDATE_ERROR.NEEDS_COLLIDER:
                    // create collider based on user choice
                    {
                        switch (_collider_type)
                        {
                            case VRBUTTON_COLLIDER_TYPE.BOX:
                                model.AddComponent<BoxCollider>();
                                break;
                            case VRBUTTON_COLLIDER_TYPE.CAPSULE:
                                model.AddComponent<CapsuleCollider>();
                                break;
                            case VRBUTTON_COLLIDER_TYPE.SPHERE:
                                model.AddComponent<SphereCollider>();
                                break;
                            case VRBUTTON_COLLIDER_TYPE.MESH:
                                {
                                    MeshCollider meshCollider = model.AddComponent<MeshCollider>();
                                    meshCollider.sharedMesh = null;
                                    meshCollider.sharedMesh = _colliderMesh;
                                }
                                break;
                        }
                        proceed = true;
                    }
                    break;
            }
            return proceed;
        }
        
        private VROBJECTBUTTON_VALIDATE_ERROR validateObjectButton()
        {
            VR3DButton vrButton = _VRObjectButtonModel.GetComponent<VR3DButton>();
            if (vrButton != null)
            {
                bool remove = EditorUtility.DisplayDialog("Object already a VR object", "The object appears to be a VR object button already. Do you want to override it?", "Yes", "No");
                if (remove)
                {
                    DestroyImmediate(vrButton);
                }
                else
                {
                    return VROBJECTBUTTON_VALIDATE_ERROR.ALREADY_VROBJECTBUTTON;
                }
            }
            

            if (_VRObjectButtonModel.GetComponent<Collider>() == null)
                return VROBJECTBUTTON_VALIDATE_ERROR.NEEDS_COLLIDER;

            return VROBJECTBUTTON_VALIDATE_ERROR.NONE;
        }

        // GUI BUTTON //
        private void drawGUIbuttonViewPanel()
        {
            Canvas[] canvas = FindObjectsOfType<Canvas>();
            if (canvas.Length == 0)
            {
                EditorGUILayout.HelpBox("The scene must have an EventSystem to handle GUI notifications. Please create a Canvas + EventSystem to support your Unity GUI", MessageType.Error);
                return;
            } else
            {
                EditorGUILayout.HelpBox("To be able to interact with Unity GUI in VR, the canvas should have render mode set to World Space", MessageType.Info);
                foreach(Canvas c in canvas)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(c.name);
                    if(GUILayout.Button("Locate"))
                    {
                        EditorGUIUtility.PingObject(c);
                    }
                    if (c.renderMode != RenderMode.WorldSpace)
                    {
                        if (GUILayout.Button("Set to World Space"))
                        {
                            c.renderMode = RenderMode.WorldSpace;
                        }
                    } else
                    {
                        EditorGUILayout.LabelField("Ready");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
            }
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Select GUI element to convert to VR GUI button",EditorStyles.wordWrappedLabel);
            _guiButtonModel = (GameObject)EditorGUILayout.ObjectField("", _guiButtonModel, typeof(GameObject), true);
            
            if(_guiButtonModel != null)
            {
                EditorGUILayout.Separator();
                RectTransform rect = _guiButtonModel.GetComponent<RectTransform>();
                UnityEngine.UI.Selectable sel = _guiButtonModel.GetComponent<UnityEngine.UI.Selectable>();
                if (rect == null || sel == null)
                {
                    EditorGUILayout.HelpBox("Selected object is not a GUI element!", MessageType.Error);
                } else
                {
                    if (GUILayout.Button("Add VRGUIButton"))
                    {
                        _guiButtonModel.AddComponent<VRGUIButton>();
                        BoxCollider col = _guiButtonModel.GetComponent<BoxCollider>();
                        col.size = new Vector3(rect.rect.width, rect.rect.height, 0.1f);
                        Selection.activeGameObject = _guiButtonModel;
                        EditorGUIUtility.PingObject(_guiButtonModel);
                        _guiButtonModel = null;
                    }
                }
                
            }
        }

        // DISPLAY BUTTON //
        private void drawDisplayButtonPanel()
        {
            _GOname = EditorGUILayout.TextField("Game Object name", _GOname);
            //_GOscale = EditorGUILayout.FloatField("Scale", _GOscale);
            if (_displayButton == null)
            {
                GameObject go = new GameObject("_");
                go.transform.position = Vector3.forward * int.MaxValue;
                go.hideFlags = HideFlags.HideInHierarchy;
                //BoxCollider col = go.AddComponent<BoxCollider>();
                //col.center = col.center;// + Vector3.forward * 2.0f;
                go.AddComponent<SpriteRenderer>();
                _displayButton = go.AddComponent<VRDisplayButton>();
            }
            VRDisplayButtonEditor.ConfigureDisplayButton(ref _displayButton, new Object[] { _displayButton });
            EditorGUILayout.Separator();
            if (GUILayout.Button("Create Display button"))
            {
                VR2DButton button = _displayButton;
                create2DButton(ref button,2f);
                _displayButton = null;
            }
        }

        // TOGGLE BUTTON //
        private void drawTogglePanel()
        {
            _GOname = EditorGUILayout.TextField("Game Object name", _GOname);
            //_GOscale = EditorGUILayout.FloatField("Scale", _GOscale);
            if (_toggleButton == null)
            {
                GameObject go = new GameObject("_");
                go.transform.position = Vector3.forward * int.MaxValue;
                go.hideFlags = HideFlags.HideInHierarchy;
                go.AddComponent<SpriteRenderer>();
                _toggleButton = go.AddComponent<VRToggle>();
            }
            VRToggleEditor.ConfigureToggle(ref _toggleButton, new Object[] { _toggleButton });
            EditorGUILayout.Separator();
            if (GUILayout.Button("Create Toggle button"))
            {
                VR2DButton button = _toggleButton;
                create2DButton(ref button);
                _toggleButton = null;
            }
        }

        // TRIGGER AREA //
        private void drawTriggerAreaPanel()
        {
            _GOname = EditorGUILayout.TextField("Game Object name", _GOname);
            EditorGUILayout.Separator();
            if (_triggerArea == null)
            {
                GameObject go = new GameObject("_");
                go.transform.position = Vector3.forward * int.MaxValue;
                go.hideFlags = HideFlags.HideInHierarchy;
                go.AddComponent<BoxCollider>().isTrigger = true;
                _triggerArea = go.AddComponent<VRTriggerArea>();
            }
            VRTriggerAreaEditor.ConfigureAreaTrigger(ref _triggerArea, new Object[] { _triggerArea });
            EditorGUILayout.Separator();
            if (GUILayout.Button("Create Trigger area"))
            {
                VREasy_utils.SetGameObjectInScene(_triggerArea.gameObject);
                _triggerArea.gameObject.name = _GOname;
                _triggerArea.gameObject.hideFlags = HideFlags.None;
                _triggerArea = null;
            }
        }

        // SLIDER //
        private void drawSliderPanel()
        {
            _GOname = EditorGUILayout.TextField("Game Object name", _GOname);
            _GOscale = EditorGUILayout.FloatField("Scale", _GOscale);
            if(_sliderDisplay == null)
            {
                GameObject go = new GameObject("_");
                go.transform.position = Vector3.forward * int.MaxValue;
                go.hideFlags = HideFlags.HideInHierarchy;
                _sliderDisplay = go.AddComponent<VRSliderDisplay>();
            }
            VRSliderDisplayEditor.ConfigureSliderDisplay(ref _sliderDisplay);
            EditorGUILayout.Separator();
            if (GUILayout.Button("Create Slider"))
            {
                VREasy_utils.SetGameObjectInScene(_sliderDisplay.gameObject);
                _sliderDisplay.gameObject.name = _GOname;
                _sliderDisplay.gameObject.hideFlags = HideFlags.None;
                _sliderDisplay.transform.localScale = Vector3.one * _GOscale * 0.1f;
                BoxCollider col = _sliderDisplay.Slider.GetComponent<BoxCollider>();
                Vector3 size = Vector3.one;
                size.z = 0.5f;
                col.size = size;
                col.isTrigger = true;
                _sliderDisplay = null;
            }
        }

        // SCROLLVIEW //
        private void drawScrollViewPanel()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Select Scrollview object (with ScrollRect) that needs VR interaction", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();
            _scrollviewModel = (ScrollRect)EditorGUILayout.ObjectField("", _scrollviewModel, typeof(ScrollRect), true);

            if (_scrollviewModel != null)
            {
                if(_scrollviewModel.GetComponentsInChildren<VRGUIScrollbar>().Length > 1)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("ScrollView has already VR Easy interaction", EditorStyles.wordWrappedLabel);
                } else
                {
                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Add VREasy interaction"))
                    {
                        Scrollbar[] scrollbars = _scrollviewModel.GetComponentsInChildren<Scrollbar>();
                        foreach(Scrollbar s in scrollbars)
                        {
                            RectTransform scrollbarTransform = s.GetComponent<RectTransform>();
                            Vector3 colliderSize = new Vector3(scrollbarTransform.rect.width, scrollbarTransform.rect.height, 1);
                            Vector3 colliderCentre = new Vector3(scrollbarTransform.rect.width/2, scrollbarTransform.rect.height/2, 1);
                            s.gameObject.AddComponent<VRGUIScrollbar>();
                            BoxCollider col = s.GetComponent<BoxCollider>();
                            if (colliderSize.x < colliderSize.y) colliderCentre *= -1;
                            col.center = colliderCentre;
                            col.size = colliderSize;
                        }
                        Selection.activeGameObject = _scrollviewModel.gameObject;
                        EditorGUIUtility.PingObject(_scrollviewModel.gameObject);
                        _scrollviewModel = null;
                        GUIUtility.ExitGUI();
                    }
                }
                // check canvas
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                Canvas c = _scrollviewModel.GetComponentInParent<Canvas>();
                if(c != null)
                {
                    if (c.renderMode != RenderMode.WorldSpace)
                    {
                        EditorGUILayout.HelpBox("To be able to interact with Unity GUI in VR, the canvas should have render mode set to World Space", MessageType.Warning);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(c.name);
                        if (GUILayout.Button("Locate"))
                        {
                            EditorGUIUtility.PingObject(c);
                        }
                        if (GUILayout.Button("Set to World Space"))
                        {
                            c.renderMode = RenderMode.WorldSpace;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                        
                } else
                {
                    EditorGUILayout.HelpBox("The ScrollView object selected must have a Canvas parent.", MessageType.Error);
                }

            }

            
        }

        // PANORAMA VIEW //
        private bool drawPanoramaViewPanel()
        {
            EditorGUILayout.Separator();
            _panoramaView = (VRPanoramaView)EditorGUILayout.ObjectField("Configure panorama view", _panoramaView, typeof(VRPanoramaView), true);
            if (_panoramaView == null)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("No Panorama view selected in current scene", EditorStyles.wordWrappedLabel);
                _panoramaImage = (Texture2D)EditorGUILayout.ObjectField("Image", _panoramaImage, typeof(Texture2D), true);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add new panorama"))
                {
                    createPanoramaView();
                }
                EditorGUILayout.EndHorizontal();
            } else
            {
                VRPanoramaViewEditor.ConfigurePanoramaView(ref _panoramaView);
            }
            return false;
        }

        private void createPanoramaView()
        {
            GameObject dome = Instantiate(Resources.Load<GameObject>("PanoramaPrefab")) as GameObject;
            if(dome == null)
            {
                Debug.LogError("[VREasy] PanoramaPrefab could not be found in Resources folder. Did you delete it?");
            } else
            {
                dome.name = "[VREasy] PanoramaView";
                // position the dome with the camera in the centre
                Camera cam = FindObjectOfType<Camera>();
                if (cam != null)
                {
                    dome.transform.position = cam.transform.position;
                }
                _panoramaView = dome.GetComponent<VRPanoramaView>();
                if(_panoramaView == null)
                {
                    _panoramaView = dome.AddComponent<VRPanoramaView>();
                }
                if (_panoramaImage != null)
                {
                    _panoramaView.Image = _panoramaImage;
                    _panoramaImage = null;
                }
            }
            
        }

        // helper functions //

        
        
    }
     
}