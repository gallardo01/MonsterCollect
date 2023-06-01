﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInfo : MonoBehaviour
{
    public static string[] mapName =
    {
        "",
        "1. Gym Leader Blue",
        "2. Gym Leader Roxie",
        "3. Gym Leader Iris",
        "4. Gym Leader Drake",
        "5. Gym Leader Jack",
        "6. Gym Leader Blaine",
        "7. Gym Leader Clair",
        "8. Gym Leader Monty",
        "9. Gym Leader Cindy",
        "10. Gym Leader Surge",
    };

    public static string[] mapType =
    {
        "",
        "<sprite=13> Water Gym",
        "<sprite=11> Fire Gym",
        "<sprite=12> Electric Gym",
        "<sprite=13> Water Gym",
        "<sprite=11> Fire Gym",
        "<sprite=14> Grass Gym",
        "<sprite=13> Water Gym",
        "<sprite=11> Fire Gym",
        "<sprite=14> Grass Gym",
        "<sprite=12> Electric Gym",
    };

    public static int[] costHeroes = { 0, 0, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 2000 };

    //public static int[,] evolveLevel = {{ 5, 5, 0, 1000 }, { 5, 5, 2, 3000 }, { 10, 5, 3, 5000 }, { 10, 10, 10, 10000 } };

    public static int[] evolLocation = {0, -800, -1600,-2400,-3200 };
    public static int[] userUpdateBase = { 10, 10, 10, 10, 10, 10, 1, 1, 1};


    public static double[] TOBaseValue = { 6.99, 9.99, 9.99, 9.99};
    public static double[] TOValue = { 3.99, 4.99, 4.99, 4.99};
    public static string[] TODescription = { "Evolve monster pack", "Pack of gem - Discount 50%", "Pack of gold - Mastery", "Powerful Item" };
    public static string[] TOPriceType = { "dollar", "dollar", "dollar", "dollar" };

    public static string Chest1Name = "Golden Chest";
    public static int Chest1Price = 1000;
    public static string Chest1PriceType = "coin";

    public static string Chest2Name = "Diamond Chest";
    public static int Chest2Price = 200;
    public static string Chest2PriceType = "gem";

    public static string Chest3Name = "Diamond Chest X10";
    public static int ChestBasePrice = 2000;
    public static int Chest3Price = 1800;
    public static string Chest3PriceType = "gem";


    public static string[] skillName =
    {
        "",
        "Thunder Wave",
        "Electroweb",
        "Electro Ball",
        "Thunder Bolt",
        "Thunder Shock",
        "Discharge",
        "Charge",
        "Electrify",
        "Spark",
        "Magnet Rise",
        "Max Lightning",
        "Zap Cannon",
    };

    public static string[] skillContent =
    {
        "",
        "Deal dame to closet enemy",
        "Throw a web on random target area  ",
        "Electric Ball fly around player",
        "Fires a thunder explosion",
        "Bounce around all enemy",
        "Generates a dissolving thunder forcefield",
        "Atk +",
        "Gold +",
        "Speed +",
        "Super effective dame +",
        "Crit dame +",
        "Defense +",
    };


    public static int[] skillDame =
    {
        0, 30, 40, 25, 45, 30, 30, 10, 10, 10, 20, 10, 10,
    };
}
