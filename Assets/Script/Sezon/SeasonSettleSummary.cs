// Assets/Script/Sezon/SeasonSettleSummary.cs
using System;

[Serializable]
public struct SeasonSettleSummary
{
    public bool hadChampion;
    public string championProfileId;
    public string championName;
    public int participants;

    public int medalsDelta;   // bu sezonda verilen madalya (genelde 1 veya 0)
    public int championMedalsAfter; // �ampiyonun yeni toplam madalyas�

    public int resetPoints;   // reset sonras� ba�lang�� puan� (SeasonManager.startPoints)
    public string note;          // debug/mesaj
}
