using Microsoft.EntityFrameworkCore;

namespace LinqExpressionParser.Tests.AspNetCore.Models
{
    [PrimaryKey(nameof(Id))]
    public record User(int Id, string Email, string PasswordHash, string Name, DateTimeOffset SignupAt, DateTimeOffset LastLoginAt, bool IsBlocked, bool IsWithdraw, int RoleId, int MembershipId, Role Role, Membership Membership, List<OrderSet> OrderSets, List<Review> Reviews, List<Point> Points);
    [PrimaryKey(nameof(Id))]
    public record Role(int Id, string Name, int Priority, List<User> Users);
    [PrimaryKey(nameof(Id))]
    public record Membership(int Id, string Name, double PointPercentage, List<User> Users);
    [PrimaryKey(nameof(Id))]
    public record Item(int Id, string Name, double Price, int SelledCount, int CategoryId, Category Category, List<Review> Reviews);
    [PrimaryKey(nameof(Id))]
    public record Category(int Id, string Name, int? ParentCategoryId, Category? ParentCategory, List<Category> ChildCategories, List<Item> Items);
    [PrimaryKey(nameof(Id))]
    public record Review(int Id, string Title, string Content, int Rating, DateTimeOffset CreatedAt, DateTimeOffset ModifiedAt, int UserId, int ItemId, User User, Item Item);
    [PrimaryKey(nameof(Id))]
    public record Point(int Id, int Balance, int Amount, DateTimeOffset CreatedAt, DateTimeOffset ExpiresAt, int UserId, User User);
    [PrimaryKey(nameof(Id))]
    public record OrderSet(int Id, DateTimeOffset OrderedAt, string Address, string PostCode, string ReceiverName, string? Message, string? PhoneNumber, int UsedPoints, int UserId, User User, List<Order> Orders);
    [PrimaryKey(nameof(Id))]
    public record Order(int Id, int OrderQty, int CancelQty, double PricePerItem, int ItemId, int OrderSetId, Item Item, OrderSet OrderSet);
}