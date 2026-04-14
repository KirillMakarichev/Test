namespace TestApp1.Services.Interfaces;

public interface ICodeGenerator
{
    string GenerateCustomerCode(DateTime registrationDate);
    string GenerateProductCode();
}