using System.ComponentModel.DataAnnotations;

namespace CMSprinkle.ViewModels;

public class EditContentSubmitModel
{
    [MaxLength(10000000, ErrorMessage = "Content is limited to a length of 10,000,000")]
    public string Content { get; set; }
}