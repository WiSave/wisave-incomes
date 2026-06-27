using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;
using WiSave.Incomes.Core.Application.Categories;
using WiSave.Incomes.Core.Application.Categories.CommandHandlers;

namespace WiSave.Incomes.Core.Application.Tests.Categories;

public class CategoryCommandHandlerTests
{
    private static readonly Guid UserId = Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
    private static readonly Guid CategoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
    private static readonly Guid SubcategoryId = Guid.Parse("318f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");

    [Fact]
    public async Task UpdateCategory_saves_and_publishes_category_updated()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new UpdateCategoryCommandHandler(repository);
        var command = new UpdateCategory(CategoryId, UserId, "Salary", 4);

        await handler.Handle(command, publisher, CancellationToken.None);

        Assert.Equal(["update", "publish"], operations);
        Assert.Equal((CategoryId, UserId, "Salary", 4), repository.UpdatedCategory);
        var published = Assert.IsType<CategoryUpdated>(Assert.Single(publisher.Published));
        Assert.Equal(CategoryId, published.Id);
        Assert.Equal(UserId, published.UserId);
        Assert.Equal("Salary", published.Name);
        Assert.Equal(4, published.SortOrder);
    }

    [Fact]
    public async Task DeleteCategory_saves_and_publishes_category_deleted()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new DeleteCategoryCommandHandler(repository);
        var command = new DeleteCategory(CategoryId, UserId);

        await handler.Handle(command, publisher, CancellationToken.None);

        Assert.Equal(["delete", "publish"], operations);
        Assert.Equal((CategoryId, UserId), repository.DeletedCategory);
        var published = Assert.IsType<CategoryDeleted>(Assert.Single(publisher.Published));
        Assert.Equal(CategoryId, published.Id);
        Assert.Equal(UserId, published.UserId);
    }

    [Fact]
    public async Task CreateSubcategory_saves_and_publishes_subcategory_created()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new CreateSubcategoryCommandHandler(repository);
        var command = new CreateSubcategory(SubcategoryId, CategoryId, UserId, "Base pay", 2);

        await handler.Handle(command, publisher, CancellationToken.None);

        Assert.Equal(["create-subcategory", "publish"], operations);
        Assert.Equal((SubcategoryId, CategoryId, UserId, "Base pay", 2), repository.CreatedSubcategory);
        var published = Assert.IsType<SubcategoryCreated>(Assert.Single(publisher.Published));
        Assert.Equal(SubcategoryId, published.Id);
        Assert.Equal(CategoryId, published.CategoryId);
        Assert.Equal(UserId, published.UserId);
        Assert.Equal("Base pay", published.Name);
        Assert.Equal(2, published.SortOrder);
    }

    [Fact]
    public async Task DeleteSubcategory_saves_and_publishes_subcategory_deleted()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new DeleteSubcategoryCommandHandler(repository);
        var command = new DeleteSubcategory(CategoryId, SubcategoryId, UserId);

        await handler.Handle(command, publisher, CancellationToken.None);

        Assert.Equal(["delete-subcategory", "publish"], operations);
        Assert.Equal((CategoryId, SubcategoryId, UserId), repository.DeletedSubcategory);
        var published = Assert.IsType<SubcategoryDeleted>(Assert.Single(publisher.Published));
        Assert.Equal(CategoryId, published.CategoryId);
        Assert.Equal(SubcategoryId, published.Id);
        Assert.Equal(UserId, published.UserId);
    }

    [Fact]
    public async Task UpdateSubcategory_saves_and_publishes_subcategory_updated()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new UpdateSubcategoryCommandHandler(repository);
        var command = new UpdateSubcategory(CategoryId, SubcategoryId, UserId, "Bonus", 5);

        await handler.Handle(command, publisher, CancellationToken.None);

        Assert.Equal(["update-subcategory", "publish"], operations);
        Assert.Equal((CategoryId, SubcategoryId, UserId, "Bonus", 5), repository.UpdatedSubcategory);
        var published = Assert.IsType<SubcategoryUpdated>(Assert.Single(publisher.Published));
        Assert.Equal(CategoryId, published.CategoryId);
        Assert.Equal(SubcategoryId, published.Id);
        Assert.Equal(UserId, published.UserId);
        Assert.Equal("Bonus", published.Name);
        Assert.Equal(5, published.SortOrder);
    }

    [Fact]
    public async Task UpdateCategory_does_not_publish_when_category_is_not_mutated()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations) { MutationResult = false };
        var publisher = new CapturingEventPublisher(operations);
        var handler = new UpdateCategoryCommandHandler(repository);
        var command = new UpdateCategory(CategoryId, UserId, "Salary", 4);

        await handler.Handle(command, publisher, CancellationToken.None);

        Assert.Equal(["update"], operations);
        Assert.Empty(publisher.Published);
    }

    [Fact]
    public async Task UpdateSubcategory_does_not_publish_when_subcategory_is_not_mutated()
    {
        var operations = new List<string>();
        var repository = new CapturingCategoryRepository(operations) { MutationResult = false };
        var publisher = new CapturingEventPublisher(operations);
        var handler = new UpdateSubcategoryCommandHandler(repository);
        var command = new UpdateSubcategory(CategoryId, SubcategoryId, UserId, "Bonus", 5);

        await handler.Handle(command, publisher, CancellationToken.None);

        Assert.Equal(["update-subcategory"], operations);
        Assert.Empty(publisher.Published);
    }

    private sealed class CapturingCategoryRepository(List<string> operations) : ICategoryRepository
    {
        public bool MutationResult { get; set; } = true;
        public (Guid Id, Guid UserId, string Name, int SortOrder)? CreatedCategory { get; private set; }
        public (Guid Id, Guid UserId, string Name, int SortOrder)? UpdatedCategory { get; private set; }
        public (Guid Id, Guid UserId)? DeletedCategory { get; private set; }
        public (Guid Id, Guid CategoryId, Guid UserId, string Name, int SortOrder)? CreatedSubcategory { get; private set; }
        public (Guid CategoryId, Guid Id, Guid UserId, string Name, int SortOrder)? UpdatedSubcategory { get; private set; }
        public (Guid CategoryId, Guid Id, Guid UserId)? DeletedSubcategory { get; private set; }

        public Task CreateAsync(
            Guid id,
            Guid userId,
            string name,
            int sortOrder,
            CancellationToken ct = default)
        {
            operations.Add("create");
            CreatedCategory = (id, userId, name, sortOrder);
            return Task.CompletedTask;
        }

        public Task<bool> UpdateAsync(
            Guid id,
            Guid userId,
            string name,
            int sortOrder,
            CancellationToken ct = default)
        {
            operations.Add("update");
            UpdatedCategory = (id, userId, name, sortOrder);
            return Task.FromResult(MutationResult);
        }

        public Task<bool> DeleteAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default)
        {
            operations.Add("delete");
            DeletedCategory = (id, userId);
            return Task.FromResult(MutationResult);
        }

        public Task<bool> CreateSubcategoryAsync(
            Guid id,
            Guid categoryId,
            Guid userId,
            string name,
            int sortOrder,
            CancellationToken ct = default)
        {
            operations.Add("create-subcategory");
            CreatedSubcategory = (id, categoryId, userId, name, sortOrder);
            return Task.FromResult(MutationResult);
        }

        public Task<bool> UpdateSubcategoryAsync(
            Guid categoryId,
            Guid id,
            Guid userId,
            string name,
            int sortOrder,
            CancellationToken ct = default)
        {
            operations.Add("update-subcategory");
            UpdatedSubcategory = (categoryId, id, userId, name, sortOrder);
            return Task.FromResult(MutationResult);
        }

        public Task<bool> DeleteSubcategoryAsync(
            Guid categoryId,
            Guid id,
            Guid userId,
            CancellationToken ct = default)
        {
            operations.Add("delete-subcategory");
            DeletedSubcategory = (categoryId, id, userId);
            return Task.FromResult(MutationResult);
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
