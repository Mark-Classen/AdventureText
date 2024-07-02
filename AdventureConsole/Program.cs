using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolveTextAdventure
{
    public interface IReader
    {
        string ReadInput();
    }

    public interface IWriter
    {
        void WriteLine(string message);
        void WriteLine();
    }

    public class TextAdventureGame
    {
        private readonly IReader reader;
        private readonly IWriter writer;
        private Player player;
        private List<Material> materials;
        private bool hasSoup = false;
        private bool hasSteak = false;
        private bool hasMagicRock = false;
        private Random random = new Random();

        public TextAdventureGame(IReader reader, IWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
            player = new Player();
            materials = new List<Material>();
            InitializeMaterials();
        }

        private void InitializeMaterials()
        {
            materials.Add(new Material { Name = "worm", Consumable = true });
            materials.Add(new Material { Name = "seed", Consumable = true });
            materials.Add(new Material { Name = "fish", Consumable = true });
            materials.Add(new Material { Name = "soup", Consumable = true });
            materials.Add(new Material { Name = "steak", Consumable = true });
            materials.Add(new Material { Name = "magic rock", Consumable = true });
        }

        public void Run()
        {
            writer.WriteLine("Welcome to the Evolve Text Adventure Game!");

            while (true)
            {
                DisplayStatus();

                writer.WriteLine("What would you like to do?");
                string input = reader.ReadInput();

                ProcessPlayerInput(input);

                writer.WriteLine(); // Empty line for readability
            }
        }

        private void DisplayStatus()
        {
            writer.WriteLine($"Current Maturity Level: {player.MaturityLevel}");
            writer.WriteLine($"Current Location: {player.Location}");
            writer.WriteLine($"Inventory: {string.Join(", ", player.Inventory)}");

            DisplayLocationDescription(player.Location);
        }

        private void DisplayLocationDescription(string location)
        {
            switch (location)
            {
                case "riverside":
                    writer.WriteLine("You are standing by the tranquil riverside, listening to the gentle flow of the water.");
                    writer.WriteLine("You can go to: forest, cave");
                    writer.WriteLine("You can hunt here.");
                    writer.WriteLine("You can consume: worm, seed, fish");
                    break;
                case "forest":
                    writer.WriteLine("The forest is dense and alive with chirping birds and rustling leaves.");
                    writer.WriteLine("You can go to: riverside, cabin, treehouse");
                    writer.WriteLine("You can hunt here.");
                    writer.WriteLine("You can consume: worm, seed");
                    break;
                case "cabin":
                    writer.WriteLine("Inside the cozy cabin, you see a warm fireplace crackling softly.");
                    writer.WriteLine("You can go to: forest");
                    if (!hasSoup) writer.WriteLine("There is soup here. You can 'get soup' to add it to your inventory.");
                    break;
                case "treehouse":
                    writer.WriteLine("From the treehouse, you have a commanding view of the surrounding forest.");
                    writer.WriteLine("You can go to: forest");
                    if (!hasSteak) writer.WriteLine("There is a steak here. You can 'get steak' to add it to your inventory.");
                    break;
                case "cave":
                    writer.WriteLine("The cave is damp and echoes with mysterious sounds.");
                    writer.WriteLine("You can go to: riverside");
                    writer.WriteLine("There is a magic rock here.");
                    writer.WriteLine("As you go closer you notice streaks of colours and light on the surface\n of the rock. A black glow emanates from it.\n It is soft and fleshy to the touch, is it edible?");
                    writer.WriteLine("");
                    break;
                default:
                    writer.WriteLine("You find yourself in an unknown location.");
                    break;
            }
        }

        private void ProcessPlayerInput(string input)
        {
            if (input.StartsWith("go "))
            {
                string destination = input.Substring(3).Trim();
                GoToLocation(destination);
            }
            else if (input == "hunt")
            {
                Hunt();
            }
            else if (input.StartsWith("consume "))
            {
                string itemName = input.Substring(8).Trim();
                ConsumeMaterial(itemName);
            }
            else if (input == "get soup")
            {
                GetSoup();
            }
            else if (input == "get steak")
            {
                GetSteak();
            }
            else if (input == "get magic rock")
            {
                GetMagicRock();
            }
            else if (input == "quit")
            {
                writer.WriteLine("Goodbye!");
                Environment.Exit(0);
            }
            else
            {
                writer.WriteLine("I'm not sure what you mean. Please try something else.");
            }
        }

        private void GoToLocation(string destination)
        {
            switch (player.Location)
            {
                case "riverside":
                    if (destination == "forest")
                    {
                        GoToForest();
                    }
                    else if (destination == "cave")
                    {
                        if (player.MaturityLevel >= 50)
                        {
                            EnterCave();
                        }
                        else
                        {
                            writer.WriteLine("You must reach maturity level 50 to enter the cave.");
                        }
                    }
                    else
                    {
                        writer.WriteLine("You can't go there from Riverside.");
                    }
                    break;
                case "forest":
                    if (destination == "riverside")
                    {
                        ReturnToRiverside();
                    }
                    else if (destination == "cabin")
                    {
                        EnterCabin();
                    }
                    else if (destination == "treehouse")
                    {
                        EnterTreehouse();
                    }
                    else
                    {
                        writer.WriteLine("You can't go there from Forest.");
                    }
                    break;
                case "cabin":
                    if (destination == "forest")
                    {
                        GoToForest();
                    }
                    else
                    {
                        writer.WriteLine("You can't go there from Cabin.");
                    }
                    break;
                case "treehouse":
                    if (destination == "forest")
                    {
                        GoToForest();
                    }
                    else
                    {
                        writer.WriteLine("You can't go there from Treehouse.");
                    }
                    break;
                case "cave":
                    if (destination == "riverside")
                    {
                        ReturnToRiverside();
                    }
                    else
                    {
                        writer.WriteLine("You can't go there from Cave.");
                    }
                    break;
                default:
                    writer.WriteLine("Invalid location.");
                    break;
            }
        }

        private void Hunt()
        {
            switch (player.Location)
            {
                case "riverside":
                    HuntInRiverside();
                    break;
                case "forest":
                    HuntInForest();
                    break;
                default:
                    writer.WriteLine("There's nothing to hunt here.");
                    break;
            }
        }

        private void HuntInRiverside()
        {
            int randomNumber = random.Next(0, 101);
            if (randomNumber < 30)
            {
                writer.WriteLine("You were not successful in the hunt. Try again.");
            }
            else
            {
                Material foundMaterial = HuntForMaterial("riverside");
                if (foundMaterial != null)
                {
                    writer.WriteLine($"You found {foundMaterial.Name}.");
                    player.Inventory.Add(foundMaterial.Name!);
                }
            }
        }

        private void HuntInForest()
        {
            int randomNumber = random.Next(0, 101);
            if (randomNumber < 20)
            {
                writer.WriteLine("You found nothing.");
            }
            else
            {
                Material foundMaterial = HuntForMaterial("forest");
                if (foundMaterial != null)
                {
                    writer.WriteLine($"You found {foundMaterial.Name}.");
                    player.Inventory.Add(foundMaterial.Name!);
                }
            }
        }

        private Material HuntForMaterial(string location)
        {
            Material? foundMaterial = null;
            do
            {
                int index = random.Next(materials.Count);
                foundMaterial = materials[index];

                if (location == "forest" && foundMaterial.Name == "fish")
                {
                    foundMaterial = null;
                }

            } while (foundMaterial == null || foundMaterial.Name == "soup" || foundMaterial.Name == "steak" || foundMaterial.Name == "magic rock");
            return foundMaterial;
        }

        private void GetSoup()
        {
            if (player.Location == "cabin")
            {
                if (!hasSoup)
                {
                    player.Inventory.Add("soup");
                    writer.WriteLine("You have obtained soup.");
                    hasSoup = true;
                }
            }
            else
            {
                writer.WriteLine("There is no soup here.");
            }
        }

        private void GetSteak()
        {
            if (player.Location == "treehouse")
            {
                if (!hasSteak)
                {
                    player.Inventory.Add("steak");
                    writer.WriteLine("You have obtained steak.");
                    hasSteak = true;
                }
            }
            else
            {
                writer.WriteLine("There is no steak here.");
            }
        }

    private void GetMagicRock()
    {
        if(player.Location == "cave")
        {
            if(!hasMagicRock)
            {
                       player.Inventory.Add("magic rock");
                    writer.WriteLine("You have obtained the magic rock.");
                    hasMagicRock = true;
                }
            }
            else
            {
                writer.WriteLine("There is no magic rock here.");
            }
        }
         
    


        private void EnterCave()
        {
            player.Location = "cave";
            writer.WriteLine("You have entered the cave.");
        } 

        private void EnterCabin()
        {
            player.Location = "cabin";
            writer.WriteLine("You have entered the cabin.");
        }

        private void EnterTreehouse()
        {
            player.Location = "treehouse";
            writer.WriteLine("You have You have entered the treehouse.");
        }

        private void GoToForest()
        {
            player.Location = "forest";
            writer.WriteLine("You have entered the forest.");
        }

        private void ReturnToRiverside()
        {
            player.Location = "riverside";
            writer.WriteLine("You have returned to the riverside.");
        }

        private void ConsumeMaterial(string materialName)
        {
            Material? materialToConsume = materials.FirstOrDefault(m => m.Name == materialName);

            if (materialToConsume != null && materialToConsume.Consumable)
            {
                if (player.Inventory.Contains(materialToConsume.Name!))
                {
                    player.Inventory.Remove(materialToConsume.Name!);
                    writer.WriteLine($"You consumed the {materialName}.");
                    IncreaseMaturity(materialName);
                }
                else
                {
                    writer.WriteLine($"You don't have any {materialName} to consume.");
                }
            }
            else
            {
                writer.WriteLine($"{materialName} is not a consumable item.");
            }
              if (materialName == "magic rock")
                    {
                        writer.WriteLine("You have consumed the magic rock...");
                        writer.WriteLine("Suddenly, you feel an overwhelming surge of energy and you are pulled into a portal.");
                        writer.WriteLine("Thank you for playing. Fare thee well!");
                        Environment.Exit(0);
        }}

        private void IncreaseMaturity(string materialName)
        {
            switch (materialName)
            {
                case "worm":
                    player.MaturityLevel += 1;
                    break;
                case "seed":
                    player.MaturityLevel += 2;
                    break;
                case "fish":
                    player.MaturityLevel += 3;
                    break;
                case "soup":
                    player.MaturityLevel += 4;
                    break;
                case "steak":
                    player.MaturityLevel += 5;
                    break;
                    case "magic rock":
                    player.MaturityLevel +=8;
                    break;
                default:
                    break;
            }
            writer.WriteLine($"Your maturity level has increased. Current level: {player.MaturityLevel}");
        }
    

    public class Player
    {
        public int MaturityLevel { get; set; }
        public string Location { get; set; }
        public List<string> Inventory { get; set; }

        public Player()
        {
            MaturityLevel = 0;
            Location = "riverside";
            Inventory = new List<string>();
        }
    }

    public class Material
    {
        public string? Name { get; set; }
        public bool Consumable { get; set; }
    }

    public class ConsoleReader : IReader
    {
        public string ReadInput()
        {
            return Console.ReadLine()!;
        }
    }

    public class ConsoleWriter : IWriter
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IReader reader = new ConsoleReader();
            IWriter writer = new ConsoleWriter();
            TextAdventureGame game = new TextAdventureGame(reader, writer);
            game.Run();
        }
    }
    }}
