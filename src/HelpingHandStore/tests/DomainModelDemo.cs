using HelpingHandStore.Domain.Person;
using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Vehicle;
using HelpingHandStore.Domain.Route;
using HelpingHandStore.Domain.H2S;
using HelpingHandStore.Domain.Shared.ValueObjects;
using HelpingHandStore.Domain.Services;
using HelpingHandStore.Domain.Shared.Services;
using Xunit;

namespace HelpingHandStore.Domain.Tests;

public class DomainModelDemo
{
    [Fact]
    public void DemonstrateH2SDomainModel()
    {
        // Create H2S organization
        var montrealAddress = new Address("123 Main St", "Montreal", "QC", "H1A 1A1");
        var h2s = new H2SAggregate("Helping Hand Store Montreal", montrealAddress, "Montreal");

        // Create a person who can be resident, volunteer, and client
        var personName = new PersonName("John", "Doe");
        var personAddress = new Address("456 Oak Ave", "Montreal", "QC", "H2B 2B2");
        var phoneNumber = new PhoneNumber("(514) 555-0123");
        var emailAddress = new EmailAddress("john.doe@email.com");
        var person = new PersonAggregate(personName, personAddress, phoneNumber, emailAddress);

        // Register person as resident
        var personService = new PersonManagementService(new SystemClock());
        personService.RegisterResident(person);

        // Register person as volunteer with available days
        var availableDays = new List<DateOnly>
        {
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(2))
        };
        personService.RegisterVolunteer(person, availableDays);

        // Register person as client with needed categories
        var neededCategories = new List<ItemCategory> { ItemCategory.BabyClothing, ItemCategory.Refrigerator };
        personService.RegisterClient(person, neededCategories);

        // Create items for pickup
        var itemDescription = new ItemDescription("Baby clothes and small refrigerator");
        var itemDimensions = new Dimensions(1.5m, 1.0m, 0.8m);
        var itemWeight = new Weight(25.0m);
        var pickupDate = new ScheduledDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

        var secondHandArticle = new SecondHandArticle(itemDescription, itemDimensions, itemWeight, pickupDate, person.Id);
        var foodItem = new FoodItem(new ItemDescription("Canned goods"), new Dimensions(0.3m, 0.2m, 0.1m), new Weight(2.0m), pickupDate, person.Id);

        // Create vehicle
        var vehicle = new VehicleAggregate("ABC-123", new Dimensions(3.0m, 2.0m, 2.0m), new Weight(1000.0m));

        // Create route
        var route = new RouteAggregate(pickupDate.Date, vehicle.Id, person.Id);
        route.AddScheduledItem(secondHandArticle.Id);
        route.AddScheduledItem(foodItem.Id);

        // Process second hand article
        var itemProcessingService = new ItemProcessingService(new SystemClock());
        itemProcessingService.ProcessSecondHandArticle(secondHandArticle, ItemCategory.BabyClothing, true);

        // Verify the domain model
        Assert.True(person.HasRole<ResidentRole>());
        Assert.True(person.HasRole<VolunteerRole>());
        Assert.True(person.HasRole<ClientRole>());
        Assert.True(secondHandArticle.CanBeDistributed());
        Assert.True(foodItem.IsDeliveredToFoodBank == false);
        Assert.True(vehicle.IsAvailable());
        Assert.True(route.IsPlanned());
    }
}
