# Gerenciamento de Tarefas API

## Visão Geral

Este projeto é uma API RESTful para gerenciamento de tarefas, permitindo que usuários organizem e monitorem suas tarefas diárias, além de promover a colaboração entre colegas de equipe. O sistema oferece funcionalidades para a criação e gerenciamento de projetos e tarefas associadas, permitindo listagens, adições, atualizações e remoções.

## Estrutura do Projeto

### Entidades

- **Usuário**: Pessoa que utiliza o aplicativo. *Nota: Não há um CRUD dedicado para usuários; a criação ocorre automaticamente ao criar um projeto caso o usuário não exista.*
- **Projeto**: Contém várias tarefas. Um usuário pode criar, visualizar e gerenciar vários projetos.
- **Tarefa**: Unidade de trabalho dentro de um projeto, contendo título, descrição, data de vencimento e status.
- **Comentário**: Anotações relacionadas às tarefas que podem ser adicionadas pelos usuários.

## Funcionalidades da API

### Rotas Disponíveis

#### Projects

- **GET** `/api/Projects/{id}`: Obtém um projeto pelo ID.
- **PUT** `/api/Projects/{id}`: Atualiza um projeto existente.
- **DELETE** `/api/Projects/{id}`: Exclui um projeto.
- **GET** `/api/Projects/user/{userId}`: Obtém todos os projetos de um usuário.
- **GET** `/api/Projects/{projectId}/tasks`: Obtém todas as tarefas associadas a um projeto.
- **POST** `/api/Projects`: Cria um novo projeto e realiza a **criação de um novo usuário**, caso ele não exista.

#### Tasks

- **GET** `/api/Tasks/{id}`: Obtém uma tarefa pelo ID.
- **PUT** `/api/Tasks/{id}`: Atualiza uma tarefa existente.
- **DELETE** `/api/Tasks/{id}`: Exclui uma tarefa.
- **POST** `/api/Tasks`: Cria uma nova tarefa.

#### Comments

- **POST** `/api/Comments`: Cria um novo comentário.

#### Reports

- **GET** `/api/reports/users/{userId}/tasks/{days}`: Relatório de tarefas concluídas por um usuário.
- **GET** `/api/reports/top/completed/comments/{days}`: Relatório das top 10 tarefas com mais comentários.
- **GET** `/api/reports/top/projects/{days}`: Relatório dos top 10 projetos com mais tarefas concluídas.

## Como Utilizar os Relatórios

Os relatórios podem ser gerados por usuários com a função de "gerente". Para solicitar um relatório, o usuário deve utilizar um dos endpoints de relatório mencionados acima. Abaixo estão instruções detalhadas sobre como usar cada endpoint de relatório:

1. **Relatório de Tarefas Concluídas por Usuário**

   - **Endpoint**: 
     ```
     GET /api/reports/users/{userId}/tasks/{days}
     ```
   - **Descrição**: Gera um relatório das tarefas que foram concluídas por um usuário específico.
   - **Parâmetros**:
     - `userId`: O ID do usuário cujas tarefas concluídas você deseja ver.
     - `days`: O número de dias a considerar no relatório. Os resultados devem ser limitados a no máximo 30 dias.
   - **Acesso**: O acesso a este relatório requer um `userRequestId` (aquele que faz a solicitação). Caso o `userRequestId` não tenha permissão (ou seja, não seja um gerente), a API retornará um erro de autorização.

2. **Relatório das Top 10 Tarefas com Mais Comentários**

   - **Endpoint**: 
     ```
     GET /api/reports/top/completed/comments/{days}
     ```
   - **Descrição**: Gera um relatório das 10 tarefas que têm mais comentários em um determinado período.
   - **Parâmetros**:
     - `days`: O número de dias a considerar no relatório. Deve ser no máximo 30.
   - **Acesso**: Assim como o relatório anterior, o acesso a este relatório requer um `userRequestId` com as permissões adequadas.

3. **Relatório dos Top 10 Projetos com Mais Tarefas Concluídas**

   - **Endpoint**: 
     ```
     GET /api/reports/top/projects/{days}
     ```
   - **Descrição**: Gera um relatório dos 10 projetos que têm mais tarefas concluídas em um determinado período.
   - **Parâmetros**:
     - `days`: O número de dias a considerar no relatório. Deve ser no máximo 30.
   - **Acesso**: Também requer que o `userRequestId` tenha as permissões adequadas para acessar este relatório.

### Exemplo de Uso

Para solicitar um relatório de tarefas concluídas por um usuário, você pode usar a seguinte chamada cURL:

```bash
curl -X GET "http://localhost:5000/api/reports/users/{userId}/tasks/30?userRequestId={userRequestId}"
```

Neste exemplo, substitua `{userId}` pelo ID do usuário cujas tarefas você deseja ver, e `{userRequestId}` pelo ID do usuário que está solicitando o relatório.

