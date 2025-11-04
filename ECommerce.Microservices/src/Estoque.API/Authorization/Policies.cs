namespace Estoque.API.Authorization
{
    public static class Policies
    {
        public const string RequireAdmin = "RequireAdmin";      // Para operações de gestão (estoque, produtos)
        public const string RequireCustomer = "RequireCustomer"; // Para operações de cliente (consultas)
    }
}