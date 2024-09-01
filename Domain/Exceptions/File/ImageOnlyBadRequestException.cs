namespace Domain.Exceptions.File;

public class ImageOnlyBadRequestException : BadRequestException
{
    public ImageOnlyBadRequestException() 
        : base("The file should be an image")
    {
    }
}
