using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RaceInfo
{
    public string raceName;
    public GameObject racePrefab;
}

[System.Serializable]
public class MaleHolderInfo
{
    public GameObject className;
    public List<RaceInfo> availableRaces = new List<RaceInfo>();
}

public class MaleHoldersManager : MonoBehaviour
{
    public List<MaleHolderInfo> Holder = new List<MaleHolderInfo>();
}

[System.Serializable]
public class FemaleHolderInfo
{
    public GameObject className;
    public List<RaceInfo> availableRaces = new List<RaceInfo>();
}

public class FemaleHoldersManager : MonoBehaviour
{
    public List<FemaleHolderInfo> Holder = new List<FemaleHolderInfo>();
}

public class ClassListController : MonoBehaviour
{
    public string _class;
    public string _gender;
    public string _race;
    public TextMeshProUGUI _introClass;
    public TextMeshProUGUI _strengthsClass;
    public TextMeshProUGUI _weaksidesClass;
    public TextMeshProUGUI _introRace;
    public TextMeshProUGUI _titleRace;

    public Toggle[] tClass;
    public Toggle[] tGender;
    public Toggle[] tRace;
    public List<MaleHolderInfo> maleHolders = new List<MaleHolderInfo>();
    public List<FemaleHolderInfo> femaleHolders = new List<FemaleHolderInfo>();


    void Update()
    {
        UpdateClass();
        UpdateGender();
        UpdateRace();
        UpdateTextFields();
        UpdateActivePrefabs();
    }

    void UpdateClass()
    {
        foreach (Toggle toggle in tClass)
        {
            if (toggle.isOn)
            {
                _class = toggle.gameObject.name;
                return;
            }
        }
        _class = "";
    }

    void UpdateGender()
    {
        foreach (Toggle toggle in tGender)
        {
            if (toggle.isOn)
            {
                _gender = toggle.gameObject.name;
                return;
            }
        }
        _gender = "";
    }

    void UpdateRace()
    {
        foreach (Toggle toggle in tRace)
        {
            if (toggle.isOn)
            {
                _race = toggle.gameObject.name;
                return;
            }
        }
        _race = "";
    }


