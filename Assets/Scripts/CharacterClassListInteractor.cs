using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CharacterClassListInteractor : TextOptionListInteractor
{
    protected void Start()
    {
        base.Start();
        Items = new List<TextOption>(new[] {
            new TextOption("guerrero", "Guerrero"), new TextOption("ladron", "Ladron"), new TextOption("mago", "Mago"),
            new TextOption("monje", "Monje"), new TextOption("artista", "Artista"), new TextOption("navegante", "Navegante"),
            new TextOption("explorador", "Explorador")
        });
    }
}
