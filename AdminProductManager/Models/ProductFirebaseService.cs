using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using AdminProductManager.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;


namespace AdminProductManager.Models
{
    public class ProductFirebaseService
    {
        private readonly IFirebaseClient _client;
        private readonly string _productsPath = "products";

        public ProductFirebaseService(IConfiguration configuration)
        {
            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = configuration["Firebase:AuthSecret"],
                BasePath = configuration["Firebase:BasePath"]
            };
            _client = new FireSharp.FirebaseClient(config);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var response = await _client.GetAsync(_productsPath);
            var data = response.ResultAs<Dictionary<string, Product>>();
            return data != null ? new List<Product>(data.Values) : new List<Product>();
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync($"{_productsPath}/{id}");
            return response.ResultAs<Product>();
        }

        public async Task CreateAsync(Product product)
        {
            try
            {
                var push = await _client.PushAsync(_productsPath, product);
                if (push == null || push.Result == null || string.IsNullOrEmpty(push.Result.name))
                    throw new Exception("Không thể tạo sản phẩm mới trên Firebase (push thất bại).");

                product.Id = push.Result.name;
                var set = await _client.SetAsync($"{_productsPath}/{product.Id}", product);
                if (set == null)
                    throw new Exception("Không thể lưu sản phẩm với Id mới trên Firebase (set thất bại).");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Firebase CreateAsync error: " + ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            await _client.SetAsync($"{_productsPath}/{product.Id}", product);
        }

        public async Task DeleteAsync(string id)
        {
            await _client.DeleteAsync($"{_productsPath}/{id}");
        }
    }
} 