using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Var olan StartBattleManager (veya benzeri) üzerindeki
/// gerçek baþlatma metodunu çaðýrmak için küçük bir köprü.
/// Inspector’dan hedef script’i ve metod adýný giriyorsun.
/// </summary>
public class BattleStartAdapter : MonoBehaviour
{
    [Header("Hedef")]
    [Tooltip("StartBattleManager script’inin olduðu Component’i sürükle.")]
    public MonoBehaviour target;     // örn: StartBattleManager

    [Tooltip("Çaðrýlacak metod adý (örn: StartBattle, BeginMatch, GoBattleScene)")]
    public string methodName = "StartBattle";

    public enum CallMode { NoArg, IntTeamSize }
    [Tooltip("Metod int (teamSize) alýyor mu? Yoksa argsýz mý?")]
    public CallMode callMode = CallMode.NoArg;

    /// <summary> Dýþarýdan çaðýr: takým boyutuyla ya da argsýz. </summary>
    public void StartMatch(int teamSize)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
        {
            Debug.LogError("[BattleStartAdapter] target/metod atanmadý.");
            return;
        }

        var t = target.GetType();
        try
        {
            if (callMode == CallMode.IntTeamSize)
            {
                var m = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
                if (m != null) { m.Invoke(target, new object[] { teamSize }); return; }
            }

            // argsýz dene
            {
                var m = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (m != null) { m.Invoke(target, null); return; }
            }

            Debug.LogError($"[BattleStartAdapter] {t.Name} içinde '{methodName}' bulunamadý. (int parametreli ya da argsýz)");
        }
        catch (Exception e)
        {
            Debug.LogError($"[BattleStartAdapter] çaðrý hatasý: {e.Message}");
        }
    }
}
