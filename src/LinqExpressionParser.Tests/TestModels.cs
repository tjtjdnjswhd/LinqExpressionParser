namespace LinqExpressionParser.Tests;

public record User(int Id, string Email, string PasswordHash, string Name, DateTimeOffset SignupAt, DateTimeOffset LastLoginAt, bool IsBlocked, bool IsWithdraw, int RoleId, int MembershipId, Role Role, Membership Membership, List<OrderSet> OrderSets, List<Review> Reviews, List<Point> Points);
public record Role(int Id, string Name, int Priority, List<User> Users);
public record Membership(int Id, string Name, double PointPercentage, List<User> Users);
public record Item(int Id, string Name, double Price, int SelledCount, Category Category, List<Review> Reviews);
public record Category(int Id, string Name, int? ParentCategoryId, Category? ParentCategory, List<Category> ChildCategories);
public record Review(int Id, string Title, string Content, int Rating, DateTimeOffset CreatedAt, DateTimeOffset ModifiedAt, int UserId, int ItemId, User User, Item Item);
public record Point(int Id, int Balance, int Amount, DateTimeOffset CreatedAt, DateTimeOffset ExpiresAt, int UserId, User User);
public record OrderSet(int Id, DateTimeOffset OrderedAt, string Address, string PostCode, string ReceiverName, string? Message, string? PhoneNumber, int UsedPoints, int UserId, User User, List<Order> Orders);
public record Order(int Id, int OrderQty, int CancelQty, double PricePerItem, int ItemId, int OrderSetId, Item Item, OrderSet OrderSet);