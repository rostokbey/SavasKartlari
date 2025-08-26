using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Var olan StartBattleManager (veya benzeri) �zerindeki
/// ger�ek ba�latma metodunu �a��rmak i�in k���k bir k�pr�.
/// Inspector�dan hedef script�i ve metod ad�n� giriyorsun.
/// </summary>
public class BattleStartAdapter : MonoBehaviour
{
    [Header("Hedef")]
    [Tooltip("StartBattleManager script�inin oldu�u Component�i s�r�kle.")]
    public MonoBehaviour target;     // �rn: StartBattleManager

    [Tooltip("�a�r�lacak metod ad� (�rn: StartBattle, BeginMatch, GoBattleScene)")]
    public string methodName = "StartBattle";

    public enum CallMode { NoArg, IntTeamSize }
    [Tooltip("Metod int (teamSize) al�yor mu? Yoksa args�z m�?")]
    public CallMode callMode = CallMode.NoArg;

    /// <summary> D��ar�dan �a��r: tak�m boyutuyla ya da args�z. </summary>
    public void StartMatch(int teamSize)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
        {
            Debug.LogError("[BattleStartAdapter] target/metod atanmad�.");
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

            // args�z dene
            {
                var m = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (m != null) { m.Invoke(target, null); return; }
            }

            Debug.LogError($"[BattleStartAdapter] {t.Name} i�inde '{methodName}' bulunamad�. (int parametreli ya da args�z)");
        }
        catch (Exception e)
        {
            Debug.LogError($"[BattleStartAdapter] �a�r� hatas�: {e.Message}");
        }
    }
}
