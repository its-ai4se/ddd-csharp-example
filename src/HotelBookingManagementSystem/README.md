# Hotel Booking Management System (HBMS) Domain Model

This project implements a Domain-Driven Design (DDD) solution for the Hotel Booking Management System, designed for business travellers to book special accommodation deals offered by participating hotels.

## Overview

The Hotel Booking Management System (HBMS) enables business travellers to:

- Register with billing information and travel preferences
- Search for accommodation with specific criteria
- Create preliminary bookings that trigger competitive offers
- Receive special offers from competing hotels within 24 hours
- Finalize bookings with payment options
- Manage cancellations with appropriate fees

## Requirements Description

```txt
Hotel Booking Management System (HBMS),"Hotel Booking Management System (HBMS)

Business travellers use HMBS for booking special accommodation deals offered by participating hotels. Travellers register to HBMS by providing their name, billing information (incl. company name and address) and optional travel preferences (e.g. breakfast included, free wifi, 24/7 front desk, etc.).

When searching for accommodation, the traveller specifies the city, the date of arrival and departure, the number of needed rooms and the type of rooms (e.g. single, double, twin), minimum hotel rating (stars), a tentative budget (max. cost per night), and optionally, further travel preferences to filter offers in the search results. HBMS lists all available offers of hotels for the given travel period, and the traveller can either create a preliminary booking or complete a booking in the regular way.

In case of a preliminary booking, HBMS forwards the key parameters of the booking information (i.e. price, city area, hotel rating and key preferences and a unique booking identifier) to other hotels so that they can compete for the traveller with special offers provided within the next 24 hours. After 24-hour deadline, HBMS sends the five best special offers to the traveller who can switch to the new offer or proceed with the original preliminary booking.

In both cases, the traveller needs to provide credit card information to finalize a booking. Each finalized booking can be either pre-paid (i.e. paid immediately when it cannot be reimbursed), or paid at hotel (when the traveller pays during his/her stay). A finalized booking needs to be confirmed by the hotel within 24 hours. A booking may also contain a cancellation deadline: if the traveller cancels a confirmed booking before this deadline, then there are no further consequences. However, if a confirmed booking is cancelled after this deadline, then 1-night accommodation is charged for the traveller. HBMS stores all past booking information for a traveller to calculate a reliability rating.

Each hotel is located in a city at a particular address, and possibly run by a hotel chain. A hotel may announce its available types of rooms for a given period in HBMS, and may also inform HBMS when a particular type of room is fully booked. HBMS sends information about the preliminary booking information to competitor hotels together with the traveller’s preferences and his/her reliability rating. The competitor hotels may then provide a special offer. Once a booking is finalized, the hotel needs to send a confirmation to the traveller. If a completed booking is not confirmed by the hotel within 24 hours, then HBMS needs to cancel it, and reimburse the traveller in case of a pre-paid booking. If the hotel needs to cancel a confirmed booking, then financial compensation must be offered to the traveller.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **TravellerAggregate** - Represents business travellers with billing information and preferences
2. **HotelAggregate** - Represents hotels with amenities, ratings, and chain information
3. **RoomAggregate** - Represents room types and availability for specific periods
4. **BookingAggregate** - Manages booking lifecycle from preliminary to confirmed states
5. **SpecialOfferAggregate** - Handles competitive offers from other hotels

### Value Objects

- **PersonName** - First and last name validation
- **Address** - Complete address with validation
- **EmailAddress** - Email validation
- **PhoneNumber** - Phone number validation
- **Money** - Currency handling with operations
- **DateRange** - Date period with overlap detection
- **HotelRating** - 1-5 star rating system
- **ReliabilityRating** - Traveller reliability scoring
- **TravelPreferences** - Amenity preferences (breakfast, WiFi, etc.)
- **RoomType** - Room configuration (single, double, twin, suite)
- **CreditCardInfo** - Secure credit card information handling

### Domain Services

- **BookingService** - Handles booking creation, finalization, and confirmation
- **OfferService** - Manages competitive offers and selection process
- **ReliabilityService** - Calculates traveller reliability ratings

## Key Business Rules

1. **Preliminary Bookings**: Travellers can create preliminary bookings that are sent to competing hotels
2. **24-Hour Offers**: Competing hotels have 24 hours to submit special offers
3. **Best Offer Selection**: System selects the 5 best offers based on savings
4. **Payment Options**: Bookings can be pre-paid or paid at hotel
5. **Confirmation Window**: Hotels must confirm bookings within 24 hours
6. **Cancellation Policy**: Cancellations after deadline incur 1-night fee
7. **Reliability Rating**: System tracks traveller reliability based on booking history

## Repository Interfaces

- `ITravellerRepository` - Traveller aggregate persistence
- `IHotelRepository` - Hotel aggregate persistence
- `IRoomRepository` - Room aggregate persistence
- `IBookingRepository` - Booking aggregate persistence
- `ISpecialOfferRepository` - Special offer aggregate persistence

## Booking Lifecycle

1. **Preliminary Booking**: Created with basic information
2. **Competitor Notification**: Sent to competing hotels in same city
3. **Special Offers**: Competing hotels submit offers within 24 hours
4. **Offer Selection**: System presents best 5 offers to traveller
5. **Finalization**: Traveller provides payment information
6. **Confirmation**: Hotel confirms booking within 24 hours
7. **Completion**: Booking is confirmed and ready for stay

## Testing

The solution includes a comprehensive demonstration that shows:

- Creating travellers with different preferences
- Setting up hotels with various amenities
- Creating preliminary bookings
- Generating competitive offers
- Processing the complete booking lifecycle
- Calculating reliability ratings
- Handling cancellation scenarios

## Project Structure

```
src/HotelBookingManagementSystem/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   └── ValueObjects/    # Value objects
│   ├── Traveller/           # Traveller aggregate and repository
│   ├── Hotel/               # Hotel aggregate and repository
│   ├── Room/                # Room aggregate and repository
│   ├── Booking/             # Booking aggregate and repository
│   ├── SpecialOffer/        # Special offer aggregate and repository
│   └── Services/            # Domain services
└── tests/
    └── HotelBookingManagementSystemDemo.cs   # Comprehensive demonstration
