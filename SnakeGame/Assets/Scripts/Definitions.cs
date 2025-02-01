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
}
