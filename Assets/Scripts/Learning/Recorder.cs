using Assets.Scripts.Learning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NNC;
using NNC.Network.Layers;
using NNC.Network;
using NNC.Network.Tensors;
using System;
using Assets.Scripts.Learning.QLearning;

public class Recorder : MonoBehaviour {
    private bool _Training;
    private bool _Recording;
    private bool _NetDrive;
    public float DeltaTime;
    public List<DriveSnapShot> Record;
    public Button Button;
    public Text ErrorText;
    public GameObject Car;
    private float _Time;
    private DenseLayer Network;
    private List<Transition> _Transitions;
    private bool _NetSelfTrain;
    private DriveSnapShot _LastSnap;
    private Tensor1D _LastInput;
    private Tensor1D _LastOutput;
    private Actions _Actions;
    private bool _SelfTraining;
    private float PreviousReward;
    private float _LastTimeSpeedHigh;
    private float _ActionTime;
    private float _DrivingTime;
    private Vector3 LastPos;
    

	// Use this for initialization
	void Start () {
        _Recording = false;
        _NetDrive = false;
        _Time = Time.time;
        _Transitions = new List<Transition>();
        _Actions = new Actions();
        _SelfTraining = false;
    }
	
	// Update is called once per frame
	void Update () {
      
	}

    void FixedUpdate()
    {
        if (_Recording)
        {

            var currenttime = Time.time;

            if (currenttime - _Time > DeltaTime)
            {
                _Time = currenttime;
                Record.Add(GrabDriveSnapShot());
            }

        }
        if (_NetDrive)
        {

            var drivecontroller = Car.GetComponent<RearWheelDrive>();
            var sensors = Car.GetComponentsInChildren<DetectionLaser>();

            drivecontroller.InputControl = false;

            var snap = GrabDriveSnapShot();

            var input = new Tensor1D(new float[] { snap.Speed, snap.SensorLeft, snap.SensorFront, snap.SensorRight });

            var result = (Tensor1D)Network.Forward(input);

            drivecontroller.SteeringAngle = result[0];
            drivecontroller.Tourque = result[1];
        }
        else if (_NetSelfTrain && Network != null)
        {
            var drivecontroller = Car.GetComponent<RearWheelDrive>();
            var sensors = Car.GetComponentsInChildren<DetectionLaser>();

            if (_SelfTraining)
            {
                return;
            }

            var snap = GrabDriveSnapShot();
            var speed = drivecontroller.Speed;
            var speedhigh = speed > 1.5;
            //var rr = 20 * Vector3.Distance(LastPos, Car.transform.position) / 2;
            var reward = -(snap.SensorFront + snap.SensorLeft + snap.SensorRight) + speed / 10; // +  rr;

            if (speedhigh || _LastTimeSpeedHigh == 0)
            {
                _LastTimeSpeedHigh = Time.time;
            }

            else if(Time.time - _LastTimeSpeedHigh > 3 || drivecontroller.Collided)
            {
                AddSample(drivecontroller, -5);
                _SelfTraining = true;
                StartCoroutine("SelfTrainEpoch");
                drivecontroller.Tourque = 0;
                drivecontroller.SteeringAngle = 0;
                return;
            }

            AddSample(drivecontroller, reward);
            
        }
        else
        {
            var drivecontroller = Car.GetComponent<RearWheelDrive>();
            drivecontroller.InputControl = true;
        }
    }

    DriveSnapShot GrabDriveSnapShot()
    {
        var drivecontroller = Car.GetComponent<RearWheelDrive>();
        var sensors = Car.GetComponentsInChildren<DetectionLaser>();

        var snapshot = new DriveSnapShot();
        snapshot.Speed = drivecontroller.Speed;
        snapshot.SteeringAngle = drivecontroller.SteeringAngle;
        snapshot.Tourgue = drivecontroller.Tourque;
        snapshot.SensorLeft = sensors.Where(s => s.Name == "L").First().Distance;
        snapshot.SensorFront = sensors.Where(s => s.Name == "F").First().Distance;
        snapshot.SensorRight = sensors.Where(s => s.Name == "R").First().Distance;

        return snapshot;
    }


    void AddSample(RearWheelDrive drivecontroller, float reward)
    {
        drivecontroller.InputControl = false;

        var snap = GrabDriveSnapShot();

        var input = new Tensor1D(new float[] { snap.Speed, snap.SensorLeft, snap.SensorFront, snap.SensorRight });

        var result = (Tensor1D)Network.Forward(input);

        var outlist = result.Tolist();

        var steeringdim = _Actions.ActionTable.GetLength(0);
        var trouquedim = _Actions.ActionTable.GetLength(1);

        //a.SteeringNumber* a.SteeringCount + a.TourgueNumber = index

        var maxindex = outlist.IndexOf(outlist.Max());

        var _rnd = new System.Random();
        var randomdouble = _rnd.NextDouble();
        if (0.2 >= randomdouble)
        {
            maxindex = _rnd.Next(0, outlist.Count);
        }

        var currenttime = Time.time;

        if (currenttime - _ActionTime > 0.2)
        {
            _ActionTime = currenttime;
        }
        else
        {
            return;
        }

        var steeringindex = maxindex / steeringdim;
        var touqueindex = maxindex % steeringdim;

        var action = _Actions.ActionTable[steeringindex, touqueindex];

        drivecontroller.SteeringAngle = action.SteeringAngle;
        drivecontroller.Tourque = Math.Max(action.Tourque, 0);

        var transition = new Transition();
        if (_LastSnap != null)
        {

            transition.InputA = _LastInput;
            transition.InputB = input;

            transition.OutputA = _LastOutput;
            transition.OutputB = result;

            transition.Reward = PreviousReward;

            transition.Action = action;

            currenttime = Time.time;

            if (currenttime - _Time > 0.2)
            {
                _Time = currenttime;
                _Transitions.Add(transition);
            }

        }

        _LastSnap = snap;
        _LastInput = input;
        _LastOutput = result;
        PreviousReward = reward;
        LastPos = Car.transform.position;
    }