## Regras de Negócio

1. **Prioridades de Tarefas**: Cada tarefa deve ter uma prioridade atribuída desde a criação.
2. **Restrições de Remoção de Projetos**: Um projeto não pode ser removido se houver tarefas pendentes.
3. **Histórico de Atualizações**: Cada atualização de uma tarefa deve ser registrada com informações sobre a modificação, persistindo o histórico no MongoDB.
4. **Limite de Tarefas por Projeto**: Cada projeto possui um limite máximo de 20 tarefas.
5. **Relatórios de Desempenho**: Endpoints para gerar relatórios com acesso restrito a usuários com função de "gerente".
6. **Comentários nas Tarefas**: Usuários podem adicionar comentários, que são registrados no histórico de alterações.

## Configuração do Ambiente

### Pré-requisitos

- [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker e Docker Compose (opcional, para execução em container)

## Execução Local

1. Clone o repositório:
   ```bash
   git clone https://github.com/fernandosmace/TasksManagement.git
   cd TasksManagement
   ```

### Formas de Executar a Aplicação

#### 1. Via Docker Compose
Na própria pasta do projeto, execute:
```bash
docker-compose up
```
Isso iniciará automaticamente os containers SQL Server, MongoDB e a aplicação. A API estará acessível na porta `5000`.

#### 2. Via Linha de Comando
1. Edite o arquivo `appsettings.json` para inserir as credenciais dos bancos de dados.
2. Restaure as dependências:
   ```bash
   dotnet restore
   ```

3. Para executar a API:
   ```bash
   dotnet run
   ```

#### 3. Via Visual Studio
1. Edite as variáveis de ambiente no perfil HTTP no `launchSettings.json` do projeto da API para inserir as credenciais dos bancos de dados.
2. Execute o projeto pelo Visual Studio, utilizando o botão "Start" no IDE.

> *Observação: A aplicação aplica automaticamente as migrações do Entity Framework durante a inicialização, portanto, a execução do comando `dotnet ef database update` não é necessária.*

## Documentação da API

### Endpoints

#### Projects
- **GET** `/api/Projects/{id}`: Obtém um projeto pelo ID.
- **PUT** `/api/Projects/{id}`: Atualiza um projeto existente.
- **DELETE** `/api/Projects/{id}`: Exclui um projeto.
- **GET** `/api/Projects/user/{userId}`: Obtém todos os projetos de um usuário.
- **GET** `/api/Projects/{projectId}/tasks`: Obtém todas as tarefas associadas a um projeto.
- **POST** `/api/Projects`: Cria um novo projeto e, se necessário, um novo usuário.

#### Tasks
- **GET** `/api/Tasks/{id}`: Obtém uma tarefa pelo ID.
- **PUT** `/api/Tasks/{id}`: Atualiza uma tarefa existente.
- **DELETE** `/api/Tasks/{id}`: Exclui uma tarefa.
- **POST** `/api/Tasks`: Cria uma nova tarefa.

#### Comments
- **POST** `/api/Comments`: Cria um novo comentário.

#### Reports
- **GET** `/api/reports/users/{userId}/tasks/{days}`: Relatório de tarefas concluídas por usuário.
- **GET** `/api/reports/top/completed/comments/{days}`: Relatório das top 10 tarefas com mais comentários.
- **GET** `/api/reports/top/projects/{days}`: Relatório dos top 10 projetos com mais tarefas concluídas.

## Testes
Para executar os testes automatizados:
```bash
dotnet test
```

## Fase 2: Refinamento

### Perguntas para o PO
1. Qual o volume de usuários é esperado para a aplicação? Importante para planejamento de expectativas de performance, escalabilidade e observabilidade.
2. A quantidade limite de tarefas por projeto tende a mudar? Poderíamos implementar uma parametrização externa de forma a não depender de alterações na aplicação.
3. Existem planos para a integração com ferramentas de gestão de projetos já existentes?
4. Quais níveis de acesso serão necessários além de Admin? Definir as permissões pode influenciar na estrutura de dados e nas validações.
5. Existem novos campos previstos à serem adicionados nas entidades já existentes?

## Fase 3: Melhorias Futuras
1. **Validações mais robustas**: Implementar validações mais complexas e feedback adequado ao usuário nas operações.
2. **Desempenho e Escalabilidade**: A considerar conforme o volume de usuários esperado para a aplicação, realizar a aplicação de estratégias de cache.
3. **Design da API**: Utilizar padrões de design como GraphQL para otimizar a busca de dados.
4. **Monitoramento e Logs**: Implementar uma solução de monitoramento para rastrear o desempenho da aplicação e identificar gargalos, bem como registrar logs de erros para melhorar a manutenção e a análise de incidentes.
