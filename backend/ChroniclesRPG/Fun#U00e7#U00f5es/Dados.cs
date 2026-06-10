namespace ChroniclesRPG.Funcoes
{
    public static class Dados
    {
        private static readonly Random Gerador = new();

        public static int Rolar(string notacao)
        {
            if (string.IsNullOrWhiteSpace(notacao))
            {
                return 0;
            }

            try
            {
                string[] partes = notacao.ToLowerInvariant().Split('d');
                int quantidade = int.Parse(partes[0]);
                int faces = int.Parse(partes[1]);

                int total = 0;
                for (int i = 0; i < quantidade; i++)
                {
                    total += Gerador.Next(1, faces + 1);
                }

                return total;
            }
            catch (Exception)
            {
                Console.WriteLine($"Erro ao rolar o dado: {notacao}. Use formatos como 1d8 ou 2d6.");
                return 0;
            }
        }

        public static int RolarD20()
        {
            return Gerador.Next(1, 21);
        }
    }
}
