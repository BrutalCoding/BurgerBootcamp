using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VR.WSA.Input;
using UnityEngine.UI;
using HoloToolkit.Unity.SpatialMapping;

public class TapRecognizer : MonoBehaviour
{
    //public GameObject ToastModelToTest;
    public TextMesh InfoText;
    private GestureRecognizer recognizer;
    private GameObject clonedObject;
    private string lastTagNameToNormalName;
    public static bool playerHoldingAnObjectNow = false;
    public GameObject foodPlank;

    // Use this for initialization
    void Start()
    {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += MyTapEventHandler;
        recognizer.StartCapturingGestures();
        //foodPlank = GameObject.Find("Snijplank");
    }

    private void MyTapEventHandler(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        RaycastHit hit = new RaycastHit();

        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;

        if (Physics.Raycast(headPosition, gazeDirection, out hit, 30.0f) && !playerHoldingAnObjectNow)
        {
            bool allowToContinue = false;

            //First check if UI buttons have been clicked
            if (hit.collider.gameObject.name == "BtnToggleMeshes")
            {
                SpatialMappingManager.Instance.DrawVisualMeshes = !SpatialMappingManager.Instance.DrawVisualMeshes;
            }
            else if(hit.collider.gameObject.name == "BtnClearFoodPlank")
            {
                foreach(var itemsToDelete in FindObjectsOfType<FixedJoint>())
                {
                    Destroy(itemsToDelete.gameObject);
                    foodPlank.GetComponent<SnapObjects>().TextMyCurrentRecipe.GetComponent<Text>().text = "Try to put some ingredients on this food plank..";
                    foodPlank.GetComponent<SnapObjects>().ingredientsInserted.RemoveAt(foodPlank.GetComponent<SnapObjects>().ingredientsInserted.Count - 1);
                    foodPlank.GetComponent<SnapObjects>().lastSetHeight = GameObject.Find("Snijplank").transform.position.y + 0.03f;
                    foodPlank.GetComponent<SnapObjects>().TextFinished.GetComponent<Text>().text = "Status: \n- Unfinished";
                }
            }
            else
            {
                //After the above UI buttons have been checekd, continue with the grab motion
                string[] validObjectTags = { "Top_Bun", "Salad" , "Bottom_Bun", "Patty", "Plank" };


                foreach (string tag in validObjectTags)
                {

                    if (hit.collider.gameObject.tag == tag)
                    {
                        switch (tag)
                        {
                            case "Top_Bun":
                                lastTagNameToNormalName = "Bun (top)";
                                break;
                            case "Bottom_Bun":
                                lastTagNameToNormalName = "Bun (bottom)";
                                break;
                            default:
                                lastTagNameToNormalName = tag;
                                break;
                        }

                        allowToContinue = true;
                        break;
                    }
                    else
                    {
                        allowToContinue = false;
                    }
                }
            }
            if (allowToContinue)
            {
                playerHoldingAnObjectNow = true;

                var gameObjectHit = hit.collider.gameObject;

                if (gameObjectHit.tag != "Plank")
                {
                    InfoText.text = "Holding " + lastTagNameToNormalName + "...";
                    //Create copy
                    //clonedObject = Instantiate(gameObjectHit, Camera.main.transform.position + gameObjectHit.gameObject.transform.position, Camera.main.transform.localRotation);
                    clonedObject = Instantiate(gameObjectHit, Camera.main.transform.position + gameObjectHit.transform.position, Camera.main.transform.localRotation);
                }
                else
                {
                    InfoText.text = "Put down the plank somewhere";
                    if (gameObjectHit.tag == "Plank")
                    {
                        //Plank is able to be moved, but without making a copy like the ingredients
                        clonedObject = gameObjectHit;
                    }
                }
            }
        }
        else
        {
            InfoText.text = "Released the " + hit.collider.gameObject.tag;
            playerHoldingAnObjectNow = false;
            //Release object to make it user gravity again!
            if (hit.collider.gameObject.tag != "Plank")
            {
                clonedObject.GetComponent<Rigidbody>().isKinematic = false;
                clonedObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (playerHoldingAnObjectNow)
        {
            Quaternion toQuat = Camera.main.transform.localRotation;

            if (clonedObject.tag == "Plank")
            {
                toQuat.eulerAngles = new Vector3(0f, 90f, 0f);
            }
            else if(clonedObject.tag == "Salad")
            {
                toQuat.eulerAngles = new Vector3(-90f, 0f, 0f);
            }
            else
            {
                toQuat.x = 0;
                toQuat.z = 0;
            }


            clonedObject.transform.position = Camera.main.transform.position + new Vector3(0f, 0.0f, 0f) + Camera.main.transform.forward;
            clonedObject.transform.rotation = toQuat;
        }
    }
}
