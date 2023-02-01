using UnityEngine;
using System.Collections.Generic;
using System;

namespace VREasy
{
    public class ToggleLightmapAction : VRAction
    {
        public override void Trigger()
        {
            toggleLightmap();
        }

        // Inspired by LightMapSwitcher in http://wiki.unity3d.com/index.php/LightMapSwitcher
        public List<Texture2D> Day_near_dir = new List<Texture2D>();
        public List<Texture2D> Day_far_light = new List<Texture2D>();
        public List<Texture2D> Night_near_dir = new List<Texture2D>();
        public List<Texture2D> Night_far_light = new List<Texture2D>();

        public LIGHTMAP_STATE lightmapState = LIGHTMAP_STATE.LIGHTMAP1;

        private LightmapData[] dayLightMaps;
        private LightmapData[] nightLightMaps;
        
        void Start()
        {
            if ((Day_near_dir.Count != Day_far_light.Count) || (Night_near_dir.Count != Night_far_light.Count))
            {
                Debug.Log("[VREasy] ChangeLightmapAction: Near (directional) and Far (light) lightmap arrays must be of equal length");
                return;
            }

            dayLightMaps = new LightmapData[Day_near_dir.Count];
            for (int i = 0; i < Day_near_dir.Count; i++)
            {
                dayLightMaps[i] = new LightmapData();
                dayLightMaps[i].lightmapDir = Day_near_dir[i];
                dayLightMaps[i].lightmapColor = Day_far_light[i];
            }

            nightLightMaps = new LightmapData[Night_near_dir.Count];
            for (int i = 0; i < Night_near_dir.Count; i++)
            {
                nightLightMaps[i] = new LightmapData();
                nightLightMaps[i].lightmapDir = Night_near_dir[i];
                nightLightMaps[i].lightmapColor = Night_far_light[i];
            }
            setLightmap(lightmapState);
        }
        
        private void toggleLightmap()
        {
            switch (lightmapState)
            {
                case LIGHTMAP_STATE.LIGHTMAP1:
                    lightmapState = LIGHTMAP_STATE.LIGHTMAP2;
                    break;
                case LIGHTMAP_STATE.LIGHTMAP2:
                    lightmapState = LIGHTMAP_STATE.LIGHTMAP1;
                    break;
            }
            setLightmap(lightmapState);
        }

        private void setLightmap(LIGHTMAP_STATE state)
        {
            switch (state)
            {
                case LIGHTMAP_STATE.LIGHTMAP1:
                    LightmapSettings.lightmaps = dayLightMaps;
                    break;
                case LIGHTMAP_STATE.LIGHTMAP2:
                    LightmapSettings.lightmaps = nightLightMaps;
                    break;
            }
        }

    }
}