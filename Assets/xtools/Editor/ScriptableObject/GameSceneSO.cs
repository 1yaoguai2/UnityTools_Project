using UnityEngine;
using UnityEngine.AddressableAssets;
using xTools;

[CreateAssetMenu(menuName = "Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public SceneType sceneType;
    public AssetReference sceneReference;
    
}
