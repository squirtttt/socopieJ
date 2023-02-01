//-----------------------------------------------------------------------
// <copyright file="ObjectController.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

/// <summary>
/// Controls target objects behaviour.
/// </summary>
public class Steak_ObjectController : MonoBehaviour
{
    /// <summary>
    /// The material to use when this object is inactive (not being gazed at).
    /// </summary>
    //public Material InactiveMaterial;

    /// <summary>
    /// The material to use when this 
    /// object is active (gazed at).
    /// </summary>
    //public Material GazedAtMaterial;
    public GameObject imageGameobj11_1;
    public GameObject imageGameobj11_2;
    public GameObject imageGameobj12;
    public GameObject imageGameobj13_1;
    public GameObject imageGameobj13_2;
    public GameObject imageGameobj13_3;
    public GameObject imageGameobj13_4;
    public GameObject imageGameobj14;
    public GameObject imageGameobj15;
    public GameObject imageGameobj16;
    public GameObject imageGameobj17;

    //public GameObject audioGameobj;
    // The objects are about 1 meter in radius, so the min/max target distance are
    // set so that the objects are always within the room (which is about 5 meters
    // across).
    private const float _minObjectDistance = 2.5f;
    private const float _maxObjectDistance = 3.5f;
    private const float _minObjectHeight = 0.5f;
    private const float _maxObjectHeight = 3.5f;

    private Renderer _myRenderer;
    private Vector3 _startingPosition;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        _startingPosition = transform.parent.localPosition;
        _myRenderer = GetComponent<Renderer>();
        imageGameobj11_1.SetActive(true);
        imageGameobj11_2.SetActive(false);
        imageGameobj12.SetActive(false);
        imageGameobj13_1.SetActive(false);
        imageGameobj13_2.SetActive(false);
        imageGameobj13_3.SetActive(false);
        imageGameobj13_4.SetActive(false);
        imageGameobj14.SetActive(false);
        imageGameobj15.SetActive(false);
        imageGameobj16.SetActive(false);
        imageGameobj17.SetActive(false);

        //audioGameobj.SetActive(false);
        //SetMaterial(false);
    }

    /// <summary>
    /// Teleports this instance randomly when triggered by a pointer click.
    /// </summary>
    public void TeleportRandomly()
    {
        // Picks a random sibling, activates it and deactivates itself.
        int sibIdx = transform.GetSiblingIndex();
        int numSibs = transform.parent.childCount;
        sibIdx = (sibIdx + Random.Range(1, numSibs)) % numSibs;
        GameObject randomSib = transform.parent.GetChild(sibIdx).gameObject;

        // Computes new object's location.
        float angle = Random.Range(-Mathf.PI, Mathf.PI);
        float distance = Random.Range(_minObjectDistance, _maxObjectDistance);
        float height = Random.Range(_minObjectHeight, _maxObjectHeight);
        Vector3 newPos = new Vector3(Mathf.Cos(angle) * distance, height,
                                     Mathf.Sin(angle) * distance);

        // Moves the parent to the new position (siblings relative distance from their parent is 0).
        transform.parent.localPosition = newPos;

        randomSib.SetActive(true);
        //gameObject.SetActive(false);
        imageGameobj11_1.SetActive(false);
        imageGameobj11_2.SetActive(false);
        imageGameobj12.SetActive(false);
        imageGameobj13_1.SetActive(false);
        imageGameobj13_2.SetActive(false);
        imageGameobj13_3.SetActive(false);
        imageGameobj13_4.SetActive(false);
        imageGameobj14.SetActive(false);
        imageGameobj15.SetActive(false);
        imageGameobj16.SetActive(false);
        imageGameobj17.SetActive(false);
        //audioGameobj.SetActive(false);
        //SetMaterial(false);
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter()
    {
        //SetMaterial(true);
        imageGameobj11_1.SetActive(true);
        Destroy(imageGameobj11_1, 9);
        imageGameobj11_2.SetActive(true);
        Destroy(imageGameobj11_2, 6);
        imageGameobj12.SetActive(true);
        Destroy(imageGameobj12, 9);
        imageGameobj13_1.SetActive(true);
        Destroy(imageGameobj13_1, 7);
        imageGameobj13_2.SetActive(true);
        Destroy(imageGameobj13_2, 16);
        imageGameobj13_3.SetActive(true);
        Destroy(imageGameobj13_3, 9);
        imageGameobj13_4.SetActive(false);
        Destroy(imageGameobj13_4, 7);
        imageGameobj14.SetActive(true);
        Destroy(imageGameobj14, 4);
        imageGameobj15.SetActive(true);
        Destroy(imageGameobj15, 11);
        imageGameobj16.SetActive(true);
        Destroy(imageGameobj16, 11); 
        imageGameobj17.SetActive(true);
        Destroy(imageGameobj17, 11);
        //audioGameobj.SetActive(true);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
        //SetMaterial(false);
        imageGameobj11_1.SetActive(false);
        imageGameobj11_2.SetActive(false);
        imageGameobj12.SetActive(false);
        imageGameobj13_1.SetActive(false);
        imageGameobj13_2.SetActive(false);
        imageGameobj13_3.SetActive(false);
        imageGameobj13_4.SetActive(false);
        imageGameobj14.SetActive(false);
        imageGameobj15.SetActive(false);
        imageGameobj16.SetActive(false);
        imageGameobj17.SetActive(false);
        //audioGameobj.SetActive(false);

    }

    /// <summary>
    /// This method is called by the Main Camera when it is gazing at this GameObject and the screen
    /// is touched.
    /// </summary>
    public void OnPointerClick()
    {
        TeleportRandomly();
    }

    /// <summary>
    /// Sets this instance's material according to gazedAt status.
    /// </summary>
    ///
    /// <param name="gazedAt">
    /// Value `true` if this object is being gazed at, `false` otherwise.
    /// </param>
    /*private void SetMaterial(bool gazedAt)
    {
        if (InactiveMaterial != null && GazedAtMaterial != null)
        {
            _myRenderer.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
        }
    }*/
}
