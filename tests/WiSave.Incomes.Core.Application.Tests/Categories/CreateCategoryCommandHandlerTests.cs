using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;
using WiSave.Incomes.Core.Application.Categories;
using WiSave.Incomes.Core.Application.Categories.CommandHandlers;

namespace WiSave.Incomes.Core.Application.Tests.Categories;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_saves_category_and_publishes_category_created_event()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new CreateCategoryCommandHandler(repository, publisher);
        var categoryId = Guid.Parse("018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var userId = Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var command = new CreateCategory(
            categoryId,
            userId,
            "Salary",
            3);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(["save", "publish"], operations);
        Assert.Equal(categoryId, repository.SavedId);
        Assert.Equal(userId, repository.SavedUserId);
        Assert.Equal("Salary", repository.SavedName);
        Assert.Equal(3, repository.SavedSortOrder);

        var published = Assert.IsType<CategoryCreated>(Assert.Single(publisher.Published));
        Assert.Equal(categoryId, published.Id);
        Assert.Equal(userId, published.UserId);
        Assert.Equal("Salary", published.Name);
        Assert.Equal(3, published.SortOrder);
    }

    [Fact]
    public async Task Handle_uses_zero_sort_order_when_command_sort_order_is_missing()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new CreateCategoryCommandHandler(repository, publisher);
        var command = new CreateCategory(
            Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"),
            Guid.Parse("318f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"),
            "Salary");

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(0, repository.SavedSortOrder);
        var published = Assert.IsType<CategoryCreated>(Assert.Single(publisher.Published));
        Assert.Equal(0, published.SortOrder);
    }

    private sealed class CapturingCategoryRepository(List<string> operations) : ICategoryRepository
    {
        public Guid SavedId { get; private set; }
        public Guid SavedUserId { get; private set; }
        public string? SavedName { get; private set; }
        public int SavedSortOrder { get; private set; }

        public Task CreateAsync(
            Guid id,
            Guid userId,
            string name,
            int sortOrder,
            CancellationToken ct = default)
        {
            operations.Add("save");
            SavedId = id;
            SavedUserId = userId;
            SavedName = name;
            SavedSortOrder = sortOrder;
            return Task.CompletedTask;
        }
    }

    private sealed class CapturingEventPublisher(List<string> operations) : IEventPublisher
    {
        public List<object> Published { get; } = [];

        public Task PublishAsync(object @event, CancellationToken ct = default)
        {
            operations.Add("publish");
            Published.Add(@event);
            return Task.CompletedTask;
        }
    }
}
