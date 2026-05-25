using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

public class DiceRoller : MonoBehaviour, IRollDice
{
    public Rigidbody rb;
    public DieFaceDetector faceDetector;

    [SerializeField] private float maxRandomForceValue = 1000;
    [SerializeField] private float startRollingForce = 1000;

    [Tooltip("The amount of time to leave the die in rest after rolling before continuing")]
    [SerializeField] private float restTime = 1.5f;

    private TaskCompletionSource<int> tsc;
    private Vector3 initialPosition;
    private float timer = 0;
    private bool isInRest = false;
    private int result = -1;

    private void Awake()
    {
        initialPosition = transform.position;
        InitDie();
        this.enabled = false;
    }

    void Update()
    {
        WaitFullRestDuration();
    }

    public int GetMaxRollValue()
    {
        return 3;
    }

    public async Task<int> RollDiceAsync()
    {
        tsc = new(TaskCreationOptions.RunContinuationsAsynchronously);
        faceDetector.onDieSettled += OnDieResult;

        RollDie();

        try
        {
            return await tsc.Task;
        }
        finally
        {
            this.enabled = false;
        }
    }

    public void RollDie()
    {
        InitDie();

        this.enabled = true;
        isInRest = false;
        timer = 0;

        var forceX = Random.Range(-maxRandomForceValue, maxRandomForceValue);
        var forceY = Random.Range(-maxRandomForceValue, maxRandomForceValue);
        var forceZ = Random.Range(-maxRandomForceValue, maxRandomForceValue);

        rb.AddForce(Vector3.up * startRollingForce);
        rb.AddTorque(forceX, forceY, forceZ);
    }

    private void InitDie()
    {
        transform.SetPositionAndRotation(initialPosition, new Quaternion(Rand(), Rand(), Rand(), 0));
    }

    private float Rand(float max = 360)
    {
        return Random.Range(0, max);
    }

    private void OnDieResult(int val)
    {
        result = val;
        isInRest = true;
        faceDetector.onDieSettled -= OnDieResult;
    }

    private void WaitFullRestDuration()
    {
        if (!isInRest)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= restTime)
        {
            tsc?.TrySetResult(result);
            isInRest = false;
        }
    }
}