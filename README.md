# Shadow Forest

RPG 2D web local em ASP.NET inspirado em AdventureQuest Worlds, com combate em turnos e regras simplificadas baseadas no SRD da 5ª Edição de D&D.

O jogo acompanha aventureiros da guilda Pluma de Prata em uma campanha linear de 20 etapas pela Floresta de Letes e pelo Bosque dos Lamentos.

## Como Rodar

Entre na pasta do projeto:

```bash
cd backend/ChroniclesRPG
```

Inicie o servidor local:

```bash
dotnet run
```

Após a inicialização, o terminal exibirá o endereço em que o jogo está disponível, como por exemplo:

```text
Now listening on: http://localhost:5000
```

Abra esse endereço no navegador.

Para encerrar o servidor, pressione `Ctrl + C`.

Se alterar HTML, CSS ou JS e a tela não atualizar, use `Ctrl + F5` para atualizar o cache.

## Onde Mexer

| Arquivo | Descrição |
|---------|-----------|
| `backend/ChroniclesRPG/wwwroot/index.html` | Estrutura da tela e interface |
| `backend/ChroniclesRPG/wwwroot/styles.css` | Tamanho da batalha, HUD, bordas e visual |
| `backend/ChroniclesRPG/wwwroot/app.js` | Lógica de jogo, novo jogo, painéis e renderização |
| `backend/ChroniclesRPG/Dados/inimigos.json` | Balanceamento e configuração dos inimigos |
| `backend/ChroniclesRPG/Dados/jornada.json` | Etapas, recompensas, cutscenes, lojas e descansos |

## Estrutura do Projeto

```
.
├── backend/
│   └── ChroniclesRPG/              # Projeto principal em C#
│       ├── wwwroot/                # Arquivos web (HTML, CSS, JS)
│       ├── Entidades/              # Classes de dados do jogo
│       │   ├── Classes/            # Definições de classes (Guerreiro, Mago, Paladino)
│       │   ├── Habilidades/        # Habilidades de combate
│       │   └── Itens/              # Armas, armaduras e consumíveis
│       ├── Sistemas/               # Lógica de jogo
│       ├── Dados/                  # Arquivos JSON de configuração
│       └── Program.cs              # Ponto de entrada
├── Documentação/                   # Documentação das classes
└── README.md                       # Este arquivo
```

## Alterações Recentes

- Nome do jogo atualizado para `Shadow Forest`
- Batalha ampliada para ocupar melhor a tela quando Ficha/Registro estão fechados
- Ficha e Registro continuam como painéis opcionais, abertos pelos botões do topo
- HUD, arena, sprites, placas de HP e botões de combate aumentados
- Layout usa uma coluna ampla por padrão e só cria coluna lateral quando Ficha/Registro estão abertos
