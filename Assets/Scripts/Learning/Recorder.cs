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

	// Use this for initialization
	void Start () {
        _Recording = false;
        _NetDrive = false;
        _Time = Time.time;
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
        var inputlayer = new DenseLayer(10, 4, Activation.Sigmoid());
        var hiddenlayer1 = new DenseLayer(inputlayer, Activation.Sigmoid(), 10, LayerType.Hidden);
        var hiddenlayer2 = new DenseLayer(hiddenlayer1, Activation.Linear(), 12, LayerType.Hidden);
        var outputlayer = new DenseLayer(hiddenlayer2, Activation.TangesHyperbolic(), 2, LayerType.Output);

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

    void NetDrive()
    {
        _NetDrive = !_NetDrive;
    }


    void ResetCar()
    {
        Car.transform.position = new Vector3(0,0,8);
        Car.transform.eulerAngles = new Vector3(0,90,0);
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