    void UpdateTextFields()
    {
        string introText;
        string strengthsText;
        string weaksidesText;
        string introRaceText;

        switch (_class)
        {            
            case "warrior":
                 introText = introClasses._introClasses["intro_war"];
                 strengthsText = introClasses._strengthsClasses["strengths_war"];
                 weaksidesText = introClasses._weaksidesClasses["weaksides_war"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "clirick":
                 introText = introClasses._introClasses["intro_priest"];
                 strengthsText = introClasses._strengthsClasses["strengths_priest"];
                 weaksidesText = introClasses._weaksidesClasses["weaksides_priest"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "assassin":
                introText = introClasses._introClasses["intro_assassin"];
                strengthsText = introClasses._strengthsClasses["strengths_assassin"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_assassin"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "jagernaut":
                introText = introClasses._introClasses["intro_jagernaut"];
                strengthsText = introClasses._strengthsClasses["strengths_jagernaut"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_jagernaut"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "vampire":
                introText = introClasses._introClasses["intro_vampire"];
                strengthsText = introClasses._strengthsClasses["strengths_vampire"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_vampire"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "witch":
                introText = introClasses._introClasses["intro_witch"];
                strengthsText = introClasses._strengthsClasses["strengths_witch"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_witch"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "ripper":
                introText = introClasses._introClasses["intro_ripper"];
                strengthsText = introClasses._strengthsClasses["strengths_ripper"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_ripper"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "caster":
                introText = introClasses._introClasses["intro_caster"];
                strengthsText = introClasses._strengthsClasses["strengths_caster"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_caster"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "defender":
                introText = introClasses._introClasses["intro_defender"];
                strengthsText = introClasses._strengthsClasses["strengths_defender"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_defender"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "torturer":
                introText = introClasses._introClasses["intro_torturer"];
                strengthsText = introClasses._strengthsClasses["strengths_torturer"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_torturer"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "archer":
                introText = introClasses._introClasses["intro_archer"];
                strengthsText = introClasses._strengthsClasses["strengths_archer"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_archer"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "mage":
                introText = introClasses._introClasses["intro_mage"];
                strengthsText = introClasses._strengthsClasses["strengths_mage"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_mage"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "paladin":
                introText = introClasses._introClasses["intro_paladin"];
                strengthsText = introClasses._strengthsClasses["strengths_paladin"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_paladin"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "crosshear":
                introText = introClasses._introClasses["intro_crosshear"];
                strengthsText = introClasses._strengthsClasses["strengths_crosshear"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_crosshear"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "bard":
                introText = introClasses._introClasses["intro_bard"];
                strengthsText = introClasses._strengthsClasses["strengths_bard"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_bard"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "dark_knight":
                introText = introClasses._introClasses["intro_dark_knight"];
                strengthsText = introClasses._strengthsClasses["strengths_dark_knight"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_dark_knight"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;
            case "necromancer":
                introText = introClasses._introClasses["intro_necromancer"];
                strengthsText = introClasses._strengthsClasses["strengths_necromancer"];
                weaksidesText = introClasses._weaksidesClasses["weaksides_necromancer"];
                _introClass.text = ParseAndFormatText(introText);
                _strengthsClass.text = ParseAndFormatText(strengthsText);
                _weaksidesClass.text = ParseAndFormatText(weaksidesText);
                break;

            default:
                _introClass.text = "";
                _strengthsClass.text = "";
                _weaksidesClass.text = "";
                break;
        }
        switch (_race)
        {
            case "Human":
                introRaceText = introClasses._introRaces["introRaces_human"];
                _introRace.text = ParseAndFormatText(introRaceText);
                _titleRace.text = "Человек";
                break;
            case "Elf":
                introRaceText = introClasses._introRaces["introRaces_Elf"];
                _introRace.text = ParseAndFormatText(introRaceText);
                _titleRace.text = "Эльф";
                break;
            case "Dworf":
                introRaceText = introClasses._introRaces["introRaces_Dworf"];
                _introRace.text = ParseAndFormatText(introRaceText);
                _titleRace.text = "Гном";
                break;
            case "Vesperian":
                introRaceText = introClasses._introRaces["introRaces_Vesperian"];
                _introRace.text = ParseAndFormatText(introRaceText);
                _titleRace.text = "Веспериан";
                break;
            case "Frangor":
                introRaceText = introClasses._introRaces["introRaces_Frangor"];
                _introRace.text = ParseAndFormatText(introRaceText);
                _titleRace.text = "Франгор";
                break;
            case "Likan":
                introRaceText = introClasses._introRaces["introRaces_Likan"];
                _introRace.text = ParseAndFormatText(introRaceText);
                _titleRace.text = "Ликан";
                break;
            case "Demon":
                introRaceText = introClasses._introRaces["introRaces_Demon"];
                _introRace.text = ParseAndFormatText(introRaceText);
                _titleRace.text = "Демон";
                break;
            default:
                _introRace.text = "";
                _titleRace.text = "";
                break;
        }
    }
    void UpdateActivePrefabs()
    {
        for (int i = 0; i < maleHolders.Count; i++)
        {
            bool classActive = (_class == maleHolders[i].className.name) && (_gender == "Male");

            maleHolders[i].className.SetActive(classActive);

            if (classActive)
            {
                for (int j = 0; j < maleHolders[i].availableRaces.Count; j++)
                {
                    bool raceActive = (_race == maleHolders[i].availableRaces[j].raceName);
                    maleHolders[i].availableRaces[j].racePrefab.SetActive(raceActive);
                }
            }
            else
            {
                foreach (RaceInfo raceInfo in maleHolders[i].availableRaces)
                {
                    raceInfo.racePrefab.SetActive(false);
                }
            }
        }

        for (int i = 0; i < femaleHolders.Count; i++)
        {
            bool classActive = (_class == femaleHolders[i].className.name) && (_gender == "Female");

            femaleHolders[i].className.SetActive(classActive);

            if (classActive)
            {
                for (int j = 0; j < femaleHolders[i].availableRaces.Count; j++)
                {
                    bool raceActive = (_race == femaleHolders[i].availableRaces[j].raceName);
                    femaleHolders[i].availableRaces[j].racePrefab.SetActive(raceActive);
                }
            }
            else
            {
                foreach (RaceInfo raceInfo in femaleHolders[i].availableRaces)
                {
                    raceInfo.racePrefab.SetActive(false);
                }
            }
        }
    }

    string ParseAndFormatText(string inputText)
    {
        string[] lines = inputText.Split(';');
        return string.Join("\n", lines);
    }
}