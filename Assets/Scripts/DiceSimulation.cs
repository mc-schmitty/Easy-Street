using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiceSimulation : MonoBehaviour
{
    private PhysicsScene physicsScene;
    private Scene simulatedScene;
    [SerializeField]
    private int maxSteps = 10000;
    [SerializeField]
    private GameObject floor;
    [SerializeField]
    private GameObject dice;

    // Start is called before the first frame update
    void Start()
    {
        CreateSceneParameters sp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        simulatedScene = SceneManager.CreateScene("Dice Roll", sp);
        physicsScene = simulatedScene.GetPhysicsScene();

        floor = Instantiate(floor);
        floor.gameObject.layer = 8;
        floor.GetComponent<MeshRenderer>().enabled = false;
        dice = Instantiate(dice, dice.transform.position, dice.transform.rotation);
        dice.gameObject.layer = 8;
        SceneManager.MoveGameObjectToScene(floor, simulatedScene);    // Move the floor to scene
        SceneManager.MoveGameObjectToScene(dice, simulatedScene);                                   // Move dice to scene, save reference

        floor.SetActive(false);
        dice.SetActive(false);
    }
    
    public DiceFace SimulateRoll(Vector3 dicePos, Quaternion diceRot, Vector3 rollForce, Vector3 rollAngForce)
    {
        //Activate the sim stuff
        dice.SetActive(true);
        floor.SetActive(true);

        Rigidbody drb = dice.GetComponent<Rigidbody>();
        FaceDetection dfd = dice.GetComponent<FaceDetection>();

        drb.position = dicePos;  // reset position
        drb.rotation = diceRot;

        drb.velocity = Vector3.zero;            // reset velocity
        drb.angularVelocity = Vector3.zero;

        drb.AddForce(rollForce, ForceMode.Impulse);         // Apply force + torque
        drb.AddTorque(rollAngForce, ForceMode.Impulse);

        // Continue simulation steps until velocity is 0 (ignore first step) or steps have reached max steps
        for(int steps = 0; steps < maxSteps && (drb.velocity != Vector3.zero || steps == 0); steps++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);     // Simulate a fixed time physics frame
        }

        // Done simulation, dice has stopped
        DiceFace result = dfd.CheckFace();
        //dice.SetActive(false);
        floor.SetActive(false);
        return result;
    }
}
