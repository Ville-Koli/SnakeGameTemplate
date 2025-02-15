using UnityEngine;

public class BorderGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D borderTexture;
    GameObject MakeBorderElement(Vector3 loc, Vector3 scale, string name, GameObject parent){
        GameObject border = new GameObject();
        SpriteRenderer sr = border.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 3;
        sr.sprite = Sprite.Create(borderTexture, new Rect(0, 0, borderTexture.width, borderTexture.height), new Vector2(0.5f, 0.5f));
        sr.material.color = Color.black;
        border.transform.position = loc;
        border.transform.localScale = scale;
        border.name = name;
        border.transform.SetParent(parent.transform);
        return border;
    }
    GameObject MakeBorderElement(GameObject inst, Vector3 loc, Vector3 scale, string name, GameObject parent){
        GameObject border = Instantiate(inst);
        border.name = name;
        border.transform.position = loc;
        border.transform.localScale = scale;
        border.transform.SetParent(parent.transform);
        return border;
    }
    /**
    <summary> Function, which generates the borders for the snake game. </summary>
    **/
    public void MakeGameBorders(Vector4 bounds, float offset, float scalar, GameObject parent){
        Vector2 diff = new Vector4(bounds.z - bounds.x, bounds.w - bounds.y);
        Vector3 borderXScale = new Vector3(diff.x * scalar, 1, 1);
        Vector3 borderYScale = new Vector3(1, diff.y * scalar, 1);
        GameObject borderX = MakeBorderElement(
        new Vector3(bounds.x + diff.x/2, bounds.w + offset), borderXScale, "Gameborder X1", parent);
        // then we copy said border and edit its position and scale for the other borders
        GameObject borderX2 = MakeBorderElement(borderX, 
        new Vector3(bounds.x + diff.x/2, bounds.y - offset), borderXScale, "Gameborder X2", parent);
        GameObject borderY1 = MakeBorderElement(borderX, 
        new Vector3(bounds.x - offset, bounds.y + diff.y/2), borderYScale, "Gameborder Y1", parent);
        GameObject borderY2 = MakeBorderElement(borderX, 
        new Vector3(bounds.z + offset, bounds.y + diff.y/2), borderYScale, "Gameborder Y2", parent);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
