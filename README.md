# WL01 — Recognition AR — Unity / AR Foundation

Projeto white label didatico para introduzir **reconhecimento de imagem em Realidade Aumentada** usando Unity e AR Foundation.

## Objetivo didatico

O aluno deve compreender o fluxo:

**camera -> ARCore/ARKit -> AR Foundation -> image tracking -> ARTrackedImage -> conteudo virtual -> interacao**

O exemplo cria um cubo sobre a imagem reconhecida e o mantem como filho do `ARTrackedImage`, preservando o registro espacial. Um toque no cubo alterna sua escala.

## Stack de referencia

- Unity 6.3 LTS
- AR Foundation 6.3.1
- Google ARCore XR Plugin 6.3.1
- Apple ARKit XR Plugin 6.3.1
- C#

## Preparacao inicial no Unity

1. Abra a pasta como projeto Unity 6.3 LTS.
2. Em **Project Settings > XR Plug-in Management**, habilite:
   - ARCore para Android;
   - ARKit para iOS, caso necessario.
3. Crie uma cena vazia.
4. Adicione **AR Session**.
5. Adicione **XR Origin (Mobile AR)**.
6. No objeto `XR Origin`, adicione `AR Tracked Image Manager`.
7. Crie uma **Reference Image Library**.
8. Adicione `Assets/Markers/wl01-marker.png` com largura fisica de aproximadamente **0.12 m**.
9. Associe a Reference Image Library ao `AR Tracked Image Manager`.
10. No mesmo objeto, adicione `ImageRecognitionController`.

## Teste

Compile para um dispositivo fisico compativel. Aponte a camera para o marcador impresso. Um cubo deve surgir sobre a imagem e acompanhar seu movimento.

## Customizacao esperada do aluno

- substituir a imagem de referencia;
- substituir o cubo por prefab/modelo 3D;
- adicionar textos, informacoes e animacoes;
- criar uma interacao relacionada ao projeto;
- manter a logica de reconhecimento e tracking separada do conteudo.

## Observacao didatica

O white label resolve o **core tecnico**. Ele nao deve entregar o projeto especifico da equipe pronto.

Consulte `docs/ATIVIDADE.md`, `docs/ARQUITETURA.md` e `docs/CUSTOMIZACAO.md`.
