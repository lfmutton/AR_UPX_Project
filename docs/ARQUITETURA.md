# Arquitetura — WL01 Unity

```text
Dispositivo
   ↓
Camera
   ↓
ARCore / ARKit
   ↓
AR Foundation
   ↓
ARTrackedImageManager
   ↓
Reference Image Library
   ↓
ARTrackedImage
   ↓
Conteudo 3D + Interacao
   ↓
Aplicacao do aluno
```

## Responsabilidades

- `ARTrackedImageManager`: detectar e acompanhar imagens conhecidas;
- `ImageRecognitionController`: reagir ao tracking e criar/atualizar conteudo;
- `TapToToggleScale`: exemplo minimo de interacao;
- `Reference Image Library`: manter os targets de reconhecimento.
