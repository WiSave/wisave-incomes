namespace WiSave.Incomes.Core.Infrastructure.Identity;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
}
