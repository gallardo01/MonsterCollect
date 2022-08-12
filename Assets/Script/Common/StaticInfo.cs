using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInfo : MonoBehaviour
{
    public static string[] mapName =
    {
        "",
        "1.Map Kien 1",
        "2.Map Kien 2",
        "3.Map Kien 3",
        "4.Map Kien 4",
        "5.Map Kien 5",
        "6.Map Kien 6",
        "7.Map Kien 7",
        "8.Map Kien 8",
        "9.Map Kien 9",
        "10.Map Kien 10",
    };

    public static int[] costHeroes = { 0, 0, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 2000 };

    public static int[] statsItem1 = { 2, 3, 5, 6 };
    public static int[] statsItem2 = { 1, 3, 5, 6 };
    public static int[] statsItem3 = { 1, 2, 5, 6 };
    public static int[] statsItem4 = { 3, 4, 5, 6 };

    public static int[,] evolveLevel = {{ 5, 5, 0, 1000 }, { 5, 5, 2, 3000 }, { 10, 5, 3, 5000 }, { 10, 10, 10, 10000 } };

    public static int[] evolLocation = {-600, -1200, -2000,-2800,-3600 };

    public static double TO1BaseValue = 6.99;
    public static double TO1Value = 3.99;
    public static string TO1Description = "Evolve monster pack";

    public static double TO2BaseValue = 9.99;
    public static double TO2Value = 4.99;
    public static string TO2Description = "Pack of gem - Discount 50%";

    public static double TO3BaseValue = 9.99;
    public static double TO3Value = 4.99;
    public static string TO3Description = "Pack of gold - Mastery";

    public static double TO4BaseValue = 9.99;
    public static double TO4Value = 4.99;
    public static string TO4Description = "Powerful Item";

}
