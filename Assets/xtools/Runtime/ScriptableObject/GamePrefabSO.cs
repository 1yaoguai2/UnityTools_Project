using UnityEngine;
using UnityEngine.AddressableAssets;
using xTools;

[CreateAssetMenu(menuName = "Game Assets/GamePrefabSO")]
public class GamePrefabSO : ScriptableObject
{
    public PrefabType prefabType;
    public AssetReference prefabReference;
    
}
