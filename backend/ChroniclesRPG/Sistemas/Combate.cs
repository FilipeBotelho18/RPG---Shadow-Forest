using ChroniclesRPG.Entidades;
using ChroniclesRPG.Funcoes;

namespace ChroniclesRPG.Sistemas
{
    public static class Combate
    {
        public static int AtacarInimigo(FichaPersonagem jogador, Inimigo inimigo, bool usarSmite)
        {
            if (jogador.ArmaEquipada == null)
            {
                Console.WriteLine("Voce precisa equipar uma arma antes de atacar.");
                return 0;
            }

            int rolagem = Dados.RolarD20();
            int bonusAtaque = jogador.ModificadorForca + BonusProficiencia(jogador.Nivel);
            int totalAtaque = rolagem + bonusAtaque;

            Console.WriteLine($"{jogador.Nome} ataca {inimigo.Nome}: d20({rolagem}) + {bonusAtaque} = {totalAtaque} vs CA {inimigo.ClasseArmadura}");

            if (rolagem == 1)
            {
                Console.WriteLine("Erro critico.");
                return 0;
            }

            if (rolagem != 20 && totalAtaque < inimigo.ClasseArmadura)
            {
                Console.WriteLine("O ataque errou.");
                return 0;
            }

            int dano = Dados.Rolar(jogador.ArmaEquipada.DadoDeDano) + jogador.ModificadorForca;
            if (rolagem == 20)
            {
                dano += Dados.Rolar(jogador.ArmaEquipada.DadoDeDano);
                Console.WriteLine("Acerto critico!");
            }

            if (usarSmite && GastarSlot(jogador, 1))
            {
                int danoSmite = Dados.Rolar("2d8");
                dano += danoSmite;
                Console.WriteLine($"Destruicao Divina adicionou {danoSmite} de dano radiante.");
            }

            dano = Math.Max(1, dano);
            inimigo.HpAtual = Math.Max(0, inimigo.HpAtual - dano);
            Console.WriteLine($"{inimigo.Nome} sofreu {dano} de dano. HP: {inimigo.HpAtual}/{inimigo.HpMaximo}");
            return dano;
        }

        public static int AtacarJogador(Inimigo inimigo, FichaPersonagem jogador)
        {
            int rolagem = Dados.RolarD20();
            int totalAtaque = rolagem + inimigo.BonusAtaque;

            Console.WriteLine($"{inimigo.Nome} ataca: d20({rolagem}) + {inimigo.BonusAtaque} = {totalAtaque} vs CA {jogador.ClasseArmadura}");

            if (rolagem == 1 || (rolagem != 20 && totalAtaque < jogador.ClasseArmadura))
            {
                Console.WriteLine($"{inimigo.Nome} errou.");
                return 0;
            }

            int dano = Dados.Rolar(inimigo.DadoDeDano) + inimigo.BonusDano;
            if (rolagem == 20)
            {
                dano += Dados.Rolar(inimigo.DadoDeDano);
                Console.WriteLine("O inimigo conseguiu um critico.");
            }

            dano = Math.Max(1, dano);
            jogador.HpAtual = Math.Max(0, jogador.HpAtual - dano);
            Console.WriteLine($"{jogador.Nome} sofreu {dano} de dano. HP: {jogador.HpAtual}/{jogador.HpMaximo}");
            return dano;
        }

        public static int CurarPelasMaos(FichaPersonagem jogador, int quantidade)
        {
            if (jogador.CuraPelasMaosRestante <= 0)
            {
                Console.WriteLine("A reserva de Cura pelas Maos acabou.");
                return 0;
            }

            int cura = Math.Min(quantidade, jogador.CuraPelasMaosRestante);
            cura = Math.Min(cura, jogador.HpMaximo - jogador.HpAtual);

            jogador.CuraPelasMaosRestante -= cura;
            jogador.HpAtual += cura;
            Console.WriteLine($"{jogador.Nome} recuperou {cura} HP. Reserva restante: {jogador.CuraPelasMaosRestante}");
            return cura;
        }

        private static bool GastarSlot(FichaPersonagem jogador, int nivelSlot)
        {
            if (!jogador.SlotsDeMagia.TryGetValue(nivelSlot, out int quantidade) || quantidade <= 0)
            {
                Console.WriteLine("Sem slots de magia para usar Smite.");
                return false;
            }

            jogador.SlotsDeMagia[nivelSlot] = quantidade - 1;
            return true;
        }

        private static int BonusProficiencia(int nivel)
        {
            return nivel >= 5 ? 3 : 2;
        }
    }
}
