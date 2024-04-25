using Discount.Grpc;

namespace Basket.Api.Basket.StoreBasket
{
    public record StoreBaskCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);

    public class StoreBasketCommandvalidator : AbstractValidator<StoreBaskCommand>
    {
        public StoreBasketCommandvalidator() 
        {
            RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
            RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }

    public class StoreBasketCommandHandler
        (IBasketRepository repository
        , DiscountProtoService.DiscountProtoServiceClient discountProto)
        : ICommandHandler<StoreBaskCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBaskCommand command, CancellationToken cancellationToken)
        {
            await DeductDiscount(command.Cart, cancellationToken);

            await repository.StoreBasket(command.Cart, cancellationToken);

            return new StoreBasketResult(command.Cart.UserName);
        }

        private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
        {
            foreach (var item in cart.Items)
            {
                var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }
}
