using System.ComponentModel.DataAnnotations;

namespace CMSprinkle.ViewModels;

public class AddContentSubmitModel
{
    [Required]
    [MaxLength(90)]
    public string Key { get; set; }

    [MaxLength(10000000)]
    public string Content { get; set; }
}
