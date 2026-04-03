using UnityEngine;

[CreateAssetMenu(fileName = "New Minigame", menuName = "WeGoCrazy/MinigameData")]
public class MinigameData : ScriptableObject
{
    public string gameName;    // Nombre exacto de la escena en Build Settings
    public string sceneName;    // Nombre exacto de la escena en Build Settings
    public string instruction; // Ej: "¡SALTA!", "¡ESQUIVA!"
}