document.body.classList.add("intro-active");

const stageEl = document.querySelector("#stage");
const hpEl = document.querySelector("#hp");
const hpBarEl = document.querySelector("#hp-bar");
const xpEl = document.querySelector("#xp");
const levelEl = document.querySelector("#level");
const armorEl = document.querySelector("#armor");
const slotsEl = document.querySelector("#slots");
const layhandsEl = document.querySelector("#layhands");
const potionsEl = document.querySelector("#potions");
const goldEl = document.querySelector("#gold");
const playerNameEl = document.querySelector("#player-name");
const playerPortraitEl = document.querySelector("#player-portrait");
const contentGrid = document.querySelector(".content-grid");
const sheetPanel = document.querySelector("#sheet-panel");
const logPanel = document.querySelector("#log-panel");
const toggleSheetButton = document.querySelector("#toggle-sheet");
const toggleLogButton = document.querySelector("#toggle-log");
const creationPanel = document.querySelector("#creation-panel");
const classCardsEl = document.querySelector("#class-cards");
const choicePanel = document.querySelector("#choice-panel");
const eventPanel = document.querySelector("#event-panel");
const eventTypeEl = document.querySelector("#event-type");
const eventTitleEl = document.querySelector("#event-title");
const eventDescriptionEl = document.querySelector("#event-description");
const shopListEl = document.querySelector("#shop-list");
const restButton = document.querySelector("#rest-button");
const continueButton = document.querySelector("#continue-button");
const combatPanel = document.querySelector("#combat-panel");
const battlefieldEl = document.querySelector("#battlefield");
const pathCardsEl = document.querySelector("#path-cards");
const enemyNameEl = document.querySelector("#enemy-name");
const enemyPlateNameEl = document.querySelector("#enemy-plate-name");
const enemyHpEl = document.querySelector("#enemy-hp");
const enemyArmorEl = document.querySelector("#enemy-armor");
const enemyHpBarEl = document.querySelector("#enemy-hp-bar");
const enemySpriteEl = document.querySelector("#enemy-sprite");
const battlePlayerHpEl = document.querySelector("#battle-player-hp");
const combatPlayerImageEl = document.querySelector("#combat-player-image");
const feedbackEl = document.querySelector("#feedback");
const sheetEl = document.querySelector("#sheet");
const inventoryListEl = document.querySelector("#inventory-list");
const logEl = document.querySelector("#log");
const finalPanel = document.querySelector("#final-panel");
const finalTitleEl = document.querySelector("#final-title");
const finalResultEl = document.querySelector("#final-result");
const finalLevelEl = document.querySelector("#final-level");
const finalKillsEl = document.querySelector("#final-kills");
const finalGoldEl = document.querySelector("#final-gold");
const finalPathsEl = document.querySelector("#final-paths");

const saveKey = "chronicles-shadow-woods-state";

const panelState = {
  sheet: false,
  log: false
};

const classImages = {
  Guerreiro: "assets/guerreiro.png",
  Mago: "assets/mago.png",
  Paladino: "assets/paladino.png"
};

const enemySpriteClasses = {
  "Twig Blight": "twig",
  "Lobo Corrompido": "wolf",
  "Esqueleto de Aventureiro": "skeleton",
  "Zumbi Putrefato": "zombie",
  "Orc da Garra de Sangue": "orc",
  "Bugbear Urso-Coruja Menor": "ogre",
  "Ogro Esmagador": "ogre",
  "Arvore Desperta Maligna": "tree",
  "Manticora Faminta": "manticore",
  "Lich Primordial Enfraquecido": "lich awakened"
};

const enemyImages = {
  "Twig Blight": "assets/enemies/twig_blight.png",
  "Esqueleto de Aventureiro": "assets/enemies/esqueleto_aventureiro.png",
  "Lobo Corrompido": "assets/enemies/lobo_corrompido.png",
  "Zumbi Putrefato": "assets/enemies/zumbi_putrefato.png",
  "Orc da Garra de Sangue": "assets/enemies/orc_sangue.png",
  "Bugbear Urso-Coruja Menor": "assets/enemies/bugbear_menor.png",
  "Ogro Esmagador": "assets/enemies/ogro_esmagador.png",
  "Arvore Desperta Maligna": "assets/enemies/arvore_maligna.png",
  "Manticora Faminta": "assets/enemies/manticora_faminta.png",
  "Lich Primordial Enfraquecido": "assets/enemies/lich_desperto.png"
};

