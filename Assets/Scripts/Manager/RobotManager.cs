﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    [Header("Subscribers- Float")]
    public RosSharp.RosBridgeClient.FloatSubscriber turretVelocity;
    public RosSharp.RosBridgeClient.FloatSubscriber turretVerticalOffset;
    public RosSharp.RosBridgeClient.FloatSubscriber flywheelWantedRpm;
    public RosSharp.RosBridgeClient.FloatSubscriber hoodWantedAngle;
    public RosSharp.RosBridgeClient.FloatSubscriber turretWantedAngle;

    [Header("Subscribers- String")]
    public RosSharp.RosBridgeClient.StringSubscriber intakeState;
    public RosSharp.RosBridgeClient.StringSubscriber shooterState;
    public RosSharp.RosBridgeClient.StringSubscriber flywheelState;
    public RosSharp.RosBridgeClient.StringSubscriber turretState; 
    public RosSharp.RosBridgeClient.StringSubscriber hoodState;

    [Header("Subsystem Controls")]
    public GameObject turret;
    public GameObject hood;
    public GameObject flywheel;
    public GameObject intake;

    private ShooterControl shooterControl;
    private TurretControl turretControl;
    private HoodControl hoodControl;
    private FlywheelControl flywheelControl;
    private IntakeControl intakeControl;

    void Awake()
    {
        shooterControl = turret.GetComponent<ShooterControl>();
        turretControl = turret.GetComponent<TurretControl>();
        hoodControl = hood.GetComponent<HoodControl>();
        flywheelControl = flywheel.GetComponent<FlywheelControl>();
        intakeControl = intake.GetComponent<IntakeControl>();
    }

    void FixedUpdate()
    {
        // Intake
        if (intakeState.getData() == "deploy")
            intakeControl.deployIntake();
        else if (intakeState.getData() == "retract")
            intakeControl.retractIntake();

        if (shooterState.getData() == "idle")
        {
            // Turret
            if (turretState.getData() == "rotate_turret")
                turretControl.setAngle(turretWantedAngle.getData());
            else
                turretControl.setIdle();

            // Flywheel
            if (flywheelState.getData() == "spin_up")
                flywheelControl.setVelocity(flywheelWantedRpm.getData());
            else
                flywheelControl.setIdle();

            // Hood
            if (hoodState.getData() == "rotate_hood")
                hoodControl.setAngle(hoodWantedAngle.getData());
            else
                hoodControl.setIdle();

            shooterControl.setIdle();

        }
        else if (shooterState.getData() == "prime")
        {
            var hoodAngle = (((Mathf.Exp(-0.106665f * turretVerticalOffset.getData()) * 40.5628f) + 1.0f) / 89.8105f) + 43.3079f;
            var flywheelRpm = (((Mathf.Exp(-0.092559f * turretVerticalOffset.getData()) * 24.7655f) + 1.0f) / 7795.87f) + 1810.29f;
            shooterControl.setPrime(turretVelocity.getData(), hoodAngle, flywheelRpm);
        }
        else if (shooterState.getData() == "shoot")
        {
            var hoodAngle = (((Mathf.Exp(-0.106665f * turretVerticalOffset.getData()) * 40.5628f) + 1.0f) / 89.8105f) + 43.3079f;
            var flywheelRpm = (((Mathf.Exp(-0.092559f * turretVerticalOffset.getData()) * 24.7655f) + 1.0f) / 7795.87f) + 1810.29f;
            shooterControl.setPrime(turretVelocity.getData(), hoodAngle, flywheelRpm);
            shooterControl.setShoot();
        }
    }
}