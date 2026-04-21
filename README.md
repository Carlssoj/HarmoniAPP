# HarmoniAPP

Aplicativo mobile inicial do ecossistema Harmoni, construído com `.NET MAUI` para consumir a `HarmoniAPI`.

## Versão atual

- `v0.1.0`
- Tag de release: `v0.1.0`

## O que já está pronto

- Estrutura inicial em `.NET MAUI`.
- Camada `Core` com:
  - cliente de autenticação
  - cliente do dashboard
  - armazenamento seguro do token
  - modelos básicos de resposta
- Fluxo inicial do MVP:
  - login
  - leitura do usuário autenticado
  - dashboard resumido
  - navegação base para Leads, Clientes, Oportunidades, Tarefas, Interações e Perfil

## Estrutura

- `src/HarmoniAPP.Mobile`: app MAUI e telas.
- `src/HarmoniAPP.Core`: clientes HTTP, modelos e contratos internos do app.

## Como executar

1. Suba a API primeiro em `https://localhost:7174`.
2. Rode o app MAUI para Windows:

```powershell
cd "C:\Users\carls\OneDrive\Documentos\GitHub Projetos\HarmoniAPP"
dotnet restore .\HarmoniAPP.slnx
dotnet build .\src\HarmoniAPP.Mobile\HarmoniAPP.Mobile.csproj -f net10.0-windows10.0.19041.0
dotnet run --project .\src\HarmoniAPP.Mobile\HarmoniAPP.Mobile.csproj -f net10.0-windows10.0.19041.0
```

## Observações importantes

- A base URL padrão está configurada para:
  - Windows: `https://localhost:7174/`
  - Android Emulator: `https://10.0.2.2:7174/`
- O build para Windows foi validado com sucesso.
- O target Android ainda depende da instalação/configuração local do Android SDK nesta máquina.

## Próximos passos recomendados

- conectar as telas de módulos às listagens reais da API
- adicionar refresh token e renovação automática de sessão
- implementar cadastros completos de tarefas e interações no app
- preparar distribuição Android e iOS
