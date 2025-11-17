using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Scriptable Objects/Category")]
public class Category : ScriptableObject
{
    [SerializeField] string categoryName;

    public string CategoryName { get { return categoryName; } set { categoryName = value; } }
}
