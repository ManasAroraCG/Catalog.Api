using Catalog.Api.Models;

namespace Catalog.Api.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<int, Product> _products = new();

    private int _nextId = 1;

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.Values.AsEnumerable());
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        _products.TryGetValue(id, out var product);

        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(Product product)
    {
        var newProduct = product with { Id = _nextId++ };

        _products.Add(newProduct.Id, newProduct);

        return Task.FromResult(newProduct);
    }

    public Task<bool> UpdateAsync(Product product)
    {
        if (!_products.ContainsKey(product.Id))
            return Task.FromResult(false);

        _products[product.Id] = product;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var deleted = _products.Remove(id);

        return Task.FromResult(deleted);
    }
}