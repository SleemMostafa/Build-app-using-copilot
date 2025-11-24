#!/usr/bin/env node

/**
 * MCP Server for SQL Server Database Access
 * Provides tools to query Coffee Restaurant database
 */

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from '@modelcontextprotocol/sdk/types.js';
import sql from 'mssql';

// Database configuration from environment
const dbConfig = {
  server: process.env.DB_HOST || 'localhost',
  database: process.env.DB_NAME || 'CoffeeRestaurantDb',
  user: process.env.DB_USER || 'sa',
  password: process.env.DB_PASSWORD || 'YourStrong@Passw0rd',
  options: {
    encrypt: true,
    trustServerCertificate: true,
    enableArithAbort: true,
  },
  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 30000,
  },
};

let pool = null;

// Initialize database connection
async function initDatabase() {
  if (!pool) {
    pool = await sql.connect(dbConfig);
  }
  return pool;
}

// Create MCP server
const server = new Server(
  {
    name: 'coffee-restaurant-database',
    version: '1.0.0',
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

// Define available tools
server.setRequestHandler(ListToolsRequestSchema, async () => {
  return {
    tools: [
      {
        name: 'query_coffee_items',
        description: 'Query all coffee items with their categories, prices, and availability',
        inputSchema: {
          type: 'object',
          properties: {
            categoryName: {
              type: 'string',
              description: 'Optional: Filter by category name (Espresso, Cappuccino, Latte, etc.)',
            },
            available: {
              type: 'boolean',
              description: 'Optional: Filter by availability status',
            },
          },
        },
      },
      {
        name: 'query_orders',
        description: 'Query orders with customer, barista, and order items details',
        inputSchema: {
          type: 'object',
          properties: {
            status: {
              type: 'string',
              enum: ['Pending', 'InProgress', 'Ready', 'Completed', 'Cancelled'],
              description: 'Optional: Filter by order status',
            },
            customerId: {
              type: 'string',
              description: 'Optional: Filter by customer GUID',
            },
            limit: {
              type: 'number',
              description: 'Optional: Limit number of results (default 50)',
            },
          },
        },
      },
      {
        name: 'query_customers',
        description: 'Query customer information including their orders count',
        inputSchema: {
          type: 'object',
          properties: {
            email: {
              type: 'string',
              description: 'Optional: Filter by customer email',
            },
          },
        },
      },
      {
        name: 'query_categories',
        description: 'Query all coffee categories with item counts',
        inputSchema: {
          type: 'object',
          properties: {},
        },
      },
      {
        name: 'execute_custom_query',
        description: 'Execute a custom SELECT query (read-only, no INSERT/UPDATE/DELETE)',
        inputSchema: {
          type: 'object',
          properties: {
            query: {
              type: 'string',
              description: 'SQL SELECT query to execute',
            },
          },
          required: ['query'],
        },
      },
      {
        name: 'get_database_stats',
        description: 'Get database statistics: total orders, customers, coffee items, revenue',
        inputSchema: {
          type: 'object',
          properties: {},
        },
      },
    ],
  };
});

// Handle tool calls
server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  try {
    await initDatabase();

    switch (name) {
      case 'query_coffee_items': {
        let query = `
          SELECT 
            ci.Id, ci.Name, ci.Description, ci.Price, ci.IsAvailable,
            c.Name as CategoryName, c.Description as CategoryDescription,
            ci.CreatedAt, ci.UpdatedAt
          FROM CoffeeItems ci
          INNER JOIN Categories c ON ci.CategoryId = c.Id
          WHERE 1=1
        `;

        if (args.categoryName) {
          query += ` AND c.Name LIKE '%${args.categoryName}%'`;
        }
        if (args.available !== undefined) {
          query += ` AND ci.IsAvailable = ${args.available ? 1 : 0}`;
        }

        query += ' ORDER BY ci.CreatedAt DESC';

        const result = await pool.request().query(query);
        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(result.recordset, null, 2),
            },
          ],
        };
      }

      case 'query_orders': {
        const limit = args.limit || 50;
        let query = `
          SELECT TOP ${limit}
            o.Id, o.Status, o.TotalPrice, o.OrderDate,
            c.Id as CustomerId, c.Name as CustomerName, c.Email as CustomerEmail,
            b.Id as BaristaId, b.Name as BaristaName
          FROM Orders o
          LEFT JOIN Customers c ON o.CustomerId = c.Id
          LEFT JOIN Baristas b ON o.BaristaId = b.Id
          WHERE 1=1
        `;

        if (args.status) {
          query += ` AND o.Status = '${args.status}'`;
        }
        if (args.customerId) {
          query += ` AND o.CustomerId = '${args.customerId}'`;
        }

        query += ' ORDER BY o.OrderDate DESC';

        const result = await pool.request().query(query);

        // Get order items for each order
        for (const order of result.recordset) {
          const itemsQuery = `
            SELECT 
              oi.Id, oi.Quantity, oi.Price,
              ci.Name as CoffeeItemName
            FROM OrderItems oi
            INNER JOIN CoffeeItems ci ON oi.CoffeeItemId = ci.Id
            WHERE oi.OrderId = '${order.Id}'
          `;
          const items = await pool.request().query(itemsQuery);
          order.Items = items.recordset;
        }

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(result.recordset, null, 2),
            },
          ],
        };
      }

      case 'query_customers': {
        let query = `
          SELECT 
            c.Id, c.Name, c.Email, c.PhoneNumber,
            COUNT(o.Id) as TotalOrders,
            SUM(o.TotalPrice) as TotalSpent
          FROM Customers c
          LEFT JOIN Orders o ON c.Id = o.CustomerId
          WHERE 1=1
        `;

        if (args.email) {
          query += ` AND c.Email LIKE '%${args.email}%'`;
        }

        query += ' GROUP BY c.Id, c.Name, c.Email, c.PhoneNumber';

        const result = await pool.request().query(query);
        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(result.recordset, null, 2),
            },
          ],
        };
      }

      case 'query_categories': {
        const query = `
          SELECT 
            c.Id, c.Name, c.Description,
            COUNT(ci.Id) as ItemCount
          FROM Categories c
          LEFT JOIN CoffeeItems ci ON c.Id = ci.CategoryId
          GROUP BY c.Id, c.Name, c.Description
          ORDER BY c.Name
        `;

        const result = await pool.request().query(query);
        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(result.recordset, null, 2),
            },
          ],
        };
      }

      case 'execute_custom_query': {
        const query = args.query.trim().toUpperCase();

        // Security: Only allow SELECT queries
        if (!query.startsWith('SELECT')) {
          throw new Error('Only SELECT queries are allowed');
        }
        if (query.includes('INSERT') || query.includes('UPDATE') || query.includes('DELETE') || query.includes('DROP')) {
          throw new Error('Only read operations are allowed');
        }

        const result = await pool.request().query(args.query);
        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(result.recordset, null, 2),
            },
          ],
        };
      }

      case 'get_database_stats': {
        const statsQuery = `
          SELECT 
            (SELECT COUNT(*) FROM Orders) as TotalOrders,
            (SELECT COUNT(*) FROM Customers) as TotalCustomers,
            (SELECT COUNT(*) FROM CoffeeItems) as TotalCoffeeItems,
            (SELECT COUNT(*) FROM Categories) as TotalCategories,
            (SELECT SUM(TotalPrice) FROM Orders WHERE Status = 'Completed') as TotalRevenue,
            (SELECT COUNT(*) FROM Orders WHERE Status = 'Pending') as PendingOrders
        `;

        const result = await pool.request().query(statsQuery);
        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(result.recordset[0], null, 2),
            },
          ],
        };
      }

      default:
        throw new Error(`Unknown tool: ${name}`);
    }
  } catch (error) {
    return {
      content: [
        {
          type: 'text',
          text: `Error: ${error.message}`,
        },
      ],
      isError: true,
    };
  }
});

// Start server
async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error('Coffee Restaurant Database MCP server running on stdio');
}

main().catch((error) => {
  console.error('Server error:', error);
  process.exit(1);
});
