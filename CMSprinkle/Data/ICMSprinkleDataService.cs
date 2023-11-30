using CMSprinkle.ViewModels;
using System.Threading.Tasks;

namespace CMSprinkle.Data;

public interface ICMSprinkleDataService
{
    Task InitializeDatabase();
    Task<string> GetAdmin(string contentKey);
    Task<GetContentResult> Get(string contentKey);
    Task<CMSprinkleHome> GetAllForHome();
    Task AddNew(AddContentSubmitModel model);
    Task Update(string contentKey, EditContentSubmitModel model);
    Task Delete(string contentKey);
}