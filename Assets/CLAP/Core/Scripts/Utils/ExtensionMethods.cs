


using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

       
    /// <summary>
    /// Transforms a Unity Vector3 of floats to a CLAP vector3d of Doubles
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3d ToDoubleVector(this Vector3 vector)
    {
        return new Vector3d(vector.x, vector.y, vector.z);
    }
    /// <summary>
    /// Transforms a Clap Vector3d of Doubles to a CLAP vector3 of floats
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 ToFloatVector(this Vector3d vector)
    {
        return new Vector3((float)vector.x, (float)vector.y, (float)vector.z);
    }
    /// <summary>
    /// Transforms a Clap Vector3d of Doubles to a vector3 of floats
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static void ToFloatVector(this Vector3d vector,out Vector3 vector3)
    {
        vector3.x = (float)vector.x;
        vector3.y = (float)vector.y;
        vector3.z = (float)vector.z;
    }
    /// <summary>
    /// Transforms a Clap Vector3d of Doubles to a vector3 of floats
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 Set(this Vector3 vector,double x,double y,double z)
    {
        vector.x = (float)x;
        vector.y = (float)y;
        vector.z = (float)z;
        return vector;
    }
    public static Vector3 Setf(this Vector3 vector, float x, float y, float z)
    {
        vector.x = x;
        vector.y = y;
        vector.z = z;
        return vector;
    }


    /// <summary>
    /// Transforms a Unity Vector2 of floats to a CLAP vector2d of Doubles
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2d ToDoubleVector(this Vector2 vector)
    {
        return new Vector2d(vector.x, vector.y);
    }
    /// <summary>
    /// Transforms a Clap Vector2d of Doubles to a CLAP vector2 of floats
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2 ToFloatVector(this Vector2d vector)
    {
        return new Vector2((float)vector.x, (float)vector.y);
    }
    public static Vector2 Set(this Vector2 vector,double x,double y)
    {
        vector.x = (float)x;
        vector.y = (float)y;
        return vector;
    }
    /// <summary>
    /// Multiplies each component in the quaternion by the float
    /// </summary>
    /// <param name="q"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static void TimesFloatAndAssign(this Quaternion q, float f)
    {
        q = q.TimesFloat(f);
    }
    /// <summary>
    /// Multiplies a quaternion by a float
    /// </summary>
    /// <param name="a"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static Quaternion TimesFloat(this Quaternion a, float f)
    {
        return new Quaternion(a.x*f, a.y * f, a.z *f, a.w * f);
    }

    /// <summary>
    /// Adds q2 to existing q1
    /// </summary>
    /// <param name="q"></param>
    /// <param name="q2"></param>
    /// <returns></returns>
    public static void AddAndAssign(this Quaternion q, Quaternion q2)
    {
        q = q.Add(q2);
    }


    /// <summary>
    /// Adds two quaternions component to component
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Quaternion Add(this Quaternion a, Quaternion b)
    {
        return new Quaternion(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }


    /// <summary>
    /// Normalize the quaternion
    /// Formula: https://es.mathworks.com/help/aeroblks/quaternionnormalize.html
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public static Quaternion Normalize(this Quaternion q)
    {
        //calculate modulo or length of all 4 components
        float modulo = new Vector4(q.x, q.y, q.z, q.w).SqrMagnitude();

        //divide each component by the modulo.
        return q.TimesFloat(1.0f / modulo);

   }


    public static Quaternion Set(this Quaternion q,double x,double y, double z,double w)
    {
        q.x = (float)x;
        q.y = (float)y;
        q.z = (float)z;
        q.w = (float)w;
        return q;
    }


    /// <summary>
    /// Returns a random ellement from the array, different than the 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="previousIndex"></param>
    /// <returns></returns>
    public static T GetRandomElementNonRepeating<T>(this T[] array, int previousIndex)
    {
        //avoid an infinite loop if only one element in the array
        if (array.Length == 1)
        {
            return array[0];
        }

        int range = array.Length;
        int index = Random.Range(0, range);
              

        while (index == previousIndex)
        {
            index = Random.Range(0, range);
        }
        return array[index];
    }
    /// <summary>
    /// Returns a random element from the array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T GetRandomElement<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static bool[] ToBooleanArray(this int i)
    {
        return System.Convert.ToString(i, 2 /*for binary*/).Select(s => s.Equals('1')).ToArray();
    }

    public static string ToStringElements<T>(this T[] anArray)
    {
        return string.Join(", ", anArray.Select(b => b.ToString()).ToArray());
    }

    public static void Populate<T>(this List<T> arr, int size, T value)
    {
        arr.Clear();
        for (int i = 0; i < size; i++)
        {
            arr.Add(value);
        }
    }

}

