using Roommates.Repositories;
using Roommates.Models;
using Roommates.Repositories;

namespace Roommates
{
    class Program
    {
        //  This is the address of the database.
        //  We define it here as a constant since it will never change.
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true; TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);

            bool runProgram = true;
            while (runProgram)
            {
                string selection = GetMenuSelection();

                switch (selection)
                {
                    case ("Show all rooms"):
                        List<Room> rooms = roomRepo.GetAll();
                        foreach (Room r in rooms)
                        {
                            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Search for room"):
                        Console.Write("Room Id: ");
                        int id = int.Parse(Console.ReadLine());

                        Room room = roomRepo.GetById(id);

                        Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Add a room"):
                        Console.Write("Room name: ");
                        string name = Console.ReadLine();

                        Console.Write("Max occupancy: ");
                        int max = int.Parse(Console.ReadLine());

                        Room roomToAdd = new Room()
                        {
                            Name = name,
                            MaxOccupancy = max
                        };

                        roomRepo.Insert(roomToAdd);

                        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Show all chores"):
                        List<Chore> chores = choreRepo.GetAll();
                        foreach (Chore c in chores)
                        {
                            Console.WriteLine($"{c.Name} has an Id of {c.Id}.");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;


                    case ("Search for chores"):
                        Console.Write("Chore Id: ");
                        int choreId = int.Parse(Console.ReadLine());

                        Chore chore = choreRepo.GetById(choreId);

                        Console.WriteLine($"{chore.Id} - {chore.Name})");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Add a chore"):
                        Console.Write("Chore name: ");
                        string choreName = Console.ReadLine();

                        Chore choreToAdd = new Chore()
                        {
                            Name = choreName,

                        };

                        choreRepo.Insert(choreToAdd);

                        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an Id of {choreToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;


                    case ("Search for roommates"):
                        Console.Write("Roommate Id: ");
                        int roommateId = int.Parse(Console.ReadLine());

                        Roommate roommate = roommateRepo.GetById(roommateId);

                        Console.WriteLine($"Name: {roommate.FirstName} Rent Portion:{roommate.RentPortion} Room: {roommate.Room.Name}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("View unassigned chores"):
                        List<Chore> unassignedChores = choreRepo.GetUnassignedChores();
                        Console.WriteLine("Unassigned Chores:");
                        foreach (Chore unassignedChore in unassignedChores)
                        {
                            Console.WriteLine($"{unassignedChore.Name} has an Id of {unassignedChore.Id}.");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Update a chore"):
                        Console.Write("Enter the Chore Id to update: ");
                        int choreIdToUpdate = int.Parse(Console.ReadLine());

                        Chore choreToUpdate = choreRepo.GetById(choreIdToUpdate);
                        if (choreToUpdate != null)
                        {
                            Console.Write("Enter the new chore name: ");
                            string newChoreName = Console.ReadLine();

                            choreToUpdate.Name = newChoreName;

                            choreRepo.Update(choreToUpdate);

                            Console.WriteLine($"Chore with Id {choreToUpdate.Id} has been updated.");
                        }
                        else
                        {
                            Console.WriteLine($"Chore with Id {choreIdToUpdate} does not exist.");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Delete a chore"):
                        Console.WriteLine("Chores:");
                        List<Chore> allChores = choreRepo.GetAll();
                        foreach (Chore c in allChores)
                        {
                            Console.WriteLine($"Chore Id: {c.Id}, Chore Name: {c.Name}");
                        }

                        Console.Write("Enter the Chore Id to delete: ");
                        int choreIdToDelete = int.Parse(Console.ReadLine());

                        try
                        {
                            choreRepo.DeleteChore(choreIdToDelete);
                            Console.WriteLine($"Chore with Id {choreIdToDelete} has been deleted.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }

                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;


                    case ("Print roommate report"):
                        Console.WriteLine("Roommate Report\n");
                        PrintRoommateReport(roommateRepo.GetAll());
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Update a room"):
                        List<Room> roomOptions = roomRepo.GetAll();
                        foreach (Room r in roomOptions)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }

                        Console.Write("Which room would you like to update? ");
                        int selectedRoomId = int.Parse(Console.ReadLine());
                        Room selectedRoom = roomOptions.FirstOrDefault(r => r.Id == selectedRoomId);

                        Console.Write("New Name: ");
                        selectedRoom.Name = Console.ReadLine();

                        Console.Write("New Max Occupancy: ");
                        selectedRoom.MaxOccupancy = int.Parse(Console.ReadLine());

                        roomRepo.Update(selectedRoom);

                        Console.WriteLine("Room has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Delete a room"):
                        Console.WriteLine("Rooms:");
                        List<Room> allRooms = roomRepo.GetAll();
                        foreach (Room r in allRooms)
                        {
                            Console.WriteLine($"Room Id: {r.Id}, Room Name: {r.Name}");
                        }

                        Console.Write("Enter the Room Id to delete: ");
                        int roomIdToDelete = int.Parse(Console.ReadLine());

                        roomRepo.Delete(roomIdToDelete);

                        Console.WriteLine($"Room with Id {roomIdToDelete} has been deleted.");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Exit"):
                        runProgram = false;
                        break;
                }
            }
        }

        static void PrintRoommateReport(List<Roommate> roommates)
        {
            foreach (Roommate roommate in roommates)
            {
                string roomName = (roommate.Room != null) ? roommate.Room.Name : "No Room Assigned";

                Console.WriteLine($"{roommate.Id}: {roommate.FirstName}, Rent Portion: {roommate.RentPortion}, Room: {roomName}");
            }
        }



        static string GetMenuSelection()
        {
            Console.Clear();

            List<string> options = new List<string>()
            {
                "Show all rooms",
                "Search for room",
                "Add a room",
                "Update a room",
                "Delete a room",
                "Show all chores",
                "Search for chores",
                "Add a chore",
                "Update a chore",
                "Delete a chore",
                "Search for roommates",
                "View unassigned chores",
                "Print roommate report",
                "Exit"
            };

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Select an option > ");

                    string input = Console.ReadLine();
                    int index = int.Parse(input) - 1;
                    return options[index];
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }
    }
}

                
            