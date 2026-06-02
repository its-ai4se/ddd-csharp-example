namespace HelpingHandStore.Domain.Shared.ValueObjects;

/// <summary>
/// The standard list of 134 article categories an H2S employee may assign (BR-016).
/// </summary>
public static class CategoryCatalog
{
    private static readonly IReadOnlyList<string> _categories = new[]
    {
        // Baby
        "Baby clothing", "Baby toys", "Baby furniture", "Baby accessories", "Baby strollers", "Baby car seats",
        // Children
        "Children's clothing", "Children's shoes", "Children's toys", "Children's books", "Children's furniture", "Children's bicycles",
        // Women's
        "Women's winter boots", "Women's summer shoes", "Women's dresses", "Women's coats", "Women's jackets", "Women's sweaters",
        "Women's jeans", "Women's skirts", "Women's blouses", "Women's activewear", "Women's accessories", "Women's handbags",
        // Men's
        "Men's winter boots", "Men's summer shoes", "Men's shirts", "Men's coats", "Men's jackets", "Men's sweaters",
        "Men's jeans", "Men's trousers", "Men's suits", "Men's activewear", "Men's accessories", "Men's belts",
        // Large appliances
        "Refrigerator", "Freezer", "Washing machine", "Clothes dryer", "Dishwasher", "Oven", "Stove", "Range hood", "Air conditioner", "Water heater",
        // Small appliances
        "Microwave", "Toaster", "Blender", "Coffee maker", "Electric kettle", "Food processor", "Mixer", "Rice cooker", "Slow cooker", "Electric grill",
        // Consumer electronics
        "Television", "Computer", "Laptop", "Tablet", "Smartphone", "Monitor", "Printer", "Headphones", "Speakers", "Camera",
        // Furniture
        "Sofa", "Armchair", "Coffee table", "Dining table", "Dining chair", "Bed frame", "Mattress", "Dresser", "Nightstand", "Bookshelf", "Wardrobe", "Desk",
        // Kitchenware
        "Dishes", "Glassware", "Cutlery", "Cookware", "Bakeware", "Kitchen utensils", "Food storage containers", "Pots and pans",
        // Home textiles
        "Bedding", "Blankets", "Pillows", "Towels", "Curtains", "Rugs",
        // Books and media
        "Books", "Magazines", "Movies", "Music CDs", "Vinyl records", "Video games",
        // Sports and recreation
        "Sports equipment", "Exercise equipment", "Bicycles", "Camping gear", "Fishing gear", "Skis", "Skateboards", "Yoga mats",
        // Tools and hardware
        "Hand tools", "Power tools", "Gardening tools", "Hardware supplies", "Ladders", "Toolboxes", "Workbenches", "Safety equipment",
        // Toys and games
        "Board games", "Puzzles", "Stuffed animals", "Action figures", "Building blocks", "Dolls",
        // Musical instruments
        "Guitars", "Musical keyboards", "Drums", "Violins",
        // Decor and miscellaneous
        "Decorations", "Artwork", "Picture frames", "Mirrors", "Lamps", "Clocks", "Jewelry", "Watches", "Bags", "Luggage"
    };

    public static bool Contains(string name) =>
        _categories.Any(c => string.Equals(c, name, StringComparison.OrdinalIgnoreCase));

    public static string Canonical(string name) =>
        _categories.First(c => string.Equals(c, name, StringComparison.OrdinalIgnoreCase));
}
