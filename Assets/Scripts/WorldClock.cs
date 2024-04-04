using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldClock : NetworkBehaviour
{

    public int TimeSpeed = 96;
    public GameObject sun;

    private NetworkVariable<byte> day = new NetworkVariable<byte>();
    private NetworkVariable<byte> month = new NetworkVariable<byte>();
    private NetworkVariable<byte> year = new NetworkVariable<byte>();

    private NetworkVariable<byte> hour = new NetworkVariable<byte>();
    private NetworkVariable<byte> minute = new NetworkVariable<byte>();

    public DateTime worldDate;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            DateTime date = RandomDay();
            day.Value = (byte)date.Day;
            month.Value = (byte)date.Month;
            year.Value = (byte)date.Year;

            hour.Value = (byte)Random.Range(0, 24);
            minute.Value = (byte)Random.Range(0, 60);
            //date = date.Date + new TimeSpan(hour.Value, minute.Value, 0);

        }

        worldDate = new DateTime(year.Value, month.Value, day.Value, hour.Value, minute.Value, 0);
        sun.SetActive(true);
    }


    private void FixedUpdate()
    {
        worldDate = worldDate.AddSeconds(TimeSpeed * Time.deltaTime);
    }


    private DateTime RandomDay()
    {
        DateTime start = new DateTime(2030, 1, 1);
        DateTime end = new DateTime(2050, 1, 1);
        int range = (end - start).Days;
        return start.AddDays(Random.Range(0, range));
    }
}
