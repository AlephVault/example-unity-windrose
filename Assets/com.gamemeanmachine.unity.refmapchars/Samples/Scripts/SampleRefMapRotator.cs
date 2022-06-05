using System;
using GameMeanMachine.Unity.RefMapChars.Authoring.Behaviours;
using GameMeanMachine.Unity.RefMapChars.Authoring.ScriptableObjects.Standard;
using GameMeanMachine.Unity.RefMapChars.Types;
using GameMeanMachine.Unity.RefMapChars.Types.Traits;
using GameMeanMachine.Unity.RefMapChars.Types.Traits.Standard;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;
using Exception = GameMeanMachine.Unity.WindRose.Types.Exception;


[RequireComponent(typeof(MapObject))]
public class SampleRefMapRotator : MonoBehaviour
{
    [SerializeField]
    private RefMapBundle bundle;

    [SerializeField]
    private bool moves;

    private MapObject mapObject;
    private Visual mainVisual;
    private RefMapStandardApplier applier;

    private int sex = 0; // 0-1
    private int bodyColor = 0; // 0-7

    private int boots = -1;
    private int bootsColor = 0;
    private int pants = -1;
    private int pantsColor = 0;
    private int waist = -1;
    private int waistColor = 0;
    private int shirt = -1;
    private int shirtColor = 0;
    private int hat = -1;
    private int hatColor = 0;
    private int hair = -1;
    private int hairColor = 0;
    private int longShirt = -1;
    private int longShirtColor = 0;
    private int arms = -1;
    private int armsColor = 0;
    private int chest = -1;
    private int chestColor = 0;
    private int shoulder = -1;
    private int shoulderColor = 0;

    private bool bootsOverPants = false;

    private const int Sexes = 2;
    private const int BodyColors = 8;
    private const int ItemColors = 10;

    private const int MaleBoots = 3;
    private const int FemaleBoots = 1;
    private const int MalePants = 5;
    private const int FemalePants = 10;
    private const int MaleWaists = 4;
    private const int FemaleWaists = 2;
    private const int MaleShirts = 11;
    private const int FemaleShirts = 17;
    private const int MaleChests = 3;
    private const int FemaleChests = 3;
    private const int MaleShoulders = 13;
    private const int FemaleShoulders = 7;
    private const int MaleHats = 7;
    private const int FemaleHats = 8;
    private const int MaleHairs = 15;
    private const int FemaleHairs = 15;
    private const int MaleLongShirts = 9;
    private const int FemaleLongShirts = 9;
    private const int MaleArms = 8;
    private const int FemaleArms = 7;

    private void Awake()
    {
        mapObject = GetComponent<MapObject>();
        mainVisual = mapObject.MainVisual;
        applier = mainVisual.GetComponent<RefMapStandardApplier>();
    }

    private void Start()
    {
        FixIndicesAndUpdate();
    }

    private void FixIndicesAndUpdate()
    {
        // First, rotating the colors is easy.
        if (bodyColor == 8) bodyColor = 0;
        if (bootsColor == 10) bootsColor = 0;
        if (pantsColor == 10) pantsColor = 0;
        if (waistColor == 10) waistColor = 0;
        if (shirtColor == 10) shirtColor = 0;
        if (chestColor == 10) chestColor = 0;
        if (longShirtColor == 10) longShirtColor = 0;
        if (shoulderColor == 10) shoulderColor = 0;
        if (armsColor == 10) armsColor = 0;
        if (hairColor == 10) hairColor = 0;
        if (hatColor == 10) hatColor = 0;
        
        // Rotating the models is more difficult.
        // If the value is now >= the cap, it will
        // become (-1) and the character will not
        // have a cloth of that type dressed.

        int maxBoots = sex == 0 ? MaleBoots : FemaleBoots;
        if (boots >= maxBoots) boots = -1;
        int maxPants = sex == 0 ? MalePants : FemalePants;
        if (pants >= maxPants) pants = -1;
        int maxWaits = sex == 0 ? MaleWaists : FemaleWaists;
        if (waist >= maxWaits) waist = -1;
        int maxShirts = sex == 0 ? MaleShirts : FemaleShirts;
        if (shirt >= maxShirts) shirt = -1;
        int maxChest = sex == 0 ? MaleChests : FemaleChests;
        if (chest >= maxChest) chest = -1;
        int maxLongShirt = sex == 0 ? MaleLongShirts : FemaleLongShirts;
        if (longShirt >= maxLongShirt) longShirt = -1;
        int maxShoulder = sex == 0 ? MaleShoulders : FemaleShoulders;
        if (shoulder >= maxShoulder) shoulder = -1;
        int maxArms = sex == 0 ? MaleArms : FemaleArms;
        if (arms >= maxArms) arms = -1;
        int maxHat = sex == 0 ? MaleHats : FemaleHats;
        if (hat >= maxHat) hat = -1;
        int maxHair = sex == 0 ? MaleHairs : FemaleHairs;
        if (hair >= maxHair) hair = -1;
        
        // Now, applying everything will take place.
        
        // Body
        RefMapSource bodySource = bundle[
            sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
        ].Body[(RefMapBody.ColorCode) bodyColor];
        applier.Use((new BodyTrait($"{sex}{bodyColor}", bodySource)), false);
        
        // Boots
        if (boots == -1)
        {
            applier.Use((BootsTrait)null, false);
        }
        else
        {
            RefMapSource bootsSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Boots
            ][(ushort) boots][(RefMapItem.ColorCode) bootsColor];
            applier.Use(new BootsTrait($"{sex}{boots}{bootsColor}", bootsSource), false);
        }
        
