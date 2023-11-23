using CMSprinkle.ViewModels;
using System.Threading.Tasks;

namespace CMSprinkle.Data;

public interface ICMSprinkleDataService
{
    Task<string> GetAdmin(string contentKey);
    Task<string> Get(string contentKey);
    Task<CMSprinkleHome> GetAllForHome();
    Task AddNew(AddContentSubmitModel model);
    Task Update(string contentKey, EditContentSubmitModel model);
}