using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CharacterClassListInteractor : TextOptionListInteractor
{
    protected override void Start()
    {
        base.Start();
        Items = new List<TextOption>(new[] {
            new TextOption("guerrero", "Warrior"), new TextOption("ladron", "Thief"), new TextOption("mago", "Mage"),
            new TextOption("monje", "Monk"), new TextOption("artista", "Artist"), new TextOption("vagabundo", "Wanderer"),
            new TextOption("explorador", "Ranger")
        });
    }
}
