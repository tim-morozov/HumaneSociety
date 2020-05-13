using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
           switch(crudOperation)
            {
                case "create":
                    AddEmployee(employee);
                    break;

                case "find":
                    FindEmployee(employee);
                    break;

                case "update":
                    UpdateEmployee(employee);
                    break;
                case "remove":
                    RemoveEmployee(employee);
                    break;

            }

                
        }

        internal static void AddEmployee(Employee employee)
        {
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }

       internal static Employee FindEmployee(Employee employee)
        {
            try
            {
               Employee foundEmployee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();
                return foundEmployee;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Employee not found!");
                return FindEmployee(employee);
            }
        }

        internal static void UpdateEmployee(Employee employee)
        {
            Employee updateEmployee = null;

            try
            {
                updateEmployee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No Employee found");
                Console.WriteLine("No update have been made.");
                return;
            }

            updateEmployee.FirstName = UserInterface.GetUserInput();
            updateEmployee.LastName = UserInterface.GetUserInput();
            updateEmployee.UserName = UserInterface.GetUserInput();
            updateEmployee.Password = UserInterface.GetUserInput();

            db.SubmitChanges();
        }

        internal static void RemoveEmployee(Employee employee)
        {
            db.Employees.DeleteOnSubmit(employee);
            db.SubmitChanges();
        }
        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal getAnimalFromDb = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            if (getAnimalFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return getAnimalFromDb;
            }
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animalFromDb = GetAnimalByID(animalId);
            if (updates.ContainsKey(1)) //Category
            {
                animalFromDb.Category = db.Categories.Where(c => c.Name == updates[1]).FirstOrDefault();
            }
            else if (updates.ContainsKey(2)) //Name
            {
                
            }
            else if (updates.ContainsKey(3)) //Age
            {
                try
                {
                    
                }
                catch
                {
                    Console.WriteLine("Invalid input.");
                }
            }
            else if (updates.ContainsKey(4)) //Demeanor
            {
                
            }
            else if (updates.ContainsKey(5)) //KidFriendly (Bool)
            {
                try
                {
                    
                }
                catch
                {
                    Console.WriteLine("Invalid input.");
                }
            }
            else if (updates.ContainsKey(6)) //PetFriendly (Bool)
            {
                try
                {
                    
                }
                catch
                {
                    Console.WriteLine("Invalid input.");
                }
            }
            else if (updates.ContainsKey(7)) //Weight
            {
                try
                {
                    
                }
                catch
                {
                    Console.WriteLine("Invalid input.");
                }
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            Animal GetAnimalFromDb = GetAnimalByID(animal.AnimalId);
            db.Animals.DeleteOnSubmit(GetAnimalFromDb);
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            if(updates.ContainsKey(1))
            {
                var category = db.Categories.Where(c => c.Name == updates[1]).FirstOrDefault();
                return db.Animals.Where(a => a.CategoryId == category.CategoryId);
            }
            else if(updates.ContainsKey(2))
            {
                return db.Animals.Where(a => a.Name == updates[1]);
            }
            else if (updates.ContainsKey(3))
            {
                int age = Convert.ToInt32(updates[3]);
                 return db.Animals.Where(a => a.Age == age);
            }
            else if(updates.ContainsKey(4))
            {
                return db.Animals.Where(a => a.Demeanor == updates[4]);
            }
            else if(updates.ContainsKey(5))
            {
                if (updates[5].Contains("yes"))
                {
                    return db.Animals.Where(a => a.KidFriendly == true);
                }
                else
                {
                    return db.Animals.Where(a => a.KidFriendly == false);
                }
            }
            else if(updates.ContainsKey(6))
            {
                if (updates[6].Contains("yes"))
                {
                    return db.Animals.Where(a => a.PetFriendly == true);
                }
                else
                {
                    return db.Animals.Where(a => a.PetFriendly == false);
                }
            }
            else if(updates.ContainsKey(7))
            {
                int weight = Convert.ToInt32(updates[7]);                
                return db.Animals.Where(a => a.Weight == weight);
            }
            else if(updates.ContainsKey(8))
            {
                int id = Convert.ToInt32(updates[8]);
                return db.Animals.Where(a => a.AnimalId == id);
            }
          return null;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            Category category = db.Categories.Where(c => c.Name == categoryName).Single();

            return category.CategoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(r => r.AnimalId == animalId).Single();

            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan dietPlan = db.DietPlans.Where(d => d.Name == dietPlanName).Single();

            return dietPlan.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption newAdoption = new Adoption();
            newAdoption.AnimalId = animal.AnimalId;
            newAdoption.ClientId = client.ClientId;
            newAdoption.ApprovalStatus = "pending";
            newAdoption.AdoptionFee = 50;
            newAdoption.PaymentCollected = false;
            db.Adoptions.InsertOnSubmit(newAdoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            return db.Adoptions.Where(a => a.ApprovalStatus == "pending");
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted == true)
            {
                adoption.ApprovalStatus = "Approved";
                adoption.PaymentCollected = true;

            }
            else
            {
                adoption.ApprovalStatus = "Approved";
            }
            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoptionFromDb = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoptionFromDb);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            return db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId);
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            Animal updateAnimal = null;
            var newShot = db.Shots.Where(s => s.Name == shotName).Single();
            try
            {
                 updateAnimal = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No animal found");
                Console.WriteLine("No update have been made.");
                return;
            }
            AnimalShot newAnimalShot = new AnimalShot();
            newAnimalShot.AnimalId = updateAnimal.AnimalId;
            newAnimalShot.ShotId = newShot.ShotId;
            newAnimalShot.DateReceived = DateTime.Today;

            db.SubmitChanges();
        }
    }
}