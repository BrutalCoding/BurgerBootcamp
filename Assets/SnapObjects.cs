using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SnapObjects : MonoBehaviour {
    public TextMesh InfoText;
    public GameObject TextFinished;
    public GameObject TextMyCurrentRecipe;
    public float originalHeight;
    public float lastSetHeight;
    public List<string> ingredientsInserted = new List<string>() { };
    // Use this for initialization
    void Start () {
        InfoText.text = "Food plank";
        TextMyCurrentRecipe.GetComponent<Text>().text = "Put some ingredients first on this food plank..";
        originalHeight = gameObject.transform.position.y;
    }

    void OnTriggerEnter(Collider hit)
    {
        InfoText.text = "Not able to place " + hit.tag;
        string[] validObjectTags = { "Top_Bun", "Bottom_Bun", "Patty", "Salad" };

        bool allowToContinue = false;
        foreach (string tag in validObjectTags)
        {
            if (hit.tag == tag)
            {
                allowToContinue = true;
                break;
            }
            else
            {
                allowToContinue = false;
            }
        }

       
        if (allowToContinue)
        {
            //Static recipe for demo purpose only
            List<string> ingredientsToFollow = new List<string>() { "Top_Bun", "Salad", "Patty", "Top_Bun" };
            

            var gameObjectHit = hit.gameObject;

            gameObjectHit.GetComponent<Rigidbody>().useGravity = true;
            gameObjectHit.GetComponent<Rigidbody>().isKinematic = false;

            //Decide the correct rotation for the deteted object
            switch(hit.tag)
            {
                case "Top_Bun":
                case "Bottom_Bun":
                    gameObjectHit.transform.Rotate(new Vector3(-90f, 0f, 0f));
                    break;
                case "Patty":
                    gameObjectHit.transform.Rotate(new Vector3(0f, 0f, 0f));
                    break;
            }

            //Add ingredient to the list
            ingredientsInserted.Add(hit.tag);
            

            //Remove boxcollider so it wont be able to be duplicated again
            gameObjectHit.GetComponent<BoxCollider>().enabled = false;

            //Put the new object a little higher than the previous one!
            lastSetHeight += 0.03f;

            //Set the detected object position
            //gameObjectHit.transform.position = new Vector3(gameObject.transform.position.x, (gameObject.transform.position.y + lastSetHeight), gameObject.transform.position.z);

            //Make the clone position as the center of the parent by zeroing
            Vector3 newPositionInsideParent = gameObject.transform.position;
            newPositionInsideParent.y += lastSetHeight;
            gameObjectHit.transform.position = newPositionInsideParent;

            //Create joint between objects to make it stick
            gameObjectHit.AddComponent<FixedJoint>().connectedBody = gameObject.GetComponent<Rigidbody>();

            //Delete the old object from the users hand and notify user
            InfoText.text = hit.tag + " placed";
            Destroy(hit.gameObject);
            TapRecognizer.playerHoldingAnObjectNow = false;

            //GameObject clone = Instantiate(gameObjectHit, Camera.main.transform.position + gameObjectHit.gameObject.transform.position, Camera.main.transform.localRotation);
            GameObject clone = Instantiate(gameObjectHit);

            //Check if recipe is finished
            if(ingredientsInserted.SequenceEqual(ingredientsToFollow))
            {
                //Finished
                TextFinished.GetComponent<Text>().text = "Status: \n- Finished";
            }
            else
            {
                //Unfinished
                TextFinished.GetComponent<Text>().text = "Status: \n- Unfinished";
            }

            TextMyCurrentRecipe.GetComponent<Text>().text = "";
            foreach(string ingredient in ingredientsInserted)
            {
                TextMyCurrentRecipe.GetComponent<Text>().text += ingredient + " + ";
            }
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        InfoText.text = "Place here!";
    }

    // Update is called once per frame
    void Update () {
    }
}