const icons = {
    atacar: "⚔",
    golpe: "🪓",
    folego: "💨",
    surto: "🔥",
    rajada: "✨",
    raio_gelo: "❄",
    raio_fogo: "🔥",
    protecao_laminas: "🛡",
    protecao_arcana: "🔷",
    misil: "☄",
    paralisar: "🌀",
    smite: "✨",
    cura: "💛",
    bencao: "🙏",
    escudo_fe: "🛡",
    trovejante: "⚡",
    ajuda: "🤝",
    restauracao: "🌿",
    arma_magica: "🔱",
    pocao: "🧪",
    foco: "🎯",
    encerrar: "⏭"
};

async function postJson(url, body = {}) {
  const response = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body)
  });

  return response.json();
}

async function loadGame() {
  // Não inicia automaticamente.
  // A tela inicial aparece primeiro.
  // O jogo só começa quando clicar em "NOVA AVENTURA".
}

function render(state) {
  localStorage.setItem(saveKey, JSON.stringify(state));

  renderPlayer(state.jogador);
  renderStage(state);
  renderClasses(state);
  renderPaths(state.caminhos || []);
  renderEvent(state);
  renderCombat(state);
  renderScene(state);
  renderSheet(state.jogador);
  renderFinal(state);
  renderLog(state.log || []);
  renderPanels();
}

function renderPanels() {
  const hasSidePanel = panelState.sheet || panelState.log;

  contentGrid.classList.toggle("has-side-panel", hasSidePanel);
  sheetPanel.classList.toggle("hidden", !panelState.sheet);
  logPanel.classList.toggle("hidden", !panelState.log);
  toggleSheetButton.classList.toggle("active", panelState.sheet);
  toggleLogButton.classList.toggle("active", panelState.log);
}

function renderScene(state) {
  const etapa = state.etapa?.numero || state.etapaAtual || 1;
  const forestPhase = etapa >= 15 ? "forest-cursed" : etapa >= 8 ? "forest-dark" : "forest-day";

  battlefieldEl.className = `battlefield scene-${state.cenario || "bosque"} ${forestPhase}`;
}

function renderPlayer(jogador) {
  const hpPercent = Math.max(0, Math.min(100, (jogador.hpAtual / jogador.hpMaximo) * 100));
  const image = classImages[jogador.classe] || classImages.Paladino;

  playerNameEl.textContent = jogador.nome;
  playerPortraitEl.style.backgroundImage = `linear-gradient(180deg, rgba(0,0,0,0), rgba(0,0,0,.35)), url("${image}")`;
  combatPlayerImageEl.src = image;
  hpEl.textContent = `HP ${jogador.hpAtual}/${jogador.hpMaximo}`;
  battlePlayerHpEl.textContent = `HP ${jogador.hpAtual}/${jogador.hpMaximo}`;
  hpBarEl.style.width = `${hpPercent}%`;
  xpEl.textContent = `XP ${jogador.xp}`;
  levelEl.textContent = `Nivel ${jogador.nivel}`;
  armorEl.textContent = jogador.classeArmadura;
  slotsEl.textContent = jogador.slotsNivel1 + jogador.slotsNivel2 + jogador.slotsNivel3;
  layhandsEl.textContent = jogador.classe === "Paladino" ? jogador.curaPelasMaos : "-";
  potionsEl.textContent = jogador.pocoes;
  goldEl.textContent = jogador.ouro;
}

function renderStage(state) {
  if (state.fase === "criacao") {
    stageEl.textContent = "Escolha quem vai atender ao chamado da Floresta de Letes.";
    return;
  }

  if (state.fase === "fim") {
    stageEl.textContent = state.vitoria
      ? "Vitoria: a floresta foi purificada."
      : "Derrota: o bosque ainda domina o caminho.";

    return;
  }

  stageEl.textContent = `Etapa ${state.etapa.numero}/${state.totalEtapas}: ${state.etapa.nome}`;
}

function renderClasses(state) {
  creationPanel.classList.toggle("hidden", state.fase !== "criacao");

  if (state.fase !== "criacao") {
    return;
  }

  classCardsEl.replaceChildren(...state.classes.map((classe) => {
    const card = document.createElement("button");

    card.className = "class-card";
    card.type = "button";

    card.innerHTML = `
      <img src="${classe.imagem}" alt="">
      <strong>${classe.nome}</strong>
      <span>${classe.arquetipo}</span>
      <p>${classe.descricao}</p>
    `;

    card.addEventListener("click", async () => {
      render(await postJson("/api/jogo/personagem", { classe: classe.id }));
    });

    return card;
  }));
}

