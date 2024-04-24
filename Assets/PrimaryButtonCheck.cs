using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

[System.Serializable]
public class PrimaryButtonEvent : UnityEvent<bool> { }

public class PrimaryButtonCheck : MonoBehaviour
{
    public PrimaryButtonEvent primaryButtonPressL;
     public PrimaryButtonEvent primaryButtonPressR;
    public PrimaryButtonEvent secondaryButtonPressL;
    public PrimaryButtonEvent secondaryButtonPressR;


    private bool lastPrimStateL = false;
    private bool lastSecStateL = false;
    private bool lastPrimStateR = false;
    private bool lastSecStateR = false;
    private List<InputDevice> devices;

    private void Awake()
    {
        if (primaryButtonPressL == null)
        {
            primaryButtonPressL = new PrimaryButtonEvent();
        }

        devices = new List<InputDevice>();
    }

    void OnEnable()
    {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach(InputDevice device in allDevices)
            InputDevices_deviceConnected(device);

        InputDevices.deviceConnected += InputDevices_deviceConnected;
        InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= InputDevices_deviceConnected;
        InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
        devices.Clear();
    }

    private void InputDevices_deviceConnected(InputDevice device)
    {
        bool discardedValue;
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out discardedValue))
        {
            devices.Add(device); // Add any devices that have a primary button.
        }
    }

    private void InputDevices_deviceDisconnected(InputDevice device)
    {
        if (devices.Contains(device))
            devices.Remove(device);
    }

    void Update()
    {
        bool primStateL = false;
        bool secStateL = false;
        bool primStateR = false;
        bool secStateR = false;

        foreach (var device in devices)
        {
            if(device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                bool primaryButtonState = false;
                primStateL = device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonState) // did get a value
                            && primaryButtonState // the value we got
                            || primStateL; // cumulative result from other controllers

                bool secondaryButtonState = false;
                secStateL = device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonState) // did get a value
                            && secondaryButtonState // the value we got
                            || secStateL; // cumulative result from other controllers
            }

            if(device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                bool primaryButtonState = false;
                primStateR = device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonState) // did get a value
                            && primaryButtonState // the value we got
                            || primStateR; // cumulative result from other controllers

                bool secondaryButtonState = false;
                secStateR = device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonState) // did get a value
                            && secondaryButtonState // the value we got
                            || secStateR; // cumulative result from other controllers
            }
        }

        if (primStateL != lastPrimStateL) // Button state changed since last frame
        {
            primaryButtonPressL.Invoke(primStateL);
            lastPrimStateL = primStateL;
        }

        if (secStateL != lastSecStateL) // Button state changed since last frame
        {
            secondaryButtonPressL.Invoke(secStateL);
            lastSecStateL = secStateL;
        }

        if (primStateR != lastPrimStateR) // Button state changed since last frame
        {
            primaryButtonPressR.Invoke(primStateR);
            lastPrimStateR = primStateR;
        }

        if (secStateR != lastSecStateR) // Button state changed since last frame
        {
            secondaryButtonPressR.Invoke(secStateR);
            lastSecStateR = secStateR;
        }
    }
}