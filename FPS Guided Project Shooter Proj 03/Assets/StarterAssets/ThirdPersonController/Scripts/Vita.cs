using UnityEngine;

public class Vita : MonoBehaviour
{
    public AIController controller;
    public AIController selfController;
    int addHealth = 1000;
    int addSpeed = 50;
    bool trigger = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //If either Vita or Tau have died run this
        if (controller.oneDeadAI == true && !trigger)
        {
            trigger = true;
            Debug.Log("Lets Get this Show rolling");
            //Update stats
            selfController.UpdateStats(addHealth, addSpeed);
        }
    }
}