function renderPaths(caminhos) {
  pathCardsEl.replaceChildren(...caminhos.map((caminho) => {
    const card = document.createElement("button");

    card.className = `path-card ${caminho.boss ? "boss-card" : ""}`;
    card.type = "button";
    card.dataset.path = caminho.id;

    card.innerHTML = `
      <span class="path-meta">${caminho.natureza}</span>
      <strong>${caminho.titulo}</strong>
      <p>${caminho.descricao}</p>
      <div class="path-tags">
        <span>${caminho.risco}</span>
        <span>${caminho.recompensa}</span>
      </div>
    `;

    card.addEventListener("click", async () => {
      render(await postJson("/api/jogo/caminho", { caminho: caminho.id }));
    });

    return card;
  }));
}

function renderEvent(state) {
  const show = ["cutscene", "descanso", "loja"].includes(state.fase);

  eventPanel.classList.toggle("hidden", !show);
  choicePanel.classList.toggle("hidden", state.fase !== "caminho");

  if (!show || !state.evento) {
    return;
  }

  eventTypeEl.textContent = state.evento.tipo;
  eventTitleEl.textContent = state.evento.titulo;
  eventDescriptionEl.textContent = state.evento.descricao;

  restButton.classList.toggle("hidden", state.fase !== "descanso");
  continueButton.classList.toggle("hidden", state.fase === "descanso");
  shopListEl.classList.toggle("hidden", state.fase !== "loja");

  if (state.fase === "loja") {
    shopListEl.replaceChildren(...state.loja.map((item) => {
      const row = document.createElement("article");

      row.className = "shop-item";

      row.innerHTML = `
        <div>
          <strong>${item.nome}</strong>
          <p>${item.descricao}</p>
        </div>
        <button type="button">${item.preco} ouro</button>
      `;

      row.querySelector("button").disabled = state.jogador.ouro < item.preco;

      row.querySelector("button").addEventListener("click", async () => {
        render(await postJson("/api/jogo/evento", { acao: "comprar", item: item.id }));
      });

      return row;
    }));
  } else {
    shopListEl.replaceChildren();
  }
}

function renderCombat(state) {
  const emCombate = state.fase === "combate" && state.inimigo;

  combatPanel.classList.toggle("hidden", !emCombate);
  finalPanel.classList.toggle("hidden", state.fase !== "fim");

  if (!emCombate) {
    return;
  }

  const inimigo = state.inimigo;
  const enemyPercent = Math.max(0, Math.min(100, (inimigo.hpAtual / inimigo.hpMaximo) * 100));

  enemyNameEl.textContent = inimigo.boss ? `BOSS: ${inimigo.nome}` : inimigo.nome;
  enemyPlateNameEl.textContent = inimigo.nome;
  enemyHpEl.textContent = `HP ${inimigo.hpAtual}/${inimigo.hpMaximo}`;
  enemyArmorEl.textContent = `CA ${inimigo.classeArmadura}`;
  enemyHpBarEl.style.width = `${enemyPercent}%`;

  const spriteClass = enemySpriteClasses[inimigo.nome] || "twig";
  const image = enemyImages[inimigo.nome] || enemyImages["Twig Blight"];

  enemySpriteEl.className = `sprite enemy-sprite image-enemy ${spriteClass}`;
  enemySpriteEl.innerHTML = `<img src="${image}" alt="">`;

  const fbText = state.feedback?.texto || "Pronto";

  feedbackEl.textContent = (inimigo && fbText === inimigo.nome) ? "" : fbText;
  feedbackEl.className = `feedback ${state.feedback?.tipo || "neutral"}`;

  updateActionButtons(state);
}

