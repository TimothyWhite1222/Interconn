using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly long _maxBytes;

    public MaxFileSizeAttribute(long maxBytes)
    {
        _maxBytes = maxBytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var files = value as IEnumerable<IFormFile>;
        if (files != null)
        {
            foreach (var file in files)
            {
                if (file.Length > _maxBytes)
                {
                    return new ValidationResult($"檔案大小不能超過 {_maxBytes / (1024 * 1024)} MB");
                }
            }
        }

        return ValidationResult.Success;
    }
}