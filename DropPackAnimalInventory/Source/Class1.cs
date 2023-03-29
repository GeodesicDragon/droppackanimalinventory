using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

public class CompPackAnimalDropInventory : ThingComp
{
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (this.parent.Faction == Faction.OfPlayer)
        {
            yield return new Command_Action
            {
                action = () =>
                {
                    DropInventory();
                },
                defaultLabel = "DAI_GizmoLabel".Translate(),
                defaultDesc = "DAI_GizmoDescription".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/DropInventory"),
                hotKey = KeyBindingDefOf.Misc9,
            };
        }
    }

    private void DropInventory()
    {
        if (this.parent.Faction != Faction.OfPlayer)
        {
            return;
        }

        List<Pawn> selectedPackAnimals = new List<Pawn>();

        foreach (Thing thing in Find.Selector.SelectedObjects)
        {
            Pawn animal = thing as Pawn;

            if (animal != null && animal.def.race.Animal)
            {
                selectedPackAnimals.Add(animal);
            }
        }

        if (selectedPackAnimals.Count > 0)
        {
            foreach (Pawn animal in selectedPackAnimals)
            {
                ThingOwner<Thing> inventory = animal.inventory.innerContainer;


                List<Thing> thingsToDrop = new List<Thing>();

                foreach (Thing thing in inventory)
                {
                    thingsToDrop.Add(thing);
                }

                foreach (Thing thing in thingsToDrop)
                {
                    inventory.Remove(thing);
                    GenPlace.TryPlaceThing(thing, animal.Position, animal.Map, ThingPlaceMode.Near);
                }
            }
        }
    }
}

[StaticConstructorOnStartup]
public static class Startup
{
    static Startup()
    {
        foreach (ThingDef animalDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (animalDef.category == ThingCategory.Pawn && animalDef.race.packAnimal && animalDef.race.Animal && animalDef.race.IsFlesh)
            {
                animalDef.comps.Add(new CompProperties
                {
                    compClass = typeof(CompPackAnimalDropInventory)
                });
            }
        }
    }
}