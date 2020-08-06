using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ZodiacListInteractor : TextOptionListInteractor
{
    protected override void Start()
    {
        base.Start();
        Items = new List<TextOption>(new[] {
            new TextOption("aries", "Aries"), new TextOption("tauro", "Tauro"), new TextOption("geminis", "Geminis"),
            new TextOption("cancer", "Cancer"), new TextOption("leo", "Leo"), new TextOption("virgo", "Virgo"),
            new TextOption("libra", "Libra"), new TextOption("escorpio", "Escorpio"), new TextOption("ofiuco", "Ofiuco"),
            new TextOption("sagitario", "Sagitario"), new TextOption("capricornio", "Capricornio"), new TextOption("acuario", "Acuario"),
            new TextOption("piscis", "Piscis")
        });
    }
}
