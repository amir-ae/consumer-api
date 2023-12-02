using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Consumer.API.Contract.V1
{
    public class Routes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = $"/{Root}/{Version}";
        
        public abstract class BaseRoute
        {
            [SetsRequiredMembers]
            public BaseRoute(string prefix, string pattern, string name, string description)
            {
                Name = name;
                Description = description;
                Pattern = pattern;
                RelativeUri = $"{prefix}/{pattern}";
            }
            
            public required string Name { get; init; }
            public required string Description { get; init; }
            public required string Pattern { get; init; }
            public required string RelativeUri { get; init; }
            public string Uri(string? id = null)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return Regex.Replace(RelativeUri, "{.*?}", id);
                }
                    
                return RelativeUri;
            }
            public string Uri(Guid id)
            {
                return Regex.Replace(RelativeUri, "{.*?}", id.ToString());
            }
        }
        
        public static class Customers
        {
            public const string Prefix = $"{Base}/customers";
            
            public class Route : BaseRoute
            {
                [SetsRequiredMembers]
                public Route(string pattern, string name, string description) 
                    : base(Prefix, pattern, name, description) { }
            }

            public static readonly Route ByPage =
                new("", "Customers", "Get customers by page");

            public static readonly Route ByPageDetail =
                new("detail", "CustomersDetail", "Get customers detail by page");

            public static readonly Route All = 
                new("all", "AllCustomers", "Get all customers");

            public static readonly Route AllDetail = 
                new("all-detail", "AllCustomersDetail", "Get all customers detail");

            public static readonly Route ById = 
                new("{customerId}", "CustomerById", "Get customer by id");

            public static readonly Route DetailById = 
                new("{customerId}/detail", "CustomerDetailById", "Get customer detail by id");

            public static readonly Route EventsById = 
                new("{customerId}/events", "CustomerEventsById", "Get customer events by id");

            public static readonly Route Post = 
                new("", "PostCustomer", "Create a new customer");

            public static readonly Route Patch = 
                new("{customerId}", "PatchCustomer", "Update customer by id");

            public static readonly Route Activate =
                new("{customerId}/activate", "ActivateCustomer", "Activate customer by id");

            public static readonly Route Deactivate =
                new("{customerId}/deactivate", "DeactivateCustomer", "Deactivate customer by id");

            public static readonly Route Delete = 
                new("{customerId}", "DeleteCustomer", "Delete customer by id");
        }

        public static class Products
        {
            public const string Prefix = $"{Base}/products";
            
            public class Route : BaseRoute
            {
                [SetsRequiredMembers]
                public Route(string pattern, string name, string description) 
                    : base(Prefix, pattern, name, description) { }
            }

            public static readonly Route ByPage =
                new("", "Products", "Get products by page");

            public static readonly Route ByPageDetail =
               new("detail", "ProductsDetail", "Get products detail by page");

            public static readonly Route All = 
                new("all", "AllProducts", "Get all products");

            public static readonly Route AllDetail = 
                new("all-detail", "AllProductsDetail", "Get all products detail");

            public static readonly Route ById = 
                new("{productId}", "ProductById", "Get product by id");

            public static readonly Route DetailById = 
                new("{productId}/detail", "ProductDetailById", "Get product detail by id");

            public static readonly Route EventsById = 
                new("{productId}/events", "ProductEventsById", "Get product events by id");
            
            public static readonly Route Check = 
                new("check/{productId}", "CheckProduct", "check product by id");

            public static readonly Route DetailByCentreId = 
                new("{centreId:guid}/detail", "ProductsDetailByCentreId", "Get products detail by centre id");

            public static readonly Route DetailByOrderId = 
                new("order/{orderId}", "ProductsDetailByOrderId", "Get product detail by order id");

            public static readonly Route Post = 
                new("", "PostProduct", "Create a new product");

            public static readonly Route Patch = 
                new("{productId}", "PatchProduct", "Update product by id");

            public static readonly Route Activate =
                new("{productId}/activate", "ActivateProduct", "Activate product by id");
            
            public static readonly Route Deactivate =
                new("{productId}/deactivate", "DeactivateProduct", "Deactivate product by id");

            public static readonly Route Delete = 
                new("{productId}", "DeleteProduct", "Delete product by id");
        }
    }
}
