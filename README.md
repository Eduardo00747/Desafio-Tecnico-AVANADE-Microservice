# Desafio-Tecnico-AVANADE-Microservice

Este repositório contém a solução para o Desafio Técnico da AVANADE, implementada com uma arquitetura de microsserviços usando C#/.NET Core e orquestrada com Docker Compose. O projeto inclui serviços para Administração, Cliente e um Gateway de API, todos comunicando-se via RabbitMQ.

## 🚀 Como Iniciar o Projeto

Para rodar o projeto localmente, você precisará ter o **Docker** e o **Docker Compose** instalados em sua máquina.

### Pré-requisitos

1.  **Docker Desktop** (inclui Docker Engine e Docker Compose):
    * [Instalar Docker](https://docs.docker.com/get-docker/)

### Passo a Passo Geral (Subir Todos os Containers)

Siga estas etapas para clonar o repositório e iniciar todos os microsserviços e o RabbitMQ em um único comando:

1.  **Clonar o Repositório:**

    ```bash
    git clone [https://github.com/Eduardo00747/Desafio-Tecnico-AVANADE-Microservice.git](https://github.com/Eduardo00747/Desafio-Tecnico-AVANADE-Microservice.git)
    ```

2.  **Navegar para a Pasta Raiz do Projeto:**

    ```bash
    cd Desafio-Tecnico-AVANADE-Microservice
    ```

3.  **Iniciar Todos os Microsserviços com Docker Compose:**

    Este comando irá construir as imagens (caso necessário) e iniciar todos os containers em modo *detached* (segundo plano).

    ```bash
    docker-compose up -d --build
    ```

4.  **Verificar os Containers:**

    Para garantir que todos os serviços estão rodando:

    ```bash
    docker ps
    ```

5.  **Encerrar os Containers:**

    Para parar e remover todos os containers, redes e volumes criados pelo `docker-compose`:

    ```bash
    docker-compose down
    ```

---

## 🛠️ Detalhamento dos Microsserviços e Acessos

O comando `docker-compose up -d --build` inicia todos os componentes listados abaixo, garantindo que eles se comuniquem corretamente dentro da rede Docker.

### 1. Gateway (ECommerce.Gateway)

O Gateway é o ponto de entrada principal para a aplicação, responsável por rotear as requisições para os microsserviços corretos.

| Informação | Detalhes |
| :--- | :--- |
| **Porta de Acesso** | `http://localhost:5000` |

**Passo a Passo de Acesso:**

1.  Certifique-se de que o Docker Compose está rodando (`docker-compose up -d`).
2.  Acesse a documentação do Swagger ou o endpoint principal do Gateway através do endereço:
    ```bash
    # Exemplo de acesso para testar
    curl http://localhost:5000/
    ```

### 2. Admin (ECommerce.Microservices.Admin)

O serviço Admin é responsável pelas funcionalidades administrativas da plataforma.

| Informação | Detalhes |
| :--- | :--- |
| **Porta de Acesso (Exposição)** | `http://localhost:5001` (Acesso direto, geralmente usado em desenvolvimento) |
| **Acesso em Produção** | Via **Gateway** |

**Passo a Passo de Acesso:**

1.  O serviço é iniciado automaticamente pelo Docker Compose.
2.  Para interagir com este serviço, você deve usar os endpoints definidos no **Gateway**.
3.  *Opcional (apenas para debug):* Acesse o Swagger do Admin em `http://localhost:5001/swagger/index.html`.

### 3. Cliente (ECommerce.Microservices.Cliente)

O serviço Cliente é responsável pelas funcionalidades voltadas ao usuário final.

| Informação | Detalhes |
| :--- | :--- |
| **Porta de Acesso (Exposição)** | `http://localhost:5002` (Acesso direto, geralmente usado em desenvolvimento) |
| **Acesso em Produção** | Via **Gateway** |

**Passo a Passo de Acesso:**

1.  O serviço é iniciado automaticamente pelo Docker Compose.
2.  Para interagir com este serviço, utilize os endpoints definidos no **Gateway**.
3.  *Opcional (apenas para debug):* Acesse o Swagger do Cliente em `http://localhost:5002/swagger/index.html`.

### 4. RabbitMQ (Message Broker)

O RabbitMQ é o *Message Broker* central que permite a comunicação assíncrona entre os microsserviços.

| Informação | Detalhes |
| :--- | :--- |
| **Porta de Gerenciamento** | `http://localhost:15672` |
| **Porta Padrão AMQP** | `5672` |
| **Usuário/Senha Padrão** | `guest`/`guest` |

**Passo a Passo de Acesso:**

1.  O RabbitMQ é iniciado automaticamente pelo Docker Compose.
2.  Abra seu navegador e acesse a interface de gerenciamento:
    ```
    http://localhost:15672
    ```
3.  Utilize as credenciais padrão: **Usuário:** `guest`, **Senha:** `guest` para visualizar as *queues* (filas) e o tráfego de mensagens entre os serviços.

---

## 🛑 Comandos de Gerenciamento do Docker

| Ação | Comando | Descrição |
| :--- | :--- | :--- |
| **Subir (Build e Run)** | `docker-compose up -d --build` | Constrói imagens e inicia todos os serviços em segundo plano. |
| **Parar e Remover** | `docker-compose down` | Para e remove containers, redes e volumes criados. |
| **Verificar Logs** | `docker-compose logs -f` | Exibe os logs de todos os containers em tempo real. |
| **Verificar Status** | `docker ps` | Lista os containers em execução. |
| **Apenas Iniciar (Sem Build)** | `docker-compose up -d` | Inicia containers a partir de imagens já existentes. |
