using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GabTab.Behaviours;

class ElementListInteractor : TextOptionListInteractor
{
    protected override void Start()
    {
        base.Start();
        Items = new List<TextOption>(new[] {
            new TextOption("agua", "Water"), new TextOption("fuego", "Fire"), new TextOption("tierra", "Earth"),
            new TextOption("viento", "Wind"), new TextOption("madera", "Wood"), new TextOption("metal", "Metal"),
            new TextOption("espiritu", "Spirit"), new TextOption("vacio", "Void"), new TextOption("mente", "Mind"),
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
            reportInvalidMessage(new InteractiveMessage.PromptBuilder().Clear().Write("You must pick exactly 3 elements.").Wait().End());
        }
        if (fuego && agua)
        {
            reportInvalidMessage(new InteractiveMessage.PromptBuilder().Clear().Write("You cannot pick both fire and water.").Wait().End());
        }
        if (tierra && viento)
        {
            reportInvalidMessage(new InteractiveMessage.PromptBuilder().Clear().Write("You cannot pick both earth and wind.").Wait().End());
        }
        if (madera && metal)
        {
            reportInvalidMessage(new InteractiveMessage.PromptBuilder().Clear().Write("You cannot pick both wood and metal. ").Wait().End());
        }
        base.ValidateSelection(selectedItems, reportInvalidMessage);
    }
}
