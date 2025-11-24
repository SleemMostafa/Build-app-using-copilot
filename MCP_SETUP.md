# Model Context Protocol (MCP) Setup Guide

## Overview

This Coffee Restaurant API project is now **MCP-enabled**, allowing AI agents (like GitHub Copilot) to interact with your database, API endpoints, Docker containers, and file system in real-time.

## 🎯 What You Get

### 4 MCP Servers Configured:

1. **Filesystem Server** - Read/write project files
2. **Database Server** - Query SQL Server database
3. **API Tester Server** - Test API endpoints automatically
4. **Docker Server** - Manage containers and view logs

## 📋 Prerequisites

- **Node.js 18+** installed
- **SQL Server** running (local or Docker)
- **Coffee Restaurant API** running on `https://localhost:7001`
- **VS Code** with **GitHub Copilot Chat** extension

## 🚀 Quick Start

### Step 1: Install Dependencies

```powershell
cd .mcp
npm install
```

This installs:
- `@modelcontextprotocol/sdk` - MCP SDK
- `mssql` - SQL Server driver
- `axios` - HTTP client

### Step 2: Configure Database Credentials

Edit `.mcp\mcp-config.json` and update the database environment variables:

```json
{
  "database": {
    "env": {
      "DB_HOST": "localhost",
      "DB_NAME": "CoffeeRestaurantDb",
      "DB_USER": "sa",
      "DB_PASSWORD": "YourStrong@Passw0rd"  // ⚠️ Change this!
    }
  }
}
```

### Step 3: Reload VS Code

1. Press `Ctrl+Shift+P`
2. Type "Reload Window"
3. Press Enter

VS Code will automatically detect the `.vscode/settings.json` MCP configuration.

### Step 4: Verify MCP is Active

In **GitHub Copilot Chat**, you should see MCP tools available. Try asking:

```
@workspace How many coffee items are in the database?
```

Copilot will automatically use the `database` MCP server to query your database!

## 🔧 Configuration Files

### `.vscode/settings.json`

Tells VS Code which MCP servers to load:

```json
{
  "github.copilot.chat.mcp.enabled": true,
  "github.copilot.chat.mcp.servers": {
    "filesystem": { ... },
    "database": { ... },
    "api-tester": { ... },
    "docker": { ... }
  }
}
```

### `.mcp/mcp-config.json`

Centralized MCP server configuration (for CLI tools and other editors).

## 📦 MCP Server Details

### 1. Filesystem Server

**Built-in official MCP server** for file operations.

**What it does:**
- Read any file in the project
- Write/edit files
- List directories
- Search file contents

**Example prompts:**
- "Show me the `CustomerConfiguration.cs` file"
- "Count how many controllers exist"
- "Find all files that mention 'CoffeeItem'"

### 2. Database Server (Custom)

**Custom-built SQL Server MCP server** with 6 specialized tools.

**Available Tools:**

| Tool | Description |
|------|-------------|
| `query_coffee_items` | Get all coffee items with categories |
| `query_orders` | Get orders with customer/barista details |
| `query_customers` | Get customer info and order counts |
| `query_categories` | Get all categories with item counts |
| `execute_custom_query` | Run custom SELECT queries (read-only) |
| `get_database_stats` | Get database statistics (orders, revenue, etc.) |

**Example prompts:**
- "How many pending orders are there?"
- "Show me all Espresso category items"
- "What's the total revenue from completed orders?"
- "List customers who spent more than $50"
- "Execute: SELECT TOP 5 * FROM Orders ORDER BY OrderDate DESC"

**Security:**
- Read-only operations (no INSERT/UPDATE/DELETE)
- SQL injection protection
- Connection pooling enabled

### 3. API Tester Server (Custom)

**Custom-built API testing MCP server** with 9 tools.

**Available Tools:**

| Tool | Description |
|------|-------------|
| `api_login` | Login and store JWT token |
| `api_register` | Register new user |
| `api_get_coffee_items` | GET /api/coffeeitems |
| `api_create_coffee_item` | POST /api/coffeeitems (Admin only) |
| `api_get_orders` | GET /api/orders |
| `api_create_order` | POST /api/orders |
| `api_health_check` | Check if API is running |
| `api_custom_request` | Make custom HTTP requests |

**Example prompts:**
- "Login to the API as admin"
- "Test the coffee items endpoint"
- "Create a test order for customer XYZ"
- "Is the API running?"
- "Make a GET request to /api/coffeeitems"

**Features:**
- Automatic JWT token management
- Supports all HTTP methods
- Handles HTTPS with self-signed certificates
- Error details included in responses

### 4. Docker Server

**Built-in official MCP server** for Docker management.

**What it does:**
- List containers
- Inspect containers
- Start/stop/restart containers
- View container logs
- List images and networks

**Example prompts:**
- "Show running Docker containers"
- "What's the status of the coffee-restaurant-api container?"
- "Show me the logs from the database container"
- "Restart the API container"

## 🎯 Real-World Usage Examples

### Scenario 1: Debugging an Order Issue

**You:** "Why is order 12345 stuck in Pending status?"

**Copilot:**
1. Uses `database` server to query the order
2. Checks if BaristaId is assigned
3. Verifies coffee items in the order exist
4. Suggests next steps

