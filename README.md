# AR Maintenance Guide

> **Projeto AR Academy — UPX: Realidade Aumentada**  
> Repositório sobre o modelo AR accademy para B'Energy desenvolvido pelo Grupo Overlay para capacitação e aprendizado na manutenção da placa PCB usando Realidade Aumentada

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![Plataforma](https://img.shields.io/badge/plataforma-Android-green)
![Tecnologia](https://img.shields.io/badge/RA-AR%20Foundation-blue)
![Unity](https://img.shields.io/badge/Unity-6000.x-black)

---

## 1. Identificação do Projeto

| Item | Informação |
|---|---|
| **Projeto** | B'Energy AR accademy |
| **Equipe** | Overlay |
| **Curso** | Engenharia da computação |
| **Disciplina** | UPX — Realidade Aumentada |
| **Professor** | Daniel Ohata |
| **Semestre** | 2026.2 |

### Integrantes

| Nome | RA | Responsabilidade |
|---|---|---|
| Felipe Roberto| 226752 | Desenvolvimento, Planejamento|
| Felipe Veronezzi | 252628 | Documentação, Planejamento |
| Gabriel Verlangieri | 222709 | Desenvolvimento,  X|
| André Vinicius | 236811 | Testes|
| Pedro Henrique | 236519 | Documentação, Planejamento |
| Gabriel Queiroz | 212065 | Testes |
| Matheus Rodrigues | 235959 | Documentação |
| Luis Felipe | 222287 | Desenvolvimento |

---

## 2. Visão Geral

O **B’Energy AR Academy** é um projeto acadêmico de aplicação móvel desenvolvido em Unity que utiliza Realidade Aumentada (RA) e QR Codes para dar suporte ao treinamento de trainees da equipe B’Energy Racing (FACENS) no desenvolvimento de veículos elétricos para a competição Fórmula SAE.

A proposta consiste em utilizar a câmera de um dispositivo móvel para reconhecer marcadores (QR Codes) posicionados junto aos componentes físicos do veículo e das placas eletrônicas (PCB/PCA). 

Após o reconhecimento, a aplicação apresenta informações técnicas sobre as funções dos componentes, orientações de segurança, questionários de fixação e um roteiro guiado de verificação de placas em quatro etapas (qualidade de soldagem, continuidade/curto-circuito, energização Bring UP e testes de integração).

Este repositório reúne a estrutura do Produto Mínimo Viável (MVP) do aplicativo, focado inicialmente no setor de Low Voltage & Data Acquisition. 
O projeto visa aproximar a teoria da prática no ambiente de oficina, proporcionando maior autonomia de aprendizado aos novos estudantes e reduzindo a dependência constante de membros veteranos.

![Visão geral do projeto](docs/images/visao_geral.svg)

---

## 3. Problema

O treinamento inicial de trainees e a verificação de sistemas técnicos frequentemente dependem de documentações extensas e da supervisão constante de membros veteranos, o que dificulta a associação direta entre o conteúdo teórico e os componentes físicos do veículo.

O B’Energy AR Academy soluciona esse problema ao utilizar Realidade Aumentada e QR Codes em dispositivos móveis para projetar orientações técnicas, avisos de segurança e roteiros de inspeção de placas eletrônicas (PCB) diretamente sobre as peças físicas.

Dessa forma, a aplicação aproxima a informação técnica do seu ponto de uso de maneira simples, promovendo a autonomia dos estudantes no aprendizado e reduzindo dúvidas operacionais no ambiente de oficina.

---

## 4. Objetivos

### 4.1 Objetivo Geral

Desenvolver uma aplicação de Realidade Aumentada capaz de reconhecer uma referência visual e apresentar informações virtuais relacionadas ao objeto identificado.

### 4.2 Objetivos Técnicos

- Configurar um projeto Unity com suporte a Realidade Aumentada.
- Utilizar AR Foundation como camada de integração.
- Realizar reconhecimento de uma imagem cadastrada.
- Instanciar um objeto 3D associado à imagem reconhecida.
- Permitir interação básica com o conteúdo virtual.
- Executar e testar a aplicação em um dispositivo móvel.
- Documentar instalação, execução, testes e limitações.

---

## 5. Público-Alvo

O B’Energy AR Academy é destinado aos trainees da equipe B’Energy Racing (FACENS), especialmente do setor de Low Voltage & Data Acquisition, que necessitam de orientação prática para inspecionar placas eletrônicas e realizar testes de validação e integração em veículos elétricos.

## 6. Funcionalidades

O aplicativo apresenta o modelo da placa, destaca os pontos de coleta de dados e de observação, exibe orientações e exemplos e solicita o preenchimento de medições e checklists durante o roteiro. RF01–RF07 estão implementados, conforme confirmação de Felipe Roberto em 30/10/2026.

| ID | Funcionalidade | Descrição | Status |
|---|---|---|---|
| RF01 | Guia de soldagem | Apresenta orientações e exemplos de soldagem, com destaques no modelo da placa para apoiar a observação dos componentes e regiões indicadas. Fonte: [Etapa1_Soldagem.asset](Assets/Etapas/Etapa1_Soldagem.asset). | ✅ Implementado |
| RF02 | Guia de continuidade | Mostra no modelo os pontos de coleta de dados e apresenta orientações, checklist e campos de medição para a etapa de continuidade. Fonte: [Etapa2_Continuidade.asset](Assets/Etapas/Etapa2_Continuidade.asset). | ✅ Implementado |
| RF03 | Guia de energização | Apresenta orientações de Bring UP, pontos de observação e coleta de dados, checklist e entrada de medições. Fonte: [Etapa3_Energizacao.asset](Assets/Etapas/Etapa3_Energizacao.asset). | ✅ Implementado |
| RF04 | Guia de integração | Apresenta orientações e destaques dos componentes e pontos de observação no modelo da placa, acompanhados do checklist da etapa. Fonte: [Etapa4_Integracao.asset](Assets/Etapas/Etapa4_Integracao.asset). | ✅ Implementado |
| RF05 | Navegação sequencial | Permite iniciar, avançar, retornar e reiniciar o roteiro de quatro etapas. Fontes: [InspectionFlow.cs](Assets/Scripts/Roteiro/InspectionFlow.cs) e [StepPanel.cs](Assets/Scripts/UI/StepPanel.cs). | ✅ Implementado |
| RF06 | Checklist de validação | Solicita a marcação dos itens de verificação de cada etapa e indica os registros necessários antes de avançar. Fontes: [InspectionSession.cs](Assets/Scripts/Dados/InspectionSession.cs) e [RecordScreen.cs](Assets/Scripts/UI/RecordScreen.cs). | ✅ Implementado |
| RF07 | Registro de medições | Solicita a entrada de valores, apresenta unidades e avalia as medições conforme a configuração da etapa. Fontes: [RecordScreen.cs](Assets/Scripts/UI/RecordScreen.cs) e [MeasurementSpec.cs](Assets/Scripts/Dados/MeasurementSpec.cs). | ✅ Implementado |

**Legenda:** ✅ Implementado.

O modelo e os destaques por etapa são apresentados por [StepOverlay.prefab](Assets/Prefabs/Overlays/StepOverlay.prefab) e [StepOverlayController.cs](Assets/Scripts/AR/StepOverlayController.cs). Ao concluir o roteiro, o aplicativo apresenta o resultado e gera o relatório local da sessão por meio de [MetricsRecorder.cs](Assets/Scripts/Dados/MetricsRecorder.cs).

---

## 7. Requisitos Não Funcionais

| ID | Categoria | Requisito |
|---|---|---|
| RNF01 | Usabilidade | A interface deve ser intuitiva e dispensar treinamentos extensos. |
| RNF02 | Ergonomia | Deve permitir o uso em suportes de bancada, mantendo as mãos livres para a inspeção. |
| RNF03 | Clareza visual | As indicações devem apresentar contraste e legibilidade adequados à iluminação da bancada. |
| RNF04 | Desempenho | O reconhecimento de QR Codes e marcadores deve ocorrer rapidamente. |
| RNF05 | Rastreamento | As indicações de RA devem permanecer alinhadas aos pontos correspondentes da PCB. |
| RNF06 | Compatibilidade | A aplicação deve funcionar em dispositivos móveis compatíveis com a tecnologia de RA utilizada. |
| RNF07 | Processamento | O MVP deve priorizar textos, imagens e elementos visuais leves, evitando modelos 3D pesados. |
| RNF08 | Manutenibilidade | A estrutura deve facilitar a inclusão de novas rotinas e componentes. |
| RNF09 | Avaliação | O sistema deve permitir registrar métricas de uso, etapas concluídas e erros durante o treinamento. |

## 8. Tecnologias Utilizadas

| Tecnologia | Versão | Utilização |
|---|---|---|
| Unity | 6000.3.25f1 | Desenvolvimento principal |
| C# | Compatível com Unity 6000.3.25f1 | Scripts da aplicação |
| AR Foundation | 6.3.5 | Camada de abstração de Realidade Aumentada |
| ARCore XR Plugin | 6.3.5 | Recursos de Realidade Aumentada em Android |
| ARKit XR Plugin | 6.3.5 | Recursos de Realidade Aumentada em iOS |
| XR Plugin Management | 4.6.1 | Gerenciamento de plugins XR |
| Blender | 4.x | Modelagem/edição 3D |
| Git | 2.x | Controle de versão |
| GitHub | — | Hospedagem do projeto e documentação |

---

## 9. Hardware Utilizado

| Equipamento | Modelo | Característica relevante | Utilização |
|---|---|---|---|
| Smartphone | Samsung Galaxy S23 (SM-S911B) | Suporte ao ARCore, Android 10+ (API ≥ 29) | Execução e testes |
| Computador | Notebook Windows 11 Home | Intel Core i5-10300H, 24 GB RAM, NVIDIA GeForce GTX 1650 | Desenvolvimento e build |

---

## 10. Arquitetura do Sistema

O B'Energy AR Academy evoluiu da base WL01 de Image Tracking para um roteiro de inspeção com modelo da placa VCU, destaques por etapa, orientações e exemplos, checklist, entrada de medições e resultado da sessão. A arquitetura conecta reconhecimento de imagem, controle do roteiro, apresentação e registro de dados.

```mermaid
flowchart TB
    CAMERA["Câmera / ARCore"] --> TRACK["AR Foundation<br/>ARTrackedImageManager"]
    LIB["XR Reference Image Library"] --> TRACK
    TRACK --> REC["ImageRecognitionController"]
    REC -->|ContentCreated| MODEL["StepOverlay.prefab<br/>Modelo VCU + marcações"]
    REC -->|MarkerDetected| FLOW["InspectionFlow"]
    HOME["HomeScreen<br/>Iniciar RA"] -->|RequestStart| FLOW
    STEPS["InspectionStep<br/>Assets/Etapas: RF01–RF04"] --> FLOW
    FLOW -->|StepChanged| PANEL["StepPanel<br/>Guia, exemplos e navegação"]
    FLOW -->|StepChanged| OVERLAY["StepOverlayController"]
    OVERLAY -->|StepOverlay / HighlightView| MODEL
    PANEL -->|Registrar| RECORD["RecordScreen<br/>Checklist e medições"]
    FLOW -->|RecordIncomplete| RECORD
    RECORD --> SESSION["InspectionSession<br/>Dados e tempos por etapa"]
    SESSION --> SPEC["MeasurementSpec<br/>Avaliação dos valores"]
    FLOW -->|Verifica registro completo| SESSION
    FLOW -->|Conclusão| METRICS["MetricsRecorder<br/>SessionReport / StepReport"]
    SESSION --> METRICS
    METRICS --> JSON["Arquivo JSON local<br/>persistentDataPath/sessoes"]
    FLOW -->|Completed com relatório| RESULT["ResultPanel<br/>Resumo e opção de reinício"]
    RESULT -->|Restart| FLOW
```

As setas representam eventos, chamadas e uso de dados. As telas exibem o estado e encaminham ações; `InspectionFlow` controla a etapa atual, verifica a completude do registro e coordena a conclusão. `InspectionSession` mantém checklist, valores e tempos em memória; `MeasurementSpec` interpreta e avalia os valores. `MetricsRecorder` monta o relatório e tenta salvá-lo; a tela de resultado informa também se a gravação foi possível.

### Componentes da cena `WL01RecognitionAR`

A cena habilitada em [EditorBuildSettings.asset](ProjectSettings/EditorBuildSettings.asset) é [Assets/Scenes/WL01RecognitionAR.unity](Assets/Scenes/WL01RecognitionAR.unity). A árvore abaixo mostra os principais objetos e componentes; os itens entre colchetes são componentes anexados ao objeto, e os demais representam objetos da hierarquia:

```text
WL01RecognitionAR
├── Directional Light                → iluminação da cena
├── AR Session                       → ciclo de vida da sessão de RA
├── XR Origin                        → origem da experiência
│   ├── [ARTrackedImageManager]       → detecta e acompanha os marcadores
│   ├── [ImageRecognitionController]  → cria e atualiza o conteúdo virtual
│   └── Camera Offset
│       └── Main Camera              → câmera de RA
├── Roteiro
│   ├── [InspectionFlow]              → controla as quatro etapas e a sessão
│   └── [StepOverlayController]       → atualiza os destaques por etapa
├── Canvas Roteiro
│   ├── [HomeScreen]                  → controla o início do roteiro
│   ├── [StepPanel]                   → guia, exemplos, progresso e navegação
│   ├── [RecordScreen]                → checklist e entrada de medições
│   ├── [ResultPanel]                 → resumo e opção de reinício
│   ├── Tela Inicial                 → apresentação e botão Iniciar RA
│   ├── Tela Registro                → formulário de registro da etapa
│   └── Tela Resultado               → apresentação do resultado da sessão
└── EventSystem                      → interações da interface
```

O campo `contentPrefab` de `ImageRecognitionController` referencia `StepOverlay.prefab`; o cubo permanece como fallback quando não há prefab configurado. `InspectionFlow` referencia os quatro assets de `Assets/Etapas` e mantém `waitForStart` ativo, coordenando o botão inicial com o reconhecimento do marcador.

O prefab de conteúdo é instanciado como filho do marcador rastreado. `StepOverlay` cria as marcações a partir dos dados da etapa, e `HighlightView` apresenta seus pontos e rótulos. A visibilidade do conteúdo acompanha o estado de tracking em `ImageRecognitionController`.

### Fluxo resumido

```text
Trainee
   ↓
Interface da Aplicação (tela inicial / Iniciar RA)
   ↓
Câmera / ARCore / AR Foundation (ARTrackedImageManager)
   ↓
Reconhecimento do marcador (ImageRecognitionController)
   ↓
Modelo VCU e destaques dos pontos da etapa
   ↓
Controle do roteiro (InspectionFlow)
   ↓
Guia, orientações e exemplos (StepPanel)
   ↓
Checklist e entrada de medições (RecordScreen)
   ↓
Verificação do registro (InspectionSession / MeasurementSpec)
   ↓
Navegação entre as quatro etapas
   ↓
Conclusão e relatório JSON (MetricsRecorder)
   ↓
Resultado da sessão (ResultPanel)
   ↓
Opção de refazer o roteiro (nova sessão)
```

Ao tentar avançar com o registro incompleto, o fluxo abre a tela de registro. Retornar à etapa anterior preserva os dados da sessão; refazer o roteiro cria uma nova sessão. O resultado utiliza os dados registrados e as regras de avaliação do aplicativo.

Os scripts de `Assets/Scripts/Editor` preparam a cena, os assets e o visual durante o desenvolvimento. Eles utilizam `UnityEditor` e não executam no aplicativo instalado; o aplicativo usa os componentes e assets já configurados.

---

## 11. Funcionamento da Realidade Aumentada

### 11.1 Técnica utilizada

Utilizamos **Image Tracking**: uma imagem cadastrada na **XR Reference Image Library** (`Assets/Markers/WL01ReferenceImageLibrary.asset`) funciona como referência para ativar o conteúdo virtual. Na versão final, o marcador será a imagem da própria placa ou um QR Code fixado nela.

O marcador atual é o `wl01-marker.png`, cadastrado com tamanho físico de **0,12 m × 0,12 m**. Informar o tamanho físico permite que o ARCore estime corretamente a escala e a posição do conteúdo.

### 11.2 Fluxo técnico
```mermaid
flowchart LR
    A["Inicializar sessão de RA<br/>AR Session + câmera"] --> B["Análise das imagens<br/>ARCore"]
    B --> C["Reconhecer marcador<br/>ARTrackedImageManager + Reference Image Library"]
    C --> D["Registrar marcador<br/>ARTrackedImage com posição e orientação"]
    D --> E["Criar conteúdo virtual<br/>ImageRecognitionController"]
    E --> F["Verificar rastreamento<br/>Tracking ativo?"]
    F -->|Sim| G["Exibir conteúdo<br/>Registrado ao marcador"]
    F -->|Não| H["Ocultar conteúdo"]
    G --> I["Interação do trainee<br/>Toque na tela"]
```
1. A aplicação inicia a sessão de RA (`AR Session`) e a câmera do dispositivo.
2. O ARCore analisa continuamente as imagens capturadas.
3. O `ARTrackedImageManager` compara a imagem observada com a Reference Image Library.
4. Quando há correspondência, é criado um `ARTrackedImage` com a posição e a orientação do marcador.
5. O `ImageRecognitionController` cria o conteúdo virtual como filho do `ARTrackedImage`, mantendo-o registrado ao marcador.
6. Enquanto o estado for `Tracking`, o conteúdo fica visível; se o marcador for perdido, o conteúdo é ocultado.
7. O trainee interage com o conteúdo tocando na tela.

### 11.3 Elementos virtuais

| Elemento | Situação atual | Planejado para o MVP |
|---|---|---|
| Modelo da placa | Modelo VCU exibido no [prefab de overlay](Assets/Prefabs/Overlays/StepOverlay.prefab), com indicações associadas às etapas. | Contemplado na versão atual. |
| Texto de orientação e exemplos | Instruções e cartões de referência apresentados para apoiar a execução e a observação em cada etapa. Fontes: [Assets/Etapas](Assets/Etapas) e [StepPanel.cs](Assets/Scripts/UI/StepPanel.cs). | Contemplado na versão atual. |
| Destaque visual | Pontos de coleta de dados, componentes e regiões de observação indicados sobre o modelo da placa. Fonte: [StepOverlayController.cs](Assets/Scripts/AR/StepOverlayController.cs). | Contemplado na versão atual. |
| Botão de ação | Início, avanço, retorno e reinício do roteiro disponíveis nas telas. Fonte: [InspectionFlow.cs](Assets/Scripts/Roteiro/InspectionFlow.cs). | Contemplado na versão atual (RF05). |
| Indicação de status | Etapa atual, checklist, campos de medição e resumo do resultado apresentados pela interface. Fontes: [RecordScreen.cs](Assets/Scripts/UI/RecordScreen.cs) e [ResultPanel.cs](Assets/Scripts/UI/ResultPanel.cs). | Contemplado na versão atual (RF06 e RF07). |

---

## 12. Estrutura do Projeto

Estrutura dos principais arquivos e diretórios do repositório. Arquivos `.meta` e diretórios gerados pelo Unity foram omitidos para facilitar a leitura.

```text
AR_UPX_Project/
├── Assets/
│   ├── Etapas/
│   │   ├── Etapa1_Soldagem.asset
│   │   ├── Etapa2_Continuidade.asset
│   │   ├── Etapa3_Energizacao.asset
│   │   └── Etapa4_Integracao.asset
│   ├── Fonts/
│   ├── Markers/
│   │   ├── wl01-marker.png
│   │   ├── vcu-marker.png
│   │   └── WL01ReferenceImageLibrary.asset
│   ├── Materials/
│   ├── Models/VCU/VCU.fbx
│   ├── Prefabs/Overlays/
│   │   ├── StepOverlay.prefab
│   │   └── DemoContent.prefab
│   ├── Scenes/WL01RecognitionAR.unity
│   ├── Scripts/
│   │   ├── AR/
│   │   │   ├── ImageRecognitionController.cs
│   │   │   ├── StepOverlayController.cs
│   │   │   ├── StepOverlay.cs
│   │   │   └── HighlightView.cs
│   │   ├── Roteiro/
│   │   │   ├── InspectionFlow.cs
│   │   │   └── InspectionStep.cs
│   │   ├── Dados/
│   │   │   ├── InspectionSession.cs
│   │   │   ├── MeasurementSpec.cs
│   │   │   └── MetricsRecorder.cs
│   │   ├── UI/
│   │   │   ├── HomeScreen.cs
│   │   │   ├── StepPanel.cs
│   │   │   ├── RecordScreen.cs
│   │   │   ├── ResultPanel.cs
│   │   │   ├── InfoCardView.cs
│   │   │   ├── ChecklistItemView.cs
│   │   │   ├── MeasurementFieldView.cs
│   │   │   └── StepResultRowView.cs
│   │   ├── Interacao/TapToToggleScale.cs
│   │   └── Editor/
│   │       ├── RoteiroSetup.cs
│   │       ├── OverlaySetup.cs
│   │       ├── RecordSetup.cs
│   │       ├── ModelSetup.cs
│   │       ├── VisualSetup.cs
│   │       └── RecordScreenSetup.cs
│   ├── Shaders/UISempreNaFrente.shader
│   ├── UI/Sprites/
│   └── XR/
├── Packages/
├── ProjectSettings/
├── docs/
│   ├── images/
│   ├── ARQUITETURA.md
│   ├── ATIVIDADE.md
│   └── CUSTOMIZACAO.md
├── .gitignore
└── README.md
```

### Pastas principais

| Pasta | Finalidade |
|---|---|
| `Assets/Etapas` | Assets `InspectionStep` com instruções, exemplos, destaques, checklist e especificações de medição das quatro etapas. |
| `Assets/Markers` | Imagens de referência e biblioteca usada no reconhecimento; a presença de uma imagem na pasta não significa que ela esteja cadastrada como target ativo. |
| `Assets/Models/VCU` | Modelo 3D da placa utilizado pelo prefab de conteúdo. |
| `Assets/Materials` | Materiais do modelo, serigrafia e marcações. |
| `Assets/Prefabs/Overlays` | Prefab `StepOverlay` com modelo e estrutura de destaques; `DemoContent` preservado no repositório. |
| `Assets/Scenes` | Cena de RA e vínculos entre reconhecimento, roteiro e interface. |
| `Assets/Scripts/AR` | Reconhecimento, controle do conteúdo rastreado e apresentação dos destaques. |
| `Assets/Scripts/Roteiro` | Controle da sequência e definição dos dados de cada etapa. |
| `Assets/Scripts/Dados` | Estado da sessão, avaliação de valores, relatórios e persistência local. |
| `Assets/Scripts/UI` | Telas, painéis e componentes visuais de orientação, registro e resultado. |
| `Assets/Scripts/Interacao` | Interação por toque/clique do conteúdo de demonstração. |
| `Assets/Scripts/Editor` | Ferramentas de preparação no Editor; não integram a execução do aplicativo. |
| `Assets/Fonts`, `Assets/UI/Sprites` e `Assets/Shaders` | Fontes, elementos gráficos e shader das marcações. |
| `Assets/XR` | Configurações, loaders e recursos XR. |
| `Packages` e `ProjectSettings` | Dependências e configurações do projeto, incluindo cena de build e plataforma. |
| `docs` | Documentação complementar e imagens do projeto. |

Os documentos [ARQUITETURA.md](docs/ARQUITETURA.md), [ATIVIDADE.md](docs/ATIVIDADE.md) e [CUSTOMIZACAO.md](docs/CUSTOMIZACAO.md) preservam a arquitetura e as orientações da base didática WL01. Referências nesses arquivos à substituição do cubo ou à evolução inicial devem ser lidas nesse contexto histórico. As seções 10–13 deste README descrevem a arquitetura atual do roteiro com a placa VCU.

---

## 13. Principais Scripts

Os componentes do aplicativo usam o namespace `Facens.WL01`. As ferramentas de preparação usam `Facens.WL01.EditorTools` e ficam na pasta `Editor`, com acesso às APIs `UnityEditor`.

| Script | Responsabilidade |
|---|---|
| [ImageRecognitionController.cs](Assets/Scripts/AR/ImageRecognitionController.cs) | Recebe mudanças das imagens rastreadas, instancia o prefab sobre o marcador, controla sua visibilidade e emite MarkerDetected e ContentCreated. Sem prefab, cria o cubo de fallback. |
| [StepOverlayController.cs](Assets/Scripts/AR/StepOverlayController.cs) | Escuta ContentCreated, StepChanged e Finished; aplica os destaques da etapa ao conteúdo criado e os limpa ao concluir o roteiro. |
| [StepOverlay.cs](Assets/Scripts/AR/StepOverlay.cs) | Cria e remove as marcações visuais a partir da lista de Highlight da etapa. |
| [HighlightView.cs](Assets/Scripts/AR/HighlightView.cs) | Posiciona cada ponto e rótulo segundo as coordenadas da etapa e aplica seu estilo visual. |
| [InspectionFlow.cs](Assets/Scripts/Roteiro/InspectionFlow.cs) | Coordena início, etapas, avanço condicionado ao registro completo, retorno e reinício; monta/salva o relatório via MetricsRecorder e publica o resultado. |
| [InspectionStep.cs](Assets/Scripts/Roteiro/InspectionStep.cs) | Define o ScriptableObject da etapa: requisito, instrução, cartões InfoCard, destaques Highlight, checklist e medições. Os quatro assets ficam em Assets/Etapas. |
| [InspectionSession.cs](Assets/Scripts/Dados/InspectionSession.cs) | Mantém checklist, valores e tempos por etapa em memória; verifica registro completo e aprovação. Uma nova execução cria uma nova sessão. |
| [MeasurementSpec.cs](Assets/Scripts/Dados/MeasurementSpec.cs) | Define rótulo, unidade e faixa opcional; interpreta números e classifica valores como vazio, inválido, sem faixa, dentro ou fora. |
| [MetricsRecorder.cs](Assets/Scripts/Dados/MetricsRecorder.cs) | Define os relatórios SessionReport, StepReport e MeasurementReport; consolida os registros e salva JSON em Application.persistentDataPath/sessoes, retornando caminho ou null em falha. |
| [HomeScreen.cs](Assets/Scripts/UI/HomeScreen.cs) | Apresenta a tela inicial e solicita o início do roteiro ao pressionar Iniciar RA. |
| [StepPanel.cs](Assets/Scripts/UI/StepPanel.cs) | Mostra a etapa, progresso, instruções e cartões; alterna o Guia, abre Registrar e encaminha avanço/retorno ao fluxo. |
| [RecordScreen.cs](Assets/Scripts/UI/RecordScreen.cs) | Apresenta checklist e campos da etapa, atualiza a contagem de pendências e encaminha as entradas à sessão; ao concluir o registro, solicita o avanço. |
| [ResultPanel.cs](Assets/Scripts/UI/ResultPanel.cs) | Apresenta duração, erros, situação das etapas e resultado do salvamento; permite refazer o roteiro. |
| [InfoCardView.cs](Assets/Scripts/UI/InfoCardView.cs) | Apresenta título e texto de um cartão de orientação ou exemplo. |
| [ChecklistItemView.cs](Assets/Scripts/UI/ChecklistItemView.cs) | Associa o texto e a marcação de um item à ação de atualização do checklist. |
| [MeasurementFieldView.cs](Assets/Scripts/UI/MeasurementFieldView.cs) | Apresenta rótulo, unidade, entrada e avaliação da medição; comunica alterações do valor ao registro. |
| [StepResultRowView.cs](Assets/Scripts/UI/StepResultRowView.cs) | Apresenta a identificação e o estado de aprovação de uma etapa no resultado. |
| [TapToToggleScale.cs](Assets/Scripts/Interacao/TapToToggleScale.cs) | Detecta toque ou clique via Input System e raycast para alternar a escala do objeto de demonstração. |
| [RoteiroSetup.cs](Assets/Scripts/Editor/RoteiroSetup.cs) | Ferramenta do Editor que cria os assets iniciais das etapas e monta a interface do roteiro na cena aberta. |
| [OverlaySetup.cs](Assets/Scripts/Editor/OverlaySetup.cs) | Ferramenta do Editor que prepara o prefab e os componentes das marcações sobre a placa. |
| [RecordSetup.cs](Assets/Scripts/Editor/RecordSetup.cs) | Ferramenta histórica de montagem do checklist e medições no painel. Na estrutura atual, informa a substituição pelo menu da Fase 9 quando o campo antigo não existe. |
| [ModelSetup.cs](Assets/Scripts/Editor/ModelSetup.cs) | Ferramenta do Editor que insere e configura o modelo VCU no prefab, além da textura/serigrafia e dos materiais das marcações. |
| [VisualSetup.cs](Assets/Scripts/Editor/VisualSetup.cs) | Ferramenta do Editor que aplica o visual, configura elementos da interface, fonte e ajustes do registro. |
| [RecordScreenSetup.cs](Assets/Scripts/Editor/RecordScreenSetup.cs) | Ferramenta do Editor que monta a tela separada de registro, conecta seus campos e remove o quiz do fluxo preparado na cena. |

### Relação entre etapa, modelo, registro e resultado

1. `HomeScreen` solicita o início; `ImageRecognitionController` informa o reconhecimento e cria o conteúdo configurado.
2. `InspectionFlow` seleciona um `InspectionStep`. Os dados vêm do asset correspondente em `Assets/Etapas`.
3. `StepPanel` apresenta a orientação e os exemplos; `StepOverlayController` aplica os pontos dessa etapa ao `StepOverlay` do modelo.
4. `RecordScreen` apresenta os itens e campos; as respostas são mantidas em `InspectionSession` e as medições avaliadas por `MeasurementSpec`.
5. Ao concluir a última etapa, `InspectionFlow` usa `MetricsRecorder` para montar e tentar gravar o relatório; em seguida emite `Completed`, recebido por `ResultPanel`.

Os menus dos scripts `Editor` registram etapas de preparação do projeto. A cena já contém os componentes configurados; esses menus não são passos executados pelo usuário no celular.

---

## 14. Dependências

Dependências declaradas em `Packages/manifest.json`:

```text
com.unity.xr.arfoundation   6.3.5
com.unity.xr.arcore         6.3.5
com.unity.xr.arkit          6.3.5
com.unity.xr.management     4.6.1
```

---

## 15. Configuração do Ambiente

### Pré-requisitos

- Windows 10 ou 11;
- Unity Hub;
- Unity **6000.3.25f1** (Unity 6.3 LTS);
- módulo **Android Build Support** (com OpenJDK e Android SDK & NDK Tools);
- Git;
- smartphone Android compatível com ARCore e com **Android 10 ou superior**.

> O módulo Visual Studio do Unity Hub é opcional. Se o Visual Studio já estiver instalado, basta selecioná-lo em **Edit → Preferences → External Tools**.

### Procedimento

1. Instale o Unity Hub.
2. Instale o Unity 6000.3.25f1 com o módulo Android Build Support.
3. Instale o Git.
4. Clone o repositório em um caminho curto e sem acentos (ex.: `C:\AR\`).
5. Abra o projeto pelo Unity Hub e aguarde a importação dos pacotes.
6. Em **Edit → Project Settings → XR Plug-in Management**, aba Android, confirme que **ARCore** está marcado.
7. Em **Project Settings → Player → Android → Other Settings**, confirme:
   - **Minimum API Level:** Android 10.0 (API 29);
   - **Active Input Handling:** Both.

---

## 16. Como Executar

```bash
git clone https://github.com/lfmutton/AR_UPX_Project.git
```

Depois:

1. Abra o Unity Hub e clique em **Add / Open Project**.
2. Selecione a pasta clonada.
3. Abra a cena `Assets/Scenes/WL01RecognitionAR.unity`.
4. Abra **File → Build Profiles** (Ctrl+Shift+B), selecione **Android** e clique em **Switch Platform** se necessário.
5. Confirme que a cena `WL01RecognitionAR` está na **Scene List**.
6. No celular, ative as **Opções do desenvolvedor** e a **Depuração USB**.
7. Conecte o celular via USB e autorize a depuração.
8. Clique em **Build and Run**.

---

## 17. Como Utilizar a Aplicação

1. Abra a aplicação no dispositivo.
2. Autorize o acesso à câmera.
3. Aponte a câmera para o marcador impresso ou para a placa.
4. Aguarde o reconhecimento.
5. Observe o conteúdo virtual sobre o marcador.
6. Toque no objeto para alternar sua escala.

---

## 18. Fluxo de Funcionamento

![Fluxo Funcionamento](docs/images/fluxo_funcionamento.png)

---

## 19. Caso de Uso

```mermaid
flowchart LR
    T((Trainee)) --> U1[Escanear marcador da placa]
    T --> U2[Visualizar orientações em RA]
    T --> U3[Interagir com o conteúdo]
    T --> U4[Seguir roteiro de inspeção]
    U4 --> U5[Registrar medições]
```

---

## 20. Fluxo de Navegação

```text
Abertura do app
    ↓
Câmera (RA ativa)
    ↓
Reconhecimento do marcador
    ↓
Conteúdo Virtual
    ↓
Interação (toque)
```

No MVP, vamos adicionar uma tela inicial e a navegação entre as quatro etapas do roteiro de inspeção (RF05).

---

## 21. Interface

As imagens do projeto ficam em `docs/images/`:

```text
docs/images/
├── cena-unity.png
├── fluxo_funcionamento.png
├── imagem_teste.jpg
└── visao_geral.svg
```

| Arquivo | Conteúdo | Onde aparece |
|---|---|---|
| `cena-unity.png` | Cena `WL01RecognitionAR` montada no Unity | Seção 25 |
| `fluxo_funcionamento.png` | Fluxo de funcionamento da aplicação | Seção 18 |
| `visao_geral.svg` | Visão geral do projeto | Seção 2 |

---

## 22. Testes Técnicos

| Teste | Procedimento | Resultado esperado | Resultado obtido | Status |
|---|---|---|---|---|
| Build Android | Build and Run no Unity | APK gerado e instalado no celular | APK gerado e instalado no Galaxy S23 | ✅ |
| Inicialização | Abrir aplicativo | Aplicação inicia | Aplicação iniciou normalmente | ✅ |
| Câmera | Abrir aplicativo e permitir câmera | Câmera de RA é ativada | Câmera ativada após a permissão | ✅ |
| Image Tracking | Apontar para o marcador | Marcador é detectado | Marcador detectado | ✅ |
| Conteúdo 3D | Reconhecer o marcador | Cubo aparece sobre o marcador | Cubo exibido sobre o marcador | ✅ |
| Estabilidade | Mover o celular lentamente | Cubo permanece registrado ao marcador | Cubo manteve-se alinhado ao marcador | ✅ |
| Interação | Tocar no cubo | Escala alterna | Escala alternou ao toque | ✅ |

---

## 23. Dispositivos Testados

| Dispositivo | Sistema | Versão | Resultado |
|---|---|---|---|
| Samsung Galaxy S23 (SM-S911B) | Android | Preencher | ✅ Todos os testes aprovados |

---

## 24. Limitações Conhecidas

- O reconhecimento depende da iluminação e da visibilidade do marcador; o aplicativo requer dispositivo compatível com ARCore e Android 10 ou superior.
- O conteúdo de RA está associado ao marcador configurado na [Reference Image Library](Assets/Markers/WL01ReferenceImageLibrary.asset). Modelo da placa e imagem de reconhecimento são recursos distintos.
- O relatório de [MetricsRecorder.cs](Assets/Scripts/Dados/MetricsRecorder.cs) é armazenado localmente; o formato atual não possui campo próprio de participante nem pontuação de quiz. A contagem de erros representa os registros avaliados na sessão e não mede, por si só, aprendizagem.
- O salvamento do relatório depende do acesso ao armazenamento local; o código registra a falha e retorna caminho nulo quando não consegue gravar.
- As evidências das seções 22–25 documentam a demonstração anterior do cubo. A documentação dessas evidências deve acompanhar a evolução para o modelo da placa e o roteiro atual.
- A avaliação científica e a eficácia educacional são tratadas nos cards DOC e de Validação.

---

## 25. Evidências de Funcionamento

- [✅] Captura de tela da cena no Unity

![PrintScreenUnityScene](./docs/images/cena-unity.png)

- [✅] Vídeo de 30 a 60 s mostrando reconhecimento, tracking e toque

[Assistir demonstração](https://facens-my.sharepoint.com/:v:/g/personal/222287_facens_br/IQDhc_5IWhQDRpW3ojZiLr4tAdURzoYrYbk4rNdl7FY4Zwc?e=xo273a&nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJTdHJlYW1XZWJBcHAiLCJyZWZlcnJhbFZpZXciOiJTaGFyZURpYWxvZy1MaW5rIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXcifX0%3D)

---

## 26. Repositórios e Recursos

| Recurso | Link |
|---|---|
| Código-fonte | https://github.com/lfmutton/AR_UPX_Project |
| Figma | https://www.figma.com/design/JEYjQr9279QAKvydvNqFS1/Untitled?node-id=0-1&p=f |
| Trello/Jira | https://trello.com/b/qlnUJc32/upx-realidade-aumentada-overlay |
| Vídeo | [Assistir demonstração](https://facens-my.sharepoint.com/:v:/g/personal/222287_facens_br/IQDhc_5IWhQDRpW3ojZiLr4tAdURzoYrYbk4rNdl7FY4Zwc?e=xo273a&nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJTdHJlYW1XZWJBcHAiLCJyZWZlcnJhbFZpZXciOiJTaGFyZURpYWxvZy1MaW5rIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXcifX0%3D) |
| Documentação complementar | [`docs/`](docs/) |

---

## 27. Cronograma Técnico

Estado funcional atualizado em 30/10/2026, conforme confirmação de Felipe Roberto sobre a evolução do aplicativo. As referências da implementação estão nas seções 6 e 11.3.

| Atividade | Responsável | Status |
|---|---|---|
| Configuração Unity, AR Foundation e cena AR | Desenvolvimento | ✅ Implementado. |
| Primeiro build e demonstração do cubo | Desenvolvimento / Validação | ✅ Etapa histórica registrada nas seções 22–25. |
| Modelo da placa e destaques dos pontos | Desenvolvimento | ✅ Implementado: modelo VCU, pontos de coleta de dados e regiões de observação. |
| Guias RF01–RF04 | Desenvolvimento | ✅ Implementado: orientações, exemplos e destaques associados às quatro etapas. |
| Navegação, checklist e medições RF05–RF07 | Desenvolvimento | ✅ Implementado: navegação entre etapas, marcação dos itens e entrada de valores. |
| Resultado e relatório JSON | Desenvolvimento | ✅ Implementado em InspectionFlow, ResultPanel e MetricsRecorder. |
| Documentação do README | Desenvolvimento / Documentação | Atualizada no escopo do DEV01; consolidação das demais seções acompanha os respectivos cards DEV. |

---

## 28. Controle de Versão

Seguimos o padrão de mensagens abaixo nos commits:

```text
feat: adiciona reconhecimento de imagem
fix: corrige posicionamento do objeto virtual
docs: atualiza instruções de execução
test: registra teste em dispositivo Android
```

Cada funcionalidade é registrada em commits separados, para que o histórico mostre a evolução do projeto.

---

## 29. Problemas Encontrados e Soluções

### Problema 1 — Build falha por API mínima do Android

**Problema:** `BuildFailedException: ARCore Required apps using Vulkan require a minimum SDK version of AndroidApiLevel29. Currently set to AndroidApiLevel25`.  
**Causa:** o ARCore com Vulkan exige Android 10 (API 29), e o projeto estava configurado com API 25.  
**Solução:** em **Player Settings → Android → Other Settings → Minimum API Level**, selecionamos **Android 10.0 (API level 29)**.

### Problema 2 — Aviso "Unsupported Input Handling on Android"

**Problema:** o Unity avisa que `Active Input Handling = Both` não é suportado no Android.  
**Causa:** o `TapToToggleScale` usa o Input Manager antigo, enquanto a câmera do XR Origin usa o Input System novo.  
**Solução:** mantivemos **Both** e escolhemos **Ignore** no aviso. Para removê-lo, será preciso migrar o `TapToToggleScale` para o Input System.

### Problema 3 — "No Android devices connected"

**Problema:** o Unity não encontrava o celular, mesmo conectado via USB.  
**Causa:** a Depuração USB não estava ativada no aparelho.  
**Solução:** ativamos as Opções do desenvolvedor (tocando 7 vezes em **Número de compilação**), ativamos a **Depuração USB** e autorizamos o computador.

### Problema 4 — Reference Image Library criada incorretamente

**Problema:** inicialmente foi criada uma pasta com o nome da library, em vez do asset.  
**Causa:** a Reference Image Library é um asset do Unity, não uma pasta.  
**Solução:** criamos a library via **Create → XR → Reference Image Library**, adicionamos a imagem e definimos o tamanho físico.

### Problema 5 — Falha ao instalar o módulo Visual Studio pelo Unity Hub

**Problema:** o Unity Hub retornava `INSTALL_ERROR` ao instalar o Visual Studio Community.  
**Causa:** o Visual Studio já estava instalado na máquina.  
**Solução:** ignoramos o módulo e selecionamos o Visual Studio existente em **Edit → Preferences → External Tools**.

---

## 30. Melhorias Futuras

- Avaliar a ampliação para outras placas e novos roteiros de inspeção, aproveitando a estrutura de etapas existente.
- Aprimorar a apresentação das orientações, exemplos e destaques a partir do feedback de uso.
- Avaliar novos indicadores e formas de organização dos relatórios de sessão conforme o protocolo de pesquisa.

Essas propostas são evoluções adicionais. A versão atual já contempla o modelo da placa, os pontos de observação e coleta de dados, os exemplos, a navegação, os checklists e a entrada de medições descritos na seção 6.

---

## 31. Custos e Recursos

| Recurso | Tipo | Custo/Condição |
|---|---|---|
| Unity | Software | Licença Personal (gratuita) |
| Blender | Software | Gratuito |
| Smartphone | Hardware | Equipamento da equipe |
| Computador | Hardware | Equipamento da equipe |

---

## 32. Licença

Projeto acadêmico desenvolvido para fins educacionais na disciplina de UPX — Realidade Aumentada.

---

## 33. Autores

- **Felipe Roberto** — desenvolvimento e planejamento
- **Gabriel Verlangieri** — desenvolvimento
- **Luis Felipe** — desenvolvimento
- **Felipe Veronezzi** — documentação e planejamento
- **Pedro Henrique** — documentação e planejamento
- **Matheus Rodrigues** — documentação
- **André Vinicius** — testes
- **Gabriel Queiroz** — testes

**Instituição:** Centro Universitário FACENS  
**Disciplina:** UPX — Realidade Aumentada  
**Semestre:** 2026.2

---

## Checklist de Entrega

Este checklist acompanha a entrega documental da versão atual. O estado funcional da seção 6 considera a confirmação de Felipe Roberto em 30/10/2026; o acompanhamento dos cards permanece nos registros de planejamento.

- [x] Inventário RF01–RF07 ligado às fontes do repositório (seção 6).
- [x] Funcionalidades RF01–RF07 registradas como implementadas conforme confirmação de Felipe Roberto em 30/10/2026.
- [x] Elementos virtuais, limitações e evolução atualizados com referências (seções 11.3, 24, 27 e 30).
- [ ] Harmonizar o README com DEV02–DEV05 e DEV07 antes do aceite do DEV01.
- [ ] Atualizar e conferir arquitetura, estrutura e scripts da versão atual (DEV03).
- [ ] Conciliar versões, configuração e procedimentos de instalação/execução (DEV06).
- [ ] Associar screenshots, vídeo e testes à revisão, dispositivo e versão Android correspondentes (DEV07).
- [ ] Atualizar os registros visuais e a identificação da versão nas evidências de funcionamento (DEV07).
- [x] Modelo da placa, pontos de coleta e observação, exemplos, checklists e entrada de dados descritos como recursos atuais.
- [ ] Revisar identificação e coerência acadêmica do README (DEV08/DEV09 e dependências DOC).

Os registros antigos de testes e evidências permanecem como histórico; precisam de contextualização na consolidação do README. DOC01–DOC18 continuam pendentes, e a revisão anterior do DOCX não conta como progresso desses cards.