function updateActionButtons(state) {
  const classe = state.jogador.classe;
  const mainUsed = state.turno.acaoPrincipalUsada;
  const bonusUsed = state.turno.acaoBonusUsada;

  document.querySelectorAll("[data-action]").forEach((button) => {
    const action = button.dataset.action;

    const isPaladin = [
      "atacar",
      "smite",
      "bencao",
      "escudo_fe",
      "trovejante",
      "ajuda",
      "restauracao",
      "arma_magica",
      "cura",
      "pocao",
      "foco",
      "encerrar"
    ].includes(action);

    const isWarrior = [
      "atacar",
      "golpe",
      "folego",
      "surto",
      "pocao",
      "foco",
      "encerrar"
    ].includes(action);

    const isMage = [
      "rajada",
      "raio_gelo",
      "raio_fogo",
      "protecao_laminas",
      "protecao_arcana",
      "misil",
      "paralisar",
      "pocao",
      "foco",
      "encerrar"
    ].includes(action);

    const visible = classe === "Paladino"
      ? isPaladin
      : classe === "Guerreiro"
        ? isWarrior
        : isMage;

    button.classList.toggle("hidden", !visible);

    const bonus = [
      "cura",
      "pocao",
      "folego",
      "foco",
      "surto",
      "escudo_fe",
      "trovejante",
      "arma_magica"
    ].includes(action);

    let disabled = bonus ? bonusUsed : mainUsed;

    if (action === "encerrar") disabled = !mainUsed;

    if (["smite", "bencao", "escudo_fe", "trovejante"].includes(action)) {
      disabled = disabled || state.jogador.slotsNivel1 <= 0;
    }

    if (["ajuda", "restauracao", "arma_magica"].includes(action)) {
      disabled = disabled || state.jogador.slotsNivel2 <= 0;
    }

    if (action === "misil") {
      disabled = disabled || state.jogador.slotsNivel1 <= 0;
    }

    if (action === "paralisar") {
      disabled = disabled || (state.jogador.slotsNivel1 + state.jogador.slotsNivel2 <= 0);
    }

    if (action === "cura") {
      disabled = disabled || state.jogador.curaPelasMaos <= 0 || state.jogador.hpAtual >= state.jogador.hpMaximo;
    }

    if (action === "pocao") {
      disabled = disabled || state.jogador.pocoes <= 0 || state.jogador.hpAtual >= state.jogador.hpMaximo;
    }

    if (action === "folego") {
      disabled = disabled || state.jogador.hpAtual >= state.jogador.hpMaximo;
    }

    if (action === "surto") {
      disabled = disabled || !state.turno.surtoDisponivel;
    }
        const labels = {
      atacar: "Ataque",
      golpe: "Golpe Preciso",
      folego: "Retomar Fôlego",
      surto: "Surto de Ação",
      rajada: "Rajada",
      raio_gelo: "Raio de Gelo",
      raio_fogo: "Raio de Fogo",
      protecao_laminas: "Proteção Lâminas",
      protecao_arcana: "Proteção Arcana",
      misil: "Mísseis Mágicos",
      paralisar: "Paralisar",
      smite: "Smite",
      cura: "Cura",
      bencao: "Bênção",
      escudo_fe: "Escudo da Fé",
      trovejante: "Golpe Trovejante",
      ajuda: "Ajuda",
      restauracao: "Restauração",
      arma_magica: "Arma Mágica",
      pocao: "Poção",
      foco: "Foco",
      encerrar: "Encerrar Turno"
    };

    button.classList.remove("action-paladin", "action-mage", "action-warrior");

    if (classe === "Paladino") {
      button.classList.add("action-paladin");
    }

    if (classe === "Mago") {
      button.classList.add("action-mage");
    }

    if (classe === "Guerreiro") {
      button.classList.add("action-warrior");
    }

    button.textContent = `${icons[action] || "✦"} ${labels[action] || action}`;

    button.disabled = disabled;
  });
}

function renderSheet(jogador) {
  const attrs = jogador.atributos;

  sheetEl.innerHTML = `
    <article><span>Classe</span><strong>${jogador.classe}</strong></article>
    <article><span>Arma</span><strong>${jogador.arma}</strong></article>
    <article><span>Armadura</span><strong>${jogador.armadura}</strong></article>
    <article><span>Slots</span><strong>1:${jogador.slotsNivel1} 2:${jogador.slotsNivel2} 3:${jogador.slotsNivel3}</strong></article>
    <article><span>FOR</span><strong>${attrs.forca}</strong></article>
    <article><span>DES</span><strong>${attrs.destreza}</strong></article>
    <article><span>CON</span><strong>${attrs.constituicao}</strong></article>
    <article><span>INT</span><strong>${attrs.inteligencia}</strong></article>
    <article><span>SAB</span><strong>${attrs.sabedoria}</strong></article>
    <article><span>CAR</span><strong>${attrs.carisma}</strong></article>
  `;

  inventoryListEl.replaceChildren(...jogador.inventario.map((item) => {
    const li = document.createElement("li");

    li.textContent = item;

    return li;
  }));
}

