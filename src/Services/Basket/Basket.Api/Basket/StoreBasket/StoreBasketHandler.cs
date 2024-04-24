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

    public class StoreBasketHandler
        (IBasketRepository repository)
        : ICommandHandler<StoreBaskCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBaskCommand command, CancellationToken cancellationToken)
        {
            var cart = await repository.StoreBasket(command.Cart, cancellationToken);

            return new StoreBasketResult(command.Cart.UserName);
        }
    }
}
