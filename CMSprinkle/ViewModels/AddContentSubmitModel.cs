using System.ComponentModel.DataAnnotations;

namespace CMSprinkle.ViewModels;

public class AddContentSubmitModel
{
    [Required(ErrorMessage = "Content key is required")]
    [MaxLength(90, ErrorMessage = "Content Key must be 90 characters or less")]
    public string Key { get; set; }

    [MaxLength(10000000, ErrorMessage = "Content is limited to a length of 10,000,000")]
    public string Content { get; set; }
}
