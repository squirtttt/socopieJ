using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VREasy
{
    public class SwitchAction : VRAction
    {
        #region
        public SWITCH_TYPE Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    processType();
                    Target = null;
                    swapObjects.Clear();
                }
            }
        }

        public Object Target
        {
            get
            {
                return _target;
            }
            set
            {
                if(_target != value)
                {
                    _target = value;
                    if (_target != null)
                    {
                        Object obj = getDefaultSwap();
                        if (obj != null) swapObjects.Insert(0, obj);
                    }
                }
            }
        }
        #endregion PROPERTIES

        public string propertyName;
        public List<Object> swapObjects = new List<Object>();

        public bool storeOption = false;
        public string optionName;
        
        private int _swapIndex = 0;
        public Object _target;
        public SWITCH_TYPE _type;

                 

        public override void Trigger()
        {
            if (!Target)
            {
                Debug.Log("SwitchAction: target not set");
                return;
            }

            switchValues(propertyName);

            return;
        }

        public void SwapTo(int index)
        {
            if (index >= swapObjects.Count) return;
            _swapIndex = index-1;
            switchValues(propertyName);
        }

        private bool updateSwapCount()
        {
            if (swapObjects.Count == 0)
            {
                Debug.Log("SwitchAction: no swappable objects are set.");
                return false;
            }
            _swapIndex++;
            if (_swapIndex >= swapObjects.Count)
                _swapIndex = 0;
            return true;
        }

        private void switchValues(string propertyName, bool isProperty = true)
        {
            if (string.IsNullOrEmpty(propertyName))
                processType();
            try
            {
                if (!updateSwapCount())
                    return;
                if (isProperty)
                    Target.GetType().GetProperty(propertyName).SetValue(Target, swapObjects[_swapIndex], null);
                else
                    Target.GetType().GetField(propertyName).SetValue(swapObjects[_swapIndex], null);

                // store option in filename
                if (storeOption) store();
                
            } catch(System.Exception e)
            {
                Debug.Log("SwitchAction: error whilst finding properies via reflection: " + e.ToString());
            }
        }

        private Object getDefaultSwap()
        {
            if (string.IsNullOrEmpty(propertyName))
                processType();
            Object obj = null;
            try
            {
                obj = (Object)Target.GetType().GetProperty(propertyName).GetValue(Target, null);
            } catch(System.Exception e)
            {
                Debug.Log("SwitchAction: error whilst finding properies via reflection: " + e.ToString());
            }
            return obj;
        }

        private void processType()
        {
            switch (_type)
            {
                case SWITCH_TYPE.MATERIAL:
                    propertyName = "sharedMaterial";
                    break;
                case SWITCH_TYPE.MESH:
                    propertyName = "sharedMesh";
                    break;
                case SWITCH_TYPE.TEXTURE:
                    propertyName = "mainTexture";
                    break;
                case SWITCH_TYPE.SPRITE:
                    propertyName = "sprite";
                    break;
                case SWITCH_TYPE.SOUND:
                    propertyName = "clip";
                    break;
                case SWITCH_TYPE.CUSTOM:
                    propertyName = "";
                    break;
            }
        }

        private void store()
        {

            if (string.IsNullOrEmpty(optionName))
            {
                Debug.LogError("[VREasy] Switch option could not be stored as option name was not specified");
                return;
            }
            Debug.Log(swapObjects[_swapIndex].name);
            // write to VREasy_utils.STORE_OPTIONS_FILE

#if NETFX_CORE || UNITY_WSA_10_0
            Debug.LogWarning("[VREasy]: SwitchAction cannot store data in a file for the Universal Windows Platform");

#else
            string storeoptionfilepath = Path.Combine(Application.persistentDataPath, VREasy_utils.STORE_OPTIONS_FILE);
            string tempFilename = Path.Combine(Application.persistentDataPath, "temp__");
            // Read the file and display it line by line.
            StreamWriter writeStream;
            bool found = false;
            if (!File.Exists(storeoptionfilepath))
            {
                writeStream = new StreamWriter(File.Create(storeoptionfilepath));

            } else
            {
                File.Delete(tempFilename);
                File.Copy(storeoptionfilepath, tempFilename);
                writeStream = new StreamWriter(storeoptionfilepath);
                StreamReader readStream = new StreamReader(tempFilename);
                char[] delimiterChars = { ',' };
                string line;
                while ((line = readStream.ReadLine()) != null)
                {
                    string[] words = line.Split(delimiterChars);
                    if (optionName.Equals(words[0]))
                    {
                        found = true;
                        writeStream.WriteLine(optionName + "," + swapObjects[_swapIndex].name);
                    }
                    else
                    {
                        writeStream.WriteLine(line);
                    }
                }
                readStream.Close();
                File.Delete(tempFilename);
            }
            // add option to file
            if (!found)
            {
                writeStream.WriteLine(optionName + "," + swapObjects[_swapIndex].name);
            }

            writeStream.Close();
            
            Debug.Log("Option stored in: " + storeoptionfilepath);
#endif


        }

    }

    
}
