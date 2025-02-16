using UnityEngine;

public class CoordinateGenerators
{
    //bounds : Lower XYZ Higher XYZ

    //Different methods for generating galaxy patterns.
    public static Vector3 Cone(float[] bounds)
    {
        float y = Random.Range(bounds[1], bounds[4]);
        float percentage = y / bounds[4];
        float radius = (bounds[3] * percentage - bounds[0] * percentage) / 2;
        float x = Random.Range(-radius, radius);
        float adjacent = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(x, 2));
        float z = Random.Range(-adjacent, adjacent);
        return new Vector3(x, y, z);
    }

    public static Vector3 Box(float[] bounds)
    {
        float x = Random.Range(bounds[0], bounds[3]);
        float y = Random.Range(bounds[1], bounds[4]);
        float z = Random.Range(bounds[2], bounds[5]);
        return new Vector3(x, y, z);
    }

    public static Vector3 SqureBasedPyramid(float[] bounds)
    {
        float y = Random.Range(bounds[1], bounds[4]);
        float percentage = y / bounds[4];
        float x = Random.Range(bounds[0] * percentage, bounds[3] * percentage);
        float z = Random.Range(bounds[2] * percentage, bounds[5] * percentage);
        return new Vector3(x, y, z);
    }

    public static Vector3 Ovoid(float[] bounds)
    {
        Vector3 vector = Box(bounds).normalized;
        vector.x *= Random.Range(bounds[0], bounds[3]);
        vector.y *= Random.Range(bounds[1], bounds[4]);
        vector.z *= Random.Range(bounds[2], bounds[5]);
        return vector;
    }
}
