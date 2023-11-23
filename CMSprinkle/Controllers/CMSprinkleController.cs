using System;
using System.Threading.Tasks;
using CMSprinkle.Data;
using CMSprinkle.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CMSprinkle.Controllers;
public class SprinkleController : Controller
{
    private readonly ICMSprinkleAuth _auth;
    private readonly ICMSprinkleDataService _dataService;

    public SprinkleController(ICMSprinkleAuth auth, ICMSprinkleDataService dataService)
    {
        _auth = auth;
        _dataService = dataService;
    }

    [HttpGet]
    [CMSprinkleRoute("/home")]
    public async Task<IActionResult> Home()
    {
        if (!(await _auth.IsAllowed())) return Unauthorized();

        return View(await _dataService.GetAllForHome());
    }

    [HttpGet]
    [CMSprinkleRoute("/add")]
    public async Task<IActionResult> Add()
    {
        if (!(await _auth.IsAllowed())) return Unauthorized();

        return View(new AddContentSubmitModel());
    }

    [CMSprinkleRoute("/add")]
    [HttpPost]
    public async Task<IActionResult> Add(AddContentSubmitModel model)
    {
        if (!(await _auth.IsAllowed())) return Unauthorized();

        try
        {
            if (!ModelState.IsValid)
                return View(model);
            await _dataService.AddNew(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("",ex.GetAllExceptionMessages());
            return View(model);
        }

        return RedirectToAction("Home");
    }

    [HttpGet]
    [CMSprinkleRoute("/edit/{contentKey}")]
    public async Task<IActionResult> Edit(string contentKey)
    {
        if (!(await _auth.IsAllowed())) return Unauthorized();

        var content = await _dataService.GetAdmin(contentKey);

        var editModel = new EditViewModel();
        editModel.Key = contentKey;
        editModel.Content = content;

        return View(editModel);
    }

    [HttpPost]
    [CMSprinkleRoute("/edit/{contentKey}")]
    public async Task<IActionResult> Edit(EditContentSubmitModel model, string contentKey)
    {
        if (!(await _auth.IsAllowed())) return Unauthorized();

        await _dataService.Update(contentKey, model);

        return RedirectToAction("Home");
    }

    [HttpGet]
    [CMSprinkleRoute("/delete/{contentKey}")]
    public async Task<IActionResult> Delete(string contentKey)
    {
        if (!(await _auth.IsAllowed())) return Unauthorized();
    
        await _dataService.Delete(contentKey);
    
        return RedirectToAction("Home");
    }
}