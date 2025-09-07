using System.Collections.Generic;
using UnityEngine;

public static class CardDatabase
{
    public static List<CardData> AllCards = new List<CardData>()
    {
        new CardData("S01-001", "Sürpriz Kutusu", "Þansa baðlý düþmaný öldürür, olmazsa HP %50 kaybeder") {
            type = CardData.CardType.Special, duration = 1
        },
        new CardData("S02-001", "Tuzak", "Hedefi 3 tur zindana atar") {
            type = CardData.CardType.Debuff, duration = 3, banish = true
        },
        new CardData("S03-001", "Klonlama", "2–6 klon oluþturur (3 tur)") {
            type = CardData.CardType.Special, duration = 3, summonCount = 3
        },
        new CardData("S04-001", "Güçlendirme", "Hasarý 2 tur %50 artýrýr") {
            type = CardData.CardType.Buff, duration = 2, damage = 50
        },
        new CardData("S05-001", "Silah", "Ek silah gücü kazandýrýr") {
            type = CardData.CardType.Buff, damage = 30
        },
        new CardData("S06-001", "Sihir", "Doðrudan büyü hasarý verir") {
            type = CardData.CardType.Attack, damage = 40
        },
        new CardData("S07-001", "Yansýtma", "Gelen hasarý yansýtýr") {
            type = CardData.CardType.Defense, reflect = 1.0f, duration = 1
        },
        new CardData("S08-001", "Kritik Artýrýcý", "Kritik hasarý %100 artýrýr (1 tur)") {
            type = CardData.CardType.Buff, critDamage = 2.0f, duration = 1
        },
        new CardData("S09-001", "Kritik Vuruþ", "Kritik þansý %30 artýrýr (2 tur)") {
            type = CardData.CardType.Buff, critChance = 0.3f, duration = 2
        },
        new CardData("S10-001", "Kombine", "2 karakterin güçlerini birleþtirir") {
            type = CardData.CardType.Special, combine = true
        },
        new CardData("S11-001", "Engelleme", "Rakibin sýradaki kartýný iptal eder") {
            type = CardData.CardType.Utility, duration = 1
        },
        new CardData("S12-001", "Susturma", "Pasif yetenekleri 2 tur kapatýr") {
            type = CardData.CardType.Debuff, duration = 2
        },
        new CardData("S13-001", "Savunma Kalkaný", "2 tur %50 hasar azaltýr") {
            type = CardData.CardType.Defense, duration = 2
        },
        new CardData("S14-001", "Hýz", "Sýrasýný öne çeker") {
            type = CardData.CardType.Buff, duration = 1
        },
        new CardData("S15-001", "Þifa", "HP %40 yeniler") {
            type = CardData.CardType.Buff, heal = 40
        },
        new CardData("S16-001", "Takým Ruhu", "Tüm karakterlere %20 buff (2 tur)") {
            type = CardData.CardType.Buff, duration = 2
        },
        new CardData("S17-001", "Paylaþýlan Acý", "Hasar tüm düþmanlara bölünür") {
            type = CardData.CardType.Special
        },
        new CardData("S18-001", "Kahramanýn Çaðrýsý", "Desteye rastgele güçlü kart ekler") {
            type = CardData.CardType.Special
        },
        new CardData("S19-001", "Manevi Güç", "Düþen karakter ruh olarak döner") {
            type = CardData.CardType.Special, duration = 1
        },
        new CardData("S20-001", "Zehir", "Her tur %10 HP kaybettirir (3 tur)") {
            type = CardData.CardType.Debuff, duration = 3
        },
        new CardData("S21-001", "Yanma", "3 tur boyunca ateþ hasarý verir") {
            type = CardData.CardType.Debuff, duration = 3, damage = 10
        },
        new CardData("S22-001", "Donma", "1 tur boyunca dondurur") {
            type = CardData.CardType.Debuff, duration = 1, stun = true
        },
        new CardData("S23-001", "Deprem", "Tüm düþmanlara alan hasarý verir") {
            type = CardData.CardType.Attack, damage = 20
        },
        new CardData("S24-001", "Rüzgar Darbesi", "Hedefin sýrasýný bozar") {
            type = CardData.CardType.Utility, duration = 1
        },
        new CardData("S25-001", "Karanlýk Sözleþme", "HP %30 feda, %100 ek hasar") {
            type = CardData.CardType.Special, damage = 100
        },
    };
}
