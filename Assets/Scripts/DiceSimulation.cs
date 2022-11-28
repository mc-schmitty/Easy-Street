using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

struct WorseTransform
{
    public Vector3 position;
    public Quaternion rotation;
}

public class DiceSimulation : MonoBehaviour
{
    private PhysicsScene physicsScene;
    private Scene simulatedScene;
    [SerializeField]
    private int maxSteps = 10000;
    [SerializeField]
    private GameObject floor;
    [SerializeField]
    private GameObject[] dice;
    private int diceLength;

    // Start is called before the first frame update
    void Start()
    {
        CreateSceneParameters sp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        simulatedScene = SceneManager.CreateScene("Dice Roll", sp);
        physicsScene = simulatedScene.GetPhysicsScene();

        floor = Instantiate(floor);
        floor.gameObject.layer = 8;
        floor.GetComponent<MeshRenderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(floor, simulatedScene);    // Move the floor to scene
        floor.SetActive(false);

        diceLength = dice.Length;
        for(int i = 0; i < diceLength; i++)
        {
            GameObject ndie = Instantiate(dice[i], dice[i].transform.position, dice[i].transform.rotation);
            dice[i] = ndie;
            ndie.gameObject.layer = 8;
            SceneManager.MoveGameObjectToScene(ndie, simulatedScene);                                   // Move dice to scene, save reference
            ndie.SetActive(false);
        }
    }
    
    public DiceFace[] SimulateRoll(bool[] enabledDice, Vector3[] dicePos, Quaternion[] diceRot, Vector3[] rollForce, Vector3[] rollAngForce)
    {
        //Activate the sim stuff
        floor.SetActive(true);

        Rigidbody[] drb = new Rigidbody[diceLength];
        FaceDetection[] dfd = new FaceDetection[diceLength];

        for(int i = 0; i < diceLength; i++)
        {
            if (enabledDice[i])
            {
                dice[i].SetActive(true);

                drb[i] = dice[i].GetComponent<Rigidbody>();
                dfd[i] = dice[i].GetComponent<FaceDetection>();

                drb[i].position = dicePos[i];  // reset position
                drb[i].rotation = diceRot[i];

                drb[i].velocity = Vector3.zero;            // reset velocity
                drb[i].angularVelocity = Vector3.zero;

                drb[i].AddForce(rollForce[i], ForceMode.Impulse);         // Apply force + torque
                drb[i].AddTorque(rollAngForce[i], ForceMode.Impulse);
            }
        }

        
        // Continue simulation steps until velocity is 0 (ignore first step) or steps have reached max steps
        for(int steps = 0; steps < maxSteps && (TestForVelocity(drb) || steps == 0); steps++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);     // Simulate a fixed time physics frame
        }

        // Done simulation, dice have stopped
        DiceFace[] result = new DiceFace[diceLength];
        for (int i = 0; i < diceLength; i++)
        {
            if (enabledDice[i])
            {
                result[i] = dfd[i].CheckFace();
                //dice[i].SetActive(false);
            }
            else
            {
                result[i] = 0;
            }
            
        }

        floor.SetActive(false);
        return result;
    }

    public bool TestForVelocity(Rigidbody[] rbList)
    {
        foreach(Rigidbody r in rbList)
        {
            if(r != null && r.velocity != Vector3.zero && r.angularVelocity != Vector3.zero)
            {
                return true;
            }
        }

        return false;
    }
}
