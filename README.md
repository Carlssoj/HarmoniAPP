# HarmoniAPP

Aplicativo mobile do ecossistema Harmoni, construído com `.NET MAUI` e integrado à `HarmoniAPI`.

## Versão atual

- `v0.2.1`
- Tag de release: `v0.2.1`

## Fase 2 entregue

Esta fase prioriza a experiência do `HarmoniAPP` como aplicativo mobile integrado à `HarmoniAPI`, sem alterar a aplicação web existente.

O app agora possui:

- fluxo real de sessão com:
  - tela de abertura
  - login com JWT
  - troca automática entre estado autenticado e não autenticado
- navegação mobile persistente com abas para:
  - `Resumo`
  - `Pipeline`
  - `Agenda`
  - `Interações`
  - `Perfil`
- dashboard resumido consumindo a API real
- pipeline unificado para:
  - leads
  - clientes
  - oportunidades
- agenda com listagem de tarefas e cadastro de nova tarefa
- interações com histórico e cadastro de nova interação
- perfil do usuário autenticado com contexto de acesso
- identidade visual própria do Harmoni no app:
  - paleta terrosa suave
  - app icon e splash personalizados
  - interface mobile pensada para uso diário da operação comercial

## Estrutura

- `src/HarmoniAPP.Mobile`: app MAUI, navegação, páginas e assets.
- `src/HarmoniAPP.Core`: modelos e clientes HTTP consumindo a API.

## Como executar

1. Suba a API primeiro em `https://localhost:7174`.
2. No Windows, execute:

```powershell
cd "C:\Users\carls\OneDrive\Documentos\GitHub Projetos\HarmoniAPP"
dotnet restore .\HarmoniAPP.slnx
dotnet build .\src\HarmoniAPP.Mobile\HarmoniAPP.Mobile.csproj -f net10.0-windows10.0.19041.0
dotnet run --project .\src\HarmoniAPP.Mobile\HarmoniAPP.Mobile.csproj -f net10.0-windows10.0.19041.0
```

## Base da API

Hoje o app usa por padrão:

- Windows: `https://localhost:7174/`
- iOS Simulator: `https://localhost:7174/`
- Android Emulator: `https://10.0.2.2:7174/`

Para dispositivos fora do ambiente local, a API precisa estar acessível em um host `HTTPS` alcançável pela rede. Em um próximo passo, podemos externalizar isso para configuração dinâmica no próprio app.

## Validação realizada

- `dotnet build .\src\HarmoniAPP.Mobile\HarmoniAPP.Mobile.csproj -f net10.0-windows10.0.19041.0`
  - sucesso, sem erros e sem avisos
- target `iOS`
  - o projeto segue disponível para evolução futura, sem foco específico em um único aparelho
  - a validação local ainda ficou bloqueada por lock de diretórios intermediários `obj` dentro do ambiente atual

## Arquitetura mobile atual

- `AuthApiClient`: login, sessão e usuário autenticado
- `DashboardApiClient`: resumo do cockpit
- `LeadsApiClient`
- `CustomersApiClient`
- `OpportunitiesApiClient`
- `TasksApiClient`
- `InteractionsApiClient`
- `AppNavigator`: coordenação do fluxo de login/app autenticado

## Próximos passos recomendados

- adicionar busca e filtros nas listas
- permitir configuração dinâmica da URL da API
- adicionar edição e conclusão de tarefas
- adicionar criação de leads, clientes e oportunidades pelo app
- retomar a validação iOS quando a trilha mobile pedir isso novamente
- preparar distribuição `TestFlight`
