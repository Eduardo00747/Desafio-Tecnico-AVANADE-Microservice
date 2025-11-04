namespace Vendas.API.Authorization
{
    public static class Policies
    {
        public const string RequireAdmin = "RequireAdmin";      // Para operações administrativas
        public const string RequireCustomer = "RequireCustomer"; // Para operações de cliente
    }
}