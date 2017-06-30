using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct Action
{
    public float SteeringAngle;
    public float Tourque;
    public int SteeringNumber;
    public int TourqueNumber;
    public int SteeringCount;
    public int TourgueCount;
}

public class Actions{

    public Action[,] ActionTable;

    public Actions()
    {
        var srw = 1f;
        var trw = 1f;

        var steeringwidth = 2f;
        var tourquewidth = 2f;

        var steeringcount = (int)Math.Ceiling(steeringwidth / srw) + 1;
        var tourquecount = (int)Math.Ceiling(tourquewidth / trw) + 1;

        ActionTable = new Action[steeringcount,tourquecount];

        for(var s = 0; s < steeringcount; s++)
        {
            for(var t = 0; t < tourquecount; t++)
            {
                ActionTable[s, t] = new Action()
                    { SteeringAngle = srw * s - 1, Tourque = trw * t - 1, TourqueNumber = t,
                    SteeringNumber = s, SteeringCount = steeringcount, TourgueCount = tourquecount };
            }
        }
    }

}