function renderFinal(state) {
  if (state.fase !== "fim" || !state.resumoFinal) {
    return;
  }

  finalTitleEl.textContent = state.vitoria ? "Vitoria" : "Derrota";
  finalResultEl.textContent = state.resumoFinal.resultado;
  finalLevelEl.textContent = state.resumoFinal.nivelFinal;
  finalKillsEl.textContent = state.resumoFinal.inimigosDerrotados;
  finalGoldEl.textContent = state.resumoFinal.ouroFinal;

  finalPathsEl.replaceChildren(...state.resumoFinal.caminhosEscolhidos.map((path) => {
    const item = document.createElement("li");
    item.textContent = path;
    return item;
  }));

  mostrarTelaFinal(state);
}

function mostrarTelaFinal(state) {
  let overlay = document.getElementById("final-overlay");

  if (!overlay) {
    overlay = document.createElement("div");
    overlay.id = "final-overlay";
    document.body.appendChild(overlay);
  }

  const titulo = state.vitoria ? "JORNADA<br>VENCIDA" : "JORNADA<br>PERDIDA";

  const subtitulo = state.vitoria
    ? "A FLORESTA FOI PURIFICADA"
    : "AS SOMBRAS AINDA DOMINAM";

  const texto = state.vitoria
    ? "Você venceu a jornada em Shadow Forest.<br>A Pluma de Prata agora canta sua lenda."
    : "Sua jornada terminou nas sombras.<br>Mas uma nova aventura ainda pode começar.";

  overlay.innerHTML = `
    <div class="final-content">
      <div class="intro-icon">⚔</div>

      <h1>${titulo}</h1>
      <h2>${subtitulo}</h2>

      <p>${texto}</p>

      <div class="final-stats-overlay">
        <article>
          <span>Nível final</span>
          <strong>${state.resumoFinal.nivelFinal}</strong>
        </article>

        <article>
          <span>Inimigos derrotados</span>
          <strong>${state.resumoFinal.inimigosDerrotados}</strong>
        </article>

        <article>
          <span>Ouro final</span>
          <strong>${state.resumoFinal.ouroFinal}</strong>
        </article>
      </div>

      <button id="restart-adventure" type="button">
        NOVA AVENTURA ▶
      </button>
    </div>
  `;

  overlay.classList.add("show");

  document.getElementById("restart-adventure").addEventListener("click", async () => {
    localStorage.removeItem(saveKey);
    overlay.remove();

    const novoJogo = await postJson("/api/jogo/novo");
    render(novoJogo);
  });
}

function renderLog(log) {
  logEl.replaceChildren(...log.map((entry) => {
    const item = document.createElement("li");

    item.textContent = entry;

    return item;
  }));
}

document.querySelector("#new-game").addEventListener("click", async () => {
  localStorage.removeItem(saveKey);

  render(await postJson("/api/jogo/novo"));
});

toggleSheetButton.addEventListener("click", () => {
  panelState.sheet = !panelState.sheet;

  renderPanels();
});

toggleLogButton.addEventListener("click", () => {
  panelState.log = !panelState.log;

  renderPanels();
});

document.querySelectorAll("[data-close-panel]").forEach((button) => {
  button.addEventListener("click", () => {
    panelState[button.dataset.closePanel] = false;

    renderPanels();
  });
});

document.querySelectorAll("[data-action]").forEach((button) => {
  button.addEventListener("click", async () => {
    render(await postJson("/api/jogo/acao", { acao: button.dataset.action }));
  });
});

restButton.addEventListener("click", async () => {
  render(await postJson("/api/jogo/evento", { acao: "descansar" }));
});

continueButton.addEventListener("click", async () => {
  render(await postJson("/api/jogo/evento", { acao: "continuar" }));
});

document.addEventListener("DOMContentLoaded", () => {
  const intro = document.getElementById("intro-screen");
  const startButton = document.getElementById("start-adventure");

  if (intro && startButton) {
    startButton.addEventListener("click", async () => {
      localStorage.removeItem(saveKey);

      const novoJogo = await postJson("/api/jogo/novo");

      intro.style.opacity = "0";
      intro.style.transition = "opacity 0.8s ease";

      setTimeout(() => {
        intro.remove();
        document.body.classList.remove("intro-active");
        render(novoJogo);
      }, 800);
    });
  }
});

loadGame();