```

## Usage Example

```csharp
// Create a business traveller
var traveller = new TravellerAggregate(
    new PersonName("John", "Doe"),
    new Address("123 Business St", "Toronto", "ON", "M5V 3A8"),
    "TechCorp Inc",
    new Address("456 Corporate Ave", "Toronto", "ON", "M5V 3B9"),
    new EmailAddress("john.doe@company.com"),
    new PhoneNumber("416-555-0123"),
    new TravelPreferences(breakfastIncluded: true, freeWifi: true, businessCenter: true)
);

// Create a preliminary booking
var booking = bookingService.CreatePreliminaryBooking(
    traveller.Id,
    hotelId,
    roomId,
    new DateRange(DateTime.Now.AddDays(7), DateTime.Now.AddDays(10)),
    1,
    PaymentType.PrePaid
);

// Send to competitors for offers
offerService.SendPreliminaryBookingToCompetitors(booking.Id);

// Get best offers
var bestOffers = offerService.GetBestOffers(booking.Id, 5);

// Finalize booking
bookingService.FinalizeBooking(booking.Id, creditCardInfo);
bookingService.ConfirmBooking(booking.Id);
```

## Business Scenarios Covered

- **Registration**: Travellers register with company and billing information
- **Search**: Search hotels by city, dates, room type, rating, and budget
- **Preliminary Booking**: Create tentative bookings sent to competitors
- **Competitive Offers**: Hotels compete with special offers
- **Offer Selection**: System presents best offers to travellers
- **Payment Processing**: Handle pre-paid and pay-at-hotel options
- **Confirmation Management**: Hotels confirm bookings within 24 hours
- **Cancellation Handling**: Manage cancellations with appropriate fees
- **Reliability Tracking**: Calculate traveller reliability ratings
- **Compensation**: Handle hotel-initiated cancellations with compensation
