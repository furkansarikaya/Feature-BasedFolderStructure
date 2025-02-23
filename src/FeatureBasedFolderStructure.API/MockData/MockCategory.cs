namespace FeatureBasedFolderStructure.API.MockData;

public class MockCategory
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public static IEnumerable<MockCategory> GetDatabaseExampleModels()
    {
        return new List<MockCategory>
        { 
            new(){Id = "1", Name = "Default Category", Description = "Default Category Description"}
        };
    }
}