### Scenario 2: Performance Analysis

**You:** "How many orders were created today?"

**Copilot:**
1. Uses `database` server with `execute_custom_query`
2. Runs: `SELECT COUNT(*) FROM Orders WHERE CAST(OrderDate AS DATE) = CAST(GETDATE() AS DATE)`
3. Returns the count

### Scenario 3: API Testing

**You:** "Test creating a new coffee item"

**Copilot:**
1. Uses `api_login` to authenticate
2. Uses `query_categories` to find a category ID
3. Uses `api_create_coffee_item` to create the item
4. Uses `api_get_coffee_items` to verify creation
5. Reports success/failure

### Scenario 4: Container Management

**You:** "The API isn't responding. Is the container running?"

**Copilot:**
1. Uses Docker server to list containers
2. Checks if coffee-restaurant-api is running
3. If stopped, suggests restarting
4. Shows recent logs for troubleshooting

## 🔐 Security Notes

### Database Server
- **Read-only**: Only SELECT queries allowed
- **No modifications**: INSERT/UPDATE/DELETE blocked
- **Connection limits**: Pooled connections (max 10)

### API Tester Server
- **Token storage**: JWT stored in memory (cleared on restart)
- **HTTPS**: Self-signed certificates allowed for local development
- **Timeout**: 30-second timeout prevents hanging

### Credentials
⚠️ **IMPORTANT**: Never commit real passwords to Git!

Update `.gitignore` to exclude sensitive configs:

```
.mcp/mcp-config.json
.vscode/settings.json
```

Use environment variables for production:

```json
{
  "env": {
    "DB_PASSWORD": "${DB_PASSWORD}"
  }
}
```

## 🧪 Testing Your Setup

### Test 1: Database Connection

In Copilot Chat:
```
@workspace Query all coffee categories from the database
```

Expected: List of categories (Espresso, Cappuccino, etc.)

### Test 2: API Connection

In Copilot Chat:
```
@workspace Login to the API and get all coffee items
```

Expected: JWT token stored + list of coffee items

### Test 3: Docker

In Copilot Chat:
```
@workspace Show me all Docker containers
```

Expected: List of running containers

### Test 4: Filesystem

In Copilot Chat:
```
@workspace How many entities are in the Domain layer?
```

Expected: Count of .cs files in `CoffeeRestaurant.Domain/Entities/`

## 🐛 Troubleshooting

### "MCP server failed to start"

**Solution:**
1. Check Node.js is installed: `node --version` (should be 18+)
2. Install dependencies: `cd .mcp && npm install`
3. Reload VS Code

### "Database connection failed"

**Solution:**
1. Verify SQL Server is running
2. Check credentials in `.mcp/mcp-config.json`
3. Test connection manually:
   ```powershell
   sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"
   ```

### "API tester can't connect"

**Solution:**
1. Verify API is running: `https://localhost:7001/swagger`
2. Check URL in `.mcp/mcp-config.json`
3. Ensure HTTPS certificate is trusted (or use HTTP endpoint)

### "Copilot doesn't use MCP tools"

**Solution:**
1. Ensure MCP is enabled: Check `.vscode/settings.json` exists
2. Reload VS Code window
3. Try explicit requests: "Use the database MCP server to query orders"

## 📚 Advanced Usage

### Custom SQL Queries

```
@workspace Execute this SQL query: 
SELECT c.Name, COUNT(ci.Id) as Items 
FROM Categories c 
LEFT JOIN CoffeeItems ci ON c.Id = ci.CategoryId 
GROUP BY c.Name
```

### API Workflow Automation

```
@workspace 
1. Login as admin
2. Create a new coffee item called "Mocha" in Latte category
3. Verify it was created
4. Report the new item's ID
```

### Container Monitoring

```
@workspace 
Check if the database container is running and show the last 50 log lines
```

### Multi-Server Queries

```
@workspace 
Compare the number of coffee items in the database vs what the API returns
```

## 🎓 Learning Resources

- **MCP Documentation**: https://modelcontextprotocol.io
- **MCP SDK**: https://github.com/modelcontextprotocol/typescript-sdk
- **Example Servers**: https://github.com/modelcontextprotocol/servers

## 🚀 Next Steps

1. **Customize servers**: Edit `.mcp/servers/database-server.js` to add more queries
2. **Add monitoring**: Create an MCP server for application logs
3. **Integrate CI/CD**: Use MCP in GitHub Actions for automated testing
4. **Build dashboards**: Create an MCP server that generates real-time reports

## 💡 Tips

- **Be specific**: "Query orders from database" is better than "Show orders"
- **Chain operations**: "Login, then create coffee item, then verify" works great
- **Use context**: "@workspace" helps Copilot understand your project
- **Iterate**: If first attempt fails, refine your prompt with more details

## ✅ Checklist

- [ ] Node.js 18+ installed
- [ ] Dependencies installed (`npm install` in `.mcp/`)
- [ ] Database credentials updated in config
- [ ] API URL updated in config
- [ ] VS Code reloaded
- [ ] Test database query works
- [ ] Test API login works
- [ ] Test Docker commands work
- [ ] Test filesystem access works

---

**You're ready to use MCP!** 🎉

Start chatting with Copilot and watch it query your database, test your API, and manage Docker containers automatically.