    void RecordButton()
    {
        var text = Button.GetComponentInChildren<Text>();
        if (!_Recording) {
            _Recording = true;
            text.text = "Stop Recording!";
            Record = new List<DriveSnapShot>();
        }
        else
        {
            _Recording = false;
            text.text = "Record!";
        }
    }


    void SaveRecord()
    {
        if (_Recording)
        {
            RecordButton();
        }

        Serializer.Serialize(Record, "record.dat");
    }

    void LoadRecord()
    {
        if (_Recording)
        {
            RecordButton();
        }
        Record = Serializer.Deserialize<List<DriveSnapShot>>("record.dat");
    }

    void TrainOnRecord()
    {
        if (_Training)
        {
            _Training = false;
        }
        else
        {
            _Training = true;
            StartCoroutine("TrainNetwork");
        }

    }

    IEnumerator TrainNetwork()
    {

        var inputlayer = new DenseLayer(10, 4, Activation.ReLU());
        var hiddenlayer1 = new DenseLayer(inputlayer, Activation.ReLU(), 30, LayerType.Hidden);
        var hiddenlayer2 = new DenseLayer(hiddenlayer1, Activation.ReLU(), 500, LayerType.Hidden);
        var outputlayer = new DenseLayer(hiddenlayer1, Activation.TangesHyperbolic(), 2, LayerType.Output);

        Network = outputlayer;

        outputlayer.Initilize();

        var trainingdata = GenerateTrainingData();

        int epoch = 0;
        int epochsize = trainingdata.GetLength(0);
        var epocherror = float.MaxValue;
        yield return 0;
        while (epocherror > 10 && _Training)
        {
            yield return 0;
            epocherror = 0f;
            epoch++;
            for (var t = 0; t < epochsize; t++)
            {

                var truth = trainingdata[t, 1];
                var input = trainingdata[t, 0];
                
                var output = (Tensor1D)outputlayer.Forward(input);
                var dif = output - truth;
                var sq = dif * dif;
                epocherror += (float)Math.Pow(sq.ElementSum(), 2);
                outputlayer.Backward(dif);
            }
            ErrorText.text = epocherror.ToString();
        }
        ErrorText.text = ("Finished!");
        _Training = false;
    }


    void SelfTrainNetwork()
    {
        _NetSelfTrain = true;

        var inputlayer = new DenseLayer(10, 4, Activation.Sigmoid());
        var hiddenlayer1 = new DenseLayer(inputlayer, Activation.Sigmoid(), 100, LayerType.Hidden);
        var hiddenlayer2 = new DenseLayer(hiddenlayer1, Activation.Sigmoid(), 20, LayerType.Hidden);
        var outputlayer = new DenseLayer(hiddenlayer2, Activation.Linear(), 9, LayerType.Output);

        Network = outputlayer;

        Network.Initilize();
        _NetSelfTrain = true;
    }

    void SelfTrainEpoch()
    {
        var learner = new QTraining();
        learner.TrainAction(_Transitions, Network);
        _LastTimeSpeedHigh = Time.time;
        _SelfTraining = false;
        ResetCar();
        _DrivingTime = Time.time;
        var drivecontroller = Car.GetComponent<RearWheelDrive>();
        drivecontroller.Collided = false;
        var removenumber = Math.Max(_Transitions.Count() - 500, 0);
        _Transitions.RemoveRange(0, removenumber);
    }

    void NetDrive()
    {
        _NetDrive = !_NetDrive;
    }


    void ResetCar()
    {
        Car.transform.position = new Vector3(0,0,8);
        Car.transform.eulerAngles = new Vector3(0,90,0);
        var rb = Car.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
    }


    Tensor1D[,] GenerateTrainingData()
    {
        var trainingdata = new Tensor1D[Record.Count, 2];

        for (var i = 0; i < Record.Count; i++)
        {
            trainingdata[i, 0] = new Tensor1D(new float[] { Record[i].Speed, Record[i].SensorLeft, Record[i].SensorFront, Record[i].SensorRight });
            trainingdata[i, 1] = new Tensor1D(new float[] { Record[i].SteeringAngle, Record[i].Tourgue});
        }

        return trainingdata;
    }
}
