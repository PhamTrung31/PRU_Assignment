using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDb;
    public TMP_Text nameTxt;
    public SpriteRenderer artworkSprite;
    private int selectionOption = 0;
    void Start()
    {
        UpdateCharacter(selectionOption);
    }
    public void NextOption()
    {
        selectionOption++;
        if (selectionOption >= characterDb.CharacterCount)
        {
            selectionOption = 0;
        }
        UpdateCharacter(selectionOption);
    }

    public void BackOption()
    {
        selectionOption--;
        if (selectionOption < 0)
        {
            selectionOption = characterDb.CharacterCount - 1;
        }
        UpdateCharacter(selectionOption);
    }
    public void UpdateCharacter(int selectionOption)
    {
         Character character = characterDb.GetCharacter(selectionOption);
        artworkSprite.sprite = character.characterSprite;
        nameTxt.text = character.characterName;
    }
}
