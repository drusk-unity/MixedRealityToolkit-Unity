﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This example script demonstrates various bounds control runtime configurations
    /// </summary>
    public class BoundsControlRuntimeExample : MonoBehaviour
    {
        public TextMeshPro statusText;

        public Material darkGrayMaterial;
        public Material redMaterial;
        public Material cyanMaterial;

        public GameObject boundsVisualsPrefab;

        private bool speechTriggeredFlag;
        private Vector3 cubePosition = new Vector3(0, 1.2f, 2);
        private Vector3 cubeSize = new Vector3(0.5f, 0.5f, 0.5f);
        private BoundsControl boundsControl;

        protected virtual void Start()
        {
            StartCoroutine(Sequence());
        }

        private void SetStatus(string status)
        {
            Debug.Assert(statusText != null, "statusText on BoundsControlRuntimeExample should not be null");
            StringBuilder b = new StringBuilder();
            b.AppendLine($"{status}");
            b.AppendLine($"Press Next or say 'next' to continue");
            statusText.text = b.ToString();
        }

        private IEnumerator Sequence()
        {
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Debug.Assert(darkGrayMaterial != null);
                cube.GetComponent<MeshRenderer>().material = darkGrayMaterial;
                cube.transform.position = cubePosition;
                cube.transform.localScale = cubeSize;

                SetStatus("Instantiate BoundsControl");
                boundsControl = InitializeBoundsControl(cube);
                yield return WaitForButtonPressOrCommand();

                SetStatus("Set Target bounds override");
                var newObject = new GameObject();
                newObject.transform.position = new Vector3(0.8f, 0.8f, 1.8f);
                var bc = newObject.AddComponent<BoxCollider>();
                bc.size = new Vector3(0.162f, 0.1f, 1);
                boundsControl.OverrideBounds = true;
                boundsControl.BoundsOverride = bc.transform;
                yield return WaitForButtonPressOrCommand();

                SetStatus("Change target bounds override size");
                bc.size = new Vector3(0.5f, 0.1f, 1);
                boundsControl.RecomputeBounds();
                yield return WaitForButtonPressOrCommand();

                SetStatus("RotateAnchor Object Origin");
                boundsControl.RotateAnchor = RotateAnchorType.ObjectOrigin;
                yield return WaitForButtonPressOrCommand();

                SetStatus("RotateAnchor Bounds Center");
                boundsControl.RotateAnchor = RotateAnchorType.BoundsCenter;
                yield return WaitForButtonPressOrCommand();

                SetStatus("ScaleAnchor Opposite Corner");
                boundsControl.ScaleAnchor = ScaleAnchorType.OppositeCorner;
                yield return WaitForButtonPressOrCommand();

                SetStatus("ScaleAnchor Bounds Center");
                boundsControl.ScaleAnchor = ScaleAnchorType.BoundsCenter;
                yield return WaitForButtonPressOrCommand();

                SetStatus("Remove target bounds override");
                boundsControl.OverrideBounds = false;
                boundsControl.BoundsOverride = null;
                Destroy(newObject);
                yield return WaitForButtonPressOrCommand();

                SetStatus("FlattenAuto");
                boundsControl.FlattenMode = FlattenMode.Auto;
                yield return WaitForButtonPressOrCommand();

                SetStatus("FlattenAlways");
                boundsControl.FlattenMode = FlattenMode.Always;
                yield return WaitForButtonPressOrCommand();

                SetStatus("FlattenNever");
                boundsControl.FlattenMode = FlattenMode.Never;
                yield return WaitForButtonPressOrCommand();

                SetStatus("BoxPadding 0.2f");
                boundsControl.BoundsPadding = 0.2f;
                yield return WaitForButtonPressOrCommand();

                SetStatus("BoxPadding 0");
                boundsControl.BoundsPadding = 0.0f;
                yield return WaitForButtonPressOrCommand();

                SetStatus("Scale X and update rig");
                cube.transform.localScale = new Vector3(1, 0.5f, 0.5f);
                yield return WaitForButtonPressOrCommand();

                SetStatus("Rotate 20 degrees and update rig");
                cube.transform.localRotation = Quaternion.Euler(0, 20, 0);
                yield return WaitForButtonPressOrCommand();

                SetStatus("HandleType None");
                boundsControl.EnabledHandles = HandleType.None;
                yield return WaitForButtonPressOrCommand();

                SetStatus("HandleType Resize");
                boundsControl.EnabledHandles = HandleType.Resize;
                yield return WaitForButtonPressOrCommand();

                SetStatus("HandleType Rotation");
                boundsControl.EnabledHandles = HandleType.Rotation;
                yield return WaitForButtonPressOrCommand();

                SetStatus("HandleType Scale");
                boundsControl.EnabledHandles = HandleType.Scale;
                yield return WaitForButtonPressOrCommand();

                SetStatus("HandleType Translation");
                boundsControl.EnabledHandles = HandleType.Translation;
                yield return WaitForButtonPressOrCommand();

                Destroy(cube);
            }

            {
                SetStatus("Many children");

                GameObject multiRoot = new GameObject();
                multiRoot.name = "multiRoot";
                Vector3 forwardOffset = Vector3.forward * .5f;
                multiRoot.transform.position = cubePosition + forwardOffset;

                Transform lastParent = null;

                int numCubes = 10;
                for (int i = 0; i < numCubes; i++)
                {
                    var cubechild = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cubechild.GetComponent<MeshRenderer>().material = darkGrayMaterial;
                    cubechild.transform.localPosition = Random.insideUnitSphere + cubePosition + forwardOffset;
                    cubechild.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360f);
                    cubechild.transform.parent = (lastParent != null) ? lastParent : multiRoot.transform;
                    float baseScale = lastParent == null ? 0.1f : 1f;
                    cubechild.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
                    lastParent = cubechild.transform;
                }

                boundsControl = InitializeBoundsControl(multiRoot);

                SetStatus("Randomize Child Scale for skewing");
                yield return WaitForButtonPressOrCommand();

                multiRoot.transform.position += Vector3.forward * 200f;

                var childTransform = multiRoot.transform;
                while (childTransform.childCount > 0)
                {
                    childTransform = childTransform.GetChild(0);
                    float baseScale = lastParent == null ? 0.1f : 1f;
                    childTransform.transform.localScale = new Vector3(baseScale * Random.Range(.5f, 2f), baseScale * Random.Range(.5f, 2f), baseScale * Random.Range(.5f, 2f));
                }

                boundsControl.RecomputeBounds();
                SetStatus("Delete GameObject");
                yield return WaitForButtonPressOrCommand();

                Destroy(multiRoot);
            }

            SetStatus("Done!");
        }

        private BoundsControl InitializeBoundsControl(GameObject target)
        {
            target.AddComponent<ConstraintManager>();
            target.AddComponent<ObjectManipulator>();
            var boundsControl = target.AddComponent<BoundsControl>();
            boundsControl.BoundsVisualsPrefab = boundsVisualsPrefab;

            return boundsControl;
        }

        private IEnumerator WaitForButtonPressOrCommand()
        {
            while (!speechTriggeredFlag)
            {
                yield return null;
            }
            speechTriggeredFlag = false;
        }

        public void OnShouldAdvanceSequence()
        {
            speechTriggeredFlag = true;
        }
    }
}