namespace WiSave.Incomes.Core.Infrastructure.Identity;

public interface ICurrentUser
{
    string UserId { get; }
    string Email { get; }
}
