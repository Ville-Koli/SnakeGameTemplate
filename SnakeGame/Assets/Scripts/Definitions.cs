using System;
using UnityEngine;

public class Logic{
    /**
    <summary> 
    Function, which sets a vector to be in bounds 
    </summary>
    <returns> vector, which is in bounds </returns>
    **/
    public static Vector2 KeepInBounds(Vector2 vector, Vector4 bounds){
        Vector2 boundedVector = vector;
        if(vector.x < bounds.x){
            boundedVector.x = bounds.z;
        }else if(vector.x > bounds.z){
            boundedVector.x = bounds.x;
        }
        if(vector.y < bounds.y){
            boundedVector.y = bounds.w;
        }else if(vector.y > bounds.w){
            boundedVector.y = bounds.y;
        }
        return boundedVector;
    }
    public static bool IsOutOfBounds(Vector2 vector, Vector4 bounds){
        bool IsOutOfBounds = false;
        if(vector.x < bounds.x){
            IsOutOfBounds = true;
        }else if(vector.x > bounds.z){
            IsOutOfBounds = true;
        }
        if(vector.y < bounds.y){
            IsOutOfBounds = true;
        }else if(vector.y > bounds.w){
            IsOutOfBounds = true;
        }
        return IsOutOfBounds;
    }
    /**
    <summary> 
    Function, which sets a vector to be in bounds 
    </summary>
    <returns> vector, which is in bounds </returns>
    **/
    public static bool IsInMarginOfError(Vector3 a, Vector3 b, float error){
        return Math.Abs((a.x - b.x)/a.x) < error && Math.Abs((a.y - b.y)/a.y) < error;
    }

    public static Vector3 TurnVectorLeft(Vector3 vector){
        return new Vector3(-vector.y, vector.x);
    }
    public static Vector3 TurnVectorRight(Vector3 vector){
        return new Vector3(vector.y, -vector.x);
    }
}
