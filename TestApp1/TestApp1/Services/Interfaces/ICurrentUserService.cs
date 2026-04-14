namespace TestApp1.Services.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetRole();
}