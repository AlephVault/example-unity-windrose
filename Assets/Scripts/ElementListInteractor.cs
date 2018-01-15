using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GabTab.Behaviours;

class ElementListInteractor : TextOptionListInteractor
{
    protected void Start()
    {
        base.Start();
        Items = new List<TextOption>(new[] {
            new TextOption("agua", "Agua"), new TextOption("fuego", "Fuego"), new TextOption("tierra", "Tierra"),
            new TextOption("viento", "Viento"), new TextOption("madera", "Madera"), new TextOption("metal", "Metal"),
            new TextOption("espiritu", "Espiritu"), new TextOption("vacio", "Vacio"), new TextOption("mente", "Mente"),
            new TextOption("pleroma", "Pleroma", false)
        });
    }

    protected override void ValidateSelection(TextOption[] selectedItems, Action<InteractiveMessage.Prompt[]> reportInvalidMessage)
    {
        bool fuego = selectedItems.Where((TextOption item) => item.Key == "fuego").Any();
        bool agua = selectedItems.Where((TextOption item) => item.Key == "agua").Any();
        bool tierra = selectedItems.Where((TextOption item) => item.Key == "tierra").Any();
        bool viento = selectedItems.Where((TextOption item) => item.Key == "viento").Any();
        bool madera = selectedItems.Where((TextOption item) => item.Key == "madera").Any();
        bool metal = selectedItems.Where((TextOption item) => item.Key == "metal").Any();
        if (selectedItems.Length != 3)
        {
            reportInvalidMessage(new[] { new InteractiveMessage.Prompt("Debes elegir exactamente 3 elementos", false, true) });
        }
        if (fuego && agua)
        {
            reportInvalidMessage(new[] { new InteractiveMessage.Prompt("No se puede seleccionar fuego y agua", false, true) });
        }
        if (tierra && viento)
        {
            reportInvalidMessage(new[] { new InteractiveMessage.Prompt("No se puede seleccionar tierra y viento", false, true) });
        }
        if (madera && metal)
        {
            reportInvalidMessage(new[] { new InteractiveMessage.Prompt("No se puede seleccionar madera y metal", false, true) });
        }
        base.ValidateSelection(selectedItems, reportInvalidMessage);
    }
}
