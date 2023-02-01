using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{

    public enum CROSSHAIR_TYPE
    {
        SINGLE_SPRITE,
        DUAL_SPRITE,
        ANIMATED_SPRITE
    }
    
    public enum VRELEMENT_TYPE
    {
        BUTTON_2D,
        BUTTON_3D,
        DISPLAY_BUTTON,
        GUI_BUTTON,
        TOGGLE_BUTTON,
        TRIGGER_AREA,
        SLIDER,
        SCROLLVIEW,
        PANORAMA_VIEW
    }

    public enum VRELEMENT_FACE_DIRECTION
    {
        UP,
        FORWARD
    }

    public enum VRBUTTON_REFRESH_TYPE
    {
        NORMAL,
        STICKY,
        BILLBOARD
    }

    public enum SWITCH_TYPE
    {
        MESH,
        MATERIAL,
        TEXTURE,
        SPRITE,
        CUSTOM,
        SOUND
    }

    public enum VRBUTTON_COLLIDER_TYPE
    {
        BOX,
        MESH,
        SPHERE,
        CAPSULE
    }

    public enum VROBJECTBUTTON_VALIDATE_ERROR
    {
        NONE,
        ALREADY_VROBJECTBUTTON,
        NEEDS_COLLIDER
    }

    public enum DISPLAY_IMAGE_TYPE
    {
        SPRITE,
        TEXTURE
    }

    public enum VRSELECTOR_TYPE
    {
        SIGHT,
        POINTER,
        TOUCH,
        MIXED_REALITY
    }

    public enum VRLOCOMOTION_INPUT
    {
        UNITY_INPUT,
        GENERIC_VR_CONTROLLER,
        STEAM_CONTROLLER,
        MOBILE_TILT,
        TRIGGER,
        OCULUS_CONTROLLER,
        GEAR_VR_CONTROLLER,
        DAYDREAM_CONTROLLER,
        WAVEVR
    }

    public enum PLAY_ACTION
    {
        PLAY,
        STOP,
        TOGGLE
    }

    public enum HOTSPOT_TYPE
    {
        LOAD_LOCATION,
        INFO
    }

    public enum GRAB_TYPE
    {
        SLIDE,
        DRAG,
        ROTATE
    }

    public enum LIGHTMAP_STATE
    {
        LIGHTMAP1,
        LIGHTMAP2
    }

    public enum GENERIC_VR_BUTTON
    {
        STEAMVR_RIGHT_CONTROLLER_MENU,
        STEAMVR_RIGHT_TRACKPAD_PRESS,
        STEAMVR_RIGHT_TRACKPAD_TOUCH,
        STEAMVR_RIGHT_TRIGGER_TOUCH,
        STEAMVR_LEFT_CONTROLLER_MENU,
        STEAMVR_LEFT_TRACKPAD_PRESS,
        STEAMVR_LEFT_TRACKPAD_TOUCH,
        STEAMVR_LEFT_TRIGGER_TOUCH,
        OCULUS_ONE_PRESS,
        OCULUS_THREE_PRESS,
        OCULUS_PRIMARY_THUMB_STICK_PRESS,
        OCULUS_SECONDARY_THUMB_STICK_PRESS,
        OCULUS_PRIMARY_THUMB_STICK_TOUCH,
        OCULUS_SECONDARY_THUMB_STICK_TOUCH,
        OCULUS_PRIMARY_INDEX_TRIGGER,
        OCULUS_SECONDARY_INDEX_TRIGGER,
        WMR_LEFT_TOUCHPAD_TOUCH,
        WMR_RIGHT_TOUCHPAD_TOUCH,
        WMR_LEFT_TOUCHPAD_PRESS,
        WMR_RIGHT_TOUCHPAD_PRESS,
        WMR_LEFT_THUMBSTICK_PRESS,
        WMR_RIGHT_THUMBSTICK_PRESS,
        WMR_LEFT_SELECT_TRIGGER_PRESS,
        WMR_RIGHT_SELECT_TRIGGER_PRESS,
        WMR_LEFT_GRIP_PRESS,
        WMR_RIGHT_GRIP_PRESS,
        WMR_LEFT_MENU_BUTTON_PRESS,
        WMR_RIGHT_MENU_BUTTON_PRESS,
        OCULUS_REMOTE_ONE,
        OCULUS_REMOTE_TWO,
        OCULUS_TWO_PRESS,
        OCULUS_FOUR_PRESS,
        OCULUS_START,
        OCULUS_PRIMARY_THUMBREST,
        OCULUS_SECONDARY_THUMBREST,
        KNUCKLES_LEFT_INNER_FACE,
        KNUCKLES_RIGHT_INNER_FACE,
        KNUCKLES_LEFT_OUTER_FACE,
        KNUCKLES_RIGHT_OUTER_FACE,
        KNUCKLES_LEFT_TRACKPAD_PRESS,
        KNUCKLES_LEFT_TRACKPAD_TOUCH,
        KNUCKLES_RIGHT_TRACKPAD_PRESS,
        KNUCKLES_RIGHT_TRACKPAD_TOUCH,
        KNUCKLES_LEFT_TRIGGER_TOUCH,
        KNUCKLES_RIGHT_TRIGGER_TOUCH,
        DAYDREAM_LEFT_TOUCHPAD_TOUCH,
        DAYDREAM_LEFT_TOUCHPAD_CLICK,
        DAYDREAM_LEFT_APP_BUTTON,
        DAYDREAM_RIGHT_TOUCHPAD_TOUCH,
        DAYDREAM_RIGHT_TOUCHPAD_CLICK,
        DAYDREAM_RIGHT_APP_BUTTON,
        OCULUS_ONE_TOUCH,
        OCULUS_TWO_TOUCH,
        OCULUS_THREE_TOUCH,
        OCULUS_FOUR_TOUCH,
    }

    public enum GENERIC_CONTROLLER_TYPE
    {
        OCULUS_TOUCH,
        STEAM_VR,
        WINDOWS_MIXED_REALITY,
        OCULUS_REMOTE,
        KNUCKLES,
        DAYDREAM
    }

    /*public enum GOOGLEVR_CONTROLLER_INPUT
    {
        IS_TOUCHING,
        APP_BUTTON,
        HOME_BUTTON_STATE,
        HOME_BUTTON_DOWN,
        TOUCH_DOWN,
        TOUCH_UP,
        CLICK_BUTTON,
        CLICK_BUTTON_DOWN,
        CLICK_BUTTON_UP,
        APP_BUTTON_DOWN,
        APP_BUTTON_UP
    }*/

    public enum GEARVR_CONTROLLER_INPUT
    {
        // https://developer.oculus.com/documentation/unity/latest/concepts/unity-ovrinput/
        TOUCHPAD_PRESS,
        TOUCHPAD_TOUCH,
        INDEX_TRIGGER,
        BACK_BUTTON,
    }

    public enum STEAM_VR_CONTROLLER_SIDE
    {
        LEFT,
        RIGHT
    }

    public enum STEAM_VR_CONTROLLER_INPUT_TYPE
    {
        PRESS,
        TOUCH
    }


    public enum OCULUS_CONTROLLER_INPUT_TYPE
    {
        BUTTON,
        TOUCH
    }

    public enum X_AXIS_TYPE
    {
        TRANSLATE,
        ROTATE,
        STEPPED_ROTATE
    }

    public enum STEREOSCOPIC_MODE
    {
        IMAGE,
        VIDEO
    }

    public enum PANORAMA_MODE
    {
        IMAGE,
        VIDEO_FILE,
        VIDEO_URL,
        CUBEMAP
    }

    public enum VIDEO_PLAYER_MODE
    {
        FILE,
        URL
    }

    public enum TRANSFORM_TARGET
    {
        POSITION,
        ROTATION
    }

    public enum TRANSFORM_TARGET_ELEMENT
    {
        X,
        Y,
        Z
    }

    public enum ANIMATION_TYPE
    {
        LEGACY,
        ANIMATOR
    }

    public enum ANIMATION_TARGET
    {
        ANIMATION_SPEED,
        ANIMATION_FRAME,
        NUMERIC_PARAMETER
    }

    public enum ANIMATOR_PARAMETER_TYPE
    {
        FLOAT,
        INT,
        BOOL,
        TRIGGER
    }

    public enum JOINT_TYPE
    {
        FIXED,
        SPRING,
        //ATTACHED
    }

    public enum TELEPORT_CONTROLLER_RENDERER
    {
        LINE,
        TEXTURES
    }

    public enum TELEPORT_CONTROLLER_BEAM
    {
        PARABOLE,
        STRAIGHT
    }

    public enum MR_CONTROLLER
    {
        HOLOLENS_HAND,
        MOTION_CONTROLLER
    }

    public enum ACTIVATION_OPTION
    {
        Toggle = 0,
        Enable = 1,
        Disable = 2
    }

    public enum VR_HMD_TYPE
    {
        OCULUS_RIFT,
        OCULUS_RIFT_S,
        OCULUS_GO,
        OCULUS_QUEST,
        GEAR_VR,
        HTC_VIVE,
        VIVE_FOCUS,
        GOOGLE_DAYDREAM,
        GOOGLE_CARDBOARD,
        WINDOWS_MIXED_REALITY
    }
    



    [Serializable]
    public class InvalidMaterialProperty : System.Exception
    {
        public InvalidMaterialProperty() : base() { }
        public InvalidMaterialProperty(string message) : base(message) { }
        public InvalidMaterialProperty(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
#if !NETFX_CORE && !UNITY_WSA_10_0
        protected InvalidMaterialProperty(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
#endif
    }


    public class VREasy_utils {

        public static string STORE_OPTIONS_FILE = "VREasy_options.txt";

        public static string WEB_URL_LINK = "http://blog.avrworks.com";
        public static string LOGO_IMAGE = "VREasy_logo";
        public static string VREASY_VERSION = "1.6";

        public static void LoadClassesFromAssembly(Type parentType, ref List<string> assemblyNames, ref List<string> names, string substractString = "")
        {
#if NETFX_CORE || UNITY_WSA_10_0
            Assembly assembly = parentType.GetTypeInfo().Assembly; //Assembly.GetAssembly(parentType);
            
            assemblyNames.Clear();
            names.Clear();
            IEnumerable<TypeInfo> subclasses = assembly.DefinedTypes.Where(t => !t.IsAbstract && t.IsSubclassOf(parentType));
            foreach (TypeInfo t in subclasses)
            {
                if (t.IsAbstract) continue;
                string name = t.Name;
                if (!string.IsNullOrEmpty(substractString))
                {
                    int index1 = name.IndexOf(substractString);
                    if (index1 != -1)
                    {
                        name = name.Remove(index1);
                    }
                }

                names.Add(name);
                assemblyNames.Add(t.AssemblyQualifiedName);
            }
#else

            Assembly assembly = Assembly.GetAssembly(parentType);
            Type[] types = assembly.GetTypes();

            assemblyNames.Clear();
            names.Clear();
            IEnumerable<Type> subclasses = types.Where(t => !t.IsAbstract && t.IsSubclassOf(parentType));
            foreach (Type t in subclasses)
            {
                if (t.IsAbstract) continue;
                string name = t.Name;
                if (!string.IsNullOrEmpty(substractString))
                {
                    int index1 = name.IndexOf(substractString);
                    if (index1 != -1)
                    {
                        name = name.Remove(index1);
                    }
                }

                names.Add(name);
                assemblyNames.Add(t.AssemblyQualifiedName);
            }
#endif


        }

        public static T LoadAndSetClassFromAssembly<T>(GameObject obj, string assemblyName) where T : Component
        {
            Type t = Type.GetType(assemblyName);
            return (T)obj.AddComponent(t);
        }

        public static void LoadComponents(GameObject target, ref List<Component> components_list, ref List<string> componentNames_list)
        {
            components_list.Clear();
            componentNames_list.Clear();
            if (target == null) return;
            Component[] components = target.GetComponents<Component>();
            foreach (Component p in components)
            {
                components_list.Add(p);
                if (componentNames_list.Contains(p.GetType().Name)) {
                    int count = 0;
                    foreach (string c in componentNames_list)
                    {
                        if (p.GetType().Name.Equals(c) || (p.GetType().Name + "_" + count).Equals(c))
                            count++;
                    }
                    componentNames_list.Add(p.GetType().Name + "_" + count);
                } else
                {
                    componentNames_list.Add(p.GetType().Name);
                }
            }

        }

        public static void LoadPropertiesFromComponent(Component c, ref List<string> properties)
        {
            properties.Clear();
            PropertyInfo[] props = c.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in props)
            {
                if (propertyInfo.PropertyType == typeof(float) || propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(Color) || propertyInfo.PropertyType == typeof(Vector2) || propertyInfo.PropertyType == typeof(Vector3) || propertyInfo.PropertyType == typeof(Vector4))
                {
                    properties.Add(propertyInfo.Name);
                }

            }

        }

        public static void LoadMethods(Type myType, ref List<MethodInfo> methods_list, ref List<string> methodNames_list)
        {
            // Get the public methods.
            MethodInfo[] myArrayMethodInfo = myType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            //Debug.Log("The number of public methods is " + myArrayMethodInfo.Length);
            methods_list.Clear();
            methodNames_list.Clear();
            for (int i = 0; i < myArrayMethodInfo.Length; i++)
            {
                MethodInfo myMethodInfo = (MethodInfo)myArrayMethodInfo[i];

                //Debug.Log("The name of the method is " + myMethodInfo.Name);
                methods_list.Add(myMethodInfo);
                methodNames_list.Add(myMethodInfo.Name);
            }
        }

        public static void LoadParameters(MethodInfo myMethodInfo, ref List<Type> parameters_list)
        {
            parameters_list.Clear();
            ParameterInfo[] arguments = myMethodInfo.GetParameters();
            foreach (ParameterInfo ta in arguments)
            {
                parameters_list.Add(ta.ParameterType);
                //Debug.Log(ta.ParameterType.FullName);
            }
        }

        public static void SetAndConfigureSprite(Sprite sp, SpriteRenderer renderer, float width, float height)
        {
            if (renderer == null) return;
            renderer.sprite = sp;
            if (sp == null) return;

            
            Vector2 sprite_size = renderer.sprite.rect.size;
            Vector2 local_sprite_size = sprite_size / renderer.sprite.pixelsPerUnit;
            Dictionary<Transform, Vector3> scales = new Dictionary<Transform, Vector3>();
            Dictionary<Transform, Vector3> locations = new Dictionary<Transform, Vector3>();
            IEnumerable<Transform> children = renderer.GetComponentsInChildren<Transform>().Where(go => go.transform.parent == renderer.transform);
            foreach (Transform t in children)
            {
                scales.Add(t, t.localScale);
                locations.Add(t, t.position);
            }
            Vector3 prevScale = renderer.transform.localScale;
            Sprite sprite = renderer.sprite;
            Bounds bounds = sprite.bounds;
            float xSize = bounds.size.x;
            float ySize = bounds.size.y;
            renderer.transform.localScale = new Vector3(width / xSize, height / ySize, 1f);
            BoxCollider col = renderer.GetComponent<BoxCollider>();
            if (col != null)
            {
                renderer.GetComponent<BoxCollider>().size = new Vector3(local_sprite_size.x, local_sprite_size.y, 0.1f); //(1.0f/ sprite.pixelsPerUnit);
            }
            foreach (Transform t in scales.Keys)
            {
                t.localScale = new Vector3(scales[t].x / (renderer.transform.localScale.x / prevScale.x), scales[t].y / (renderer.transform.localScale.y / prevScale.y), scales[t].z);
                t.position = locations[t];
            }


        }

        public static Camera GetMainCamera()
        {
            Camera[] cams = GameObject.FindObjectsOfType<Camera>();
            foreach(Camera cam in cams)
            {
                if (cam.stereoTargetEye == StereoTargetEyeMask.Both)
                {
                    return cam;
                }
            }

            if (Camera.main != null)
            {
                return Camera.main;
            }
            else
            {
                Camera cam = GameObject.FindObjectOfType<Camera>();
                if (cam != null) return cam;
                else return null;
            }
        }

        public static Transform GetMainCameraTransform()
        {
            Camera cam = GetMainCamera();
            return (cam == null) ? null : cam.transform;
        }

        public static void MeshFromPoints(List<Vector3> points, MeshFilter Mesh_Filter, Vector3 up, float LineWidth)
        {
            if(Mesh_Filter.sharedMesh != null) Mesh_Filter.sharedMesh.Clear();
            Mesh mesh = new Mesh();
            Mesh_Filter.mesh = mesh;

            if (points.Count == 0) return;
            Vector3[] vertices = new Vector3[(points.Count - 1) * 4];
            int[] triangles = new int[((points.Count - 1) * 2) * 3];
            Vector3[] normals = new Vector3[(points.Count - 1) * 4];
            Vector2[] uvs = new Vector2[(points.Count - 1) * 4];
            for (int ii = 1; ii < points.Count; ii++)
            {
                int index = (ii - 1) * 4;
                Vector3 v = points[ii] - points[ii - 1];
                Vector3 p = -Vector3.Cross(v, up).normalized * LineWidth;

                vertices[index] = (points[ii] - p); // pL + p1
                vertices[index + 1] = (points[ii - 1] + p); // pR + p0 
                vertices[index + 2] = (points[ii - 1] - p); // pL + p0
                vertices[index + 3] = (points[ii] + p); // pR + p1

                triangles[(ii - 1) * 6] = index;
                triangles[(ii - 1) * 6 + 1] = index + 1;
                triangles[(ii - 1) * 6 + 2] = index + 2;
                triangles[(ii - 1) * 6 + 3] = index + 3;
                triangles[(ii - 1) * 6 + 4] = index + 1;
                triangles[(ii - 1) * 6 + 5] = index + 0;

                normals[index] = up; // pL + p1
                normals[index + 1] = up; // pR + p0 
                normals[index + 2] = up; // pL + p0
                normals[index + 3] = up; // pR + p1

                uvs[index] = Vector2.one; // pL + p1
                uvs[index + 1] = Vector2.zero; // pR + p0 
                uvs[index + 2] = Vector2.up; // pL + p0
                uvs[index + 3] = Vector2.right; // pR + p1
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
        }

        public static void SetGameObjectInScene(GameObject go, Camera referenceCamera = null)
        {
#if UNITY_EDITOR
            if (SceneView.lastActiveSceneView == null)
            {
                go.transform.position = Vector3.zero;
            }
            else
            {
                if (referenceCamera == null)
                    referenceCamera = SceneView.lastActiveSceneView.camera;
                Vector3 pos = referenceCamera.transform.position + (referenceCamera.transform.forward) * 2;
                go.transform.position = pos;
                Vector3 lookAtPosition = referenceCamera.transform.position;
                lookAtPosition.y = go.transform.position.y;
                go.transform.LookAt(lookAtPosition);
                go.transform.Rotate(Vector3.up, 180.0f);
            }
#else
            go.transform.position = Vector3.zero;
#endif
        }

#if UNITY_EDITOR

        public static bool CreateOverlayGUIButton(float xCentre, float yCentre, ref Rect rect, string tooltip)
        {
            float originalX = rect.x;
            float originalY = rect.y;
            rect.x += xCentre - rect.width / 2;
            rect.y += yCentre - rect.height / 2;
            // choose this for visual testing input buttons
            //bool state = GUI.Button(rect, new GUIContent("", tooltip));
            // choose this for production
            bool state = GUI.Button(rect, new GUIContent("", tooltip), GUIStyle.none);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            rect.x = originalX;
            rect.y = originalY;

            return state;
        }

        

        private static string PREFS_SHOWABOUT = "PREFS_SHOWABOUT";

        public static void DrawHelperInfo()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            bool showAbout = PlayerPrefs.GetInt(PREFS_SHOWABOUT, 1) == 1;
            showAbout = EditorGUILayout.Foldout(showAbout, "About");
            PlayerPrefs.SetInt(PREFS_SHOWABOUT, showAbout ? 1 : 0);
            if (!showAbout)
                return;
            
            Handles.BeginGUI();
            //GUIContent content = new GUIContent("logo");
            //GUIStyle style = GUIStyle.none;
            Texture logo = Resources.Load<Texture>(LOGO_IMAGE);
            //content.image = logo;
            Rect rect = GUILayoutUtility.GetRect(75, 75);//content, style);
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            Handles.BeginGUI();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Version " + VREASY_VERSION);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("For tutorials, manuals and more info");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Rect labelRect = GUILayoutUtility.GetRect(7 * WEB_URL_LINK.Length, 25);
            GUI.Label(labelRect, WEB_URL_LINK);
            if (Event.current.type == EventType.MouseUp && labelRect.Contains(Event.current.mousePosition))
                Application.OpenURL(WEB_URL_LINK);
            GUI.color = new Color(0.2f,0.7f,1.0f);
            GUI.Label(labelRect, WEB_URL_LINK);
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Copyright AVR Works 2019");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            Handles.EndGUI();
        }
#endif
        }
}