using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Rendering.PostProcessing;

public class Kart : MonoBehaviour {
    [SerializeField] private Transform kartModel;
    [SerializeField] private Transform kartNormal;
    [SerializeField] private Rigidbody sphere;

    private float speed, currentSpeed;
    private float rotate, currentRotate;
    private int driftDirection;
    private float driftPower;
    private int driftMode;
    private bool firstTier, secondTier, thirdTier;
    private Color c;

    [Header("Bools")]
    [SerializeField] private bool drifting;

    [Header("Kart Parts")]
    [SerializeField] private Transform frontWheels;
    [SerializeField] private Transform rearWheels;
    [SerializeField] private Transform steeringWheel;

    [Header("Particle Effects")]
    [SerializeField] private Transform wheelParticles;
    [SerializeField] private Transform flashParticles;
    [SerializeField] private Color[] turboColours;

} 