        // Pants
        if (pants == -1)
        {
            applier.Use((PantsTrait)null, false);
        }
        else
        {
            RefMapSource pantsSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Pants
            ][(ushort) pants][(RefMapItem.ColorCode) pantsColor];
            applier.Use(new PantsTrait($"{sex}{pants}{pantsColor}", pantsSource), false);
        }
        
        // Waist
        if (waist == -1)
        {
            applier.Use((WaistTrait)null, false);
        }
        else
        {
            RefMapSource waistSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Waist
            ][(ushort) waist][(RefMapItem.ColorCode) waistColor];
            applier.Use(new WaistTrait($"{sex}{waist}{waistColor}", waistSource), false);
        }
        
        // Shirt
        if (shirt == -1)
        {
            applier.Use((ShirtTrait)null, false);
        }
        else
        {
            RefMapSource shirtSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Shirt
            ][(ushort) shirt][(RefMapItem.ColorCode) shirtColor];
            applier.Use(new ShirtTrait($"{sex}{shirt}{shirtColor}", shirtSource), false);
        }
        
        // Chest
        if (chest == -1)
        {
            applier.Use((ChestTrait)null, false);
        }
        else
        {
            RefMapSource chestSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Chest
            ][(ushort) chest][(RefMapItem.ColorCode) chestColor];
            applier.Use(new ChestTrait($"{sex}{chest}{chestColor}", chestSource), false);
        }
        
        // LongShirt
        if (longShirt == -1)
        {
            applier.Use((LongShirtTrait)null, false);
        }
        else
        {
            RefMapSource longShirtSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.LongShirt
            ][(ushort) longShirt][(RefMapItem.ColorCode) longShirtColor];
            applier.Use(new LongShirtTrait($"{sex}{longShirt}{longShirtColor}", longShirtSource), false);
        }
        
        // Shoulder
        if (shoulder == -1)
        {
            applier.Use((ShoulderTrait)null, false);
        }
        else
        {
            RefMapSource shoulderSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Shoulder
            ][(ushort) shoulder][(RefMapItem.ColorCode) shoulderColor];
            applier.Use(new ShoulderTrait($"{sex}{shoulder}{shoulderColor}", shoulderSource), false);
        }
        
        // Arms
        if (arms == -1)
        {
            applier.Use((ArmsTrait)null, false);
        }
        else
        {
            RefMapSource armsSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Arms
            ][(ushort) arms][(RefMapItem.ColorCode) armsColor];
            applier.Use(new ArmsTrait($"{sex}{arms}{armsColor}", armsSource), false);
        }
        
        // Hat
        if (hat == -1)
        {
            applier.Use((HatTrait)null, false);
        }
        else
        {
            RefMapSource hatSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Hat
            ][(ushort) hat][(RefMapItem.ColorCode) hatColor];
            applier.Use(new HatTrait($"{sex}{hat}{hatColor}", hatSource), false);
        }
        
        // Hair - This one is a bit different
        if (hair == -1)
        {
            applier.Use((HairTrait)null, false);
        }
        else
        {
            RefMapSource hairSource = bundle[
                sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
            ][
                RefMapSex.ItemTypeCode.Hair
            ][(ushort) hair][(RefMapItem.ColorCode) hairColor];
            RefMapSource hairTailSource = null;
            try
            {
                hairTailSource = bundle[
                    sex == 0 ? RefMapBundle.SexCode.Male : RefMapBundle.SexCode.Female
                ][
                    RefMapSex.ItemTypeCode.HairTail
                ][(ushort) hair][(RefMapItem.ColorCode) hairColor];
            }
            catch (System.Exception e)
            {
            }
            applier.Use(new HairTrait($"{sex}{hair}{hairColor}", hairSource, hairTailSource));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Rotate Boots
            boots++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Rotate Pants
            pants++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Rotate Waist
            waist++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Rotate Shirt
            shirt++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // Rotate Chest
            chest++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            // Rotate LongShirt
            longShirt++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            // Rotate Shoulder
            shoulder++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            // Rotate Arms
            arms++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            // Rotate Hair
            hair++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // Rotate Hat
            hat++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Rotate Boots Color
            bootsColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Rotate Pants Color
            pantsColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Rotate Waist Color
            waistColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Rotate Shirt Color
            shirtColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Rotate Chest Color
            chestColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // Rotate LongShirt Color
            longShirtColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            // Rotate Shoulder Color
            shoulderColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Rotate Arms Color
            armsColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Rotate Hair Color
            hairColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Rotate Hat Color
            hatColor++;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Toggle Boots over Pants
            bootsOverPants = !bootsOverPants;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Toggle Sex
            sex = 1 - sex;
            FixIndicesAndUpdate();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Rotate Body
            bodyColor++;
            FixIndicesAndUpdate();
        }

        // Movement starts here.
        
        if (Input.GetKey(KeyCode.DownArrow))
        {
            mapObject.Orientation = Direction.DOWN;
            if (moves) mapObject.StartMovement(Direction.DOWN);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            mapObject.Orientation = Direction.UP;
            if (moves) mapObject.StartMovement(Direction.UP);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            mapObject.Orientation = Direction.LEFT;
            if (moves) mapObject.StartMovement(Direction.LEFT);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            mapObject.Orientation = Direction.RIGHT;
            if (moves) mapObject.StartMovement(Direction.RIGHT);
        }
    }
}
