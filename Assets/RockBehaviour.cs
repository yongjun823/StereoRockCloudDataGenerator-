﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour
{
    int cnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cnt++;
        transform.rotation = Quaternion.Euler(cnt * 10 + cnt, cnt * 10 + cnt, cnt * 10 + cnt);
    }
}