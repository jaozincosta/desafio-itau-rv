using System;
using System.Linq;
using InvestimentosRendaVariavel.DbContexto; 

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Testando conexão com o banco...");

        try
        {
            using var context = new InvestimentoContext();
            var usuarios = context.Usuarios.ToList();

            Console.WriteLine($"Conexão OK. {usuarios.Count} usuários encontrados.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao conectar no banco:");
            Console.WriteLine(ex.Message);
        }
    }
}
