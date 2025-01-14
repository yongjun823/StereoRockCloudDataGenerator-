﻿using Pcx;
using System.Linq;
using UnityEngine;
using Microsoft.ML.Probabilistic.Math;
using System.Collections.Generic;

public class PointCamera : MonoBehaviour
{
    private const string Value = "cnt,x,y,z,rx,ry,rz";
    #region Editable attributes
    public string RockName = "";
    #endregion

    int cnt = 0;
    Data.Point[] pointList;

    void Start()
    { 
        using (var file = new System.IO.StreamWriter($"C:\\sr\\{RockName}.csv"))
        {
            file.WriteLine(Value);
        }

        var pcr = GameObject.Find("Rock Cloud").GetComponent<PointCloudRenderer>();
        var computeBuffer = pcr.sourceData.computeBuffer;
        pointList = new Data.Point[computeBuffer.count];
        computeBuffer.GetData(pointList);
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt > 10000)
        {
            return;
        }

        var TransformsMatrix = GetTransformationMatrix(transform.rotation, transform.position);

        var points = pointList.ToList()
             .OrderBy(x => Random.Range(0, pointList.Length))
             .Take(2048)
             .Select(point => CameraTransformation(point.position, TransformsMatrix))
             .ToList();

        WritePointData(points, cnt);
        WritePointInfo(transform.rotation, transform.position, cnt);

        cnt++;
    }

    void WritePointInfo(Quaternion rotate, Vector3 translate, int cnt)
    {
        var euler = rotate.eulerAngles;

        using (var file = new System.IO.StreamWriter($"C:\\sr\\{RockName}.csv", true))
        {
            file.WriteLine($"{cnt},{translate.x},{translate.y},{translate.z},{euler.x},{euler.y},{euler.z}");
        }
    }

    void WritePointData(List<Vector3> points, int cnt)
    {
        using (var file = new System.IO.StreamWriter($@"C:\sr\{RockName}\{cnt:000}.csv"))
        {
            foreach (var point in points)
            {
                file.WriteLine($"{point.x},{point.y},{point.z}");
            }
        }
    }

    Vector3 CameraTransformation(Vector3 point, Matrix tranformsMatrix)
    {
        var PositionVector = new Matrix(new double[4, 1] {
            { point.x }, { point.y }, { point.z }, { 1.0 } });

        var TransPosition = tranformsMatrix * PositionVector;
        var TransVector = new Vector3(
            x: (float)TransPosition[0],
            y: (float)TransPosition[1],
            z: (float)TransPosition[2]);

        return TransVector;
    }

    Matrix GetTransformationMatrix(Quaternion rotate, Vector3 translate)
    {
        var tRotation = new Vector3(
            x: rotate.eulerAngles.x * Mathf.Deg2Rad,
            y: rotate.eulerAngles.y * Mathf.Deg2Rad,
            z: rotate.eulerAngles.z * Mathf.Deg2Rad);

        var fXRotationMatrix = new Matrix(new double[3, 3]
            { { 1.0, 0.0, 0.0 },
            { 0.0, Mathf.Cos(tRotation.x), -Mathf.Sin(tRotation.x) },
            { 0.0, Mathf.Sin(tRotation.x), Mathf.Cos(tRotation.x) } });

        var fYRotationMatrix = new Matrix(new double[3, 3]
            { {Mathf.Cos(tRotation.y), 0.0, Mathf.Sin(tRotation.y) },
            { 0.0, 1.0, 0.0},
            { -Mathf.Sin(tRotation.y), 0.0, Mathf.Cos(tRotation.y)}});

        var fZRotationMatrix = new Matrix(new double[3, 3]
            {{Mathf.Cos(tRotation.z), -Mathf.Sin(tRotation.z), 0.0},
            { Mathf.Sin(tRotation.z), Mathf.Cos(tRotation.z), 0.0 },
            { 0.0, 0.0, 1.0 }});

        var fRotationMatrix = fXRotationMatrix * fYRotationMatrix * fZRotationMatrix;

        var fTransformationMatrix = new Matrix(new double[4, 4]
        {
            {fRotationMatrix[0, 0], fRotationMatrix[0, 1], fRotationMatrix[0, 2], translate.x },
            {fRotationMatrix[1, 0], fRotationMatrix[1, 1], fRotationMatrix[1, 2], translate.y },
            {fRotationMatrix[2, 0], fRotationMatrix[2, 1], fRotationMatrix[2, 2], translate.z },
            {0.0, 0.0, 0.0, 1.0 }
        });

        return fTransformationMatrix;
    }
}
