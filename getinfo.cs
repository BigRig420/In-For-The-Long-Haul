using TMPro;
using System;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class getinfo : MonoBehaviour
{
    [SerializeField] private float updateInterval = 1;

    public TextMeshProUGUI processorType;
    public TextMeshProUGUI processorCount;
    [SerializeField] private TMP_Text cpuCounterText;
    public float CpuUsage;
    private float _lasCpuUsage;
    private Thread _cpuThread;
    

    public TextMeshProUGUI graphicsDeviceName;
    [SerializeField] private TMP_Text gpuCounterText;
    public float GpuUsage;
    private float _lasGpuUsage;
    private Thread _gpuThread;


    void Start()
    {
        //cpu
        //Debug.Log(SystemInfo.processorCount);
        processorType.text = (SystemInfo.processorType);
        processorCount.text = (SystemInfo.processorCount).ToString();
        cpuCounterText.text = "0% CPU";
        //processorFrequency.text = ((SystemInfo.processorFrequency) / 1000 + " MHz").ToString();
        _cpuThread = new Thread(UpdateCPUUsage)
        {
            IsBackground = true,
            // we don't want that our measurement thread
            // steals performance
            Priority = System.Threading.ThreadPriority.BelowNormal
        };
        _cpuThread.Start();

        //gpu
        graphicsDeviceName.text = (SystemInfo.graphicsDeviceName); 
        gpuCounterText.text = "0% GPU";
        _gpuThread = new Thread(UpdateGPUUsage)
        {
            IsBackground = true,
            // we don't want that our measurement thread
            // steals performance
            Priority = System.Threading.ThreadPriority.BelowNormal
        };
        _gpuThread.Start();

        //graphicsMemoerySize.text = (SystemInfo.graphicsMemorySize).ToString();
    }

    private void OnDestroy()
    {
        _cpuThread?.Abort();
        _gpuThread?.Abort();
    }

    private void Update()
    {
        //cpu
        if (Mathf.Approximately(_lasCpuUsage, CpuUsage)) return;

        if (CpuUsage < 0 || CpuUsage > 100) return;

        cpuCounterText.text = CpuUsage.ToString("F1") + "% CPU";

        _lasCpuUsage = CpuUsage;


        //gpu
        if (Mathf.Approximately(_lasGpuUsage, GpuUsage)) return;

        if (GpuUsage < 0 || GpuUsage > 100) return;

        gpuCounterText.text = GpuUsage.ToString("F1") + "% GPU";

        _lasGpuUsage = GpuUsage;
    }

    //cpu
    private void UpdateCPUUsage()
    {
        var lastCpuTime = new TimeSpan(0);

        while (true)
        {
            var cpuTime = new TimeSpan(0);

            var AllProcesses = Process.GetProcesses();

            cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);

            var newCPUTime = cpuTime - lastCpuTime;

            lastCpuTime = cpuTime;

            CpuUsage = 100f * (float)newCPUTime.TotalSeconds / updateInterval;

            Thread.Sleep(Mathf.RoundToInt(updateInterval * 1000));
        }
    }

    //gpu
    private void UpdateGPUUsage()
    {
        var lastGpuTime = new TimeSpan(0);

        while (true)
        {
            var gpuTime = new TimeSpan(0);

            var AllProcesses = Process.GetProcesses();

            gpuTime = AllProcesses.Aggregate(gpuTime, (current, process) => current + process.TotalProcessorTime);

            var newGPUTime = gpuTime - lastGpuTime;

            lastGpuTime = gpuTime;

            GpuUsage = 100f * (float)newGPUTime.TotalSeconds / updateInterval;

            Thread.Sleep(Mathf.RoundToInt(updateInterval * 1000));
        }
    }
}
