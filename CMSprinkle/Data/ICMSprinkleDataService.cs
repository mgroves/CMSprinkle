using CMSprinkle.ViewModels;
using System.Threading.Tasks;

namespace CMSprinkle.Data;

public interface ICMSprinkleDataService
{
    /// <summary>
    /// Any setup that's needed for the database. Create a table, collection, index, whatever.
    /// This is run once on web app startup.
    /// </summary>
    Task InitializeDatabase();

    /// <summary>
    /// Returns content for a given key, returns an error message as content if it's not been added yet.
    /// </summary>
    /// <param name="contentKey">Content key</param>
    /// <returns>GetContentResult, containing Key and Content</returns>
    Task<GetContentResult> Get(string contentKey);

    /// <summary>
    /// Returns all content for display in the admin area home page
    /// </summary>
    /// <returns>CMSprinkleHome, containing AllContent, which is a List of CMSprinkleContent</returns>
    Task<CMSprinkleHome> GetAllForHome();

    /// <summary>
    /// Add new content, also stamps with username and (create) timestamp
    /// </summary>
    /// <param name="model">object containing values to save to database</param>
    Task AddNew(AddContentSubmitModel model);

    /// <summary>
    /// Make changes to existing content, stamp with username and (update) timestamp
    /// </summary>
    /// <param name="contentKey">key for the content you want to update</param>
    /// <param name="model">object containing values to save to database</param>
    Task Update(string contentKey, EditContentSubmitModel model);

    /// <summary>
    /// Remove content
    /// </summary>
    /// <param name="contentKey">Key for the content to remove</param>
    Task Delete(string contentKey);
}